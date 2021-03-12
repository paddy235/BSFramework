using BSFramework.Application.Code;
using BSFramework.Application.Web.Areas.Works.Views.EduTrain;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Application.Busines.ToolManage;
using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Application.Busines.DrugManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.EmergencyManage;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class SearchController : MvcControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult Index(FormCollection fc)
        {
            var from = fc.Get("from");
            var to = fc.Get("to");
            var name = fc.Get("name");

            var dfrom = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var dto = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);

            var user = OperatorProvider.Provider.Current();
            var bll1 = new WorkmeetingBLL();
            var cnt1 = bll1.GetIndex(user.DeptId, dfrom, dto, name);
            ViewData["item2"] = cnt1;

            var bll2 = new ActivityBLL();
            var cnt2 = bll2.GetIndex(user.DeptId, dfrom, dto, name);
            ViewData["item3"] = cnt2;

            var bll3 = new ToolBorrowBLL();
            var cnt3 = bll3.GetList(user.UserId, user.DeptId, dfrom, dto, name);
            ViewData["item4"] = cnt3.Count();

            var bll4 = new DrugBLL();
            int cnt4 = bll4.GetOutListCount(user.DeptId, dfrom, dto, name);
            ViewData["item5"] = cnt4;

            int total = 0;
            var bll5 = new LllegalBLL();
            int cnt5 = bll5.GetList(user.DeptId, "", "", dfrom, dto, 1, 12, out total).Count();
            ViewData["item6"] = total;

            int total1 = 0;
            var bll6 = new NoticeBLL();
            int cnt6 = bll6.GetAllNotice(user.DeptId, dfrom, dto, name, 1, 12, out total1).Count();
            ViewData["item7"] = total1;

            int total2 = 0;
            var bll7 = new EmergencyBLL();
            int cnt7 = bll7.EmergencyReportGetPageList(user.DeptId, name, dfrom, dto, 1, 12, out total2).Count();
            ViewData["item8"] = total2;

            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["name"] = name;

            return View();
        }
    }
}
