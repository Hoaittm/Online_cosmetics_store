using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: Admin/Brand
        public ActionResult Index()
        {
            var brands = db.Brands
                .Where(b => b.status != 0)
                .OrderByDescending(b => b.created_at)
                .ToList();

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(brands);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            // Retrieve the product by ID
            var brand = db.Brands.Where(n => n.id == id).FirstOrDefault();
            // If the product is not found, return a 404 error
            if (brand == null)
            {
                return HttpNotFound();
            }

            // Pass the product object to the view
            return View(brand);
        }
        public ActionResult Trash()
        {
            var brands = db.Brands.Where(b => b.status == 0).ToList();
            return View(brands);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Brand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brand brand)
        {
            int flag = 0;

            // Log the entry into the Create method
            Debug.WriteLine("Entered Create method");

            // Check for null or invalid values and set flags accordingly
            if (string.IsNullOrEmpty(brand.name))
            {
                ModelState.AddModelError("name", "Tên thương hiệu không được để trống.");
                Debug.WriteLine("Validation failed: Name is empty.");
                flag = 1;
            }
            else
            {
                // Check if the brand name already exists
                var existingBrandWithSameName = db.Brands.FirstOrDefault(b => b.name == brand.name);
                if (existingBrandWithSameName != null)
                {
                    ModelState.AddModelError("name", "Tên thương hiệu đã tồn tại. Vui lòng chọn tên khác.");
                    Debug.WriteLine($"Validation failed: Name '{brand.name}' already exists.");
                    flag = 1;
                }
            }



            if (!string.IsNullOrEmpty(brand.description) && brand.description.Length > 1000)
            {
                ModelState.AddModelError("description", "Mô tả không được dài quá 1000 ký tự.");
                Debug.WriteLine("Validation failed: Description is too long.");
                flag = 1;
            }

            if (brand.status < 0 || brand.status > 2)
            {
                ModelState.AddModelError("status", "Trạng thái phải là 0, 1 hoặc 2.");
                Debug.WriteLine("Validation failed: Invalid status.");
                flag = 1;
            }

            

            if (flag == 1)
            {
                Debug.WriteLine("Validation failed: Returning to the view with validation errors.");
                return View(brand);
            }

            try
            {
                // Log before generating slug
                Debug.WriteLine("Generating slug from the brand name.");

                // Generate a slug from the brand name
                brand.slug = GenerateSlug(brand.name);
                Debug.WriteLine($"Generated slug: {brand.slug}");

                // Handle the image upload
               
 

                // Set additional properties
                brand.created_at = DateTime.Now;
               // brand.created_by = "admin"; // Replace with your logic to get the current user ID
                brand.status = 1;

                // Add to database
                db.Brands.Add(brand);
                db.SaveChanges();
                Debug.WriteLine("Brand added to database successfully.");

                TempData["SuccessMessage"] = "Brand added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the brand: " + ex.Message);
                Debug.WriteLine("Error occurred: " + ex.Message);
            }

            Debug.WriteLine("Returning to the view with the brand model.");
            return View(brand);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Brand brand, HttpPostedFileBase ImageUpload)
        {
            int flag = 0;

            // Basic validation for the brand fields
            if (string.IsNullOrEmpty(brand.name))
            {
                ModelState.AddModelError("name", "Tên thương hiệu không được để trống.");
                flag = 1;
            }
            else
            {
                var existingBrandWithSameName = db.Brands
                    .FirstOrDefault(b => b.name == brand.name && b.id != brand.id);

                if (existingBrandWithSameName != null)
                {
                    ModelState.AddModelError("name", "Tên thương hiệu đã tồn tại. Vui lòng chọn tên khác.");
                    flag = 1;
                }
            }

            if (!string.IsNullOrEmpty(brand.description) && brand.description.Length > 1000)
            {
                ModelState.AddModelError("description", "Mô tả không được dài quá 1000 ký tự.");
                flag = 1;
            }

            if (brand.status < 0 || brand.status > 2)
            {
                ModelState.AddModelError("status", "Trạng thái phải là 0, 1 hoặc 2.");
                flag = 1;
            }
 

            if (flag == 1)
            {
                return View("Edit", brand);
            }

            try
            {
                var existingBrand = db.Brands.Find(brand.id);
                if (existingBrand == null)
                {
                    TempData["ErrorMessage"] = "Brand not found.";
                    return RedirectToAction("Index");
                }

                // Update the brand details
                existingBrand.name = brand.name;
                existingBrand.slug = GenerateSlug(brand.name);
                existingBrand.description = brand.description;
                existingBrand.status = brand.status;
                existingBrand.updated_at = DateTime.Now;
              //  existingBrand.updated_by = "admin"; // Replace with your logic to get the current user ID

                // Handle the image upload if a new image is provided
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    // Create a new file name using the current timestamp
                    string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = existingBrand.slug + "_" + timeStamp + Path.GetExtension(ImageUpload.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Brands/"), fileName);

                    // Delete the old image file if a new image is uploaded
                    if (!string.IsNullOrEmpty(existingBrand.image))
                    {
                        string oldPath = Path.Combine(Server.MapPath("~/Content/Images/Brands/"), existingBrand.image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // Save the new image
                    ImageUpload.SaveAs(path);
                    existingBrand.image = fileName;
                }

                db.Entry(existingBrand).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Brand updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the brand: " + ex.Message);
            }

            return View(brand);
        }



        public ActionResult Status(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            brand.status = (brand.status == 2) ? 1 : 2;
            brand.updated_at = DateTime.Now;
           // brand.updated_by = "admin"; // Replace with your logic to get the current user ID

            db.Entry(brand).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            brand.status = 0;
            brand.updated_at = DateTime.Now;
          //  brand.updated_by = "admin"; // Replace with your logic to get the current user ID

            db.Entry(brand).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Restore(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            brand.status = 2;
            brand.updated_at = DateTime.Now;
          //  brand.updated_by = "admin"; // Replace with your logic to get the current user ID

            db.Entry(brand).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Trash");
        }

        public ActionResult Destroy(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            db.Brands.Remove(brand);
            db.SaveChanges();

            return RedirectToAction("Trash");
        }

        private string GenerateSlug(string name)
        {
            string slug = name.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = Regex.Replace(slug, @"\s", "-");
            return slug;
        }
    }
}

