using Bst.Fx.Uploading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bst.Fx.IScheduler;
using Bst.Fx.Scheduler;
using Bst.Fx.SchedulerEntities;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    public class SchedulerController : Controller
    {
        //
        // GET: /SystemManage/Scheduler/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取计划任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetData()
        {
            ISchedulerTask taskbll = new SchedulerTask();
            var data = taskbll.GetTasks();

            return Json(new { rows = data, total = data.Count });
        }

        [HttpPost]
        public JsonResult Execute(SchedulerTaskEntity task)
        {
            var success = true;
            var message = string.Empty;
            ISchedulerTask taskbll = new SchedulerTask();
            try
            {
                taskbll.Execute(task.TaskName);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            return Json(new { success, message });
        }

        [HttpPost]
        public JsonResult Enable(SchedulerTaskEntity task)
        {
            var success = true;
            var message = string.Empty;
            ISchedulerTask taskbll = new SchedulerTask();
            try
            {
                taskbll.Enable(task);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            return Json(new { success, message });
        }

        [HttpPost]
        public JsonResult Disable(SchedulerTaskEntity task)
        {
            var success = true;
            var message = string.Empty;
            ISchedulerTask taskbll = new SchedulerTask();
            try
            {
                taskbll.Disable(task);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            return Json(new { success, message });
        }

        public ViewResult Edit(string id)
        {
            ISchedulerTask taskbll = new SchedulerTask();
            var list = new List<SelectListItem>() { new SelectListItem() { Value = "天", Text = "天" }, new SelectListItem() { Value = "周", Text = "周" }, new SelectListItem() { Value = "月", Text = "月" } };
            ViewData["Category"] = list;
            var data = taskbll.GetDetail(id);
            ViewBag.value1 = Newtonsoft.Json.JsonConvert.SerializeObject(data.DayOfWeek);
            ViewBag.value2 = Newtonsoft.Json.JsonConvert.SerializeObject(data.DaysOfMonth);
            ViewBag.value3 = Newtonsoft.Json.JsonConvert.SerializeObject(data.MonthsOfYear);
            list.Find(x => x.Value == data.Category).Selected = true;
            return View(data);
        }

        [HttpPost]
        public JsonResult Edit(SchedulerTaskEntity model)
        {
            var success = true;
            var message = "保存成功！";
            ISchedulerTask taskbll = new SchedulerTask();

            try
            {
                taskbll.Edit(model);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }
    }
}
