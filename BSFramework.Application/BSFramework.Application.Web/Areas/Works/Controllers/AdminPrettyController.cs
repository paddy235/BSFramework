using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Application.Service.BaseManage;
using System.Data;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Busines.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Service.WorkMeeting;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Util;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class AdminPrettyController : MvcControllerBase
    {
        //
        // GET: /Works/AdminPretty/
        AdminPrettyService service = new AdminPrettyService();
        DepartmentService dservice = new DepartmentService();
        private WorkmeetingBLL workmeetingbll = new WorkmeetingBLL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Photos()
        {
            var fb = new FileInfoBLL();

            var user = OperatorProvider.Provider.Current();
            if (user.DeptId == "0") user.DeptCode = "0";
            var now = DateTime.Now;
            var jobs = service.GetJobs(user.DeptCode, "1999-01-01");
            var flist = new List<Entity.PublicInfoManage.FileInfoEntity>();
            var jobfile = new List<Entity.PublicInfoManage.FileInfoEntity>();

            if (jobs.Count() > 20)
            {
                jobs = jobs.Take(20).ToList();
            }

            ViewData["Photos"] = jobs;
            return View();
        }
        [HttpPost]
        public JsonResult GetSSJKNumbers()
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();


            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
            DateTime nsdt = new DateTime(DateTime.Now.Year, month, 1);
            var user = OperatorProvider.Provider.Current();

            if (user.DeptId == "0") user.DeptCode = "0";
            //var all = service.GetMeeting(user.DeptCode, sdt.ToString("yyyy-MM-dd")); //各班组开展班会数量
            var ldt = service.GetLllegals(user.DeptId, sdt.ToString("yyyy-MM-dd"));  //各班组未整改违章数量
            var edudt = service.GetEducations(user.DeptCode, sdt.ToString("yyyy-MM-dd"));//各班组未评价的教育培训活动

            var acdt = service.GetActivitys(user.DeptCode, sdt.ToString("yyyy-MM-dd"));

            var kytdt = service.GetDanger(user.DeptCode, sdt.ToString("yyyy-MM-dd"));

            
            var edt = DateTime.Now;
            int days = (edt - sdt).Days;
            int wd = 0;
            while (sdt < edt) //循环比较，获得工作日数量
            {
                if (sdt.DayOfWeek != DayOfWeek.Saturday && sdt.DayOfWeek != DayOfWeek.Sunday)
                {
                    ++wd;
                }
                sdt = sdt.AddDays(1);
            }

            
            int s = wd * 2; //应开展班会数量
            int bzcount = 0;
            int meetings = 0;
            DepartmentBLL deptBll = new DepartmentBLL();
            var groups = deptBll.GetAllGroups().Where(x => x.EnCode.Contains(user.DeptCode));
            int meet = 0;
            int undo = 0;
            int undogroup = 0;
            foreach (DepartmentEntity d in groups)
            {
                bool b = false;
                for (int i = 0; i < s; i++)
                {
                    meet = service.GetUndoMeeting(d.DepartmentId, nsdt.AddDays(i).ToString("yyyy-MM-dd"), nsdt.AddDays(i + 1).ToString("yyyy-MM-dd"));
                    if (meet == 0) 
                    {
                        b = true;
                        undo++;
                    }
                }
                if (b)  //该班组有未开展的活动
                {
                    undogroup++;  //班组数量+1
                }
            }
            bzcount = undogroup;
            var total1 = undo;

            //违章
            int count1 = 0;
            int count2 = 0;
            foreach (DataRow row in ldt.Rows)
            {
                if (Convert.ToInt32(row[2]) != 0)
                {
                    ++count1;
                    count2 += Convert.ToInt32(row[2]);
                }
            }

            //教育培训
            int count3 = 0;
            int count4 = 0;
            foreach (DataRow row in edudt.Rows)
            {
                if (Convert.ToInt32(row[2]) != 0)
                {
                    ++count3;
                    count4 += Convert.ToInt32(row[2]);
                }
            }


            //安全日活动

            DateTime dtWeekSt1; //季度开始日，周开始日期
            DateTime dtWeekSt2; //当前周开始日期
            int w1 = (int)nsdt.DayOfWeek;
            int w2 = (int)DateTime.Now.DayOfWeek;
            if (w1 == 0)
            {
                w1 = 7;
            }
            if (w2 == 0)
            {
                w2 = 7;
            }
            dtWeekSt1 = nsdt.AddDays(1 - w1);
            dtWeekSt2 = DateTime.Now.AddDays(1 - w2);
            TimeSpan ts = dtWeekSt2 - dtWeekSt1;
            int weeks = (int)ts.Days / 7; //周数 == 应开展数量
            int count5 = 0;
            int count6 = 0;
            int acs = 0;


            
            var total = 0;
            int g = 0;
            int n = 0;
            var data = new List<newActivity>();
            
            var bll = new ActivityBLL();
            foreach (DepartmentEntity d in groups)
            {
                var dept = deptBll.GetEntity(d.ParentId);
                bool b = false;
                for (int i = 0; i < weeks; i++)
                {
                    n = bll.GetActivityList(d.DepartmentId, nsdt.AddDays(i * 7).ToString("yyyy-MM-dd"), nsdt.AddDays((i + 1) * 7).ToString("yyyy-MM-dd"));
                    if (n == 0)
                    {
                        b = true;
                        total++;
                        //data.Add(new newActivity { GroupName = d.FullName, FromTo = sdt.AddDays(i * 7).ToString("yyyy-MM-dd") + "至" + sdt.AddDays((i + 1) * 7).ToString("yyyy-MM-dd"), Remark = "", DeptName = dept == null ? "" : dept.FullName });
                    }
                }
                if (b)  //该班组有未开展的活动
                {
                    g++;  //班组数量+1
                }
            }
            count5 = g;
            count6 = total;


            //KYT
            int count7 = 0;
            int count8 = 0;
            foreach (DataRow row in kytdt.Rows)
            {
                if (Convert.ToInt32(row[2]) != 0)
                {
                    ++count7;
                    count8 += Convert.ToInt32(row[2]);
                }
            }

            //stopwatch.Stop();
            //var sec = stopwatch.Elapsed.TotalSeconds;


            var abll = new ActivityService();
            var ebll = new EduBaseInfoService();
            var mbll = new WorkmeetingService();
            string code = user.DeptCode;
            var alist = service.GetActs(code);
            var edulist = service.GetEdus(code);
            var mlist = service.GetDangers(code);
            var entity = new NewEntity();
            var data1 = new List<string>();
            foreach (ActivityEntity a in alist)
            {
                data1.Add(a.GroupId);
            }
            foreach (EduBaseInfoEntity a in edulist)
            {
                data1.Add(a.BZId);

            }
            foreach (DangerEntity a in mlist)
            {
                data1.Add(a.GroupId);

            }
            count4 = data1.Count;
            count3 = data1.Distinct().Count();
            var res = service.FindCount(code);
            return Json(new {  meeting1 = bzcount, meeting2 = total1, lllegals1 = count1, lllegals2 = count2, edu1 = count3, edu2 = count4, activity1 = count5, activity2 = count6, kyt1 = count7, kyt2 = count8  });
        }
        public ActionResult SSJK() 
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;
            ViewBag.deptName = user.DeptName;
            ModuleBLL moduleBLL = new ModuleBLL();
            var module = new ModuleEntity();
            var list = moduleBLL.GetList().Where(x => x.UrlAddress != "" && x.UrlAddress != null);
            module = list.Where(x => x.UrlAddress.Contains("/Works/WorkMeeting/Index3")).FirstOrDefault();//班会
            ViewBag.meetid = module.ModuleId;
            ViewBag.meeturl = module.UrlAddress;
            ViewBag.meettext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains("/Works/Education/Index4")).FirstOrDefault();//  /Works/Education/Index2
            ViewBag.eduid = module.ModuleId;
            ViewBag.eduurl = module.UrlAddress;
            ViewBag.edutext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Danger/Index2")).FirstOrDefault();//KYT
            ViewBag.kytid = module.ModuleId;
            ViewBag.kyturl = module.UrlAddress;
            ViewBag.kyttext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Activity/Index4")).FirstOrDefault();//KYT
            ViewBag.actid = module.ModuleId;
            ViewBag.acturl = module.UrlAddress;
            ViewBag.acttext = module.FullName;

            ViewBag.cpname = Config.GetValue("CustomerModel");
            return View();
        }
        
        public ActionResult DBSX() 
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetYGFC()
        {
            var fb = new FileInfoBLL();
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

            var user = OperatorProvider.Provider.Current();
            var now = DateTime.Now;
            var jobs = service.GetJobs(user.DeptCode, "1999-01-01");
            var flist = new List<Entity.PublicInfoManage.FileInfoEntity>();
            var jobfile = new List<Entity.PublicInfoManage.FileInfoEntity>();

            if (jobs.Count() > 20)
            {
                jobs = jobs.Take(20).ToList();
            }
            List<string> paths = new List<string>();
            foreach (FileInfoEntity f in jobs)
            {
                paths.Add(f.FilePath.TrimStart('~'));

            }
            return Json(new { data = paths });
        }
        public ActionResult YGFC() 
        {
            var fb = new FileInfoBLL();

            var user = OperatorProvider.Provider.Current();
            if (user.DeptId == "0") user.DeptCode = "0";
            var now = DateTime.Now;
            var jobs = service.GetJobs(user.DeptCode, "1999-01-01");
            var flist = new List<Entity.PublicInfoManage.FileInfoEntity>();
            var jobfile = new List<Entity.PublicInfoManage.FileInfoEntity>();

            if (jobs.Count() > 20)
            {
                jobs = jobs.Take(20).ToList();
            }

            ViewData["Photos"] = jobs;
            return View();
        }
        public ActionResult KPPM()
        {
            var bll = new EvaluateBLL();
            var total = 0;
            var data = bll.GetEvaluations(null, null, 10, 1, out total);
            var categories = bll.GetCategories();
            var use = OperatorProvider.Provider.Current();

            var year = DateTime.Now.Year;
            var season = "";
            if (DateTime.Now.Month < 4) { season = "4"; year = year - 1; }
            else if (DateTime.Now.Month < 7) season = "1";
            else if (DateTime.Now.Month < 10) season = "2";
            else season = "3";

            IList<EvaluateGroupEntity> pcts = new List<EvaluateGroupEntity>();
            var name = year + "年第" + season + "季度";
            if (data.Count > 0 && data.FirstOrDefault(row => row.EvaluateSeason == name)!=null)
            {
                pcts = bll.GetCalcScore(data.Count == 0 ? "xxx" : data.First(row => row.EvaluateSeason == name).EvaluateId, null);
                
                
            }
            ViewData["All"] = pcts;

           
            return View(pcts);
        }


        [HttpPost]
        public JsonResult GetGZTZ()
        {
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);
            var user = OperatorProvider.Provider.Current();

            if (user.DeptId == "0") user.DeptCode = "0";

            var total = 0;

            var list = workmeetingbll.GetData(10000, 1, out total, new Dictionary<string, string>() { { "departmentid", user.DeptId }, { "meetingstarttime", sdt.ToString("yyyy-MM-dd") } });
            int count1 = total;

            //var list1 = workmeetingbll.GetBaseDataNew("", 10000, 1, out total).Where(x => x.CreateDate > sdt);
            int count2 = service.GetAllDanger(user.DeptCode, sdt.ToString("yyyy-MM-dd"));

            int count3 = service.GetAllActivity(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
            int count4 = service.GetAllEducation(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
           int count5 = service.GetAllLllegal(user.DeptId, sdt.ToString("yyyy-MM-dd"));
            int count6 = service.GetAllEmergencyWork(user.DeptName, sdt.ToString("yyyy-MM-dd"));
            int count7 = service.GetAllToolborrow(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
           int count8 = service.GetAllGlassStock(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
           return Json(new { count1 = count1, count2 = count2, count3 = count3, count4 = count4, count5 = count5, count6 = count6, count7 = count7, count8 = count8 });
        }
        public ActionResult GZTZ()
        {
            var module = new ModuleEntity();
            //int month = 1;
            //if (DateTime.Now.Month < 4) month = 1;
            //else if (DateTime.Now.Month < 7) month = 4;
            //else if (DateTime.Now.Month < 10) month = 7;
            //else if (DateTime.Now.Month <= 12) month = 10;
            //DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);
            //var user = OperatorProvider.Provider.Current();

            //if (user.DeptId == "0") user.DeptCode = "0";

            //var total = 0;

            //var list = workmeetingbll.GetData(10000, 1, out total, new Dictionary<string, string>() { { "departmentid", user.DeptId }, { "meetingstarttime", sdt.ToString("yyyy-MM-dd") } });
            //ViewData["Count1"] = total;

            ////var list1 = workmeetingbll.GetBaseDataNew("", 10000, 1, out total).Where(x => x.CreateDate > sdt);
            //ViewData["Count2"] = service.GetAllDanger(user.DeptCode, sdt.ToString("yyyy-MM-dd"));

            //ViewData["Count3"] = service.GetAllActivity(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
            //ViewData["Count4"] = service.GetAllEducation(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
            //ViewData["Count5"] = service.GetAllLllegal(user.DeptId, sdt.ToString("yyyy-MM-dd"));
            //ViewData["Count6"] = service.GetAllEmergencyWork(user.DeptName, sdt.ToString("yyyy-MM-dd"));
            //ViewData["Count7"] = service.GetAllToolborrow(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
            //ViewData["Count8"] = service.GetAllGlassStock(user.DeptCode, sdt.ToString("yyyy-MM-dd"));
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;
            ViewBag.deptName = user.DeptName;
            ModuleBLL moduleBLL = new ModuleBLL();
            var list = moduleBLL.GetList().Where(x => x.UrlAddress != "" && x.UrlAddress != null);
            module = list.Where(x => x.UrlAddress.Contains( "/Works/WorkMeeting/Index2")).FirstOrDefault();//班会
            ViewBag.meetid = module.ModuleId;
            ViewBag.meeturl = module.UrlAddress;
            ViewBag.meettext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Danger/Index2")).FirstOrDefault();//KYT
            ViewBag.kytid = module.ModuleId;
            ViewBag.kyturl = module.UrlAddress;
            ViewBag.kyttext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Activity/Index3")).FirstOrDefault();//KYT
            ViewBag.actid = module.ModuleId;
            ViewBag.acturl = module.UrlAddress;
            ViewBag.acttext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Education/Index2")).FirstOrDefault();//教育培训
            ViewBag.eduid = module.ModuleId;
            ViewBag.eduurl = module.UrlAddress;
            ViewBag.edutext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Lllegal/Index2")).FirstOrDefault();//教育培训
            ViewBag.legalid = module.ModuleId;
            ViewBag.legalurl = module.UrlAddress;
            ViewBag.legaltext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/emergency/Index1")).FirstOrDefault();//教育培训
            ViewBag.emid = module.ModuleId;
            ViewBag.emurl = module.UrlAddress;
            ViewBag.emtext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains( "/Works/Drug/Index")).FirstOrDefault();//教育培训
            ViewBag.glassid = module.ModuleId;
            ViewBag.glassurl = module.UrlAddress;
            ViewBag.glasstext = module.FullName;

            ViewBag.cpname = Config.GetValue("CustomerModel");
            return View();
        }


    }
}
