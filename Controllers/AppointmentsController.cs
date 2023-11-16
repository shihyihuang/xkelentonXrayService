using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using xkelenton.Models;
using Ganss.Xss;

namespace xkelenton.Controllers
{
    [Authorize(Roles = "patient")]
    public class AppointmentsController : Controller
    {
        private xkelentonModelContainer db = new xkelentonModelContainer();

        // GET: Appointments
        public ActionResult Index()
        {
            //https://stackoverflow.com/questions/20637776/a-specified-include-path-is-not-valid-the-entitytype-does-not-declare-a-navigat
            // Retrieve logged-in patient's appointments and related info for the current user
            string currentUserId = User.Identity.GetUserId();
            var appointments = db.Appointments
                .Where(a => a.Patient.UserId == currentUserId)
                .Include(a => a.Practitioner)
                .Include(a => a.Patient)
                .ToList();

            return View(appointments);
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            //https://stackoverflow.com/questions/1745691/linq-when-to-use-singleordefault-vs-firstordefault-with-filtering-criteria
            string currentUserId = User.Identity.GetUserId();
            // If the user did not create patient info, redirect them to create it, as appointment table requires those info
            Patient patient = db.Patients.SingleOrDefault(p => p.UserId == currentUserId);
            if (patient == null)
            {
                return RedirectToAction("Create", "Patients");
            }

            // Combine first name and last name for the dropdown list
            //https://stackoverflow.com/questions/50064059/how-to-select-only-ids-with-linq-when-there-is-list-in-list
            var practitioners = db.Practitioners.Select(p => new
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName 
            }).ToList();
            //https://stackoverflow.com/questions/16594958/how-to-use-a-viewbag-to-create-a-dropdownlist
            ViewBag.PractitionerId = new SelectList(practitioners, "Id", "FullName");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AppointmentTime,PractitionerId")] Appointment appointment)
        {

            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                string sanitizedAppointmentTime = sanitizer.Sanitize(appointment.AppointmentTime.ToString());
                appointment.AppointmentTime = DateTime.Parse(sanitizedAppointmentTime);

                string currentUserId = User.Identity.GetUserId(); 
                int? currentPatientId = db.Patients.Where(p => p.UserId == currentUserId).Select(p => (int?)p.Id).FirstOrDefault();

                if (currentPatientId.HasValue)
                {
                    // Check if the chosen appointment time has already been booked
                    bool isAppointmentTimeChosen = db.Appointments.Any(a => a.AppointmentTime == appointment.AppointmentTime);
                    if (!isAppointmentTimeChosen)
                    {
                        appointment.PatientId = currentPatientId.Value;

                        db.Appointments.Add(appointment);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("AppointmentTime", "This timeslot has been booked, please choose another time");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The patient info does not exist");
                }
            }

            // Combine first name and last name for the dropdown list
            //https://stackoverflow.com/questions/50064059/how-to-select-only-ids-with-linq-when-there-is-list-in-list
            var practitioners = db.Practitioners.Select(p => new
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName 
            }).ToList();

            //https://stackoverflow.com/questions/16594958/how-to-use-a-viewbag-to-create-a-dropdownlist
            ViewBag.PractitionerId = new SelectList(practitioners, "Id", "FullName", appointment.PractitionerId);
            return View(appointment);
        }



        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            // Combine first name and last name for the dropdown list
            //https://stackoverflow.com/questions/22645760/combining-two-database-columns-in-a-dropdown-list
            var practitioners = db.Practitioners.Select(p => new
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName
            }).ToList();

            ViewBag.PractitionerId = new SelectList(practitioners, "Id", "FullName");
/*            ViewBag.PatientId = new SelectList(db.Patients, "Id", "FirstName", appointment.PatientId);*/
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AppointmentTime,PractitionerId,PatientId")] Appointment appointment)
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                string sanitizedAppointmentTime = sanitizer.Sanitize(appointment.AppointmentTime.ToString());
                appointment.AppointmentTime = DateTime.Parse(sanitizedAppointmentTime);

                string currentUserId = User.Identity.GetUserId();
                int? currentPatientId = db.Patients.Where(p => p.UserId == currentUserId).Select(p => (int?)p.Id).FirstOrDefault();
                if (currentPatientId.HasValue)
                {
                    // Check if the chosen appointment time has already been booked
                    bool isAppointmentTimeChosen = db.Appointments.Any(a => a.AppointmentTime == appointment.AppointmentTime);
                    if (!isAppointmentTimeChosen)
                    {
                        appointment.PatientId = currentPatientId.Value;

                        db.Entry(appointment).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("AppointmentTime", "This timeslot has been booked, please choose another time");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The patient info does not exist");
                }
            }

            // Combine first name and last name for the dropdown list
            //https://stackoverflow.com/questions/22645760/combining-two-database-columns-in-a-dropdown-list
            var practitioners = db.Practitioners.Select(p => new
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName
            }).ToList();

            ViewBag.PractitionerId = new SelectList(practitioners, "Id", "FullName", appointment.PractitionerId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
