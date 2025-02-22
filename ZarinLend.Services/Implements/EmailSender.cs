using Common;
using Common.Utilities;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Services
{
    public class EmailSender : IEmailSender, ISingletonDependency
    {
        private readonly MailSettings mailSettings;

        public EmailSender(IOptions<SiteSettings> siteSettings)
        {
            mailSettings = siteSettings.Value.MailSettings;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(mailSettings.Mail, mailSettings.DisplayName),
                    //To = { new MailAddress(mailRequest.ToEmail) },
                    Subject = mailRequest.Subject,
                    Body = mailRequest.Body,
                    IsBodyHtml = true,
                    Sender = new MailAddress(mailSettings.Mail)
                };

                foreach (var address in mailRequest.ToEmail.Split(";", StringSplitOptions.RemoveEmptyEntries))
                    mail.To.Add(address);

                if (mailRequest.Attachments != null)
                {
                    foreach (var file in mailRequest.Attachments.Where(file => file.Length > 0))
                    {
                        byte[] fileBytes;
                        await using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            fileBytes = ms.ToArray();
                        }
                        mail.Attachments.Add(new Attachment(file.FileName, new ContentType(file.ContentType)));
                    }
                }

                if (mailRequest.FilePathAttachments != null && mailRequest.FilePathAttachments.Any())
                {
                    foreach (var file in mailRequest.FilePathAttachments)
                    {
                        if (!File.Exists(file)) continue;
                        mail.Attachments.Add(new Attachment(Path.GetFileName(file), FileExtensions.GetMimeTypeForFileExtension(file)));
                    }
                }

                if (mailRequest.StreamAttachments != null && mailRequest.StreamAttachments.Any())
                {
                    for (var i = 0; i < mailRequest.StreamAttachments.Count; i++)
                    {
                        mail.Attachments.Add(new Attachment(mailRequest.StreamAttachments[i], $"Attachments{i + 1}.zip"));
                    }
                }

                using var smtp = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(mailSettings.Mail, mailSettings.Password),
                    Host = mailSettings.Host,
                    Port = mailSettings.Port,
                    EnableSsl = mailSettings.EnableSsl
                };

                await smtp.SendMailAsync(mail);
            }
            catch (Exception exp)
            {
                // ignored
            }
        }
    }
}
