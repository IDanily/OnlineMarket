using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OnlineMarket.Application.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "online.market.03@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect);

                await client.AuthenticateAsync("online.market.03@mail.ru", "syPWd2CJdKYRFY4atTKq");

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
