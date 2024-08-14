using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;
namespace TranThiMinhHoai_2122110262.Controllers
{
    public class ProductController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: Product
        public ActionResult Index(string sortOrder, int? page)
        {
            var products = db.Products.AsQueryable();

            // Apply sorting based on sortOrder
            switch (sortOrder)
            {
                case "latest":
                    products = products.OrderByDescending(p => p.created_at);
                    break;
                case "oldest":
                    products = products.OrderBy(p => p.created_at);
                    break;
                case "priceAsc":
                    products = products.OrderBy(p => p.pricesale ?? p.price);
                    break;
                case "priceDesc":
                    products = products.OrderByDescending(p => p.pricesale ?? p.price);
                    break;
                default:
                    products = products.OrderByDescending(p => p.created_at); // Default to latest items
                    break;
            }

            ViewBag.SortOrder = sortOrder;
            int pageSize = 8; // Number of items per page
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public ActionResult Search(string searchType, string query, string sortOrder)
        {
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                switch (searchType)
                {
                    case "Product":
                        products = products.Where(p => p.name.Contains(query));
                        break;
                    case "Category":
                        products = from p in db.Products
                                   join c in db.Categories on (int)p.category_id equals c.id
                                   where c.name.Contains(query)
                                   select p;
                        break;
                    case "Brand":
                        products = from p in db.Products
                                   join b in db.Brands on p.brand_id equals b.id
                                   where b.name.Contains(query)
                                   select p;
                        break;
                    default:
                        break;
                }
            }


            // Apply sorting based on sortOrder
            switch (sortOrder)
            {
                case "latest":
                    products = products.OrderByDescending(p => p.created_at);
                    break;
                case "oldest":
                    products = products.OrderBy(p => p.created_at);
                    break;
                case "priceAsc":
                    products = products.OrderBy(p => p.pricesale ?? p.price); // Sort by price or sale price if available
                    break;
                case "priceDesc":
                    products = products.OrderByDescending(p => p.pricesale ?? p.price); // Sort by price or sale price if available
                    break;
                default:
                    products = products.OrderByDescending(p => p.created_at); // Default to latest items
                    break;
            }

            ViewBag.SearchType = searchType;
            ViewBag.Query = query;
            ViewBag.SortOrder = sortOrder;

            return View(products.ToList());
        }
    }
}