using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranThiMinhHoai_2122110262.Models
{
    public class HomeModel
    {
        public List<Product> ListProduct { get; set; }
        public List<Category> ListCategory { get; set; }
        public List<Banner> ListBanner { get; set; }
        public List<Brand> ListBrand { get; set; }
    }
}