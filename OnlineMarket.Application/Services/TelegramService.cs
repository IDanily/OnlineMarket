using System.Text;

namespace OnlineMarket.Application.Services
{
    public class TelegramService
    {
        private readonly string _telegramBotToken = "8146316538:AAHPQvqPlfPOHOFEB15rlfcR0KgAYFCT3Bk"; // Замените на ваш токен
        private readonly string _chatId = "852420570"; // Замените на ваш chat_id

        public async Task SendMessageAsync(string name, string email, string message)
        {
            var client = new HttpClient();
            var url = $"https://api.telegram.org/bot{_telegramBotToken}/sendMessage";

            var content = new StringContent(
                $"{{\"chat_id\": \"{_chatId}\", \"text\": \"Сообщение от {name} ({email}): {message}\"}}",
                Encoding.UTF8, "application/json");

            await client.PostAsync(url, content);
        }
    }
}
