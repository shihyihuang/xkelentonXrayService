using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xkelenton.Models;
using xkelenton.Utils;
using Ganss.Xss;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AngleSharp.Dom.Events;
using SendGrid.Helpers.Mail;

namespace xkelenton.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "practitioner")]
        public ActionResult SendEmail()
        {
            return View(new EmailViewModels());
        }

        [Authorize(Roles = "practitioner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmail(EmailViewModels model)
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                try
                {
                    // Split input email addresses
                    //https://www.c-sharpcorner.com/UploadFile/mahesh/split-string-in-C-Sharp/
                    List<EmailAddress> toEmails = new List<EmailAddress>();
                    string[] toEmailArray = model.ToEmail.Split(',');
                    foreach (string email in toEmailArray)
                    {
                        toEmails.Add(new EmailAddress(email.Trim()));
                    }
                    // Sanitize user input before storing to db
                    String subject = sanitizer.Sanitize(model.Subject); 
                    String body = sanitizer.Sanitize(model.Body); 
                    HttpPostedFileBase attachment = model.Attachment;

                    // encode user input before embedding into email body
                    // reference https://www.twilio.com/blog/prevent-email-html-injection-in-csharp-and-dotnet
/*                    string encodedBody = HttpUtility.HtmlEncode(body);*/

                    EmailSender es = new EmailSender();
                    es.Send(toEmails, subject, body, attachment);

                    ViewBag.Result = "Successfully sent the email";

                    ModelState.Clear();

                    return View(new EmailViewModels());
                }
                catch
                {
                    return View();
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}