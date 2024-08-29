using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: Admin/User
        public ActionResult Index()
        {
            // Retrieve the list of users from the database
            var users = db.Users.ToList();

            // Return the view with the list of users
            return View(users);
        }

        // GET: Admin/User/Details/{id}
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Retrieve the user by ID
            var user = db.Users.FirstOrDefault(u => u.id == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the list of orders associated with the user
            var orders = db.Orders.Where(o => o.userid == id).ToList();

            // Create the ViewModel to hold both user information and their orders
            var viewModel = new UserOrderDetailsViewModel
            {
                User = user,
                Orders = orders
            };

            // Return the view with the ViewModel
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }

    // ViewModel to hold user information and their orders
    public class UserOrderDetailsViewModel
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }
    }

}