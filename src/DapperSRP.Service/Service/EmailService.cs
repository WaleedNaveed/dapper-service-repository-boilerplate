using DapperSRP.Service.Interface;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using DapperSRP.Common.Configuration;

namespace DapperSRP.Service.Service
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpConfig> _smtp;

        public EmailService(IOptions<SmtpConfig> smtp)
        {
            _smtp = smtp;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient
            {
                Host = _smtp.Value.Host,
                Port = _smtp.Value.Port,
                Credentials = new NetworkCredential(
                    _smtp.Value.Username,
                    _smtp.Value.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtp.Value.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
