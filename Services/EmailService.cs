using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PaieApi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var settings = _configuration.GetSection("EmailSettings");
            var smtpServer = settings["SmtpServer"];
            var port = int.Parse(settings["Port"]);
            var senderEmail = settings["SenderEmail"];
            var senderName = settings["SenderName"];
            var password = settings["Password"];

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password) || password.Contains("YOUR_APP_PASSWORD"))
            {
                Console.WriteLine($"[EMAIL MOCK BLOCKED] Real email credentials missing. Would send to {to}");
                return;
            }

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL SENT] To: {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.Message}");
                throw;
            }
        }
    }
}
