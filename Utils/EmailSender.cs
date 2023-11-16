using Ganss.Xss;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace xkelenton.Utils
{
    public class EmailSender
    {
        [ValidateAntiForgeryToken]
        public void Send(List<EmailAddress> toEmails, string subject, string body, HttpPostedFileBase postedFile)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            // Sanitize the email body 
            var sanitizer = new HtmlSanitizer();

            try
            {
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("hclaire1007@gmail.com", "Claire");
                body = sanitizer.Sanitize(body);
                var plainTextContent = body;
                var htmlContent = "<p>" + body + "</p";
                var msg = SendToMultipleRecipients(from, toEmails, subject, plainTextContent, htmlContent);

                if (postedFile != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        postedFile.InputStream.CopyTo(stream);
                        var fileBytes = stream.ToArray();
                        var file = Convert.ToBase64String(fileBytes);

                        msg.AddAttachment(postedFile.FileName, file);
                    }
                }

                var response = client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        [ValidateAntiForgeryToken]
        //https://github.com/sendgrid/sendgrid-csharp/blob/main/src/SendGrid/Helpers/Mail/MailHelper.cs
        public static SendGridMessage SendToMultipleRecipients( EmailAddress from, List<EmailAddress> tos, 
            string subject,string plainTextContent, string htmlContent)
        {
            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.SetGlobalSubject(subject);
            if (!string.IsNullOrEmpty(plainTextContent))
            {
                msg.AddContent(MimeType.Text, plainTextContent);
            }

            if (!string.IsNullOrEmpty(htmlContent))
            {
                msg.AddContent(MimeType.Html, htmlContent);
            }

            for (var i = 0; i < tos.Count; i++)
            {
                msg.AddTo(tos[i], i);
            }

            return msg;
        }

    }
}