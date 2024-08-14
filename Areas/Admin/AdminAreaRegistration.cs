using System.Web.Mvc;

namespace TranThiMinhHoai_2122110262.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                   new[] { "TranThiMinhHoai_2122110262.Areas.Admin.Controllers" }
            );
        }
    }
}