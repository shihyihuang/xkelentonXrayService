using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using xkelenton.Models;
using System.Linq;
using System.Web;

namespace xkelenton.Models
{
    public class PatientTableViewModels
    {
        [Display(Name = "Patient Id")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please enter your First Name")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Please include only alphabetic characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Please enter your Last Name")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Please include only alphabetic characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /*        https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-validation*/
        [Required(ErrorMessage = "Please choose your date of birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{{0:dd/MM/yyyy}}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Of Birth")]
        public System.DateTime DateOfBirth { get; set; }


        [Required(ErrorMessage = "Please enter your Mobile Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please include a valid 10-digit number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        public string Email { get; set; }

        [Display(Name = "Image Id")]
        public int ImageId { get; set; }

        [Required(ErrorMessage = "Please select a scan date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{{0:dd/MM/yyyy}}", ApplyFormatInEditMode = true)]
        [Display(Name = "Scan Date")]
        public System.DateTime ScanDate { get; set; }

    }
}