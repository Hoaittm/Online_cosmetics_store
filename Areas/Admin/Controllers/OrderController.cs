using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using TranThiMinhHoai_2122110262.Models;

namespace TranThiMinhHoai_2122110262.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private TranThiMinhHoaiEntities1 db = new TranThiMinhHoaiEntities1();

        // GET: Admin/Order
        public ActionResult Index()
        {
            // Lấy danh sách các đơn hàng từ cơ sở dữ liệu, sắp xếp theo ngày tạo
            var orders = db.Orders
                .OrderByDescending(o => o.CreatedOnUtc)
                .ToList();

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(orders);
        }

        // GET: Admin/Order/Details/{id}
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Lấy đơn hàng theo ID
            var order = db.Orders.FirstOrDefault(o => o.id == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách chi tiết đơn hàng
            var orderDetails = db.Orderdetails.Where(od => od.order_id == id).ToList();

            // Tạo view model để chứa cả thông tin đơn hàng và chi tiết đơn hàng
            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderDetails = orderDetails
            };

            return View(viewModel);
        }

        // POST: Admin/Order/UpdateStatus
        [HttpPost]
        public ActionResult UpdateStatus(int id, int status)
        {
            // Lấy đơn hàng theo ID
            var order = db.Orders.FirstOrDefault(o => o.id == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Cập nhật trạng thái đơn hàng
            order.status = status;

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            // Thông báo thành công
            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công.";

            // Quay lại trang chi tiết đơn hàng
            return RedirectToAction("Details", new { id = id });
        }
    }

    // ViewModel để chứa thông tin đơn hàng và chi tiết đơn hàng
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public List<Orderdetail> OrderDetails { get; set; }
    }
}
