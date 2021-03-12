using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Service.Activity;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{

    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    /// 
    public class DangerController : MvcControllerBase
    {
        private DangerBLL dangerLL = new DangerBLL();
        private WorkmeetingBLL workmeetingbll = new WorkmeetingBLL();
        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string from, string to)
        {
            ViewData["from"] = from;
            ViewData["to"] = to;

            return View();
        }

        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            DataTable dt = dangerLL.GetJobList(BSFramework.Application.Code.OperatorProvider.Provider.Current().DeptId);
            //if (dt.Rows.Count == 0 && Request.QueryString["mode"] != "1")
            //{
            //    return Redirect("Index");
            //}

            var user = OperatorProvider.Provider.Current();
            var bll = new UserBLL();
            var users = bll.GetDeptUsers(user.DeptId);

            return View(users);
        }
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail(string id)
        {
            var entity = dangerLL.GetEntity(id);
            ActivityBLL actBll = new ActivityBLL();
            entity.Evaluateions = actBll.GetEntityList().Where(x => x.Activityid == entity.Id).ToList();
            ViewData["model"] = entity;

            return View();
        }
        public ActionResult Detail3(string name, string from, string to)
        {
            ViewBag.name = name;
            ViewBag.from = from;
            ViewBag.to = to;
            return View();
        }
        public ActionResult GetDangerJsonNew(Pagination pagination, string queryJson, string name, string from, string to)
        {
            var bll = new ActivityBLL();

            Operator user = OperatorProvider.Provider.Current();
            //pagination.p_kid = "ID";
            //pagination.p_fields = "a.jobname,a.jobuser,a.groupname,a.jobtime,a.status,a.scoreremark,a.createdate,a.deptcode,a.score,a.operdate,appraisecontent"; //必须包含where条件所需字段
            //pagination.p_tablename = " wg_danger a ";
            //pagination.conditionJson = " 1=1 and status = '2' and groupname = '" + name + "'";
            //if (!string.IsNullOrEmpty(from)) pagination.conditionJson += " and jobtime >= '" + from + "'";
            //if (!string.IsNullOrEmpty(to)) pagination.conditionJson += " and jobtime < '" + to + "'";

            var watch = CommonHelper.TimerStart();
            // DataTable dt = dangerLL.GetDangerPageList(user.UserId, pagination, queryJson);
            var dt = dangerLL.GetDangerJsonNew(pagination, queryJson, name, from, to, user.UserId);
            dt.Columns.Add("number");
            dt.Columns.Add("time");
            int total = 0;
            foreach (DataRow row in dt.Rows)
            {
                int n = bll.GetActivityEvaluateEntity(row["ID"].ToString(), 10000, 1, out total).Count();
                if (n > 0)
                {
                    if (bll.IsEvaluate(row["ID"].ToString(), user.UserId))
                    {//本人已评价
                        row["number"] = "本人已评价（" + n + "人）";
                    }
                    else
                    {//本人未评价
                        row["number"] = "本人未评价（" + n + "人）";
                    }
                }
                else
                {
                    row["number"] = "本人未评价";
                }
                string start = Convert.ToDateTime(row["jobtime"]).ToString("yyyy-MM-dd HH:mm");
                row["time"] = start;
                if (row["operdate"].ToString() != "")
                {
                    string end = Convert.ToDateTime(row["operdate"]).ToString("yyyy-MM-dd HH:mm");
                    row["time"] = start + " - " + end.Substring(10, 6);
                }
            }
            var JsonData = new
            {
                rows = dt,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ActionResult GetListJson(Pagination pagination, string queryJson)
        {
            var user = BSFramework.Application.Code.OperatorProvider.Provider.Current();
            //pagination.p_kid = "Id";
            //pagination.p_fields = "deptcode,jobname,jobtime,status,operdate,groupname,(select max(filepath) from base_fileinfo where recid=a.id and (filetype='jpg' or filetype='gif' or filetype='bmp')) 'filepath'";
            //pagination.p_tablename = "wg_danger a";
            //pagination.conditionJson = "deptcode like '" + user.DeptCode + "%' and status = '2'";
            var watch = CommonHelper.TimerStart();


            var data = dangerLL.GetPagesList(pagination, queryJson);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }


        public JsonResult GetRecords(int pagesize, int pageindex, string name, string from, string to)
        {
            var dfrom = default(DateTime?);
            var dto = default(DateTime?);

            if (!string.IsNullOrEmpty(from)) dfrom = DateTime.Parse(from);
            if (!string.IsNullOrEmpty(to)) dto = DateTime.Parse(to);
            var user = OperatorProvider.Provider.Current();

            var total = 0;
            var data = dangerLL.GetRecords(user.DeptId, name, dfrom, dto, pagesize, pageindex, out total);

            var list = data.Select(x => new { x.Id, x.JobName, JobTime = x.JobTime == null ? string.Empty : x.JobTime.Value.ToString("yyyy-MM-dd HH:mm"), x.Files });
            foreach (var item in list)
            {
                foreach (var item1 in item.Files)
                {
                    item1.FilePath = Url.Content(item1.FilePath);
                }
            }
            return Json(new { data = list, total = total }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetJobListJson()
        {
            DataTable dt = dangerLL.GetJobList(BSFramework.Application.Code.OperatorProvider.Provider.Current().DeptId);
            return Content(dt.ToJson());
        }
        public ActionResult Index2(string type)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = dept.FullName;
            ViewBag.type = "";

            //ViewBag.from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            //ViewBag.to = DateTime.Now.Date.ToString("yyyy-MM-dd");
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString("yyyy-MM-dd");  //当前季度开始日期
            string edt = DateTime.Now.ToString("yyyy-MM-dd");
            string mstart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(type))
            {
                ViewBag.type = type;
                ViewBag.from = sdt;

                ViewBag.to = edt;
            }
            else
            {
                ViewBag.from = mstart;

                ViewBag.to = edt;
            }
            ViewBag.cpname = Config.GetValue("CustomerModel");
            return View();
        }

        public ActionResult Index4(string type)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;
            ViewBag.deptName = user.DeptName;
            ViewBag.type = "";

            ViewBag.from = "";
            ViewBag.to = "";
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString("yyyy-MM-dd");  //当前季度开始日期
            string edt = DateTime.Now.ToString("yyyy-MM-dd");
            string mstart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(type))
            {
                ViewBag.type = type;
                ViewBag.from = sdt;

                ViewBag.to = edt;
            }
            return View();
        }
        public ActionResult Detail2(string id, string type)
        {
            DangerEntity d = dangerLL.GetEntity(id);
            FileInfoBLL fiBll = new FileInfoBLL();
            MeasuresBLL measureBll = new MeasuresBLL();
            IList<FileInfoEntity> list = fiBll.GetFilesByRecIdNew(id).ToList();
            ViewData["files"] = list;
            var mlist = measureBll.GetMeasureList(id);
            ViewData["measures"] = mlist;
            ViewBag.type = type;

            ViewBag.cpname = Config.GetValue("CustomerModel");
            var dservice = new DangerService();
            d.JobUsers = dservice.GetUsersByDanger(d.JobId);
            PeopleBLL peobll = new PeopleBLL();
            foreach (var jobuser in d.JobUsers)
            {
                var people = peobll.GetEntity(jobuser.UserId);
                if (people == null) continue;
                if (!string.IsNullOrEmpty(people.RoleDutyId))
                {
                    var dutydanger = peobll.GetDutyDangerEntityByRole(people.RoleDutyId);
                    if (dutydanger != null)
                    {
                        if (!string.IsNullOrEmpty(dutydanger.DutyContent))
                        {

                            jobuser.DangerDutyContent = dutydanger.DutyContent;
                            jobuser.Danger = dutydanger.Danger;
                            jobuser.Measure = dutydanger.Measure;
                        }
                    }
                }
            }
            ViewData["jobusers"] = d.JobUsers;
            ViewData["checker"] = "";
            var entity = new WorkmeetingService().GetDangerJobUsers(d.JobId).Where(x => x.JobType == "ischecker").FirstOrDefault();
            if (entity != null) ViewData["checker"] = entity.UserName;
            if (d.Measure == null) d.Measure = "";
            if (d.StopMeasure == null) d.StopMeasure = "";
            var arrms = d.Measure.Split(new char[3] { '<', '&', '>' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
            var arrstops = d.StopMeasure.Split(new char[3] { '<', '&', '>' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
            ViewData["mea1"] = arrms;
            ViewData["mea2"] = arrstops;
            ViewBag.action = HttpContext.Request.QueryString["action"] ?? "show";
            return View(d);
        }

        public ActionResult Detail4(string id, string type)
        {
            DangerEntity d = dangerLL.GetEntity(id);
            FileInfoBLL fiBll = new FileInfoBLL();
            MeasuresBLL measureBll = new MeasuresBLL();
            IList<FileInfoEntity> list = fiBll.GetFilesByRecIdNew(id).ToList();
            ViewData["files"] = list;
            var mlist = measureBll.GetMeasureList(id);
            ViewData["measures"] = mlist;
            var dservice = new DangerService();
            d.JobUsers = dservice.GetUsersByDanger(d.JobId);
            PeopleBLL peobll = new PeopleBLL();
            foreach (var jobuser in d.JobUsers)
            {
                var people = peobll.GetEntity(jobuser.UserId);
                if (!string.IsNullOrEmpty(people.RoleDutyId))
                {
                    var dutydanger = peobll.GetDutyDangerEntityByRole(people.RoleDutyId);
                    if (dutydanger != null)
                    {
                        if (!string.IsNullOrEmpty(dutydanger.DutyContent))
                        {

                            jobuser.DangerDutyContent = dutydanger.DutyContent;
                        }
                    }
                }
            }
            ViewData["jobusers"] = d.JobUsers;
            ViewBag.type = type;
            ViewBag.action = HttpContext.Request.QueryString["action"];
            //获取评论信息

            return View(d);
        }
        /// <summary>
        /// 删除安全双述
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult DeleteB(string keyValue)
        {
            try
            {
                dangerLL.RemoveForm(keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult SaveFormNew()
        {
            var success = true;
            return Success("操作成功", success);
        }
        public ActionResult GetMeasures(Pagination pagination, string dangerid)
        {
            MeasuresBLL mbll = new MeasuresBLL();
            //pagination.p_kid = "ID";
            //pagination.p_fields = "dutyman,measure,dangersource,isover,dangerid,createdate";
            //pagination.p_tablename = "wg_measures t";
            //pagination.conditionJson = "dangerid ='" + dangerid + "'";
            //pagination.sidx = "createdate";
            var watch = CommonHelper.TimerStart();
            var data = mbll.GetMeasuresByIds(pagination, dangerid);
            //var data = mbll.GetMeasuresById(pagination);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        [HttpGet]
        public ActionResult GetAppraise(string keyValue)
        {
            MeasuresBLL measureBll = new MeasuresBLL();
            var data = measureBll.GetMeasureList(keyValue);
            return Content(data.ToJson());
        }
        public ActionResult GetDangerJson(Pagination pagination, string queryJson, string type, string deptid, string select)
        {
            var bll = new ActivityBLL();

            Operator user = OperatorProvider.Provider.Current();

            if (string.IsNullOrEmpty(deptid))
            {
                var root = departmentBLL.GetAuthorizationDepartment(user.DeptId);
                deptid = root.DepartmentId;
            }
            var depts = departmentBLL.GetSubDepartments(deptid, null);

            //pagination.p_kid = "ID";
            //pagination.p_fields = "a.jobname,a.jobuser,a.groupname,a.jobtime,a.status,a.scoreremark,a.createdate,a.deptcode,a.score,a.operdate,appraisecontent,a.ticketid,a.jobid"; //必须包含where条件所需字段
            //pagination.p_tablename = " wg_danger a ";
            //pagination.conditionJson = " 1=1 ";
            //pagination.conditionJson += string.Format(" and deptcode like '{0}%'", code);

            //if (type == "undo")  //主页点击，只查询未完成的
            //{
            //    pagination.conditionJson += " and status != '2'";
            //    if (string.IsNullOrEmpty(queryJson)) //queryJson包含时间查询，所以只再首次加载时，筛选本季度
            //    {
            //        int month = 1;
            //        if (DateTime.Now.Month < 4) month = 1;
            //        else if (DateTime.Now.Month < 7) month = 4;
            //        else if (DateTime.Now.Month < 10) month = 7;
            //        else if (DateTime.Now.Month <= 12) month = 10;
            //        string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString();  //当前季度开始日期
            //        string edt = DateTime.Now.ToString();
            //        pagination.conditionJson += string.Format(" and jobtime >= '{0}' and jobtime < '{1} 23:59:59'", sdt, edt);
            //    }
            //}
            //else
            //{
            //    pagination.conditionJson += " and status = '2'";
            //    if (type == "4") //首页工作台账，本月数据 
            //    {
            //        if (string.IsNullOrEmpty(queryJson)) //queryJson包含时间查询，所以只再首次加载时，筛选本季度
            //        {
            //            int month = 1;
            //            if (DateTime.Now.Month < 4) month = 1;
            //            else if (DateTime.Now.Month < 7) month = 4;
            //            else if (DateTime.Now.Month < 10) month = 7;
            //            else if (DateTime.Now.Month <= 12) month = 10;
            //            string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString();  //当前季度开始日期
            //            string edt = DateTime.Now.ToString();
            //            pagination.conditionJson += string.Format(" and jobtime >= '{0}' and jobtime < '{1}'", sdt, edt);
            //        }
            //    }
            //    else if (string.IsNullOrEmpty(queryJson))
            //    {
            //        string sdt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString();
            //        string edt = DateTime.Now.ToString();
            //        pagination.conditionJson += string.Format(" and jobtime >= '{0}' and jobtime < '{1}'", sdt, edt);
            //    }

            //}

            var watch = CommonHelper.TimerStart();
            // DataTable dt = dangerLL.GetDangerPageList(user.UserId, pagination, queryJson);
            var dt = dangerLL.GetDangerJson(pagination, queryJson, type, depts.Select(x => x.DepartmentId).ToArray(), user.UserId);
            dt.Columns.Add("number");
            dt.Columns.Add("time");
            dt.Columns.Add("checker");
            dt.Columns.Add("doperson");
            int total = 0;
            foreach (DataRow row in dt.Rows)
            {
                int n = bll.GetActivityEvaluateEntity(row["ID"].ToString(), 10000, 1, out total).Count();
                if (n > 0)
                {
                    if (bll.IsEvaluate(row["ID"].ToString(), user.UserId))
                    {//本人已评价
                        row["number"] = "本人已评价（" + n + "人）";
                    }
                    else
                    {//本人未评价
                        row["number"] = "本人未评价（" + n + "人）";
                    }
                }
                else
                {
                    row["number"] = "本人未评价";
                }
                string start = Convert.ToDateTime(row["jobtime"]).ToString("yyyy-MM-dd HH:mm");
                row["time"] = start;
                if (row["operdate"].ToString() != "")
                {
                    string end = Convert.ToDateTime(row["operdate"]).ToString("yyyy-MM-dd HH:mm");
                    row["time"] = start + " - " + end.Substring(10, 6);
                }
                var jobuser = new WorkmeetingService().GetDangerJobUsers(row["jobid"].ToString()).Where(x => x.JobType == "ischecker").FirstOrDefault();
                if (jobuser != null) row["checker"] = jobuser.UserName;

            }

            var JsonData = new
            {
                rows = dt,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpPost]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = dangerLL.GetEntity(keyValue);
            FileInfoBLL fiBll = new FileInfoBLL();
            var list = fiBll.GetFilesByRecIdNew(keyValue);
            MeasuresBLL measureBll = new MeasuresBLL();

            var d = data;
            if (d.Measure == null) d.Measure = "";
            if (d.StopMeasure == null) d.StopMeasure = "";
            var arrms = d.Measure.Split(new char[3] { '<', '&', '>' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
            var arrstops = d.StopMeasure.Split(new char[3] { '<', '&', '>' }).Where(s => !string.IsNullOrEmpty(s)).ToList();

            return ToJsonResult(new { formData = data, files = list, measures = measureBll.GetMeasureList(keyValue), users = dangerLL.GetJobUserList(data.JobId), mea1 = arrms, mea2 = arrstops });
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 获取实体 
        /// 保存记录/结束训练  提交时对比measure与dangertemplate
        /// entity.jobid - 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">对象实体</param>
        /// <param name="measures">防控措施</param>
        /// <returns>返回对象Json</returns>
        [HttpPost]
        public ActionResult SaveForm(string keyValue, string jobtemplateid, string status, DangerEntity entity, string measures)
        {
            MeasuresBLL mbll = new MeasuresBLL();
            entity.OperDate = DateTime.Now;
            entity.OperUserId = OperatorProvider.Provider.Current().UserId;
            List<MeasuresEntity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MeasuresEntity>>(measures);

            if (status == "2")
            {
                MeetingJobEntity m = new MeetingJobEntity();
                JobTemplateEntity j = new JobTemplateEntity();

                m = workmeetingbll.GetJobDetail(entity.JobId);
                j = workmeetingbll.GetJobTemplate(m.TemplateId);

                if (j != null)
                {
                    if (j.JobType == "danger")
                    {
                        //使用次数
                        j.Usetime += 1;

                        var dangertemplates = workmeetingbll.getdangertemplate(j.JobId).OrderBy(x => x.Dangerous).ToList();
                        var meaureslist = mbll.GetMeasureList(keyValue).OrderBy(x => x.DangerSource).ToList();
                        if (meaureslist.Count() != dangertemplates.Count())
                        {
                            j.EditTime += 1;
                        }
                        else
                        {

                            for (int i = 0; i < dangertemplates.Count(); i++)
                            {
                                if (dangertemplates[i].Dangerous != meaureslist[i].DangerSource || dangertemplates[i].Measure != meaureslist[i].Measure)
                                {
                                    j.EditTime += 1;
                                    break;
                                }
                            }
                        }
                        j.Percent = Math.Round(Convert.ToDecimal(j.EditTime) / Convert.ToDecimal(j.Usetime), 2);
                    }
                }
                var messagebll = new MessageBLL();
                messagebll.FinishTodo(Config.GetValue("CustomerModel"), keyValue);
            }
            else
            {
                var danger = dangerLL.GetEntity(keyValue);
                if (danger.Status != int.Parse(status ?? "0")) return Success("操作成功。");
            }
            dangerLL.Update(keyValue, entity, list);
            return Success("操作成功。");
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult Builder(string keyValue)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeVersion = 10;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeForegroundColor = Color.Black;
            Bitmap bmp = qrCodeEncoder.Encode(keyValue, Encoding.UTF8);//指定utf-8编码， 支持中文
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            return File(ms.ToArray(), @"image/jpg");
        }
        #endregion

        public JsonResult GetJobUsers()
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new UserBLL();
            var users = bll.GetDeptUsers(user.DeptId);
            var data = users.Select(x => new { x.UserId, UserName = x.RealName }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddDanger()
        {
            var json = this.Request.Form.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<DangerEntity>(json);
            var result = true;
            var message = string.Empty;
            var data = default(DangerEntity);

            model.CreateDate = DateTime.Now;
            model.CreateUserId = OperatorProvider.Provider.Current().UserId;
            model.OperUserId = OperatorProvider.Provider.Current().UserId;
            model.OperDate = DateTime.Now;
            model.Status = 0;
            model.Sno = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (model.JobUsers != null)
            {
                for (int i = 0; i < model.JobUsers.Count; i++)
                {
                    if (i == 0)
                        model.JobUsers[i].JobType = "ischecker";
                    else
                        model.JobUsers[i].JobType = "isdoperson";
                }
            }

            try
            {
                var bll = new DangerBLL();
                data = bll.AddTraining(model);
                var messagebll = new MessageBLL();
                if (data != null)
                    messagebll.SendMessage(Config.GetValue("CustomerModel"), data.Id);
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, data = data, message });
        }

        public JsonResult AddItem()
        {
            var json = this.Request.Form.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<DangerEntity>(json);
            var result = true;
            var message = string.Empty;
            var data = default(MeasuresEntity);

            MeasuresEntity entity = Newtonsoft.Json.JsonConvert.DeserializeObject<MeasuresEntity>(json);
            entity.CreateDate = DateTime.Now;
            entity.CreateUserId = OperatorProvider.Provider.Current().UserId;

            try
            {
                var bll = new DangerBLL();
                data = bll.AddItem(entity);
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, data = data, message });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult EditItem(MeasuresEntity model)
        {

            var json = this.Request.Form.Get("json");
            var result = true;
            var message = string.Empty;
            var data = default(MeasuresEntity);

            model.CreateDate = DateTime.Now;


            try
            {
                var bll = new DangerBLL();
                data = bll.EditItem(model);
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, data = data, message });
        }

        public JsonResult DeleteItem(string id)
        {
            var result = true;
            var message = string.Empty;
            var data = default(MeasuresEntity);
            try
            {
                var bll = new DangerBLL();
                bll.DeleteItem(id);
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, message, data = id });
        }

        public JsonResult FindTrainings(string query, int limit)
        {
            var bll = new DangerBLL();
            Operator user = OperatorProvider.Provider.Current();
            var data = default(List<DangerEntity>);
            data = bll.FindTrainings(query, limit);
            data = data.Where(x => user.DeptCode.StartsWith(x.DeptCode) || x.DeptCode.StartsWith(user.DeptCode)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateTraingItems(string dangerid, string jobid)
        {
            var result = true;
            var message = string.Empty;
            var data = default(DangerEntity);
            try
            {
                var bll = new DangerBLL();
                var id = bll.UpdateTraingItems(dangerid, jobid);
                data = new DangerEntity() { Id = id };
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, message, data });
        }

        public JsonResult FindDangerous(string query, int limit)
        {
            var bll = new DangerBLL();
            Operator user = OperatorProvider.Provider.Current();
            var total = 0;
            var data = bll.GetDangerous(query, limit, 1, out total);
            data = data.Where(x => string.IsNullOrEmpty(x.DeptCode) || user.DeptCode.StartsWith(x.DeptCode) || x.DeptCode.StartsWith(user.DeptCode)).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateTrainingPerson(string itemid, string userid, string users)
        {
            var result = true;
            var message = string.Empty;
            var data = default(MeasuresEntity);
            try
            {
                var bll = new DangerBLL();
                data = bll.UpdateTrainingPerson(itemid, userid, users);
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, message, data });
        }

        public JsonResult UpdateTrainingState(string itemid, bool isover)
        {
            var result = true;
            var message = string.Empty;
            var data = default(MeasuresEntity);
            try
            {
                var bll = new DangerBLL();
                data = bll.UpdateTrainingState(itemid, isover);
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }

            return this.Json(new { success = true, message, data });
        }

        [HttpPost]
        public JsonResult GetNewData(string from, string to)
        {
            string deptid = OperatorProvider.Provider.Current().DeptId;
            DateTime f = DateTime.Now.AddYears(-100);
            DateTime t = DateTime.Now.AddDays(1);
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            return Json(new { rows = dangerLL.GetCount(deptid, f, t) });
        }

        public ActionResult Index3()
        {
            return View();
        }

        public ActionResult Evaluate(string id)
        {
            ViewBag.userName = OperatorProvider.Provider.Current().UserName;
            ViewBag.userId = OperatorProvider.Provider.Current().UserId;
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public ActionResult SaveEvaluate(string id, string category, ActivityEvaluateEntity model)
        {
            var success = false;
            var bll = new ActivityBLL();
            var userName = OperatorProvider.Provider.Current().UserName;
            var userId = OperatorProvider.Provider.Current().UserId;

            model.ActivityEvaluateId = Guid.NewGuid().ToString();
            model.EvaluateDate = DateTime.Now;
            model.EvaluateId = userId;
            model.EvaluateUser = userName;
            if (model.EvaluateContent == null)
            {
                model.EvaluateContent = "";
            }
            model.CREATEDATE = DateTime.Now;
            model.CREATEUSERID = userId;
            model.CREATEUSERNAME = userName;
            bll.SaveEvaluate(id, category, model);

            var danger = dangerLL.GetEntity(model.Activityid);
            danger.AppraiseContent = "1";
            dangerLL.Update(danger.Id, danger, new List<MeasuresEntity>());

            var messagebll = new MessageBLL();
            messagebll.SendMessage(Config.GetValue("CustomerModel") + "评价", danger.Id);
            return Success("评价成功", success);
        }

        public JsonResult GetDataEvaluate(string keyValue, int rows)
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;

            var user = OperatorProvider.Provider.Current();
            var bll = new ActivityBLL();
            //var data = bll.GetEvaluationsManoeuvre(name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
            var data = bll.GetActivityEvaluateEntity(keyValue, rows, page, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(string id)
        {
            var success = true;
            var message = string.Empty;
            try
            {
                dangerLL.Delete(id);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ViewResult Setting()
        {
            var setting = new DataItemDetailBLL().GetDetail("系统管理", "危险预知训练关联工作任务状态");
            ViewBag.setting = setting.ItemValue;
            return View();
        }

        public JsonResult SaveSetting(DataItemDetailEntity model)
        {
            var success = true;
            var message = "保存成功";
            try
            {
                var setting = new DataItemDetailBLL().GetDetail("系统管理", "危险预知训练关联工作任务状态");
                var bll = new DataItemDetailBLL();
                setting.ItemValue = model.ItemValue;
                bll.SaveForm(setting.ItemDetailId, setting);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }


        public ActionResult TimeCount()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetTimeCount(string from, string to)
        {
            string code = OperatorProvider.Provider.Current().DeptCode;
            string deptid = OperatorProvider.Provider.Current().DeptId;
            string role = OperatorProvider.Provider.Current().RoleName;
            var dept = departmentBLL.GetEntity(deptid);
            if (dept != null)
            {
                if (dept.IsSpecial || role.Contains("厂级用户") || role.Contains("厂级部门用户") || role.Contains("公司领导"))
                {
                    dept = departmentBLL.GetRootDepartment();
                }
                code = dept.EnCode;
            }
            else
            {
                dept = departmentBLL.GetRootDepartment();
                code = dept.EnCode;
            }
            DateTime f = DateTime.Now.AddYears(-100);
            DateTime t = DateTime.Now.AddYears(1);
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            var dt = dangerLL.GetTimeCount(code, f, t);
            var dtStr = dt.ToJson();
            var dtList = dtStr.ToList<DangerGetGroupCountList>();
            var deptList = departmentBLL.GetSubDepartments(dept.DepartmentId, "");
            string r = "";
            foreach (var item in deptList)
            {
                var deptDt = dtList.Where(x => x.deptcode.StartsWith(item.EnCode));

                var personcount = 0;
                foreach (var persons in deptDt)
                {
                    var jobusercount = string.IsNullOrEmpty(persons.jobuser) ? 0 : persons.jobuser.Split(',').Length;
                    var personscount = string.IsNullOrEmpty(persons.persons) ? 0 : persons.persons.Split(',').Length;
                    personcount += jobusercount + personscount;
                    if (jobusercount != 0 && personscount != 0)
                    {
                        foreach (var one in persons.jobuser.Split(','))
                        {
                            if (persons.persons.Contains(one))
                            {
                                personcount--;
                            }
                        }
                    }
                }
                double timecount = 0;
                var deptcount = deptDt.Count();
                foreach (var timeList in deptDt)
                {
                    if (timeList.endtime.HasValue)
                    {
                        TimeSpan tsDiffer = timeList.endtime.Value - timeList.starttime;
                        var tsDecimal = Math.Round(tsDiffer.TotalHours, 2);
                        timecount += tsDecimal;

                    }

                }
                r += "{" + string.Format("deptid:'{0}',deptname:'{1}',activitycount:'{2}',personcount:'{3}',timecount:'{4}'", item.DepartmentId, item.FullName, deptcount, personcount, timecount) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            // return Success("0", new { rows =dt.Rows  });
            return Json(new { rows = r });
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
    }
}
