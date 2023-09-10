using System.Web;
using System.Web.Mvc;

namespace ExcelUpload_into_Employee
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
