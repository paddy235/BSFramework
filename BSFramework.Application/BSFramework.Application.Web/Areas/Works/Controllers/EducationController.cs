using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Util.WebControl;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Entity.EducationManage;
using System.Data;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Busines.PublicInfoManage;
using ThoughtWorks.QRCode.Codec;
using System.Text;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Service.WorkMeeting;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Cache.Factory;
using Aspose.Cells;
using System.Drawing;
using BSFramework.Util.Offices;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Application.Busines.Activity;
using System.Management;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class EducationController : MvcControllerBase
    {
        //
        // GET: /Works/Education/
        private EducationBLL edubll = new EducationBLL();
        private ActivityEvaluateBLL evaluateBLL = new ActivityEvaluateBLL();
        private UserBLL userbll = new UserBLL();
        private DepartmentBLL dpbll = new DepartmentBLL();
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();

            var bll = new EdActivityBLL();
            var dic = bll.GetIndex(user.UserId, user.DeptId, "");
            var all = edubll.GetBaseInfoList(user.DeptId);
            ViewData["type1"] = all.Where(x => (x.Flow == "0" || x.Flow == "2") && x.EduType == "1").ToList();
            ViewData["type2"] = all.Where(x => (x.Flow == "0" || x.Flow == "2") && x.EduType == "2").ToList();
            ViewData["type3"] = all.Where(x => (x.Flow == "0" || x.Flow == "2") && x.EduType == "3").ToList();
            ViewData["type4"] = all.Where(x => (x.Flow == "0" || x.Flow == "2") && x.EduType == "4").ToList();
            ViewData["type5"] = dic;
            return View();
        }
        public ActionResult Index4()
        {
            return View();
        }

        public ActionResult Index6()
        {
            return View();
        }

        //public JsonResult GetActivityNew()
        //{

        //    int month = 1;
        //    if (DateTime.Now.Month < 4) month = 1;
        //    else if (DateTime.Now.Month < 7) month = 4;
        //    else if (DateTime.Now.Month < 10) month = 7;
        //    else if (DateTime.Now.Month <= 12) month = 10;
        //    DateTime sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期


        //    var user = OperatorProvider.Provider.Current();
        //    if (user.DeptId == "0" || string.IsNullOrEmpty(user.DeptCode)) user.DeptCode = "0";
        //    var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
        //    var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
        //    var code = this.Request.QueryString.Get("code");
        //    DepartmentBLL deptBll = new DepartmentBLL();
        //    //var groups = deptBll.GetAllGroups().Where(x => x.EnCode.Contains(code)).Select(x => x.DepartmentId);
        //    int all = 0;
        //    var total = 0;
        //    var data = new List<NewEntity>();

        //    var service = new AdminPrettyService();
        //    if (string.IsNullOrEmpty(code)) code = user.DeptCode;
        //    var alist = service.GetActs(code) as EdActivityEntity;
        //    var edulist = service.GetEdus(code);
        //    var mlist = service.GetDangers(code);
        //    var entity = new NewEntity();
        //    foreach (EdActivityEntity a in alist)
        //    {
        //        entity = new NewEntity();
        //        entity.Type = a.ActivityType;
        //        entity.BZID = a.GroupId;
        //        var dept = deptBll.GetEntity(entity.BZID);

        //        entity.BZName = dept.FullName;
        //        entity.Date = a.StartTime;
        //        entity.ID = a.ActivityId;
        //        entity.Theme = a.Subject;
        //        entity.Remark = "1";
        //        data.Add(entity);
        //        all++;
        //    }
        //    foreach (EduBaseInfoEntity a in edulist)
        //    {
        //        entity = new NewEntity();
        //        string type = "教育培训";
        //        if (a.EduType == "1") type = "技术讲课";
        //        if (a.EduType == "2" || a.EduType == "5") type = "技术问答";
        //        if (a.EduType == "3") type = "事故预想";
        //        if (a.EduType == "4") type = "反事故演习";
        //        entity.Type = type;
        //        entity.BZID = a.BZId;
        //        entity.BZName = a.BZName;
        //        entity.Date = a.ActivityDate.Value;
        //        entity.ID = a.ID;
        //        entity.Theme = a.Theme;
        //        entity.Remark = "2";
        //        data.Add(entity);
        //        all++;
        //    }
        //    foreach (DangerEntity a in mlist)
        //    {
        //        entity = new NewEntity();
        //        entity.Type = "危险预知训练";
        //        entity.BZID = a.GroupId;
        //        var dept = deptBll.GetEntity(entity.BZID);

        //        entity.BZName = dept.FullName;
        //        entity.Date = a.JobTime.Value;
        //        entity.ID = a.Id;
        //        entity.Theme = a.JobName;
        //        entity.Remark = "3";
        //        data.Add(entity);
        //        all++;
        //    }
        //    data = data.OrderByDescending(x => x.Date).ToList();
        //    data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        //    return Json(new { rows = data, records = all, page = page, total = Math.Ceiling((decimal)all / pagesize) }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Index2(string type)
        {
            ViewBag.dafen = Url.Action("Index", "Evaluate", new { area = "EvaluateAbout" });
            var etypes = Config.GetValue("EducationType");
            string[] etype = new string[] { "1", "2", "3", "4", "5" };
            if (!string.IsNullOrEmpty(etypes))
            {
                etype = etypes.Split(',');
            }
            ViewBag.from = "";
            ViewBag.to = "";
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString("yyyy-MM-dd");  //当前季度开始日期
            string edt = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(type))
            {
                ViewBag.type = "0";
            }
            else
            {
                ViewBag.type = type;
                ViewBag.from = sdt;
                ViewBag.to = edt;
            }

            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            ViewBag.etype = etype;
            return View();
        }

        public ActionResult Prediction()
        {
            return View();
        }

        public ActionResult List(int page, int pagesize, string category, string from, string to, FormCollection fc)
        {
            var filebll = new FileInfoBLL();
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            if (string.IsNullOrEmpty(from)) from = fc.Get("from");
            if (string.IsNullOrEmpty(to)) to = fc.Get("to");

            ViewData["from"] = from;
            ViewData["to"] = to;
            //ViewData["name"] = name;
            ViewData["category"] = category;

            var user = OperatorProvider.Provider.Current();
            var all = edubll.GetBaseInfoList(user.DeptId).Where(x => x.EduType == category && x.Flow == "1").ToList();


            if (!string.IsNullOrEmpty(from)) all = all.Where(x => x.ActivityDate >= DateTime.Parse(from)).ToList();
            if (!string.IsNullOrEmpty(to)) all = all.Where(x => x.ActivityDate <= DateTime.Parse(to).AddDays(1)).ToList();
            if (!string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(to)) all = all.Where(x => x.ActivityDate >= DateTime.Parse(from) && x.ActivityDate <= DateTime.Parse(to).AddDays(1)).ToList();

            var total = all.Count();
            all = all.OrderByDescending(x => x.ActivityDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            foreach (EduBaseInfoEntity e in all)
            {
                var files = new FileInfoService().GetBgImage(e.ID);
                //files = files.Where(x => x.FileType.ToLower() == "jpg" || x.FileType == "png" || x.FileType == "gif");
                //files = files.Where(x => x.Description != "教育培训二维码");
                files = files.Where(x => x.Description != null);
                files = files.Where(x => x.Description.Contains("照片"));

                if (files.Count() > 0)
                {
                    e.BgImage = files.FirstOrDefault().FilePath;
                }
                else
                {
                    var alist = edubll.GetAnswerList(e.ID);
                    foreach (EduAnswerEntity an in alist)
                    {
                        files = new FileInfoService().GetBgImage(an.ID);
                        //files = files.Where(x => x.FileType.ToLower().Contains("jpg") || x.FileType.ToLower().Contains( "png") || x.FileType.ToLower().Contains( "gif"));
                        //files = files.Where(x => x.Description != "教育培训二维码");
                        files = files.Where(x => x.Description.Contains("照片"));
                        if (files.Count() > 0)
                        {
                            e.BgImage = files.FirstOrDefault().FilePath;
                            break;
                        }
                    }
                }
            }
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.category = category;
            return View(all);
        }
        /// <summary>
        /// 展示页
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public ActionResult Appoint(string category)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var users = userbll.GetDeptUsers(OperatorProvider.Provider.Current().DeptId).ToList();
            var user = OperatorProvider.Provider.Current();
            var model = new EduBaseInfoEntity() { ID = Guid.NewGuid().ToString(), ActivityDate = DateTime.Now, ActivityEndDate = DateTime.Now.AddHours(4), RegisterDate = DateTime.Now, ActivityLocation = "班组办公室", ActivityTime = "4小时", Teacher = user.UserName, TeacherId = user.UserId, RegisterPeople = user.UserName, RegisterPeopleId = user.UserId, Remind = "提前15分钟", EduType = category, BZId = user.DeptId, BZName = user.DeptName, AttendPeople = string.Join(",", users.Select(x => x.RealName)), AttendPeopleId = string.Join(",", users.Select(x => x.UserId)), CreateUser = user.UserId, CreateDate = DateTime.Now };

            var list = edubll.GetBaseInfoList(user.DeptId).Where(x => x.EduType == category && x.Flow == "2");
            if (list.Count() > 0)
            {
                model = list.FirstOrDefault();
                model.Files = fbll.GetFilesByRecIdNew(model.ID).Where(x => x.Description != "教育培训二维码").ToList();
                ViewData["files1"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description == "1").Select(x => new { x.FileId, x.FileName }));
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Where(x => x.Description == "课件").Select(x => new { x.FileId, x.FileName }));
            }
            var plist = new PeopleBLL().GetListByDept(user.DeptId).ToList();
            //string[] property = new string[] { "Planer", "Name" };
            //bool[] sort = new bool[] { true, true };

            //plist = new IListSort<PeopleEntity>(plist, property, sort).Sort().ToList();
            ViewData["users"] = plist;
            return View(model);
        }
        public JsonResult SelEduInventory(string query, int limit)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            var total = 0;
            var data = edubll.GetInventoryList(dept.EnCode, null, "1", query, limit, 1, out total);
            foreach (EduInventoryEntity e in data)
            {
                e.Files = fbll.GetFilesByRecIdNew(e.ID);
                if (e.Files.Count > 0)
                {
                    e.Name = e.Files.FirstOrDefault().FileName;
                    e.ID = e.Files.FirstOrDefault().FileId;
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult newfile(string id, string eduid)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var file = fbll.GetEntity(id);
            file.RecId = eduid;
            file.FileId = Guid.NewGuid().ToString();
            file.Description = "1";
            fbll.SaveForm(file);
            return Success("0", new { filename = file.FileName, fileid = file.FileId });
        }
        public ActionResult deletenewfile(string id)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var file = fbll.GetEntity(id);
            fbll.Delete(id);
            var files = fbll.GetFilesByRecIdNew(file.RecId);
            var files1 = Newtonsoft.Json.JsonConvert.SerializeObject(files.Where(x => x.Description == "1").Select(x => new { x.FileId, x.FileName }));
            return Success("0", new { files = files1 });
        }
        public ActionResult AppointJSJK(string category)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var users = userbll.GetDeptUsers(OperatorProvider.Provider.Current().DeptId).ToList();
            var user = OperatorProvider.Provider.Current();
            var model = new EduBaseInfoEntity() { ID = Guid.NewGuid().ToString(), ActivityDate = DateTime.Now, ActivityEndDate = DateTime.Now.AddHours(4), RegisterDate = DateTime.Now, ActivityLocation = "班组办公室", ActivityTime = "4小时", Teacher = user.UserName, TeacherId = user.UserId, RegisterPeople = user.UserName, RegisterPeopleId = user.UserId, Remind = "提前15分钟", EduType = category, BZId = user.DeptId, BZName = user.DeptName, AttendPeople = string.Join(",", users.Select(x => x.RealName)), AttendPeopleId = string.Join(",", users.Select(x => x.UserId)), CreateUser = user.UserId, CreateDate = DateTime.Now, Flow = "2" };

            var list = edubll.GetBaseInfoList(user.DeptId).Where(x => x.EduType == category && x.Flow == "2");
            if (list.Count() > 0)
            {
                model = list.FirstOrDefault();
                model.Files = fbll.GetFilesByRecIdNew(model.ID).Where(x => x.Description != "教育培训二维码").ToList();
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName }));
            }

            var plist = new PeopleBLL().GetListByDept(user.DeptId).ToList();
            //string[] property = new string[] { "Planer", "Name" };
            //bool[] sort = new bool[] { true, true };

            //plist = new IListSort<PeopleEntity>(plist, property, sort).Sort().ToList();
            ViewData["users"] = plist;
            return View(model);
        }
        /// <summary>
        /// 提交教育培训预约
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Appoint(EduBaseInfoEntity model, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var filebll = new FileInfoBLL();
            var users = userbll.GetDeptUsers(user.DeptId).ToList();
            var plist = new PeopleBLL().GetListByDept(user.DeptId).ToList();
            //string[] property = new string[] { "Planer", "Name" };
            //bool[] sort = new bool[] { true, true };

            //plist = new IListSort<PeopleEntity>(plist, property, sort).Sort().ToList();
            ViewData["users"] = plist;

            //获取附件
            model.Files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInfoEntity>>(fc["filedata"]);

            //20190123 本地上传课件，页面刷新，获取课件库课件
            var files = filebll.GetFilesByRecIdNew(model.ID);
            ViewData["files1"] = Newtonsoft.Json.JsonConvert.SerializeObject(files.Where(x => x.Description == "1").Select(x => new { x.FileId, x.FileName }));

            if (model.Files == null) model.Files = new List<FileInfoEntity>();


            if (fc.Get("isfile") == "true")
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (!string.IsNullOrEmpty(model.Files[i].FileId))
                        model.Files[i] = filebll.GetEntity(model.Files[i].FileId);
                }
                var file = this.SaveFile(model.ID);

                model.Files.Add(file);
                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName }));


                return View(model);
            }
            //删除附件
            var fileid = fc.Get("delete");
            if (!string.IsNullOrEmpty(fileid))
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    if (model.Files[i].FileId == fileid)
                        model.Files.Remove(model.Files[i]);
                }

                var filepath = filebll.Delete(fileid);
                //多条数据使用同一个文件，所以不删除实体文件
                //if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath(filepath)))
                //    System.IO.File.Delete(Server.MapPath(filepath));

                fc.Clear();

                ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.Files.Select(x => new { x.FileId, x.FileName }));
                return View(model);
            }

            //处理时间类型
            if (model.ActivityTime.EndsWith("小时"))
            {
                model.ActivityEndDate = model.ActivityDate.Value.AddHours(Convert.ToDouble(model.ActivityTime.Substring(0, model.ActivityTime.Length - 2)));
            }
            if (model.ActivityTime.EndsWith("分钟"))
            {
                model.ActivityEndDate = model.ActivityDate.Value.AddMinutes(Convert.ToDouble(model.ActivityTime.Substring(0, model.ActivityTime.Length - 2)));
            }
            //model.Files = new List<FileInfoEntity>() { this.BuildImage(model.ID) };
            FileInfoBLL fbll = new FileInfoBLL();
            var flist = fbll.GetFilesByRecIdNew(model.ID).Where(x => x.Description == "教育培训二维码");
            if (string.IsNullOrEmpty(model.Flow))
            {
                model.Flow = "2";
                if (flist.Count() < 1)
                {
                    filebll.SaveForm(this.BuildImage(model.ID));
                }

            }
            else
            {
                model.Flow = "0";
            }
            int num = model.AttendPeopleId.Split(',').Count();
            model.AttendNumber = num;
            //保存教育培训预约
            edubll.SaveEduBaseInfo(model.ID, model);
            //发送通知
            SendMessage(model);
            //return View(model);

            if (model.Flow == "0")
            {
                return RedirectToAction("SkillAnswer", new { id = model.ID });
            }
            else
            {
                return RedirectToAction("Appoint", new { category = model.EduType });
            }
        }
        /// <summary>
        /// 开始活动，为所有参加人员发送通知
        /// </summary>
        /// <param name="edu"></param>
        private void SendMessage(EduBaseInfoEntity edu)
        {
            var user = OperatorProvider.Provider.Current();
            string[] ids = edu.AttendPeopleId.Split(',');
            string type = "";
            switch (edu.EduType)
            {
                case "1":
                    type = "技术讲课";
                    break;
                case "2":
                    type = "技术问答";
                    break;
                case "3":
                    type = "事故预想";
                    break;
            }
            foreach (string id in ids)
            {
                var entity = new EduMessageEntity();
                entity.ID = Guid.NewGuid().ToString();
                entity.EduId = edu.ID;
                entity.CreateDate = DateTime.Now;
                entity.CreateUser = user.UserId;
                entity.InceptPeople = "";
                entity.InceptPeopleId = id;
                entity.Content = "请按时参加" + type + "活动";
                edubll.SaveMessage(string.Empty, entity);
            }
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        private FileInfoEntity BuildImage(string activityid)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(activityid + "|教育培训", Encoding.UTF8);
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
                Description = "教育培训二维码",
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

        /// <summary>
        /// 保存附件信息
        /// </summary>
        /// <param name="refid"></param>
        /// <returns></returns>
        private FileInfoEntity SaveFile(string refid)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = departmentBLL.GetEntity(user.DeptId);
            while (dept.Nature != "厂级")
            {
                dept = departmentBLL.GetEntity(dept.ParentId);
            }
            var path = "~/Resource/DocumentFile/";

            var id = Guid.NewGuid().ToString();
            var file = this.Request.Files[0];
            var ext = Path.GetExtension(file.FileName);

            file.SaveAs(Path.Combine(Server.MapPath(path), id + ext));

            var model = new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, DeleteMark = 0, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = "课件", FileExtensions = ext, FileName = file.FileName, FilePath = path + id + ext, FileType = ext.Replace(".", ""), ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = refid };


            FileInfoBLL bll = new FileInfoBLL();
            bll.SaveForm(model);


            var obj = new EduInventoryEntity();
            obj.ID = Guid.NewGuid().ToString();
            obj.CreateDate = DateTime.Now;
            obj.CreateUserId = user.UserId;
            obj.CreateUserName = user.UserName;
            obj.EduType = "1";
            obj.Name = model.FileName;
            obj.ModifyDate = DateTime.Now;
            obj.DeptCode = dept.EnCode;
            edubll.SaveForm(obj.ID, obj);

            model.FileId = Guid.NewGuid().ToString();
            model.RecId = obj.ID;
            bll.SaveForm(model);
            return model;
        }

        /// <summary>
        /// 平台首页列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public JsonResult GetEducationJson(string name, string type, string from, string to, string appraise, string deptid, int rows, int page)
        {
            DateTime? start = null;
            DateTime? end = null;
            if (!string.IsNullOrEmpty(from)) start = DateTime.Parse(from);
            if (!string.IsNullOrEmpty(to)) end = DateTime.Parse(to);

            Operator user = OperatorProvider.Provider.Current();

            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);

            var total = 0;
            var data = edubll.GetList(name, type, start, end, appraise, depts.Select(x => x.DepartmentId).ToArray(), rows, page, out total);

            foreach (var item in data)
            {
                item.Status = evaluateBLL.GetEvaluateStatus(item.ID, user.UserId);
            }

            return Json(new { rows = data, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);


            //var dept = departmentBLL.GetEntity(user.DeptId);
            //if (dept == null)
            //{
            //    user.DeptCode = "0";
            //}
            //else
            //{
            //    if (dept.IsSpecial)
            //    {
            //        dept = departmentBLL.GetEntity(dept.ParentId);
            //    }
            //    user.DeptCode = dept.EnCode;
            //}

            //if (!string.IsNullOrEmpty(deptid))
            //{
            //    dept = departmentBLL.GetEntity(deptid);
            //    user.DeptCode = dept.EnCode;
            //}
            //if (!string.IsNullOrEmpty(select))
            //{
            //    dept = departmentBLL.GetEntity(select);
            //    user.DeptCode = dept.EnCode;
            //}

            //pagination.p_kid = "ID";
            //pagination.p_fields = "edutype,theme,bzname,createdate,appraisecontent,activitydate,flow,bzid,activityenddate,learntime,attendnumber"; //必须包含where条件所需字段
            //                                                                                                                                       // pagination.p_tablename = "wg_edubaseinfo ";
            //pagination.conditionJson = "1=1 and flow = '1' and bzid in(select departmentid from base_department where encode like '" + user.DeptCode + "%' and nature = '班组')";
            //StringBuilder tableStr = new StringBuilder();
            //tableStr.Append("(select ID,edutype,theme,bzname,createdate,appraisecontent,activitydate,flow,bzid,activityenddate,learntime,attendnumber from ");
            //tableStr.Append(" wg_edubaseinfo where  flow = '1' ");

            //if (string.IsNullOrEmpty(queryJson))  //queryJson 下拉框中包含是否评价条件，为避免重复 所以只在首次加载数据时，增加筛选条件  
            //{
            //    if (type == "2")
            //    {
            //        pagination.conditionJson += " and (appraisecontent != '1' or appraisecontent is null)";

            //    }
            //    if (type == "4")
            //    {
            //        int month = 1;
            //        if (DateTime.Now.Month < 4) month = 1;
            //        else if (DateTime.Now.Month < 7) month = 4;
            //        else if (DateTime.Now.Month < 10) month = 7;
            //        else if (DateTime.Now.Month <= 12) month = 10;
            //        string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString();  //当前季度开始日期
            //        string edt = DateTime.Now.ToString();
            //        pagination.conditionJson += string.Format(" and activitydate > '{0}'", sdt);


            //    }

            //}
            //tableStr.Append(" union all select a.activityid as ID,a.activitytype as edutype,a.subject as theme ,b.FULLNAME as bzname");
            //tableStr.Append(",a.createdate,case when (SELECT count(activityid) FROM   wg_activityevaluate  where activityid=a.activityid )>0 then 1 else NULL  end  as appraisecontent,a.starttime as activitydate,1 as flow,a.groupid as bzid,a.endtime as activityenddate ,");
            //tableStr.Append("  ROUND(TIMESTAMPDIFF(MINUTE,a.starttime,a.endtime)/60,2) as learntime,(SELECT count(activityid) FROM   wg_edactivityperson where activityid=a.activityid )  as attendnumber  ");
            //tableStr.Append(" from wg_edactivity as a LEFT JOIN base_department as b on a.groupid=b.DEPARTMENTID  ");
            //tableStr.Append(") as r");
            //pagination.p_tablename = tableStr.ToString();
            //int total = 0;

            //var watch = CommonHelper.TimerStart();
            //DataTable dt = edubll.GetEducationPageList(pagination, queryJson);
            //dt.Columns.Add("state");
            //dt.Columns.Add("date");
            //dt.Columns.Add("attendnum");
            //foreach (DataRow row in dt.Rows)
            //{

            //    //decimal learntime = Convert.ToDecimal(row["learntime"]) / Convert.ToInt32(row["attendnumber"]);
            //    //row["learntime"] =Math.Round( learntime,2);
            //    //int n = bll.GetActivityEvaluateEntity(row["ID"].ToString(), 10000, 1, out total).Count();
            //    var n = bll.GetActivityEvaluateEntity(row["ID"].ToString());
            //    if (n.Count() > 0)
            //    {
            //        var ck = n.Where(x => x.EvaluateUser == user.UserName);
            //        if (ck.Count() > 0)
            //        {
            //            row["state"] = "本人已评价";
            //        }
            //        else
            //        {
            //            row["state"] = "本人未评价";
            //        }
            //    }
            //    else
            //    {
            //        row["state"] = "本人未评价";
            //    }

            //    int num = peoplebll.GetListByDept(row["bzid"].ToString()).Count();
            //    string start = Convert.ToDateTime(row["activitydate"]).ToString("yyyy-MM-dd HH:mm");
            //    string end = Convert.ToDateTime(row["activityenddate"]).ToString("yyyy-MM-dd HH:mm");
            //    row["date"] = start + " - " + end.Substring(10, 6);

            //    row["attendnum"] = row["attendnumber"].ToString() + "/" + num;
            //    if (row["edutype"].ToString() == "5" || row["edutype"].ToString() == "6")
            //    {
            //        row["date"] = start.Substring(0, 10) + " - " + end.Substring(0, 10);
            //        row["attendnum"] = "";
            //    }

            //}
            //var JsonData = new
            //{
            //    rows = dt,
            //    total = pagination.total,
            //    page = pagination.page,
            //    records = pagination.records,
            //    costtime = CommonHelper.TimerEnd(watch)
            //};
            //return Content(JsonData.ToJson());
        }

        public ActionResult GetEduPlanJson()
        {
            var eduplanbll = new EduPlanBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = departmentBLL.GetEntity(user.DeptId);
            // if (string.IsNullOrEmpty(user.DeptCode)) user.DeptCode = "0";
            DepartmentBLL deptBll = new DepartmentBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var from = this.Request.QueryString.Get("from");
            var to = this.Request.QueryString.Get("to");
            var name = this.Request.QueryString.Get("name");
            var subtype = this.Request.QueryString.Get("subtype");
            //var state = this.Request.QueryString.Get("state");
            var code = this.Request.QueryString.Get("code");
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
            var groups = deptBll.GetAllGroups().Where(x => x.EnCode.Contains(code)).Select(x => x.DepartmentId);
            var data = eduplanbll.GetPlanList().Where(x => groups.Contains(x.BZID) && (x.SubmitState == "已提交" || (x.SubmitState == "待提交" && x.VerifyState == "审核不通过")));
            if (!string.IsNullOrEmpty(from))
            {
                data = data.Where(x => x.SubmitDate >= Convert.ToDateTime(from));
            }
            if (!string.IsNullOrEmpty(to))
            {
                data = data.Where(x => x.SubmitDate < Convert.ToDateTime(to).AddDays(1));
            }
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(subtype) && subtype != "0")
            {
                if (subtype == "3")
                {
                    data = data.Where(x => x.VerifyState == "待审核");
                }
                else
                {
                    data = data.Where(x => x.VerifyState == subtype);
                }
            }
            total = data.Count();
            data = data.OrderByDescending(x => x.SubmitDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index5()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            return View();
        }

        public ActionResult GetExplains()
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");

            var queryJson = this.Request.QueryString.Get("queryJson");
            var queryParam = queryJson.ToJObject();
            var name = ""; var des = "";
            if (!queryParam["name"].IsEmpty()) name = queryParam["name"].ToString();
            if (!queryParam["des"].IsEmpty()) des = queryParam["des"].ToString();
            var total = 0;

            var watch = CommonHelper.TimerStart();
            var data = fileBll.GetFileList(x => x.Description == "操作说明书" || x.Description == "操作介绍视频").OrderBy(p => p.SortCode).ToList();
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.FileName.Contains(name)).ToList();
            }
            if (!string.IsNullOrEmpty(des))
            {
                data = data.Where(x => x.Description == des).ToList();
            }
            foreach (var f in data)
            {
                f.FilePath = Url.Content(f.FilePath);
            }

            total = data.Count();

            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddExplain()
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var file = new FileInfoEntity();
            file.Description = "操作说明书";
            //获取文件的最大排序号 heming
            int max = fbll.GetExplainMaxSortCode();
            max++;
            file.SortCode = max;

            return View(file);
        }
        public ActionResult EditExplain(string id)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var file = fbll.GetEntity(id);
            return View(file);
        }
        [HttpGet]
        public ActionResult GetEducations()
        {
            Operator user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            if (user.DeptCode == null) user.DeptCode = "0";

            FileInfoBLL fileBll = new FileInfoBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");

            var queryJson = this.Request.QueryString.Get("queryJson");
            var queryParam = queryJson.ToJObject();
            var name = "";
            var deptcode = dept.EnCode;
            if (!queryParam["name"].IsEmpty()) name = queryParam["name"].ToString();
            if (!queryParam["deptcode"].IsEmpty()) deptcode = queryParam["deptcode"].ToString();
            var type = this.Request.QueryString.Get("type");
            var total = 0;
            if (string.IsNullOrEmpty(name)) name = "";
            var watch = CommonHelper.TimerStart();
            //var data = new List<EduInventoryEntity>();
            //if (type == "1") {
            //    data = edubll.GetInventoryList("1").ToList();
            //    if (!string.IsNullOrEmpty(name))
            //    {
            //        data = data.Where(x => x.Name.Contains(name)).ToList();
            //    }
            //} 
            //if (type == "3" || type == "6")
            //{
            //    var one = edubll.GetInventoryList("3").ToList();
            //    var two = edubll.GetInventoryList("6").ToList();
            //    data.AddRange(one);
            //    data.AddRange(two);
            //    if (!string.IsNullOrEmpty(name))
            //    {
            //        data = data.Where(x => x.Name.Contains(name)).ToList();
            //    }
            //}
            //if (type == "5" || type == "2")
            //{
            //    var one = edubll.GetInventoryList("2").ToList();
            //    var two = edubll.GetInventoryList("5").ToList();
            //    data.AddRange(one);
            //    data.AddRange(two);
            //    if (!string.IsNullOrEmpty(name))
            //    {
            //        data = data.Where(x => x.Question.Contains(name)).ToList();
            //    }
            //}
            //data = data.Where(x => (x.UseDeptCode != null && x.UseDeptCode.StartsWith(deptcode)) || x.UseDeptCode == "0").ToList();

            var data = edubll.GetInventoryList(deptcode, null, type, name, pagesize, page, out total);

            //var data = edubll.GetInventoryList("");
            //data = data.Where(x => (x.UseDeptCode != null && x.UseDeptCode.StartsWith(deptcode)) || x.UseDeptCode == "0");
            //if (type == "1") data = data.Where(x => x.EduType == "1" && x.Name.Contains(name));
            //if (type == "3" || type == "6") data = data.Where(x => (x.EduType == "3" || x.EduType == "6") && x.Name.Contains(name));
            //if (type == "2" || type == "5") data = data.Where(x => (x.EduType == "2" || x.EduType == "5") && x.Question.Contains(name));
            //total = data.Count();
            //data = data.OrderByDescending(x => x.ModifyDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();

            if (type == "1")
            {
                foreach (EduInventoryEntity e in data)
                {
                    var file = fileBll.GetFilesByRecIdNew(e.ID).FirstOrDefault();
                    if (file != null)
                    {
                        e.kjname = file.FileName;
                        e.kjpath = Url.Content(file.FilePath);
                        e.kjid = file.FileId;
                    }
                }
            }
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadFileNew(string uptype, string id)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            if (uptype != "2")
            {
                IList<FileInfoEntity> fl = fileBll.GetFilesByRecIdNew(id).ToList();
                foreach (FileInfoEntity fe in fl)
                {
                    string filepath = fileBll.Delete(fe.FileId);
                    if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath("~" + filepath)))
                        System.IO.File.Delete(Server.MapPath("~" + filepath));
                }
            }

            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }

            string FileEextension = Path.GetExtension(files[0].FileName);
            string type = files[0].ContentType;
            if (uptype == "2" && !type.Contains("image"))  //图片
            {
                return Success("1");
            }
            string Id = OperatorProvider.Provider.Current().UserId;
            Id = Guid.NewGuid().ToString();
            string virtualPath = string.Format("~/Content/eduinventoryfile/{0}{1}", Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            //if (uptype == "1") id = Guid.NewGuid().ToString();  //技术讲课，无需关联id，重新生成
            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = id,
                RecId = id,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = uptype
            };
            fileBll.SaveForm(fi);
            var upfileid = "";
            if (uptype == "1") upfileid = fi.RecId;
            var filelist = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = Url.Content(f.FilePath);
            }
            return Success("上传成功。", new { path = virtualPath.TrimStart('~'), name = fi.FileName, files = filelist, newpath = Url.Content(virtualPath), upfileid = upfileid });
        }
        public ActionResult EditUpload(string uptype, string id)
        {
            FileInfoBLL fileBll = new FileInfoBLL();

            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }

            string FileEextension = Path.GetExtension(files[0].FileName);
            string type = files[0].ContentType;

            if (uptype.Contains("音频") && !type.Contains("audio"))
            {
                return Success("1");
            }
            if ((uptype.Contains("图片") || uptype.Contains("照片")) && !type.Contains("image"))
            {
                return Success("1");
            }
            if (uptype == "视频" && !type.Contains("video"))
            {
                return Success("1");
            }
            string Id = OperatorProvider.Provider.Current().UserId;
            Id = Guid.NewGuid().ToString();
            string virtualPath = string.Format("~/Content/eduinventoryfile/{0}{1}", Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            //if (uptype == "1") id = Guid.NewGuid().ToString();  //技术讲课，无需关联id，重新生成
            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = id,
                RecId = id,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = uptype
            };
            fileBll.SaveForm(fi);

            var filelist = fileBll.GetFilesByRecIdNew(id).ToList();
            if (uptype == "1") filelist = filelist.Where(x => x.Description == uptype || x.Description == "课件").ToList();
            else filelist = filelist.Where(x => x.Description == uptype).ToList();
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = Url.Content(f.FilePath);
            }
            return Success("上传成功。", new { path = virtualPath.TrimStart('~'), name = fi.FileName, files = filelist, newpath = Url.Content(virtualPath) });
        }

        public ActionResult delfile(string uptype, string id, string drugid)
        {
            FileInfoBLL fibll = new FileInfoBLL();
            fibll.Delete(id);
            var filelist = fibll.GetFilesByRecIdNew(drugid).ToList();
            if (uptype == "1") filelist = filelist.Where(x => x.Description == uptype || x.Description == "课件").ToList();
            else filelist = filelist.Where(x => x.Description == uptype).ToList();
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = Url.Content(f.FilePath);
            }
            return Success("上传成功。", new { files = filelist });
        }
        public ActionResult UploadFileNew1(string id, string des)
        {
            FileInfoBLL fileBll = new FileInfoBLL();


            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string FileEextension = Path.GetExtension(files[0].FileName);
            string type = files[0].ContentType;

            if (des == "操作介绍视频" && !type.Contains("mp4"))  //图片
            {
                return Success("1", new { mes = "请上传MP4视频文件！" });
            }
            if (des == "操作说明书" && !type.Contains("pdf"))
            {
                return Success("1", new { mes = "请上传PDF文件！" });
            }
            string Id = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(id)) Id = id;
            string virtualPath = string.Format("~/Content/eduinventoryfile/{0}{1}", Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = Id,
                RecId = Id,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = des,
                CreateDate = DateTime.Now
            };
            if (!string.IsNullOrEmpty(id))  //ID不为空，表示修改
            { fileBll.SaveForm(fi.FileId, fi); }
            else
            {
                fileBll.SaveForm(fi);
            }


            return Success("上传成功。", new { name = fi.FileName, newpath = Url.Content(virtualPath), fileid = fi.FileId });
        }
        public ActionResult DeleteOne1(string id)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            fbll.Delete(id);

            return Success("删除成功。");
        }
        public ActionResult delimg(string id, string drugid)
        {
            FileInfoBLL fibll = new FileInfoBLL();
            fibll.Delete(id);
            var filelist = fibll.GetFilesByRecIdNew(drugid).Where(x => x.Description == "2").ToList();
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = f.FilePath.TrimStart('~');
            }
            return Success("删除成功。", new { files = filelist });
        }
        public ActionResult DeleteMore(string ids)
        {
            var idsarr = ids.Split(',');
            foreach (string id in idsarr)
            {
                edubll.RemoveForm(id);
            }
            return Success("删除成功。");
        }
        public ActionResult DeleteOne(string id)
        {
            edubll.DelEducation(id);

            return Success("删除成功。");
        }
        public JsonResult SaveFileInfo(string id, string name, string des, int? sortCode)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            var file = fbll.GetEntity(id);
            file.Description = des;
            file.FileName = name;
            if (sortCode.HasValue)
            {
                file.SortCode = sortCode.Value;
            }
            fbll.SaveForm(file.FileId, file);
            return Json(new { success = true, message = "操作成功" });
        }
        public ActionResult AddInventory(string type, string id)
        {
            FileInfoBLL fbll = new FileInfoBLL();
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
            var model = new EduInventoryEntity();
            model.ID = Guid.NewGuid().ToString();
            model.CreateDate = DateTime.Now;
            model.CreateUserId = user.UserId;
            model.CreateUserName = user.UserName;
            model.EduType = type;
            model.DeptCode = user.DeptCode;
            ViewBag.pics = new List<FileInfoEntity>();
            ViewBag.type = "add";

            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.type = "edit";
                model = edubll.GetEntity(id);
                ViewBag.pics = fbll.GetFilesByRecIdNew(id).ToList();
                if (model.EduType == "1")
                {
                    var file = fbll.GetFilesByRecIdNew(model.ID).FirstOrDefault();
                    if (file != null)
                    {
                        model.kjname = file.FileName;
                        model.kjpath = Url.Content(file.FilePath);
                    }

                }
            }
            var dcdept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dcdept.DepartmentId;
            return View(model);
        }
        public ActionResult EduInventoryInfo(string id)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            Operator user = OperatorProvider.Provider.Current();

            var model = edubll.GetEntity(id);
            ViewBag.pics = fbll.GetFilesByRecIdNew(id).ToList();
            return View(model);
        }
        [HttpPost]
        public JsonResult SaveEduInventory(string id, EduInventoryEntity model)
        {
            FileInfoBLL fbll = new FileInfoBLL();
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            //if (dept == null)
            //{
            //    user.DeptCode = "0";
            //}
            //else if (dept.Nature == "部门")
            //{
            //    var pdept = deptBll.GetEntity(dept.ParentId);
            //    user.DeptCode = pdept == null ? dept.EnCode : pdept.EnCode;
            //}
            var entity = edubll.GetEntity(id);

            if (entity == null)  //新增
            {
                var arrids = model.UseDeptId.Split(',');
                var arrnames = model.UseDeptName.Split(',');
                var arrcodes = model.UseDeptCode.Split(',');
                for (int i = 0; i < arrids.Length; i++)
                {

                    model.ID = Guid.NewGuid().ToString();
                    model.ModifyDate = DateTime.Now;
                    model.ModifyUserId = user.UserId;
                    model.ModifyUserName = user.UserName;
                    model.DeptCode = user.DeptCode;
                    model.UseDeptId = arrids[i];
                    model.UseDeptCode = arrcodes[i];
                    model.UseDeptName = arrnames[i];
                    edubll.SaveForm(model.ID, model);
                    var files = fbll.GetFilesByRecIdNew(id);
                    foreach (FileInfoEntity f in files)
                    {
                        fbll.RemoveForm(f.FileId);
                        f.FileId = Guid.NewGuid().ToString();
                        f.RecId = model.ID;
                        fbll.SaveForm(f);
                    }
                }
            }
            else
            {
                model.ModifyDate = DateTime.Now;
                model.ModifyUserId = user.UserId;
                model.ModifyUserName = user.UserName;
                model.DeptCode = user.DeptCode;
                edubll.SaveForm(model.ID, model);
            }
            return Json(new { success = true, message = "操作成功" });
        }
        [HttpPost]
        public JsonResult SaveEduInventory1(string ids)
        {
            FileInfoBLL fbll = new FileInfoBLL();
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
            var arrids = ids.Split(',');
            foreach (string id in arrids)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var file = fbll.GetFilesByRecIdNew(id).FirstOrDefault();
                    if (file != null)
                    {
                        var model = new EduInventoryEntity();
                        model.ID = id;
                        model.CreateDate = DateTime.Now;
                        model.CreateUserId = user.UserId;
                        model.CreateUserName = user.UserName;
                        model.EduType = "1";
                        model.Name = file.FileName;
                        model.DeptCode = user.DeptCode;
                        edubll.SaveForm(model.ID, model);
                    }
                }
            }
            return Json(new { success = true, message = "操作成功" });
        }

        public ActionResult ImportCard()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            ViewBag.userName = user.UserName;
            return View();
        }
        [HttpPost]
        public JsonResult ImportCardContext(string keyValue, EduInventoryEntity model)
        {
            var success = true;
            var message = "新增成功";
            try
            {
                DepartmentBLL deptBll = new DepartmentBLL();
                Operator user = OperatorProvider.Provider.Current();
                var dept = deptBll.GetEntity(user.DeptId);
                //if (dept == null)
                //{
                //    user.DeptCode = "0";
                //}
                //else if (dept.Nature == "部门")
                //{
                //    var pdept = deptBll.GetEntity(dept.ParentId);
                //    user.DeptCode = pdept == null ? dept.EnCode : pdept.EnCode;
                //}
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                var fileList = fileInfoBLL.GetFilesByRecIdNew(keyValue);
                var arrids = model.UseDeptId.Split(',');
                var arrcodes = model.UseDeptCode.Split(',');
                var arrnames = model.UseDeptName.Split(',');
                for (int j = 0; j < arrids.Length; j++)
                {
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        model = new EduInventoryEntity();
                        model.ID = Guid.NewGuid().ToString();
                        model.CreateDate = DateTime.Now;
                        model.CreateUserId = user.UserId;
                        model.CreateUserName = user.UserName;
                        model.ModifyDate = DateTime.Now;
                        model.EduType = "1";
                        model.Name = fileList[i].FileName;
                        model.BZID = user.DeptId == null ? "" : user.DeptId;
                        model.DeptCode = user.DeptCode;
                        model.UseDeptCode = arrcodes[j];
                        model.UseDeptId = arrids[j];
                        model.UseDeptName = arrnames[j];
                        edubll.SaveForm(model.ID, model);

                        var file = fileList[i];
                        file.FileId = Guid.NewGuid().ToString();
                        file.RecId = model.ID;
                        fileInfoBLL.SaveForm("", file);

                    }
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
        public string PostFile([System.Web.Http.FromUri] string filePath, [System.Web.Http.FromUri] string recId, [System.Web.Http.FromUri] int isDate = 0)
        {
            string newFilePath = "";
            if (Request.Files.Count > 0)
            {
                foreach (string key in Request.Files.Keys)
                {
                    HttpPostedFileBase file = Request.Files[key];
                    //原始文件名
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string fileGuid = Guid.NewGuid().ToString();
                    long filesize = file.ContentLength;
                    string FileEextension = Path.GetExtension(fileName);
                    string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                    string dir = isDate == 0 ? string.Format("~/Resource/{0}", filePath) : string.Format("~/Resource/{0}/{1}", filePath, uploadDate);
                    string newFileName = fileGuid + FileEextension;
                    newFilePath = dir + "/" + newFileName;
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
                        fileInfoEntity.FileId = fileGuid;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            fileInfoEntity.FolderId = filePath;
                        }
                        else
                        {
                            fileInfoEntity.FolderId = "0";
                        }
                        fileInfoEntity.RecId = recId;
                        fileInfoEntity.FileName = fileName;
                        fileInfoEntity.FilePath = dir + "/" + newFileName;
                        fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                        fileInfoEntity.FileExtensions = FileEextension;
                        fileInfoEntity.FileType = FileEextension.TrimStart('.');
                        FileInfoBLL fileInfoBLL = new FileInfoBLL();
                        fileInfoBLL.SaveForm("", fileInfoEntity);
                    }
                }

            }
            return newFilePath;
        }
        public ActionResult ImportNew(string type)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            ViewBag.type = type;

            return View();
        }
        //导入技术讲课及事故预想
        public JsonResult DoImportNew(string type, string codes, string ids, string names)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            var success = true;
            var message = string.Empty;

            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) return Json(new { success = false, message = "请上传excel文件！" });

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
                var arrids = ids.Split(',');
                var arrnames = names.Split(',');
                var arrcodes = codes.Split(',');
                for (int j = 0; j < arrids.Length; j++)
                {
                    if (type == "2")  //导入技术讲课
                    {
                        if (sheet.Cells[0, 0].StringValue != "试题内容" || sheet.Cells[0, 1].StringValue != "参考答案" || sheet.Cells[0, 2].StringValue != "实体文件名")
                        {
                            return Json(new { success = false, message = "请使用正确的模板导入！" });
                        }
                        var date = DateTime.Now;
                        FileInfoEntity fi = new FileInfoEntity();

                        for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                        {
                            var question = sheet.Cells[i, 0].StringValue;

                            if (string.IsNullOrEmpty(question))
                            {
                                return Json(new { success = false, message = "试题内容不能为空！" });
                            }
                        }
                        for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                        {
                            var question = sheet.Cells[i, 0].StringValue;
                            var answer = sheet.Cells[i, 1].StringValue;
                            if (string.IsNullOrEmpty(question))
                            {
                                continue;
                            }
                            var entity = new EduInventoryEntity();
                            string id = Guid.NewGuid().ToString();
                            entity.CreateUserName = user.UserName;
                            entity.CreateUserId = user.UserId;
                            entity.CreateDate = date;
                            entity.ID = id;
                            entity.Question = question;
                            entity.Answer = answer;
                            entity.EduType = type;
                            entity.DeptCode = user.DeptCode;
                            entity.UseDeptCode = arrcodes[j];
                            entity.UseDeptId = arrids[j];
                            entity.UseDeptName = arrnames[j];
                            string filetype = "";
                            string extension = "";
                            string virtualPath = "";
                            //图片
                            string pic = sheet.Cells[i, 2].StringValue;
                            if (pic != "")
                            {
                                pic = pic.TrimStart('{').TrimEnd('}');
                                filetype = pic.Substring(pic.LastIndexOf('.')).TrimStart('.');
                                extension = pic.Substring(pic.LastIndexOf('.'));
                                virtualPath = string.Format("~/Content/eduinventoryfile/{0}", pic);
                                fi = new FileInfoEntity
                                {
                                    FileId = Guid.NewGuid().ToString(),
                                    FolderId = id,
                                    RecId = id,
                                    FileName = pic,
                                    FilePath = virtualPath,
                                    FileType = filetype,
                                    FileExtensions = extension,
                                    FileSize = "",
                                    DeleteMark = 0,
                                    Description = "2"
                                };
                                fileBll.SaveForm(fi);
                            }
                            edubll.SaveForm(entity.ID, entity);
                        }
                    }
                    else if (type == "3")  //导入事故预想
                    {
                        if (sheet.Cells[0, 0].StringValue != "题目" || sheet.Cells[0, 1].StringValue != "事故现象" || sheet.Cells[0, 2].StringValue != "采取措施")
                        {
                            return Json(new { success = false, message = "请使用正确的模板导入！" });
                        }
                        var date = DateTime.Now;
                        FileInfoEntity fi = new FileInfoEntity();
                        for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                        {
                            var name = sheet.Cells[i, 0].StringValue;
                            var danger = sheet.Cells[i, 1].StringValue;
                            var measure = sheet.Cells[i, 1].StringValue;
                            if (string.IsNullOrEmpty(name))
                            {
                                continue;
                            }
                            var entity = new EduInventoryEntity();
                            string id = Guid.NewGuid().ToString();
                            entity.CreateUserName = user.UserName;
                            entity.CreateUserId = user.UserId;
                            entity.CreateDate = date;
                            entity.ID = id;
                            entity.Name = name;
                            entity.Danger = danger;
                            entity.Measure = measure;
                            entity.EduType = type;
                            entity.DeptCode = user.DeptCode;
                            entity.UseDeptCode = arrcodes[j];
                            entity.UseDeptId = arrids[j];
                            entity.UseDeptName = arrnames[j];
                            edubll.SaveForm(entity.ID, entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }

        public ActionResult Export(string type, string name, string ids)
        {
            var user = OperatorProvider.Provider.Current();
            PostBLL pbll = new PostBLL();
            if (string.IsNullOrEmpty(name)) name = "";
            var total = 0;
            var data = edubll.GetInventoryList(null, ids, type, name, int.MaxValue, 1, out total).ToList();
            //if (type == "1" || type == "3") data = data.Where(x => x.Name.Contains(name)).ToList();
            //if (type == "2") data = data.Where(x => x.Question.Contains(name)).ToList();
            //if (!string.IsNullOrEmpty(ids)) data = edubll.GetInventoryList(type).Where(x => ids.Contains(x.ID)).ToList();
            Workbook workbook = new Workbook();
            Worksheet sheet = (Worksheet)workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            Style style1 = workbook.Styles[workbook.Styles.Add()];
            style1.Font.Color = Color.Black;
            style1.Font.Size = 11;
            style1.Font.IsBold = true;

            style1.HorizontalAlignment = TextAlignmentType.Center;
            style1.VerticalAlignment = TextAlignmentType.Center;

            Style style2 = workbook.Styles[workbook.Styles.Add()];
            style2.Font.Color = Color.Black;
            style2.Font.Size = 11;
            style2.Font.IsBold = false;
            style2.HorizontalAlignment = TextAlignmentType.Center;
            style2.VerticalAlignment = TextAlignmentType.Center;
            cells.SetColumnWidth(0, 15); cells.SetColumnWidth(2, 30);
            cells.SetColumnWidth(1, 30);
            cells.SetColumnWidth(3, 30);
            cells.SetColumnWidth(4, 30);
            cells.SetColumnWidth(5, 30);
            if (type == "2")
            {
                cells[0, 0].PutValue("试题内容"); cells[0, 0].SetStyle(style1);
                cells[0, 1].PutValue("参考答案"); cells[0, 1].SetStyle(style1);
                for (int i = 0; i < data.Count(); i++)
                {

                    var role = data[i];
                    cells[i + 1, 0].PutValue(role.Question); cells[i + 1, 0].SetStyle(style2);
                    cells[i + 1, 1].PutValue(role.Answer); cells[i + 1, 1].SetStyle(style2);
                }
            }
            if (type == "3")
            {
                cells[0, 0].PutValue("题目"); cells[0, 0].SetStyle(style1);
                cells[0, 1].PutValue("事故现象"); cells[0, 1].SetStyle(style1);
                cells[0, 2].PutValue("采取措施"); cells[0, 2].SetStyle(style1);
                for (int i = 0; i < data.Count(); i++)
                {

                    var role = data[i];
                    cells[i + 1, 0].PutValue(role.Name); cells[i + 1, 0].SetStyle(style2);
                    cells[i + 1, 1].PutValue(role.Danger); cells[i + 1, 1].SetStyle(style2);
                    cells[i + 1, 2].PutValue(role.Measure); cells[i + 1, 1].SetStyle(style2);
                }
            }

            var expname = "技术问答导出.xlsx";
            if (type == "3") expname = "事故预想导出.xlsx";

            var path = Server.MapPath("~/Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            workbook.Save(Path.Combine(path, expname), Aspose.Cells.SaveFormat.Xlsx);
            ExcelHelper.DownLoadFile(Path.Combine(path, expname), expname);
            return Success("导出成功。", new { });
        }

        // public ActionResult GetLearnTime(Pagination pagination, string from,string to) 
        // {

        //var bll = new ActivityBLL();
        //var peoplebll = new PeopleBLL();
        //Operator user = OperatorProvider.Provider.Current();
        //pagination.p_kid = "ID";
        //pagination.p_fields = "edutype,theme,bzname,createdate,appraisecontent,activitydate,flow,bzid,activityenddate,learntime,attendnumber"; //必须包含where条件所需字段
        //pagination.p_tablename = "wg_edubaseinfo ";
        //pagination.conditionJson = "1=1 and flow = '1' and bzid in(select departmentid from base_department where encode like '" + user.DeptCode + "%' and nature = '班组')";

        //int total = 0;

        //var watch = CommonHelper.TimerStart();
        //DataTable dt = edubll.GetEducationPageList(pagination, queryJson);

        //string deptid = OperatorProvider.Provider.Current().DeptCode;
        //DateTime f = DateTime.Now.AddYears(-100);
        //DateTime t = DateTime.Now.AddDays(1);
        //if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
        //if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
        //var dt = edubll.GetGroupCount(deptid, f, t);
        //dt.Columns.Add("total");
        //dt.Columns.Add("avg");
        //foreach (DataRow row in dt.Rows)
        //{
        //    decimal learns = 0;
        //    var edulist = edubll.GetAllList().Where(x => x.BZId == row[0].ToString());
        //    foreach (EduBaseInfoEntity e in edulist)
        //    {
        //        learns += Convert.ToDecimal(e.LearnTime * e.AttendNumber);
        //    }
        //    row["total"] = learns;
        //    if (row[3].ToString() == "" || row[2].ToString() == "0")
        //    {
        //        row["avg"] = "0";
        //    }
        //    else
        //    {
        //        row["avg"] = Math.Round(Convert.ToDecimal(row[3]) / Convert.ToDecimal(row[2]), 2);
        //    }
        //}
        //var JsonData = new
        //{
        //    rows = dt,
        //    total = pagination.total,
        //    page = pagination.page,
        //    records = pagination.records,
        //    costtime = CommonHelper.TimerEnd(watch)
        //};
        //return Content(JsonData.ToJson());
        // }
        public ActionResult LeartTimeCount(string from, string to)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptCode;
            //DateTime f = DateTime.Now.AddYears(-100);
            //DateTime t = DateTime.Now.AddDays(1);
            //if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            //if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            //var dt = edubll.GetGroupCount(deptid, f, t);
            //dt.Columns.Add("total");
            //dt.Columns.Add("avg");
            //foreach (DataRow row in dt.Rows)
            //{
            //    decimal learns = 0;
            //    var edulist = edubll.GetAllList().Where(x => x.BZId == row[0].ToString() && x.ActivityDate >= f && x.ActivityDate < t);
            //    foreach (EduBaseInfoEntity e in edulist)
            //    {
            //        learns += Convert.ToDecimal(e.LearnTime * e.AttendNumber);
            //    }
            //    row["total"] = learns;
            //    if (row[3].ToString() == "" || row[2].ToString() == "0")
            //    {
            //        row["avg"] = "0";
            //    }
            //    else
            //    {
            //        row["avg"] = Math.Round(Convert.ToDecimal(row[3]) / Convert.ToDecimal(row[2]), 2);
            //    }
            //}
            //ViewData["dt"] = dt;
            return View();
        }

        public ActionResult LearnCount()
        {

            return View();
        }
        public ActionResult Detail(string id)
        {
            Operator user = OperatorProvider.Provider.Current();
            var model = edubll.GetBaseInfoEntity(id);
            if (string.IsNullOrEmpty(model.AppraisePeople))
            {
                model.AppraisePeople = user.UserName;
                model.AppraisePeopleId = user.UserId;
                model.AppraiseDate = DateTime.Now;
            }
            model.Answers = edubll.GetAnswerList(model.ID).ToList();
            return View(model);
        }

        public ActionResult Form2(string id)
        {
            Operator user = OperatorProvider.Provider.Current();
            var model = edubll.GetBaseInfoEntity(id);
            if (string.IsNullOrEmpty(model.AppraisePeople))
            {
                model.AppraisePeople = user.UserName;
                model.AppraisePeopleId = user.UserId;
                model.AppraiseDate = DateTime.Now;
            }
            model.Answers = edubll.GetAnswerList(model.ID).ToList();

            model.Files2 = new FileInfoBLL().GetFilesByRecIdNew(model.ID).ToList();

            model.Files = new FileInfoBLL().GetFilesByRecIdNew(model.ID).Where(x => x.Description == "视频").ToList();
            model.Files1 = new FileInfoBLL().GetFilesByRecIdNew(model.ID).Where(x => x.Description == "照片").ToList();
            if (user.DeptCode == null) user.DeptCode = "0";
            return View(model);

        }
        public ActionResult Detail2(string id)
        {
            FileInfoBLL fileInfoBLL = new FileInfoBLL();
            Operator user = OperatorProvider.Provider.Current();
            var model = edubll.GetBaseInfoEntity(id);
            if (string.IsNullOrEmpty(model.AppraisePeople))
            {
                model.AppraisePeople = user.UserName;
                model.AppraisePeopleId = user.UserId;
                model.AppraiseDate = DateTime.Now;
            }

            model.Answers = edubll.GetAnswerList(model.ID).ToList();

            model.Files2 = fileInfoBLL.List(model.ID, null);
            if (!string.IsNullOrEmpty(model.MeetingId))
            {
                var meetingFiles = fileInfoBLL.List(model.MeetingId, new string[] { "视频" });
                if (model.Files2 != null && meetingFiles != null)
                    ((List<FileInfoEntity>)model.Files2).AddRange(meetingFiles);
            }

            model.Files = model.Files2.Where(x => x.Description == "视频").ToList();
            model.Files1 = model.Files2.Where(x => x.Description == "照片").ToList();
            if (user.DeptCode == null) user.DeptCode = "0";
            return View(model);
        }

        public ActionResult LearnDetail(string id)
        {
            PeopleBLL pb = new PeopleBLL();
            var plist = pb.GetListByDept(id);
            var edulist = edubll.GetAllList().Where(x => x.BZId == id && x.EduType != "5");
            foreach (PeopleEntity p in plist)
            {
                decimal learns = 0;
                foreach (EduBaseInfoEntity e in edulist)
                {
                    if (e.AttendPeopleId.Contains(p.ID))
                    {
                        learns += e.LearnTime;
                    }
                }
                p.Scores = learns.ToString();
            }
            ViewData["learns"] = plist;
            return View();
        }
        [HttpPost]
        public JsonResult SaveForm(string id, EduBaseInfoEntity model)
        {
            edubll.SaveEduBaseInfo(model.ID, model);

            return Json(new { success = true, message = "评价成功" });
        }


        /// <summary>
        /// 结束教育培训
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult FinshEduBaseInfo(string id, DateTime startdate, string par1, string par2, string par3, string par4, string par5, string par6, string par7, string answerData)
        {
            EduBaseInfoEntity baseEntity = edubll.GetBaseInfoEntity(id);
            TimeSpan ts = DateTime.Now - startdate;
            baseEntity.LearnTime += Convert.ToDecimal(Math.Round(ts.TotalHours, 2));
            baseEntity.Theme = par1;
            baseEntity.Teacher = par2;
            baseEntity.TeacherId = par3;
            baseEntity.RegisterPeople = par4;
            baseEntity.RegisterPeopleId = par5;
            baseEntity.RunWay = par7;
            baseEntity.NewAppraiseContent = par6;
            edubll.SaveEduBaseInfo(baseEntity.ID, baseEntity);
            edubll.FinshEduBaseInfo(id);
            #region 生成评价规则
            // 教育培训类型  EduType 1.技术讲课   2.技术问答  3.事故预想 4.反事故预想 5.新技术问答  6.新事故预想  7.拷问讲解
            ActivityBLL act = new ActivityBLL();
            //var type = string.Empty;
            //switch (baseEntity.EduType)
            //{
            //    case "1":
            //        type = "技术讲课";
            //        break;
            //    case "2":
            //        type = "技术问答";
            //        break;
            //    case "3":
            //        type = "事故预想";
            //        break;
            //    case "4":
            //        type = "反事故演习";
            //        break;
            //    case "5":
            //        type = "技术问答";
            //        break;
            //    case "6":
            //        type = "事故预想";
            //        break;
            //    case "7":
            //        type = "拷问讲解";
            //        break;
            //}
            act.setToDo(baseEntity.EduType, baseEntity.ID, baseEntity.BZId);
            #endregion

            List<EduAnswerEntity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EduAnswerEntity>>(answerData);

            edubll.UdateAnswerComment(list, baseEntity);
            return Json("true");
        }
        [HttpPost]
        public JsonResult SaveTime(string id, DateTime startdate)
        {
            EduBaseInfoEntity baseEntity = edubll.GetBaseInfoEntity(id);
            if (baseEntity.Flow != "1")
            {
                TimeSpan ts = DateTime.Now - startdate;
                baseEntity.LearnTime += Convert.ToDecimal(Math.Round(ts.TotalHours, 2));
                edubll.SaveEduBaseInfo(baseEntity.ID, baseEntity);
            }
            return Json("保存成功");
        }
        /// <summary>
        /// 获取点评数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEduCommentTag()
        {
            Operator user = OperatorProvider.Provider.Current();
            string deptId = user.DeptId;
            var list = edubll.GetCommentTagList(deptId);

            return Json(list);
        }

        public JsonResult PostEduCommentTag(FormCollection fc)
        {
            var success = true;
            var message = string.Empty;
            var entity = default(EduCommentTagEntity);
            try
            {
                var user = OperatorProvider.Provider.Current();
                var strTag = fc.Get("EduAnswerCommentTag");

                entity = new EduCommentTagEntity() { ID = Guid.NewGuid().ToString(), DeptId = user.DeptId, Tag = strTag, CreateDate = DateTime.Now };
                edubll.PostEduCommentTag(entity);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, entity });
        }

        /// <summary>
        /// 技术问答页面请求
        /// </summary>
        /// <returns></returns>
        public ActionResult SkillAnswer(string id)
        {
            var users = userbll.GetDeptUsers(OperatorProvider.Provider.Current().DeptId).ToList();
            var plist = new PeopleBLL().GetListByDept(OperatorProvider.Provider.Current().DeptId).ToList();
            //string[] property = new string[] { "Planer", "Name" };
            //bool[] sort = new bool[] { true, true };

            //plist = new IListSort<PeopleEntity>(plist, property, sort).Sort().ToList();
            ViewData["users"] = plist;
            FileInfoBLL fbll = new FileInfoBLL();
            //获取培训预约信息
            var entity = edubll.GetBaseInfoEntity(id);
            entity.Files1 = fbll.GetFilesByRecIdNew(entity.ID).Where(x => x.Description != "教育培训二维码").ToList();
            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(entity.Files1.Where(x => x.Description == "课件" || x.Description == "1").Select(x => new { x.FileId, x.FileName })); //绑定课件
            //entity.ActivityDate = DateTime.Now;
            entity.StartDate = DateTime.Now;
            return View(entity);
        }

        public ActionResult Show(string id)
        {

            var entity = edubll.GetBaseInfoEntity(id);
            ActivityBLL actBll = new ActivityBLL();
            FileInfoBLL fbll = new FileInfoBLL();
            entity.Appraises = actBll.GetEntityList().Where(x => x.Activityid == entity.ID).ToList();
            entity.Files1 = fbll.GetFilesByRecIdNew(entity.ID).Where(x => x.Description != "教育培训二维码").ToList();
            ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(entity.Files1.Where(x => x.Description == "1").Select(x => new { x.FileId, x.FileName })); //绑定课件

            // ViewData["files"] = Newtonsoft.Json.JsonConvert.SerializeObject(entity.Files1.Where(x => x.Description == "课件" || x.Description == "1").Select(x => new { x.FileId, x.FileName })); //绑定课件
            return View(entity);
        }

        /// <summary>
        /// 保存技术问答评价
        /// </summary>
        /// <param name="data">评价数据</param>
        /// <param name="eduId">教育培训ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveAnserComment(string answerData, string eduBaseData)
        {
            ////string strJson = Request.Form["data"].ToString();
            ////string strEduBaseJson = Request.Form["eduBaseData"].ToString();
            //string strJson = Request.QueryString["answerData"].ToString();
            //string strEduBaseJson = fc.Get("eduBaseData");

            EduBaseInfoEntity baseEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<EduBaseInfoEntity>(eduBaseData);
            //TimeSpan ts = DateTime.Now - baseEntity.ActivityDate.Value;
            //baseEntity.LearnTime += Convert.ToDecimal(Math.Round(ts.TotalHours, 1));
            //edubll.SaveEduBaseInfo(baseEntity.ID, baseEntity);

            List<EduAnswerEntity> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EduAnswerEntity>>(answerData);

            edubll.UdateAnswerComment(list, baseEntity);
            return Json("保存成功");
        }

        /// <summary>
        /// 随机选择人员
        /// </summary>
        /// <param name="eduId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RandomSelectAnswerUser(string eduId)
        {
            string selectUser = edubll.RandomAnswerSelectUser(eduId);

            return Json(selectUser);
        }

        public ActionResult Evaluate(string id)
        {
            ViewBag.userName = OperatorProvider.Provider.Current().UserName;
            ViewBag.userId = OperatorProvider.Provider.Current().UserId;
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Verify(string id)
        {
            ViewBag.userName = OperatorProvider.Provider.Current().UserName;
            ViewBag.userId = OperatorProvider.Provider.Current().UserId;
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public ActionResult SaveFormNew(EduBaseInfoEntity model)
        {
            var success = true;
            if (model != null)
                edubll.SaveEduBaseInfo(model.ID, model);
            return Success("操作成功", success);
        }
        [HttpPost]
        public ActionResult UpdateFormNew(EduBaseInfoEntity model)
        {
            var success = true;

            return Success("操作成功", success);
        }
        [HttpPost]
        public ActionResult SaveEvaluate(string id, ActivityEvaluateEntity model)
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
            bll.SaveEvaluate(id, null, model);

            var edu = edubll.GetBaseInfoEntity(model.Activityid);
            edu.AppraiseContent = "1";
            edubll.SaveEduBaseInfo(edu.ID, edu);
            #region
            //var type = string.Empty;
            //switch (edu.EduType)
            //{
            //    case "1":
            //        type = "技术讲课";
            //        break;
            //    case "2":
            //        type = "技术问答";
            //        break;
            //    case "3":
            //        type = "事故预想";
            //        break;
            //    case "4":
            //        type = "反事故演习";
            //        break;
            //    case "5":
            //        type = "技术问答";
            //        break;
            //    case "6":
            //        type = "事故预想";
            //        break;
            //    case "7":
            //        type = "拷问讲解";
            //        break;
            //}
            bll.NextTodo(edu.EduType, model.Activityid);
            #endregion
            var messagebll = new MessageBLL();
            messagebll.SendMessage("教育培训评价", model.ActivityEvaluateId);
            return Success("评价成功", success);
        }

        [HttpPost]
        public ActionResult SaveEduVerify(string id, EduPlanVerifyEntity model)
        {
            var success = false;
            var eduplanbll = new EduPlanBLL();
            var userName = OperatorProvider.Provider.Current().UserName;
            var userId = OperatorProvider.Provider.Current().UserId;
            model.ID = Guid.NewGuid().ToString();
            model.VerifyPerson = userName;
            model.VerifyPersonId = userId;
            model.VerifyDate = DateTime.Now;
            model.Read = "n";
            eduplanbll.SaveEduPlanVerify(string.Empty, model);

            EduPlanInfoEntity ep = eduplanbll.GetPlanInfoEntity(model.PlanId);
            if (model.VerifyResult == "0")
            {
                ep.VerifyState = "审核通过";

            }
            if (model.VerifyResult == "1")
            {
                ep.VerifyState = "审核不通过";
                ep.SubmitState = "待提交";
            }
            var messagebll = new MessageBLL();
            messagebll.SendMessage("培训计划审核", ep.ID);
            eduplanbll.SaveEduPlanInfo(ep.ID, ep);
            return Success("审核成功", success);
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



        [HttpPost]
        public JsonResult GetNewData(string from, string to)
        {
            string code = "0";
            string deptid = OperatorProvider.Provider.Current().DeptId;
            var dept = departmentBLL.GetEntity(deptid);
            if (dept != null)
            {
                if (dept.IsSpecial)
                {
                    dept = departmentBLL.GetEntity(dept.ParentId);
                }
                code = dept.EnCode;
            }
            DateTime f = DateTime.Now.AddYears(-100);
            DateTime t = DateTime.Now.AddDays(1);
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            return Json(new { rows = edubll.GetCount(code, f, t) });
        }
        [HttpPost]
        public JsonResult GetLeartTimeCount(string from, string to)
        {
            string code = OperatorProvider.Provider.Current().DeptCode;
            string deptid = OperatorProvider.Provider.Current().DeptId;
            if (deptid == "0")
            {
                code = "0";
            }
            var dept = departmentBLL.GetEntity(deptid);
            if (dept != null)
            {
                if (dept.IsSpecial)
                {
                    dept = departmentBLL.GetEntity(dept.ParentId);
                }
                code = dept.EnCode;
            }
            DateTime f = DateTime.Now.AddYears(-100);
            DateTime t = DateTime.Now.AddDays(1);
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            var dt = edubll.GetGroupCount(code, f, t);
            var dtTable = edubll.GetCountTable(code, f, t);
            //var all = edubll.GetAllList();
            string r = "";
            foreach (DataRow row in dt.Rows)
            {
                decimal learns = 0;
                var dtTableLinq = dtTable.AsEnumerable();
                var result = dtTableLinq.Where(x => x.Field<string>("departmentid") == row[0].ToString());
                //var edulist = all.Where(x => x.BZId == row[0].ToString() && x.ActivityDate >= f && x.ActivityDate < t);
                //foreach (EduBaseInfoEntity e in edulist)
                //{
                //    learns += Convert.ToDecimal(e.LearnTime * e.AttendNumber);
                //}
                foreach (DataRow rows in result)
                {
                    learns += Convert.ToDecimal(rows["learntimeandattendnumber"].ToString());
                }
                decimal avg = 0;
                if (learns.ToString() == "" || row[2].ToString() == "0")
                {

                }
                else
                {
                    avg = Math.Round(Convert.ToDecimal(learns) / Convert.ToDecimal(row[2]), 2);
                }
                r += "{" + string.Format("bzid:'{4}',bzname:'{0}',persons:'{1}',totalhours:'{2}',avghours:'{3}'", row[1], row[2], learns, avg, row[0]) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            // return Success("0", new { rows =dt.Rows  });
            return Json(new { rows = r });
        }
        [HttpPost]
        public JsonResult GetLearnCount(string from, string to)
        {
            string code = OperatorProvider.Provider.Current().DeptCode;
            string deptid = OperatorProvider.Provider.Current().DeptId;
            var dept = departmentBLL.GetEntity(deptid);

            if (dept != null)
            {
                if (dept.IsSpecial)
                {
                    dept = departmentBLL.GetEntity(dept.ParentId);
                }
                code = dept.EnCode;
            }
            if (deptid == "0")
            {
                code = "0";
            }
            DateTime f = DateTime.Now.AddYears(-100);
            DateTime t = DateTime.Now.AddDays(1);
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            var etypes = Config.GetValue("EducationType");
            string[] type = new string[] { "1", "2", "3", "4", "5" };
            if (!string.IsNullOrEmpty(etypes))
            {
                type = etypes.Split(',');
            }
            return Json(new { rows = edubll.GetLearnCount(code, f, t), types = type });
        }
        public JsonResult FindTrainings(string query, int limit)
        {
            var dt = edubll.FindTrainings(query, limit);
            return Json(dt, JsonRequestBehavior.AllowGet);
        }


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
            var entity = new DataItemBLL().GetEntityByName("教育培训类型");
            var list = new DataItemDetailBLL().GetList(entity.ItemId);
            var getcategory = list.FirstOrDefault(x => x.ItemName == category);
            if (getcategory != null)
            {
                category = getcategory.ItemValue;
            }
            DateTime f = DateTime.Now.AddYears(-100);
            DateTime t = DateTime.Now.AddDays(1);
            if (!string.IsNullOrEmpty(from)) f = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to)) t = Convert.ToDateTime(to).AddDays(1);
            var dt = edubll.GetTimeCount(code, f, t, category);
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
                // if (item.Nature == "班组")
                //{
                tree.Attribute = "Code";
                tree.AttributeValue = item.EnCode;
                // }
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


        public ActionResult Decoupling(string type)
        {
            ViewBag.from = "";
            ViewBag.to = "";
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;
            string sdt = new DateTime(DateTime.Now.Year, month, 1).ToString("yyyy-MM-dd");  //当前季度开始日期
            string edt = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(type))
            {
                ViewBag.type = "0";
            }
            else
            {
                ViewBag.type = type;
                ViewBag.from = sdt;
                ViewBag.to = edt;
            }

            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            return View();
        }
        #endregion



        #region 培训计划

        public ActionResult IndexAddEduPlan(string keyValue)
        {
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            var gettype = ditem.GetEntityByCode("edutype");
            List<DataItemDetailEntity> etype = detail.GetList(gettype.ItemId).Where(x => x.EnabledMark.HasValue).Where(x => x.EnabledMark.Value == 1 && x.ItemName != "全部" && x.ItemName != "其他").ToList();
            ViewBag.etype = etype;
            var user = OperatorProvider.Provider.Current();
            ViewBag.username = user.UserName;
            ViewBag.deptname = user.DeptName;
            ViewBag.keyValue = keyValue;
            return View();
        }
        public ActionResult Select()
        {
            return View();
        }
        public ActionResult SelectShow()
        {
            return View();
        }

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectUser(string keyvalue)
        {
            var user = OperatorProvider.Provider.Current();
            ReportBLL cbll = new ReportBLL();
            var data = cbll.GetSubmitPerson(keyvalue) as List<ItemEntity>;

            return Json(data.Where(x => x.ParentItemId == "0").Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                value = x.ItemId,
                text = x.ItemName,
                showcheck = true,
                ckselect = x.ItemType == "dept" ? "false" : "",
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildrenUser(data, x.ItemId)
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel> GetChildrenUser(List<ItemEntity> data, string id)
        {
            var user = OperatorProvider.Provider.Current();
            return data.Where(x => x.ParentItemId == id).Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                showcheck = true,
                ckselect = x.ItemType == "dept" ? "false" : "",
                value = x.ItemId,
                text = x.ItemName,
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildrenUser(data, x.ItemId)
            }).ToList();

        }
        public ActionResult SelectDept()
        {
            return View();
        }

        /// <summary>
        /// 部门
        /// </summary>
        /// <returns></returns>
        public JsonResult getDept()
        {
            var user = OperatorProvider.Provider.Current();
            DepartmentBLL deptbll = new DepartmentBLL();

            string deptid = string.Empty;
            if (user.DeptId == "0")
            {
                var dept = departmentBLL.GetRootDepartment();
                deptid = dept.DepartmentId;
            }
            else
            {
                deptid = user.DeptId;
            }
            var data = deptbll.GetSubDepartments(deptid, "厂级,部门,班组").Select(x => new ItemEntity { ItemType = x.Nature, ItemCode = x.EnCode, ItemId = x.DepartmentId, ItemName = x.FullName, ParentItemId = x.ParentId }).ToList();

            return Json(data.Where(x => x.ItemId == deptid).Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                value = x.ItemId,
                text = x.ItemName,
                code = x.ItemCode,
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
                code = x.ItemCode,
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList();

        }

        /// <summary>
        /// FileDrop组件以流的方式实现文件上传
        /// </summary>
        /// <param name="filePath">文件存储路径</param>
        /// <param name="recId">关联记录Id</param>
        /// <param name="isDate">是否按日期目录存储文件</param>
        /// <returns></returns>
        [HttpPost]
        public string PostFilePlanInfo([System.Web.Http.FromUri] string filePath, [System.Web.Http.FromUri] string recId, [System.Web.Http.FromUri] int isDate = 0)
        {
            string newFilePath = "";
            if (Request.Files.Count > 0)
            {
                foreach (string key in Request.Files.Keys)
                {
                    HttpPostedFileBase file = Request.Files[key];
                    //原始文件名
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string fileGuid = Guid.NewGuid().ToString();
                    long filesize = file.ContentLength;
                    string FileEextension = Path.GetExtension(fileName);
                    string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                    string dir = isDate == 0 ? string.Format("~/Resource/{0}", filePath) : string.Format("~/Resource/{0}/{1}", filePath, uploadDate);
                    string newFileName = fileGuid + FileEextension;
                    newFilePath = dir + "/" + newFileName;
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
                        fileInfoEntity.FileId = fileGuid;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            fileInfoEntity.FolderId = filePath;
                        }
                        else
                        {
                            fileInfoEntity.FolderId = "0";
                        }
                        fileInfoEntity.RecId = recId;
                        fileInfoEntity.FileName = fileName;
                        fileInfoEntity.FilePath = dir + "/" + newFileName;
                        fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                        fileInfoEntity.FileExtensions = FileEextension;
                        fileInfoEntity.FileType = FileEextension.TrimStart('.');
                        FileInfoBLL fileInfoBLL = new FileInfoBLL();
                        fileInfoBLL.SaveForm("", fileInfoEntity);
                    }
                }

            }
            return newFilePath;
        }
        public ActionResult DetailPlanInfo(string DetailPlanInfo)
        {
            //DetailPlanInfo ViewBag.planid = id;
            //var model = new EduPlanBLL().GetEduPlanEntity(id);
            //ViewBag.state = model.VerifyState;
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptcode = user.DeptCode;
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            var DrugLevel = ditem.GetEntityByCode("edutype");
            List<DataItemDetailEntity> dlist = detail.GetList(DrugLevel.ItemId).Where(x => x.EnabledMark.HasValue).Where(x => x.EnabledMark.Value == 1).ToList();
            ViewData["edutype"] = dlist.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName });
            return View();
        }

        public ActionResult IndexEduPlan()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            var gettype = ditem.GetEntityByCode("edutype");
            List<DataItemDetailEntity> etype = detail.GetList(gettype.ItemId).Where(x => x.EnabledMark.HasValue).Where(x => x.EnabledMark.Value == 1).ToList();
            ViewBag.etype = etype;
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            ViewBag.all = "n";
            if (dept.FullName.Contains("安监部") || dept.FullName.Contains("人力资源部"))
            {
                ViewBag.all = "y";
                ViewBag.code = "0";
            }
            return View();
        }
        public JsonResult GetEduPlanInfo()
        {
            //DataItemBLL ditem = new DataItemBLL();
            //DataItemDetailBLL detail = new DataItemDetailBLL();
            //var DrugLevel = ditem.GetEntityByCode("edutype");
            //List<DataItemDetailEntity> dlist = detail.GetList(DrugLevel.ItemId).ToList();
            var eduplanbll = new EduPlanBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var state = this.Request.QueryString.Get("state");
            var month = this.Request.QueryString.Get("month");
            var year = this.Request.QueryString.Get("year");
            var edutype = this.Request.QueryString.Get("edutype");
            var txt_Keyword = this.Request.QueryString.Get("txt_Keyword");
            var deptCode = this.Request.QueryString.Get("deptCode");
            var verifyhtml = this.Request.QueryString.Get("verifyhtml");
            var total = 0;

            var user = OperatorProvider.Provider.Current();
            //var data = eduplanbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
            //if (!string.IsNullOrEmpty(edutype))
            //{
            //    if (edutype != "0")
            //        data = data.Where(x => x.TrainType == edutype).ToList();
            //}
            //if (!string.IsNullOrEmpty(verifyhtml))
            //{
            //    data = data.Where(x => x.VerifyState == "待审核").ToList();

            //}
            //if (!string.IsNullOrEmpty(month))
            //{
            //    if (month != "0")
            //    {
            //        data = data.Where(x => x.TrainDateMonth == month).ToList();

            //    }
            //}
            //if (!string.IsNullOrEmpty(year))
            //{
            //    data = data.Where(x => x.TrainDateYear == year).ToList();
            //}
            //if (!string.IsNullOrEmpty(state))
            //{
            //    data = data.Where(x => x.workState == state).ToList();
            //}
            //if (!string.IsNullOrEmpty(txt_Keyword))
            //{
            //    data = data.Where(x => x.TrainProject == txt_Keyword).ToList();
            //}
            //if (!string.IsNullOrEmpty(deptCode))
            //{
            //    data = data.Where(x => !string.IsNullOrEmpty(x.GroupCode)).ToList();
            //    data = data.Where(x => x.GroupCode.StartsWith(deptCode)).ToList();
            //}
            //total = data.Count();
            //data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            var data = eduplanbll.GetPlanInfoList(edutype, verifyhtml, month, year, state, txt_Keyword, deptCode, page, pagesize, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEduPlanVerify(string keyValue)
        {
            var eduplanbll = new EduPlanBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;
            var user = OperatorProvider.Provider.Current();
            var data = eduplanbll.GetVerifyList(keyValue).OrderByDescending(x => x.CreateDate).ToList();
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EduPlanInfoForm(string keyValue, EduPlanInfoEntity e)
        {
            var user = OperatorProvider.Provider.Current();
            EduPlanBLL epbll = new EduPlanBLL();
            var success = false;
            e.ModifyDate = DateTime.Now;
            e.ModifyUserName = user.UserName;
            e.ModifyUserId = user.UserId;
            e.ModifyDeptId = user.DeptId;
            e.ModifyDeptName = user.DeptName;
            if (string.IsNullOrEmpty(e.ID))
            {
                e.CreateDate = DateTime.Now;
                e.CreateUser = user.UserName;
                e.CreateUserId = user.UserId;
                e.createDeptid = user.DeptId;
                e.createDeptName = user.DeptName;
                e.SubmitState = "未提交";
                e.SubmitDate = DateTime.Now;
                if (string.IsNullOrEmpty(e.workState))
                {
                    e.workState = "未完成";
                }
            }
            e.ID = keyValue;
            if (e.TrainTarget == "本班组")
            {
                var pbll = new PeopleBLL();
                var list = new List<PeopleEntity>();
                list = pbll.GetListByDept(user.DeptId).ToList();
                foreach (PeopleEntity p in list)
                {
                    e.TrainUserId += p.ID + ',';
                    e.TrainUserName += p.Name + ',';
                }
                if (e.TrainUserId.EndsWith(",")) e.TrainUserId = e.TrainUserId.Substring(0, e.TrainUserId.Length - 1);
                if (e.TrainUserName.EndsWith(",")) e.TrainUserName = e.TrainUserName.Substring(0, e.TrainUserName.Length - 1);
            }
            epbll.SaveEduPlanInfo(e.ID, e);
            return Success("操作成功", success);
        }

        public JsonResult GetEduPlanInfoDetail(string keyValue)
        {

            var epbll = new EduPlanBLL();
            FileInfoBLL fileBll = new FileInfoBLL();
            var obj = epbll.GetPlanInfoEntity(keyValue);
            obj.Files = fileBll.GetFilesByRecIdNew(keyValue);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportData()
        {

            var eduplanbll = new EduPlanBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var state = this.Request.QueryString.Get("state");
            var month = this.Request.QueryString.Get("month");
            var year = this.Request.QueryString.Get("year");
            var edutype = this.Request.QueryString.Get("edutype");
            var txt_Keyword = this.Request.QueryString.Get("txt_Keyword");
            var deptCode = this.Request.QueryString.Get("deptCode");
            var type = this.Request.QueryString.Get("type");
            var user = OperatorProvider.Provider.Current();
            var data = eduplanbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
            if (!string.IsNullOrEmpty(edutype))
            {
                if (edutype != "0")
                    data = data.Where(x => x.TrainType == edutype).ToList();
            }
            if (!string.IsNullOrEmpty(month))
            {
                data = data.Where(x => x.TrainDateMonth == month).ToList();
            }
            if (!string.IsNullOrEmpty(year))
            {
                data = data.Where(x => x.TrainDateYear == year).ToList();
            }
            if (!string.IsNullOrEmpty(state))
            {
                data = data.Where(x => x.workState == state).ToList();
            }
            if (!string.IsNullOrEmpty(txt_Keyword))
            {
                data = data.Where(x => x.TrainProject == txt_Keyword).ToList();
            }
            if (!string.IsNullOrEmpty(deptCode))
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.GroupCode)).ToList();
                data = data.Where(x => x.GroupCode.StartsWith(deptCode)).ToList();
            }
            var datalist = new List<object>();
            int i = 0;
            foreach (var item in data)
            {
                i++;
                var one = new
                {
                    xuhao = i,
                    item.TrainTypeName,
                    TrainDate = item.TrainDateYear + "年" + (string.IsNullOrEmpty(item.TrainDateMonth) ? "" : item.TrainDateMonth + "月"),
                    item.TrainHostUserName,
                    item.GroupName,
                    item.TrainUserName,
                    item.TrainProject,
                    item.TrainContent
                };
                datalist.Add(one);
            }
            //取出数据源
            var datastr = datalist.ToJson();
            int total = data.Count();
            DataTable exportTable = BSFramework.Util.Json.ToTable(datastr);
            //exportTable.Columns.Remove("");
            //exportTable.Columns[""].ColumnName = "";
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            //excelconfig.Title = "违章信息";
            //excelconfig.TitleFont = "微软雅黑";
            //excelconfig.TitlePoint = 25;
            excelconfig.HeadHeight = 50;
            excelconfig.HeadPoint = 12;
            excelconfig.Background = Color.Green;
            excelconfig.HeadFont = "宋体";
            excelconfig.FileName = "培训计划导出.xls";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            ColumnEntity columnentity = new ColumnEntity();
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "xuhao", ExcelColumn = "序号" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainTypeName", ExcelColumn = "培训类型" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainDate", ExcelColumn = "培训时间" });
            if (type != "web")
            {
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GroupName", ExcelColumn = "组织部门" });

            }
            else
            {
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainHostUserName", ExcelColumn = "组织人" });

            }
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainUserName", ExcelColumn = "培训对象" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainProject", ExcelColumn = "培训主题" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainContent", ExcelColumn = "培训内容" });
            //调用导出方法
            ExcelHelper.ExcelDownload(exportTable, excelconfig);
            return Content(null);
        }

        public ActionResult delEduPlanInfoDetail(string keyValue)
        {
            var success = false;
            var epbll = new EduPlanBLL();
            epbll.RemoveEduPlanInfo(keyValue);
            success = true;
            return Success("操作成功", success);
        }

        public ActionResult EduPlanVeriFy()
        {

            return View();
        }

        public ActionResult importPlanInfo()
        {
            var type = this.Request.QueryString.Get("type");
            ViewBag.type = type;
            return View();
        }
        /// <summary>
        /// 管理平台应急预案导入
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Importxlsx(string type)
        {
            DepartmentBLL departmentBLL = new DepartmentBLL();
            UserBLL userBLL = new UserBLL();
            var user = OperatorProvider.Provider.Current();
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var success = true;
            var message = string.Empty;
            EduPlanBLL epbll = new EduPlanBLL();
            int count = 0;
            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) throw new Exception("请上传 Excel 文件");
                //读取文件
                var book = new Workbook(this.Request.Files[0].InputStream);
                //获取第一个sheet
                var sheet = book.Worksheets[0];
                //列表实体
                var templates = new List<EduPlanInfoEntity>();
                //主表
                DateTime dtDate;
                var userList = userBLL.GetUserList();
                var deptList = departmentBLL.GetList();
                for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new EduPlanInfoEntity();
                    entity.ID = Guid.NewGuid().ToString();
                    DataItemBLL ditem = new DataItemBLL();
                    DataItemDetailBLL detail = new DataItemDetailBLL();
                    var gettype = ditem.GetEntityByCode("edutype");
                    List<DataItemDetailEntity> etype = detail.GetList(gettype.ItemId).Where(x => x.EnabledMark.HasValue).Where(x => x.EnabledMark.Value == 1).ToList();
                    entity.TrainTypeName = sheet.Cells[i, 1].StringValue;
                    if (string.IsNullOrEmpty(entity.TrainTypeName))
                    {
                        message = message + "第" + (i + 1) + "行,请填写培训类型！";
                        continue;
                    }
                    var cktype = etype.FirstOrDefault(x => x.ItemName == entity.TrainTypeName);
                    if (cktype == null)
                    {
                        message = message + "第" + (i + 1) + "行,系统不存在该类型！";
                        continue;
                    }
                    entity.TrainType = cktype.ItemValue;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserName = user.UserName;
                    entity.ModifyUserId = user.UserId;
                    entity.ModifyDeptId = user.DeptId;
                    entity.ModifyDeptName = user.DeptName;
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = user.UserName;
                    entity.CreateUserId = user.UserId;
                    entity.createDeptid = user.DeptId;
                    entity.createDeptName = user.DeptName;
                    entity.SubmitState = "未提交";
                    entity.SubmitDate = DateTime.Now;
                    var TrainDate = sheet.Cells[i, 2].StringValue;
                    if (string.IsNullOrEmpty(TrainDate))
                    {
                        message = message + "第" + (i + 1) + "行,请填写培训时间！";
                        continue;
                    }
                    if (TrainDate.Contains("年"))
                    {
                        if (TrainDate.Contains("月"))
                        {
                            var TrainDateStr = TrainDate.Replace("年", ",").Replace("月", "").Split(',');
                            entity.TrainDateYear = TrainDateStr[0];
                            entity.TrainDateMonth = TrainDateStr[1];
                        }
                        else
                        {
                            entity.TrainDateYear = TrainDate.Replace("年", "");
                        }
                    }
                    else
                    {
                        message = message + "第" + (i + 1) + "行,时间格式不正确！";
                        continue;
                    }

                    var userlist = userBLL.GetList();
                    var name = sheet.Cells[i, 3].StringValue;
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (type == "web")
                        {
                            var userStr = name.Split(',');
                            var open = false;
                            var userStrId = new List<string>();
                            for (int j = 0; j < userStr.Length; j++)
                            {
                                if (!string.IsNullOrEmpty(userStr[j]))
                                {
                                    var getUser = userlist.FirstOrDefault(x => x.RealName == userStr[j]);
                                    if (getUser == null)
                                    {
                                        message = message + "第" + (i + 1) + "行,不存在该用户！";
                                        open = true;
                                        break;
                                    }
                                    userStrId.Add(getUser.UserId);

                                }
                            }

                            if (open)
                            {
                                continue;
                            }
                            entity.TrainHostUserId = string.Join(",", userStrId);
                            entity.TrainHostUserName = name;

                        }
                        else
                        {
                            var dept = departmentBLL.GetList();
                            var getDept = dept.FirstOrDefault(x => x.FullName == name);
                            if (getDept == null)
                            {
                                message = message + "第" + (i + 1) + "行,不存在该部门！";
                                continue;
                            }
                            entity.GroupCode = getDept.EnCode;
                            entity.GroupId = getDept.DepartmentId;
                            entity.GroupName = getDept.FullName;

                        }
                    }
                    if (string.IsNullOrEmpty(entity.GroupCode))
                    {
                        entity.GroupCode = user.DeptCode;
                        entity.GroupId = user.DeptId;
                        entity.GroupName = user.DeptName;
                    }
                    var username = sheet.Cells[i, 4].StringValue;
                    if (string.IsNullOrEmpty(username))
                    {
                        message = message + "第" + (i + 1) + "行,请填写培训对象！";
                        continue;
                    }
                    var usernameopen = false;
                    var usernameStr = username.Split(',');
                    var usernameStrId = new List<string>();
                    for (int j = 0; j < usernameStr.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(usernameStr[j]))
                        {
                            var getUser = userlist.FirstOrDefault(x => x.RealName == usernameStr[j]);
                            if (getUser == null)
                            {
                                message = message + "第" + (i + 1) + "行,不存在该用户！";
                                usernameopen = true;
                                break;
                            }
                            usernameStrId.Add(getUser.UserId);

                        }
                    }
                    if (usernameopen)
                    {
                        continue;
                    }
                    entity.TrainUserId = string.Join(",", usernameStrId);
                    entity.TrainUserName = username;
                    entity.workState = "未完成";
                    entity.TrainProject = sheet.Cells[i, 5].StringValue;
                    entity.TrainContent = sheet.Cells[i, 6].StringValue;
                    epbll.SaveEduPlanInfo(entity.ID, entity);
                    count++;
                }
                message = message + "成功导入" + count + "条数据！";
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });
        }
        /// <summary>
        /// 获取审核消息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEduPlanVerifyList()
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;
            var user = OperatorProvider.Provider.Current();
            var epbll = new EduPlanBLL();
            var dataInfo = epbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
            if (!string.IsNullOrEmpty(user.DeptCode))
            {
                dataInfo = dataInfo.Where(x => x.GroupCode.StartsWith(user.DeptCode)).ToList();

            }
            var planids = dataInfo.Select(x => x.ID).ToList();
            var data = epbll.GetVerifyList("").Where(x => planids.Contains(x.PlanId)).OrderByDescending(x => x.CreateDate).ToList();
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 有参数则表示先删除后查询，否则只查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="fc"></param>
        /// <returns></returns>current
        public ActionResult IndexEduPlanWeb(string id, int page, int pagesize, FormCollection fc)
        {
            var users = OperatorProvider.Provider.Current();
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            var DrugLevel = ditem.GetEntityByCode("edutype");
            List<DataItemDetailEntity> dlist = detail.GetList(DrugLevel.ItemId).Where(x => x.EnabledMark.HasValue).Where(x => x.EnabledMark.Value == 1).ToList();
            ViewBag.edutype = dlist.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName }).ToList();
            ViewBag.deptcode = users.DeptCode;
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;
            var eduplanbll = new EduPlanBLL();
            var total = 0;
            var edtype = string.Empty;
            var state = string.Empty;
            var month = string.Empty;
            var year = string.Empty;
            if (fc.Keys.Count > 0)
            {
                edtype = fc.GetValue("edtype").AttemptedValue;
                state = fc.GetValue("state").AttemptedValue;
                month = fc.GetValue("month").AttemptedValue;
                year = fc.GetValue("year").AttemptedValue;
            }

            var data = eduplanbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
            if (!string.IsNullOrEmpty(edtype))
            {
                if (edtype != "0")
                    data = data.Where(x => x.TrainType == edtype).ToList();
            }
            if (!string.IsNullOrEmpty(state))
            {
                if (state != "全部")
                    data = data.Where(x => x.workState == state).ToList();
            }
            if (!string.IsNullOrEmpty(month))
            {
                data = data.Where(x => x.TrainDateMonth == month).ToList();
            }
            if (!string.IsNullOrEmpty(year))
            {
                data = data.Where(x => x.TrainDateYear == year).ToList();
            }
            var deptCode = users.DeptCode;
            if (!string.IsNullOrEmpty(deptCode))
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.GroupCode)).ToList();
                data = data.Where(x => x.GroupCode.StartsWith(deptCode)).ToList();
            }
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            ViewBag.year = year;
            ViewBag.month = month;
            ViewBag.edtype = edtype;
            ViewBag.state = state;
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.pagesize = pagesize;
            return View(data);
        }

        public ActionResult EditEduPlanInfo(string keyValue)
        {
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            var gettype = ditem.GetEntityByCode("edutype");
            List<DataItemDetailEntity> etype = detail.GetList(gettype.ItemId).Where(x => x.EnabledMark.HasValue).Where(x => x.EnabledMark.Value == 1 && x.ItemName != "全部" && x.ItemName != "其他").ToList();
            ViewBag.etype = etype;
            var user = OperatorProvider.Provider.Current();
            ViewBag.username = user.UserName;
            ViewBag.deptcode = user.DeptCode;
            ViewBag.deptid = user.DeptId;
            ViewBag.deptname = user.DeptName;
            ViewBag.keyValue = keyValue;
            return View();
        }

        public ActionResult VeriFyEduPlanInfo(EduPlanInfoEntity e)
        {
            return View();
        }
        public ActionResult VeriFyPlanInfo(EduPlanInfoEntity e)
        {
            var success = false;
            var epbll = new EduPlanBLL();
            var users = OperatorProvider.Provider.Current();
            var data = epbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
            var deptCode = users.DeptCode;
            if (!string.IsNullOrEmpty(deptCode))
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.GroupCode)).ToList();
                data = data.Where(x => x.GroupCode.StartsWith(deptCode)).ToList();
            }
            if (e.workState == "月度")
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateYear)).ToList();
                data = data.Where(x => x.TrainDateYear.Contains(e.TrainDateYear)).ToList();
                data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateMonth)).ToList();
                data = data.Where(x => x.TrainDateMonth.Contains(e.TrainDateMonth)).ToList();
            }
            else if (e.workState == "年度")
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateYear)).ToList();
                data = data.Where(x => x.TrainDateYear.Contains(e.TrainDateYear)).ToList();
            }
            else
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateYear)).ToList();
                data = data.Where(x => x.TrainDateMonth.Contains(e.TrainDateYear)).ToList();
                data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateMonth)).ToList();
                var ck = false;
                switch (e.TrainDateMonth)
                {
                    case "1":
                        data = data.Where(x => (x.TrainDateMonth.Contains("1") || x.TrainDateMonth.Contains("2") || x.TrainDateMonth.Contains("3"))).ToList();
                        break;
                    case "2":
                        data = data.Where(x => (x.TrainDateMonth.Contains("4") || x.TrainDateMonth.Contains("5") || x.TrainDateMonth.Contains("6"))).ToList();
                        break;
                    case "3":
                        data = data.Where(x => (x.TrainDateMonth.Contains("7") || x.TrainDateMonth.Contains("8") || x.TrainDateMonth.Contains("9"))).ToList();
                        break;
                    case "4":
                        data = data.Where(x => (x.TrainDateMonth.Contains("10") || x.TrainDateMonth.Contains("11") || x.TrainDateMonth.Contains("12"))).ToList();
                        break;
                    default:
                        ck = true;
                        break;
                }
                if (ck)
                {
                    return Success("不能存在该季度", success);
                }
            }
            foreach (var item in data)
            {
                item.SubmitState = "已提交";
                item.SubmitDate = DateTime.Now;

                item.VerifyState = "待审核";
                epbll.SaveEduPlanInfo(item.ID, item);
            }
            success = true;
            return Success("操作成功", success);
        }
        #endregion

        public JsonResult GetDetail(string id)
        {
            var data = new EducationBLL().GetDetail(id);
            if (data.Answers != null)
            {
                foreach (var item in data.Answers)
                {
                    if (item.Files != null)
                    {
                        foreach (var item1 in item.Files)
                        {
                            item1.FilePath = Url.Content(item1.FilePath);
                        }
                    }
                }
            }
            if (data.Files != null)
            {
                foreach (var item in data.Files)
                {
                    item.FilePath = Url.Content(item.FilePath);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
