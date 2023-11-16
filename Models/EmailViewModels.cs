using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace xkelenton.Models
{
    public class EmailViewModels
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Please enter an email address.")]
        //https://stackoverflow.com/questions/4412725/how-to-match-a-comma-separated-list-of-emails-with-regex
        [RegularExpression(@"^([\w\.-]+@[\w\.-]+)(,\s*[\w\.-]+@[\w\.-]+)*$", ErrorMessage = "Please enter a list of email addresses separated by commas")]
        public string ToEmail { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        public string Subject {  get; set; }

        [Required(ErrorMessage = "Please include content.")]
        [AllowHtml]
        public string Body { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

    }
}