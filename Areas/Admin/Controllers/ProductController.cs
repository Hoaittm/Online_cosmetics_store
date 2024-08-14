using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        TranThiMinhHoaiEntities1 objTranThiMinhHoaiEntities = new TranThiMinhHoaiEntities1();
        // GET: Admin/Product
        public ActionResult Index()
        {
            var listProduct = objTranThiMinhHoaiEntities.Products.ToList();

            return View(listProduct);
        }
        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }
    }
}