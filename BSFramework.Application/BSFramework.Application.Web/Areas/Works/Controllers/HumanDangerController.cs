using Aspose.Cells;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.HuamDanger;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.Web.Areas.BaseManage.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using ICSharpCode.SharpZipLib.Zip;
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
    /// 人身风险预控库
    /// </summary>
    public class HumanDangerController : MvcControllerBase
    {
        public ViewResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var config = Config.GetValue("TrainingDept");
            // var isallowd = user.DeptName == config || user.IsSystem;
            var isallowd = user.DeptName == config || user.RoleName.Contains("公司管理员") || user.RoleName.Contains("厂级管理员") || user.IsSystem;//人身风险预控库和设置，应只有公司管理员和安监部(HSE部)可操作，其他人只能查看 hm 2019-06-28
            ViewBag.isallowd = isallowd;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }

        public ViewResult Index2()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;

            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;

            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);
            ViewBag.from = from.ToString("yyyy/MM/dd");
            ViewBag.to = to.ToString("yyyy/MM/dd");

            return View();
        }

        public ViewResult Index3(string cometype)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;

            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;

            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);
            if (!string.IsNullOrEmpty(cometype))
            {
                from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                to = from.AddDays(1).AddMinutes(-1);
            }
            ViewBag.from = from.ToString("yyyy/MM/dd");
            ViewBag.to = to.ToString("yyyy/MM/dd");

            return View();
        }

        public ViewResult Index4()
        {
            return View();
        }

        public ViewResult Index5()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            switch (dept.Nature)
            {
                case "班组":
                    ViewBag.approvestatus = 1;
                    break;
                case "部门":
                    ViewBag.approvestatus = 2;
                    break;
                case "厂级":
                    ViewBag.approvestatus = 3;
                    break;
                default:
                    break;
            }
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }

        public ViewResult Evaluate(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            var model = new ActivityEvaluateEntity() { ActivityEvaluateId = Guid.NewGuid().ToString(), Activityid = id, Score = 0, EvaluateId = user.UserId, EvaluateUser = user.UserName, EvaluateDate = DateTime.Now, CREATEUSERID = user.UserId, CREATEUSERNAME = user.UserName, CREATEDATE = DateTime.Now, Nature = dept == null ? null : dept.Nature, DeptName = dept == null ? null : dept.FullName, EvaluateDeptId = dept.DepartmentId };
            return View(model);
        }

        public ContentResult IsEvaluate(string id)
        {
            var user = OperatorProvider.Provider.Current();

            ActivityBLL actBll = new ActivityBLL();
            var data = actBll.IsEvaluate(id, user.UserId);
            if (data) return Content(null);
            return Content(id);
        }

        [HttpPost]
        public JsonResult SaveEvaluate(string id, string category, ActivityEvaluateEntity model)
        {
            var bll = new HumanDangerBLL();
            var user = OperatorProvider.Provider.Current();
            var messagebll = new MessageBLL();

            var success = true;
            var message = "保存成功";
            try
            {
                bll.Evaluate(model);
                messagebll.SendMessage("人身风险预控评价", model.ActivityEvaluateId);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ContentResult EnsureEvaluate(string id)
        {
            var bll = new HumanDangerTrainingBLL();
            bll.Evaluate(id);
            return Content(null);

        }

        public JsonResult GetData(string deptid, string categoryid, string key, int rows, int page)
        {
            var bll = new HumanDangerBLL();
            var user = OperatorProvider.Provider.Current();
            var total = 0;
            var data = bll.GetData(key, rows, page, deptid, out total);

            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page });
        }

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public JsonResult GetTrainings(FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var deptid = fc.Get("deptid");
            var key = fc.Get("key");
            var from = fc.Get("from");
            var to = fc.Get("to");
            var status = fc.Get("status");
            var page = int.Parse(fc.Get("page") ?? "1");
            var rows = int.Parse(fc.Get("rows") ?? "20");
            var fzuser = fc.Get("fzuser");
            string evaluatelevel = fc.Get("evaluatelevel");
            var bll = new HumanDangerTrainingBLL();
            var evaluatestatus = fc.Get("evaluatestatus");
            var d1 = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var d2 = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);
            if (d2 != null) d2 = d2.Value.AddDays(1).AddMinutes(-1);
            ViewBag.from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy/MM/dd");
            ViewBag.to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddMinutes(-1).ToString("yyyy/MM/dd");

            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);
            var total = 0;
            var data = bll.GetTrainings(user.UserId, depts.Select(x => x.DepartmentId).ToArray(), key, d1, d2, evaluatestatus, rows, page, fzuser, evaluatelevel, status, out total);
            //评价
            //var activitybll = new ActivityBLL();
            //List<ActivityEvaluateEntity> evaluateList = activitybll.GetActivityEvaluateEntity(data.SelectMany(x => x.TrainingUsers).Select(y => y.TrainingUserId.ToString()).ToList());
            //data.ForEach(x =>
            //{
            //    List<string> trainingUserIds = x.TrainingUsers.Select(p => p.TrainingUserId.ToString()).ToList();
            //    x.Evaluates = evaluateList.Where(e => trainingUserIds.Contains(e.Activityid)).ToList();
            //});
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public JsonResult GetUndo(FormCollection fc)
        {
            var deptid = fc.Get("deptid");
            var key = fc.Get("key");
            var from = fc.Get("from");
            var to = fc.Get("to");
            var status = fc.Get("status");
            var page = int.Parse(fc.Get("page") ?? "1");
            var rows = int.Parse(fc.Get("rows") ?? "20");
            var bll = new HumanDangerTrainingBLL();
            int evaluate = 0;
            if (status == "已评价") evaluate = 3;
            else if (status == "未评价") evaluate = 1;
            else if (status == "部分评价") evaluate = 2;
            var d1 = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var d2 = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);
            if (d2 != null) d2 = d2.Value.AddDays(1).AddMinutes(-1);

            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);
            var total = 0;
            var data = bll.GetUndo(depts.Select(x => x.DepartmentId).ToArray(), key, d1, d2, evaluate, rows, page, out total);

            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page });
        }

        /// <summary>
        /// 风险预控任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Edit(string id)
        {
            HumanDangerEntity model = null;
            var user = OperatorProvider.Provider.Current();
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new HumanDangerBLL();
                model = bll.GetDetail(id);
            }
            else
            {

                model = new HumanDangerEntity()
                {
                    OperateTime = DateTime.Now,
                    OperateUserId = user.UserId,
                    OperateUser = user.UserName
                };
            }

            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
            var list1 = dataitembll.GetDataItems("人身伤害风险等级");
            var list2 = dataitembll.GetDataItems("作业性质类型");

            var data1 = list1.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });
            var data2 = list2.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });

            ViewData["DangerLevel"] = data1;
            ViewData["TaskType"] = data2;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                var bll = new HumanDangerTrainingBLL();
                bll.Delete(keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        /// <summary>
        /// 删除分析人
        /// </summary>
        /// <param name="keyValue">TrainingUserId</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult DeleteTraining(string keyValue)
        {
            try
            {
                var bll = new HumanDangerTrainingBLL();
                bll.DeleteTraining(keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 台账详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Edit2(string id)
        {
            var bll = new HumanDangerTrainingBLL();
            var model = bll.GetContent(id);
            foreach (var item in model)
            {
                item.Measures = item.Measures.OrderBy(x => x.Category).OrderBy(x => x.State).OrderBy(x => x.DangerReason).ToList();
                item.TaskTypes = item.TaskTypes.OrderBy(x => x.State).ToList();
            }
            ViewBag.action = HttpContext.Request.QueryString["action"];

            return View(model);

        }

        /// <summary>
        /// 新增和修改风险预控措施
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(string id, HumanDangerEntity model)
        {
            var bll = new HumanDangerBLL();
            var user = OperatorProvider.Provider.Current();

            var success = true;
            var message = "保存成功";
            try
            {
                model.OperateUserId = user.UserId;
                model.OperateUser = user.UserName;
                model.OperateTime = DateTime.Now;
                if (string.IsNullOrEmpty(model.HumanDangerId))
                {
                    model.HumanDangerId = Guid.NewGuid().ToString();
                    //判断数据库是否有重复的数据，即如果工作任务名称相同，而且适用部门包含同一部门（班组），则不能添加成功
                    bool checkunique = bll.CheckUnique(model.HumanDangerId, model.Task, model.DeptId);
                    if (checkunique)
                    {
                        throw new Exception("存在工作任务与适用部门重复的数据");
                    }
                }

                bll.Save(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        [HttpPost]
        public JsonResult Approve(string id, HumanDangerEntity model)
        {
            var bll = new HumanDangerBLL();
            var user = OperatorProvider.Provider.Current();

            var success = true;
            var message = "审核通过";

            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            if (dept != null)
            {
                switch (dept.Nature)
                {
                    case "班组":
                        model.State = 2;
                        break;
                    case "部门":
                        model.State = 3;
                        break;
                    case "厂级":
                        model.State = 0;
                        break;
                    default:
                        break;
                }

                if (dept.IsSpecial) model.State = 0;
            }
            else
            {
                model.State = 0;
            }

            var approve = new ApproveRecordEntity()
            {
                ApproveRecordId = Guid.NewGuid(),
                ApproveUserId = user.UserId,
                ApproveUser = user.UserName,
                ApproveDeptId = user.DeptId,
                ApproveDeptName = user.DeptName,
                ApproveTime = model.ApproveTime.Value,
                RecordId = model.HumanDangerId.ToString()
            };

            try
            {
                bll.Approve(model, approve);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Remove(string id)
        {
            var bll = new HumanDangerBLL();

            var success = true;
            var message = "删除成功";

            try
            {
                bll.Delete(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ViewResult Import()
        {
            return View();
        }

        public ViewResult EditItem()
        {
            ViewBag.id = Guid.NewGuid().ToString();
            return View();
        }

        [HttpPost]
        public JsonResult DoImport()
        {
            var success = true;
            var message = "保存成功！";
            var user = OperatorProvider.Provider.Current();

            if (this.Request.Files.Count > 0)
            {
                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];

                var bll = new HumanDangerBLL();
                try
                {
                    var titleIndex = this.GetTitleRow(sheet);
                    var data = this.GetHumanDangerData(sheet, titleIndex);
                    System.Collections.Hashtable ht = new System.Collections.Hashtable();
                    List<TaskDeptModel> dic = new List<TaskDeptModel>();
                    foreach (var item in data)
                    {

                        item.OperateTime = DateTime.Now;
                        item.OperateUser = user.UserName;
                        item.OperateUserId = user.UserId;
                        if (!string.IsNullOrWhiteSpace(item.DeptId))
                        {
                            var deptIds = item.DeptId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var deptId in deptIds)
                            {
                                if (dic.Any(x => x.DeptId == deptId && x.TaskName == item.Task))
                                {
                                    throw new Exception("存在工作任务与适用部门重复的数据，请检查数据后再操作");
                                }
                                dic.Add(new TaskDeptModel() { DeptId = deptId, TaskName = item.Task });
                            }
                        }
                        else
                        {
                            if (dic.Any(x => x.DeptId == null && x.TaskName == item.Task))
                            {
                                throw new Exception("存在工作任务与适用部门重复的数据，请检查数据后再操作");
                            }
                            dic.Add(new TaskDeptModel() { DeptId = null, TaskName = item.Task });
                        }
                    }
                    bool ishave = bll.CheckUnique(dic);
                    if (ishave)
                    {
                        throw new Exception("存在工作任务与适用部门重复的数据，请检查数据后再操作");
                    }
                    List<HumanDangerEntity> insertList = new List<HumanDangerEntity>();
                    data.ForEach(x =>
                    {
                        string[] deptIds = x.DeptId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var deptNames = x.DeptName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (deptIds != null && deptIds.Length > 0)
                        {
                            for (int i = 0; i < deptIds.Length; i++)
                            {
                                var insert = new HumanDangerEntity
                                {
                                    HumanDangerId = Guid.NewGuid().ToString(),
                                    DeptId = deptIds[i],
                                    DeptName = deptNames[i],
                                    Task = x.Task,
                                    TaskArea = x.TaskArea,
                                    TaskType = x.TaskType,
                                    DangerLevel = x.DangerLevel,
                                    OtherMeasure = x.OtherMeasure,
                                    OperateUser = x.OperateUser,
                                    OperateUserId = x.OperateUserId,
                                    OperateTime = x.OperateTime,
                                    State = x.State,
                                    Measures = x.Measures
                                };
                                insert.Measures = x.Measures.Select(y => new HumanDangerMeasureEntity
                                {
                                    HumanDangerMeasureId = Guid.NewGuid().ToString(),
                                    Category = y.Category,
                                    CategoryId = y.CategoryId,
                                    DangerReason = y.DangerReason,
                                    HumanDangerId = y.HumanDangerId,
                                    MeasureContent = y.MeasureContent,
                                    MeasureId = y.MeasureId
                                }).ToList();
                                //var insert = DataHelper.InputToOutput<HumanDangerEntity, HumanDangerEntity>(x);
                                //insert.Measures.ForEach(p =>
                                //{
                                //    p.HumanDangerMeasureId = Guid.NewGuid();
                                //});
                                //insert.HumanDangerId = Guid.NewGuid();
                                //insert.DeptId = deptIds[i];
                                //insert.DeptName = deptNames[i];
                                insertList.Add(insert);
                            }
                        }
                    });

                    bll.Add(insertList);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        private int GetTitleRow(Worksheet sheet)
        {
            for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
            {
                if (sheet.Cells[i, 0].StringValue == "序号") return i;
            }

            throw new Exception("无法识别文件！");
        }

        private List<HumanDangerEntity> GetHumanDangerData(Worksheet sheet, int titleIndex)
        {
            var result = new List<HumanDangerEntity>();
            var bll = new DangerMeasureBLL();
            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
            var list = dataitembll.GetDataItems("作业性质类型");
            var list2 = dataitembll.GetDataItems("人身伤害风险等级");
            var bll2 = new DangerMeasureBLL();
            var categories = bll2.GetCategories(null);
            var reasons = bll2.GetAllReasons();
            DepartmentBLL departmentBLL = new DepartmentBLL();
            var departList = departmentBLL.GetList();//所有的部门
            for (int i = titleIndex + 1; i <= sheet.Cells.MaxDataRow; i++)
            {
                var humandanger = new HumanDangerEntity();
                if (sheet.Cells[i, 1].IsMerged)
                {
                    var range = sheet.Cells[i, 1].GetMergedRange();
                    if (range.FirstRow == i)
                    {
                        humandanger.HumanDangerId = Guid.NewGuid().ToString();
                        humandanger.Task = sheet.Cells[i, 1].StringValue;
                        if (string.IsNullOrEmpty(humandanger.Task)) throw new Exception(string.Format("行 {0} 工作任务为空！", i + 1));
                        if (humandanger.Task.Length > 200) throw new Exception(string.Format("行 {0} 工作任务字数超过200！", i + 1));
                        humandanger.TaskArea = sheet.Cells[i, 2].StringValue;
                        if (!string.IsNullOrEmpty(humandanger.TaskArea) && humandanger.TaskArea.Length > 500) throw new Exception(string.Format("行 {0} 作业区域字数超过500！", i + 1));
                        humandanger.DeptName = sheet.Cells[i, 3].StringValue;
                        if (!string.IsNullOrEmpty(humandanger.DeptName) && humandanger.DeptName.Length > 500) throw new Exception(string.Format("行 {0} 使用班组字数超过500！", i + 1));
                        var depts = humandanger.DeptName.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                        humandanger.DeptName = string.Empty;
                        List<string> addedDeptIds = new List<string>();//已经记录的部门的ID。在添加下级部门时，防止重复添加下级部门
                        if (depts != null || depts.Count > 0)
                        {
                            foreach (var dept in depts)
                            {
                                var thisDept = departList.FirstOrDefault(x => x.FullName.Equals(dept));
                                if (thisDept != null && !addedDeptIds.Any(x => x == thisDept.DepartmentId))//没包含过的才添加
                                {
                                    humandanger.DeptName += thisDept.FullName + ",";
                                    humandanger.DeptId += thisDept.DepartmentId + ",";
                                    addedDeptIds.Add(thisDept.DepartmentId);
                                    //如果是部门或者公司级别的，则连下级部门一起添加进去
                                    var childDept = departList.Where(m => m.EnCode.Contains(thisDept.EnCode)).ToList();
                                    if (childDept != null && childDept.Count > 0)
                                    {
                                        childDept.ForEach(child =>
                                        {
                                            if (child != null && !addedDeptIds.Any(x => x == child.DepartmentId))//没包含过的才添加
                                            {
                                                humandanger.DeptName += child.FullName + ",";
                                                humandanger.DeptId += child.DepartmentId + ",";
                                                addedDeptIds.Add(child.DepartmentId);
                                            }
                                        });
                                    }

                                }
                            }
                            if (!string.IsNullOrWhiteSpace(humandanger.DeptId) && !string.IsNullOrWhiteSpace(humandanger.DeptName))
                            {
                                humandanger.DeptName = humandanger.DeptName.TrimEnd(',');
                                humandanger.DeptId = humandanger.DeptId.TrimEnd(',');
                            }
                        }

                        humandanger.TaskType = sheet.Cells[i, 4].StringValue;
                        if (string.IsNullOrEmpty(humandanger.TaskType)) throw new Exception(string.Format("行 {0} 作业性质类型为空！", i + 1));
                        var types = humandanger.TaskType.Split(';');
                        if (!types.All(x => list.Any(y => y.ItemValue == x))) throw new Exception(string.Format("行 {0} 作业性质类型不存在！", i + 1));
                        humandanger.TaskType = string.Join(",", types);
                        humandanger.DangerLevel = sheet.Cells[i, 8].StringValue;
                        if (!string.IsNullOrEmpty(humandanger.DangerLevel) && !list2.Any(x => x.ItemValue == humandanger.DangerLevel)) throw new Exception(string.Format("行 {0} 人身伤害风险不存在！", i + 1));
                        humandanger.OtherMeasure = sheet.Cells[i, 9].StringValue;
                        if (!string.IsNullOrEmpty(humandanger.OtherMeasure) && humandanger.OtherMeasure.Length > 500) throw new Exception(string.Format("行 {0} 预控效果及补充措施字数超过500！", i + 1));
                        humandanger.Measures = new List<HumanDangerMeasureEntity>();
                        var measure = new HumanDangerMeasureEntity();
                        measure.HumanDangerMeasureId = Guid.NewGuid().ToString();
                        measure.Category = sheet.Cells[i, 5].StringValue;
                        if (string.IsNullOrEmpty(measure.Category)) throw new Exception(string.Format("行 {0} 风险类别为空！", i + 1));
                        var category = categories.Find(x => x.CategoryName == measure.Category);
                        if (category == null) throw new Exception(string.Format("行 {0} 风险类别不存在！", i + 1));
                        measure.CategoryId = category.CategoryId;
                        measure.DangerReason = sheet.Cells[i, 6].StringValue;
                        if (string.IsNullOrEmpty(measure.DangerReason)) throw new Exception(string.Format("行 {0} 风险因素为空！", i + 1));
                        var reason = reasons.Find(x => x.CategoryId == measure.CategoryId && x.DangerReason == measure.DangerReason);
                        if (reason == null) throw new Exception(string.Format("行 {0} 风险因素不存在！", i + 1));
                        measure.MeasureId = reason.MeasureId;
                        measure.MeasureContent = sheet.Cells[i, 7].StringValue;
                        if (!string.IsNullOrEmpty(measure.MeasureContent) && measure.MeasureContent.Length > 500) throw new Exception(string.Format("行 {0} 风险预控措施字数超过500！", i + 1));
                        humandanger.Measures.Add(measure);
                        result.Add(humandanger);
                    }
                    else
                    {
                        humandanger = result.Last();
                        var measure = new HumanDangerMeasureEntity();
                        measure.HumanDangerMeasureId = Guid.NewGuid().ToString();
                        measure.Category = sheet.Cells[i, 5].StringValue;
                        if (string.IsNullOrEmpty(measure.Category)) throw new Exception(string.Format("行 {0} 风险类别为空！", i + 1));
                        var category = categories.Find(x => x.CategoryName == measure.Category);
                        if (category == null) throw new Exception(string.Format("行 {0} 风险类别不存在！", i + 1));
                        measure.CategoryId = category.CategoryId;
                        measure.DangerReason = sheet.Cells[i, 6].StringValue;
                        if (string.IsNullOrEmpty(measure.DangerReason)) throw new Exception(string.Format("行 {0} 风险因素为空！", i + 1));
                        var reason = reasons.Find(x => x.CategoryId == measure.CategoryId && x.DangerReason == measure.DangerReason);
                        if (reason == null) throw new Exception(string.Format("行 {0} 风险因素不存在！", i + 1));
                        measure.MeasureId = reason.MeasureId;
                        measure.MeasureContent = sheet.Cells[i, 7].StringValue;
                        if (!string.IsNullOrEmpty(measure.MeasureContent) && measure.MeasureContent.Length > 500) throw new Exception(string.Format("行 {0} 风险预控措施字数超过500！", i + 1));
                        humandanger.Measures.Add(measure);
                    }
                }
                else
                {
                    humandanger.HumanDangerId = Guid.NewGuid().ToString();
                    humandanger.Task = sheet.Cells[i, 1].StringValue;
                    if (string.IsNullOrEmpty(humandanger.Task)) throw new Exception(string.Format("行 {0} 工作任务为空！", i + 1));
                    if (humandanger.Task.Length > 200) throw new Exception(string.Format("行 {0} 工作任务字数超过200！", i + 1));
                    humandanger.TaskArea = sheet.Cells[i, 2].StringValue;
                    if (!string.IsNullOrEmpty(humandanger.TaskArea) && humandanger.TaskArea.Length > 500) throw new Exception(string.Format("行 {0} 作业区域字数超过500！", i + 1));
                    humandanger.DeptName = sheet.Cells[i, 3].StringValue;
                    if (!string.IsNullOrEmpty(humandanger.DeptName) && humandanger.DeptName.Length > 500) throw new Exception(string.Format("行 {0} 使用班组字数超过500！", i + 1));
                    humandanger.TaskType = sheet.Cells[i, 4].StringValue;
                    if (string.IsNullOrEmpty(humandanger.TaskType)) throw new Exception(string.Format("行 {0} 作业性质类型为空！", i + 1));
                    var types = humandanger.TaskType.Split(';');
                    if (!types.All(x => list.Any(y => y.ItemValue == x))) throw new Exception(string.Format("行 {0} 作业性质类型不存在！", i + 1));
                    humandanger.TaskType = string.Join(",", types);
                    humandanger.DangerLevel = sheet.Cells[i, 8].StringValue;
                    if (humandanger.DangerLevel != null && !list2.Any(x => x.ItemValue == humandanger.DangerLevel)) throw new Exception(string.Format("行 {0} 人身伤害风险不存在！", i + 1));
                    humandanger.OtherMeasure = sheet.Cells[i, 9].StringValue;
                    if (!string.IsNullOrEmpty(humandanger.OtherMeasure) && humandanger.OtherMeasure.Length > 500) throw new Exception(string.Format("行 {0} 预控效果及补充措施字数超过500！", i + 1));
                    humandanger.Measures = new List<HumanDangerMeasureEntity>();
                    var measure = new HumanDangerMeasureEntity();
                    measure.HumanDangerMeasureId = Guid.NewGuid().ToString();
                    measure.Category = sheet.Cells[i, 5].StringValue;
                    if (string.IsNullOrEmpty(measure.Category)) throw new Exception(string.Format("行 {0} 风险类别为空！", i + 1));
                    var category = categories.Find(x => x.CategoryName == measure.Category);
                    if (category == null) throw new Exception(string.Format("行 {0} 风险类别不存在！", i + 1));
                    measure.CategoryId = category.CategoryId;
                    measure.DangerReason = sheet.Cells[i, 6].StringValue;
                    if (string.IsNullOrEmpty(measure.DangerReason)) throw new Exception(string.Format("行 {0} 风险因素为空！", i + 1));
                    var reason = reasons.Find(x => x.CategoryId == measure.CategoryId && x.DangerReason == measure.DangerReason);
                    if (reason == null) throw new Exception(string.Format("行 {0} 风险因素不存在！", i + 1));
                    measure.MeasureId = reason.MeasureId;
                    measure.MeasureContent = sheet.Cells[i, 7].StringValue;
                    if (!string.IsNullOrEmpty(measure.MeasureContent) && measure.MeasureContent.Length > 500) throw new Exception(string.Format("行 {0} 风险预控措施字数超过500！", i + 1));
                    humandanger.Measures.Add(measure);
                    result.Add(humandanger);
                }
            }

            return result;
        }
        /// <summary>
        /// 预控措施关联同步
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult MeasureAssociation()
        {
            try
            {
                var bll = new HumanDangerBLL();
                bll.Association();
            }
            catch (Exception ex)
            {
                return Error("操作失败：" + ex.Message);
            }
            return Success("操作成功");
        }

        /// <summary>
        /// 查看Pdf详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public ActionResult ShowPdf(string id, string userName, string taskName)
        {

            string strDocPath = Request.PhysicalApplicationPath + @"Content\export\人身风险预控模板.docx";
            string filePath = Request.PhysicalApplicationPath + @"Resource\HumanDanger\";
            string fileName = id.ToString() + userName + ".PDF";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var bll = new HumanDangerTrainingBLL();
            var entity = bll.GetContent(id).FirstOrDefault(p => p.UserName.Equals(userName));
            if (entity == null)
            {
                return Content("没有找到对应的数据");
            }
            Aspose.Words.Document doc = new Aspose.Words.Document(strDocPath);
            //表格内容
            Dictionary<string, object> di = new Dictionary<string, object>();
            List<string> desc = new List<string>();
            //填充值
            di.Add("deptname", entity.DeptName);
            di.Add("taskname", entity.TrainingTask);
            di.Add("TrainingPlace", entity.TrainingPlace);
            di.Add("CreateTime", entity.TrainingTime.HasValue ? entity.TrainingTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
            di.Add("TrainingUserName", entity.UserName);
            di.Add("No", entity.No);
            di.Add("OtherMeasure", entity.OtherMeasure);
            //作业性质类型
            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
            var list2 = dataitembll.GetDataItems("作业性质类型");
            string taskTypStr = string.Empty;
            list2.ForEach(taskty =>
            {
                if (entity.TaskTypes != null && entity.TaskTypes.Count(x => x.TaskTypeName.Equals(taskty.ItemValue) && x.State != 2) > 0)
                {
                    taskTypStr += "√" + taskty.ItemValue + "  ";
                }
                else
                {
                    taskTypStr += "×" + taskty.ItemValue + "  ";
                }
            });
            di.Add("TaskType", taskTypStr);
            //员因素风险
            DangerMeasureBLL dmbll = new DangerMeasureBLL();


            DataTable dt = new DataTable();
            dt.TableName = "DangerMeasure";
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Content", typeof(string));
            //=========预控措施=======//
            string measurecontent = string.Empty;
            int count = 0;
            //DataTable dtMeasure = new DataTable("Measure");
            //dtMeasure.Columns.Add("MeasureContent", typeof(string));
            //=========预控措施 end =========
            List<DangerMeasureEntity> dmEntity = dmbll.GetAllReasons();
            var groupList = dmEntity.GroupBy(x => x.Category).ToList();
            groupList.ForEach(p =>
            {
                DataRow dr = dt.NewRow();
                dr["Category"] = p.Key;
                StringBuilder str = new StringBuilder();
                var measureList = p.ToList();


                measureList.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.DangerReason))
                    {
                        if (entity.Measures != null && entity.Measures.Any(m => m.DangerReason == x.DangerReason && m.Category == x.Category && m.State != 2))
                        {
                            var measures = entity.Measures.FirstOrDefault(me => me.DangerReason == x.DangerReason);
                            if (measures != null && !string.IsNullOrWhiteSpace(measures.MeasureContent))
                            {
                                count++;
                                measurecontent += count.ToString() + "、" + measures.MeasureContent + "    ";
                            }

                            str.AppendFormat("√{0}  ", x.DangerReason);
                        }
                        else
                        {
                            str.AppendFormat("×{0}  ", x.DangerReason);
                        }
                    }
                });

                dr["Content"] = str.ToString();
                dt.Rows.Add(dr);

            });
            di.Add("MeasureContent", measurecontent);
            //评价
            var activitybll = new ActivityBLL();
            var data = activitybll.GetActivityEvaluateEntity(entity.TrainingId.ToString());
            DataTable dtEvaluate = new DataTable();
            dtEvaluate.TableName = "Evaluate";
            dtEvaluate.Columns.Add("DeptName", typeof(string));
            dtEvaluate.Columns.Add("EvaluateUser", typeof(string));
            dtEvaluate.Columns.Add("EvaluateDate", typeof(string));
            dtEvaluate.Columns.Add("EvaluateContent", typeof(string));
            data.ForEach(evaluate =>
            {
                DataRow evaluatedr = dtEvaluate.NewRow();
                evaluatedr["DeptName"] = evaluate.DeptName;
                evaluatedr["EvaluateUser"] = evaluate.EvaluateUser;
                evaluatedr["EvaluateDate"] = evaluate.EvaluateDate.ToString("yyyy-MM-dd");
                evaluatedr["EvaluateContent"] = evaluate.EvaluateContent;
                dtEvaluate.Rows.Add(evaluatedr);
            });
            doc.MailMerge.ExecuteWithRegions(dt);
            doc.MailMerge.ExecuteWithRegions(dtEvaluate);
            //doc.MailMerge.ExecuteWithRegions(dtMeasure);
            doc.MailMerge.Execute(di.Keys.ToArray(), di.Values.ToArray());
            doc.MailMerge.DeleteFields();
            doc.Save(filePath + fileName, Aspose.Words.SaveFormat.Pdf);

            return File(filePath + fileName, "application/pdf", taskName + "_" + userName + "评.pdf");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult BatchPdf(string trainingUserIds, DateTime? starttime, DateTime? endtime)
        {
            try
            {
                Dictionary<string, Stream> streams = new Dictionary<string, Stream>();
                string strDocPath = Request.PhysicalApplicationPath + @"Content\export\人身风险预控模板.docx";
                string filePath = Request.PhysicalApplicationPath + @"Resource\HumanDanger\";
                var trainUserIds = trainingUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (!string.IsNullOrWhiteSpace(trainingUserIds))
                {
                    int filecount = 0;
                    var bll = new HumanDangerTrainingBLL();
                    if (endtime.HasValue)
                    {
                        endtime = endtime.Value.AddDays(1).Date;
                    }
                    List<HumanDangerTrainingEntity> entities = bll.GetTrainingsByTrainingUser(trainUserIds.ToArray(), starttime, endtime);
                    if (entities != null && entities.Count > 0)
                    {
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        entities = entities.Where(x => trainUserIds.Contains(x.TrainingUserId.ToString())).ToList();
                        //评价
                        var activitybll = new ActivityBLL();
                        var evalueteUserIds = entities.SelectMany(x => x.TrainingUsers).Select(x => x.TrainingUserId.ToString()).ToList();
                        var evaluateList = activitybll.GetActivityEvaluateEntity(evalueteUserIds);

                        entities.ForEach(entity =>
                        {
                            var saveFilePath = filePath + entity.TrainingId.ToString() + entity.UserName + ".PDF";

                            Aspose.Words.Document doc = new Aspose.Words.Document(strDocPath);
                            //表格内容
                            Dictionary<string, object> di = new Dictionary<string, object>();
                            List<string> desc = new List<string>();
                            //填充值
                            di.Add("deptname", entity.DeptName);
                            di.Add("taskname", entity.TrainingTask);
                            di.Add("TrainingPlace", entity.TrainingPlace);
                            di.Add("CreateTime", entity.TrainingTime.HasValue ? entity.TrainingTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
                            di.Add("TrainingUserName", entity.UserName);
                            di.Add("No", entity.No);
                            di.Add("OtherMeasure", entity.OtherMeasure);
                            //作业性质类型
                            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
                            var list2 = dataitembll.GetDataItems("作业性质类型");
                            string taskTypStr = string.Empty;
                            list2.ForEach(taskty =>
                            {
                                if (entity.TaskTypes != null && entity.TaskTypes.Count(x => x.TaskTypeName.Equals(taskty.ItemValue) && x.State != 2) > 0)
                                {
                                    taskTypStr += "√" + taskty.ItemValue + "  ";
                                }
                                else
                                {
                                    taskTypStr += "×" + taskty.ItemValue + "  ";
                                }
                            });
                            di.Add("TaskType", taskTypStr);
                            //员因素风险
                            DangerMeasureBLL dmbll = new DangerMeasureBLL();
                            List<DangerMeasureEntity> dmEntity = dmbll.GetAllReasons();
                            var groupList = dmEntity.GroupBy(x => x.Category).ToList();
                            DataTable dt = new DataTable();
                            dt.TableName = "DangerMeasure";
                            dt.Columns.Add("Category", typeof(string));
                            dt.Columns.Add("Content", typeof(string));
                            //=========预控措施=======
                            string measurecontent = string.Empty;
                            int count = 0;
                            //=========预控措施 end =========
                            groupList.ForEach(p =>
                            {
                                DataRow dr = dt.NewRow();
                                dr["Category"] = p.Key;
                                StringBuilder str = new StringBuilder();
                                var measureList = p.ToList();
                                measureList.ForEach(x =>
                                {

                                    if (!string.IsNullOrWhiteSpace(x.DangerReason))
                                    {
                                        if (entity.Measures != null && entity.Measures.Any(m => m.DangerReason == x.DangerReason && m.Category == x.Category && m.State != 2))
                                        {

                                            var measures = entity.Measures.FirstOrDefault(me => me.DangerReason == x.DangerReason);
                                            if (measures != null && !string.IsNullOrWhiteSpace(measures.MeasureContent))
                                            {
                                                count++;
                                                measurecontent += count.ToString() + "、" + measures.MeasureContent + "    ";
                                            }

                                            str.AppendFormat("√{0}  ", x.DangerReason);
                                        }
                                        else
                                        {
                                            str.AppendFormat("×{0}  ", x.DangerReason);
                                        }
                                    }
                                });

                                dr["Content"] = str.ToString();
                                dt.Rows.Add(dr);
                            });
                            di.Add("MeasureContent", measurecontent);

                            //==============评价========
                            var thisEvaluate = evaluateList.Where(ev => ev.Activityid == entity.UserId).ToList();
                            DataTable dtEvaluate = new DataTable();
                            dtEvaluate.TableName = "Evaluate";
                            dtEvaluate.Columns.Add("DeptName", typeof(string));
                            dtEvaluate.Columns.Add("EvaluateUser", typeof(string));
                            dtEvaluate.Columns.Add("EvaluateDate", typeof(string));
                            dtEvaluate.Columns.Add("EvaluateContent", typeof(string));
                            thisEvaluate.ForEach(evaluate =>
                            {
                                DataRow evaluatedr = dtEvaluate.NewRow();
                                evaluatedr["DeptName"] = evaluate.DeptName;
                                evaluatedr["EvaluateUser"] = evaluate.EvaluateUser;
                                evaluatedr["EvaluateDate"] = evaluate.EvaluateDate.ToString("yyyy-MM-dd");
                                evaluatedr["EvaluateContent"] = evaluate.EvaluateContent;
                                dtEvaluate.Rows.Add(evaluatedr);
                            });
                            doc.MailMerge.ExecuteWithRegions(dt);
                            doc.MailMerge.ExecuteWithRegions(dtEvaluate);
                            //doc.MailMerge.ExecuteWithRegions(dtMeasure);
                            doc.MailMerge.Execute(di.Keys.ToArray(), di.Values.ToArray());
                            doc.MailMerge.DeleteFields();
                            doc.Save(saveFilePath, Aspose.Words.SaveFormat.Pdf);
                            Stream streamWriter = null;
                            filecount++;
                            streamWriter = System.IO.File.Open(saveFilePath, FileMode.Open);
                            streams.Add(entity.TrainingTask + "_" + entity.UserName + "_" + entity.CreateTime.ToString("yyyyMMddHHmmss") + filecount.ToString() + ".PDF", streamWriter);
                        });






                        //下面可以直接复制粘贴
                        MemoryStream ms = new MemoryStream();
                        ms = PackageManyZip(streams);
                        byte[] bytes = new byte[(int)ms.Length];
                        ms.Read(bytes, 0, bytes.Length);
                        ms.Close();
                        Response.ContentType = "application/octet-stream";
                        //通知浏览器下载文件而不是打开
                        Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode("人身风险预控.zip", System.Text.Encoding.UTF8));
                        Response.BinaryWrite(bytes);
                        Response.Flush();

                        return Content("正在生成PDF文件，稍后自动开始下载");
                    }
                }
                return Content("未找到可以导出的数据");
            }
            catch (Exception ex)
            {
                return Content("下载文件失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 导出台账条件选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportForm()
        {
            return View();
        }

        /// <summary>
        /// 分析人选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UserForm()
        {
            return View();
        }


        /// <summary>
        /// 将多个流进行zip压缩，返回压缩后的流
        /// </summary>
        /// <param name="streams">key：文件名；value：文件名对应的要压缩的流</param>
        /// <returns>压缩后的流</returns>
        static MemoryStream PackageManyZip(Dictionary<string, Stream> streams)
        {
            byte[] buffer = new byte[6500];
            MemoryStream returnStream = new MemoryStream();
            var zipMs = new MemoryStream();
            using (ZipOutputStream zipStream = new ZipOutputStream(zipMs))
            {
                zipStream.SetLevel(9);
                foreach (var kv in streams)
                {
                    string fileName = kv.Key;
                    using (var streamInput = kv.Value)
                    {
                        zipStream.PutNextEntry(new ZipEntry(fileName));
                        while (true)
                        {
                            var readCount = streamInput.Read(buffer, 0, buffer.Length);
                            if (readCount > 0)
                            {
                                zipStream.Write(buffer, 0, readCount);
                            }
                            else
                            {
                                break;
                            }
                        }
                        zipStream.Flush();
                    }
                }
                zipStream.Finish();
                zipMs.Position = 0;
                zipMs.CopyTo(returnStream, 5600);
            }
            returnStream.Position = 0;
            return returnStream;
        }

        public JsonResult GetToEvaluate(string analyst, DateTime? begin, DateTime? end, int rows, int page)
        {
            if (end != null) end = end.Value.AddDays(1);
            var user = OperatorProvider.Provider.Current();
            var total = 0;
            var bll = new HumanDangerTrainingBLL();
            var data = bll.GetToEvaluate(user.DeptId, analyst, begin, end, rows, page, out total);
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page });
        }

        public ViewResult Template(string name, string status, bool? onlyMine = false)
        {
            var user = OperatorProvider.Provider.Current();
            var people = new PeopleBLL().GetEntity(user.UserId);
            var isApproveUser = false;
            if (people != null && (people.Quarters.Contains("班长") || people.Quarters.Contains("副班长") || people.Quarters.Contains("技术员"))) isApproveUser = true;
            ViewBag.isApproveUser = isApproveUser;

            if (onlyMine == true)
            {
                name = string.Empty;
                status = "全部";
            }
            ViewData["name"] = name;
            var list = new SelectList(new List<string>() { "全部", "班组审核中", "部门审核中", "公司审核中" }, status);
            ViewData["status"] = list;
            ViewData["onlyMine"] = onlyMine;

            var code = 0;
            switch (status)
            {
                case "班组审核中":
                    code = 1;
                    break;
                case "部门审核中":
                    code = 2;
                    break;
                case "公司审核中":
                    code = 3;
                    break;
                default:
                    break;
            }

            if (onlyMine == true)
            {
                var dept = new DepartmentBLL().GetEntity(user.DeptId);
                switch (dept.Nature)
                {
                    case "班组":
                        code = 1;
                        break;
                    case "部门":
                        code = 2;
                        break;
                    case "厂级":
                        code = 3;
                        break;
                    default:
                        break;
                }
            }

            var bll = new HumanDangerBLL();
            var total = 0;
            var data = bll.GetTemplates(user.DeptId, name, code, int.MaxValue, 1, out total);
            foreach (var item in data)
            {
                switch (item.State)
                {
                    case 1:
                        item.StateDescription = "班组审核中";
                        break;
                    case 2:
                        item.StateDescription = "部门审核中";
                        break;
                    case 3:
                        item.StateDescription = "公司审核中";
                        break;
                    default:
                        break;
                }
            }

            return View(data);
        }

        public JsonResult TemplateData(string deptid, string name, int? status, int rows, int page)
        {
            var code = 0;

            var total = 0;
            var bll = new HumanDangerBLL();
            var data = bll.GetTemplates(deptid, name, status ?? 0, rows, page, out total);

            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page });
        }

        public ViewResult Edit3(string id)
        {
            HumanDangerEntity model = null;
            var user = OperatorProvider.Provider.Current();
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new HumanDangerBLL();
                model = bll.GetDetail(id);
            }
            else
            {
                model = new HumanDangerEntity()
                {
                    OperateTime = DateTime.Now,
                    OperateUserId = user.UserId,
                    OperateUser = user.UserName
                };
            }

            model.ApproveUser = user.UserName;
            model.ApproveTime = DateTime.Now;

            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
            var list1 = dataitembll.GetDataItems("人身伤害风险等级");
            var list2 = dataitembll.GetDataItems("作业性质类型");

            var data1 = list1.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });
            var data2 = list2.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });

            ViewData["DangerLevel"] = data1;
            ViewData["TaskType"] = data2;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View(model);
        }

        public ViewResult Edit4(string id)
        {
            HumanDangerEntity model = null;
            var user = OperatorProvider.Provider.Current();
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new HumanDangerBLL();
                model = bll.GetDetail(id);
            }
            else
            {
                model = new HumanDangerEntity()
                {
                    OperateTime = DateTime.Now,
                    OperateUserId = user.UserId,
                    OperateUser = user.UserName
                };
            }

            model.ApproveRecords = new ApproveRecordBLL().List(model.HumanDangerId.ToString());

            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
            var list1 = dataitembll.GetDataItems("人身伤害风险等级");
            var list2 = dataitembll.GetDataItems("作业性质类型");

            var data1 = list1.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });
            var data2 = list2.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });

            ViewData["DangerLevel"] = data1;
            ViewData["TaskType"] = data2;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View(model);
        }
    }
}
