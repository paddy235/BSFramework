using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SetManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Bst.Fx.Uploading;
using Newtonsoft.Json;
using Shell32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class WorkMeetingController : MvcControllerBase
    {
        private readonly WorkmeetingBLL workmeetingbll;
        private readonly EducationBLL educationBLL;
        private readonly EducationAnswerBLL educationAnswerBLL;

        /// <summary>
        /// ctor
        /// </summary>
        public WorkMeetingController()
        {
            workmeetingbll = new WorkmeetingBLL();
            educationBLL = new EducationBLL();
            educationAnswerBLL = new EducationAnswerBLL();
        }

        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var model = workmeetingbll.GetWorkMeeting(user.DeptId, DateTime.Now);
            //是否开始班会
            if (model == null)
            {
                //以前的版本不直接生成班前会
                return RedirectToAction("Start");
                //model = workmeetingbll.CreateStartMeeting(user.DeptId, DateTime.Now);
                //return RedirectToAction("Edit", new { id = model.MeetingId });
            }
            else
            {
                //时间排序获取数据 根据最新的一条数据进行判断
                if (model.MeetingType == "班前会")
                {
                    //为班后会则可以判断未昨天的班后会   需要进行今天的班后会
                    if (model.IsOver)
                        return RedirectToAction("End", new { startmeetingid = model.MeetingId });
                    else
                        return RedirectToAction("Edit", new { id = model.MeetingId });
                }
                else
                {
                    //为班前会则可以判断未进行今天的班前会   需要进行今天的班前会
                    if (model.IsOver)
                    {
                        //以前的版本不直接生成班前会
                        return RedirectToAction("Start");
                        //model = workmeetingbll.CreateStartMeeting(user.DeptId, DateTime.Now);
                        //return RedirectToAction("Edit", new { id = model.MeetingId });
                    }
                    else
                        return RedirectToAction("Edit", new { id = model.MeetingId });
                }
            }
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Start()
        {
            return View();
        }

        public JsonResult GetStartMeeting()
        {
            var user = OperatorProvider.Provider.Current();
            var trainingtype = Config.GetValue("TrainingType");
            var meetingset = new WorkOrderBLL().GetWorkOrderList(DateTime.Now, user.DeptId);
            var hasSet = false;
            DateTime? start = null;
            DateTime? over = null;
            string code = null;
            if (meetingset[0] != "无")
            {
                hasSet = true;
                var part = meetingset[1].Split('-');
                start = DateTime.Parse(part[0] + ":00");
                over = DateTime.Parse(part[1] + ":00");
            }
            var begin = start;
            var end = over;
            if (!hasSet)
            {
                begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                end = begin.Value.AddDays(1).AddSeconds(-1);
            }

            var bll = new WorkmeetingBLL();
            var lastmeeting = bll.GetLastMeeting(user.DeptId);
            var meeting = default(WorkmeetingEntity);

            if (lastmeeting == null)
            {
                //没有班会数据
                var jobs = bll.FindMeetingJobs(user.DeptId, DateTime.Today);
                meeting = new WorkmeetingEntity() { Jobs = jobs, ShouldStartTime = start, ShouldEndTime = over, MeetingCode = code };
            }
            else
            {
                if (lastmeeting.MeetingType == "班前会")
                {
                    //班前会
                    if (lastmeeting.IsOver)
                    {
                        var jobs = bll.GetMeetingJobs(lastmeeting.MeetingId);
                        meeting = new WorkmeetingEntity() { Jobs = jobs, OtherMeetingId = lastmeeting.MeetingId, OtherMeetingStartTime = lastmeeting.MeetingStartTime };
                        //结束
                    }
                    else
                    {
                        //未结束
                        meeting = bll.GetDetail(lastmeeting.MeetingId);
                    }
                }
                else
                {
                    //班后会
                    if (lastmeeting.IsOver)
                    {
                        List<MeetingJobEntity> jobs = null;
                        if (hasSet)
                        {
                            //有排班
                            if (lastmeeting.ShouldStartTime == start && lastmeeting.ShouldEndTime == over && lastmeeting.ShouldStartTime.HasValue && lastmeeting.MeetingStartTime.Date == lastmeeting.ShouldStartTime.Value.Date)
                            {
                                //已开班会
                                var jobs2 = bll.FindMeetingJobs2(user.DeptId, DateTime.Now);
                                jobs = jobs2;
                                var longjobs = bll.FindLongJobs(user.DeptId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs2.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                            else
                            {
                                //未开班会
                                jobs = bll.FindMeetingJobs(user.DeptId, DateTime.Now);
                                var longjobs = bll.FindLongJobs(user.DeptId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                        }
                        else
                        {
                            //无排班
                            if (lastmeeting.MeetingStartTime.Date == DateTime.Today)
                            {
                                //已开班会
                                var jobs2 = bll.FindMeetingJobs2(user.DeptId, DateTime.Now);
                                jobs = jobs2;
                                var longjobs = bll.FindLongJobs(user.DeptId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs2.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                            else
                            {
                                //未开班会
                                jobs = bll.FindMeetingJobs(user.DeptId, DateTime.Now);
                                var longjobs = bll.FindLongJobs(user.DeptId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                        }
                        meeting = new WorkmeetingEntity() { Jobs = jobs, ShouldStartTime = start, ShouldEndTime = over, MeetingCode = code };
                    }
                    else
                    {
                        //未结束
                        meeting = bll.GetDetail(lastmeeting.MeetingId);
                        meeting.OtherMeetingStartTime = lastmeeting.MeetingStartTime;
                    }
                }
            }

            return Json(meeting, JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetDetail(string id)
        {
            var model = workmeetingbll.GetDetail(id);
            if (model.Files == null) model.Files = new List<FileInfoEntity>();
            foreach (var item in model.Files)
            {
                item.FilePath = Url.Content(item.FilePath);
            }
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(model, new Newtonsoft.Json.JsonSerializerSettings() { DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat }));
        }


        /// <summary>
        /// 后台保存班前班后会
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        /// <returns></returns>
        public ActionResult SaveManagerForm(string keyValue, WorkmeetingEntity entity)
        {
            workmeetingbll.ManagerSaveForm(keyValue, entity);
            return Success("操作成功。");
        }


        /// <summary>
        /// 后台保存班前班后会数据删除
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                FileInfoBLL fileBll = new FileInfoBLL();
                var startmeeting = workmeetingbll.GetDetail(keyValue);//班前会  
                if (startmeeting != null)
                {
                    var endmeeting = workmeetingbll.GetDetail(startmeeting.OtherMeetingId);//班后会
                    if (endmeeting != null)
                    {
                        foreach (var item in endmeeting.Files)
                        {//班后附件
                            fileBll.DeleteFile(endmeeting.MeetingId, item.FileName, Server.MapPath(item.FilePath));
                        }
                        workmeetingbll.DeltetMeeting(endmeeting.MeetingId);
                    }
                    foreach (var item in startmeeting.Files)
                    {//班前附件
                        fileBll.DeleteFile(startmeeting.MeetingId, item.FileName, Server.MapPath(item.FilePath));
                    }
                    workmeetingbll.DeltetMeeting(startmeeting.MeetingId);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return Success("操作成功。");
        }

        public JsonResult GetDangerous()
        {
            var user = OperatorProvider.Provider.Current();

            RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
            var list = riskFactorSetBLL.GetList(user.DeptId, string.Empty);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeasures(string dangerid)
        {
            var user = OperatorProvider.Provider.Current();

            MeasureSetBLL measureSetBLL = new MeasureSetBLL();
            var list = measureSetBLL.GetList(dangerid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult End(string startmeetingid)
        {
            if (string.IsNullOrEmpty(startmeetingid)) return View(new WorkmeetingEntity() { Jobs = new List<MeetingJobEntity>(), Signins = new List<MeetingSigninEntity>() });

            var user = OperatorProvider.Provider.Current();

            var model = default(WorkmeetingEntity);
            var trainingtype = Config.GetValue("TrainingType");
            model = workmeetingbll.BuildWorkMeeting(startmeetingid, user.DeptId, DateTime.Now, trainingtype);

            model.OtherMeetingId = startmeetingid;
            model.MeetingPerson = user.UserName;

            return View(model);
        }

        [HttpPost]
        public JsonResult Start(WorkmeetingEntity model)
        {
            var now = DateTime.Now;
            var user = OperatorProvider.Provider.Current();
            var bll = new WorkmeetingBLL();

            model.MeetingId = Guid.NewGuid().ToString();
            model.MeetingType = "班前会";
            model.MeetingStartTime = DateTime.Now;
            model.GroupId = user.DeptId;
            model.GroupName = user.DeptName;
            model.IsStarted = true;
            var file = BuildImage(model.MeetingId, "班前班后会");
            model.Files = new List<FileInfoEntity>() { file };
            if (model.Jobs == null) model.Jobs = new List<MeetingJobEntity>();
            model.Jobs.ForEach(x => x.GroupId = user.DeptId);
            bll.UpdateJobs(model.Jobs);

            model = bll.StartMeeting(model, DateTime.Now, user.UserName);

            return Json(new { success = true, data = model });
        }

        private FileInfoEntity BuildImage(string meetingid, string meetingtype)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(meetingid + "|" + meetingtype, Encoding.UTF8);
            var path = "~/Resource/DocumentFile/";
            if (!Directory.Exists(Server.MapPath(path)))
                Directory.CreateDirectory(Server.MapPath(path));

            image.Save(Path.Combine(Server.MapPath(path), id + ".jpg"));

            var user = OperatorProvider.Provider.Current();

            return new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = "二维码", FileExtensions = ".jpg", FileName = id + ".jpg", FilePath = path + id + ".jpg", FileType = "jpg", ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = meetingid };
        }

        [HttpPost]
        public ActionResult End(WorkmeetingEntity model, FormCollection fc)
        {
            if (string.IsNullOrEmpty(model.OtherMeetingId)) return RedirectToAction("Start");

            model.MeetingId = Guid.NewGuid().ToString();
            model.MeetingStartTime = DateTime.Now;
            model.IsStarted = true;
            model.Files = new List<FileInfoEntity>();

            var file = this.BuildImage(model.MeetingId, "班前班后会");
            model.Files.Add(file);

            if (model.Jobs == null)
                model.Jobs = new List<MeetingJobEntity>();

            if (model.Signins == null)
                model.Signins = new List<MeetingSigninEntity>();

            try
            {
                workmeetingbll.AddEndMeeting(model);
            }
            catch (Exception e)
            {
                if (e.Message == "meeting is not over")
                    return this.RedirectToAction("Index");
            }

            return RedirectToAction("Edit", new { id = model.MeetingId });
        }

        [HttpPost]
        public JsonResult Edit(WorkmeetingEntity model)
        {
            var user = OperatorProvider.Provider.Current();

            if (model.IsOver)
                model.MeetingEndTime = DateTime.Now;

            model.MeetingPerson = user.UserName;
            if (model.Jobs == null) model.Jobs = new List<MeetingJobEntity>();
            foreach (var item in model.Jobs)
            {
                item.CreateUserId = user.UserId;

                if (string.IsNullOrEmpty(item.JobId))
                {
                    if (string.IsNullOrEmpty(item.JobType))
                        item.JobType = "班前班后会";
                    if (string.IsNullOrEmpty(item.JobId))
                        item.JobId = Guid.NewGuid().ToString();
                    item.IsFinished = "undo";
                    item.GroupId = model.GroupId;
                    item.CreateDate = DateTime.Now;
                    item.CreateUserId = user.UserId;
                    item.Relation.StartMeetingId = model.MeetingId;
                    item.Relation.IsFinished = item.IsFinished;
                    item.Relation.JobId = item.JobId;
                    item.Relation.JobUserId = string.Join(",", item.Relation.JobUsers.Select(x => x.UserId));
                    item.Relation.JobUser = string.Join(",", item.Relation.JobUsers.Select(x => x.UserName));
                    if (string.IsNullOrEmpty(item.Relation.MeetingJobId))
                        item.Relation.MeetingJobId = Guid.NewGuid().ToString();
                    foreach (var item1 in item.Relation.JobUsers)
                    {
                        item1.JobUserId = Guid.NewGuid().ToString();
                        item1.CreateDate = DateTime.Now;
                        item1.MeetingJobId = item.Relation.MeetingJobId;
                    }
                    if (item.DangerousList == null) item.DangerousList = new List<JobDangerousEntity>();
                    foreach (var item1 in item.DangerousList)
                    {
                        item1.JobDangerousId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobId = item.JobId;
                        foreach (var item2 in item1.MeasureList)
                        {
                            item2.JobMeasureId = Guid.NewGuid().ToString();
                            item2.CreateTime = DateTime.Now;
                            item2.JobDangerousId = item1.JobDangerousId;
                        }
                    }
                }
                else
                {
                    item.Relation.JobUserId = string.Join(",", item.Relation.JobUsers.Select(x => x.UserId));
                    item.Relation.JobUser = string.Join(",", item.Relation.JobUsers.Select(x => x.UserName));
                    foreach (var item1 in item.Relation.JobUsers)
                    {
                        if (string.IsNullOrEmpty(item1.JobUserId))
                        {
                            item1.JobUserId = Guid.NewGuid().ToString();
                            item1.MeetingJobId = item.Relation.MeetingJobId;
                        }
                    }
                    if (item.DangerousList == null) item.DangerousList = new List<JobDangerousEntity>();
                    foreach (var item1 in item.DangerousList)
                    {
                        if (string.IsNullOrEmpty(item1.JobDangerousId))
                        {
                            item1.JobDangerousId = Guid.NewGuid().ToString();
                            item1.CreateTime = DateTime.Now;
                            item1.JobId = item.JobId;
                        }
                        if (item1.MeasureList == null) item1.MeasureList = new List<JobMeasureEntity>();
                        foreach (var item2 in item1.MeasureList)
                        {
                            if (string.IsNullOrEmpty(item2.JobMeasureId))
                            {
                                item2.JobMeasureId = Guid.NewGuid().ToString();
                                item2.CreateTime = DateTime.Now;
                                item2.JobDangerousId = item1.JobDangerousId;
                            }
                        }
                    }
                }
            }

            if (model.MeetingType == "班前会")
            {
                workmeetingbll.EditStartMeeting(model, OperatorProvider.Provider.Current().UserId);
            }
            else
            {
                workmeetingbll.EditEndMeeting(model, OperatorProvider.Provider.Current().UserId);
            }

            if (model.IsOver)
            {
                var messagebll = new MessageBLL();
                if (model.MeetingType == "班前会")
                {
                    foreach (var item in model.Jobs)
                    {
                        messagebll.SendMessage("工作提示", item.Relation.MeetingJobId);
                    }
                }
                else
                {
                    foreach (var item in model.Jobs)
                    {
                        messagebll.FinishTodo("工作提示", item.Relation.MeetingJobId);
                    }
                }
            }

            model = workmeetingbll.GetDetail(model.MeetingId);
            foreach (var item in model.Files)
            {
                item.FilePath = Url.Content(item.FilePath);
            }

            return Json(model);
        }

        public ViewResult Edit(string id)
        {
            ViewBag.id = id;
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var type = itembll.GetEntityByName("缺勤原因");
            var content = itemdetialbll.GetList(type.ItemId).ToList();
            var onduty = content.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName }).ToList();
            ViewData["onduty"] = onduty;
            //var user = OperatorProvider.Provider.Current();
            //var measures = workmeetingbll.GetMeasures(user.DeptId);
            //ViewBag.measure = measures;

            //var dangerous = workmeetingbll.GetDangerous(user.DeptId);
            //ViewBag.dangerous = dangerous;

            //UserBLL userBLL = new UserBLL();
            //var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            //ViewData["users"] = users;

            //var model = workmeetingbll.GetDetail(id);
            ////model.MeetingStartTime = DateTime.Now;
            //ViewBag.MeetingType = model.MeetingType;
            //ViewBag.dutyperson = model.DutyPerson == null || model.DutyPerson.Count == 0 ? "无" : string.Join(",", model.DutyPerson.Select(x => x.UserName).Distinct());
            //if (model.Files.Count(x => x.Description == "二维码") > 0)
            //{
            //    ViewBag.qr = model.Files.SingleOrDefault(x => x.Description == "二维码").FileId;
            //}
            //else
            //{
            //    ViewBag.qr = string.Empty;
            //}

            //if (model.Files == null) model.Files = new List<FileInfoEntity>();
            //foreach (var item in model.Files)
            //{
            //    item.FilePath = string.Format("{0}://{1}{2}", this.Request.Url.Scheme, this.Request.Url.Host, Url.Content(item.FilePath));
            //}

            //model.Jobs = model.Jobs.OrderBy(x => x.CreateDate).ToList();

            return View();
        }

        public ActionResult List(int page, int pagesize, string from, string to, FormCollection fc)
        {
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            if (string.IsNullOrEmpty(from)) from = fc.Get("MeetingStartTime");
            if (string.IsNullOrEmpty(to)) to = fc.Get("MeetingEndTime");

            ViewData["from"] = from;
            ViewData["to"] = to;

            var user = OperatorProvider.Provider.Current();

            var total = 0;
            var data = workmeetingbll.GetList(new string[] { user.DeptId }, string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), null, null, page, pagesize, out total);
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            return View(data);
        }

        public ActionResult Detail(string id)
        {
            var bll = new ActivityBLL();
            var startmeeting = workmeetingbll.GetDetail(id);
            if (startmeeting == null) startmeeting = new WorkmeetingEntity();

            var endmeeting = default(WorkmeetingEntity);
            if (startmeeting == null || string.IsNullOrEmpty(startmeeting.OtherMeetingId))
                endmeeting = new WorkmeetingEntity() { Signins = new List<MeetingSigninEntity>(), Files = new List<FileInfoEntity>(), Jobs = new List<MeetingJobEntity>() };
            else
            {
                endmeeting = workmeetingbll.GetDetail(startmeeting.OtherMeetingId);
                endmeeting.Evaluates = bll.GetEntityList().Where(x => x.Activityid == startmeeting.OtherMeetingId).ToList();
            }
            ViewData["users"] = string.Join("、", startmeeting.Signins.Where(x => x.IsSigned).Select(x => x.PersonName));
            if (startmeeting.Signins.Count(x => !x.IsSigned) > 0)
            {
                ViewData["queqin"] = string.Join("；", startmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName + "：" + x.Reason));
            }
            else
            {
                ViewData["queqin"] = "无";
            }
            if (startmeeting.Signins.Count(x => x.MentalCondition != "正常") > 0)
            {
                ViewData["state"] = string.Join("、", startmeeting.Signins.Where(x => x.MentalCondition != "正常").Select(x => x.PersonName + "：" + x.MentalCondition));
            }
            else
            {
                ViewData["state"] = "正常";
            }
            if (endmeeting != null)
            {
                ViewData["users2"] = string.Join("、", endmeeting.Signins.Where(x => x.IsSigned).Select(x => x.PersonName));
                if (endmeeting.Signins.Count(x => !x.IsSigned) > 0)
                {
                    ViewData["queqin2"] = string.Join("；", endmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName + "：" + x.Reason));
                }
                else
                {
                    ViewData["queqin2"] = "无";
                }
            }

            return View(new List<WorkmeetingEntity>() { startmeeting, endmeeting });
        }

        public ActionResult ViewJobs()
        {
            var user = OperatorProvider.Provider.Current();
            var data = workmeetingbll.GetGroupJobs(user.DeptId);
            if (data.Count > 0)
            {
                FileInfoBLL bll = new FileInfoBLL();
                foreach (var item in data)
                {
                    var file = bll.GetFilesByRecIdNew(item.Relation.MeetingJobId);
                    item.Files = file.Where(x => x.Description == "照片").ToList();
                    item.FileList1 = file.Where(x => x.Description == "音频").ToList();
                }
            }
            return View(data);
        }

        public JsonResult DeleteFile(string fileid)
        {
            var filebll = new FileInfoBLL();
            var filepath = filebll.Delete(fileid);
            if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                System.IO.File.Delete(Server.MapPath(filepath));

            return Json(new { success = true, msg = string.Empty });
        }

        /// <summary>
        /// 新增任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult NewJob(string id)
        {
            ViewBag.meetingid = id;
            return View();
        }

        /// <summary>
        /// 新增任务-提交数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult NewJob(MeetingJobEntity model)
        {
            var user = OperatorProvider.Provider.Current();

            model.JobId = Guid.NewGuid().ToString();
            model.CreateDate = DateTime.Now;
            model.CreateUserId = user.UserId;
            model.GroupId = user.DeptId;
            model.Relation.MeetingJobId = Guid.NewGuid().ToString();
            model.Relation.JobId = model.JobId;
            model.Relation.IsFinished = model.IsFinished = "undo";
            model.Relation.JobUserId = string.Join(",", model.Relation.JobUsers.Select(x => x.UserId));
            model.Relation.JobUser = string.Join(",", model.Relation.JobUsers.Select(x => x.UserName));
            foreach (var item in model.Relation.JobUsers)
            {
                item.JobUserId = Guid.NewGuid().ToString();
                item.CreateDate = DateTime.Now;
                item.MeetingJobId = model.Relation.MeetingJobId;
            }
            if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();
            foreach (var item in model.DangerousList)
            {
                item.JobDangerousId = Guid.NewGuid().ToString();
                item.CreateTime = DateTime.Now;
                item.JobId = model.JobId;
                foreach (var item1 in item.MeasureList)
                {
                    item1.JobMeasureId = Guid.NewGuid().ToString();
                    item1.CreateTime = DateTime.Now;
                    item1.JobDangerousId = item.JobDangerousId;
                }
            }

            var success = true;
            var message = string.Empty;
            try
            {
                new WorkmeetingBLL().AddNewJob(model);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Json(new { success, message, data = model });
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        [HttpPost]
        public ActionResult CancelJob(string Id, string meetingjobid)
        {
            var user = OperatorProvider.Provider.Current();

            //取消任务
            workmeetingbll.CancelJob(Id, meetingjobid);
            new MessageBLL().SendMessage("工作任务取消", meetingjobid);

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();

            ViewData["users"] = users;
            ViewData["JobTime"] = string.Empty;
            ViewBag.LoadData = "false";
            return View("ChangeJob");
        }

        /// <summary>
        /// 变更任务
        /// </summary>
        /// <returns></returns>
        public ViewResult ChangeJob(string id, string meetingjobid)
        {
            var model = workmeetingbll.GetJobDetail(id, meetingjobid, null);
            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();

            ViewData["users"] = users;
            ViewData["JobTime"] = string.Empty;
            ViewBag.LoadData = "true";
            ViewBag.dangerousList = Newtonsoft.Json.JsonConvert.SerializeObject(model.DangerousList);

            if (model.Relation.JobUsers.Count == 0)
                model.Relation.JobUsers.Add(new JobUserEntity() { });

            return View(model);
        }

        /// <summary>
        /// 变更任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeJob(MeetingJobEntity model, FormCollection fc)
        {
            var time = fc.Get("JobTime");

            //var parts = time.Replace(" - ", "|").Split('|');
            //model.StartTime = DateTime.Parse(parts[0].Trim());
            //model.EndTime = DateTime.Parse(parts[1].Trim());

            var json = fc.Get("Persons");
            var persons = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JobUserEntity>>(json);
            model.Relation.JobUsers = persons;
            json = fc.Get("dangerousList");
            model.DangerousList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JobDangerousEntity>>(json);
            if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();
            foreach (var item in model.DangerousList)
            {
                if (string.IsNullOrEmpty(item.JobDangerousId))
                {
                    item.JobDangerousId = Guid.NewGuid().ToString();
                    item.CreateTime = DateTime.Now;
                    item.JobId = model.JobId;
                    if (item.MeasureList == null) item.MeasureList = new List<JobMeasureEntity>();
                    foreach (var item1 in item.MeasureList)
                    {
                        if (string.IsNullOrEmpty(item1.JobMeasureId))
                        {
                            item1.JobMeasureId = Guid.NewGuid().ToString();
                            item1.JobDangerousId = item.JobDangerousId;
                            item1.CreateTime = DateTime.Now;
                        }
                    }
                }
            }

            if (model.Relation.JobUsers != null)
            {
                foreach (var item in model.Relation.JobUsers)
                {
                    if (string.IsNullOrEmpty(item.JobUserId)) item.JobUserId = Guid.NewGuid().ToString();

                    item.MeetingJobId = model.Relation.MeetingJobId;
                    item.CreateDate = DateTime.Now;
                }
            }
            model.IsFinished = "undo";

            workmeetingbll.ChangeJob(model);

            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();

            ViewData["users"] = users;
            ViewData["JobTime"] = string.Format("{0} - {1}", model.StartTime.ToString("yyyy/M/d H:mm"), model.EndTime.ToString("yyyy/M/d H:mm"));

            ViewBag.callback = "jQuery(function(){var pp = jQuery(parent).get(0).fn$callback();});";
            ViewBag.LoadData = "false";

            return View(model);
        }


        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = workmeetingbll.GetDetail(keyValue);
            return ToJsonResult(data);
        }

        public ActionResult Index2(string type)
        {
            var user = OperatorProvider.Provider.Current();

            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
            ViewBag.from = sdt.ToString("yyyy-MM-dd");
            ViewBag.to = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.type = type;

            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;

            var today = DateTime.Today;
            var start1 = new DateTime(today.Year, today.Month, today.Day);
            var end1 = start1.AddMonths(1).AddSeconds(-1);
            var start2 = today.AddDays(-(int)today.DayOfWeek);
            var end2 = start2.AddDays(7).AddSeconds(-1);
            var dt1 = new DataTable();
            var dt2 = new DataTable();
            var monthtotal = 0;
            var weektotal = 0;
            //var depts = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, null);
            //var total = 0;
            //var data = workmeetingbll.GetList(depts.Select(x => x.DepartmentId).ToArray(), user.UserId, start1, end1, "",false, 1, 1000000, out total);
            //workmeetingbll.workSetmeeting( start1, end1, data, out dt2, out monthtotal);
            //workmeetingbll.workSetmeeting(getdeptId, start2, end2, dt1, out dt2, out weektotal);
            ViewBag.monthtotal = monthtotal;
            ViewBag.weektotal = weektotal;

            #region  今日工作统计

            var depts = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, "");
            var DeptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            Dictionary<string, int> dic = new WorkmeetingBLL().TodayWorkStatistics(DeptStr, DateTime.Now);
            ViewBag.TodayWork = dic;
            #endregion

            return View();
        }


        #region 获取机构部门组织树菜单
        private OrganizeBLL organizeBLL = new OrganizeBLL();

        private DepartmentBLL departmentBLL = new DepartmentBLL();

        /// <summary>
        /// 获取当前电厂下所有的班组
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public ActionResult GetDeptTreeJson(string Ids, int checkMode = 0, int mode = 0)
        {
            OrganizeBLL organizeBLL = new OrganizeBLL();
            if (Ids == "0")
            {
                //存在机构 否则为0
                var getIds = organizeBLL.GetList().FirstOrDefault(x => x.ParentId == "0");
                if (getIds != null)
                {
                    Ids = getIds.OrganizeId;
                }
            }
            Operator user = OperatorProvider.Provider.Current();
            List<OrganizeEntity> organizedata = new List<OrganizeEntity>();
            List<DepartmentEntity> departmentdata = new List<DepartmentEntity>();
            string roleNames = user.RoleName;
            if (user.IsSystem)
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                organizedata = organizedata1.ToList();
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }
                departmentdata = departmentdata1.ToList();
            }
            else
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }

                organizedata = organizedata1.Where(t => t.OrganizeId == user.OrganizeId).OrderByDescending(x => x.CreateDate).ToList();
                departmentdata = departmentdata1.OrderBy(x => x.SortCode).ToList();
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                #region 部门
                TreeEntity tree = new TreeEntity();
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                if (item.ParentId == "0")
                {
                    tree.parentId = item.OrganizeId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                if (item.Nature == "班组")
                {
                    tree.Attribute = "Code";
                    tree.AttributeValue = item.EnCode;
                }
                if (item.Nature == "班组" || item.Nature == "部门")
                {
                    tree.showcheck = checkMode == 0 ? false : true;
                }
                treeList.Add(tree);
                #endregion
            }
            //从整个树形中截取
            var gettreeList = new List<TreeEntity>();
            getTree(treeList, Ids, gettreeList);

            if (gettreeList.Count > 0)
            {

                var parent = treeList.FirstOrDefault(x => x.id == Ids);
                if (parent == null)
                {
                    if (organizedata.Count == 1)
                    {
                        parent = treeList.FirstOrDefault(x => x.parentId == Ids);
                    }
                }
                if (parent != null)
                {
                    var one = gettreeList.FirstOrDefault(x => x.id == parent.id);
                    if (one == null)
                    {
                        parent.parentId = "0";
                        gettreeList.Add(parent);
                    }

                }

            }
            else
            {
                var one = treeList.FirstOrDefault(x => x.id == Ids);
                if (one != null)
                {
                    one.parentId = "0";
                    gettreeList.Add(one);
                }
            }
            return Content(gettreeList.TreeToJson());
        }
        private void getTree(List<TreeEntity> my, string id, List<TreeEntity> get)
        {
            var go = my.Where(x => x.parentId == id).ToList();

            if (go.Count > 0)
            {
                get.AddRange(go);
            }
            foreach (var item in go)
            {
                getTree(my, item.id, get);
            }

        }
        private List<TreeEntity> GetDeptTreeId(string Ids, int checkMode = 0, int mode = 0)
        {
            OrganizeBLL organizeBLL = new OrganizeBLL();
            if (Ids == "0")
            { //存在机构 否则为0
                var getIds = organizeBLL.GetList().FirstOrDefault(x => x.ParentId == "0");
                if (getIds != null)
                {
                    Ids = getIds.OrganizeId;
                }
            }
            Operator user = OperatorProvider.Provider.Current();
            List<OrganizeEntity> organizedata = new List<OrganizeEntity>();
            List<DepartmentEntity> departmentdata = new List<DepartmentEntity>();
            string roleNames = user.RoleName;
            if (user.IsSystem)
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                organizedata = organizedata1.ToList();
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }
                departmentdata = departmentdata1.ToList();
            }
            else
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }

                organizedata = organizedata1.Where(t => t.OrganizeId == user.OrganizeId).OrderByDescending(x => x.CreateDate).ToList();
                departmentdata = departmentdata1.OrderBy(x => x.SortCode).ToList();
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                #region 部门
                TreeEntity tree = new TreeEntity();
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                if (item.ParentId == "0")
                {
                    tree.parentId = item.OrganizeId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                if (item.Nature == "班组")
                {
                    tree.Attribute = "Code";
                    tree.AttributeValue = item.EnCode;
                }
                if (item.Nature == "班组" || item.Nature == "部门")
                {
                    tree.showcheck = checkMode == 0 ? false : true;
                }
                treeList.Add(tree);
                #endregion
            }
            var gettreeList = new List<TreeEntity>();
            getTree(treeList, Ids, gettreeList);

            if (gettreeList.Count > 0)
            {
                var parent = treeList.FirstOrDefault(x => x.id == Ids);
                if (parent == null)
                {
                    if (organizedata.Count == 1)
                    {
                        parent = treeList.FirstOrDefault(x => x.parentId == Ids);
                    }
                }
                if (parent != null)
                {
                    var one = gettreeList.FirstOrDefault(x => x.id == parent.id);
                    if (one == null)
                    {
                        parent.parentId = "0";
                        gettreeList.Add(parent);
                    }

                }

            }
            else
            {
                var one = treeList.FirstOrDefault(x => x.id == Ids);
                gettreeList.Add(one);
                var two = treeList.FirstOrDefault(x => x.id == one.parentId);
                if (two != null)
                {
                    two.parentId = "0";
                    gettreeList.Add(two);
                }


            }

            return gettreeList.ToList();
        }
        #endregion
        public ContentResult GetData()
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var department = this.Request.QueryString.Get("department");
            var team = this.Request.QueryString.Get("team");
            var sdate = this.Request.QueryString.Get("meetingstarttime");
            var edate = this.Request.QueryString.Get("meetingendtime");

            var total = 0;
            var data = workmeetingbll.GetData(pagesize, page, out total, new Dictionary<string, string>() { { "departmentid", user.DeptId }, { "department", department }, { "team", team }, { "meetingstarttime", sdate }, { "meetingendtime", edate } });
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" }));
        }
        public ContentResult GetDataNew(string type, string deptid)
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var department = this.Request.QueryString.Get("department");
            var team = this.Request.QueryString.Get("team");
            var sdate = this.Request.QueryString.Get("meetingstarttime");
            var edate = this.Request.QueryString.Get("meetingendtime");
            var orderby = this.Request.QueryString.Get("orderby");
            var appraise = this.Request.QueryString.Get("appraise");
            orderby = orderby == "升" ? "ASC" : "DESC";
            var total = 0;
            if (!string.IsNullOrEmpty(department))
            {
                deptid = department;
                //调整权限
            }
            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);
            int mytotal = 0;
            #region 初始化时间 一周一月
            var time = DateTime.Now;
            var year = time.Year;
            var month = time.Month;
            var day = time.Day;
            var monthSum = Util.Time.GetDaysOfMonth(time);
            var monthStartTime = new DateTime(year, month, 1);
            var monthEndTime = new DateTime(year, month, day);
            var week = (int)time.DayOfWeek;
            var sumday = -(week - 1);
            var weekStartTime = monthEndTime.AddDays(sumday);
            var weekEndTime = weekStartTime.AddDays(6);
            #endregion
            var data = workmeetingbll.GetDataNew(pagesize, page, out total, new Dictionary<string, string>() { { "appraise", appraise }, { "departmentid", deptid }, { "team", team }, { "meetingstarttime", sdate }, { "meetingendtime", edate }, { "orderby", orderby } }, depts.Select(x => x.DepartmentId).ToArray()) as DataTable;
            if (type == "4")  //首页工作台账跳转，查询本季度数据
            {
                month = 1;
                if (DateTime.Now.Month < 4) month = 1;
                else if (DateTime.Now.Month < 7) month = 4;
                else if (DateTime.Now.Month < 10) month = 7;
                else if (DateTime.Now.Month <= 12) month = 10;
                DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
                var from = sdt.ToString("yyyy-MM-dd");
                var to = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                data = workmeetingbll.GetDataNew(pagesize, page, out total, new Dictionary<string, string>() { { "appraise", appraise }, { "departmentid", deptid }, { "team", team }, { "meetingstarttime", from }, { "meetingendtime", to }, { "orderby", orderby } }, depts.Select(x => x.DepartmentId).ToArray()) as DataTable;
            }
            DataTable mydata = new DataTable();
            //获取部门id
            var getdeptId = GetDeptTreeId(deptid);
            //比较数据
            workmeetingbll.workSetmeeting(getdeptId, monthStartTime, monthEndTime, data, out mydata, out mytotal);
            int monthTotal = mytotal;
            workmeetingbll.workSetmeeting(getdeptId, weekStartTime, monthEndTime, data, out mydata, out mytotal);
            int weekTotal = mytotal;
            FileInfoBLL bll = new FileInfoBLL();
            //加入总计数量
            var path = AppDomain.CurrentDomain.BaseDirectory;
            //var path = Url.Content("~").Substring(0, @Url.Content("~").Length - 1);
            //添加照片展示  添加音频展示 去除编码差异
            for (int i = 0; i < data.Rows.Count; i++)
            {

                //data.Rows[i]["aftertime"] = data.Rows[i]["aftertime"].ToString().Replace('?', ':');
                //data.Rows[i]["beforetime"] = data.Rows[i]["beforetime"].ToString().Replace('?', ':');
                data.Rows[i]["monthTotal"] = monthTotal;
                data.Rows[i]["weekTotal"] = weekTotal;
                data.Rows[i]["usedeptid"] = deptid;
                var beforemeetingid = data.Rows[i]["beforemeetingid"].ToString();
                var aftermeetingid = data.Rows[i]["aftermeetingid"].ToString();
                if (!string.IsNullOrEmpty(beforemeetingid))
                {


                    var beforefile = bll.GetFilesByRecIdNew(beforemeetingid);
                    if (beforefile.Count > 0)
                    {
                        data.Rows[i]["beforepic"] = beforefile.Where(x => x.Description == "照片").Count();
                        //data.Rows[i]["beforevideo"] = beforefile.Where(x => x.Description == "音频").Count();
                        var modelpath = beforefile.FirstOrDefault(x => x.Description == "音频");
                        if (modelpath != null)
                        {
                            string filePath = path + modelpath.FilePath.Replace("/", "\\").Substring(1, modelpath.FilePath.Length - 1);
                            var timeStr = Util.WMP.GetDurationByWMPLib(filePath);
                            data.Rows[i]["beforevideo"] = timeStr;
                        }
                        else
                        {
                            data.Rows[i]["beforevideo"] = "";
                        }
                    }
                }
                if (!string.IsNullOrEmpty(aftermeetingid))
                {

                    var afterfile = bll.GetFilesByRecIdNew(aftermeetingid);
                    if (afterfile.Count > 0)
                    {
                        data.Rows[i]["afterpic"] = afterfile.Where(x => x.Description == "照片").Count();
                        //data.Rows[i]["aftervideo"] = afterfile.Where(x => x.Description == "音频").Count();
                        var modelpath = afterfile.FirstOrDefault(x => x.Description == "音频");
                        if (modelpath != null)
                        {
                            string filePath = path + modelpath.FilePath.Replace("/", "\\").Substring(1, modelpath.FilePath.Length - 1);
                            var timeStr = Util.WMP.GetDurationByWMPLib(filePath);
                            data.Rows[i]["aftervideo"] = timeStr;
                        }
                        else
                        {

                            data.Rows[i]["aftervideo"] = "";
                        }

                    }

                }
            }
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" }));
        }


        private static string Index3department = "";
        private static string Index3type = "";
        public ActionResult Index3(string type)
        {
            // "&department=" + deptid + "&type=" + type;
            #region 初始化时间 一周一月
            var time = DateTime.Now;
            var year = time.Year;
            var month = time.Month;
            var day = time.Day;
            var monthSum = Util.Time.GetDaysOfMonth(time);
            var monthStartTime = new DateTime(year, month, 1);
            var monthEndTime = new DateTime(year, month, day);
            var week = Util.Time.GetWeekNumberOfDay(time);
            var sumday = -(week - 1);
            var weekStartTime = monthEndTime.AddDays(sumday);
            var weekEndTime = weekStartTime.AddDays(6);
            #endregion
            ViewBag.weekStartTime = weekStartTime.ToString("yyyy-MM-dd");
            ViewBag.monthStartTime = monthStartTime.ToString("yyyy-MM-dd");
            ViewBag.monthEndTime = monthEndTime.ToString("yyyy-MM-dd");
            ViewBag.type = type;
            Index3type = type;

            month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
            ViewBag.seasonStartTime = sdt.ToString("yyyy-MM-dd");
            return View();
        }
        public ContentResult GetWorkMeet()
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var type = this.Request.QueryString.Get("type");
            var department = this.Request.QueryString.Get("department");
            var team = this.Request.QueryString.Get("team");
            var sdate = this.Request.QueryString.Get("meetingstarttime");
            var edate = this.Request.QueryString.Get("meetingendtime");
            //默认排序
            var orderby = this.Request.QueryString.Get("orderby");
            orderby = orderby == "升" ? "ASC" : "DESC";
            var total = 0;
            var detpid = string.Empty;
            if (string.IsNullOrEmpty(department))
            {
                if (!string.IsNullOrEmpty(Index3department))
                {
                    detpid = Index3department;
                }
                else
                {
                    detpid = user.DeptId;
                }

            }
            else
            {
                detpid = department;
            }
            var depts = new DepartmentBLL().GetSubDepartments(detpid, null);
            DataTable mydata = new DataTable();
            #region 初始化时间 一周一月
            var time = DateTime.Now;
            var year = time.Year;
            var month = time.Month;
            var day = time.Day;
            var monthSum = Util.Time.GetDaysOfMonth(time);
            var monthStartTime = new DateTime(year, month, 1);
            var monthEndTime = new DateTime(year, month, day);
            var week = Util.Time.GetWeekNumberOfDay(time);
            var sumday = -(week - 1);
            var weekStartTime = monthEndTime.AddDays(sumday);
            var weekEndTime = weekStartTime.AddDays(6);

            // 20181120 当前季度开始日期
            month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
            #endregion
            var data = new DataTable();
            int mytotal = 0;
            if (string.IsNullOrEmpty(type))
            {
                if (Index3type == "1")
                {
                    data = workmeetingbll.GetDataNew(pagesize, page, out total, new Dictionary<string, string>() { { "departmentid", detpid }, { "team", team }, { "meetingstarttime", weekStartTime.ToString("yyyy-MM-dd") }, { "meetingendtime", weekEndTime.AddDays(1).ToString("yyyy-MM-dd") }, { "orderby", orderby } }, depts.Select(x => x.DepartmentId).ToArray()) as DataTable;
                    //获取部门id
                    var getdeptId = GetDeptTreeId(detpid);
                    workmeetingbll.workSetmeeting(getdeptId, weekStartTime, monthEndTime, data, out mydata, out mytotal);
                }
                else if (Index3type == "3")   // 20181120  新增，季度数据
                {
                    data = workmeetingbll.GetDataNew(pagesize, page, out total, new Dictionary<string, string>() { { "departmentid", detpid }, { "team", team }, { "meetingstarttime", sdt.ToString("yyyy-MM-dd") }, { "meetingendtime", monthEndTime.AddDays(1).ToString("yyyy-MM-dd") }, { "orderby", orderby } }, depts.Select(x => x.DepartmentId).ToArray()) as DataTable;
                    //获取部门id
                    var getdeptId = GetDeptTreeId(detpid);
                    workmeetingbll.workSetmeeting(getdeptId, sdt, monthEndTime, data, out mydata, out mytotal);
                }
                else
                {
                    data = workmeetingbll.GetDataNew(pagesize, page, out total, new Dictionary<string, string>() { { "departmentid", detpid }, { "team", team }, { "meetingstarttime", monthStartTime.ToString("yyyy-MM-dd") }, { "meetingendtime", monthEndTime.AddDays(1).ToString("yyyy-MM-dd") }, { "orderby", orderby } }, depts.Select(x => x.DepartmentId).ToArray()) as DataTable;
                    //获取部门id
                    var getdeptId = GetDeptTreeId(detpid);
                    workmeetingbll.workSetmeeting(getdeptId, monthStartTime, monthEndTime, data, out mydata, out mytotal);
                }
            }
            else
            {
                data = workmeetingbll.GetDataNew(pagesize, page, out total, new Dictionary<string, string>() { { "departmentid", detpid }, { "team", team }, { "meetingstarttime", sdate }, { "meetingendtime", edate }, { "orderby", orderby } }, depts.Select(x => x.DepartmentId).ToArray()) as DataTable;
                //获取部门id
                var getdeptId = GetDeptTreeId(detpid);
                sdate = string.IsNullOrEmpty(sdate) ? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") : sdate;
                edate = string.IsNullOrEmpty(edate) ? DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd") : edate;
                workmeetingbll.workSetmeeting(getdeptId, DateTime.Parse(sdate), DateTime.Parse(edate), data, out mydata, out mytotal);
            }
            DataTable returnData = mydata.Clone();
            if (!string.IsNullOrEmpty(team) && mydata.Rows.Count > 0)
            {
                var group = from t in mydata.AsEnumerable()
                            where t.Field<string>("workset").Contains(team)
                            || t.Field<string>("team").Contains(team)
                            || t.Field<string>("department").Contains(team)
                            select t;
                if (group.Count() == 0)
                {
                    mydata.Clear();
                }
                else { mydata = group.CopyToDataTable(); }

            }
            mytotal = mydata.Rows.Count;
            int go = pagesize * (page - 1);
            int end = (pagesize * page) - mydata.Rows.Count;
            if (pagesize > end && (pagesize * page) > mydata.Rows.Count)
            {
                int over = pagesize - end;
                for (int i = 0; i < over; i++)
                {
                    returnData.ImportRow(mydata.Rows[go + i]);
                }
            }
            else
            {
                if (mydata.Rows.Count == 0)
                {
                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = returnData, records = mytotal, page = page, total = Math.Ceiling((decimal)mytotal / pagesize) }, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" }));
                }
                //几条数据
                for (int i = 0; i < pagesize; i++)
                {
                    returnData.ImportRow(mydata.Rows[go + i]);
                }
            }

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = returnData, records = mytotal, page = page, total = Math.Ceiling((decimal)mytotal / pagesize) }, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" }));

        }
        [HttpPost]
        public JsonResult workMeetEvaluate(string meetid, string context)
        {
            ActivityEvaluateEntity model = new ActivityEvaluateEntity();
            var success = true;
            var message = string.Empty;
            try
            {

                var bll = new ActivityBLL();
                var userName = OperatorProvider.Provider.Current().UserName;
                var userId = OperatorProvider.Provider.Current().UserId;
                var deptid = OperatorProvider.Provider.Current().DeptId;
                var deptname = OperatorProvider.Provider.Current().DeptName;
                model.ActivityEvaluateId = Guid.NewGuid().ToString();
                model.Activityid = meetid;
                model.EvaluateDate = DateTime.Now;
                model.EvaluateId = userId;
                model.EvaluateUser = userName;
                model.EvaluateContent = context;
                model.CREATEDATE = DateTime.Now;
                model.CREATEUSERID = userId;
                model.CREATEUSERNAME = userName;
                model.EvaluateDeptId = deptid;
                model.DeptName = deptname;
                bll.SaveEvaluate("", null, model);
                return Json(new { success, message });
            }
            catch (Exception e)
            {
                success = false;
                message = e.InnerException.Message.ToString();
                return Json(new { success, message });
            }
        }
        /// <summary>
        /// 首页展示明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult HomeDetail(string id, string jobid)
        {
            //var model = new MeetingJobEntity() { };
            var user = OperatorProvider.Provider.Current();
            //var Basemodel = default(WorkmeetingEntity);
            var job = workmeetingbll.GetJobDetail(jobid, id, null);
            //公共方法获取班会数据   班前班后会为同一条数据 展示不同
            //Basemodel = workmeetingbll.HomeMeetingDetail(id);
            //model = Basemodel.Jobs.FirstOrDefault(x => x.JobId == jobid);
            ViewData["JobTime"] = string.Format("{0} - {1}", job.StartTime.ToString("yyyy/M/dd H:mm"), job.EndTime.ToString("yyyy/M/dd H:mm"));
            ViewData["xxxx"] = string.Join(",", job.Relation.JobUsers.Where(x => x.JobType == "isdoperson").Select(x => x.UserName));
            return View(job);
        }

        /// <summary>
        ///班前班后会平台详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail2(string id)
        {
            var trainingtype = Config.GetValue("TrainingType");
            var CustomerModel = Config.GetValue("CustomerModel");

            if (trainingtype == "人身风险预控")
                ViewBag.trainingtype = trainingtype;
            else
            {
                if (string.IsNullOrEmpty(CustomerModel))
                    ViewBag.trainingtype = CustomerModel;
                else
                    ViewBag.trainingtype = "危险预知训练";
            }

            var startmeeting = workmeetingbll.GetDetail(id);

            var endmeeting = workmeetingbll.GetDetail(startmeeting.OtherMeetingId);
            #region 班前一题/班前一课
            MeetSubjectBLL sbll = new MeetSubjectBLL();
            MeetingQuestionBLL qbll = new MeetingQuestionBLL();
            var sSubject = sbll.getDataByMeetID(startmeeting.MeetingId);

            var sQuestion = qbll.GetEntityByMeetingId(startmeeting.MeetingId);

            ViewBag.sSubject = sSubject;

            ViewBag.sQuestion = sQuestion;

            #endregion
            foreach (var item in startmeeting.Files)
            {
                if (!string.IsNullOrEmpty(item.OtherUrl))
                {
                    if (!new Uploader().Query(item.OtherUrl))
                        item.OtherUrl = string.Empty;
                }
            }
            if (endmeeting != null)
            {
                foreach (var item in endmeeting.Files)
                {
                    if (!string.IsNullOrEmpty(item.OtherUrl))
                    {
                        if (!new Uploader().Query(item.OtherUrl))
                            item.OtherUrl = string.Empty;
                    }
                }
            }

            ViewData["users"] = string.Join("、", startmeeting.Signins.Where(x => x.IsSigned).Select(x => string.IsNullOrEmpty(x.Reason) ? x.PersonName : string.Format("{0}（{1}）", x.PersonName, x.Reason)));
            if (startmeeting.Signins.Count(x => !x.IsSigned) > 0)
            {
                //ViewData["queqin"] = string.Join("、", startmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName));
                ViewData["queqin"] = string.Join("；", startmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName + "：" + x.Reason));
            }
            else
            {
                ViewData["queqin"] = "无";
            }
            if (startmeeting.Signins.Count(x => x.MentalCondition != "正常") > 0)
            {
                ViewData["state"] = string.Join("、", startmeeting.Signins.Where(x => x.MentalCondition != "正常").Select(x => x.PersonName + "：" + x.MentalCondition));
            }
            else
            {
                ViewData["state"] = "正常";
            }
            if (endmeeting != null)
            {
                ViewData["users2"] = string.Join("、", endmeeting.Signins.Where(x => x.IsSigned).Select(x => string.IsNullOrEmpty(x.Reason) ? x.PersonName : string.Format("{0}（{1}）", x.PersonName, x.Reason)));
                if (endmeeting.Signins.Count(x => !x.IsSigned) > 0)
                {
                    //ViewData["queqin2"] = string.Join("、", endmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName));
                    ViewData["queqin2"] = string.Join("；", endmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName + "：" + x.Reason));
                }
                else
                {
                    ViewData["queqin2"] = "无";
                }
            }
            var user = OperatorProvider.Provider.Current();

            ViewBag.Name = user.UserName;
            ViewBag.Evaluate = workmeetingbll.GetEntityEvaluate(startmeeting.OtherMeetingId).OrderByDescending(x => x.CREATEDATE).ToList();

            #region 班前班后会 当班记事附件
            IList<FileInfoEntity> startNoteFiles = new List<FileInfoEntity>();
            IList<FileInfoEntity> endNoteFiles = new List<FileInfoEntity>();
            WorkMeetingNoteEntity startNote = null;
            WorkMeetingNoteEntity endNote = null;
            if (startmeeting != null) startNote = new WorkMeetingNoteBLL().GetEntityByMeetingId(startmeeting.MeetingId);
            if (endmeeting != null) endNote = new WorkMeetingNoteBLL().GetEntityByMeetingId(endmeeting.MeetingId);
            if (startNote != null)
            {
                startNoteFiles = new FileInfoBLL().GetFilesByRecIdNew(startNote.Id);
            }
            if (endNote != null)
            {
                endNoteFiles = new FileInfoBLL().GetFilesByRecIdNew(endNote.Id);
            }
            ViewBag.StartNoteFiles = startNoteFiles;
            ViewBag.EndNoteFiles = endNoteFiles;
            #region 早安中铝
            MeetingRecordEntity meetingRecord = null;
            if (startmeeting != null)
            {
                meetingRecord = new MeetingRecordBLL().GetEntityByMeetingId(startmeeting.MeetingId);
            }
            ViewBag.MeetingReocrd = meetingRecord;
            #endregion
            #endregion

            ViewBag.meetingid = startmeeting.MeetingId;
            return View(new List<WorkmeetingEntity>() { startmeeting, endmeeting });
        }

        /// <summary>
        ///班前班后会平台编辑视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail3(string id)
        {
            var startmeeting = workmeetingbll.GetDetail(id);//班前会  
            var endmeeting = workmeetingbll.GetDetail(startmeeting.OtherMeetingId);//班后会

            ViewData["users"] = string.Join("、", startmeeting.Signins.Where(x => x.IsSigned).Select(x => x.PersonName));//参与人员
            if (startmeeting.Signins.Count(x => !x.IsSigned) > 0)
            {
                //ViewData["queqin"] = string.Join("、", startmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName));
                ViewData["queqin"] = string.Join("；", startmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName + "：" + x.Reason));//缺勤人员及原因
            }
            else
            {
                ViewData["queqin"] = "无";
            }
            if (startmeeting.Signins.Count(x => x.MentalCondition != "正常") > 0)
            {//人员状态
                ViewData["state"] = string.Join("、", startmeeting.Signins.Where(x => x.MentalCondition != "正常").Select(x => x.PersonName + "：" + x.MentalCondition));
            }
            else
            {
                ViewData["state"] = "正常";
            }
            if (endmeeting != null)
            {//班后会
                ViewData["users2"] = string.Join("、", endmeeting.Signins.Where(x => x.IsSigned).Select(x => x.PersonName));
                if (endmeeting.Signins.Count(x => !x.IsSigned) > 0)
                {
                    ViewData["queqin2"] = string.Join("；", endmeeting.Signins.Where(x => !x.IsSigned).Select(x => x.PersonName + "：" + x.Reason));//缺勤人员及原因
                }
                else
                {
                    ViewData["queqin2"] = "无";
                }
            }
            var user = OperatorProvider.Provider.Current();
            ViewBag.Name = user.UserName;
            ViewBag.Evaluate = workmeetingbll.GetEntityEvaluate(startmeeting.OtherMeetingId).OrderByDescending(x => x.CREATEDATE).ToList();
            return View(new List<WorkmeetingEntity>() { startmeeting, endmeeting });
        }



        public DataTable MoreOverView()
        {
            var user = OperatorProvider.Provider.Current();
            var data = workmeetingbll.GetOverView(user.DeptId);
            var result = data.Select(x => new WorkmeetingEntity() { MeetingId = x.FullName, MeetingPerson = ((decimal)x.Layer.Value / x.SortCode.Value).ToString("p") }).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Bz");
            dt.Columns.Add("Dec");
            foreach (WorkmeetingEntity w in result)
            {
                DataRow row = dt.NewRow();
                row[0] = w.MeetingId;
                row[1] = w.MeetingPerson;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public ActionResult OverView()
        {
            var user = OperatorProvider.Provider.Current();
            var data = workmeetingbll.GetOverView(user.DeptId);
            var result = data.Select(x => new WorkmeetingEntity() { MeetingId = x.FullName, MeetingPerson = ((decimal)x.Layer.Value / (x.SortCode.Value == 0 ? 1 : x.SortCode.Value)).ToString("p") }).ToList();
            return View(result);
        }

        public JsonResult FindJob(int limit, string deptid, string query)
        {
            var user = OperatorProvider.Provider.Current();
            var data = new List<JobTemplateEntity>();
            var trainingtype = Config.GetValue("TrainingType");
            var total = 0;
            if (trainingtype == "人身风险预控")
            {
                HumanDangerBLL bll1 = new HumanDangerBLL();
                var data1 = bll1.Find(query, deptid, 10, 1, out total);
                data = data1.Select(x => new JobTemplateEntity() { JobId = x.HumanDangerId.ToString(), JobContent = x.Task }).ToList();
            }
            else
            {
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                var data2 = meetBll.Find(query, user.DeptId, 10);
                data.AddRange(data2);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 任务详情
        /// </summary>
        /// <returns></returns>
        public ViewResult JobDetail()
        {
            return View();
        }



        public JsonResult PostMesuare(MeasureSetEntity model)
        {
            var success = true;
            var message = string.Empty;
            var user = OperatorProvider.Provider.Current();

            try
            {
                model.ID = Guid.NewGuid().ToString();
                model.CreateDate = DateTime.Now;
                MeasureSetBLL measuresetbll = new MeasureSetBLL();
                measuresetbll.Insert(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data = model });
        }

        public JsonResult PostDangerous(RiskFactorSetEntity model)
        {
            var success = true;
            var message = string.Empty;
            var user = OperatorProvider.Provider.Current();

            try
            {
                RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
                model.Create();
                model.DeptId = user.DeptId;
                riskFactorSetBLL.SaveForm(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data = model });
        }

        public JsonResult DeleteDangerous(RiskFactorSetEntity model)
        {
            var success = true;
            var message = string.Empty;
            var user = OperatorProvider.Provider.Current();

            try
            {
                RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
                riskFactorSetBLL.RemoveForm(model.ID);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }

        public JsonResult DeleteMeasure(MeasureSetEntity model)
        {
            var success = true;
            var message = string.Empty;
            var user = OperatorProvider.Provider.Current();

            try
            {
                MeasureSetBLL measuresetbll = new MeasureSetBLL();
                measuresetbll.Delete(model.ID);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }

        public JsonResult GetJobFiles(string id)
        {
            var files = new FileInfoBLL().GetFilesByRecIdNew(id).ToList();
            var picpath = Url.Content("~").Substring(0, @Url.Content("~").Length - 1);
            var audios = files.Where(x => x.Description == "音频").Select(x => new FileInfoEntity() { FileId = x.FileId, CreateUserName = x.CreateUserName, FileName = x.FileName, FilePath = picpath + x.FilePath.Substring(1, x.FilePath.Length - 1) });
            var images = files.Where(x => x.Description == "照片").Select(x => new FileInfoEntity() { FileId = x.FileId, CreateUserName = x.CreateUserName, FileName = x.FileName, FilePath = picpath + x.FilePath.Substring(1, x.FilePath.Length - 1) });

            return Json(new { success = true, message = string.Empty, data = new { audios = audios, images = images } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobUsres(string jobid, string meetingjobid)
        {
            var success = true;
            var message = string.Empty;
            var data = default(List<JobUserEntity>);

            try
            {
                data = workmeetingbll.GetJobUsers(jobid, meetingjobid);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostScore(string data)
        {
            var success = true;
            var message = string.Empty;

            var entities = new List<JobUserEntity>();
            var items = data.Split(',');
            foreach (var item in items)
            {
                entities.Add(new JobUserEntity() { JobUserId = item.Split('|')[0], Score = int.Parse(item.Split('|')[1]) });
            }

            try
            {
                workmeetingbll.PostScore(entities);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }

        public JsonResult GetDutyPerson(string id)
        {
            var success = true;
            var message = string.Empty;
            var data = default(List<UnSignRecordEntity>);
            var bll = new WorkmeetingBLL();

            try
            {
                data = bll.GetDutyPerson(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostDutyPerson(List<WorkmeetingEntity> models)
        {
            var success = true;
            var message = string.Empty;
            var data = string.Empty;

            if (models == null) models = new List<WorkmeetingEntity>();
            foreach (var item in models)
            {
                if (item.DutyPerson == null) item.DutyPerson = new List<UnSignRecordEntity>();
                foreach (var item1 in item.DutyPerson)
                {
                    item1.UnSignRecordId = Guid.NewGuid().ToString();
                }
            }

            var bll = new WorkmeetingBLL();

            try
            {
                bll.PostDutyPerson(models);
                data = string.Join("、", models.SelectMany(x => x.DutyPerson).Select(x => x.UserName).Distinct());
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateState(MeetingAndJobEntity model)
        {
            var success = true;
            var message = string.Empty;
            try
            {
                var trainingtype = Config.GetValue("TrainingType");

                var bll = new WorkmeetingBLL();
                bll.UpdateState(model.MeetingJobId, model.IsFinished, trainingtype);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });

        }

        public JsonResult GetPersons()
        {
            var user = OperatorProvider.Provider.Current();
            var data = new PeopleBLL().GetListByDept(user.DeptId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Index4()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;

            return View();
        }

        public JsonResult GetData2(string deptid, int rows, int page)
        {
            var total = 0;
            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);
            var data = workmeetingbll.GetData2(depts.Select(x => x.DepartmentId).ToArray(), rows, page, out total);
            ShellClass sh = new ShellClass();
            foreach (var item in data)
            {
                if (item.Video1 != null)
                {
                    var path = Server.MapPath(item.Video1);
                    item.Video1 = WMP.GetDurationByWMPLib(path);
                }

                if (item.Video2 != null)
                {
                    var path = Server.MapPath(item.Video2);
                    item.Video2 = WMP.GetDurationByWMPLib(path);
                }
            }
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetList(string deptid, DateTime? begin, DateTime? end, string appraise, string orderby, int rows, int page)
        {
            var basepath = AppDomain.CurrentDomain.BaseDirectory;
            var user = OperatorProvider.Provider.Current();

            if (end != null) end = end.Value.AddDays(1).AddSeconds(-1);

            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);
            var total = 0;
            var data = workmeetingbll.GetList(depts.Select(x => x.DepartmentId).ToArray(), user.UserId, begin, end, appraise, orderby == "升", rows, page, out total);
            foreach (var item in data)
            {
                if (item.Files != null && item.Files.Count > 0)
                {
                    var file = item.Files.Find(x => x.Description == "视频");
                    if (file != null)
                    {
                        var path = Path.Combine(basepath, file.FilePath.Trim('~').Replace('/', '\\'));
                        if (System.IO.File.Exists(path))
                            file.duration = Util.WMP.GetDurationByWMPLib(path);
                    }
                }
                if (item.Files2 != null && item.Files2.Count > 0)
                {
                    var file = item.Files2.Find(x => x.Description == "视频");
                    if (file != null)
                    {
                        var path = Path.Combine(basepath, file.FilePath.Trim('~').Replace('/', '\\'));
                        if (System.IO.File.Exists(path))
                            file.duration = Util.WMP.GetDurationByWMPLib(path);
                    }
                }
            }
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page }, JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetIsMeet(string deptid, string type, DateTime? begin, DateTime? end)
        {
            var deptbll = new DepartmentBLL();
            var user = OperatorProvider.Provider.Current();
            var company = deptbll.GetCompany(user.DeptId);
            #region 初始化时间 一周一月
            var time = DateTime.Now;
            var year = time.Year;
            var month = time.Month;
            var day = time.Day;
            var monthSum = Util.Time.GetDaysOfMonth(time);
            var monthStartTime = new DateTime(year, month, 1);
            var monthEndTime = new DateTime(year, month, day);
            var week = Util.Time.GetWeekNumberOfDay(time);
            var sumday = -(week - 1);
            var weekStartTime = monthEndTime.AddDays(sumday);
            var weekEndTime = weekStartTime.AddDays(6);
            #endregion
            if (type == "1")
            {
                begin = weekStartTime;
                end = DateTime.Now;
            }
            else if (type == "2")
            {
                begin = monthStartTime;
                end = DateTime.Now;
            }
            var depts = new DepartmentBLL().GetSubDepartments(company.DepartmentId, null);
            var total = 0;
            var data = workmeetingbll.GetList(depts.Select(x => x.DepartmentId).ToArray(), user.UserId, begin, end, "0", false, 200000, 1, out total);
            var Tables = workmeetingbll.GetIsMeet(depts.Where(x => x.Nature == "班组").ToList(), begin.Value, end.Value, data);
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { rows = Tables, total = Tables.Rows.Count }, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" }));
            //   return Json(new { data = Tables, total=Tables.Rows.Count }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult UpdateJobs(WorkmeetingEntity meeting)
        {
            var success = true;
            var message = string.Empty;
            var user = OperatorProvider.Provider.Current();
            var bll = new WorkmeetingBLL();
            if (meeting.Jobs == null) meeting.Jobs = new List<MeetingJobEntity>();

            meeting.Jobs.ForEach(x => x.GroupId = user.DeptId);
            try
            {
                bll.UpdateJobs(meeting.Jobs);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
                throw;
            }
            var now = DateTime.Now;
            var cnt = meeting.Jobs.RemoveAll(x => !(x.StartTime <= now && x.EndTime >= now));
            if (cnt > 0) message = "任务开始时间并非今日，将推送至对应日期的班会中！";
            return Json(new { success, message, data = meeting });
        }

        /// <summary>
        /// 获取今日工作的 各风险等级的数量
        /// </summary>
        /// <returns></returns>
        public ActionResult RiskLevelDetail()
        {
            return View();
        }

        #region 今日工作
        /// <summary>
        /// 分页查询今日工作
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public ActionResult GetJobPagedList(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            try
            {
                var user = OperatorProvider.Provider.Current();
                if (user.IsSystem)
                {
                    var a = queryJson.ToJObject();
                    a.Add("DeptCode", user.OrganizeCode);
                    queryJson = JsonConvert.SerializeObject(a);
                }
                else
                {
                    var a = queryJson.ToJObject();
                    var dept = new DepartmentBLL().GetEntity(user.DeptId);
                    if (dept.IsSpecial || dept.IsOrg == true)
                    {
                        a.Add("DeptCode", user.OrganizeCode);
                    }
                    else
                    {
                        a.Add("DeptCode", dept.EnCode);
                    }
                    queryJson = JsonConvert.SerializeObject(a);
                }

                var data = new WorkmeetingBLL().GetJobPagedList(pagination, queryJson);
                int total = 1;//总页数
                if (pagination.records % pagination.rows == 0)
                    total = pagination.records / pagination.rows;
                else
                    total = pagination.records / pagination.rows + 1;

                var JsonData = new
                {
                    rows = data,
                    total,
                    pagination.page,
                    pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(JsonData);
            }
            catch (Exception)
            {
                var JsonData = new
                {
                    rows = new List<string>(),
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(JsonData);
            }
        }

        #region  今日工作统计
        [HttpGet]
        public ActionResult TodayWorkStatistics(string startTime)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            var depts = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, "");
            var DeptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            DateTime searchTime;
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                if (DateTime.TryParse(startTime, out searchTime))
                {
                    searchTime = DateTime.Now;
                }
            }
            else
            {
                searchTime = DateTime.Now;
            }

            Dictionary<string, int> dic = new WorkmeetingBLL().TodayWorkStatistics(DeptStr, searchTime.Date);
            return ToJsonResult(dic);
        }
        #endregion
        #endregion

        #region 早安中铝
        public ActionResult MorningStatistics()
        {
            return View();
        }

        /// <summary>
        /// 获取各部门早安中铝完成情况
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult GetMorningData(string startTime, string endTime)
        {
            try
            {
                Operator user = OperatorProvider.Provider.Current();
                string deptCode = string.Empty;
                if (user.isPlanLevel == "1" || user.IsSystem)
                {
                    deptCode = user.OrganizeCode;
                }
                else
                {
                    var userDept = new DepartmentBLL().GetEntity(user.DeptId);
                    if (userDept == null) throw new Exception("找不到用户所在的部门");
                    deptCode = userDept.EnCode;
                    if (userDept.IsSpecial || userDept.IsOrg == true)
                    {
                        deptCode = user.OrganizeCode;
                    }
                }

                var data = new MeetingRecordBLL().GetStatistics(startTime, endTime, deptCode);

                return Success("查询成功", data);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        #endregion
        #region 班前一题
        public ActionResult QuestionStatistics()
        {
            return View();
        }

        /// <summary>
        /// 获取各部门早安中铝完成情况
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult GetQuestionData(string startTime, string endTime)
        {
            try
            {
                Operator user = OperatorProvider.Provider.Current();
                string deptCode = string.Empty;
                if (user.isPlanLevel == "1" || user.IsSystem)
                {
                    deptCode = user.OrganizeCode;
                }
                else
                {
                    var userDept = new DepartmentBLL().GetEntity(user.DeptId);
                    if (userDept == null) throw new Exception("找不到用户所在的部门");
                    deptCode = userDept.EnCode;
                    if (userDept.IsSpecial || userDept.IsOrg == true)
                    {
                        deptCode = user.OrganizeCode;
                    }
                }

                var data = new MeetingQuestionBLL().GetStatistics(startTime, endTime, deptCode);

                return Success("查询成功", data);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        #endregion


        public JsonResult GetMeetingNote(string id)
        {
            WorkMeetingNoteBLL noteBLL = new WorkMeetingNoteBLL();
            var entity = noteBLL.GetEntityByMeetingId(id);

            return Success(new AjaxResult() { type = ResultType.success, resultdata = entity });
        }

        public ViewResult DangerReview()
        {
            return View();
        }

        public JsonResult GetAccidentPreview(string id)
        {
            var data = educationBLL.FilterByMeeting(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccidentAnswer(string id)
        {
            var data = educationAnswerBLL.List(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}