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
    
    public partial class Post
    {
        public long id { get; set; }
        public Nullable<int> topic_id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string detail { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public int created_by { get; set; }
        public Nullable<int> updated_by { get; set; }
        public byte status { get; set; }
    }
}
