using Ganss.Xss;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using xkelenton.Models;

namespace xkelenton.Controllers
{
    [Authorize(Roles = "patient, admin")]
    public class FeedbacksController : Controller
    {
        private xkelentonModelContainer db = new xkelentonModelContainer();

        // GET: Feedbacks
        [Authorize(Roles = "patient")]
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            // Retrieve logged-in patient's feedbacks from db
            var feedbacks = db.Feedbacks
                .Include(f => f.Patient)
                .Where(f => f.Patient.UserId == currentUserId);

            // Calculate the average rating score from all feedbacks in db not limit to the current patient
            double averageRatingScore = db.Feedbacks.Any() ? db.Feedbacks.Average(f => f.RatingScore) : 0.0;
            averageRatingScore = Math.Round(averageRatingScore, 1);

            // Create a view model to pass current logged-in patient's feedbacks and average rating score of all feedbacks in db to the view
            var viewModel = new FeedbackViewModels
            {
                Feedbacks = feedbacks.ToList(),
                AverageRatingScore = averageRatingScore
            };

            return View(viewModel);
        }


        // GET: Feedbacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // GET: Feedbacks/Create
        public ActionResult Create()
        {
            string currentUserId = User.Identity.GetUserId();
            // If the user did not create patient info, redirect them to create it, as feedback table requires those info
            Patient patient = db.Patients.SingleOrDefault(p => p.UserId == currentUserId);
            if (patient == null)
            {
                return RedirectToAction("Create", "Patients");
            }

            Feedback feedback = new Feedback();
            feedback.PatientId = patient.Id;

            return View(feedback);
   
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,RatingScore,Comment,PatientId")] Feedback feedback)
        {
            try
            {
                var sanitizer = new HtmlSanitizer();

                if (ModelState.IsValid)
                {
                    // Sanitize user input before storing to the db
                    feedback.Comment = sanitizer.Sanitize(feedback.Comment);

                    db.Feedbacks.Add(feedback);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (HttpRequestValidationException ex)
            {
                if (feedback.Comment.Contains("<script>"))
                {
                    ModelState.AddModelError("Comment", "Invalid input. Please remove script in your comment");
                }
            }

            /*            ViewBag.PatientId = new SelectList(db.Patients, "Id", "FirstName", feedback.PatientId);*/
            return View(feedback);
        }


        // GET: Feedbacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "FirstName", feedback.PatientId);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,RatingScore,Comment,PatientId")] Feedback feedback)
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                feedback.Comment = sanitizer.Sanitize(feedback.Comment);

                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
/*            ViewBag.PatientId = new SelectList(db.Patients, "Id", "FirstName", feedback.PatientId);*/
            return View(feedback);
        }

/*        [Authorize(Roles = "admin")]*/
        // GET: Feedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
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
