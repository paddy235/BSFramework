using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.QuestionManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.Log;
using BSFramework.Util.Offices;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// 
    /// </summary>
    public class ActivityController : MvcControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.user = user.UserName;

            var bll = new ActivityBLL();
            var dic = bll.GetIndex(user.DeptId, fc.Get("name"));
            return View(dic);
        }

        public JsonResult PostCategory(ActivityCategoryEntity model)
        {
            var user = OperatorProvider.Provider.Current();

            var bll = new ActivityBLL();
            model.ActivityCategoryId = Guid.NewGuid().ToString();
            model.CreateTime = DateTime.Now;
            model.CreateUserId = OperatorProvider.Provider.Current().UserId;
            model.DeptId = user.DeptId;

            var success = true;
            var message = string.Empty;
            try
            {
                bll.AddCategory(model);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Json(new { success, message });
        }
        //删除

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List(int page, int pagesize, string category, string from, string to, string name, FormCollection fc)
        {
            category = HttpUtility.UrlDecode(category);

            var bll = new ActivityBLL();
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            if (string.IsNullOrEmpty(from)) from = fc.Get("from");
            if (string.IsNullOrEmpty(to)) to = fc.Get("to");

            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["name"] = name;

            var user = OperatorProvider.Provider.Current();

            var total = 0;
            var data = bll.GetList(user.DeptId, string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), name, page, pagesize, category, out total);
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.category = category;
            return View(data);
        }

        public ActionResult Over(ActivityEntity model)
        {
            var bll = new ActivityBLL();
            model.EndTime = DateTime.Now;
            if (model.Remark != null)
                model.Remark = model.Remark.Replace("\r\n", "\n");
            bll.Over(model);
            bll.setToDo(model.ActivityType, model.ActivityId, model.GroupId);
            return RedirectToAction("Index");
        }

        public ViewResult Index3(string type)
        {
            var deptid = OperatorProvider.Provider.Current().DeptId;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(deptid);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.type = type;

            var bll = new ActivityBLL();
            var types = bll.GetActivityCategories(deptid);

            var list = new List<SelectListItem>() { new SelectListItem() { Value = "全部", Text = "全部", Selected = true } };
            list.AddRange(types.Select(x => x.ActivityCategory).Distinct().Select(x => new SelectListItem() { Value = x, Text = x }));
            ViewData["categories"] = list;

            return View();
        }
        public ViewResult Index5(string category)
        {
            var deptid = OperatorProvider.Provider.Current().DeptId;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(deptid);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.category = category;

            //var bll = new ActivityBLL();
            //var types = bll.GetActivityCategories(deptid);

            //var list = new List<SelectListItem>() { new SelectListItem() { Value = "全部", Text = "全部", Selected = true } };
            //list.AddRange(types.Select(x => x.ActivityCategory).Distinct().Select(x => new SelectListItem() { Value = x, Text = x }));
            //ViewData["categories"] = list;

            return View();
        }
        public ViewResult Index4()
        {
            return View();
        }
        public JsonResult GetActivityNew()
        {
            var user = OperatorProvider.Provider.Current();
            if (user.DeptId == "0") user.DeptCode = "0";
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            DepartmentBLL deptBll = new DepartmentBLL();
            var total = 0;
            var data = new List<newActivity>();
            var groups = deptBll.GetAllGroups().Where(x => x.EnCode.Contains(user.DeptCode)); ;

            var bll = new ActivityBLL();
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
            //DateTime nsdt = new DateTime(DateTime.Now.Year, month, 1);

            DateTime dtWeekSt1;
            DateTime dtWeekSt2;
            int w1 = (int)sdt.DayOfWeek;
            int w2 = (int)DateTime.Now.DayOfWeek;
            if (w1 == 0)
            {
                w1 = 7;
            }
            if (w2 == 0)
            {
                w2 = 7;
            }
            dtWeekSt1 = sdt.AddDays(1 - w1);
            dtWeekSt2 = DateTime.Now.AddDays(1 - w2);
            TimeSpan ts = dtWeekSt2 - dtWeekSt1;
            int weeks = (int)ts.Days / 7; //周数 == 应开展数量 ==循环次数
            int n = 0;
            foreach (DepartmentEntity d in groups)
            {
                var dept = deptBll.GetEntity(d.ParentId);
                //sdt = new DateTime(DateTime.Now.Year, month, 1);
                for (int i = 0; i < weeks; i++)
                {
                    n = bll.GetActivityList(d.DepartmentId, sdt.AddDays(i * 7).ToString("yyyy-MM-dd"), sdt.AddDays((i + 1) * 7).ToString("yyyy-MM-dd"));
                    if (n == 0)
                    {
                        data.Add(new newActivity { GroupName = d.FullName, FromTo = sdt.AddDays(i * 7).ToString("yyyy-MM-dd") + "至" + sdt.AddDays((i + 1) * 7).ToString("yyyy-MM-dd"), Remark = "", DeptName = dept == null ? "" : dept.FullName });
                    }
                }
            }
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        public ViewResult Form(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetEntity(id);
            var QuestionList = new List<HistoryQuestionEntity>();
            if (model.ActivityType == "安全日活动")
            {
                QuestionBankBLL question = new QuestionBankBLL();
                var qustionId = id.Split(',');

                foreach (var item in qustionId)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var questionModel = question.GetHistoryDetailbyOutId(item);
                    //foreach (var questionentity in questionModel)
                    //{
                    //    var ckFile = true;

                    //    var filesId = questionentity.fileids.Split(',');
                    //    foreach (var fileid in filesId)
                    //    {
                    //        if (!string.IsNullOrEmpty(fileid))
                    //        {
                    //            var ck = model.Files.Where(x => x.FileId == fileid).ToList();
                    //            if (ck.Count == 0)
                    //            {
                    //                ckFile = false;
                    //                break;
                    //            }

                    //        }
                    //    }
                    //    if (ckFile)
                    //    {
                    //        var FilesId = model.Files.Where(x => !string.IsNullOrEmpty(x.FolderId)).Where(x => questionentity.fileids.Contains(x.FolderId)).ToList();
                    //        questionentity.fileids = string.Join(",", FilesId.Select(x => x.FileId));
                    //        QuestionList.Add(questionentity);
                    //    }

                    //}
                    QuestionList.AddRange(questionModel);
                }

            }
            ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);

            return View(model);
        }
        /// <summary>
        ///活动类别列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityType()
        {
            return View();
        }
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        //[HandlerMonitor(3, "分页查询用户信息!")]
        public ActionResult GetActivityCategoryListJson(Pagination pagination)
        {
            var user = OperatorProvider.Provider.Current();
            var watch = CommonHelper.TimerStart();
            var bll = new ActivityBLL();
            var data = bll.GetCategoryList();
            data = data.OrderByDescending(x => x.CreateTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
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
        /// <summary>
        /// 活动详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult TypeDetail(string keyvalue)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new ActivityCategoryEntity();
            //获取数据
            if (!string.IsNullOrEmpty(keyvalue))
            {
                var bll = new ActivityBLL();
                model = bll.GetCategory(keyvalue);
            }
            else
            {
                model.CreateUser = user.UserName;
                model.CreateUserId = user.UserId;
                model.CreateTime = DateTime.Now;
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult SaveFormType(ActivityCategoryEntity model)
        {
            var bll = new ActivityBLL();

            var success = true;
            var message = "保存成功";

            try
            {
                if (string.IsNullOrEmpty(model.ActivityCategoryId))
                {
                    model.ActivityCategoryId = Guid.NewGuid().ToString();
                    bll.AddCategoryType(model);
                }
                else
                {
                    bll.UpdateActivityCategoryType(model);
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
        [HttpPost]
        public JsonResult delCategory(string id)
        {
            var bll = new ActivityBLL();

            var success = true;
            var message = "操作成功";

            try
            {
                bll.DeleteCategoryType(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
        public ActionResult New(string category, string id)
        {
            category = HttpUtility.UrlDecode(category);

            var user = OperatorProvider.Provider.Current();
            UserBLL userBLL = new UserBLL();
            SafetydayBLL sbll = new SafetydayBLL();
            var users = new PeopleBLL().GetListByDept(user.DeptId).ToList();
            ViewData["users"] = users;

            var bzaqy = users.FirstOrDefault(x => x.Quarters != null && x.Quarters.Contains("安全员"));

            //var model = new ActivityEntity() { ActivityId = Guid.NewGuid().ToString(), PlanStartTime = DateTime.Now, PlanEndTime = DateTime.Now.AddHours(4), ActivityPlace = "班组办公室", ActivityLimited = "4小时", ChairPerson = user.UserName, RecordPerson = bzaqy == null ? string.Empty : bzaqy.RealName, AlertType = "提前15分钟", ActivityType = category, GroupId = user.DeptId, Persons = string.Join(",", users.Select(x => x.RealName)), PersonId = string.Join(",", users.Select(x => x.UserId)) };
            //model.ActivityPersons = users.Select(x => new ActivityPersonEntity() { ActivityId = model.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.UserId, Person = x.RealName, IsSigned = true }).ToList();
            var model = new ActivityEntity();
            var safetyday = sbll.GetIdEntityList(id);
            var strSubject = string.Empty;
            if (safetyday == null)
            {
                model = new ActivityEntity() { ActivityId = Guid.NewGuid().ToString(), PlanStartTime = DateTime.Now, PlanEndTime = DateTime.Now.AddHours(4), ActivityPlace = "班组办公室", ActivityLimited = "4小时", ChairPerson = user.UserName, RecordPerson = bzaqy == null ? string.Empty : bzaqy.Name, AlertType = "提前15分钟", ActivityType = category, GroupId = user.DeptId, Persons = string.Join(",", users.Select(x => x.Name)), PersonId = string.Join(",", users.Select(x => x.ID)) };
            }
            else
            {
                foreach (SafetydayEntity se in safetyday)
                {
                    strSubject += se.Subject + " ";
                    this.SaveRead(se.Id, user.UserId);
                }
                model = new ActivityEntity() { ActivityId = Guid.NewGuid().ToString(), Subject = strSubject, PlanStartTime = DateTime.Now, PlanEndTime = DateTime.Now.AddHours(4), ActivityPlace = "班组办公室", ActivityLimited = "4小时", ChairPerson = user.UserName, RecordPerson = bzaqy == null ? string.Empty : bzaqy.Name, AlertType = "提前15分钟", ActivityType = category, GroupId = user.DeptId, Persons = string.Join(",", users.Select(x => x.Name)), PersonId = string.Join(",", users.Select(x => x.ID)) };
                FileInfoBLL fbll = new FileInfoBLL();
                foreach (SafetydayEntity se in safetyday)
                {
                    var file = fbll.GetFilesByRecIdNew(se.Id).ToList();
                    if (model.Files == null) model.Files = new List<FileInfoEntity>();
                    if (file != null)
                    {
                        foreach (FileInfoEntity fie in file.ToList())
                        {
                            var fileid = Guid.NewGuid().ToString();
                            var newfile = new FileInfoEntity() { FileId = fileid, FolderId = fie.FileId, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = "班组活动", FileExtensions = fie.FileExtensions, FileName = fie.FileName, FilePath = fie.FilePath, FileType = fie.FileType, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = model.ActivityId };
                            newfile.Description = "材料";
                            fbll.SaveForm(newfile);
                            model.Files.Add(newfile);
                        }
                    }
                }
            }
            model.ActivityPersons = users.Select(x => new ActivityPersonEntity() { ActivityId = model.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.ID, Person = x.Name, IsSigned = true }).ToList();
            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.FolderId }));
            var QuestionList = new List<QuestionBankEntity>();
            ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);
            var bll = new ActivityBLL();
            model.PlanEndTime = model.PlanStartTime.AddHours(2);
            model.StartTime = DateTime.Now;
            model.CreateDate = DateTime.Now;
            model.Files = new List<FileInfoEntity>() { this.BuildImage(model.ActivityId) };
            model.State = "Ready";
            model.safetyday = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult New(ActivityEntity model, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);
            var QuestionBank = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryQuestionEntity>>(fc["QuestionList"]);

            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            var filebll = new FileInfoBLL();
            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }
                var file = this.SaveFile(model.ActivityId);
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);

                model.Files.Add(file);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.FolderId }));
                return View(model);
            }

            var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }
                var remove = new List<HistoryQuestionEntity>();
                for (int i = 0; i < QuestionBank.Count; i++)
                {
                    if (QuestionBank[i].fileids.Contains(fileid))
                        remove.Add(QuestionBank[i]);
                }
                if (remove.Count > 0)
                {
                    foreach (var item in remove)
                    {
                        QuestionBank.Remove(item);
                    }

                }
                var filepath = filebll.Delete(fileid);
                //if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                //    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.FolderId }));
                return View(model);
            }

            var bll = new ActivityBLL();
            model.PlanEndTime = model.PlanStartTime.AddHours(2);
            model.StartTime = DateTime.Now;
            model.CreateDate = DateTime.Now;
            model.Files = new List<FileInfoEntity>() { this.BuildImage(model.ActivityId) };
            //if (model.ActivityType == "安全日活动")
            //{
            model.State = "Ready";
            bll.SaveForm(null, model);
            //生成答题台账
            if (QuestionBank.Count > 0)
            {
                DepartmentBLL dept = new DepartmentBLL();
                QuestionBankBLL question = new QuestionBankBLL();
                var list = new List<TheTitleEntity>();
                var operationUser = userBLL.GetEntity(user.UserId);
                var mydept = dept.GetEntity(model.GroupId);
                var i = 0;
                foreach (var item in QuestionBank)
                {
                    item.sort = i;
                    i++;
                }

                foreach (var bank in QuestionBank)
                {
                    bank.Id = null;
                    //bank.titleid = title.Id;
                    bank.outkeyvalue = model.ActivityId;
                }
                question.SaveFormHistoryQuestion(list, QuestionBank, operationUser, "");

            }
            return RedirectToAction("ReadySafetyday", new { id = model.ActivityId });
            //}
            //else
            //{
            //    bll.Start(model);
            //    return RedirectToAction("Study", new { id = model.ActivityId });
            //}
        }

        private FileInfoEntity BuildImage(string activityid)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(activityid + "|班组活动", Encoding.UTF8);
            var path = "~/Resource/DocumentFile/";
            if (!Directory.Exists(Server.MapPath(path)))
                Directory.CreateDirectory(Server.MapPath(path));

            image.Save(Path.Combine(Server.MapPath(path), id + ".jpg"));

            var user = OperatorProvider.Provider.Current();

            return new FileInfoEntity()
            {
                FileId = id,
                CreateDate = DateTime.Now,
                CreateUserId = user.UserId,
                CreateUserName = user.UserName,
                Description = "二维码",
                FileExtensions = ".jpg",
                FileName = id + ".jpg",
                FilePath = path + id + ".jpg",
                FileType = "jpg",
                ModifyDate = DateTime.Now,
                ModifyUserId = user.UserId,
                ModifyUserName = user.UserName,
                RecId = activityid
            };
        }

        public ActionResult Study(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetEntity(id);

            if (model.Files == null) model.Files = new List<FileInfoEntity>();
            foreach (var item in model.Files)
            {
                if (!string.IsNullOrEmpty(item.FilePath))
                    item.FilePath = string.Format("{0}://{1}{2}", this.Request.Url.Scheme, this.Request.Url.Host, Url.Content(item.FilePath));
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Study(ActivityEntity model, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);
            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            var filebll = new FileInfoBLL();
            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }
                var file = this.SaveFile(model.ActivityId);

                model.Files.Add(file);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName }));
                return View(model);
            }

            var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }

                var filepath = filebll.Delete(fileid);
                //if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                //    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();

                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName }));
                return View(model);
            }

            var bll = new ActivityBLL();
            model.PlanEndTime = model.PlanStartTime.AddHours(2);
            model.StartTime = DateTime.Now;
            model.CreateDate = DateTime.Now;
            model.Files = new List<FileInfoEntity>() { this.BuildImage(model.ActivityId) };

            bll.Start(model);
            return RedirectToAction("Study", new { id = model.ActivityId });
        }
        private FileInfoEntity SaveFile(string refid)
        {
            var user = OperatorProvider.Provider.Current();
            var path = "~/Resource/DocumentFile/";

            var id = Guid.NewGuid().ToString();
            var file = this.Request.Files[0];
            var ext = Path.GetExtension(file.FileName);

            file.SaveAs(Path.Combine(Server.MapPath(path), id + ext));

            var model = new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = "班组活动", FileExtensions = ext, FileName = file.FileName, FilePath = path + id + ext, FileType = ext.Replace(".", ""), ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = refid };
            model.Description = "材料";
            FileInfoBLL bll = new FileInfoBLL();
            bll.SaveForm(model);

            return model;
        }

        public ActionResult Edit(string id)
        {
            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            var bll = new ActivityBLL();
            var model = bll.GetEntity(id);
            model.PlanStartTime = DateTime.Now;
            model.PlanEndTime = DateTime.Now.AddHours(4);
            model.StartTime = model.EndTime = DateTime.Now;
            model.ActivityLimited = "4小时";
            model.ActivityPlace = "班组办公室";
            model.ChairPerson = user.UserName;
            model.RecordPerson = user.UserName;
            model.AlertType = "提前15分钟";
            if (model.ActivityPersons == null || model.ActivityPersons.Count == 0)
            {
                model.ActivityPersons = users.Select(x => new ActivityPersonEntity() { ActivityId = model.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.UserId, Person = x.RealName, IsSigned = true }).ToList();
            }
            model.Persons = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned).Select(x => x.Person));
            model.PersonId = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned).Select(x => x.PersonId));

            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, Description = string.Empty }));

            return View(model);
        }

        [HttpPost]
        public ActionResult EditRemark(string id, string remark)
        {
            try
            {
                var bll = new ActivityBLL();
                var model = bll.GetEntity(id);
                model.Remark = remark;
                model.Evaluates = null;
                model.ActivityRecords = null;
                model.Supplys = null;
                model.Files = null;
                model.ActivityPersons = null;
                bll.modfiyEntity(model);

                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Edit(string id, ActivityEntity model, FormCollection fc)
        {
            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);
            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            var filebll = new FileInfoBLL();
            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }
                var file = this.SaveFile(model.ActivityId);
                file.Description = "candelete";

                model.Files.Add(file);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.Description }));
                return View(model);
            }

            var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }

                var filepath = filebll.Delete(fileid);
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();

                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.Description }));
                return View(model);
            }

            if (model.ActivityLimited.Contains("分钟"))
            {
                model.PlanEndTime = model.PlanStartTime.AddMinutes(double.Parse(model.ActivityLimited.Replace("分钟", string.Empty)));
            }
            else if (model.ActivityLimited.Contains("小时"))
            {
                model.PlanEndTime = model.PlanStartTime.AddHours(double.Parse(model.ActivityLimited.Replace("小时", string.Empty)));
            }
            else
            {
                model.PlanEndTime = model.PlanStartTime.AddHours(2);
            }

            model.StartTime = DateTime.Now;
            model.Files.Add(this.BuildImage(model.ActivityId));
            model.CreateDate = DateTime.Now;
            var bll = new ActivityBLL();

            bll.SaveForm(id, model);
            return RedirectToAction("Ready", new { id = model.ActivityId });
        }
        /// <summary>
        /// 加载班组活动类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult editCategory(string id)
        {
            var abll = new ActivityBLL();
            ActivityCategoryEntity model = abll.GetCategoryEntity(id);
            return Success("修改成功！", new { name = model.ActivityCategory, id = id });
        }
        /// <summary>
        /// 加载班组活动类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ActivityCategory"></param>
        /// <returns></returns>

        public ActionResult EditCategoryType(string id, string ActivityCategory, string ActivityCategoryId)
        {
            var success = "false";
            try
            {
                var abll = new ActivityBLL();
                var user = OperatorProvider.Provider.Current();
                ActivityCategoryEntity t = new ActivityCategoryEntity();
                //t= abll.GetCategoryEntity(id);
                t.ActivityCategoryId = id;
                t.ActivityCategory = ActivityCategory;
                t.CreateUserId = OperatorProvider.Provider.Current().UserId;
                //t.DeptId = user.DeptId;
                t.CreateTime = DateTime.Now;
                t.DeptId = user.DeptId;
                //判断是否为系统班组类型
                var model = abll.GetCategoryEntity(id);
                if (string.IsNullOrEmpty(model.DeptId) || model.DeptId == "")
                {

                }
                else
                {
                    abll.SaveFormCategory(id, t);
                    success = "true";
                }
            }
            catch (Exception ex)
            {
                success = HttpUtility.JavaScriptStringEncode(ex.Message);
                return Error(success);
            }
            return Success(success);
        }
        /// <summary>
        /// 删除班组活动类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Del(string id)
        {
            var success = true;
            var message = string.Empty;

            try
            {
                var abll = new ActivityBLL();
                var user = OperatorProvider.Provider.Current();
                //判断是否为系统班组类型
                var model = abll.GetCategoryEntity(id);
                if (!string.IsNullOrEmpty(model.deptname))
                {
                    throw new Exception("该活动类型为管理平台添加，请在管理平台操作！");
                }
                //查询是否存在班组活动
                var Index = abll.GetIndexEntity(model.ActivityCategory);
                if (string.IsNullOrEmpty(model.DeptId) || model.DeptId == "")
                {
                    success = false;
                    message = "系统班组活动类型,不允许删除！";
                }
                else if (Index > 0)
                {
                    success = false;
                    message = "该类型已经存在班组活动,不允许删除！";
                }
                else
                {
                    abll.Del(id);
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }
        public ActionResult Ready(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetEntity(id);

            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            if (model.ActivityPersons == null || model.ActivityPersons.Count == 0)
            {
                model.ActivityPersons = users.Select(x => new ActivityPersonEntity() { ActivityId = model.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.UserId, Person = x.RealName, IsSigned = true }).ToList();
            }
            model.Persons = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned).Select(x => x.Person));
            model.PersonId = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned).Select(x => x.PersonId));
            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, Description = string.Empty }));
            var QuestionList = new List<HistoryQuestionEntity>();
            if (model.ActivityType == "安全日活动")
            {
                QuestionBankBLL question = new QuestionBankBLL();
                QuestionList = question.GetHistoryDetailbyActivityId(id);
            }
            ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ready(string id, ActivityEntity model, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            QuestionBankBLL question = new QuestionBankBLL();
            var QuestionBank = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryQuestionEntity>>(fc["QuestionList"]);
            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);
            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            var filebll = new FileInfoBLL();
            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }
                var file = this.SaveFile(model.ActivityId);
                file.Description = "candelete";

                model.Files.Add(file);
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, x.Description }));
                return View(model);
            }

            var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }

                var filepath = filebll.Delete(fileid);
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();
                var remove = new List<HistoryQuestionEntity>();
                for (int i = 0; i < QuestionBank.Count; i++)
                {
                    if (QuestionBank[i].fileids.Contains(fileid))
                        remove.Add(QuestionBank[i]);
                }
                if (remove.Count > 0)
                {
                    foreach (var item in remove)
                    {
                        QuestionBank.Remove(item);
                    }

                }
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, x.Description }));
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                return View(model);
            }

            if (model.ActivityLimited.Contains("分钟"))
            {
                model.PlanEndTime = model.PlanStartTime.AddMinutes(double.Parse(model.ActivityLimited.Replace("分钟", string.Empty)));
            }
            else if (model.ActivityLimited.Contains("小时"))
            {
                model.PlanEndTime = model.PlanStartTime.AddHours(double.Parse(model.ActivityLimited.Replace("小时", string.Empty)));
            }
            else
            {
                model.PlanEndTime = model.PlanStartTime.AddHours(2);
            }
            var bll = new ActivityBLL();
            if (fc.Get("issave") == "true")
            {
                
                bll.Edit(model);
                UserBLL userBLL = new UserBLL();
                var list = new List<TheTitleEntity>();

                var operationUser = userBLL.GetEntity(user.UserId);
                //生成答题台账
                if (QuestionBank.Count > 0)
                {

                    DepartmentBLL dept = new DepartmentBLL();
                    var mydept = dept.GetEntity(model.GroupId);
                   
                    var i = 0;
                    foreach (var item in QuestionBank)
                    {
                        item.sort = i;
                        i++;
                    }
                    //QuestionBank = QuestionBank.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                    foreach (var bank in QuestionBank)
                    {
                        //bank.titleid = title.Id;
                        bank.outkeyvalue = model.ActivityId;
                       
                    }
                }
                var questionStr = fc.Get("qdelete");
                question.SaveFormHistoryQuestion(list, QuestionBank, operationUser, questionStr);
                QuestionBank = question.GetHistoryDetailbyActivityId(id);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, x.Description }));
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                return RedirectToAction("ReadySafetyday", new { id = model.ActivityId });
            }
            model.StartTime = DateTime.Now;
            model.Files.Add(this.BuildImage(model.ActivityId));
            //生成答题台账
            if (QuestionBank.Count > 0)
            {
                UserBLL userBLL = new UserBLL();
                DepartmentBLL dept = new DepartmentBLL();

                var list = new List<TheTitleEntity>();
                var operationUser = userBLL.GetEntity(user.UserId);
                var mydept = dept.GetEntity(model.GroupId);
                var i = 0;
                foreach (var item in QuestionBank)
                {
                    item.sort = i;
                    i++;
                }
                foreach (var item in model.ActivityPersons)
                {
                    var title = new TheTitleEntity();
                    title.Id = Guid.NewGuid().ToString();
                    title.startTime = DateTime.Now;
                    title.iscomplete = false;
                    title.category = model.ActivityType;
                    title.activityid = model.ActivityId;
                    var workuser = userBLL.GetEntity(item.PersonId);
                    title.userid = workuser.UserId;
                    title.username = workuser.RealName;
                    title.deptcode = mydept.EnCode;
                    title.deptid = mydept.DepartmentId;
                    title.deptname = mydept.FullName;
                    title.endTime = null;
                    title.score = "0";
                    list.Add(title);
                }
                //QuestionBank = QuestionBank.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                foreach (var bank in QuestionBank)
                {
                    //bank.titleid = title.Id;
                    bank.outkeyvalue = model.ActivityId;
                }
                question.SaveFormHistoryQuestion(list, QuestionBank, operationUser, "");

            }

            bll.Ready(model);
            return RedirectToAction("Study", new { id = model.ActivityId });
        }

        public ActionResult Go(string id, string category)
        {
            category = HttpUtility.UrlDecode(category);

            var bll = new ActivityBLL();
            var user = OperatorProvider.Provider.Current();

            if (string.IsNullOrEmpty(id))
            {

                var data = bll.GetActivities(category, user.DeptId);
                if (category != "安全日活动")
                {
                    if (data.State == "Prepare")
                        return RedirectToAction("Edit", new { id = data.ActivityId });
                    else if (data.State == "Ready")
                        return RedirectToAction("Ready", new { id = data.ActivityId });
                    else if (data.State == "Study")
                        return RedirectToAction("Study", new { id = data.ActivityId });
                }
                else
                {
                    if (data.State == "Ready")
                        return RedirectToAction("ReadySafetyday", new { id = data.ActivityId });
                    else if (data.State == "Study")
                        return RedirectToAction("Study", new { id = data.ActivityId });
                }
            }

            var model = bll.GetEntity(id);

            if (model.Files == null) model.Files = new List<FileInfoEntity>();
            foreach (var item in model.Files)
            {
                item.FilePath = string.Format("{0}://{1}{2}", this.Request.Url.Scheme, this.Request.Url.Host, Url.Content(item.FilePath));
            }
            return View(model);
        }

        public ActionResult Detail(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetEntity(id);
            model.Evaluates = bll.GetEntityList().Where(x => x.Activityid == id).OrderByDescending(x => x.CREATEDATE).ToList();
            var files = model.Files.Where(x => x.Description == "活动记录材料").ToList();
            foreach (var item in files)
            {
                model.Files.Remove(item);
            }

            ViewData["files"] = JsonConvert.SerializeObject(files);
            return View(model);
        }
        public JsonResult ImportEditPush(string keyvalue)
        {
            var success = true;
            var message = string.Empty;
            try
            {
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                string newFilePath = "";
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                var file = this.Request.Files[0];
                string fileName = System.IO.Path.GetFileName(file.FileName);
                //if (!fileName.Contains(".pdf"))
                //{
                //    throw new Exception("请检查文件格式！");
                //}

                //var fileListold = fileInfoBLL.GetFilesByRecIdNew(keyvalue);
                //for (int i = 0; i < fileListold.Count; i++)
                //{
                //    fileInfoBLL.DeleteFile(fileListold[i].RecId, fileListold[i].FileName, fileListold[i].FilePath);
                //}
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = file.ContentLength;
                string FileEextension = Path.GetExtension(fileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string dir = string.Format("~/Resource/{0}/{1}", "AppFile", "Activies", uploadDate);
                string newFileName = fileGuid + FileEextension;
                newFilePath = dir + "/" + newFileName;
                var ck = fileInfoBLL.GetEntity(keyvalue, fileName);
                if (ck != null && ck.Description == "活动记录材料")
                {
                    success = false;
                    message = "已经存在该文件";
                    return Json(new { success, message });
                }
                if (!Directory.Exists(Server.MapPath(dir)))
                {
                    Directory.CreateDirectory(Server.MapPath(dir));
                }

                FileInfoEntity fileInfoEntity = new FileInfoEntity();
                if (!System.IO.File.Exists(Server.MapPath(newFilePath)))
                {
                    //保存文件
                    file.SaveAs(Server.MapPath(newFilePath));
                    //文件信息写入数据库
                    fileInfoEntity.Create();
                    fileInfoEntity.FolderId = "activity";
                    fileInfoEntity.FileId = fileGuid;
                    fileInfoEntity.RecId = keyvalue;
                    fileInfoEntity.FileName = fileName;
                    fileInfoEntity.FilePath = dir + "/" + newFileName;
                    fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                    fileInfoEntity.FileExtensions = FileEextension;
                    fileInfoEntity.FileType = FileEextension.TrimStart('.');
                    fileInfoEntity.Description = "活动记录材料";
                    fileInfoBLL.SaveForm("", fileInfoEntity);
                }
                message = JsonConvert.SerializeObject(fileInfoEntity);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });

        }
        /// <summary>
        /// 编辑视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail2(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetDetail(id);
            if (model != null)
            {
                ViewBag.time = model.StartTime.ToString("yyyy/MM/dd HH:mm") + " ~ " + model.EndTime.ToString("HH:mm");
                ViewBag.person1 = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned == true).Select(x => x.Person));
                ViewBag.person2 = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned == false).Select(x => x.Person));

                model.PersonId = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned == true).Select(x => x.PersonId));
                model.Persons = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned == true).Select(x => x.Person));
            }
            return View(model);
        }

        /// <summary>
        /// 查看视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail3(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetDetail(id);
            if (model != null)
            {
                ViewBag.time = model.StartTime.ToString("yyyy/MM/dd HH:mm") + " ~ " + model.EndTime.ToString("HH:mm");
                ViewBag.person1 = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned == true).Select(x => x.Person));
                ViewBag.person2 = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned == false).Select(x => x.Person));
            }
            return View(model);
        }

        public JsonResult DeleteFile(string fileid)
        {
            var filebll = new FileInfoBLL();
            var filepath = filebll.Delete(fileid);
            if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                System.IO.File.Delete(Server.MapPath(filepath));

            return Json(new { success = true, msg = string.Empty });
        }


        public ActionResult Index2()
        {
            return View();
        }

        public JsonResult GetData()
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new ActivityBLL();

            var data = bll.GetData(user.DeptId);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson, string type, string category, string deptid, string select, string isEvaluate)
        {
            var user = OperatorProvider.Provider.Current();
            category = HttpUtility.UrlDecode(category);

            //pagination.p_kid = "activityid";
            //pagination.p_fields = "activityid,(select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='" + user.UserId + "')>0 as PjCount, activitytype,subject,planstarttime,planendtime,starttime,endtime,activityplace,state,a.fullname, b.fullname as deptname,case WHEN c.number is NULL then 0 else c.number end number,groupid, a.encode";
            //pagination.p_tablename = "wg_activity  left join base_department a on groupid=a.departmentid left join base_department b on b.departmentid = a.parentid left join (SELECT COUNT( 1 ) AS number, a1.activityid AS id FROM wg_activityevaluate a2 inner join wg_activity a1 on a1.activityid = a2.activityid GROUP BY a1.activityid) c on c.id=activityid ";
            //pagination.conditionJson = "1=1";

            var acStr = "select  activityid,activitytype,subject,planstarttime,planendtime,starttime,endtime,activityplace,state,groupid from wg_activity where 1=1 ";
            var dpStr = "select a.fullname,b.fullname as deptname,a.DEPARTMENTID,a.encode from  base_department a LEFT JOIN base_department b  on b.departmentid = a.parentid where 1=1";
            var el = string.Format("select count(activityid) as number,activityid as id from wg_activityevaluate where evaluateid = '{0}' GROUP BY activityid ", user.UserId);
            if (type == "4")
            {
                // if (string.IsNullOrEmpty(queryJson)) //queryJson包含时间查询，所以只再首次加载时，筛选本月份
                //{

                int month = 1;

                if (DateTime.Now.Month < 4) month = 1;
                else if (DateTime.Now.Month < 7) month = 4;
                else if (DateTime.Now.Month < 10) month = 7;
                else if (DateTime.Now.Month <= 12) month = 10;
                string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString();  //当前季度开始日期
                string edt = DateTime.Now.ToString();
                //pagination.conditionJson += string.Format(" and starttime >= '{0}' and starttime < '{1}'", sdt, edt);
                acStr += string.Format(" and starttime >= '{0}' and starttime < '{1}'", sdt, edt);
                // }
            }

            if (!string.IsNullOrEmpty(select)) deptid = select;

            if (!string.IsNullOrEmpty(deptid))
            {
                var dept = new DepartmentBLL().GetEntity(deptid);
                if (dept == null) dept = new DepartmentBLL().GetRootDepartment();
                //pagination.conditionJson += string.Format(" and a.encode like '{0}%'", dept.EnCode);
                var depts = new DepartmentBLL().GetSubDepartments(deptid, null);
                //pagination.conditionJson += string.Format(" and groupid in ('{0}')", string.Join("','", depts.Select(x => x.DepartmentId)));
                acStr += string.Format(" and groupid in ('{0}')", string.Join("','", depts.Select(x => x.DepartmentId)));
                dpStr += string.Format(" and a.departmentid in ('{0}')", string.Join("','", depts.Select(x => x.DepartmentId)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                var categories = new string[] { "上级精神宣贯", "安全日活动", "政治学习", "班务会", "民主管理会", "节能记录", "制度学习", "劳动保护监查" };
                if (categories.Contains(category))
                {
                    //pagination.conditionJson += " and activitytype = '" + category + "'";
                    acStr += " and activitytype = '" + category + "'";
                }
                else
                {
                    // pagination.conditionJson += " and activitytype not in ('" + string.Join("','", categories) + "')";
                    acStr += " and activitytype not in ('" + string.Join("','", categories) + "')";
                }
            }

            var tableStr = string.Empty;
            if (isEvaluate == "本人已评价")
            {

                //pagination.conditionJson += " and number > 0";
                // pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='{0}')>0 ", user.UserId);
                el += string.Format(" where evaluateid='{0}' GROUP BY a1.activityid ", user.UserId);

            }
            else if (isEvaluate == "本人未评价")
            {
                //pagination.conditionJson += " and (number = 0 or number is null)";
                // pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='{0}')=0 ", user.UserId);\
                el += " GROUP BY a1.activityid";


            }

            tableStr = string.Format("(select activityid,(select count(1) from wg_activityevaluate g where c.id = g.activityid and g.evaluateid = '{3}') > 0 as PjCount," +
           "activitytype, subject, planstarttime, planendtime, starttime, endtime, activityplace, state," +
           "fullname, deptname,case WHEN c.number is NULL then 0 else c.number end number,groupid, encode " +
           "from ({0}) x left join ({1}) y on x.groupid=y.DEPARTMENTID left join ({2}) c on c.id=x.activityid ) dd ", acStr, dpStr, el, user.UserId);
            // string TbNew = string.Format("(select {0} from {1} where {2}) dd ", pagination.p_fields, pagination.p_tablename, pagination.conditionJson);
            pagination.p_tablename = tableStr;
            pagination.p_fields = " activityid,PjCount, starttime,endtime,activityplace,state, activitytype,subject,planstarttime,planendtime,fullname,deptname,number ";
            pagination.conditionJson = "1=1";

            if (isEvaluate == "本人已评价")
            {
                pagination.conditionJson += " and number> 0";
            }
            else if (isEvaluate == "本人未评价")
            {
                pagination.conditionJson += " and number= 0";
            }
            var watch = CommonHelper.TimerStart();

            ActivityBLL actBll = new ActivityBLL();
            DepartmentBLL deptBll = new DepartmentBLL();
            var data = actBll.GetPageList(pagination, queryJson);
            //foreach (DataRow dr in data.Rows)
            //{
            //    if (int.Parse(dr["number"].ToString()) == 0)
            //    {
            //        dr["number"] = "未评价";
            //    }
            //    else
            //    {
            //        dr["number"] = "已评价";
            //    }
            //}
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

        /// <summary>
        /// 分页查询活动列表
        /// </summary>
        /// <param name="pagination">分页数据</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="deptid">查询部门的Id</param>
        /// <param name="category">活动分类</param>
        /// <param name="select">组织机构</param>
        /// <param name="isEvaluate">是否本人已评</param>
        /// <returns></returns>
        public ActionResult GetActivityPageList(Pagination pagination, string queryJson, string deptid, string category, string select, string isEvaluate)
        {
            var watch = CommonHelper.TimerStart();
            var user = OperatorProvider.Provider.Current();
            pagination.p_fields = @" a.activityid, starttime,endtime,activityplace,state, activitytype,subject,planstarttime,planendtime,fullname,b.fullname as deptname,b.ENCODE ,
	 case c.isEvaluate WHEN c.isEvaluate>0 THEN 1
				 ELSE 0
				 END
				 as number";
            pagination.p_tablename = $@"wg_activity a left join base_department b on a.groupid=b.DEPARTMENTID
				 left join (select  activityid, COUNT(1)  isEvaluate
				 from  wg_activityevaluate where evaluateid = '{user.UserId}' group by activityid) c on a.activityid=c.activityid";
            pagination.conditionJson = "1=1";


            if (!string.IsNullOrEmpty(select)) deptid = select;
            if (!string.IsNullOrEmpty(deptid))
            {
                var dept = new DepartmentBLL().GetEntity(deptid);
                if (dept == null) dept = new DepartmentBLL().GetRootDepartment();
                pagination.conditionJson += $" AND encode LIKE '{dept.EnCode}%'";
            }

            if (!string.IsNullOrEmpty(category))
            {
                var categories = new string[] { "上级精神宣贯", "安全日活动", "政治学习", "班务会", "民主管理会", "节能记录", "制度学习", "劳动保护监查" };
                if (categories.Contains(category))
                {
                    pagination.conditionJson += " and activitytype = '" + category + "'";
                }
                else
                {
                    pagination.conditionJson += " and activitytype not in ('" + string.Join("','", categories) + "')";
                }
            }

            var tableStr = string.Empty;
            if (isEvaluate == "本人已评价")
            {
                pagination.conditionJson += " and number=1";

            }
            else if (isEvaluate == "本人未评价")
            {
                pagination.conditionJson += " and number=0";
            }

            ActivityBLL actBll = new ActivityBLL();
            var data = actBll.GetPageList(pagination, queryJson);
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
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveForm(string keyValue)
        {
            ActivityBLL actBll = new ActivityBLL();
            FileInfoBLL fileBll = new FileInfoBLL();
            var bll = new ActivityBLL();
            var model = bll.GetDetail(keyValue);
            foreach (var item in model.Files)
            {//删除附件
                fileBll.Delete(item.FileId);
            }
            actBll.RemoveForm(keyValue);
            return Success("删除成功。");
        }

        ///// <summary>
        ///// 保存角色表单（新增、修改）
        ///// </summary>
        ///// <param name="keyValue">主键值</param>
        ///// <param name="entity">角色实体</param>
        ///// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AjaxOnly]
        //[HandlerMonitor(5, "保存(新增或修改)信息")]
        //public ActionResult SaveForm(string keyValue, ActivityEntity entity)
        //{
        //    ActivityBLL actBll = new ActivityBLL();
        //    actBll.SaveForm(keyValue, entity);
        //    return Success("操作成功。");
        //}
        ///// <summary>
        ///// 获取信息 
        ///// </summary>
        ///// <param name="keyValue">主键值</param>
        ///// <returns>返回对象Json</returns>
        //[HttpGet]
        //public ActionResult GetFormJson(string keyValue)
        //{
        //    ActivityBLL actBll = new ActivityBLL();
        //    var data = actBll.GetEntity(keyValue);
        //    return Content(data.ToJson("yyyy-MM-dd HH:mm"));
        //}
        #endregion
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Evaluate(string id, string category)
        {
            ViewBag.userName = OperatorProvider.Provider.Current().UserName;
            ViewBag.userId = OperatorProvider.Provider.Current().UserId;
            ViewBag.Id = id;
            ViewBag.category = category;
            return View();
        }
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveEvaluate(string id, string category, ActivityEvaluateEntity model)
        {
            var success = false;
            var bll = new ActivityBLL();
            var userName = OperatorProvider.Provider.Current().UserName;
            var userId = OperatorProvider.Provider.Current().UserId;
            var deptname = OperatorProvider.Provider.Current().DeptName;
            var deptid = OperatorProvider.Provider.Current().DeptId;
            if (model.Activityid.Contains(","))
            {
                var ids = model.Activityid.Split(',');
                var entitylist = new List<ActivityEvaluateEntity>();
                foreach (var item in ids)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var entity = new ActivityEvaluateEntity();
                        entity.ActivityEvaluateId = Guid.NewGuid().ToString();
                        entity.EvaluateDate = DateTime.Now;
                        entity.EvaluateId = userId;
                        entity.EvaluateUser = userName;
                        entity.EvaluateDeptId = deptid;
                        if (model.EvaluateContent == null)
                        {
                            entity.EvaluateContent = "";
                        }
                        else
                        {
                            entity.EvaluateContent = model.EvaluateContent;
                        }
                        entity.DeptName = deptname;
                        entity.CREATEDATE = DateTime.Now;
                        entity.CREATEUSERID = userId;
                        entity.CREATEUSERNAME = userName;
                        entity.Nature = model.Nature;
                        entity.Score = model.Score;
                        entity.Activityid = item;
                        entitylist.Add(entity);
                    }
                }
                bll.SaveEvaluate(entitylist);
            }
            else
            {
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
            }
            return Success("评价成功", success);
        }
        /// <summary>
        /// 展示评价
        /// </summary>
        /// <returns></returns>
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
        public JsonResult GetActivitySupply(string keyValue, int rows)
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;

            var user = OperatorProvider.Provider.Current();
            var bll = new ActivityBLL();
            //var data = bll.GetEvaluationsManoeuvre(name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
            var data = bll.GetSupplyById().Where(x => x.ActivityId == keyValue);
            total = data.Count();
            foreach (ActivitySupplyEntity a in data)
            {
                var peoples = bll.GetPeopleById().Where(x => x.SupplyId == a.ID);
                string username = "";
                foreach (SupplyPeopleEntity s in peoples)
                {
                    username += s.UserName + ",";
                }
                if (username.Length > 0)
                {
                    username = username.Substring(0, username.Length - 1);
                }
                a.UserName = username;
            }
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存活动
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        /// <returns></returns>
        public ActionResult SaveForm(string keyValue, ActivityEntity entity)
        {
            ActivityBLL abll = new ActivityBLL();
            abll.Update(keyValue, entity);
            return Success("操作成功。");
        }

        /// <summary>
        /// 后台保存活动
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        /// <returns></returns>
        public ActionResult SaveManagerForm(string keyValue, ActivityEntity entity)
        {
            ActivityBLL abll = new ActivityBLL();
            abll.ManagerUpdate(keyValue, entity);
            return Success("操作成功。");
        }




        public int GetIsEvaluate(string keyValue, string category)
        {

            ActivityBLL actBll = new ActivityBLL();
            var userId = OperatorProvider.Provider.Current().UserId;
            if (keyValue.Contains(","))
            {
                return 0;
            }

            string strSql = "SELECT activitytype as edutype ,(case when state= 'Finish' then '1' else '0' end) as state,createdate  from wg_activity  where activityid='{0}' "
                     + " union all select activitytype as edutype,(case when state= 'Finish' then '1' else '0' end) as state ,createdate  from wg_edactivity where activityid='{0}' "
                     + " union all select edutype as typeinfo ,flow as state,activitydate as createdate  from wg_edubaseinfo where  id='{0}' union all "
                     + " select '班前班后会' as  typeinfo, (case when isover then '1' else '0' end) as state,meetingstarttime as createdate  from wg_workmeeting where  meetingid='{0}' ";
            string toSql = string.Format(strSql, keyValue);
            var data = actBll.getIsModuleData(toSql);
            if (data.Rows.Count > 0)
            {
                category = data.Rows[0][0].ToString();
                var state = data.Rows[0][1].ToString();
                //未完成
                if (state == "0")
                {
                    return 1;
                }
                var times = Convert.ToDateTime(data.Rows[0][2].ToString());
                //是否存在  设置模块
                var categoryck = actBll.getEvaluateSetBymodule(category);
                //不存在设置
                if (categoryck.Count == 0)
                {
                    return 0;
                }
                //评价数据是否在设置之前
                foreach (var item in categoryck.OrderByDescending(x => x.createdate))
                {
                    if (item.createdate > times)
                    {
                        return 0;
                    }
                }
                var userck = actBll.AcWorkToDo(userId, keyValue).Count() > 0;
                return userck ? 0 : 1;
            }
            else
            {
                return 0;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Statistics()
        {
            var user = OperatorProvider.Provider.Current();
            DepartmentBLL dbll = new DepartmentBLL();
            //if (user.DeptId != "0")
            //{
            //    ViewBag.deptID = user.DeptId;
            //    ViewBag.deptName = dbll.GetEntity(user.DeptId).FullName;
            //}
            //else
            //{
            //    var dept = dbll.GetList().Where(x => x.ParentId == "0" && x.Nature == "厂级").FirstOrDefault();
            //    if (dept != null)
            //    {
            //        ViewBag.deptID = dept.DepartmentId;
            //        ViewBag.deptName = dept.FullName;
            //    }
            //}
            var dept = dbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptID = dept.DepartmentId;
            ViewBag.deptName = dept.FullName;
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StrTime"></param>
        /// <param name="ctype"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStatisticsChart(string StrTime, string ctype, string deptid)
        {
            #region
            //存放
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            StringBuilder Str = new StringBuilder();
            var user = OperatorProvider.Provider.Current();//用户 user.Account
            Str.Append(user.Account + "^");
            #endregion
            var abll = new ActivityBLL();
            var activityList = abll.GetActivityList(deptid, StrTime);
            float safety = activityList.Where(x => x.ActivityType == "安全日活动").ToList().Count();
            float democratic = activityList.Where(x => x.ActivityType == "民主生活会").ToList().Count();
            float politics = activityList.Where(x => x.ActivityType == "政治学习").ToList().Count();
            float team = activityList.Where(x => x.ActivityType == "班务会").ToList().Count();
            var elseactivity = activityList.Where(x => x.ActivityType != "安全日活动" && x.ActivityType != "民主生活会" && x.ActivityType != "政治学习" && x.ActivityType != "班务会").Count();
            string content = "[{ name: '安全日活动', value: " + safety + ", }, { name: '民主生活会', value: " + democratic + " }, { name: '政治学习', value: " + politics + " }, { name: '班务会', value: " + team + " }, { name: '其他活动', value: " + elseactivity + " }]";
            var dataList = content.ToList<GetStatisticsChartJsonEntity>();
            if (content.Count() > 0)
            {

                string[] nameList = new string[dataList.Count()];
                float sum = 0;
                for (int i = 0; i < dataList.Count(); i++)
                {
                    nameList[i] = dataList[i].name;
                    sum = sum + dataList[i].value;
                }
                var dataNew = new List<GetStatisticsChartJsonEntity>();
                for (int i = 0; i < 5; i++)
                {
                    var name = "";
                    if (i == 0)
                        name = "安全日活动";
                    if (i == 1)
                        name = "民主生活会";
                    if (i == 2)
                        name = "政治学习";
                    if (i == 3)
                        name = "班务会";
                    if (i == 4)
                        name = "其他活动";
                    var check = dataList.FirstOrDefault(row => row.name == name);
                    if (check == null)
                    {
                        GetStatisticsChartJsonEntity one = new GetStatisticsChartJsonEntity();
                        one.name = name;
                        one.value = 0;
                        dataNew.Add(one);
                    }
                    else
                    {
                        dataNew.Add(check);
                    }
                }
                List<GetStatisticsChartJsonEntity> arr = new List<GetStatisticsChartJsonEntity>();
                List<GetStatisticsChartSumJsonEntity> sumJson = new List<GetStatisticsChartSumJsonEntity>();
                foreach (var item in dataNew)
                {
                    GetStatisticsChartJsonEntity one = new GetStatisticsChartJsonEntity();
                    one.name = item.name;
                    one.value = (item.value / sum) * 100;
                    arr.Add(one);
                    GetStatisticsChartSumJsonEntity onesum = new GetStatisticsChartSumJsonEntity();
                    onesum.name = item.name;
                    onesum.value = (int)item.value;
                    sumJson.Add(onesum);

                }
                //GetHtLevelChartSumJsonEntity osum = new GetHtLevelChartSumJsonEntity();
                //osum.name = "合计";
                //osum.value = (int)sum;
                //sumJson.Add(osum);
                GetStatisticsChartReturnEntity returnRsult = new GetStatisticsChartReturnEntity();
                returnRsult.sumJson = sumJson;
                returnRsult.data = nameList;
                returnRsult.arr = arr;
                var context = BSFramework.Util.Json.ToJson(returnRsult);
                return Content(context);
            }
            else
            {
                GetStatisticsChartReturnEntity returnRsult = new GetStatisticsChartReturnEntity();
                var context = BSFramework.Util.Json.ToJson(returnRsult);
                return Content(context);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="StrTime"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetDataStatistics(string keyValue, string StrTime, int rows, int page)
        {
            var StartTime = this.Request.QueryString.Get("SumTime");
            var deptid = this.Request.QueryString.Get("deptid");
            if (string.IsNullOrEmpty(StartTime))
                StartTime = DateTime.Now.ToString();
            if (string.IsNullOrEmpty(deptid))
                deptid = OperatorProvider.Provider.Current().DeptId;
            //var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            //var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;

            var user = OperatorProvider.Provider.Current();
            var bll = new ActivityBLL();
            //var data = bll.GetEvaluationsManoeuvre(name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
            var data = bll.GetActivityStatisticsEntity(deptid, rows, page, StartTime, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DoImport(string refid)
        {
            var success = true;
            var message = string.Empty;
            var filename = string.Empty;
            var workmeetingbll = new WorkmeetingBLL();
            var filebll = new FileInfoBLL();
            FileInfoEntity fie = new FileInfoEntity();
            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");

                var user = OperatorProvider.Provider.Current();
                var path = "~/Resource/DocumentFile/";

                var id = Guid.NewGuid().ToString();
                var file = this.Request.Files[0];
                var ext = Path.GetExtension(file.FileName);

                file.SaveAs(Path.Combine(Server.MapPath(path), id + ext));

                fie = new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = "班组活动", FileExtensions = ext, FileName = file.FileName, FilePath = path + id + ext, FileType = ext.Replace(".", ""), ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = refid };
                fie.Description = "材料";
                FileInfoBLL bll = new FileInfoBLL();
                bll.SaveForm(fie);

            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, filename, fie });
        }
        [HttpPost]
        public JsonResult RemoveFile(string recId)
        {
            var success = true;
            var message = "";
            FileInfoBLL fbll = new FileInfoBLL();
            var file = fbll.GetEntity(recId);
            fbll.Delete(file.FileId);
            return Json(new { success, message });

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="category"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ActionResult ListSafetyday(string id, int page, string category, int pagesize)
        {
            category = HttpUtility.UrlDecode(category);

            SafetydayBLL sbll = new SafetydayBLL();
            var bll = new ActivityBLL();
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            category = HttpUtility.UrlDecode(category);

            var total = 0;
            var data = sbll.GetSafetydayList(page, pagesize, category, out total).OrderByDescending(x => x.CreateDate);

            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.pagesize = pagesize;
            ViewBag.category = category;
            var user = OperatorProvider.Provider.Current();
            var act = bll.GetActivities(category, user.DeptId);
            if (act != null)
            {
                if (category == "安全日活动")
                {
                    if (act.State == "Ready")
                    {
                        if (string.IsNullOrEmpty(act.Leader))
                        {
                            ViewBag.safetyState = "Ready";
                        }
                        else
                        {
                            ViewBag.safetyState = "already";
                        }
                    }
                    else if (act.State == "Study")
                        ViewBag.safetyState = "Study";
                }
                else if (category == "政治学习")
                {
                    if (act.State == "Ready")
                    {
                        if (string.IsNullOrEmpty(act.Leader))
                        {
                            ViewBag.safetyState = "Ready";
                        }
                        else
                        {
                            ViewBag.safetyState = "already";
                        }
                    }
                    else if (act.State == "Study")
                        ViewBag.politicsState = "Study";
                }
            }

            return View(data);
        }
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DetailSafetyday(string keyValue)
        {
            var userId = OperatorProvider.Provider.Current().UserId;
            this.SaveRead(keyValue, userId);
            return View();
        }
        public ActionResult NewSafetyday(string category, string id)
        {
            category = HttpUtility.UrlDecode(category);

            var user = OperatorProvider.Provider.Current();
            UserBLL userBLL = new UserBLL();
            SafetydayBLL sbll = new SafetydayBLL();
            var users = new PeopleBLL().GetListByDept(user.DeptId);
            ViewData["users"] = users;
            var model = new ActivityEntity();
            var bzaqy = users.FirstOrDefault(x => x.Quarters != null && x.Quarters.Contains("安全员"));
            var safetyday = sbll.GetIdEntityList(id);
            var strSubject = string.Empty;
            if (safetyday == null)
            {
                model = new ActivityEntity() { ActivityId = Guid.NewGuid().ToString(), PlanStartTime = DateTime.Now, PlanEndTime = DateTime.Now.AddHours(4), ActivityPlace = "班组办公室", ActivityLimited = "4小时", ChairPerson = user.UserName, RecordPerson = bzaqy == null ? string.Empty : bzaqy.Name, AlertType = "提前15分钟", ActivityType = category, GroupId = user.DeptId, Persons = string.Join(",", users.Select(x => x.Name)), PersonId = string.Join(",", users.Select(x => x.ID)) };
            }
            else
            {
                foreach (SafetydayEntity se in safetyday)
                {
                    strSubject += se.Subject + " ";
                    this.SaveRead(se.Id, user.UserId);
                }
                model = new ActivityEntity() { ActivityId = Guid.NewGuid().ToString(), Subject = strSubject, PlanStartTime = DateTime.Now, PlanEndTime = DateTime.Now.AddHours(4), ActivityPlace = "班组办公室", ActivityLimited = "4小时", ChairPerson = user.UserName, RecordPerson = bzaqy == null ? string.Empty : bzaqy.Name, AlertType = "提前15分钟", ActivityType = category, GroupId = user.DeptId, Persons = string.Join(",", users.Select(x => x.Name)), PersonId = string.Join(",", users.Select(x => x.ID)) };
                FileInfoBLL fbll = new FileInfoBLL();
                foreach (SafetydayEntity se in safetyday)
                {
                    var file = fbll.GetFilesByRecIdNew(se.Id).ToList();
                    if (model.Files == null) model.Files = new List<FileInfoEntity>();
                    if (file != null)
                    {
                        foreach (FileInfoEntity fie in file.ToList())
                        {
                            var fileid = Guid.NewGuid().ToString();
                            var query = new FileInfoEntity() { FileId = fileid, FolderId = fie.FileId, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = "班组活动", FileExtensions = fie.FileExtensions, FileName = fie.FileName, FilePath = fie.FilePath, FileType = fie.FileType, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = model.ActivityId };
                            query.RecId = model.ActivityId;
                            query.Description = "材料";
                            fbll.SaveForm(query);
                            model.Files.Add(query);
                        }
                    }
                }
            }
            model.ActivityPersons = users.Select(x => new ActivityPersonEntity() { ActivityId = model.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.ID, Person = x.Name, IsSigned = true }).ToList();
            //ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName }));
            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files);

            var bll = new ActivityBLL();
            model.PlanEndTime = model.PlanStartTime.AddHours(2);
            model.StartTime = DateTime.Now;
            model.CreateDate = DateTime.Now;
            //model.Files = new List<FileInfoEntity>() { this.BuildImage(model.ActivityId) };
            model.Files.Add(this.BuildImage(model.ActivityId));
            model.State = "Ready";
            model.safetyday = id;
            var QuestionList = new List<QuestionBankEntity>();
            if (category == "安全日活动")
            {
                QuestionBankBLL question = new QuestionBankBLL();
                var qustionId = id.Split(',');

                foreach (var item in qustionId)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var questionModel = question.GetDetailbyOutId(item);
                    foreach (var questionentity in questionModel)
                    {
                        var ckFile = true;

                        var filesId = questionentity.fileids.Split(',');
                        foreach (var fileid in filesId)
                        {
                            if (!string.IsNullOrEmpty(fileid))
                            {
                                var ck = model.Files.Where(x => x.FolderId == fileid).ToList();
                                if (ck.Count == 0)
                                {
                                    ckFile = false;
                                    break;
                                }

                            }
                        }
                        if (ckFile)
                        {
                            var FilesId = model.Files.Where(x => !string.IsNullOrEmpty(x.FolderId)).Where(x => questionentity.fileids.Contains(x.FolderId)).ToList();
                            questionentity.fileids = string.Join(",", FilesId.Select(x => x.FileId));
                            questionentity.safetydayid = item;
                            QuestionList.Add(questionentity);
                        }

                    }

                }

            }
            ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);

            //bll.SaveFormSafetyday(model.ActivityId, model, safetyday);

            return View(model);
        }

        [HttpPost]
        public ActionResult NewSafetyday(ActivityEntity model, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            SafetydayBLL sbll = new SafetydayBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);
            var QuestionBank = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryQuestionEntity>>(fc["QuestionList"]);

            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            var filebll = new FileInfoBLL();
            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }

                var file = this.SaveFile(model.ActivityId);

                model.Files.Add(file);
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);

                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.FolderId }));
                return View(model);
            }

            var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }
                var remove = new List<HistoryQuestionEntity>();
                for (int i = 0; i < QuestionBank.Count; i++)
                {
                    if (QuestionBank[i].fileids.Contains(fileid))
                        remove.Add(QuestionBank[i]);
                }
                if (remove.Count>0)
                {
                    foreach (var item in remove)
                    {
                        QuestionBank.Remove(item);
                    }
                   
                }
                var filepath = filebll.Delete(fileid);
                //if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                //    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();

                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName, x.FolderId }));

                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                return View(model);
            }

            var bll = new ActivityBLL();
            model.PlanEndTime = model.PlanStartTime.AddHours(2);
            model.StartTime = DateTime.Now;
            model.CreateDate = DateTime.Now;
            //model.Files = new List<FileInfoEntity>() { this.BuildImage(model.ActivityId) };
            model.Files.Add(this.BuildImage(model.ActivityId));
            model.State = "Ready";
            var safe = sbll.GetIdEntityList(model.safetyday).ToList();
            bll.SaveFormSafetyday(null, model, safe, user.UserId);
            //生成答题台账
            if (QuestionBank.Count > 0)
            {
                DepartmentBLL dept = new DepartmentBLL();
                QuestionBankBLL question = new QuestionBankBLL();
                var list = new List<TheTitleEntity>();
                var operationUser = userBLL.GetEntity(user.UserId);
                var mydept = dept.GetEntity(model.GroupId);
                var i = 0;
                foreach (var item in QuestionBank)
                {
                    item.sort = i;
                    i++;
                }

                foreach (var bank in QuestionBank)
                {
                    bank.Id = null;
                    //bank.titleid = title.Id;
                    bank.outkeyvalue = model.ActivityId;
                }
                question.SaveFormHistoryQuestion(list, QuestionBank, operationUser,"");

            }
            return RedirectToAction("ReadySafetyday", new { id = model.ActivityId });
        }

        public ActionResult ReadySafetyday(string id)
        {
            var bll = new ActivityBLL();
            var model = bll.GetEntity(id);

            var user = OperatorProvider.Provider.Current();

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;

            if (model.ActivityPersons == null || model.ActivityPersons.Count == 0)
            {
                model.ActivityPersons = users.Select(x => new ActivityPersonEntity() { ActivityId = model.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.UserId, Person = x.RealName, IsSigned = true }).ToList();
            }
            model.Persons = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned).Select(x => x.Person));
            model.PersonId = string.Join(",", model.ActivityPersons.Where(x => x.IsSigned).Select(x => x.PersonId));
            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, Description = string.Empty }));
            var QuestionList = new List<HistoryQuestionEntity>();
            if (model.ActivityType == "安全日活动")
            {
                QuestionBankBLL question = new QuestionBankBLL();
                QuestionList = question.GetHistoryDetailbyActivityId(id);
            }
            ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);

            return View(model);
        }

        [HttpPost]
        public ActionResult ReadySafetyday(string id, ActivityEntity model, FormCollection fc)
        {

            var user = OperatorProvider.Provider.Current();
            QuestionBankBLL question = new QuestionBankBLL();
            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);
            var QuestionBank = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryQuestionEntity>>(fc["QuestionList"]);
            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            var filebll = new FileInfoBLL();
            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }
                var file = this.SaveFile(model.ActivityId);
                file.Description = "candelete";

                model.Files.Add(file);
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, x.Description }));
                return View(model);
            }
         
                var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }

                var filepath = filebll.Delete(fileid);
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();
                var remove = new List<HistoryQuestionEntity>();
                for (int i = 0; i < QuestionBank.Count; i++)
                {
                    if (QuestionBank[i].fileids.Contains(fileid))
                        remove.Add(QuestionBank[i]);
                }
                if (remove.Count > 0)
                {
                    foreach (var item in remove)
                    {
                        QuestionBank.Remove(item);
                    }

                }
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, x.Description }));
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                return View(model);
            }

            if (model.ActivityLimited.Contains("分钟"))
            {
                model.PlanEndTime = model.PlanStartTime.AddMinutes(double.Parse(model.ActivityLimited.Replace("分钟", string.Empty)));
            }
            else if (model.ActivityLimited.Contains("小时"))
            {
                model.PlanEndTime = model.PlanStartTime.AddHours(double.Parse(model.ActivityLimited.Replace("小时", string.Empty)));
            }
            else
            {
                model.PlanEndTime = model.PlanStartTime.AddHours(2);
            }

            model.StartTime = DateTime.Now;
            model.Files.Add(this.BuildImage(model.ActivityId));
            var bll = new ActivityBLL();
            if (fc.Get("issave") == "true")
            {
                bll.Edit(model);
                UserBLL userBLL = new UserBLL();
                var list = new List<TheTitleEntity>();

                var operationUser = userBLL.GetEntity(user.UserId);
                //生成答题台账
                if (QuestionBank.Count > 0)
                {
                   
                    DepartmentBLL dept = new DepartmentBLL();
                    var mydept = dept.GetEntity(model.GroupId);
                    var i = 0;
                  
                    foreach (var item in QuestionBank)
                    {
                        item.sort = i;
                        i++;
                    }
                    //QuestionBank = QuestionBank.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                    foreach (var bank in QuestionBank)
                    {
                        //bank.titleid = title.Id;
                        bank.outkeyvalue = model.ActivityId;
                    }
                }
                var questionStr = fc.Get("qdelete");
                question.SaveFormHistoryQuestion(list, QuestionBank, operationUser, questionStr);
                QuestionBank = question.GetHistoryDetailbyActivityId(id);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description != "二维码").Select(x => new { x.FileId, x.FileName, x.Description }));
                ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionBank);
                return RedirectToAction("ReadySafetyday", new { id = model.ActivityId });
            }
           
            //生成答题台账
            if (QuestionBank.Count > 0)
            {
                UserBLL userBLL = new UserBLL();
                DepartmentBLL dept = new DepartmentBLL();
               
                var list = new List<TheTitleEntity>();
                var operationUser = userBLL.GetEntity(user.UserId);
                var mydept = dept.GetEntity(model.GroupId);
                var i = 0;
                foreach (var item in QuestionBank)
                {
                    item.sort = i;
                    i++;
                }
                foreach (var item in model.ActivityPersons)
                {
                    var title = new TheTitleEntity();
                    title.Id = Guid.NewGuid().ToString();
                    title.startTime = DateTime.Now;
                    title.iscomplete = false;
                    title.category = model.ActivityType;
                    title.activityid = model.ActivityId;
                    var workuser = userBLL.GetEntity(item.PersonId);
                    title.userid = workuser.UserId;
                    title.username = workuser.RealName;
                    title.deptcode = mydept.EnCode;
                    title.deptid = mydept.DepartmentId;
                    title.deptname = mydept.FullName;
                    title.endTime = null;
                    title.score = "0";
                    list.Add(title);
                }
                //QuestionBank = QuestionBank.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                foreach (var bank in QuestionBank)
                {
                    //bank.titleid = title.Id;
                    bank.outkeyvalue = model.ActivityId;
                }
                question.SaveFormHistoryQuestion(list, QuestionBank, operationUser,"");

            }
      
            bll.Ready(model);
            return RedirectToAction("Study", new { id = model.ActivityId });
        }
        public ActionResult SaveRead(string keyValue, string userId)
        {
            SafetydayBLL sbll = new SafetydayBLL();
            sbll.SaveRead(keyValue, userId);
            return Success("操作成功。");
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

        public ActionResult GetActivityCode(string name)
        {
            string code = string.Empty;
            switch (name)
            {
                case "安全日活动":
                    code = "activity_safeday";
                    break;
                case "政治学习":
                    code = "activity_study";
                    break;
                case "民主管理会":
                    code = "activity_manage";
                    break;
                case "班务会":
                    code = "activity_meet";
                    break;
                case "上级精神宣贯":
                    code = "activity_superior";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(code))
            {
                code = "code=\"" + code + "\"";
            }
            return Content(code);
        }
        #endregion


        #region 评价设置
        /// <summary>
        /// 评价设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EvaluateSet()
        {
            //var itemdetialbll = new DataItemDetailBLL();
            //var itembll = new DataItemBLL();
            ////
            //var mainType = itembll.GetEntityByCode("EvaluateDept");
            //var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            //contentType.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            //ViewData["EvaluateDept"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            return View();
        }

        public ActionResult EvaluateForm()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userid = user.UserId;
            ViewBag.deptid = user.DeptId;
            return View();
        }
        public ActionResult Selectjob()
        {
            return View();
        }
        public ActionResult SelectDept()
        {
            return View();
        }
        public ActionResult moduleGroup()
        {
            return View();
        }
        public ActionResult EvaluateFormDetail()
        {
            return View();
        }

        public List<RoleEntity> getNewData(string userid, string deptid)
        {
            if (deptid == "0") deptid = "";
            var dict = new
            {
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetPostByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            return JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
            //return new List<RoleEntity>();
        }
        public ActionResult getjobs(string userid, string deptid, string jobname, int checkMode = 0, int mode = 0)
        {
            List<TreeEntity> treeList = new List<TreeEntity>();
            var user = OperatorProvider.Provider.Current();
            var data = getNewData(user.UserId, user.DeptId).Distinct(new DuplicateDefine());
            foreach (RoleEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = false;
                tree.id = item.RoleId;
                tree.text = item.FullName;
                tree.value = item.RoleId;
                tree.isexpand = true;
                tree.complete = true;
                if (!string.IsNullOrEmpty(jobname))
                {
                    if (jobname.Contains(item.FullName)) tree.checkstate = 1;
                }
                tree.showcheck = checkMode == 0 ? false : true;
                tree.hasChildren = hasChildren;
                tree.parentId = "0";
                tree.Attribute = "Code";
                tree.AttributeValue = item.EnCode;
                treeList.Add(tree);
            }
            return Content(treeList.ToJson());
        }


        private class DuplicateDefine : IEqualityComparer<RoleEntity>
        {
            public bool Equals(RoleEntity x, RoleEntity y)
            {
                return x.FullName == y.FullName;
            }
            public int GetHashCode(RoleEntity obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        /// <returns></returns>
        public JsonResult getDept()
        {
            var user = OperatorProvider.Provider.Current();
            DepartmentBLL deptbll = new DepartmentBLL();
            var rootdpet = deptbll.GetRootDepartment();
            var data = deptbll.GetSubDepartments(rootdpet.DepartmentId, "部门").Select(x => new ItemEntity { ItemType = x.Nature, ItemId = x.DepartmentId, ItemName = x.FullName, ParentItemId = "0" }).ToList();

            return Json(data.Where(x => x.ParentItemId == "0").Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                value = x.ItemId,
                text = x.ItemName,
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel> GetChildren(List<ItemEntity> data, string id)
        {
            var user = OperatorProvider.Provider.Current();
            return data.Where(x => x.ParentItemId == id).Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                showcheck = true,
                value = x.ItemId,
                text = x.ItemName,
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList();

        }
        public ActionResult GetEvaluateSetData(Pagination pagination, string queryJson)
        {
            var user = OperatorProvider.Provider.Current();
            var watch = CommonHelper.TimerStart();
            pagination.p_kid = "Id";
            pagination.p_fields = @"module,modulename,deptname,deptid,isdept,isgroup,userrole,userjobs,sort,isprofessional,evaluatesort";
            pagination.p_tablename = @"wg_evaluatesteps  ";
            pagination.sidx = "CREATEDATE";
            pagination.sord = "desc";
            pagination.conditionJson = "1=1";
            Operator currUser = OperatorProvider.Provider.Current();

            var bll = new ActivityBLL();
            var queryParam = queryJson.ToJObject();
            if (queryJson.Contains("txt_Keyword"))
            {
                if (!queryParam["txt_Keyword"].IsEmpty())
                {
                    pagination.conditionJson += string.Format(" and module like'%{0}%'", queryParam["txt_Keyword"].ToString());
                }
            }

            var data = bll.GetEvaluateSetData(pagination, null);
            var dataJson = data.ToJson();
            var list = dataJson.ToObject<List<EvaluateStepsEntity>>();
            var group = list.GroupBy(x => x.module);
            var dataNew = new List<EvaluateStepsEntity>();
            foreach (var item in group)
            {
                dataNew.AddRange(item.OrderBy(x => x.sort));
            }
            //var dataNew = data.Clone();
            //dataNew.Clear();
            //var tableRow = data.Rows;
            //var grs = data.Rows.OfType<DataRow>().GroupBy(x => x["module"]);
            //foreach (var gr in grs)
            //{
            //    var grOrder = gr.OrderBy(x => x["sort"]);
            //    foreach (var item in grOrder)
            //    {
            //        dataNew.ImportRow(item);
            //    }

            //}

            var JsonData = new
            {
                rows = dataNew,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult SaveEvaluateSet(string keyValue, EvaluateStepsEntity entity)
        {
            try
            {
                var bll = new ActivityBLL();
                bll.SaveEvaluateSet(keyValue, entity);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }


        }
        public ActionResult deleteEvaluateSet(string keyValue)
        {
            try
            {
                var bll = new ActivityBLL();
                bll.deleteEvaluateSet(keyValue);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }


        }

        public ActionResult getEvaluateSet(string keyValue)
        {
            try
            {
                var bll = new ActivityBLL();
                var entity = bll.getEvaluateSet(keyValue);
                return Content(entity.ToJson());
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }


        }

        /// <summary>
        /// 在线查看文档
        /// </summary>
        /// <param name="fileId">文件的ID</param>
        /// <returns></returns>
        public ActionResult ShowActivityFile(string fileId)
        {
            try
            {

                if (Debugger.IsAttached)
                {
                    Office2PDFHelper.DOCConvertToPDF(Server.MapPath("~/Resource/DocumentFile/111.doc"), Server.MapPath("~/Resource/ActivityPDF/word.pdf"));
                    Office2PDFHelper.XLSConvertToPDF(Server.MapPath("~/Resource/DocumentFile/111.xls"), Server.MapPath("~/Resource/ActivityPDF/xls.pdf"));
                }
                FileInfoEntity file = new FileInfoBLL().GetEntity(fileId);
                string pathDic = "~/Resource/ActivityPDF/";
                if (!Directory.Exists(Server.MapPath(pathDic))) Directory.CreateDirectory(Server.MapPath(pathDic));
                if (file != null && System.IO.File.Exists(Server.MapPath(file.FilePath)))
                {


                    string fileUrl = file.FilePath;
                    string fileExtensions = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);//文件的类型
                    bool suceess = true;

                    switch (fileExtensions.ToLower())
                    {
                        case "doc":
                        case "docx":
                        case "xls":
                        case "xlsx":
                            //属于OFFCIE类型的  判断是否已经有转换成PDF过
                            var filename = file.FileName.Substring(0, file.FileName.LastIndexOf(".")) + ".pdf";
                            if (!System.IO.File.Exists(Server.MapPath(pathDic + file.FileId + "_" + filename)))
                            {
                                string savePath = pathDic + file.FileId + "_" + filename;
                                fileUrl = savePath;
                                switch (fileExtensions.ToLower())
                                {
                                    case "doc":
                                    case "docx":
                                        suceess = Office2PDFHelper.DOCConvertToPDF(Server.MapPath(file.FilePath), Server.MapPath(savePath));
                                        break;
                                    case "xls":
                                    case "xlsx":
                                        suceess = Office2PDFHelper.XLSConvertToPDF(Server.MapPath(file.FilePath), Server.MapPath(savePath));
                                        break;
                                }
                            }
                            else
                            {
                                fileUrl = pathDic + file.FileId + "_" + filename;
                            }
                            break;
                        default:
                            suceess = false;
                            break;
                    }
                    if (suceess)
                    {
                        return Success("获取文件成功！", new { FileUrl = Request.Url.Host + Url.Content(fileUrl) });
                    }
                    else
                    {
                        return Success("获取文件成功！", new { FileUrl = Request.Url.Host + Url.Content(file.FilePath) });
                    }
                }
                else
                {
                    return Error("未找到指定的文件");
                }
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"参数:{fileId}\r\n" + JsonConvert.SerializeObject(ex), "ShowFile");
                return Error(ex.Message);
            }
        }

        #endregion


        #region 统计
        public ActionResult TimeCount()
        {


            return View();
        }
        [HttpPost]
        public JsonResult GetTimeCount(string from, string to, string category)
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
            DateTime t = DateTime.Now.AddDays(1);
            var bll = new ActivityBLL();
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            var dt = bll.GetGroupCount(code, f, t, category);
            var dtStr = dt.ToJson();
            var dtList = dtStr.ToList<GetGroupCountList>();
            var deptList = departmentBLL.GetSubDepartments(dept.DepartmentId, "班组");
            string r = "";
            foreach (var item in deptList)
            {
                var deptDt = dtList.Where(x => x.deptcode.StartsWith(item.EnCode));
                var personcount = deptDt.Sum(x => x.sum);
                double timecount = 0;
                var deptcount = deptDt.Count();
                foreach (var timeList in deptDt)
                {
                    TimeSpan tsDiffer = timeList.endtime - timeList.starttime;
                    var tsDecimal = Math.Round(tsDiffer.TotalHours, 2);
                    timecount += tsDecimal;
                }
                r += "{" + string.Format("deptid:'{0}',deptname:'{1}',activitycount:'{2}',personcount:'{3}',timecount:'{4}'", item.DepartmentId, item.FullName, deptcount, personcount, timecount) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            // return Success("0", new { rows =dt.Rows  });
            return Json(new { rows = r });
        }
        //[HttpPost]
        //public JsonResult GetLearnCount(string from, string to)
        //{
        //    string code = OperatorProvider.Provider.Current().DeptCode;
        //    string deptid = OperatorProvider.Provider.Current().DeptId;
        //    string role = OperatorProvider.Provider.Current().RoleName;
        //    var dept = departmentBLL.GetEntity(deptid);
        //    if (dept != null)
        //    {
        //        if (dept.IsSpecial || role.Contains("厂级用户") || role.Contains("厂级部门用户") || role.Contains("公司领导"))
        //        {
        //            dept = departmentBLL.GetRootDepartment();
        //        }
        //        code = dept.EnCode;
        //    }
        //    DateTime f = DateTime.Now.AddYears(-100);
        //    DateTime t = DateTime.Now.AddDays(1);
        //    if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
        //    if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
        //    var etypes = Config.GetValue("EducationType");
        //    string[] type = new string[] { "1", "2", "3", "4", "5" };
        //    if (!string.IsNullOrEmpty(etypes))
        //    {
        //        type = etypes.Split(',');
        //    }
        //    return Json(new { rows = edubll.GetLearnCount(code, f, t), types = type });
        //}
        #endregion
    }

}
