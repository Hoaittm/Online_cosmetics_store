//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TranThiMinhHoai_2122110262.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Orderdetail
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public decimal price { get; set; }
        public int qty { get; set; }
        public decimal discount { get; set; }
        public decimal amount { get; set; }
        public System.DateTime updated_at { get; set; }
        public System.DateTime created_at { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
