using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class ScoreController : Controller
    {
        //
        // GET: /Works/Score/
        private LllegalBLL bll = new LllegalBLL();
        private UserBLL userbll = new UserBLL();
        private PeopleBLL pbll = new PeopleBLL();
        private WorkmeetingBLL wbll = new WorkmeetingBLL();
        private ScoreBLL sbll = new ScoreBLL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ScoreAll(FormCollection fc)
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
            ViewData["currentyear"] = year;
            var user = OperatorProvider.Provider.Current();

            var users = userbll.GetDeptUsers(user.DeptId).ToList();
            var peoples = new List<PeopleEntity>();
            IList<string> pers = new List<string>();
            IList<string> ids = new List<string>();
            foreach (UserEntity u in users)
            {
                var p = pbll.GetEntity(u.UserId);
                if (p == null) continue;
                //var total = wbll.GetJobs(p.ID, year.ToString(), month.ToString(), "", "");   //总数
                //var finish = wbll.GetJobs(p.ID, year.ToString(), month.ToString(), "", "finish");  //已完成
                //var per = 1.0;
                //if (total.Count() > 0)
                //{
                //    per = Math.Round(Convert.ToDouble(finish.Count()) / total.Count());
                //}
                //var score = 0;
                //foreach (MeetingJobEntity m in finish) 
                //{
                //    var jobuser = wbll.GetJobUsers(m.JobId).Where(x => x.UserId == u.UserId).FirstOrDefault();
                //    if (jobuser != null)
                //    {
                //        if (jobuser.Score != null)
                //        {
                //            score += jobuser.Score.Value;
                //        }
                //    }
                //}

                //p.Jobs = finish.Count().ToString();
                //p.Scores = score.ToString();
                //p.Percent = (per * 100).ToString();
                p.Jobs = sbll.PersonFinishCount(u.UserId, year, month).ToString();
                p.Percent = Convert.ToInt32(sbll.PersonPercent(u.UserId, year, month)).ToString();
                p.Scores = sbll.PersonTotalScore(u.UserId, year, month).ToString();
                peoples.Add(p);
                pers.Add(p.Percent);
                ids.Add(p.ID);
            }

            return View(peoples);
        }

        public ActionResult MonthAll(string currentmonth, string currentyear, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var users = userbll.GetDeptUsers(user.DeptId).ToList();



            var year = int.Parse(fc.Get("year") ?? DateTime.Now.Year.ToString());
            if (!string.IsNullOrEmpty(currentyear))
            {
                year = int.Parse(currentyear);
            }
            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            ViewData["year"] = new SelectList(years, "value", "text", year);

            var month = int.Parse(fc.Get("month") ?? DateTime.Now.Month.ToString());
            if (!string.IsNullOrEmpty(currentmonth))
            {
                month = int.Parse(currentmonth);
            }
            var months = new List<dynamic>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new { value = i.ToString(), text = (i + "月") });
            }
            ViewData["month"] = new SelectList(months, "value", "text", month);
            ViewData["cmonth"] = month;
            var finish = sbll.GetFinishTaskCount(user.DeptId, year, month);
            var unfinish = sbll.GetUnfinishTaskCount(user.DeptId, year, month);
            ViewData["finish"] = finish;
            ViewData["unfinish"] = unfinish;
            ViewData["finishpercent"] = (Math.Round(sbll.GetScore4(user.DeptId, year, month), 4)).ToString("p");
            ViewData["totalscore"] = sbll.GetTotalScore(user.DeptId, year, month);
            ViewData["avgfinish"] = sbll.GetAvgTaskCount(user.DeptId, year, month);
            ViewData["avgscore"] = sbll.GetAvgScore(user.DeptId, year, month);


            return View();
        }
        public ActionResult PersonScore(string id, string t, string currentmonth, string currentyear, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var users = userbll.GetDeptUsers(user.DeptId).ToList();

            string seluser = id;
            string seluser1 = id;
            if (string.IsNullOrEmpty(id)) seluser = fc.Get("user");
            if (string.IsNullOrEmpty(id)) seluser1 = fc.Get("user1");
            var dpusers = new List<dynamic>();
            foreach (UserEntity u in users)
            {
                dpusers.Add(new { value = u.UserId, text = u.RealName });
            }
            ViewData["user"] = new SelectList(dpusers, "value", "text", seluser);
            ViewData["user1"] = new SelectList(dpusers, "value", "text", seluser1);
            ViewBag.t = t ?? "0"; //首次加载，无参数  赋0
            var year = int.Parse(fc.Get("year") ?? DateTime.Now.Year.ToString());
            var year1 = int.Parse(fc.Get("year1") ?? DateTime.Now.Year.ToString());
            if (!string.IsNullOrEmpty(currentyear))
            {
                year = year1 = int.Parse(currentyear);
            }
            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            ViewData["year"] = new SelectList(years, "value", "text", year);
            ViewData["year1"] = new SelectList(years, "value", "text", year1);


            var month = int.Parse(fc.Get("month") ?? DateTime.Now.Month.ToString());
            if (!string.IsNullOrEmpty(currentmonth))
            {
                month = int.Parse(currentmonth);
            }
            var months = new List<dynamic>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new { value = i.ToString(), text = (i + "月") });
            }
            ViewData["month"] = new SelectList(months, "value", "text", month);
            //var scoretype = Request["scoretype"];

            //ViewBag.scoretype = "1";
            var data = sbll.PersonJobs(seluser1, year, month);
            //foreach (MeetingJobEntity m in data)
            //{
            //    var jobuser = wbll.GetJobUsers(m.JobId, null).Where(x => x.UserId == seluser1).FirstOrDefault();

            //    if (jobuser != null)
            //    {
            //        if (jobuser.Score != null)
            //        {
            //            m.Score = jobuser.Score.ToString();
            //        }
            //        else
            //        {
            //            m.Score = "0";
            //        }
            //        // m.Score = jobuser.Score == null ? jobuser.Score.ToString() : "0";
            //    }
            //    else
            //    {
            //        m.Score = "0";
            //    }
            //}
            ViewData["jobs"] = data;
            ViewData["total"] = sbll.PersonTotalScore(seluser1, year, month);
            return View();
        }

        public JsonResult GetMonthAll(string year, string month, string type)
        {
            var user = OperatorProvider.Provider.Current();
            //var users = userbll.GetDeptUsers(user.DeptId).ToList();

            if (type == "1")
                return Json(new { data = sbll.GetScore1(user.DeptId, int.Parse(year), int.Parse(month)) });
            if (type == "2")
                return Json(new { data = sbll.GetScore2(user.DeptId, int.Parse(year), int.Parse(month)) });
            if (type == "3")
                return Json(new { data = sbll.GetScore3(user.DeptId, int.Parse(year), int.Parse(month)) });
            return Json(new { data = "Errow" });
        }
        public JsonResult GetPersonScore(string user, string year)
        {
            var userobj = userbll.GetEntity(user);
            return Json(new { data = sbll.GetPersonScore(user, int.Parse(year)), data1 = sbll.GetDeptScoreAvg(userobj.DepartmentId, int.Parse(year)) });
        }

        public JsonResult GetPercents(string year, string month)
        {
            var user = OperatorProvider.Provider.Current();

            var users = userbll.GetDeptUsers(user.DeptId).ToList();
            var peoples = new List<PeopleEntity>();
            IList<string> pers = new List<string>();
            IList<string> ids = new List<string>();
            //foreach (UserEntity u in users)
            //{
            //    var p = pbll.GetEntity(u.UserId);
            //    var total = wbll.GetJobs(p.ID, year.ToString(), month.ToString(), "", "");   //总数
            //    var finish = wbll.GetJobs(p.ID, year.ToString(), month.ToString(), "", "finish");  //已完成
            //    var per = 1.0;
            //    if (total.Count() > 0)
            //    {
            //        per = Math.Round(Convert.ToDouble(finish.Count()) / total.Count());
            //    }
            //    var score = 0;
            //    foreach (MeetingJobEntity m in finish)
            //    {
            //        var jobuser = wbll.GetJobUsers(m.JobId).Where(x => x.UserId == u.UserId).FirstOrDefault();
            //        if (jobuser != null)
            //        {
            //            if (jobuser.Score != null)
            //            {
            //                score += jobuser.Score.Value;
            //            }
            //        }
            //    }
            //    p.Jobs = finish.Count().ToString();
            //    p.Scores = score.ToString();
            //    p.Percent = (per * 100).ToString();
            //    peoples.Add(p);
            //    pers.Add(p.Percent);
            //    ids.Add(p.ID);
            //}

            foreach (UserEntity u in users)
            {
                var p = pbll.GetEntity(u.UserId);
                if (p != null)
                {
                    p.Jobs = sbll.PersonFinishCount(u.UserId, int.Parse(year), int.Parse(month)).ToString();
                    p.Percent = Convert.ToInt32(sbll.PersonPercent(u.UserId, int.Parse(year), int.Parse(month))).ToString();
                    p.Scores = sbll.PersonTotalScore(u.UserId, int.Parse(year), int.Parse(month)).ToString();
                    peoples.Add(p);
                    pers.Add(p.Percent);
                    ids.Add(p.ID);
                }
            }
            return Json(new { pers = pers, ids = ids });
        }
    }
}
