using BSFramework.Application.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 描 述：电子签章
    /// </summary>
    public class SignetController : MvcControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

