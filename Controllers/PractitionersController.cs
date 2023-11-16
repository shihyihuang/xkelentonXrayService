using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganss.Xss;

namespace xkelenton.Models
{
    [Authorize(Roles ="practitioner")]
    public class PractitionersController : Controller
    {
        private xkelentonModelContainer db = new xkelentonModelContainer();

        // GET: Practitioners
        public ActionResult Index()
        {
            //https://stackoverflow.com/questions/20925822/asp-net-mvc-5-identity-how-to-get-current-applicationuser
            string currentUserId = User.Identity.GetUserId();
            // Check if the user creates practitioner data yet
            Practitioner existingPractitioner = db.Practitioners.FirstOrDefault(p => p.UserId == currentUserId);
            // Redirect to the Create action if the user did not create practitioner data yet
            if (existingPractitioner == null)
            {
                return RedirectToAction("Create");
            }
            return View(db.Practitioners.Where(m => m.UserId == currentUserId).ToList());
        }

        // GET: Practitioners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Practitioner practitioner = db.Practitioners.Find(id);
            if (practitioner == null)
            {
                return HttpNotFound();
            }
            return View(practitioner);
        }

        // GET: Practitioners/Create
        public ActionResult Create()
        {
            Practitioner practitioner = new Practitioner();
            // Set the practitioner table's UserId to the logged in user's ID
            string currentUserId = User.Identity.GetUserId();
            practitioner.UserId = currentUserId;
            return View(practitioner);
        }

        // POST: Practitioners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,AhpraNumber,MobileNumber,UserId")] Practitioner practitioner)
        {
            var sanitizer = new HtmlSanitizer();

            string currentUserId = User.Identity.GetUserId();
            // redirect to the edit page if a practitioner record already created  
            Practitioner existingPractitioner = db.Practitioners.FirstOrDefault(p => p.UserId == currentUserId);
            if (existingPractitioner != null)
            {
                return RedirectToAction("Edit", new { id = existingPractitioner.Id });
            }

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                practitioner.FirstName = sanitizer.Sanitize(practitioner.FirstName);
                practitioner.LastName = sanitizer.Sanitize(practitioner.LastName);
                practitioner.AhpraNumber = sanitizer.Sanitize(practitioner.AhpraNumber);
                practitioner.MobileNumber = sanitizer.Sanitize(practitioner.MobileNumber);

                db.Practitioners.Add(practitioner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(practitioner);
        }

        // GET: Practitioners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Practitioner practitioner = db.Practitioners.Find(id);
            if (practitioner == null)
            {
                return HttpNotFound();
            }
            return View(practitioner);
        }

        // POST: Practitioners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,AhpraNumber,MobileNumber,UserId")] Practitioner practitioner)
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                practitioner.FirstName = sanitizer.Sanitize(practitioner.FirstName);
                practitioner.LastName = sanitizer.Sanitize(practitioner.LastName);
                practitioner.AhpraNumber = sanitizer.Sanitize(practitioner.AhpraNumber);
                practitioner.MobileNumber = sanitizer.Sanitize(practitioner.MobileNumber);

                db.Entry(practitioner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(practitioner);
        }

        // GET: Practitioners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Practitioner practitioner = db.Practitioners.Find(id);
            if (practitioner == null)
            {
                return HttpNotFound();
            }
            return View(practitioner);
        }

        // POST: Practitioners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Practitioner practitioner = db.Practitioners.Find(id);
            db.Practitioners.Remove(practitioner);
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
