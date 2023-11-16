using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using xkelenton.Models;

namespace xkelenton.Controllers
{
    [Authorize(Roles = "practitioner")]
    public class ImagesController : Controller
    {
        private xkelentonModelContainer db = new xkelentonModelContainer();

        // GET: Images
        public ActionResult Index()
        {
            var images = db.Images.Include(i => i.Patient);
            return View(images.ToList());
        }

        // GET: Images/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Images/Create
        public ActionResult Create()
        {
            // Combine first name and last name for the dropdown list
            //https://stackoverflow.com/questions/50064059/how-to-select-only-ids-with-linq-when-there-is-list-in-list
            var patients = db.Patients.Select(p => new
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName
            }).ToList();

            ViewBag.PatientId = new SelectList(patients, "Id", "FullName");
            return View();

        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ScanDate,ImageUrl,PatientId")] Image image, HttpPostedFileBase postedFile)
        {
            var sanitizer = new HtmlSanitizer();

            ModelState.Clear();
            var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            image.ImageUrl = myUniqueFileName;
            TryValidateModel(image);

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                string sanitizedScanDate = sanitizer.Sanitize(image.ScanDate.ToString());
                image.ScanDate = DateTime.Parse(sanitizedScanDate);

                if (postedFile != null)
                {
                    // Get the file extension
                    string fileExtension = Path.GetExtension(postedFile.FileName);
                    // reference: https://www.c-sharpcorner.com/article/file-upload-extension-validation-in-asp-net-mvc-and-javascript/
                    // references: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0 
                    // Check if the uploaded file is an accepted file type
                    string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                    if (allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        string serverPath = Server.MapPath("~/UploadImages/");
                        string filePath = image.ImageUrl + fileExtension;
                        image.ImageUrl = filePath;

                        //check if the folder exist
                        //https://stackoverflow.com/questions/9065598/if-a-folder-does-not-exist-create-it
                        if (!Directory.Exists(serverPath))
                        {
                            Directory.CreateDirectory(serverPath);
                        }

                        postedFile.SaveAs(Path.Combine(serverPath, image.ImageUrl));
                        db.Images.Add(image);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Please upload jpeg/jpg/png only");
                    }
                }
            }

            // Combine first name and last name for the dropdown list
            //https://stackoverflow.com/questions/50064059/how-to-select-only-ids-with-linq-when-there-is-list-in-list
            var patients = db.Patients.Select(p => new
            {
                Id = p.Id,
                FullName = p.FirstName + " " + p.LastName
            }).ToList();

            ViewBag.PatientId = new SelectList(patients, "Id", "FullName", image.PatientId);

            return View(image);
        }


        // GET: Images/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "FirstName", image.PatientId);
            return View(image);
        }


        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ScanDate,ImageUrl,PatientId")] Image image)
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                // Sanitize user input before storing to db
                //https://csharp.hotexamples.com/examples/-/HtmlSanitizer/-/php-htmlsanitizer-class-examples.html
                string sanitizedScanDate = sanitizer.Sanitize(image.ScanDate.ToString());
                image.ScanDate = DateTime.Parse(sanitizedScanDate);

                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "FirstName", image.PatientId);
            return View(image);
        }



        // GET: Images/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Image image = db.Images.Find(id);
            db.Images.Remove(image);
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
