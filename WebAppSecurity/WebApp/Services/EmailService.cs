using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using WebApp.Settings;
using MailKit.Security;
using MimeKit;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<STMPSetting> smtpSetting;

        public EmailService(IOptions<STMPSetting> smtpSetting)
        {
            this.smtpSetting = smtpSetting;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            //var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("Abdullah Gomaa", from));
            //message.To.Add(new MailboxAddress("Recipient Name", to));
            //message.Subject = subject;
            //message.Body = new TextPart("plain") { Text = body };
            var message = new MailMessage(from, to, subject, body);
            using (var emailClient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(smtpSetting.Value.User, smtpSetting.Value.Password);
                emailClient.EnableSsl = true;
                await emailClient.SendMailAsync(message);
            }
            //using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    await emailClient.ConnectAsync(smtpSetting.Value.Host, smtpSetting.Value.Port, SecureSocketOptions.StartTls);
            //    await emailClient.AuthenticateAsync(smtpSetting.Value.User, smtpSetting.Value.Password);
            //    await emailClient.SendAsync(message);
            //    await emailClient.DisconnectAsync(true);
            //}
        }
    }
}
