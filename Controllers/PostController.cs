using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Controllers
{
    public class PostController : Controller
    {
        TranThiMinhHoaiEntities1 objTranThiMinhHoaiEntities = new TranThiMinhHoaiEntities1();
        // GET: Post
        public ActionResult Index()
        {

            var lstPost = objTranThiMinhHoaiEntities.Posts.ToList();

            return View(lstPost);
        }
        public ActionResult Details(int id)
        {
            var post = objTranThiMinhHoaiEntities.Posts.FirstOrDefault(p => p.id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }
    }
}