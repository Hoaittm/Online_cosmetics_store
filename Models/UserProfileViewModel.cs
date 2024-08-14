using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranThiMinhHoai_2122110262.Models
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<OrderWithDetailsModel> Orders { get; set; }
    }

    public class OrderWithDetailsModel
    {
        public Order Order { get; set; }
        public List<Orderdetail> OrderDetails { get; set; }
    }
}