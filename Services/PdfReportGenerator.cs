using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MilkAnalyzerTest.Services
{
    public static class PdfReportGenerator
    {
        // Minimal PDF writer: writes a single-page PDF with simple text lines. No external PDF library required.
        public static void GeneratePdfReport(string filePath, int resultId, string customerName, string nic, DateTime date, List<(string ParameterName, double? Value)> values)
        {
            var sb = new StringBuilder();

            // We'll assemble objects and then compute xref.
            var objects = new List<string>();

            // Content stream (text)
            var contentSb = new StringBuilder();
            contentSb.AppendLine("BT");
            contentSb.AppendLine("/F1 12 Tf");
            double y = 800;
            AppendText(contentSb, 50, y, "Milk Test Report");
            y -= 18;
            AppendText(contentSb, 50, y, $"Result ID: {resultId}");
            y -= 14;
            AppendText(contentSb, 50, y, $"Date (UTC): {date:yyyy-MM-dd HH:mm:ss}");
            y -= 14;
            AppendText(contentSb, 50, y, $"Customer: {customerName}");
            y -= 14;
            AppendText(contentSb, 50, y, $"NIC: {nic}");
            y -= 20;
            AppendText(contentSb, 50, y, "");
            y -= 10;
            AppendText(contentSb, 50, y, "Parameter                     Value");
            y -= 12;

            foreach (var v in values)
            {
                var line = $"{v.ParameterName}                     {v.Value?.ToString() ?? string.Empty}";
                AppendText(contentSb, 50, y, line);
                y -= 12;
                if (y < 40)
                {
                    // no multi-page support in this minimal writer
                    break;
                }
            }

            contentSb.AppendLine("ET");
            var contentBytes = Encoding.ASCII.GetBytes(contentSb.ToString());

            // Object 1: catalog
            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
            // Object 2: pages
            objects.Add("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            // Object 3: page
            objects.Add("<< /Type /Page /Parent 2 0 R /Resources << /Font << /F1 4 0 R >> >> /MediaBox [0 0 595 842] /Contents 5 0 R >>");
            // Object 4: font
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            // Object 5: content stream
            var contentStream = $"stream\n{Encoding.ASCII.GetString(contentBytes)}\nendstream";
            objects.Add(contentStream);

            // Build PDF
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(fs, new UTF8Encoding(false));

            writer.WriteLine("%PDF-1.4");

            var offsets = new List<long>();

            for (int i = 0; i < objects.Count; i++)
            {
                writer.Flush();
                offsets.Add(fs.Position);

                int objNum = i + 1;
                if (objNum == 5)
                {
                    // content stream requires length attribute
                    var bytes = Encoding.ASCII.GetBytes(objects[i]);
                    writer.WriteLine($"{objNum} 0 obj");
                    writer.WriteLine($"<< /Length {bytes.Length} >>");
                    writer.WriteLine(objects[i]);
                    writer.WriteLine("endobj");
                }
                else
                {
                    writer.WriteLine($"{objNum} 0 obj");
                    writer.WriteLine(objects[i]);
                    writer.WriteLine("endobj");
                }
            }

            var xrefPosition = fs.Position;
            writer.WriteLine("xref");
            writer.WriteLine($"0 {objects.Count + 1}");
            writer.WriteLine("0000000000 65535 f ");
            foreach (var off in offsets)
            {
                writer.WriteLine(off.ToString("D10") + " 00000 n ");
            }

            writer.WriteLine("trailer");
            writer.WriteLine("<<");
            writer.WriteLine($"/Size {objects.Count + 1}");
            writer.WriteLine($"/Root 1 0 R");
            writer.WriteLine(">>");
            writer.WriteLine("startxref");
            writer.WriteLine(xrefPosition);
            writer.WriteLine("%%EOF");

            writer.Flush();
        }

        private static void AppendText(StringBuilder sb, double x, double y, string text)
        {
            // PDF text positioning: x y Td (text) Tj
            sb.AppendLine($"1 0 0 1 {x} {y} Tm");
            sb.AppendLine($"({Escape(text)}) Tj");
        }

        private static string Escape(string s)
        {
            return s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }
    }
}
