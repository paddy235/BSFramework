using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 部门首页
    /// </summary>
    [AllowAnonymous]
    public class DepartmentHomeController : Controller
    {
        public JsonResult GetData2(string userid, string deptid, string category)
        {
            var dept = new DepartmentBLL().GetAuthorizationDepartment(deptid);
            var depts = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, "班组");
            var from = DateTime.Today;
            var to = from.AddDays(1);
            if (category == "month")
            {
                from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                to = from.AddMonths(1);
            }

            var data = new HumanDangerTrainingBLL().GetData2(userid, depts.Select(x => x.DepartmentId).ToArray(), from, to);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDynamic(string userid)
        {
            var data = new EdActivityBLL().GetIndexDay(userid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivity(string userid, string type)
        {
            var data = new EdActivityBLL().GetAqxxrTotal(type, userid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducation(string userid)
        {
            var data = new EducationBLL().GetMonthTotal(userid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeetings(string deptid)
        {
            var dept = new DepartmentBLL().GetAuthorizationDepartment(deptid);
            var depts = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, "班组");
            var date = DateTime.Today;
            var workorder = new WorkOrderBLL();
            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            var num4 = 0;
            var workmeetingbll = new WorkmeetingBLL();
            var d = new DateTime(date.Year, date.Month, 1);
            var list = new List<string>();
            var meetings1 = new List<MeetingModel>();
            var meetings2 = new List<MeetingModel>();
            while (d <= date)
            {
                foreach (var item in depts)
                {
                    var ss = workorder.GetWorkOrderTotal(d, item.DepartmentId);
                    if (ss[0] != "无")
                    {
                        if (d == date)
                            num1++;
                        var meeting = workmeetingbll.HasMeeting(item.DepartmentId, d);
                        if (string.IsNullOrEmpty(meeting.MeetingId) || string.IsNullOrEmpty(meeting.OtherMeetingId))
                        {
                            list.Add(item.DepartmentId);
                            if (d == date)
                                num2++;
                            num4++;
                            meetings2.Add(new MeetingModel() { DeptName = item.FullName, MeetingDate = d, MeetingName = ss[0] });
                        }
                        if (d == date)
                        {
                            meetings1.Add(new MeetingModel() { DeptName = item.FullName, Meeting1 = string.IsNullOrEmpty(meeting.MeetingId) ? "未开" : "已开", Meeting2 = string.IsNullOrEmpty(meeting.OtherMeetingId) ? "未开" : "已开" });
                        }
                    }
                }
                d = d.AddDays(1);
            }
            num3 = list.Distinct().Count();


            return Json(new { num1, num2, num3, num4, meetings1, meetings2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDutyData(string deptid)
        {
            var from = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            if (DateTime.Today.DayOfWeek == 0)
                from = DateTime.Today.AddDays(-7);
            var to = from.AddDays(7);
            var bll = new WorkmeetingBLL();
            var date = from;
            var data = new List<object>();
            while (date < to)
            {
                var d = bll.GetDutyPerson(deptid, date);
                data.Add(new { Date = date.Date, Day = date.ToString("ddd"), Data = d == null ? string.Empty : string.Join(",", d.Select(x => x.UserName).Distinct()) });
                date = date.AddDays(1);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkApi()
        {
            var url = Config.GetValue("ErchtmsApiUrl");
            return Json(new { url }, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userid = user.UserId;
            ViewBag.deptid = user.DeptId;
            return View();
        }
    }
}