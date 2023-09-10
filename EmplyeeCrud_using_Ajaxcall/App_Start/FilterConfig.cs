using System.Web;
using System.Web.Mvc;

namespace EmplyeeCrud_using_Ajaxcall
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
