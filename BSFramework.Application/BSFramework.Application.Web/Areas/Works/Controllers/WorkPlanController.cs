using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkPlanManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class WorkPlanController : MvcControllerBase
    {
        //
        // GET: /Works/WorkPlan/
        public WorkPlanBLL bll = new WorkPlanBLL();
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.code = dept.EnCode;


            ViewBag.deptName = user.DeptName;
            ViewBag.userid = user.UserId;
            return View();
        }

        public ActionResult GetPlanJson()
        {
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var from = this.Request.QueryString.Get("from");
            var to = this.Request.QueryString.Get("to");
            var name = this.Request.QueryString.Get("name");
            var plantype = this.Request.QueryString.Get("plantype");
            var code = this.Request.QueryString.Get("code");
            var deptid = this.Request.QueryString.Get("deptid");
            if (string.IsNullOrEmpty(code))
            {
                code = user.DeptCode;
                if (dept == null)
                {
                    code = "0";
                }
                else
                {
                    if (dept.IsSpecial)
                    {
                        code = "0";
                    }
                }
            }
            var total = 0;
            var watch = CommonHelper.TimerStart();
            //var groups = deptBll.GetAllGroups().Where(x => x.EnCode.Contains(code)).Select(x => x.DepartmentId);
            var data = bll.GetPlanList().Where(x => x.DeleteRemark == false);
            if (!string.IsNullOrEmpty(from))
            {
                data = data.Where(x => x.StartDate >= Convert.ToDateTime(from));
            }
            if (!string.IsNullOrEmpty(to))
            {
                data = data.Where(x => x.EndDate < Convert.ToDateTime(to).AddDays(1));
            }
            if (!string.IsNullOrEmpty(plantype) && plantype != "全部")
            {
                data = data.Where(x => x.PlanType == plantype);
            }
            if (!string.IsNullOrEmpty(code))
            {
                data = data.Where(x => x.UseDeptCode.StartsWith(code));
            }

            data = data.ToList();
            //var ndata = new List<WorkPlanEntity>();
            foreach (WorkPlanEntity w in data)
            {
                w.date = w.StartDate.ToString("yyyy-MM-dd") + "--" + w.EndDate.ToString("yyyy-MM-dd");
                //var codes = w.UseDeptCode.Split(',');
                //foreach (string c in codes) 
                //{
                //    if (c.StartsWith(code)) 
                //    {
                //        ndata.Add(w);
                //        break;
                //    }
                //}
            }
            total = data.Count();
            data = data.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlanContent(string keyValue)
        {
            var watch = CommonHelper.TimerStart();
            var list = bll.GetContentList(keyValue).Where(x => x.DeleteRemark == false);
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = list.Count();
            list = list.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = list, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlanContentNew(string keyValue)
        {
            var watch = CommonHelper.TimerStart();
            var list = bll.GetContentList(keyValue);
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = list.Count();
            list = list.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = list, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(string id)
        {

            WorkPlanEntity di = bll.GetWorkPlanEntity(id);
            ViewBag.planid = di.ID;
            return View(di);
        }
        public ActionResult Form(string id)
        {
            ViewBag.type = "add";
            //新增时，删除所有与0关联的子任务（上次操作新增了子任务但未保存父任务）
            var contents = bll.GetContentList("0");
            foreach (WorkPlanContentEntity w in contents)
            {
                bll.RemoveWorkPlanContent(w.ID);
            }
            Operator user = OperatorProvider.Provider.Current();
            WorkPlanEntity di = new WorkPlanEntity();
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.type = "edit";
                di = bll.GetWorkPlanEntity(id);
            }
            else
            {

                di.ID = "0";
                di.CreateDate = DateTime.Now;
                di.CreateUserId = user.UserId;
                di.CreateUser = user.UserName;
                di.StartDate = DateTime.Now;
                di.EndDate = DateTime.Now;
            }
            ViewBag.planid = di.ID;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;

            return View(di);
        }
        public ActionResult FormContent(string keyValue)
        {
            Operator user = OperatorProvider.Provider.Current();

            var di = bll.GetWorkPlanContentEntity(keyValue);
            if (di == null)
            {
                di = new WorkPlanContentEntity();
                di.ID = Guid.NewGuid().ToString();
                di.CreateDate = DateTime.Now;
                di.CreateUserId = user.UserId;
                di.CreateUser = user.UserName;
                di.PlanId = keyValue;
            }
            return View(di);
        }
        [HttpPost]
        public JsonResult SaveFormContent(WorkPlanContentEntity model)
        {
            Operator user = OperatorProvider.Provider.Current();
            var now = DateTime.Now;
            var old = bll.GetWorkPlanContentEntity(model.ID);
            if (old == null)
            {
                model.IsFinished = "未完成";
                bll.SaveWorkPlanContent(model.ID, model);
            }
            else
            {
                if (old.WorkContent != model.WorkContent)
                {
                    var change = "由" + user.UserName + "更新于" + now.Year + "年" + now.Month + "月" + now.Day + "日" + now.Hour + "时" + now.Minute + "分 ";
                    change += "工作内容从 '" + old.WorkContent + "' 变更为 '" + model.WorkContent + "'&";
                    model.ChangeRemark += change;

                }
                bll.SaveWorkPlanContent(model.ID, model);
            }

            return Json(new { success = true, message = "操作成功" });
        }
        [HttpPost]
        public JsonResult SaveForm(WorkPlanEntity model)
        {
            Operator user = OperatorProvider.Provider.Current();
            var now = DateTime.Now;
            if (model.ID != "0")  //编辑
            {
                var old = bll.GetWorkPlanEntity(model.ID);
                if (model.StartDate != old.StartDate || model.EndDate != old.EndDate)   //修改工作计划时间
                {
                    var contents = bll.GetContentList(model.ID).Where(x => x.DeleteRemark == false);
                    foreach (WorkPlanContentEntity w in contents)
                    {
                        var change = "由" + user.UserName + "更新于" + now.Year + "年" + now.Month + "月" + now.Day + "日" + now.Hour + "时" + now.Minute + "分 ";
                        change += "计划时间从 '" + old.StartDate.ToString("yyyy-MM-dd") + " -- " + old.EndDate.ToString("yyyy-MM-dd") + "' 变更为 '" + model.StartDate.ToString("yyyy-MM-dd") + " -- " + model.EndDate.ToString("yyyy-MM-dd") + "'&";
                        w.ChangeRemark += change;
                        bll.SaveWorkPlanContent(w.ID, w);
                    }
                    bll.SaveWorkPlan(model.ID, model);
                }

            }
            else
            {
                var deptids = model.UseDeptId.Split(',');
                var deptnames = model.UseDeptName.Split(',');
                var deptcodes = model.UseDeptCode.Split(',');
                var contents = bll.GetContentList("0");
                for (int i = 0; i < deptids.Length; i++)
                {
                    model.ID = Guid.NewGuid().ToString();
                    model.UseDeptId = deptids[i];
                    model.UseDeptCode = deptcodes[i];
                    model.UseDeptName = deptnames[i];
                    model.IsFinished = "未完成";
                    model.DeleteRemark = false;
                    foreach (WorkPlanContentEntity w in contents)
                    {
                        w.ID = Guid.NewGuid().ToString();
                        w.PlanId = model.ID;
                        w.BZID = model.UseDeptId;
                        model.IsFinished = "未完成";
                        //w.StartDate = model.StartDate;
                        //w.EndDate = model.EndDate;
                        bll.SaveWorkPlanContent(w.ID, w);
                    }
                    bll.SaveWorkPlan(model.ID, model);
                }
                contents = bll.GetContentList("0");
                foreach (WorkPlanContentEntity w in contents)
                {
                    bll.RemoveWorkPlanContent(w.ID);
                }

                new MessageBLL().SendMessage("工作计划", model.ID);
            }

            return Json(new { success = true, message = "操作成功" });
        }

        public JsonResult DeleteOne(string keyValue)
        {
            Operator user = OperatorProvider.Provider.Current();
            var model = bll.GetWorkPlanEntity(keyValue);
            model.DeleteRemark = true;
            bll.SaveWorkPlan(keyValue, model);
            var contents = bll.GetContentList(keyValue).Where(x => x.DeleteRemark == false);

            var now = DateTime.Now.Date;
            int week = (int)now.DayOfWeek;
            if (week == 0) week = 7;
            var weekstart = now.AddDays(0 - week + 1);
            var weekend = weekstart.AddDays(7);
            var monthstart = new DateTime(now.Year, now.Month, 1);
            var monthend = monthstart.AddMonths(1);
            if (model.PlanType == "周工作计划")
            {
                if (model.StartDate >= weekstart && model.EndDate < weekend)  //该计划属于本月计划或本周计划 ,增加删除标记，且不删除计划
                {
                    foreach (WorkPlanContentEntity w in contents)
                    {
                        w.DeleteRemark = true;
                        w.IsFinished = "已取消";
                        var change = "由" + user.UserName + "更新于" + now.Year + "年" + now.Month + "月" + now.Day + "日" + now.Hour + "时  工作内容被取消";
                        w.ChangeRemark += change;
                        bll.SaveWorkPlanContent(w.ID, w);

                        w.ChildrenContent = bll.GetContentList("").Where(x => x.ParentId == w.ID).ToList();
                        foreach (WorkPlanContentEntity w1 in w.ChildrenContent)
                        {
                            w1.DeleteRemark = true;
                            w1.IsFinished = "已取消";
                            bll.SaveWorkPlanContent(w1.ID, w1);
                        }
                    }
                }
                else //直接删除计划及内容
                {
                    foreach (WorkPlanContentEntity w in contents)
                    {
                        bll.RemoveWorkPlanContent(w.ID);
                    }
                    bll.RemoveWorkPlan(keyValue);
                }
            }
            if (model.PlanType == "月工作计划")
            {
                if (model.StartDate >= monthstart && model.EndDate < monthend)  //该计划属于本月计划或本周计划 ,增加删除标记，且不删除计划
                {
                    foreach (WorkPlanContentEntity w in contents)
                    {
                        w.DeleteRemark = true;
                        w.IsFinished = "已取消";
                        var change = "由" + user.UserName + "更新于" + now.Year + "年" + now.Month + "月" + now.Day + "日" + now.Hour + "时  工作内容被取消";
                        w.ChangeRemark += change;
                        bll.SaveWorkPlanContent(w.ID, w);

                        w.ChildrenContent = bll.GetContentList("").Where(x => x.ParentId == w.ID).ToList();
                        foreach (WorkPlanContentEntity w1 in w.ChildrenContent)
                        {
                            w1.DeleteRemark = true;
                            w1.IsFinished = "已取消";
                            bll.SaveWorkPlanContent(w1.ID, w1);
                        }
                    }
                }
                else //直接删除计划及内容
                {
                    foreach (WorkPlanContentEntity w in contents)
                    {
                        bll.RemoveWorkPlanContent(w.ID);
                    }
                    bll.RemoveWorkPlan(keyValue);
                }
            }


            return Json(new { success = true, message = "操作成功" });
        }
        public JsonResult DeleteContent(string keyValue)
        {
            Operator user = OperatorProvider.Provider.Current();
            var model1 = bll.GetWorkPlanContentEntity(keyValue);
            var model = bll.GetWorkPlanEntity(model1.PlanId);
            var now = DateTime.Now;
            int week = (int)now.DayOfWeek;
            if (week == 0) week = 7;
            var weekstart = now.AddDays(0 - week + 1);
            var weekend = weekstart.AddDays(7);
            var monthstart = new DateTime(now.Year, now.Month, 1);
            var monthend = monthstart.AddMonths(1);
            if (model == null) //新增计划时，删除计划内容，不记录变更
            {
                bll.RemoveWorkPlanContent(keyValue);
            }
            else
            {
                if (model.PlanType == "月工作计划")
                {
                    if (model.StartDate >= monthstart && model.EndDate < monthend)  //该计划属于本月计划或本周计划 ,增加删除标记，且不删除计划
                    {
                        model1.DeleteRemark = true;
                        model1.IsFinished = "已取消";
                        var change = "由" + user.UserName + "更新于" + now.Year + "年" + now.Month + "月" + now.Day + "日" + now.Hour + "时  工作内容被取消";
                        model1.ChangeRemark += change;
                        bll.SaveWorkPlanContent(model1.ID, model1);
                        model1.ChildrenContent = bll.GetContentList("").Where(x => x.ParentId == model1.ID).ToList();
                        foreach (WorkPlanContentEntity w in model1.ChildrenContent)
                        {
                            w.DeleteRemark = true;
                            w.IsFinished = "已取消";
                            bll.SaveWorkPlanContent(w.ID, w);
                        }
                    }
                    else //修改计划时，删除计划内容，但计划不属于本月或本周计划，直接删除数据
                    {
                        bll.RemoveWorkPlanContent(keyValue);
                    }
                }
                else if (model.PlanType == "周工作计划")
                {
                    if (model.StartDate >= weekstart && model.EndDate < weekend)  //该计划属于本月计划或本周计划 ,增加删除标记，且不删除计划
                    {
                        model1.DeleteRemark = true;
                        model1.IsFinished = "已取消";
                        var change = "由" + user.UserName + "更新于" + now.Year + "年" + now.Month + "月" + now.Day + "日" + now.Hour + "时  工作内容被取消";
                        model1.ChangeRemark += change;
                        bll.SaveWorkPlanContent(model1.ID, model1);
                        model1.ChildrenContent = bll.GetContentList("").Where(x => x.ParentId == model1.ID).ToList();
                        foreach (WorkPlanContentEntity w in model1.ChildrenContent)
                        {
                            w.DeleteRemark = true;
                            w.IsFinished = "已取消";
                            bll.SaveWorkPlanContent(w.ID, w);
                        }
                    }
                }
                else //修改计划时，删除计划内容，但计划不属于本月或本周计划，直接删除数据
                {
                    bll.RemoveWorkPlanContent(keyValue);
                }
            }
            return Json(new { success = true, message = "操作成功" });
        }
    }
}
