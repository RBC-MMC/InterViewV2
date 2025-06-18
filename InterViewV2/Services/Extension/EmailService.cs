using InterViewV2.Services.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace InterViewV2.Services.Extension
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string message, IEnumerable<(byte[] file, string name)> attachments = null)
        {
            await SendEmailAsync(new string[] { to }, subject, message, attachments);
        }

        public async Task SendEmailAsync(IEnumerable<string> to, string subject, string message, IEnumerable<(byte[] file, string name)> attachments = null)
        {

            var builder = new BodyBuilder
            {
                HtmlBody = message
            };

            if (attachments != null)
            {
                foreach (var (file, name) in attachments)
                {
                    var attachmentName = name?.Replace("/", "_");
                    builder.Attachments.Add(attachmentName, file);
                }
            }

            await SendEmailAsync(to.Select(x => new MailboxAddress(x)), subject, builder.ToMessageBody());
        }
        private async Task SendEmailAsync(IEnumerable<InternetAddress> to, string subject, MimeEntity body)
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = body
            };
            var fromName = configuration["EmailOptions:FromName"];
            var fromPassword = configuration["EmailOptions:FromPass"];
            var fromAddress = configuration["EmailOptions:FromAddress"];
            var smtpClient = configuration["EmailOptions:SmtpClient"];
            var port = int.Parse(configuration["EmailOptions:Port"]);
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.AddRange(to);

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using var client = new SmtpClient();

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(smtpClient, port, SecureSocketOptions.Auto);
            client.Authenticate("interview", "InterCDC2@24!");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
