using BSFramework.Application.Busines.AttendManage;
using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class AttendController : Controller
    {
        //
        // GET: /Works/Attend/
        private AttendBLL bll = new AttendBLL();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData()
        {
            string deptid = OperatorProvider.Provider.Current().DeptId;
            return Json(new { rows = bll.GetCount(deptid,DateTime.Now) });
        }

    }
}
