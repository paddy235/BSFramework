using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class ReportController : MvcControllerBase
    {
        //
        // GET: /Works/Report/

        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData(int page, int rows, DateTime? start, DateTime? end, string key, string category)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new ReportBLL();
            var filebll = new FileInfoBLL();
            var total = 0;
            var data = bll.GetReports("全部", category, null, start, end, key, rows, page, out total, user.UserId, null);
            //var data = bll.GetReports(null, category, null, start, end, key, rows, page, out total, null, null);
            foreach (var item in data)
            {
                if (item.EndTime.HasValue) item.EndTime = item.EndTime.Value.AddDays(-1);
                var fileinfo = filebll.GetFilesByRecIdNew(item.ReportId.ToString());
                if (fileinfo != null && fileinfo.Count > 0) item.FilePath = Url.Content(fileinfo[0].FilePath);
            }

            return Json(new { rows = data, total = Math.Ceiling((double)total / rows), records = total });
        }

        public ViewResult Detail1(string id)
        {
            var bll = new ReportBLL();
            var data = bll.GetDetail(id);
            return View(data);
        }

        public ViewResult Detail2(string id)
        {
            var bll = new ReportBLL();
            var data = bll.GetDetail(id);
            return View(data);
        }

        public ViewResult Settings()
        {
            return View();
        }

        public ViewResult SettingDetail()
        {
            var bll = new ReportBLL();
            var settings = bll.GetSettings();
            var setting1 = settings.Find(x => x.SettingName == "周工作总结");
            if (setting1 != null)
            {
                ViewBag.date1 = setting1.Start;
                ViewBag.start1 = setting1.StartTime.ToString("H:mm:ss");
            }
            var setting2 = settings.Find(x => x.SettingName == "月工作总结");
            if (setting2 != null)
            {
                ViewBag.date2 = setting2.Start;
                ViewBag.start2 = setting2.StartTime.ToString("H:mm:ss");
            }
            return View();
        }

        [HttpPost]
        public JsonResult SettingDetail(FormCollection values)
        {
            var date1 = values.Get("date1");
            var date2 = values.Get("date2");
            var time1 = values.Get("start1");
            var time2 = values.Get("start2");

            var setting1 = new ReportSettingEntity() { SettingName = "周工作总结", Start = int.Parse(date1), StartTime = DateTime.Parse(time1), End = int.Parse(date1), EndTime = DateTime.Parse(time1) };
            var setting2 = new ReportSettingEntity() { SettingName = "月工作总结", Start = int.Parse(date2), StartTime = DateTime.Parse(time2), End = int.Parse(date2), EndTime = DateTime.Parse(time2) };

            var bll = new ReportBLL();
            bll.Setting(new List<ReportSettingEntity>() { setting1, setting2 });

            return Json(new AjaxResult { type = ResultType.success, message = "保存成功" });
        }

        public JsonResult GetDetail(string id)
        {
            var bll = new ReportBLL();
            var data = bll.GetDetail(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
