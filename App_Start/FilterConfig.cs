using System.Web;
using System.Web.Mvc;

namespace TranThiMinhHoai_2122110262
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
