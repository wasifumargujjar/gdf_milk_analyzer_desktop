using System;
using System.Windows.Forms;
using MilkAnalyzerTest.DataAccess;
using Microsoft.Data.SqlClient;

namespace MilkAnalyzerTest
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Read connection string from environment variable 'MILK_DB_CONN'
            var conn = Environment.GetEnvironmentVariable("MILK_DB_CONN");
            if (!string.IsNullOrEmpty(conn))
            {
                Database.ConnectionString = conn;
            }
            else
            {
                // Default to Windows Authentication on local SQLEXPRESS instance.
                // Server name provided by user: SQLEXPRESS
                // Default database name: MilkAnalyzer
                Database.ConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=MilkAnalyzerTest;Integrated Security=True;TrustServerCertificate=True;";
            }

            // Seed Profile table with default entries (idempotent)
            try
            {
                var profiles = new[]
                {
                    new { Name = "Default Clinic", NIC = "CLINIC001", Phone = "000-000-0000", WhatsappNumber = "000-000-0000", Address = "123 Main St" },
                    new { Name = "Test User", NIC = "USER001", Phone = "111-111-1111", WhatsappNumber = "111-111-1111", Address = "456 Test Ave" }
                };

                foreach (var p in profiles)
                {
                    var exists = Database.ExecuteScalarAsync<int?>(
                        "SELECT Id FROM dbo.Profile WHERE NIC = @NIC",
                        new SqlParameter("@NIC", System.Data.SqlDbType.NVarChar, 50) { Value = (object?)p.NIC ?? DBNull.Value }
                    ).GetAwaiter().GetResult();

                    if (!exists.HasValue)
                    {
                        Database.ExecuteNonQueryAsync(
                            "INSERT INTO dbo.Profile (Name, NIC, Phone, WhatsappNumber, Address) VALUES (@Name, @NIC, @Phone, @WhatsappNumber, @Address)",
                            new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = p.Name },
                            new SqlParameter("@NIC", System.Data.SqlDbType.NVarChar, 50) { Value = (object?)p.NIC ?? DBNull.Value },
                            new SqlParameter("@Phone", System.Data.SqlDbType.NVarChar, 50) { Value = (object?)p.Phone ?? DBNull.Value },
                            new SqlParameter("@WhatsappNumber", System.Data.SqlDbType.NVarChar, 50) { Value = (object?)p.WhatsappNumber ?? DBNull.Value },
                            new SqlParameter("@Address", System.Data.SqlDbType.NVarChar, 500) { Value = (object?)p.Address ?? DBNull.Value }
                        ).GetAwaiter().GetResult();
                    }
                }

                // Seed analyzer parameters (idempotent)
                try
                {
                    Database.SeedAnalyzerParametersAsync().GetAwaiter().GetResult();
                }
                catch (Exception seedEx)
                {
                    MessageBox.Show($"Failed to seed analyzer parameters: {seedEx.Message}", "Seeding Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to seed Profile table: {ex.Message}", "Seeding Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}