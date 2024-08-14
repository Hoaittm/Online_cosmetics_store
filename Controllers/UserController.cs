using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;
using TranThiMinhHoai_2122110262.Helpers;
using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
namespace TranThiMinhHoai_2122110262.Controllers
{
    public class UserController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: User
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Email,Password")] User model)
        {

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.email) || string.IsNullOrEmpty(model.password))
                {
                    if (string.IsNullOrEmpty(model.email))
                    {
                        ModelState.AddModelError("Email", "Email không được bỏ trống.");
                    }

                    if (string.IsNullOrEmpty(model.password))
                    {
                        ModelState.AddModelError("Mật khẩu", "Mật khẩu không được bỏ trống.");
                    }

                    return View(); // Return the view with validation errors
                }
                var user = db.Users.SingleOrDefault(u => u.email == model.email);
                if (user != null && PasswordHelper.VerifyMd5Hash(model.password, user.password))
                {
                    // Successfully authenticated, set up session or authentication cookie
                    Session["UserId"] = user.id;
                    Session["City"] = user.city;
                    Session["Country"] = user.country;
                    Session["Email"] = user.email;
                    Session["Phone"] = user.phone;
                    Session["Name"] = user.name;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Sai", "Email hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult UserOrder(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var orders = db.Orders.Where(o => o.userid == id).ToList();
            var ordersWithDetails = orders.Select(o => new OrderWithDetailsModel
            {
                Order = o,
                OrderDetails = db.Orderdetails.Where(od => od.order_id == o.id).ToList()
            }).ToList();
            var viewModel = new UserProfileViewModel
            {
                User = user,
                Orders = ordersWithDetails
            };
            return View(viewModel);
        }
        public new ActionResult Profile(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var orders = db.Orders.Where(o => o.userid == id).ToList();
            var ordersWithDetails = orders.Select(o => new OrderWithDetailsModel
            {
                Order = o,
                OrderDetails = db.Orderdetails.Where(od => od.order_id == o.id).ToList()
            }).ToList();
            var viewModel = new UserProfileViewModel
            {
                User = user,
                Orders = ordersWithDetails
            };
            return View(viewModel);
        }
        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "id,name,email,phone,gender,city,country,password,confirm_password")] User user)
        {
            int flag = 0;

            if (string.IsNullOrEmpty(user.name))
            {
                ModelState.AddModelError("name", "Họ tên không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.email))
            {
                ModelState.AddModelError("email", "Email không được để trống.");
                flag = 1;
            }
            else
            {
                // Check if the email already exists
                var existingUser = db.Users.FirstOrDefault(u => u.email == user.email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("email", "Email đã tồn tại. Vui lòng sử dụng email khác.");
                    flag = 1;
                }
            }
            if (string.IsNullOrEmpty(user.phone))
            {
                ModelState.AddModelError("phone", "Số điện thoại không được để trống.");
                flag = 1;
            }
            else if (!user.phone.All(char.IsDigit))
            {
                ModelState.AddModelError("phone", "Số điện thoại chỉ được chứa chữ số.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.city))
            {
                ModelState.AddModelError("city", "Thành phố không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.country))
            {
                ModelState.AddModelError("country", "Quốc gia không được để trống.");
                flag = 1;
            }
            if (string.IsNullOrEmpty(user.password))
            {
                ModelState.AddModelError("password", "Mật khẩu không được để trống.");
                flag = 1;
            }
            if (!string.Equals(user.password, Request.Form["confirm_password"]))
            {
                ModelState.AddModelError("confirm_password", "Mật khẩu và nhập lại mật khẩu không khớp.");
                flag = 1;
            }

            if (flag == 1)
            {
                return View(user);
            }

            // Hash the password before saving
            user.password = PasswordHelper.GetMd5Hash(user.password);

            db.Users.Add(user);
            db.SaveChanges();

            // Set success message
            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";

            return RedirectToAction("Login");
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,email,gender,city,country,password")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,email,gender,city,country,password")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
