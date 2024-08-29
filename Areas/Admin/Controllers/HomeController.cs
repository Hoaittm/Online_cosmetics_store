using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                ProductCount = db.Products.Count(),
                CategoryCount = db.Categories.Count(),
                BrandCount = db.Brands.Count(),
                UserCount = db.Users.Count(),
                OrderCount = db.Users.Count()
            };

            return View(viewModel);

        }
    }
}
