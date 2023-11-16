//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace xkelenton.Models
{
    using ExpressiveAnnotations.Attributes;
    using System.ComponentModel.DataAnnotations;


    public partial class Image
    {
        public int Id { get; set; }

        /*        https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-validation*/
        [Required(ErrorMessage = "Please select a scan date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{{0:dd/MM/yyyy}}", ApplyFormatInEditMode = true)]
        [AssertThat("ScanDate < Now()", ErrorMessage = "Can't choose a date later than today")]
        [Display(Name = "Scan Date")]
        public System.DateTime ScanDate { get; set; }

        [Required(ErrorMessage = "Please select a file to upload")]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Please select a patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
