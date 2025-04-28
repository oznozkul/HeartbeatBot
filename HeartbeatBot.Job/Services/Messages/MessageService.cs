using System.Text;

namespace HeartbeatBot.Job.Services.Messages
{
    public class MessageService : IMessageService
    {
        private static readonly string botToken = "8048099727:AAFAoPb5eDQO8c-STKOWr6tOhvAXEr4";
        private static readonly string chatId = "-447330";
        public async Task SendTelegramMessage(string message)
        {
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("📩 Telegram API Yanıtı: " + responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Telegram mesajı gönderilemedi: " + ex.Message);
                }
            }
        }
        public async Task SendStatusMessage(string appName, bool isRunning)
        {
            string status = isRunning ? "✅ *Bildirim*: " : "⚠️ *UYARI*: ";
            string message = isRunning
                ? $"{appName} Çalışmaya Başladı!"
                : $"{appName} çalışmıyor!";

            await SendTelegramMessage($"{status}{message}");
        }
        public async Task SendStatusMessage(string appName, string errorDescription)
        {
            string status = "⚠️ *UYARI*: ";
            string message =  $"{appName} hata oluştu!";

            await SendTelegramMessage($"{status}{message} : {errorDescription}");
        }
    }
}
