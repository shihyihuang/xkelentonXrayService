using Ganss.Xss;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using xkelenton.Models;



namespace xkelenton.Controllers
{

    [Authorize(Roles =("patient"))]
    public class PatientsController : Controller
    {
        private xkelentonModelContainer db = new xkelentonModelContainer();

        // GET: Patients
        public ActionResult Index()
        {
            //https://stackoverflow.com/questions/20925822/asp-net-mvc-5-identity-how-to-get-current-applicationuser
            string currentUserId = User.Identity.GetUserId();
            // Check if the user creates patient data yet
            Patient existingPatient = db.Patients.FirstOrDefault(p => p.UserId == currentUserId);
            // Redirect to the Create action if the user did not create patient data yet
            if (existingPatient == null)
            {
                return RedirectToAction("Create");
            }
            return View(db.Patients.Where(m => m.UserId == currentUserId).ToList());
        }




        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            Patient patient = new Patient();
            // Set the patient table's UserId to the logged in user's ID
            string currentUserId = User.Identity.GetUserId();
            patient.UserId = currentUserId; 
            return View(patient);
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,DateOfBirth,MobileNumber,UserId")] Patient patient)
        {
            var sanitizer = new HtmlSanitizer();
            string currentUserId = User.Identity.GetUserId();
            // redirect to the edit page if a patient record already created  
            Patient existingPatient = db.Patients.FirstOrDefault(p => p.UserId == currentUserId);
            if (existingPatient != null)
            {
                return RedirectToAction("Edit", new { id = existingPatient.Id });
            }

            if (ModelState.IsValid)
            {
                //https://stackoverflow.com/questions/12936604/how-to-add-modelstate-addmodelerror-message-when-model-item-is-not-binded
                if (!IsValidName(patient.FirstName))
                {
                    ModelState.AddModelError("FirstName", "First Name is invalid.");
                }

                if (!IsValidName(patient.LastName))
                {
                    ModelState.AddModelError("LastName", "Last Name is invalid.");
                }

                if (!IsValidMobileNumber(patient.MobileNumber))
                {
                    ModelState.AddModelError("MobileNumber", "Mobile Number is invalid.");
                }

                if (ModelState.IsValid)
                {
                    // Sanitize user input before storing to db
                    //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                    patient.FirstName = sanitizer.Sanitize(patient.FirstName);
                    patient.LastName = sanitizer.Sanitize(patient.LastName);
                    string sanitizedDateOfBirth = sanitizer.Sanitize(patient.DateOfBirth.ToString());
                    patient.DateOfBirth = DateTime.Parse(sanitizedDateOfBirth);
                    patient.MobileNumber = sanitizer.Sanitize(patient.MobileNumber);

                    db.Patients.Add(patient);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(patient);
        }

        //https://stackoverflow.com/questions/6017778/c-sharp-regex-checking-for-a-z-and-a-z
        public bool IsValidName(String name)
        {
            bool isValidName = true;
            if (!Regex.IsMatch(name, @"^[A-Za-z]+$"))
            {
                isValidName = false;
            }
            return isValidName;
        }

        public bool IsValidMobileNumber(String mobileNumber)
        {
            bool isValidMobileNumber = true;

            if (!Regex.IsMatch(mobileNumber, @"\d"))
            {
                ModelState.AddModelError("mobileNumber", "Mobile Number includes only numeric characters");
                isValidMobileNumber = false;
            }
            if (mobileNumber.Length != 10)
            {
                ModelState.AddModelError("mobileNumber", "Mobile Number is limited to a length of 10");
                isValidMobileNumber = false;
            }

            return isValidMobileNumber;
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,DateOfBirth,MobileNumber,UserId")] Patient patient)
        {
            var sanitizer = new HtmlSanitizer();
            if (ModelState.IsValid)
            {
                //https://stackoverflow.com/questions/12936604/how-to-add-modelstate-addmodelerror-message-when-model-item-is-not-binded
                if (!IsValidName(patient.FirstName))
                {
                    ModelState.AddModelError("FirstName", "First Name is invalid.");
                }

                if (!IsValidName(patient.LastName))
                {
                    ModelState.AddModelError("LastName", "Last Name is invalid.");
                }

                if (!IsValidMobileNumber(patient.MobileNumber))
                {
                    ModelState.AddModelError("MobileNumber", "Mobile Number is invalid.");
                }
                if (ModelState.IsValid)
                {
                    // Sanitize user input before storing to db
                    //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                    patient.FirstName = sanitizer.Sanitize(patient.FirstName);
                    patient.LastName = sanitizer.Sanitize(patient.LastName);
                    string sanitizedDateOfBirth = sanitizer.Sanitize(patient.DateOfBirth.ToString());
                    patient.DateOfBirth = DateTime.Parse(sanitizedDateOfBirth);
                    patient.MobileNumber = sanitizer.Sanitize(patient.MobileNumber);

                    db.Entry(patient).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
