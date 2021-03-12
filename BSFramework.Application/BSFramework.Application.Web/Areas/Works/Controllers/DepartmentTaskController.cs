using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class DepartmentTaskController : MvcControllerBase
    {
        public DateTime ModifyTime { get; private set; }

        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userid = user.UserId;
            return View();
        }

        public JsonResult List1(DateTime? startdate, DateTime? enddate, string status, int rows, int page)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new DepartmentTaskBLL();
            var total = 0;
            var data = bll.List1(user.DeptId, startdate, enddate, status, rows, page, out total);
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List2(DateTime? startdate, DateTime? enddate, string status, int rows, int page)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new DepartmentTaskBLL();
            var total = 0;
            var data = bll.List2(user.UserId, startdate, enddate, status, rows, page, out total);
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page }, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Edit(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new DepartmentTaskEntity() { DutyDepartmentId = user.DeptId, DutyDepartment = user.DeptName };
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new DepartmentTaskBLL();
                model = bll.Detail(id);
            }

            if (model.SubTasks == null) model.SubTasks = new List<DepartmentTaskEntity>();
            model.SubTasks.ForEach(x => x.State = 0);
            model.SubTasks = model.SubTasks.Select(x => new { Seq = x.DutyDepartmentId == user.DeptId ? 0 : 1, Task = x }).OrderBy(x => x.Seq).ThenBy(x => x.Task.DutyDepartmentId).ThenByDescending(x => x.Task.CreateTime).Select(x => x.Task).ToList();
            ViewBag.userid = user.UserId;
            ViewBag.id = id;
            return View(model);
        }

        public ViewResult Update(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new DepartmentTaskEntity() { DutyDepartmentId = user.DeptId, DutyDepartment = user.DeptName };
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new DepartmentTaskBLL();
                model = bll.Detail(id);
            }

            if (model.SubTasks == null) model.SubTasks = new List<DepartmentTaskEntity>();
            model.SubTasks.ForEach(x => x.State = 0);
            model.SubTasks = model.SubTasks.Select(x => new { Seq = x.DutyDepartmentId == user.DeptId ? 0 : 1, Task = x }).OrderBy(x => x.Seq).ThenBy(x => x.Task.DutyDepartmentId).ThenByDescending(x => x.Task.CreateTime).Select(x => x.Task).ToList();
            ViewBag.userid = user.UserId;
            ViewBag.id = id;
            return View(model);
        }

        public ViewResult Detail(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new DepartmentTaskEntity() { DutyDepartmentId = user.DeptId, DutyDepartment = user.DeptName };
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new DepartmentTaskBLL();
                model = bll.Detail(id);
            }

            if (model.SubTasks == null) model.SubTasks = new List<DepartmentTaskEntity>();
            model.SubTasks.ForEach(x => x.State = 0);
            model.SubTasks = model.SubTasks.Select(x => new { Seq = x.DutyDepartmentId == user.DeptId ? 0 : 1, Task = x }).OrderBy(x => x.Seq).ThenBy(x => x.Task.DutyDepartmentId).ThenByDescending(x => x.Task.CreateTime).Select(x => x.Task).ToList();
            ViewBag.userid = user.UserId;
            ViewBag.id = id;

            var date = DateTime.Today;
            if (model.Status == "未开始")
            {
                if (model.StartDate.Value <= date && model.EndDate.Value >= date) model.Status = "进行中";
                else if (model.EndDate < DateTime.Now) model.Status = "未完成";
            }

            return View(model);
        }

        public ViewResult Edit2(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.category = dept.Nature == "厂级" ? "厂级,部门" : "厂级,部门,班组";
            ViewBag.id = id;
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }

        public ViewResult Update2(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.category = dept.Nature == "厂级" ? "厂级,部门" : "厂级,部门,班组";
            ViewBag.id = id;
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }

        public ViewResult Detail2(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.id = id;
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }

        [HttpPost]
        public JsonResult Edit(string id, DepartmentTaskEntity model)
        {
            var user = OperatorProvider.Provider.Current();
            var success = true;
            var message = "保存成功！";
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    model.TaskId = Guid.NewGuid().ToString();
                    model.Status = "未开始";
                    model.CreateUserId = model.ModifyUserId = user.UserId;
                    model.CreateUser = model.ModifyUser = user.UserName;
                    model.CreateTime = model.ModifyTime = DateTime.Now;
                    model.CreateDeptId = user.DeptId;
                    model.CreateDept = user.DeptName;
                }
                else
                {
                    model.TaskId = id;
                    model.ModifyTime = DateTime.Now;
                    model.ModifyUserId = user.UserId;
                    model.ModifyUser = user.UserName;
                }

                if (model.SubTasks == null) model.SubTasks = new List<DepartmentTaskEntity>();
                foreach (var item in model.SubTasks)
                {
                    item.CreateUserId = item.ModifyUserId = user.UserId;
                    item.CreateUser = item.ModifyUser = user.UserName;
                    item.CreateTime = item.ModifyTime = DateTime.Now;
                    item.CreateDeptId = user.DeptId;
                    item.CreateDept = user.DeptName;
                    item.ParentTaskId = model.TaskId;
                    if (item.Status != "已完成" && item.Status != "已取消") item.Status = "未开始";
                }

                var bll = new DepartmentTaskBLL();
                bll.Edit(model);

                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                var deptbll = new DepartmentBLL();
                var trainingtype = Config.GetValue("TrainingType");
                foreach (var item in model.SubTasks)
                {
                    if (item.IsPublish == true) continue;

                    var dept = deptbll.GetEntity(item.DutyDepartmentId);
                    if (dept==null) continue;
                    if (dept.Nature != "班组") continue;

                    var job = new MeetingJobEntity();
                    job.JobId = Guid.NewGuid().ToString();
                    job.CreateDate = DateTime.Now;
                    job.CreateUserId = user.UserId;
                    job.GroupId = dept.DepartmentId;
                    job.Relation = new MeetingAndJobEntity();
                    job.Relation.MeetingJobId = Guid.NewGuid().ToString();
                    job.Relation.JobId = job.JobId;
                    job.IsFinished = job.Relation.IsFinished = "undo";
                    job.Relation.JobUserId = item.DutyUserId;
                    job.Relation.JobUser = item.DutyUser;
                    job.Job = item.Content;
                    job.TemplateId = item.TaskId;
                    job.StartTime = DateTime.Parse(item.StartDate.Value.ToString("yyyy/MM/dd") + " 08:30");
                    job.EndTime = DateTime.Parse(item.EndDate.Value.ToString("yyyy/MM/dd") + " 17:30");
                    job.RiskLevel = "低风险";
                    job.TaskType = "管理任务";
                    job.Relation.JobUsers = new List<JobUserEntity>() { new JobUserEntity() { UserId = item.DutyUserId, UserName = item.DutyUser, JobUserId = Guid.NewGuid().ToString(), JobType = "ischecker", CreateDate = DateTime.Now, MeetingJobId = job.Relation.MeetingJobId } };
                    if (job.DangerousList == null) job.DangerousList = new List<JobDangerousEntity>();

                    job = meetBll.PostJob(job);
                    if (string.IsNullOrEmpty(job.Relation.StartMeetingId))
                    {
                        message = "任务已分配至班前会，等待下发。";
                    }
                    else
                    {
                        var meeting = meetBll.GetEntity(job.Relation.StartMeetingId);
                        if (meeting.IsOver)
                            message = "任务已下发。";
                        else
                            message = "任务已分配至班前会，等待下发。";
                    }

                    if (!string.IsNullOrEmpty(job.Relation.StartMeetingId))
                    {
                        var meeting = meetBll.GetDetail(job.Relation.StartMeetingId);
                        if (meeting != null && meeting.IsOver && job.NeedTrain)
                        {
                            if (trainingtype == "人身风险预控")
                            {
                                var training = new HumanDangerTrainingEntity() { TrainingId = Guid.NewGuid().ToString(), TrainingTask = job.Job, CreateTime = DateTime.Now, CreateUserId = user.UserId, MeetingJobId = job.Relation.MeetingJobId, DeptId = job.GroupId, TrainingPlace = job.JobAddr, No = job.TicketCode };
                                if (!string.IsNullOrEmpty(job.TemplateId)) training.HumanDangerId = job.TemplateId;
                                training.TrainingUsers = job.Relation.JobUsers.Select(x => new TrainingUserEntity() { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, TrainingRole = x.JobType == "ischecker" ? 1 : 0 }).ToList();
                                new HumanDangerTrainingBLL().Add(training);
                            }
                            else
                            {
                                var dangerbll = new DangerBLL();
                                var messagebll = new MessageBLL();

                                var danger = dangerbll.Save(job);
                                if (danger != null)
                                    messagebll.SendMessage("危险预知训练", danger.Id);
                            }
                        }
                    }
                    var msgbll = new MessageBLL();
                    msgbll.SendMessage("工作提示", job.Relation.MeetingJobId);

                    item.IsPublish = true;
                }

                bll.Publish(model.SubTasks.Where(x => x.IsPublish == true).ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        [HttpPost]
        public JsonResult Cancel(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var success = true;
            var message = "取消任务成功！";
            try
            {
                var bll = new DepartmentTaskBLL();
                bll.Cancel(id, user.UserName);
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
