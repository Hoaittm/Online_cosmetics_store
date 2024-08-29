using PagedList;
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
    public class ProductController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: Admin/Product
        //public ActionResult Index(int? page)
        //{
        //   var products = db.products
        //  .Where(p => p.status != 0)
        //  .OrderByDescending(p => p.created_at)
        //  .ToList();
        //    if (TempData["SuccessMessage"] != null)
        //    {
        //        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        //    }

        //    int pageSize = 8; // Số lượng sản phẩm hiển thị trên mỗi trang
        //    int pageNumber = (page ?? 1); // Nếu không có số trang thì mặc định là 1

        //    return View(products.ToPagedList(pageNumber, pageSize));
        //}
        public ActionResult Index(string searchString, int? page)
        {
            var products = db.Products
                .Where(p => p.status != 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.name.Contains(searchString) || p.description.Contains(searchString));
            }

            products = products.OrderByDescending(p => p.created_at);

            int pageSize = 8; // Số lượng sản phẩm hiển thị trên mỗi trang
            int pageNumber = (page ?? 1); // Nếu không có số trang thì mặc định là 1

            ViewBag.SearchString = searchString; // Lưu lại chuỗi tìm kiếm để hiển thị trong form

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Trash()
        {
            var products = db.Products.Where(p => p.status == 0).ToList();
            return View(products);
        }



        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.Categories, "id", "name");
            ViewBag.brand_id = new SelectList(db.Brands, "id", "name");
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            int flag = 0;

            // Check for null or invalid values and set flags accordingly
            if (string.IsNullOrEmpty(product.name))
            {
                ModelState.AddModelError("name", "Tên sản phẩm không được để trống.");
                flag = 1;
            }
            else
            {
                // Check if the product name already exists
                var existingProductWithSameName = db.Products.FirstOrDefault(p => p.name == product.name);

                if (existingProductWithSameName != null)
                {
                    ModelState.AddModelError("name", "Tên sản phẩm đã tồn tại. Vui lòng chọn tên khác.");
                    flag = 1;
                }
            }
            if (product.category_id <= 0)
            {
                ModelState.AddModelError("category_id", "Danh mục không hợp lệ.");
                flag = 1;
            }
            if (product.brand_id <= 0)
            {
                ModelState.AddModelError("brand_id", "Thương hiệu không hợp lệ.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(product.detail))
            {
                ModelState.AddModelError("detail", "Chi tiết không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(product.description))
            {
                ModelState.AddModelError("description", "Mô tả không được để trống.");
                flag = 1;
            }
            if (product.price <= 0)
            {
                ModelState.AddModelError("price", "Giá phải lớn hơn 0.");
                flag = 1;
            }
            if (product.pricesale >= product.price)
            {
                ModelState.AddModelError("pricesale", "Giá giam phai nho hon gia ban dau.");
                flag = 1;
            }
            if (product.qty <= 0 || product.qty == null)
            {
                ModelState.AddModelError("qtyy", "Số lượng không hop le.");
                flag = 1;
            }
            if (product.ImageUpload == null || product.ImageUpload.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUpload", "Vui lòng chọn tệp hình ảnh.");
                flag = 1;
            }
            else
            {
                string extension = Path.GetExtension(product.ImageUpload.FileName).ToLower();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif" && extension != ".webp")
                {
                    ModelState.AddModelError("ImageUpload", "Chỉ chấp nhận các tệp hình ảnh có đuôi là .jpg, .jpeg, .png, .gif, .webp.");
                    flag = 1;
                }
            }

            if (flag == 1)
            {
                // If there are any errors, re-populate the ViewBag and return to the view
                ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
                ViewBag.brand_id = new SelectList(db.Brands, "id", "name", product.brand_id);
                return View(product);
            }

            try
            {
                // Generate a slug from the product name
                if (!string.IsNullOrEmpty(product.name))
                {
                    product.slug = GenerateSlug(product.name);
                }

                // Handle the image upload
                if (product.ImageUpload != null && product.ImageUpload.ContentLength > 0)
                {
                    string extension = Path.GetExtension(product.ImageUpload.FileName);
                    string file_name = product.slug + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/product/"), file_name);

                    if (System.IO.File.Exists(path))
                    {
                        // If the file exists, use the existing image
                        product.image = file_name;
                    }
                    else
                    {
                        // If the file does not exist, save the new image
                        product.image = file_name;
                        product.ImageUpload.SaveAs(path);
                    }
                }

                product.created_at = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the product: " + ex.Message);
            }

            // Re-populate the ViewBag in case of failure
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            ViewBag.brand_id = new SelectList(db.Brands, "id", "name", product.brand_id);
            return View(product);
        }




        private string GenerateSlug(string name)
        {
            // Convert to lower case
            string slug = name.ToLower();

            // Remove invalid characters, replace spaces with hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s", "-");

            return slug;
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            // Retrieve the product by ID
            var product = db.Products.Where(n => n.id == id).FirstOrDefault();
            // If the product is not found, return a 404 error
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryName = db.Categories.FirstOrDefault(c => c.id == product.category_id)?.name;
            ViewBag.BrandName = db.Brands.FirstOrDefault(b => b.id == product.brand_id)?.name;
            // Pass the product object to the view
            return View(product);
        }


        [HttpPost]
        public ActionResult Store(Product product, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                product.slug = GenerateSlug(product.name);
                if (image != null)
                {
                    var ext = Path.GetExtension(image.FileName);
                    if (new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" }.Contains(ext.ToLower()))
                    {
                        var fileName = product.slug + ext;
                        var path = Path.Combine(Server.MapPath("~/images/products"), fileName);
                        image.SaveAs(path);
                        product.image = fileName;
                    }
                }

                product.created_at = DateTime.Now;
                product.created_by = 1; // Replace with your logic to get the current user ID
                product.status = product.status;

                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("Create");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var brands = db.Brands.Where(b => b.status != 0).ToList();
            var categories = db.Categories.Where(c => c.status != 0).ToList();

            ViewBag.brandList = new SelectList(brands, "id", "name", product.brand_id);
            ViewBag.categoryList = new SelectList(categories, "id", "name", product.category_id);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Product product, HttpPostedFileBase ImageUpload)
        {
            int flag = 0;

            // Basic validation for the product fields
            if (string.IsNullOrEmpty(product.name))
            {
                ModelState.AddModelError("name", "Tên sản phẩm không được để trống.");
                flag = 1;
            }
            else
            {
                // Check if the product name already exists, excluding the current product
                var existingProductWithSameName = db.Products
                    .FirstOrDefault(p => p.name == product.name && p.id != product.id);

                if (existingProductWithSameName != null)
                {
                    ModelState.AddModelError("name", "Tên sản phẩm đã tồn tại. Vui lòng chọn tên khác.");
                    flag = 1;
                }
            }
            if (product.category_id <= 0)
            {
                ModelState.AddModelError("category_id", "Danh mục không hợp lệ.");
                flag = 1;
            }
            if (product.brand_id <= 0)
            {
                ModelState.AddModelError("brand_id", "Thương hiệu không hợp lệ.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(product.detail))
            {
                ModelState.AddModelError("detail", "Chi tiết không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(product.description))
            {
                ModelState.AddModelError("description", "Mô tả không được để trống.");
                flag = 1;
            }
            if (product.price <= 0)
            {
                ModelState.AddModelError("price", "Giá phải lớn hơn 0.");
                flag = 1;
            }
            if (product.pricesale >= product.price)
            {
                ModelState.AddModelError("pricesale", "Giá giảm phải nhỏ hơn giá ban đầu.");
                flag = 1;
            }
            if (product.qty <= 0 || product.qty == null)
            {
                ModelState.AddModelError("qty", "Số lượng không hợp lệ.");
                flag = 1;
            }

            // Validate the image upload if a new image is provided
            if (ImageUpload != null && ImageUpload.ContentLength > 0)
            {
                var ext = Path.GetExtension(ImageUpload.FileName).ToLower();
                if (!new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" }.Contains(ext))
                {
                    ModelState.AddModelError("ImageUpload", "Chỉ chấp nhận các tệp hình ảnh có đuôi là .jpg, .jpeg, .png, .gif, .webp.");
                    flag = 1;
                }
            }

            if (flag == 1)
            {
                // Reload the brands and categories for the dropdown lists
                ViewBag.brandList = new SelectList(db.Brands.Where(b => b.status != 0).ToList(), "id", "name", product.brand_id);
                ViewBag.categoryList = new SelectList(db.Categories.Where(c => c.status != 0).ToList(), "id", "name", product.category_id);
                return View("Edit", product);
            }

            try
            {
                var existingProduct = db.Products.Find(product.id);
                if (existingProduct == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                // Update the product details
                existingProduct.name = product.name;
                existingProduct.slug = GenerateSlug(product.name);
                existingProduct.description = product.description;
                existingProduct.detail = product.detail;
                existingProduct.category_id = product.category_id;
                existingProduct.brand_id = product.brand_id;
                existingProduct.price = product.price;
                existingProduct.pricesale = product.pricesale;
                existingProduct.qty = product.qty;
                existingProduct.status = product.status;
                existingProduct.updated_at = DateTime.Now;
                existingProduct.updated_by = 1; // Replace with your logic to get the current user ID

                // Handle the image upload if a new image is provided
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    var ext = Path.GetExtension(ImageUpload.FileName).ToLower();
                    if (new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" }.Contains(ext))
                    {
                        // Generate a new unique file name based on the current time
                        string fileName = DateTime.Now.Ticks.ToString() + ext;
                        string path = Path.Combine(Server.MapPath("~/Content/images/product/"), fileName);

                        // Delete the old image file if it exists
                        if (!string.IsNullOrEmpty(existingProduct.image))
                        {
                            string oldPath = Path.Combine(Server.MapPath("~/Content/images/product/"), existingProduct.image);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Save the new image
                        ImageUpload.SaveAs(path);
                        Debug.WriteLine("New image saved successfully.");

                        // Update the image path in the database
                        existingProduct.image = fileName;
                    }
                }

                db.Entry(existingProduct).State = EntityState.Modified;
                db.SaveChanges();

                Debug.WriteLine("Product updated successfully in the database.");

                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the product: " + ex.Message);
                Debug.WriteLine("Error while updating product: " + ex.Message);
            }

            return View(product);
        }






        public ActionResult Status(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            product.status = (byte)((product.status == 2) ? 1 : 2);
            product.updated_at = DateTime.Now;
            product.updated_by = 1; // Replace with your logic to get the current user ID

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            product.status = 0;
            product.updated_at = DateTime.Now;
            product.updated_by = 1; // Replace with your logic to get the current user ID

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Restore(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            product.status = 2;
            product.updated_at = DateTime.Now;
            product.updated_by = 1; // Replace with your logic to get the current user ID

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Trash");
        }

        public ActionResult Destroy(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Trash");
        }


    }
}
