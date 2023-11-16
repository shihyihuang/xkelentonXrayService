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
    using System;
    using System.ComponentModel.DataAnnotations;
    using ExpressiveAnnotations.Attributes;

    public partial class Appointment
    {
        public int Id { get; set; }

        [Display(Name = "Appointment Time ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        // reference: https://github.com/jwaliszko/ExpressiveAnnotations
        [AssertThat("AppointmentTime > Now()", ErrorMessage = "Please choose your preferred time from tomorrow")]
        public System.DateTime AppointmentTime { get; set; }

        [Display(Name = "Practitioner ")]
        public int PractitionerId { get; set; }
        public int PatientId { get; set; }

        public virtual Practitioner Practitioner { get; set; }
        public virtual Patient Patient { get; set; }
    }

}
