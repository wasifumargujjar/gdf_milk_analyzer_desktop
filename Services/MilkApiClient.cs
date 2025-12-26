using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using MilkAnalyzerTest.Config;

namespace MilkAnalyzerTest.Services
{
    // Simple client to login and submit milk test result to the API
    public class MilkApiClient
    {
        private readonly HttpClient _http;

        public MilkApiClient(HttpClient? http = null)
        {
            _http = http ?? new HttpClient();
            // Ensure base address ends with trailing slash so relative paths resolve under /api/
            var baseUrl = Settings.ApiBaseUrl ?? string.Empty;
            if (!baseUrl.EndsWith('/')) baseUrl += '/';
            _http.BaseAddress = new Uri(baseUrl);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var payload = new { Username = username, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var resp = await _http.PostAsync("auth/login", content);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Login failed: {resp.StatusCode} - {body}");

            using var doc = JsonDocument.Parse(body);
            // accept either camelCase or PascalCase token property
            if (!doc.RootElement.TryGetProperty("token", out var tok) && !doc.RootElement.TryGetProperty("Token", out tok))
            {
                // try nested 'data.token' pattern
                if (doc.RootElement.TryGetProperty("data", out var dataElem) && (dataElem.ValueKind == JsonValueKind.Object))
                {
                    if (!dataElem.TryGetProperty("token", out tok) && !dataElem.TryGetProperty("Token", out tok))
                    {
                        throw new InvalidOperationException($"Login response does not contain token: {body}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Login response does not contain token: {body}");
                }
            }

            var token = tok.GetString();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException($"Login returned empty token: {body}");

            return token.Trim();
        }

        public async Task SubmitMilkTestAsync(string token, object submitDto)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("JWT token is required", nameof(token));

            // Use per-request HttpRequestMessage and set Authorization header on the request
            var request = new HttpRequestMessage(HttpMethod.Post, "milktest/submit");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
            request.Content = new StringContent(JsonSerializer.Serialize(submitDto), Encoding.UTF8, "application/json");

            using var resp = await _http.SendAsync(request);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Submit failed: {resp.StatusCode} - {body}");
        }
    }
}
