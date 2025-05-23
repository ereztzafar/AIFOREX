using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIFOREX
{
    /// <summary>
    /// מחלקה לשליחת הודעות טלגרם לבוטים/ערוצים שונים.
    /// </summary>
    public class TelegramNotifier
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// בנאי דיפולטי – עובד עם בוט וערוץ ברירת מחדל.
        /// </summary>
        public TelegramNotifier()
        {
            _botToken = "7692413290:AAEk7llQ2e7cejvn3XNlSw4AntlxGGkIdh4";  // טוקן לדוגמה
            _chatId = "813610615"; // מזהה צ'אט ברירת מחדל
        }

        /// <summary>
        /// בנאי מותאם אישית – מאפשר שליחה לבוט וצ'אט אחרים.
        /// </summary>
        /// <param name="botToken">מפתח גישה לבוט</param>
        /// <param name="chatId">מזהה צ'אט (קבוצה, ערוץ, משתמש)</param>
        public TelegramNotifier(string botToken, string chatId)
        {
            _botToken = botToken;
            _chatId = chatId;
        }

        /// <summary>
        /// שולח הודעה לטלגרם באופן חסום.
        /// </summary>
        public void Send(string message)
        {
            SendMessageAsync(message).Wait();
        }

        /// <summary>
        /// שולח הודעה לטלגרם באופן אסינכרוני.
        /// </summary>
        private async Task SendMessageAsync(string message)
        {
            try
            {
                var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("chat_id", _chatId),
                    new KeyValuePair<string, string>("text", message)
                });

                var response = await _httpClient.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ שגיאה בטלגרם: {response.StatusCode}");
                    Console.WriteLine($"🔍 תגובה: {result}");

                    if ((int)response.StatusCode == 429 && result.Contains("retry_after"))
                    {
                        int retryAfter = ExtractRetryAfter(result);
                        Console.WriteLine($"⏳ ממתין {retryAfter} שניות לניסיון חוזר...");
                        await Task.Delay(retryAfter * 1000);
                        await SendMessageAsync(message); // ניסיון חוזר
                    }
                }
                else
                {
                    Console.WriteLine($"✅ נשלחה הודעה לטלגרם: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בשליחת טלגרם: {ex.Message}");
            }
        }

        /// <summary>
        /// שולף את ערך retry_after מתוך תגובת JSON.
        /// </summary>
        private static int ExtractRetryAfter(string json)
        {
            var match = Regex.Match(json, @"""retry_after"":\s*(\d+)");
            return match.Success ? int.Parse(match.Groups[1].Value) : 5;
        }
    }
}
