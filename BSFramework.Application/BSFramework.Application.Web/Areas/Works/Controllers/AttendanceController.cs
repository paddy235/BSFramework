using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.Offices;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// 考勤
    /// </summary>
    public class AttendanceController : MvcControllerBase
    {
        public ViewResult Index(string year, string month)
        {
            if (string.IsNullOrEmpty(year)) year = DateTime.Now.Year.ToString();
            if (string.IsNullOrEmpty(month)) month = DateTime.Now.Month.ToString();

            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            var months = new List<dynamic>();
            //for (int i = 1; i <= 12; i++)
            //{
            //    months.Add(new { value = i.ToString(), text = (i + "月") });
            //}
            months.Add(new { value = 1, text = ("一月") });
            months.Add(new { value = 2, text = ("二月") });
            months.Add(new { value = 3, text = ("三月") });
            months.Add(new { value = 4, text = ("四月") });
            months.Add(new { value = 5, text = ("五月") });
            months.Add(new { value = 6, text = ("六月") });
            months.Add(new { value = 7, text = ("七月") });
            months.Add(new { value = 8, text = ("八月") });
            months.Add(new { value = 9, text = ("九月") });
            months.Add(new { value = 10, text = ("十月") });
            months.Add(new { value = 11, text = ("十一月") });
            months.Add(new { value = 12, text = ("十二月") });

            ViewData["year"] = new SelectList(years, "value", "text", year);
            ViewData["month"] = new SelectList(months, "value", "text", month);

            var user = OperatorProvider.Provider.Current();
            var bll = new WorkmeetingBLL();
            //var data = bll.GetAttendanceData2(user.DeptId, new DateTime(int.Parse(year), int.Parse(month), 1), new DateTime(int.Parse(year), int.Parse(month), 1).AddMonths(1).AddMinutes(-1));
            var data = bll.GetAttendanceData(user.DeptId, int.Parse(year), int.Parse(month));
            return View(data);
        }

        public JsonResult GetAttendanceData2(string from, string to)
        {
            var dfrom = DateTime.Parse(from);
            var dto = DateTime.Parse(to);
            var user = OperatorProvider.Provider.Current();
            var bll = new WorkmeetingBLL();
            var isMenu = getMenu();
            var data = bll.GetAttendanceData2(user.DeptId, dfrom, dto, isMenu);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Detail(string id)
        {
            ViewBag.id = id;
            var bll = new UserBLL();
            var user = bll.GetEntity(id);
            ViewBag.username = user.RealName;

            return View();
        }

        public ContentResult GetAttendance(string id, DateTime date)
        {
            var bll = new WorkmeetingBLL();
            var state = bll.GetAttendance(id, date);
            return Content(state);
        }

        public JsonResult GetMonthAttendance(string userid, int year, int month)
        {
            var bll = new WorkmeetingBLL();
            var data = bll.GetMonthAttendance(userid, year, month);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMonthAttendance2(string userid, int year, int month)
        {
            var bll = new WorkmeetingBLL();
            var data = bll.GetMonthAttendance2(userid, new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddMinutes(-1));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDayAttendance(int year, int month, int day, string id)
        {
            var success = true;
            var message = string.Empty;
            var bll = new WorkmeetingBLL();
            var data = default(DayAttendanceEntity);

            try
            {
                data = bll.GetDayAttendance(id, new DateTime(year, month, day));
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDayAttendance2(int year, int month, int day, string id)
        {
            var success = true;
            var message = string.Empty;
            var bll = new WorkmeetingBLL();
            var data = default(DayAttendanceEntity);

            try
            {
                data = bll.GetDayAttendance4(id, new DateTime(year, month, day));
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostUserState(string id, int year, int month, int day, DayAttendanceEntity model)
        {
            var success = true;
            var message = string.Empty;
            var bll = new WorkmeetingBLL();

            try
            {
                bll.PostUserState(id, new DateTime(year, month, day), model);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }

        public ContentResult Export(int year, int month)
        {
            var date = new DateTime(year, month, 1);
            var book = new Workbook();
            var sheet = book.Worksheets[0];
            sheet.Name = "考勤统计";
            sheet.Cells[0, 0].PutValue("序号");
            sheet.Cells.SetColumnWidthPixel(0, 40);
            var style = sheet.Cells[0, 0].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[0, 0].SetStyle(style);

            sheet.Cells.Merge(0, 0, 2, 1);
            sheet.Cells[0, 1].PutValue("姓名");
            sheet.Cells.SetColumnWidthPixel(1, 60);
            style = sheet.Cells[0, 1].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[0, 1].SetStyle(style);

            sheet.Cells.Merge(0, 1, 2, 1);
            sheet.Cells[0, 2].PutValue("日期及出缺勤情况");
            style = sheet.Cells[0, 2].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[0, 2].SetStyle(style);

            var i = 0;
            while (date.AddDays(i).Month == date.Month)
            {
                i++;

                sheet.Cells[1, 1 + i].PutValue(i);
                sheet.Cells.SetColumnWidthPixel(1 + i, 30);

            }
            sheet.Cells.Merge(0, 2, 1, i);
            sheet.Cells[0, 1 + i + 1].PutValue("缺勤");
            sheet.Cells.SetColumnWidthPixel(1 + i + 1, 60);
            style = sheet.Cells[0, 1 + i + 1].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[0, 1 + i + 1].SetStyle(style);
            sheet.Cells.Merge(0, 1 + i + 1, 1, 6);

            sheet.Cells[1, 1 + i + 1].PutValue("病假");
            style = sheet.Cells[1, 1 + i + 1].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[1, 1 + i + 1].SetStyle(style);

            sheet.Cells[1, 1 + i + 2].PutValue("事假");
            style = sheet.Cells[1, 1 + i + 2].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[1, 1 + i + 2].SetStyle(style);

            sheet.Cells[1, 1 + i + 3].PutValue("调休");
            style = sheet.Cells[1, 1 + i + 3].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[1, 1 + i + 3].SetStyle(style);

            sheet.Cells[1, 1 + i + 4].PutValue("公休");
            style = sheet.Cells[1, 1 + i + 4].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[1, 1 + i + 4].SetStyle(style);

            sheet.Cells[1, 1 + i + 5].PutValue("出差");
            style = sheet.Cells[1, 1 + i + 5].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[1, 1 + i + 5].SetStyle(style);

            sheet.Cells[1, 1 + i + 6].PutValue("其他");
            style = sheet.Cells[1, 1 + i + 6].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[1, 1 + i + 6].SetStyle(style);

            var user = OperatorProvider.Provider.Current();
            var bll = new WorkmeetingBLL();
            var data = bll.GetSigninData(user.DeptId, year, month);

            for (int j = 0; j < data.Count; j++)
            {
                sheet.Cells[j + 2, 0].PutValue(j + 1);
                sheet.Cells[j + 2, 1].PutValue(data[j].UserName);

                var cnt1 = 0d;
                var cnt2 = 0d;
                var cnt3 = 0d;
                var cnt4 = 0d;
                var cnt5 = 0d;
                var cnt6 = 0d;

                var m = 0;
                while (date.AddDays(m).Month == date.Month)
                {
                    var signins = data[j].Signins.Where(x => x.MeetingStartTime > date.AddDays(m) && x.MeetingStartTime < date.AddDays(m + 1)).ToList();

                    if (signins.Count > 0)
                    {
                        var g = signins.GroupBy(x => new { x.MeetingId, x.MeetingStartTime }).OrderByDescending(x => x.Key.MeetingStartTime).First();
                        switch (g.Count())
                        {
                            case 1:
                                if (g.First().IsSignin)
                                    sheet.Cells[j + 2, m + 2].PutValue("/");
                                else
                                {
                                    sheet.Cells[j + 2, m + 2].PutValue(g.First().Reason);
                                    switch (g.First().Reason)
                                    {
                                        case "病假":
                                            cnt1 = cnt1 + 0.5;
                                            break;
                                        case "事假":
                                            cnt2 = cnt2 + 0.5;
                                            break;
                                        case "调休":
                                            cnt3 = cnt3 + 0.5;
                                            break;
                                        case "公休":
                                            cnt4 = cnt4 + 0.5;
                                            break;
                                        case "出差":
                                            cnt5 = cnt5 + 0.5;
                                            break;
                                        case "其他":
                                            cnt6 = cnt6 + 0.5;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case 2:
                                var start = g.OrderBy(x => x.CreateDate).First();
                                var end = g.OrderBy(x => x.CreateDate).Last();
                                switch (g.Count(x => x.IsSignin))
                                {
                                    case 0:
                                        sheet.Cells[j + 2, m + 2].PutValue(start.Reason);
                                        switch (start.Reason)
                                        {
                                            case "病假":
                                                cnt1 = cnt1 + 0.5;
                                                break;
                                            case "事假":
                                                cnt2 = cnt2 + 0.5;
                                                break;
                                            case "调休":
                                                cnt3 = cnt3 + 0.5;
                                                break;
                                            case "公休":
                                                cnt4 = cnt4 + 0.5;
                                                break;
                                            case "出差":
                                                cnt5 = cnt5 + 0.5;
                                                break;
                                            case "其他":
                                                cnt6 = cnt6 + 0.5;
                                                break;
                                            default:
                                                break;
                                        }
                                        switch (end.Reason)
                                        {
                                            case "病假":
                                                cnt1 = cnt1 + 0.5;
                                                break;
                                            case "事假":
                                                cnt2 = cnt2 + 0.5;
                                                break;
                                            case "调休":
                                                cnt3 = cnt3 + 0.5;
                                                break;
                                            case "公休":
                                                cnt4 = cnt4 + 0.5;
                                                break;
                                            case "出差":
                                                cnt5 = cnt5 + 0.5;
                                                break;
                                            case "其他":
                                                cnt6 = cnt6 + 0.5;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case 1:
                                        var reason = start.IsSignin ? end.Reason : start.Reason;
                                        sheet.Cells[j + 2, m + 2].PutValue(reason);
                                        switch (reason)
                                        {
                                            case "病假":
                                                cnt1 = cnt1 + 0.5;
                                                break;
                                            case "事假":
                                                cnt2 = cnt2 + 0.5;
                                                break;
                                            case "调休":
                                                cnt3 = cnt3 + 0.5;
                                                break;
                                            case "公休":
                                                cnt4 = cnt4 + 0.5;
                                                break;
                                            case "出差":
                                                cnt5 = cnt5 + 0.5;
                                                break;
                                            case "其他":
                                                cnt6 = cnt6 + 0.5;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    default:
                                        sheet.Cells[j + 2, m + 2].PutValue("/");
                                        break;
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    m++;
                }


                if (cnt1 > 0)
                    sheet.Cells[j + 2, m + 2].PutValue(cnt1);
                if (cnt2 > 0)
                    sheet.Cells[j + 2, m + 2 + 1].PutValue(cnt2);
                if (cnt3 > 0)
                    sheet.Cells[j + 2, m + 2 + 2].PutValue(cnt3);
                if (cnt4 > 0)
                    sheet.Cells[j + 2, m + 2 + 3].PutValue(cnt4);
                if (cnt5 > 0)
                    sheet.Cells[j + 2, m + 2 + 4].PutValue(cnt5);
                if (cnt6 > 0)
                    sheet.Cells[j + 2, m + 2 + 5].PutValue(cnt6);


            }

            style = sheet.Cells[0, 0].GetStyle();
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            style.Borders[BorderType.TopBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.BottomBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.LeftBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style.Borders[BorderType.RightBorder].Color = System.Drawing.Color.Black;
            style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

            var range = sheet.Cells.CreateRange(0, 0, data.Count + 2, i + 8);
            range.ApplyStyle(style, new StyleFlag() { All = true });

            book.Save(System.Web.HttpContext.Current.Response, "考勤统计表.xlsx", ContentDisposition.Attachment, new XlsSaveOptions(SaveFormat.Xlsx));

            return Content(null);
        }
        #region  index3
        private WorkOrderBLL workorder = new WorkOrderBLL();
        public ActionResult index3(FormCollection fc)
        {
            var year = int.Parse(fc.Get("year") ?? DateTime.Now.Year.ToString());
            var month = int.Parse(fc.Get("month") ?? DateTime.Now.Month.ToString());
            var date = new DateTime(year, month, 1);
            date = date.AddDays(-date.Day);
            date = date.AddDays(-(int)date.DayOfWeek);

            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            var months = new List<dynamic>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new { value = i.ToString(), text = (i + "月") });
            }
            ViewData["date"] = date;
            ViewData["currentmonth"] = month;
            ViewData["year"] = new SelectList(years, "value", "text", year);
            ViewData["month"] = new SelectList(months, "value", "text", month);
            var user = OperatorProvider.Provider.Current();
            //var data = workorder.AttendanceList(date, date.AddDays(43), user.DeptId);
            //return View(data.GroupBy
            //    (x => x.startTime.ToString("yyyy-MM-dd")));
            return View();
        }

        #endregion

        public ViewResult Duty(FormCollection fc)
        {
            var year = int.Parse(fc.Get("year") ?? DateTime.Now.Year.ToString());
            var month = int.Parse(fc.Get("month") ?? DateTime.Now.Month.ToString());
            var date = new DateTime(year, month, 1);
            date = date.AddDays(-date.Day);
            date = date.AddDays(-(int)date.DayOfWeek);
            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            var months = new List<dynamic>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new { value = i.ToString(), text = (i + "月") });
            }

            ViewData["date"] = date;
            ViewData["currentmonth"] = month;
            ViewData["year"] = new SelectList(years, "value", "text", year);
            ViewData["month"] = new SelectList(months, "value", "text", month);

            var user = OperatorProvider.Provider.Current();
            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            var from = DateTime.Today.AddDays((1 - DateTime.Today.Day));
            from = from.AddDays(-GetDay(from.DayOfWeek));
            var to = from.AddDays(42);
            var bll = new WorkmeetingBLL();
            var data = bll.GetDutyData(user.DeptId, from, to);
            var list = new List<string>();
            var d = from;
            while (d < to)
            {
                list.Add(string.Join(",", data.Where(x => x.UnSignDate.Ticks == d.Ticks).Select(x => x.UserName)));

                d = d.AddDays(1);
            }

            return View(list);
        }

        private int GetDay(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 6;
                case DayOfWeek.Monday:
                    return 0;
                case DayOfWeek.Tuesday:
                    return 1;
                case DayOfWeek.Wednesday:
                    return 2;
                case DayOfWeek.Thursday:
                    return 3;
                case DayOfWeek.Friday:
                    return 4;
                case DayOfWeek.Saturday:
                    return 5;
                default:
                    return 0;
            }
        }

        public JsonResult GetDutyPerson(DateTime date)
        {
            var success = true;
            var message = string.Empty;
            var data = string.Empty;
            var bll = new WorkmeetingBLL();

            var user = OperatorProvider.Provider.Current();

            try
            {
                var persons = bll.GetDutyPerson(user.DeptId, date);
                data = persons == null ? string.Empty : string.Join(",", persons.Select(x => x.UserName).Distinct());
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDutyPersons(DateTime date)
        {
            var success = true;
            var message = string.Empty;
            var data = default(List<UnSignRecordEntity>);
            var bll = new WorkmeetingBLL();

            var user = OperatorProvider.Provider.Current();

            try
            {
                data = bll.GetDutyPerson(user.DeptId, date);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostDutyPerson(FormCollection fc)
        {
            var success = true;
            var message = string.Empty;
            var data = string.Empty;

            var json = fc.Get("data");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkmeetingEntity>(json);

            var user = OperatorProvider.Provider.Current();
            var bll = new WorkmeetingBLL();

            try
            {
                model.GroupId = user.DeptId;
                model.DutyPerson.ForEach(x =>
                {
                    x.UnSignRecordId = Guid.NewGuid().ToString();
                    x.UnSignDate = model.MeetingStartTime.Date;
                    //x.StartTime = x.ReasonRemark == "白班" ? new DateTime(model.MeetingStartTime.Year, model.MeetingStartTime.Month, model.MeetingStartTime.Day, 8, 0, 0) : new DateTime(model.MeetingStartTime.Year, model.MeetingStartTime.Month, model.MeetingStartTime.Day, 18, 0, 0)
                });

                bll.PostDutyPerson(new List<WorkmeetingEntity>() { model });
                data = model.DutyPerson == null ? string.Empty : string.Join(",", model.DutyPerson.Select(x => x.UserName).Distinct());
                //data = bll.GetDutyPerson(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        #region 考勤统计
        /// <summary>
        /// 管理平台统计页面
        /// </summary>
        /// <returns></returns>
        public ViewResult List()
        {
           
            var user = OperatorProvider.Provider.Current();
            var tree = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            return View();
        }

        /// <summary>
        /// 管理平台
        /// </summary>
        /// <returns></returns>
        public ViewResult ListDetail()
        {

            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var bll = new WorkmeetingBLL();
            decimal total = 0;
            int records = 0;
            var isMenu = getMenu();
            var data = bll.GetAttendanceData3(pagination, queryJson, out total, out records, isMenu);
            var JsonData = new
            {
                rows = data,
                total = total,
                page = pagination.page,
                records = records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListDetailJson(string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var bll = new WorkmeetingBLL();
            var deptid = string.Empty;
            var userId = string.Empty;
            var nowTime = DateTime.Now;
            var start = new DateTime(nowTime.Year, nowTime.Month, 1);
            var end = start.AddMonths(1);
            var queryParam = queryJson.ToJObject();
            //deptid
            if (!queryParam["deptId"].IsEmpty())
            {
                string pdeptid = queryParam["deptId"].ToString();
                var tree = new DepartmentBLL().GetSubDepartments(pdeptid, "");
                deptid = string.Join(",", tree.Select(x => x.DepartmentId));
            }
            //name deptname
            if (!queryParam["userId"].IsEmpty())
            {
                userId = queryParam["userId"].ToString();
            }
            // start
            if (!queryParam["Start"].IsEmpty())
            {
                start = Convert.ToDateTime(queryParam["Start"].ToString());
            }
            //end
            if (!queryParam["End"].IsEmpty())
            {
                end = Convert.ToDateTime(queryParam["End"].ToString());
            }
            var isMenu = getMenu();
            var data = bll.GetAttendanceUserData3(userId, deptid, start, end, isMenu);
            var JsonData = new
            {
                rows = data,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        private bool[] getMenu()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetList();
            var root = dept.FirstOrDefault(x => x.ParentId == "0");
            var get = dept.Where(x => x.ParentId == root.DepartmentId).OrderBy(x => x.EnCode).First();
            var dyresultS = MywebClient("MenuConfig/GetMenuList",
               "{'userid':'" + user.UserId + "','data':{'id':'" + get.DepartmentId + "','platform':1,'themetype':0}}");
            bool[] IsMenu = new bool[2];
            if (dyresultS.Contains("人脸签到"))
            {
                IsMenu[0] = true;
            }
            else
            {
                IsMenu[0] = false;
            }
            if (dyresultS.Contains("考勤签到"))
            {
                IsMenu[1] = true;
            }
            else
            {
                IsMenu[1] = false;
            }

            return IsMenu;
        }
        private static string MywebClient(string url, string val)
        {
            var webclient = new WebClient();
            var ApiIp = Config.GetValue("ErchtmsApiUrl");
            NameValueCollection postVal = new NameValueCollection();
            postVal.Add("json", val);
            var getData = webclient.UploadValues(ApiIp + url, postVal);
            var result = System.Text.Encoding.UTF8.GetString(getData);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-获取考情授权\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", url, val, result);
            return result;
        }
        public ActionResult GetAttendanceExportData3(string queryJson)
        {
           
            var user = OperatorProvider.Provider.Current();
            var queryParam = queryJson.ToJObject();
            var bll = new WorkmeetingBLL();
            var deptid = string.Empty;
            var name = string.Empty;
            var nowTime = DateTime.Now;
            var start = new DateTime(nowTime.Year, nowTime.Month, 1);
            var end = start.AddMonths(1);
            //deptid
            if (!queryParam["deptid"].IsEmpty())
            {
                string pdeptid = queryParam["deptid"].ToString();
                var tree = new DepartmentBLL().GetSubDepartments(pdeptid, "");
                deptid = string.Join(",", tree.Select(x => x.DepartmentId));
            }
            //name deptname
            if (!queryParam["name"].IsEmpty())
            {
                name = queryParam["name"].ToString();
            }
            // start
            if (!queryParam["Start"].IsEmpty())
            {
                start = Convert.ToDateTime(queryParam["Start"].ToString());
            }
            //end
            if (!queryParam["End"].IsEmpty())
            {
                end = Convert.ToDateTime(queryParam["End"].ToString());
            }
            var isMenu = getMenu();
            //取出数据源
            int total = 0;
            DataTable exportTable = bll.GetAttendanceExportData3(name, deptid, start, end, isMenu);
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            //excelconfig.Title = "违章信息";
            //excelconfig.TitleFont = "微软雅黑";
            //excelconfig.TitlePoint = 25;
            excelconfig.HeadHeight = 50;
            excelconfig.HeadPoint = 12;
            excelconfig.HeadFont = "宋体";
            excelconfig.FileName = "考情统计导出.xls";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            //ColumnEntity columnentity = new ColumnEntity();
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "jobcontent", ExcelColumn = "作业内容", Width = 12, Alignment = "fill" });
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "workquarters", ExcelColumn = "作业岗位", Width = 15 });

            //调用导出方法
            ExcelHelper.ExcelDownload(exportTable, excelconfig);
            return Content(null);
        }
        #endregion

    }
}