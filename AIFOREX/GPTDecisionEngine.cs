using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIFOREX
{
    public class GPTDecisionEngine
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GPTDecisionEngine(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> AskDecisionAsync(string symbol, double price, double macd, double rsi)
        {
            string prompt = $"האם כדאי לקנות, למכור או להמתין עם מניית {symbol} לפי הנתונים הבאים: מחיר נוכחי {price}, MACD {macd}, RSI {rsi}? תן המלצה אחת ברורה עם סיבה.";

            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 150
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content);
            var responseText = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseText);
            var result = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

            return result?.Trim();
        }
    }
}
