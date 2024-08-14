using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Controllers
{
    public class HomeController : Controller
    {
           TranThiMinhHoaiEntities1 objTranThiMinhHoaiEntities = new TranThiMinhHoaiEntities1();
        public ActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();

            objHomeModel.ListCategory = objTranThiMinhHoaiEntities.Categories.ToList();
            objHomeModel.ListProduct = objTranThiMinhHoaiEntities.Products.ToList();
            objHomeModel.ListBanner = objTranThiMinhHoaiEntities.Banners.ToList();
            objHomeModel.ListBrand = objTranThiMinhHoaiEntities.Brands.Take(4).ToList();
            return View(objHomeModel);
        } 

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Detail(int id)
        {
            var lstPro = objTranThiMinhHoaiEntities.Products.ToList();
            Product product = null;
            foreach (Product objProduct in lstPro)
            {
                if (objProduct.id == id)
                {
                    product = objProduct;
                }
            }
            return View(product);
        }
    }
}