using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xkelenton.Models;


namespace xkelenton.Controllers
{
    public class PatientTableController : Controller

    {
        private xkelentonModelContainer db = new xkelentonModelContainer();

        // GET: PatientTable
        public ActionResult Index()
        {
            // Create a list to hold the view models
            List<PatientTableViewModels> viewModelList = new List<PatientTableViewModels>();

            // https://stackoverflow.com/questions/27465286/creating-simple-viewmodel-to-show-two-tables-of-data-in-one-view-in-mvc-5
            //https://www.codeproject.com/Questions/1063117/How-to-retrieve-all-register-user-from-AspNetUsers
            // Retrieve data from patient and image table
            var patients = db.Patients.ToList(); 
            var images = db.Images.ToList();
            // Retrieve user data from the Identity db
            var context = new IdentityDbContext();
            var users = context.Users.ToList();

            foreach (var patient in patients)
            {
                var patientImages = images.Where(i => i.PatientId == patient.Id).ToList(); 

                foreach (var image in patientImages)
                {
                    // Find the user data belongs to the patient to retrieve user email
                    var user = users.FirstOrDefault(i => i.Id == patient.UserId);
                    if (user != null)
                    {
                        // retrieve data for viewModel
                        var viewModel = new PatientTableViewModels
                        {
                            PatientId = patient.Id,
                            FirstName = patient.FirstName,
                            LastName = patient.LastName,
                            DateOfBirth = patient.DateOfBirth,
                            MobileNumber = patient.MobileNumber,
                            Email = user.UserName,
                            ImageId = image.Id,
                            ScanDate = image.ScanDate
                        };

                        viewModelList.Add(viewModel);
                    }
                }
            }

            return View(viewModelList);
        }


        // GET: PatientTable/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PatientTable/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PatientTable/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientTable/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PatientTable/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientTable/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PatientTable/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
