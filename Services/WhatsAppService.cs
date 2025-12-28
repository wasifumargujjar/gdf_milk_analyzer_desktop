using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MilkAnalyzerTest.Services
{
    // Service to send document via WhatsApp Cloud API (Facebook Graph).
    // Requires environment variables:
    // - WHATSAPP_TOKEN : permanent or short-lived access token
    // - WHATSAPP_PHONE_NUMBER_ID : your WhatsApp Business Cloud phone-number-id
    // Example usage:
    // await WhatsAppService.SendDocumentAsync("+1234567890", "path/to/file.pdf", "Caption text");
    public static class WhatsAppService
    {
        private static string? GetEnv(string name) => Environment.GetEnvironmentVariable(name);

        public static async Task SendDocumentAsync(string toNumber, string filePath, string caption)
        {
            var twilioID = "VA68XDQKFP8JWJ5WW3K6YN4C";
            var token = "EAAtoLoCYzV0BQNNaZC5GwCxV4os3GbIzT793p0HT7ItVZCmPIZAQiLL5xlZAFKQhKsf0jTiDiTPsVMpwn5vE5BPZBBZAGZBZBvPstZALRaaQxZCZAcELjE8A6WRUOhcUbKbaPeqZCOhwoYxdfbFN1JVK6emzvWqyrZBJeKclUIPBwOHw29lOpTiBZAiXE2Gv97udS9e2mKnAGZB4WfJRtJLYrvTDO8ZCTAd81Hdc3ZBZBBBSfD94jSAXz0YojZCxG8ae9zcTNuwVARxQMmkkmq9PqwFviJrAu4YLf7IUj4QRMwqitJ6swZDZD"; //GetEnv("WHATSAPP_TOKEN");
            var phoneNumberId = "962753810246709"; // GetEnv("WHATSAPP_PHONE_NUMBER_ID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(phoneNumberId))
                throw new InvalidOperationException("WHATSAPP_TOKEN and WHATSAPP_PHONE_NUMBER_ID environment variables must be set.");

            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found", filePath);

            using var http = new HttpClient { BaseAddress = new Uri("https://graph.facebook.com/") };
            // 1) upload media
            using var content = new MultipartFormDataContent();
            var fileBytes = File.ReadAllBytes(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent, "file", Path.GetFileName(filePath));
            content.Add(new StringContent("application/pdf"), "type");
            // messaging_product is required by WhatsApp Cloud API for media upload
            content.Add(new StringContent("whatsapp"), "messaging_product");

            var uploadUrl = $"v17.0/{phoneNumberId}/media"; // API version may be adjusted
            using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = content;

            using var uploadResp = await http.SendAsync(req);
            var uploadBody = await uploadResp.Content.ReadAsStringAsync();
            if (!uploadResp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Media upload failed: {uploadResp.StatusCode} - {uploadBody}");
            }

            using var doc = JsonDocument.Parse(uploadBody);
            if (!doc.RootElement.TryGetProperty("id", out var mediaIdElem))
                throw new InvalidOperationException($"Unexpected upload response: {uploadBody}");
            var mediaId = mediaIdElem.GetString();

            // 2) send message referencing media id
            var messageUrl = $"v17.0/{phoneNumberId}/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = toNumber,
                type = "document",
                document = new { id = mediaId, caption }
            };

            using var msgReq = new HttpRequestMessage(HttpMethod.Post, messageUrl);
            msgReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            msgReq.Content = new StringContent(JsonSerializer.Serialize(payload));
            msgReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var msgResp = await http.SendAsync(msgReq);
            var msgBody = await msgResp.Content.ReadAsStringAsync();
            if (!msgResp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Send message failed: {msgResp.StatusCode} - {msgBody}");
            }
        }

        public static Task SendPdfReportToCustomerAsync(string whatsappNumber, string customerName, string pdfFilePath)
        {
            var message = $"Dear customer {customerName}, your requested milk test report is attached. Thanks";
            return SendDocumentAsync(whatsappNumber, pdfFilePath, message);
        }
    }
}
