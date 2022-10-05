using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Email.Services;
using Elsa.Services.Models;
using MimeKit;
using Volo.Abp.Emailing;

namespace Passingwind.Abp.ElsaModule
{
    public class AbpSmtpService : ISmtpService
    {
        private readonly IEmailSender _emailSender;

        public AbpSmtpService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendAsync(ActivityExecutionContext context, MimeMessage message, CancellationToken cancellationToken)
        {
            var mailMessage = new MailMessage()
            {
                Subject = message.Subject,
                Body = message.HtmlBody,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
            };

            if (message.From.Any())
                mailMessage.From = new MailAddress(message.From[0].Name);

            if (message.To.Any())
                foreach (var item in message.To)
                {
                    mailMessage.To.Add(item.Name);
                }

            if (message.Cc.Any())
                foreach (var item in message.Cc)
                {
                    mailMessage.CC.Add(item.Name);
                }

            if (message.Bcc.Any())
                foreach (var item in message.Bcc)
                {
                    mailMessage.Bcc.Add(item.Name);
                }

            if (message.Attachments.Any())
            {
                // TODO
            }

            await _emailSender.SendAsync(mailMessage);
        }
    }
}
