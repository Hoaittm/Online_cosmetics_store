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
    
    public partial class Banner
    {
        public long id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string link { get; set; }
        public int sort_order { get; set; }
        public string position { get; set; }
        public string description { get; set; }
        public int created_by { get; set; }
        public Nullable<int> updated_by { get; set; }
        public byte status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}
