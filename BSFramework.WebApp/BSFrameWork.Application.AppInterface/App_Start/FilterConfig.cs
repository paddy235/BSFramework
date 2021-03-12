using System.Web;
using System.Web.Mvc;

namespace BSFrameWork.Application.AppInterface
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandlerErrorAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}