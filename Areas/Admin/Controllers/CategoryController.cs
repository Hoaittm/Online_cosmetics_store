using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: Admin/Category
        public ActionResult Index()
        {
            var categories = db.Categories
                .Where(c => c.status != 0)
                .OrderByDescending(c => c.created_at)
                .ToList();

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(categories);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            // Retrieve the product by ID
            var category = db.Categories.Where(n => n.id == id).FirstOrDefault();
            // If the product is not found, return a 404 error
            if (category == null)
            {
                return HttpNotFound();
            }

            // Pass the product object to the view
            return View(category);
        }
        public ActionResult Trash()
        {
            var categories = db.Categories.Where(c => c.status == 0).ToList();
            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            int flag = 0;

            // Validate the name
            if (string.IsNullOrEmpty(category.name))
            {
                ModelState.AddModelError("name", "Tên danh mục không được để trống.");
                flag = 1;
            }
            else
            {
                // Check if the product name already exists
                var existingProductWithSameName = db.Categories.FirstOrDefault(p => p.name == category.name);

                if (existingProductWithSameName != null)
                {
                    ModelState.AddModelError("name", "Tên danh muc đã tồn tại. Vui lòng chọn tên khác.");
                    flag = 1;
                }
            }
            if (string.IsNullOrEmpty(category.description))
            {
                ModelState.AddModelError("description", "Mo ta không được để trống.");
                flag = 1;
            }
            // Validate the image upload
            if (category.ImageUpload != null && category.ImageUpload.ContentLength > 0)
            {
                string extension = Path.GetExtension(category.ImageUpload.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(extension))
                {
                    ModelState.AddModelError("ImageUpload", "Chỉ chấp nhận các tệp hình ảnh có đuôi là .jpg, .jpeg, .png, .gif, .webp.");
                    flag = 1;
                }
            }
            else
            {
                ModelState.AddModelError("ImageUpload", "Vui lòng chọn tệp hình ảnh.");
                flag = 1;
            }

            // If there are validation errors, return to the view
            if (flag == 1)
            {
                return View(category);
            }

            try
            {
                // Generate a slug from the category name
                if (!string.IsNullOrEmpty(category.name))
                {
                    category.slug = GenerateSlug(category.name);
                }

                // Handle the image upload
                if (category.ImageUpload != null && category.ImageUpload.ContentLength > 0)
                {
                    string extension = Path.GetExtension(category.ImageUpload.FileName).ToLower();
                    string fileName = category.slug + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Category/"), fileName);

                    // Save the image if it doesn't already exist
                    if (!System.IO.File.Exists(path))
                    {
                        category.ImageUpload.SaveAs(path);
                    }

                    category.image = fileName;
                }

                // Set the creation date
                category.created_at = DateTime.Now;

                // Save the category to the database
                db.Categories.Add(category);
                db.SaveChanges();

                // Set a success message and redirect to the index
                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle any errors that occurred during save
                ModelState.AddModelError("", "An error occurred while saving the category: " + ex.Message);
            }

            // If we get here, something went wrong
            return View(category);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction("Index");
            }

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category, HttpPostedFileBase ImageUpload)
        {
            int flag = 0;

            // Validate the name
            if (string.IsNullOrEmpty(category.name))
            {
                ModelState.AddModelError("name", "Tên danh mục không được để trống.");
                flag = 1;
            }
            else
            {
                // Check if the product name already exists, excluding the current product
                var existingProductWithSameName = db.Categories
                    .FirstOrDefault(p => p.name == category.name && p.id != category.id);

                if (existingProductWithSameName != null)
                {
                    ModelState.AddModelError("name", "Tên danh muc đã tồn tại. Vui lòng chọn tên khác.");
                    flag = 1;
                }
            }

            if (string.IsNullOrEmpty(category.description))
            {
                ModelState.AddModelError("description", "Mô tả không được để trống.");
                flag = 1;
            }

            // Validate the image upload
            if (ImageUpload != null && ImageUpload.ContentLength > 0)
            {
                string extension = Path.GetExtension(ImageUpload.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(extension))
                {
                    ModelState.AddModelError("ImageUpload", "Chỉ chấp nhận các tệp hình ảnh có đuôi là .jpg, .jpeg, .png, .gif, .webp.");
                    flag = 1;
                }
            }

            // If there are validation errors, return to the view
            if (flag == 1)
            {
                return View(category);
            }

            try
            {
                var existingCategory = db.Categories.Find(category.id);
                if (existingCategory == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction("Index");
                }

                // Update the category details
                existingCategory.name = category.name;
                existingCategory.slug = GenerateSlug(category.name);
                existingCategory.description = category.description;
                existingCategory.status = category.status;
                existingCategory.updated_at = DateTime.Now;

                // Handle the image upload
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    string extension = Path.GetExtension(ImageUpload.FileName).ToLower();
                    string fileName = existingCategory.slug + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Category/"), fileName);

                    // Log paths and actions
                    Debug.WriteLine($"New image path: {path}");

                    // Delete the old image file if a new image is uploaded
                    if (!string.IsNullOrEmpty(existingCategory.image))
                    {
                        string oldPath = Path.Combine(Server.MapPath("~/Content/Images/Category/"), existingCategory.image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            Debug.WriteLine($"Deleting old image: {oldPath}");
                            System.IO.File.Delete(oldPath);
                        }
                        else
                        {
                            Debug.WriteLine("Old image not found or already deleted.");
                        }
                    }

                    // Save the new image
                    ImageUpload.SaveAs(path);
                    Debug.WriteLine("New image saved successfully.");

                    // Update the image path in the database
                    existingCategory.image = fileName;
                }
                // If no new image is uploaded, keep the existing image
                else
                {
                    Debug.WriteLine("No new image uploaded, keeping the existing image.");
                }

                db.Entry(existingCategory).State = EntityState.Modified;
                db.SaveChanges();
                Debug.WriteLine("Category updated successfully in the database.");

                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category: " + ex.Message);
                Debug.WriteLine("Error updating category: " + ex.Message);
            }

            return View(category);
        }


        public ActionResult Status(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            category.status = (byte)((category.status == 2) ? 1 : 2);
            category.updated_at = DateTime.Now;

            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            category.status = 0;
            category.updated_at = DateTime.Now;

            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Restore(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            category.status = 2;
            category.updated_at = DateTime.Now;

            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Trash");
        }

        public ActionResult Destroy(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Trash");
        }

        private string GenerateSlug(string name)
        {
            string slug = name.ToLower();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s", "-");

            return slug;
        }
    }
}
