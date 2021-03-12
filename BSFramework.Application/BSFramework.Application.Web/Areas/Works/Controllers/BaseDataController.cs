using Aspose.Cells;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Service.Activity;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.Offices;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class BaseDataController : MvcControllerBase
    {
        //
        // GET: /Works/BaseData/
        private WorkmeetingBLL workmeetingbll = new WorkmeetingBLL();

        public ViewResult Index(int? page, int? pagesize, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var total = 0;
            var name = fc.Get("name");
            var data = workmeetingbll.GetBaseData(user.DeptId, name, string.Empty, pagesize ?? 15, page ?? 1, out total);
            ViewBag.pages = Math.Ceiling((decimal)total / (pagesize ?? 15));
            ViewBag.current = page ?? 1;
            ViewBag.name = name;
            return View(data);
        }

        public ActionResult Import(FormCollection fc)
        {
            return View();
        }
        public ActionResult Danger(string jobid, string dangerid)
        {
            var user = OperatorProvider.Provider.Current();
            DangerTemplateEntity entity = new DangerTemplateEntity();
            if (string.IsNullOrEmpty(dangerid))
            {
                entity.JobId = jobid;
                entity.DangerId = Guid.NewGuid().ToString();
                entity.CreateTime = DateTime.Now;
                entity.CreateUserId = user.UserId;
            }
            else
            {
                entity = workmeetingbll.getdangtemplateentity(dangerid);
            }
            return View(entity);
        }
        [HttpPost]
        public ActionResult SaveMeasure(DangerTemplateEntity model)
        {
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            if (dept == null)
            {
                user.DeptCode = "0";
            }
            else if (dept.Nature == "部门")
            {
                var pdept = deptBll.GetEntity(dept.ParentId);
                user.DeptCode = pdept == null ? dept.EnCode : pdept.EnCode;
            }
            model.DeptCode = user.DeptCode;
            workmeetingbll.savedangertemplateentity(model);

            return Success("成功");
        }
        public ActionResult Index2(FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;
            ViewBag.deptName = user.DeptName;
            return View();
        }
        public ActionResult GetDangers(string jobid, int rows)
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;

            var user = OperatorProvider.Provider.Current();
            var bll = new ActivityBLL();
            //var data = bll.GetEvaluationsManoeuvre(name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
            var data = workmeetingbll.getdangertemplate(jobid);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id ==  wg_jobtemlate - jobid</param>
        /// <returns></returns>
        public ActionResult EditList(string id)
        {
            MeasuresBLL mbll = new MeasuresBLL();
            var meetingjoblist = workmeetingbll.GetJobs("", "", "", "", "").Where(x => x.TemplateId == id).Select(x => x.JobId).ToList();
            var dangers = new DangerService().GetList("").ToList().Where(x => meetingjoblist.Contains(x.JobId)).ToList();
            foreach (DangerEntity d in dangers)
            {
                var measures = mbll.GetMeasureList(d.Id).OrderBy(x => x.DangerSource);
                var dangertemplates = workmeetingbll.getdangertemplate(id).OrderBy(x => x.Dangerous);
                int i = 1;
                foreach (MeasuresEntity m in measures)
                {
                    d.newdanger += "(" + i + ")." + m.DangerSource + "<br />";
                    d.newmeasure += "(" + i + ")." + m.Measure + "<br />";
                    i++;
                }
                i = 1;
                foreach (DangerTemplateEntity t in dangertemplates)
                {
                    d.olddanger += "(" + i + ")." + t.Dangerous + "<br />";
                    d.oldmeasure += "(" + i + ")." + t.Measure + "<br />";
                    i++;
                }
            }
            ViewData["dangers"] = dangers;
            return View();
        }
        public ActionResult Form(string id)
        {
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            if (dept == null)
            {
                user.DeptCode = "0";
            }
            else if (dept.Nature == "部门")
            {
                var pdept = deptBll.GetEntity(dept.ParentId);
                user.DeptCode = pdept == null ? dept.EnCode : pdept.EnCode;
            }
            var model = new JobTemplateEntity();

            if (!string.IsNullOrEmpty(id))
            {
                model = workmeetingbll.GetJobTemplate(id);
                model.Dangers = workmeetingbll.getdangertemplate(model.JobId);
                //foreach (DangerTemplateEntity d in model.Dangers)
                //{
                //    model.d += d.Dangerous + "\r\n";
                //    model.m += d.Measure + "\r\n";
                //}
            }
            else
            {


                model.JobId = Guid.NewGuid().ToString();
                model.CreateUser = user.UserName;
                model.CreateUserId = user.UserId;
                model.CreateDate = DateTime.Now;
                model.DeptCode = user.DeptCode;
            }
            return View(model);
        }
        public ActionResult Detail(string id)
        {
            ViewBag.path = "";
            ViewBag.url = "";
            FileInfoBLL fileBll = new FileInfoBLL();
            var model = workmeetingbll.GetJobTemplate(id);
            model.Dangers = workmeetingbll.getdangertemplate(model.JobId);
            foreach (DangerTemplateEntity d in model.Dangers)
            {
                model.d += d.Dangerous + "\r\n";
                model.m += d.Measure + "\r\n";
            }
            FileInfoEntity fe = fileBll.GetFilesByRecIdNew(model.JobId).FirstOrDefault();
            if (fe != null)
            {
                ViewBag.path = fe.FileId;
                ViewBag.url = Url.Content(fe.FilePath);
            }
            return View(model);
        }
        public JsonResult DeleteOne(string keyValue)
        {
            workmeetingbll.DeleteJobTemplate(keyValue);
            return Json(new { success = true, message = "删除成功" });
        }
        public JsonResult DeleteDanger(string keyValue)
        {
            workmeetingbll.deldangertemplateentity(keyValue);
            return Json(new { success = true, message = "删除成功" });
        }
        [HttpPost]
        public JsonResult SaveForm(string id, JobTemplateEntity model)
        {
            var user = OperatorProvider.Provider.Current();
            var success = true;
            var message = "保存成功";
            model.JobType = "danger";
            model.CreateUserId = user.UserId;
            model.CreateUser = user.UserName;

            if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();
            foreach (var item in model.DangerousList)
            {
                if (string.IsNullOrEmpty(item.JobDangerousId)) item.JobDangerousId = Guid.NewGuid().ToString();
                item.CreateTime = DateTime.Now;
                item.JobId = model.JobId;
                if (item.MeasureList == null) item.MeasureList = new List<JobMeasureEntity>();
                foreach (var item1 in item.MeasureList)
                {
                    if (string.IsNullOrEmpty(item1.JobMeasureId)) item1.JobMeasureId = Guid.NewGuid().ToString();
                    item1.CreateTime = DateTime.Now;
                    item1.JobDangerousId = item.JobDangerousId;
                }
            }


            try
            {
                string r = workmeetingbll.UpdateJobTemplate(model);
                if (r == "1")
                    message = "操作失败，危险源与防范措施必须保持一致。";
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
        public ActionResult UploadFile(string id)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            IList<FileInfoEntity> fl = fileBll.GetFilesByRecIdNew(id);
            foreach (FileInfoEntity fe in fl)
            {
                string filepath = fileBll.Delete(fe.FileId);
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath("~" + filepath)))
                    System.IO.File.Delete(Server.MapPath("~" + filepath));
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string FileEextension = Path.GetExtension(files[0].FileName);
            string type = files[0].ContentType;
            if (!type.Contains("image"))
            {
                return Success("1");
            }
            string Id = OperatorProvider.Provider.Current().UserId;
            Id = Guid.NewGuid().ToString();
            string virtualPath = string.Format("~/Content/jobtemplate/{0}{1}", Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);


            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = id,
                RecId = id,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension,
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0
            };
            fileBll.SaveForm(fi);
            return Success("上传成功。", new { path = virtualPath.TrimStart('~'), name = fi.FileName });
        }
        public ActionResult ImportNew()
        {
            return View();
        }
        public ActionResult export(string name)
        {
            var user = OperatorProvider.Provider.Current();
            //取出数据源
            int total = 0;
            DataTable exportTable = workmeetingbll.getExport(name);
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            //excelconfig.Title = "违章信息";
            //excelconfig.TitleFont = "微软雅黑";
            //excelconfig.TitlePoint = 25;
            excelconfig.HeadHeight = 50;
            excelconfig.HeadPoint = 12;
            excelconfig.HeadFont = "宋体";
            excelconfig.FileName = "危险预知训练导出.xls";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            ColumnEntity columnentity = new ColumnEntity();
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "jobcontent", ExcelColumn = "作业内容", Width = 12, Alignment = "fill" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "workquarters", ExcelColumn = "作业岗位", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "usetime", ExcelColumn = "使用次数", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "redactiondate", ExcelColumn = "修订时间", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "redactionperson", ExcelColumn = "修订人", Width = 18 });
            //调用导出方法
            ExcelHelper.ExcelDownload(exportTable, excelconfig);
            return Success("导出成功。");
        }
        [HttpGet]
        public JsonResult GetData()
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var content = this.Request.QueryString.Get("jobcontent");
            var total = 0;
            var data = workmeetingbll.GetBaseDataNew(content, pagesize, page, out total);
            foreach (JobTemplateEntity j in data)
            {
                if (j.Usetime == 0 || j.EditTime == 0)
                {
                    j.Percent = 0;
                }
                else
                {
                    j.Percent = Math.Round(Convert.ToDecimal(j.EditTime) / j.Usetime, 2);
                }
            }
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {

            var user = OperatorProvider.Provider.Current();

            var watch = CommonHelper.TimerStart();
            var data = workmeetingbll.GetPageList(user.DeptCode, pagination, queryJson);
            foreach (JobTemplateEntity j in data)
            {
                if (j.Usetime == 0 || j.EditTime == 0)
                {
                    j.Percent = 0;
                }
                else
                {
                    j.Percent = Math.Round(Convert.ToDecimal(j.EditTime) / j.Usetime, 2);
                }
            }
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
        public JsonResult DoImportNew()
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            var success = true;
            var message = string.Empty;

            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) throw new Exception("请上传 Excel 文件");

                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];
                DepartmentBLL deptBll = new DepartmentBLL();
                Operator user = OperatorProvider.Provider.Current();
                var dept = deptBll.GetEntity(user.DeptId);
                if (dept == null)
                {
                    user.DeptCode = "0";
                }
                else if (dept.Nature == "部门")
                {
                    var pdept = deptBll.GetEntity(dept.ParentId);
                    user.DeptCode = pdept == null ? dept.EnCode : pdept.EnCode;
                }


                if (sheet.Cells[0, 1].StringValue != "工作任务" || sheet.Cells[0, 2].StringValue != "作业类别" || sheet.Cells[0, 3].StringValue != "作业岗位" || sheet.Cells[0, 4].StringValue != "任务描述" || sheet.Cells[0, 5].StringValue != "资源准备" || sheet.Cells[0, 6].StringValue != "作业区域" || sheet.Cells[0, 7].StringValue != "潜在危险" || sheet.Cells[0, 8].StringValue != "防范措施")
                {
                    return Json(new { success = false, message = "请使用正确的模板导入！" });
                }
                var templates = new List<JobTemplateEntity>();
                var dtemplates = new List<DangerTemplateEntity>();
                var date = DateTime.Now;
                for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new JobTemplateEntity();
                    var dentity = new DangerTemplateEntity();
                    entity.CreateUser = user.UserName;
                    entity.CreateUserId = user.UserId;
                    entity.JobId = Guid.NewGuid().ToString();
                    entity.JobContent = sheet.Cells[i, 1].StringValue;
                    entity.WorkType = sheet.Cells[i, 2].StringValue;
                    entity.WorkQuarters = sheet.Cells[i, 3].StringValue;
                    entity.WorkDescribe = sheet.Cells[i, 4].StringValue;
                    entity.ResPrepare = sheet.Cells[i, 5].StringValue;
                    entity.WorkArea = sheet.Cells[i, 6].StringValue;
                    //entity.DeptId = user.DeptId;
                    entity.CreateDate = date.AddMinutes(i);
                    entity.DangerType = "";
                    entity.JobType = "danger";
                    if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                    {
                        entity.JobId = templates[i - 1 - 1].JobId;
                    }
                    entity.PicNumber = sheet.Cells[i, 9].StringValue;
                    entity.DeptCode = user.DeptCode;

                    if (!string.IsNullOrEmpty(entity.JobContent))
                    {
                        if (templates.Exists(x => x.JobContent == entity.JobContent))
                            throw new Exception(string.Format("行 {0} 任务重复！", i + 1));
                        if (workmeetingbll.ExistJob(entity))
                            throw new Exception(string.Format("行 {0} 任务已存在！", i + 1));
                    }

                    templates.Add(entity);


                }


                for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var dentity = new DangerTemplateEntity();
                    dentity.Dangerous = sheet.Cells[i, 7].StringValue;
                    dentity.Measure = sheet.Cells[i, 8].StringValue;
                    dentity.CreateTime = DateTime.Now;
                    dentity.CreateUserId = user.UserId;
                    dentity.JobId = templates[i - 1].JobId;
                    dentity.DangerId = Guid.NewGuid().ToString();
                    dtemplates.Add(dentity);
                }
                templates = templates.Where(x => x.JobContent != "").ToList();

                for (int i = 0; i < templates.Count(); i++)
                {

                    //if (System.IO.File.Exists("D:\\JobImages\\" + templates[i].PicNumber)) 
                    //{
                    //    FileStream fs = new FileStream("D:\\JobImages\\" + templates[i].PicNumber, FileMode.Open);

                    //}
                    //string virtualPath = string.Format("Content/export/pics/{0}", templates[i].PicNumber);
                    //image

                    FileInfoEntity fi = new FileInfoEntity();
                    fi.FileId = Guid.NewGuid().ToString();
                    fi.RecId = Guid.NewGuid().ToString();
                    fi.FolderId = templates[i].JobId;
                    fi.RecId = templates[i].JobId;
                    fi.FileName = templates[i].PicNumber;
                    fi.FilePath = "~/Content/export/pics/" + fi.FileName;
                    fi.FileType = "";
                    fi.FileExtensions = "";
                    fi.FileSize = "";
                    fi.DeleteMark = 0;
                    fileBll.SaveForm(fi);
                }



                workmeetingbll.AddJobTemplates(templates);
                workmeetingbll.AddDangerTemplates(dtemplates);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }
        public JsonResult DoImport()
        {
            var success = true;
            var message = string.Empty;

            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) throw new Exception("请上传 Excel 文件");

                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];
                var user = OperatorProvider.Provider.Current();

                var templates = new List<JobTemplateEntity>();
                var date = DateTime.Now;
                UserBLL userBLL = new UserBLL();
                var users = userBLL.GetDeptUsers(user.DeptId).ToList();
                for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new JobTemplateEntity();
                    entity.JobId = Guid.NewGuid().ToString();
                    entity.JobContent = sheet.Cells[i, 0].StringValue;
                    if (string.IsNullOrEmpty(entity.JobContent))
                    {

                        if (templates.Count > 0 && string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                        {
                            break;
                        }
                        success = false;
                        message +="第"+(i+1)+ "行，未填写工作任务</br>";
                        continue;
                        //return Json(new { success, message });

                    }
                    var sss = sheet.Cells[i, 1].StringValue;
                    if (!string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                    {
                        var itemdetialbll = new DataItemDetailBLL();
                        var itembll = new DataItemBLL();
                        var type = itembll.GetEntityByName("任务库任务类型");
                        var content = itemdetialbll.GetList(type.ItemId).ToList();
                        var typename = sheet.Cells[i, 1].StringValue.Trim();
                        var gettype = content.FirstOrDefault(row => row.ItemName == typename);
                        if (gettype == null)
                        {
                            success = false;
                            message += "第" + (i + 1) + "行，不存在该类型</br>";
                            continue;
                            //return Json(new { success, message });
                        }
                        entity.jobplantype = gettype.ItemName;
                        entity.jobplantypeid = gettype.ItemId;
                    }
                    else
                    {
                        success = false;
                        message += "第" + (i + 1) + "行，任务类型不能为空</br>";
                        continue;
                        //return Json(new { success, message });
                    }
                    entity.RiskLevel = sheet.Cells[i, 2].StringValue;
                    if (string.IsNullOrEmpty(entity.RiskLevel))
                    {
                        success = false;
                        message += "第" + (i + 1) + "行，未填写风险等级</br>";
                        continue;
                        // return Json(new { success, message });
                    }

                    var jobperson = sheet.Cells[i, 3].StringValue;
                    if (jobperson.Contains(","))
                    {
                        var person = string.Empty;
                        var personid = string.Empty;
                        var personList = jobperson.Split(',');
                        for (int j = 0; j < personList.Length; j++)
                        {
                            var ckjobperson = users.FirstOrDefault(row => row.RealName == personList[j]);
                            if (ckjobperson == null)
                            {
                                success = false;
                                message += "第" + (i + 1) + "行，"+ personList[j] + "作业人错误</br>";
                                continue;
                                //return Json(new { success, message });
                            }
                            if (j >= personList.Length - 1)
                            {
                                person += personList[j];
                                personid += ckjobperson.DepartmentId;
                            }
                            else
                            {
                                person += personList[j] + ",";
                                personid += ckjobperson.DepartmentId + ",";

                            }

                        }

                        entity.JobPerson = person;
                        entity.JobPersonId = personid;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(jobperson))
                        {
                            var ckjobperson = users.FirstOrDefault(row => row.RealName == jobperson);
                            if (ckjobperson == null)
                            {
                                success = false;
                                message += "第" + (i + 1) + "行，"+ jobperson + "作业人错误</br>";
                                continue;
                                // return Json(new { success, message });
                            }
                            entity.JobPerson = jobperson;
                            entity.JobPersonId = ckjobperson == null ? null : ckjobperson.UserId;
                        }

                    }
                    //string otherperson = sheet.Cells[i, 3].StringValue;
                    //if (!string.IsNullOrEmpty(otherperson))
                    //{
                    //    var ckotherperson = users.FirstOrDefault(row => row.RealName == otherperson);
                    //    if (ckotherperson == null)
                    //    {
                    //        success = false;
                    //        message = "选填作业人错误";
                    //        return Json(new { success, message });
                    //    }
                    //}
                    //if (otherperson == jobperson)
                    //{
                    //    success = false;
                    //    message = "作业人不能相同";
                    //    return Json(new { success, message });
                    //}
                    //entity.otherperson = otherperson;
                    entity.Device = sheet.Cells[i, 4].StringValue;
                    var getNow = DateTime.Now.ToString("yyyy-MM-dd");
                    //entity.JobStartTime = sheet.Cells[i, 5].StringValue == "" ? Convert.ToDateTime(getNow + " 08:30") : Convert.ToDateTime(getNow + " " + sheet.Cells[i, 5].StringValue);
                    //entity.JobEndTime = sheet.Cells[i, 6].StringValue == "" ? Convert.ToDateTime(getNow + " 17:30") : Convert.ToDateTime(getNow + " " + sheet.Cells[i, 6].StringValue);
                    entity.JobStartTime = Convert.ToDateTime(getNow + " 08:30");
                    entity.JobEndTime = Convert.ToDateTime(getNow + " 17:30");
                    if (!string.IsNullOrEmpty(sheet.Cells[i, 5].StringValue))
                    {
                        //每年，二月、九月，15日，白班  每月，第一个、第三个，星期五，白班
                        var Cycle = sheet.Cells[i, 5].StringValue.Trim();

                        var cycleType = Cycle.Split('，');
                        if (cycleType[0] != "每天" && cycleType[0] != "每周" && cycleType[0] != "每月" && cycleType[0] != "每年")
                        {
                            success = false;
                            message += "第" + (i + 1) + "行，周期规则错误</br>";
                            continue;
                            // return Json(new { success, message });
                        }

                        entity.Cycle = cycleType[0];
                        var ck = false;
                        var data = string.Empty;
                        for (int j = 1; j < cycleType.Length; j++)
                        {

                            //if (cycleType[j] == "白班" || cycleType[j] == "夜班")
                            //{
                            //    entity.worksetname = cycleType[j];

                            //}
                            //else
                            if (cycleType[j] == "截止")
                            {
                                entity.isend = true;

                            }
                            else
                            if (cycleType[j] == "最后一天")
                            {
                                if ((Cycle.Contains("每月") || Cycle.Contains("每年")) && Cycle.Contains("日"))
                                {
                                    entity.islastday = true;
                                }
                                else
                                {
                                    success = false;
                                    message += "第" + (i + 1) + "行，周期规则错误</br>";
                                    continue;
                                    //return Json(new { success, message });
                                }

                            }
                            else
                            if (cycleType[j].Contains("双休"))
                            {
                                if ((Cycle.Contains("每月") || Cycle.Contains("每年")) && Cycle.Contains("日"))
                                {
                                    entity.isweek = true;
                                }
                                else if (Cycle.Contains("每天"))
                                {
                                    entity.isweek = true;
                                }
                                else
                                {
                                    success = false;
                                    message += "第" + (i + 1) + "行，周期规则错误</br>";
                                    continue;
                                    //return Json(new { success, message });
                                }
                            }
                            else
                            {
                                data += cycleType[j].Replace('日', ' ').Trim().Replace('、', ',') + ";";
                                ck = true;
                            }


                        }

                        if (ck)
                        {
                            data = data.Substring(0, data.Length - 1);
                            entity.CycleDate = data;
                        }
                        else
                        {
                            entity.CycleDate = data;
                        }

                    }
                    else
                    {
                        if (entity.jobplantype != "临时任务")
                        {
                            success = false;
                            message += "第" + (i + 1) + "行，周期不能为空</br>";
                            continue;
                            //return Json(new { success, message });
                        }
                    }
                    entity.Dangerous = sheet.Cells[i, 6].StringValue;
                    entity.Measure = sheet.Cells[i, 7].StringValue;
                    var EnableTraining = sheet.Cells[i, 8].StringValue;
                    entity.EnableTraining = EnableTraining == "是";
                    //entity.EnableTraining = false;
                    entity.worksetname = sheet.Cells[i, 9].StringValue;
                    if (entity.jobplantype == "设备巡回检查")
                        entity.TaskType = "巡回检查";
                    else if (entity.jobplantype == "定期工作")
                        entity.TaskType = "定期工作";
                    else
                        entity.TaskType = "日常工作";
                    var setupid = string.Empty;
                    var createuserid = string.Empty;
                    WorkOrderBLL orderbll = new WorkOrderBLL();
                    orderbll.GetWorkSettingByDept(user.DeptId, out setupid, out createuserid);
                    WorkSettingBLL settingbll = new WorkSettingBLL();
                    var setting = settingbll.GetList("");
                    var getbanci = setting.Where(x => x.WorkSetupId == setupid && x.CreateUserId == createuserid);
                    if (entity.worksetname.Contains(","))
                    {
                        var setname = string.Empty;
                        var setnameid = string.Empty;
                        var setList = entity.worksetname.Split(',');
                        for (int j = 0; j < setList.Length; j++)
                        {
                            var ckset = getbanci.FirstOrDefault(x => x.Name == entity.worksetname);
                            if (ckset == null)
                            {
                                success = false;
                                message += "第" + (i + 1) + "行，不存在该班次" + setList[j]+ "</br>";
                                continue;
                                //return Json(new { success, message });
                            }
                            if (j >= setList.Length - 1)
                            {
                                setname += setList[j];
                                setnameid += ckset.WorkSettingId;
                            }
                            else
                            {
                                setname += setList[j] + ",";
                                setnameid += ckset.WorkSettingId + ",";

                            }

                        }

                        entity.worksetname = setname;
                        entity.worksetid = setnameid;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(entity.worksetname))
                        {
                            if (entity.jobplantype != "临时任务")
                            {
                                success = false;
                                message += "第" + (i + 1) + "行，班次不能为空</br>";
                                continue;
                                //return Json(new { success, message });
                            }
                        }
                        else
                        {
                            var ckset = getbanci.FirstOrDefault(x => x.Name == entity.worksetname);
                            if (ckset == null)
                            {
                                success = false;
                                message += "第" + (i + 1) + "行，不存在该班次" + entity.worksetname+ "</br>";
                                continue;
                                //return Json(new { success, message });
                            }
                            entity.worksetname = ckset.Name;
                            entity.worksetid = ckset.WorkSettingId;
                        }
                    }
                    entity.DeptId = user.DeptId;
                    entity.CreateDate = date.AddMinutes(i);
                    entity.DangerType = "job";
                    templates.Add(entity);
                }
                if (!success)
                {
                    return Json(new { success, message });
                }
                foreach (var item in templates)
                {
                    if (item.DangerousList == null) item.DangerousList = new List<JobDangerousEntity>();
                    if (!string.IsNullOrEmpty(item.Dangerous))
                    {
                        var dangerArray = item.Dangerous.Split('。');
                        var dangerMeasureArray = item.Measure?.Split('。');
                        for (int i = 0; i < dangerArray.Length; i++)
                        {
                            var danger = dangerArray[i];
                            if (string.IsNullOrEmpty(danger))
                            {
                                continue;
                            }
                            var templateDangerousEntity = new JobDangerousEntity { Content = danger };
                            if (dangerMeasureArray != null && dangerMeasureArray.Length > i)
                            {
                                templateDangerousEntity.MeasureList = new List<JobMeasureEntity>();
                                var dangerMeasure = dangerMeasureArray[i];
                                if (!string.IsNullOrEmpty(dangerMeasure))
                                {
                                    var measureArray = dangerMeasure.Split('；');
                                    foreach (var measure in measureArray)
                                    {
                                        if (string.IsNullOrEmpty(measure))
                                        {
                                            continue;
                                        }
                                        templateDangerousEntity.MeasureList.Add(new JobMeasureEntity { Content = measure });
                                    }
                                }
                            }
                            item.DangerousList.Add(templateDangerousEntity);
                        }
                    }
                    foreach (var item1 in item.DangerousList)
                    {
                        if (string.IsNullOrEmpty(item1.JobDangerousId)) item1.JobDangerousId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobId = item.JobId;
                        if (item1.MeasureList == null) item1.MeasureList = new List<JobMeasureEntity>();
                        foreach (var item2 in item1.MeasureList)
                        {
                            if (string.IsNullOrEmpty(item2.JobMeasureId)) item2.JobMeasureId = Guid.NewGuid().ToString();
                            item2.CreateTime = DateTime.Now;
                            item2.JobDangerousId = item1.JobDangerousId;
                        }
                    }

                    item.CreateUserId = user.UserId;
                    item.CreateUser = user.UserName;
                    workmeetingbll.UpdateJobTemplate(item);
                }
                // workmeetingbll.AddJobTemplates(templates);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }

        public JsonResult DoDelete(string id)
        {
            var success = true;
            var message = string.Empty;

            try
            {
                workmeetingbll.DeleteJobTemplate(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }



        public ViewResult Edit(string id)
        {
            var user = OperatorProvider.Provider.Current();
            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            //任务类型
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var type = itembll.GetEntityByName("任务库任务类型");
            var content = itemdetialbll.GetList(type.ItemId).ToList();
            var jobtype = content.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName }).ToList();
            WorkSettingBLL settingbll = new WorkSettingBLL();
            var setting = settingbll.GetList("");
            //var pDeptid = dept.GetParent(deptid,"部门");
            var setupid = string.Empty;
            var createuserid = string.Empty;
            //WorkOrderBLL orderbll = new WorkOrderBLL();
            //var banci = orderbll.getSetValue(user.DeptId).Select(x => new SelectListItem() { Text = x.Value, Value = x.Key }).ToList();
            //orderbll.GetWorkSettingByDept(user.DeptId, out setupid, out createuserid);
            //var getbanci = setting.Where(x => x.WorkSetupId == setupid && x.CreateUserId == createuserid);
            //var banci = getbanci.OrderBy(x => x.StartTime).Select(x => new SelectListItem() { Text = x.Name, Value = x.WorkSettingId }).ToList();
            //ViewData["banci"] = banci;
            ViewData["users"] = users;
            ViewData["jobtype"] = jobtype;
            var job = workmeetingbll.GetJobTemplate(id);
            if (job == null)
            {
                job = new JobTemplateEntity();
                job.JobStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
                job.JobEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 30, 0);
                job.Cycle = "";
            }
            ViewBag.JobTime = string.Format("{0} - {1}", job.JobStartTime.HasValue ? job.JobStartTime.Value.ToString("HH:mm") : string.Empty, job.JobEndTime.HasValue ? job.JobEndTime.Value.ToString("HH:mm") : string.Empty);

            return View(job);
        }

        [HttpPost]
        public ActionResult Edit(string id, JobTemplateEntity model, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var time = fc.Get("JobTime");
            try
            {



                var reg = new Regex("^(\\d{2}):(\\d{2}) - (\\d{2}):(\\d{2})$");
                if (reg.IsMatch(time))
                {
                    var matches = reg.Matches(time);
                    model.JobStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(matches[0].Groups[1].Value), int.Parse(matches[0].Groups[2].Value), 0);
                    model.JobEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(matches[0].Groups[3].Value), int.Parse(matches[0].Groups[4].Value), 0);
                }
                else
                {
                    var nowTime = DateTime.Now.ToString("yyyy-MM-dd");
                    model.JobStartTime = Convert.ToDateTime(nowTime + " 08:30");
                    model.JobEndTime = Convert.ToDateTime(nowTime + " 17:30");
                }

                model.CycleDate = model.CycleDate ?? string.Empty;
                model.JobId = id;
                model.DeptId = user.DeptId;
                model.CreateDate = DateTime.Now;
                model.CreateUser = user.UserName;
                model.CreateUserId = user.UserId;

                if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();

                if (!string.IsNullOrEmpty(model.Dangerous))
                {
                    var dangerArray = model.Dangerous.Split('。');
                    var dangerMeasureArray = model.Measure?.Split('。');
                    for (int i = 0; i < dangerArray.Length; i++)
                    {
                        var danger = dangerArray[i];
                        if (string.IsNullOrEmpty(danger))
                        {
                            continue;
                        }
                        var templateDangerousEntity = new JobDangerousEntity { Content = danger };
                        if (dangerMeasureArray != null && dangerMeasureArray.Length > i)
                        {
                            templateDangerousEntity.MeasureList = new List<JobMeasureEntity>();
                            var dangerMeasure = dangerMeasureArray[i];
                            if (!string.IsNullOrEmpty(dangerMeasure))
                            {
                                var measureArray = dangerMeasure.Split('；');
                                foreach (var measure in measureArray)
                                {
                                    if (string.IsNullOrEmpty(measure))
                                    {
                                        continue;
                                    }
                                    templateDangerousEntity.MeasureList.Add(new JobMeasureEntity { Content = measure });
                                }
                            }
                        }
                        model.DangerousList.Add(templateDangerousEntity);
                    }
                }
                foreach (var item in model.DangerousList)
                {
                    if (string.IsNullOrEmpty(item.JobDangerousId)) item.JobDangerousId = Guid.NewGuid().ToString();
                    item.CreateTime = DateTime.Now;
                    item.JobId = model.JobId;
                    if (item.MeasureList == null) item.MeasureList = new List<JobMeasureEntity>();
                    foreach (var item1 in item.MeasureList)
                    {
                        if (string.IsNullOrEmpty(item1.JobMeasureId)) item1.JobMeasureId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobDangerousId = item.JobDangerousId;
                    }
                }
                //任务类型，填充任务类别
                switch (model.jobplantype)
                {
                    case "设备巡回检查":
                        model.TaskType = "巡回检查";
                        break;
                    case "周期任务":
                        model.TaskType = "定期工作";
                        break;
                    default:
                        model.TaskType = "日常任务";
                        break;
                }
                string r = workmeetingbll.UpdateJobTemplate(model);

                ViewBag.callback = "jQuery(function(){var pp = jQuery(parent).get(0).fn$callback();});";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.callback = "jQuery(function(){ layer.msg(\"" + ex.Message + "\")});";
                return View();

            }
        }


        public ViewResult Cycle()
        {
            var user = OperatorProvider.Provider.Current();
            WorkOrderBLL orderbll = new WorkOrderBLL();
            var banci = orderbll.getSetValue(user.DeptId).Select(x => new SelectListItem() { Text = x.Value, Value = x.Key }).ToList();
            ViewData["banci"] = banci;
            return View();
        }


        }
}
