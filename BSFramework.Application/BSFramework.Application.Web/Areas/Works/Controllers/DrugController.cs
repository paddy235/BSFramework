using System;
using BSFramework.Application.Code;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BSFramework.Util.WebControl;
using BSFramework.Util.Extension;
using BSFramework.Util;
using System.Data;
using System.Text;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.DrugManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.Entity.SystemManage;
using Newtonsoft.Json;
using BSFramework.Application.Busines.ToolManage;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.PublicInfoManage;
using Aspose.Cells;
using ThoughtWorks.QRCode.Codec;
using Aspose.Words;
using Aspose.Words.Drawing;
using BSFramework.Util.Offices;


namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class DrugController : MvcControllerBase
    {
        private ToolTypeBLL lbll = new ToolTypeBLL();
        DrugBLL dbll = new DrugBLL();
        DataItemBLL ditem = new DataItemBLL();
        DataItemDetailBLL detail = new DataItemDetailBLL();
        private UserBLL userbll = new UserBLL();
        FileInfoBLL fibll = new FileInfoBLL();
        /// <summary>
        /// 获取药品列表
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult List(FormCollection fc)
        {
            var users = userbll.GetDeptUsers(OperatorProvider.Provider.Current().DeptId).ToList();
            ViewData["users"] = users;
            Operator user = OperatorProvider.Provider.Current();
            //.Where(x => x.DutyName != null && x.DutyName.Contains("药品管理员"))
            List<UserEntity> userList = new UserBLL().GetDeptUsers(user.DeptId).ToList();
            //药品等级
            var DrugLevel = ditem.GetEntityByCode("DrugLevel");
            List<DataItemDetailEntity> DrugLevel_list = detail.GetList(DrugLevel.ItemId).ToList();
            //药品单位
            var DrugUnit = ditem.GetEntityByCode("DrugUnit");
            List<DataItemDetailEntity> DrugUnit_list = detail.GetList(DrugUnit.ItemId).ToList();
            //药品规格
            var DrugUSL = ditem.GetEntityByCode("DrugUSL");
            List<DataItemDetailEntity> DrugUSL_list = detail.GetList(DrugUSL.ItemId).ToList();
            //药品数量
            var DrugNum = ditem.GetEntityByCode("DrugNum");
            List<DataItemDetailEntity> DrugNum_list = detail.GetList(DrugNum.ItemId).ToList();

            ViewData["DrugNumlist"] = DrugNum_list;
            ViewData["DrugUSLlist"] = DrugUSL_list;
            ViewData["DrugLevellist"] = DrugLevel_list;
            ViewData["DrugUnitlist"] = DrugUnit_list;

            ViewData["CurrUser"] = user.UserId;
            ViewData["CurrUserName"] = user.UserName;
            ViewData["NowTime"] = DateTime.Now;
            if (userList.Count > 0)
            {
                ViewData["GuarDianId"] = userList.FirstOrDefault().UserId;
                ViewData["GuarDianName"] = userList.FirstOrDefault().RealName;
            }
            else
            {
                ViewData["GuarDianId"] = null;
                ViewData["GuarDianName"] = null;
            }


            //List<DrugEntity> list = dbll.GetList().Where(x => x.State == "1").ToList();


            //for (int i = 0; i < list.Count; i++) 
            //{
            //    if (list[i].Surplus < Convert.ToDecimal(list[i].Warn)) 
            //    {
            //        DrugEntity d = list[i];
            //        list.Remove(d);
            //        list.Insert(0, d);
            //    }
            //}
            List<DrugStockOutEntity> list = dbll.GetStockOutList(user.DeptId, "", "").Where(x => x.BZId == user.DeptId).ToList();


            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Total <= list[i].Warn)
                {
                    DrugStockOutEntity d = list[i];
                    list.Remove(d);
                    list.Insert(0, d);
                }
            }
            ViewData["DrugList"] = list;
            return View(list);
        }

        public ActionResult DrugInventoryList()
        {
            return View();
        }
        public ActionResult Index(FormCollection fc)
        {
            return View();
        }
        public ActionResult IndexAssay(FormCollection fc)
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson, string type)
        {
            var watch = CommonHelper.TimerStart();
            var data = dbll.GetPageList(pagination, queryJson, "玻璃器皿").OrderByDescending(x => x.CreateDate);
            //foreach (DrugGlassWareEntity j in data)
            //{
            //    if (j.Usetime == 0 || j.EditTime == 0)
            //    {
            //        j.Percent = 0;
            //    }
            //    else
            //    {
            //        j.Percent = Math.Round(Convert.ToDecimal(j.EditTime) / j.Usetime, 2);
            //    }
            //}
            FileInfoEntity f = new FileInfoEntity();
            foreach (DrugGlassWareEntity g in data)
            {
                g.OperateVideo = "";
                f = fibll.GetFilesByRecIdNew(g.GlassWareId).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) g.OperateVideo = f.FilePath.TrimStart('~');
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
        [HttpGet]
        public ActionResult GetPageListJsonAssay(Pagination pagination, string queryJson, string type)
        {
            var watch = CommonHelper.TimerStart();
            var data = dbll.GetPageList(pagination, queryJson, "化验仪器").OrderBy(x => x.GlassWareName);
            //foreach (DrugGlassWareEntity j in data)
            //{
            //    if (j.Usetime == 0 || j.EditTime == 0)
            //    {
            //        j.Percent = 0;
            //    }
            //    else
            //    {
            //        j.Percent = Math.Round(Convert.ToDecimal(j.EditTime) / j.Usetime, 2);
            //    }
            //}
            FileInfoEntity f = new FileInfoEntity();
            foreach (DrugGlassWareEntity g in data)
            {
                g.OperateVideo = "";
                f = fibll.GetFilesByRecIdNew(g.GlassWareId).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) g.OperateVideo = f.FilePath.TrimStart('~');
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
        public ActionResult GetDrugInventoryData(Pagination pagination, string queryJson)
        {

            Operator user = OperatorProvider.Provider.Current();
            pagination.p_kid = "ID";
            pagination.p_fields = "drugname,englishname,casno,createdate,deptcode "; //必须包含where条件所需字段
            pagination.p_tablename = "wg_druginventory";
            pagination.conditionJson = "1=1 ";

            int total = 0;

            var watch = CommonHelper.TimerStart();
            string userDept = null;
            if (!user.IsSystem) userDept = user.DeptCode;

            var dt = dbll.GetDrugPageList(userDept, pagination, queryJson);
            //dt.Columns.Add("msds");
            //dt.Columns.Add("video");
            FileInfoEntity f = new FileInfoEntity();

            foreach (var row in dt)
            {

                f = fibll.GetFilesByRecIdNew(row.Id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) row.msds = f.FilePath.TrimStart('~');
                f = fibll.GetFilesByRecIdNew(row.Id).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) row.video = f.FilePath.TrimStart('~');
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
        public ActionResult Detail(string id)
        {
            Operator user = OperatorProvider.Provider.Current();
            DrugInventoryEntity di = dbll.GetDrugInventoryEntity(id);
            var pics = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
            string msds = "";
            string video = "";
            FileInfoEntity f = new FileInfoEntity();
            f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
            if (f != null) msds = f.FilePath;
            f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
            if (f != null) video = f.FilePath;
            ViewBag.pics = pics;
            ViewBag.msds = msds;
            ViewBag.video = video;
            return View(di);
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
            DrugInventoryEntity di = new DrugInventoryEntity();
            if (!string.IsNullOrEmpty(id))
            {
                di = dbll.GetDrugInventoryEntity(id);
                var pics = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
                string msds = "";
                string video = "";
                FileInfoEntity f = new FileInfoEntity();
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) msds = f.FilePath;
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) video = f.FilePath;
                ViewBag.pics = pics;
                ViewBag.msds = msds;
                ViewBag.video = video;
            }
            else
            {
                di = new DrugInventoryEntity();
                di.Id = Guid.NewGuid().ToString();
                di.CreateDate = DateTime.Now;
                di.CreateUserId = user.UserId;
                di.CreateUserName = user.UserName;
                di.DeptCode = user.DeptCode;
                ViewBag.pics = new List<FileInfoEntity>();
                ViewBag.msds = "";
                ViewBag.video = "";
            }

            return View(di);
        }
        public ActionResult FormGlassWare(string id)
        {
            Operator user = OperatorProvider.Provider.Current();
            DrugGlassWareEntity di = new DrugGlassWareEntity();
            if (!string.IsNullOrEmpty(id))
            {
                di = dbll.GetDrugGlassWareEntity(id);
                var pics = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
                string msds = "";
                string video = "";
                FileInfoEntity f = new FileInfoEntity();
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) msds = f.FilePath;
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) video = f.FilePath;
                ViewBag.pics = pics;
                ViewBag.msds = msds;
                ViewBag.video = video;
            }
            else
            {
                di = new DrugGlassWareEntity();
                di.GlassWareId = Guid.NewGuid().ToString();
                di.CreateDate = DateTime.Now;
                di.CreateUserId = user.UserId;
                di.CreateUserName = user.UserName;
                ViewBag.pics = new List<FileInfoEntity>();
                ViewBag.msds = "";
                ViewBag.video = "";
            }

            return View(di);
        }
        public ActionResult FormAssay(string id)
        {
            Operator user = OperatorProvider.Provider.Current();
            DrugGlassWareEntity di = new DrugGlassWareEntity();
            if (!string.IsNullOrEmpty(id))
            {
                di = dbll.GetDrugGlassWareEntity(id);
                var pics = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
                string msds = "";
                string video = "";
                FileInfoEntity f = new FileInfoEntity();
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) msds = f.FilePath;
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) video = f.FilePath;
                ViewBag.pics = pics;
                ViewBag.msds = msds;
                ViewBag.video = video;
            }
            else
            {
                di = new DrugGlassWareEntity();
                di.GlassWareId = Guid.NewGuid().ToString();
                di.CreateDate = DateTime.Now;
                di.CreateUserId = user.UserId;
                di.CreateUserName = user.UserName;
                ViewBag.pics = new List<FileInfoEntity>();
                ViewBag.msds = "";
                ViewBag.video = "";
            }

            return View(di);
        }


        [HttpPost]
        public JsonResult SaveForm(string id, DrugInventoryEntity model)
        {
            model.Create();
            dbll.SaveDrugInventory(model.Id, model);

            fibll.SaveForm(this.BuildImage(model.Id));
            return Json(new { success = true, message = "操作成功" });
        }
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        public JsonResult SaveFormDrugGlassWare(string id, DrugGlassWareEntity model)
        {
            var user = OperatorProvider.Provider.Current();

            model.GlassWareType = "玻璃器皿";
            dbll.SaveDrugGlassWare(model.GlassWareId, model);
            var dlist = dbll.GetList(user.DeptId).Where(x => x.DrugInventoryId == model.GlassWareId);
            var files = fibll.GetFilesByRecIdNew(model.GlassWareId).Where(x => x.Description == "玻璃器皿二维码");
            if (files.Count() == 0)
            {
                fibll.SaveForm(this.BuildImageGlassWare(model.GlassWareId, "玻璃器皿"));
            }
            if (!string.IsNullOrEmpty(model.Img))
            {
                var files1 = fibll.GetFilesByRecIdNew(model.GlassWareId).Where(x => x.Description == "3");
                string ext = model.Img.Substring(model.Img.LastIndexOf("."));
                string name = model.Img.Substring(model.Img.LastIndexOf("/"));
                if (files1.Count() == 0)  //附件表不存在则表示为选择的图库图片，需创建数据
                {
                    var fi = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FolderId = model.GlassWareId,
                        RecId = model.GlassWareId,
                        FileName = name.Substring(1, name.Length - 1),
                        FilePath = "~/" + model.Img.Substring(model.Img.LastIndexOf("Content")),
                        FileExtensions = ext,
                        FileType = ext.Substring(1, ext.Length - 1),
                        FileSize = "",
                        DeleteMark = 0,
                        Description = "3"
                    };
                    fibll.SaveForm(fi);
                }

            }
            if (!string.IsNullOrEmpty(model.BGImg))
            {
                var files1 = fibll.GetFilesByRecIdNew(model.GlassWareId).Where(x => x.Description == "4");
                string ext = model.Img.Substring(model.Img.LastIndexOf("."));
                string name = model.Img.Substring(model.Img.LastIndexOf("/"));
                if (files1.Count() == 0)  //附件表不存在则表示为选择的图库图片，需创建数据
                {
                    var fi = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FolderId = model.GlassWareId,
                        RecId = model.GlassWareId,
                        FileName = name.Substring(1, name.Length - 1),
                        FilePath = "~/" + model.Img.Substring(model.Img.LastIndexOf("Content")),
                        FileExtensions = ext,
                        FileType = ext.Substring(1, ext.Length - 1),
                        FileSize = "",
                        DeleteMark = 0,
                        Description = "4"
                    };
                    fibll.SaveForm(fi);
                }

            }
            return Json(new { success = true, message = "操作成功" });
        }
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        public JsonResult SaveFormDrugAssay(string id, DrugGlassWareEntity model)
        {
            var user = OperatorProvider.Provider.Current();

            model.GlassWareType = "化验仪器";
            //model.BGImg = model.Img;
            dbll.SaveDrugGlassWare(model.GlassWareId, model);
            var dlist = dbll.GetList(user.DeptId).Where(x => x.DrugInventoryId == model.GlassWareId);
            var files = fibll.GetFilesByRecIdNew(model.GlassWareId).Where(x => x.Description == "化验仪器二维码");
            if (files.Count() == 0)
            {
                fibll.SaveForm(this.BuildImageGlassWare(model.GlassWareId, "化验仪器"));
            }

            return Json(new { success = true, message = "操作成功" });
        }
        [HttpPost]
        public JsonResult CheckName(string name)
        {
            bool b = false;
            var drugs = dbll.GetDrugInventoryList().Where(x => x.DrugName == name);
            if (drugs.Count() > 0)
            {
                b = true;
            }
            return Json(new { success = b, message = "操作成功" });
        }
        [HttpPost]
        public JsonResult CheckGlassWareName(string name, string type)
        {
            bool b = false;
            var drugs = dbll.GetDrugGlassWareList().Where(x => x.GlassWareName == name && x.GlassWareType == type);
            if (drugs.Count() > 0)
            {
                b = true;
            }
            return Json(new { success = b, message = "操作成功" });
        }
        public JsonResult DeleteOne(string keyValue)
        {
            DrugInventoryEntity di = dbll.GetDrugInventoryEntity(keyValue);
            dbll.DelDrugInventory(di);
            return Json(new { success = true, message = "删除成功" });
        }
        public JsonResult DeleteGlassWare(string keyValue)
        {
            DrugGlassWareEntity di = dbll.GetDrugGlassWareEntity(keyValue);
            dbll.DeleteGlassWare(di);
            return Json(new { success = true, message = "删除成功" });
        }
        public JsonResult FindTrainings(string query, int limit)
        {
            Operator user = OperatorProvider.Provider.Current();
            var data = dbll.GetDrugInventoryList().Where(x => x.DrugName.Contains(query) && (user.DeptCode.StartsWith(x.DeptCode) || x.DeptCode.StartsWith(user.DeptCode))).Take(limit);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportNew(string type)
        {
            ViewBag.type = type;
            return View();
        }
        public ActionResult MSDS(string id)
        {
            DrugInventoryEntity d = dbll.GetDrugInventoryEntity(id);
            ViewData["drugname"] = d.DrugName;
            string path = "";
            FileInfoEntity f = new FileInfoEntity();
            f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
            if (f != null) path = Url.Content(f.FilePath);
            ViewBag.path = path;

            //var path = Url.Content("~").Substring(0, @Url.Content("~").Length - 1);
            //FileInfoEntity f = new FileInfoEntity();
            //f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
            //if (f != null) path = path + f.FilePath.Substring(1, f.FilePath.Length - 1);
            //ViewBag.path = path;

            return View();
        }
        public ActionResult Video(string id)
        {
            DrugInventoryEntity d = dbll.GetDrugInventoryEntity(id);
            ViewData["drugname"] = d.DrugName;
            string path = "";
            FileInfoEntity f = new FileInfoEntity();
            f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
            if (f != null) path = f.FilePath;
            ViewBag.path = path;
            return View();
        }
        public ActionResult GlassWareVideo(string id)
        {
            DrugGlassWareEntity d = dbll.GetDrugGlassWareEntity(id);
            ViewData["glasswarename"] = d.GlassWareName;
            string path = "";
            FileInfoEntity f = new FileInfoEntity();
            f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
            if (f != null) path = f.FilePath;
            ViewBag.path = path;
            return View();
        }
        public JsonResult DoImportNew(string type)
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
                #region 导入药品
                if (type == "1")  //导入药品
                {
                    if (sheet.Cells[0, 1].StringValue != "药品名称" || sheet.Cells[0, 2].StringValue != "英文名" || sheet.Cells[0, 3].StringValue != "CAS号" || sheet.Cells[0, 4].StringValue != "分子式" || sheet.Cells[0, 5].StringValue != "分子量" || sheet.Cells[0, 6].StringValue != "性状")
                    {
                        return Json(new { success = false, message = "请使用正确的模板导入！" });
                    }
                    //var templates = new List<JobTemplateEntity>();
                    //var dtemplates = new List<DangerTemplateEntity>();
                    var date = DateTime.Now;
                    FileInfoEntity fi = new FileInfoEntity();
                    for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                        {
                            continue;
                        }
                        var entity = new DrugInventoryEntity();
                        string id = Guid.NewGuid().ToString();
                        entity.CreateUserName = user.UserName;

                        entity.DeptCode = user.DeptCode;
                        entity.CreateUserId = user.UserId;
                        entity.Id = id;
                        entity.CreateDate = DateTime.Now;
                        entity.DrugName = sheet.Cells[i, 1].StringValue;
                        var drugs = dbll.GetDrugInventoryList().Where(x => x.DrugName == entity.DrugName).ToList();
                        foreach (DrugInventoryEntity d in drugs)
                        {
                            dbll.DelDrugInventory(d);
                        }
                        entity.EnglishName = sheet.Cells[i, 2].StringValue;
                        entity.CASNO = sheet.Cells[i, 3].StringValue;
                        entity.MolecularFormula = sheet.Cells[i, 4].StringValue;
                        entity.MolecularWeight = sheet.Cells[i, 5].StringValue;
                        entity.Properties = sheet.Cells[i, 6].StringValue;
                        entity.Purpose = sheet.Cells[i, 7].StringValue;
                        entity.DangerInstruction = sheet.Cells[i, 8].StringValue;
                        entity.Measure = sheet.Cells[i, 9].StringValue;
                        entity.Danger = sheet.Cells[i, 10].StringValue;
                        entity.Position = sheet.Cells[i, 11].StringValue;
                        entity.Dispose = sheet.Cells[i, 12].StringValue;

                        string filetype = "";
                        string extension = "";
                        string virtualPath = "";
                        //MSDS
                        string msds = sheet.Cells[i, 13].StringValue;
                        if (msds != "")
                        {
                            msds = msds.TrimStart('{').TrimEnd('}');
                            filetype = msds.Substring(msds.LastIndexOf('.')).TrimStart('.');
                            extension = msds.Substring(msds.LastIndexOf('.'));
                            virtualPath = string.Format("~/Content/drugmsds/{0}", msds);
                            fi = new FileInfoEntity
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FolderId = id,
                                RecId = id,
                                FileName = msds,
                                FilePath = virtualPath,
                                FileType = filetype,
                                FileExtensions = extension,
                                FileSize = "",
                                DeleteMark = 0,
                                Description = "0"
                            };
                            fileBll.SaveForm(fi);
                        }
                        //视频文件
                        string video = sheet.Cells[i, 14].StringValue;
                        if (video != "")
                        {
                            video = video.TrimStart('{').TrimEnd('}');
                            filetype = video.Substring(video.LastIndexOf('.')).TrimStart('.');
                            extension = video.Substring(video.LastIndexOf('.'));
                            virtualPath = string.Format("~/Content/drugvideo/{0}", video);
                            fi = new FileInfoEntity
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FolderId = id,
                                RecId = id,
                                FileName = video,
                                FilePath = virtualPath,
                                FileType = filetype,
                                FileExtensions = extension,
                                FileSize = "",
                                DeleteMark = 0,
                                Description = "1"
                            };
                            fileBll.SaveForm(fi);
                        }
                        //警示标志
                        string pics = sheet.Cells[i, 15].StringValue;
                        if (pics != "")
                        {
                            string[] files = pics.Split(',');
                            foreach (string f in files)
                            {
                                string p = f.TrimStart('{').TrimEnd('}');
                                filetype = p.Substring(p.LastIndexOf('.')).TrimStart('.');
                                extension = p.Substring(p.LastIndexOf('.'));
                                virtualPath = string.Format("~/Content/drugimg/{0}", p);
                                fi = new FileInfoEntity
                                {
                                    FileId = Guid.NewGuid().ToString(),
                                    FolderId = id,
                                    RecId = id,
                                    FileName = p,
                                    FilePath = virtualPath,
                                    FileType = filetype,
                                    FileExtensions = extension,
                                    FileSize = "",
                                    DeleteMark = 0,
                                    Description = "2"
                                };
                                fileBll.SaveForm(fi);
                            }
                        }
                        dbll.SaveDrugInventory(entity.Id, entity);
                        fibll.SaveForm(this.BuildImage(entity.Id));
                    }
                }
                #endregion
                else  //2化验仪器    3玻璃器皿
                {
                    //string path = "http://" + this.Request.UserHostAddress + this.Request.ApplicationPath;
                    string glasstype = "";
                    if (type == "2")
                    {
                        glasstype = "化验仪器";
                        if (sheet.Cells[0, 1].StringValue != "仪器名称" || sheet.Cells[0, 2].StringValue != "使用方法" || sheet.Cells[0, 3].StringValue != "注意事项" || sheet.Cells[0, 4].StringValue != "背景图片" || sheet.Cells[0, 5].StringValue != "详情图片" || sheet.Cells[0, 6].StringValue != "视频")
                        {
                            return Json(new { success = false, message = "请使用正确的模板导入！" });
                        }
                    }
                    else if (type == "3")
                    {
                        glasstype = "玻璃器皿";
                        if (sheet.Cells[0, 1].StringValue != "玻璃器皿名称" || sheet.Cells[0, 2].StringValue != "使用方法" || sheet.Cells[0, 3].StringValue != "注意事项" || sheet.Cells[0, 4].StringValue != "背景图片" || sheet.Cells[0, 5].StringValue != "详情图片" || sheet.Cells[0, 6].StringValue != "视频")
                        {
                            return Json(new { success = false, message = "请使用正确的模板导入！" });
                        }
                    }
                    var date = DateTime.Now;
                    FileInfoEntity fi = new FileInfoEntity();
                    for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                        {
                            return Json(new { success = false, message = "名称不能为空！" });
                        }
                        var entity = new DrugGlassWareEntity();
                        string id = Guid.NewGuid().ToString();
                        entity.CreateUserName = user.UserName;

                        entity.GlassWareType = glasstype;
                        entity.CreateUserId = user.UserId;
                        entity.GlassWareId = id;
                        entity.CreateDate = DateTime.Now;
                        entity.GlassWareName = sheet.Cells[i, 1].StringValue;

                        //名称相同，删除原有数据

                        var drugs = dbll.GetDrugGlassWareList().Where(x => x.GlassWareName == entity.GlassWareName && x.GlassWareType == glasstype).ToList();
                        foreach (DrugGlassWareEntity d in drugs)
                        {
                            dbll.DeleteGlassWare(d);
                        }

                        entity.UseWay = sheet.Cells[i, 2].StringValue;
                        entity.Attention = sheet.Cells[i, 3].StringValue;

                        string filetype = "";
                        string extension = "";
                        string virtualPath = "";
                        //背景图
                        string pics = sheet.Cells[i, 4].StringValue;
                        if (pics != "")
                        {

                            string p = pics.TrimStart('{').TrimEnd('}');
                            filetype = p.Substring(p.LastIndexOf('.')).TrimStart('.');
                            extension = p.Substring(p.LastIndexOf('.'));
                            virtualPath = string.Format("~/Content/glassimg/{0}", p);
                            fi = new FileInfoEntity
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FolderId = id,
                                RecId = id,
                                FileName = p,
                                FilePath = virtualPath,
                                FileType = filetype,
                                FileExtensions = extension,
                                FileSize = "",
                                DeleteMark = 0,
                                Description = "4"
                            };
                            fileBll.SaveForm(fi);
                            entity.BGImg = Url.Content(fi.FilePath);
                        }
                        //详情图
                        string infopics = sheet.Cells[i, 5].StringValue;
                        if (infopics != "")
                        {
                            string p = infopics.TrimStart('{').TrimEnd('}');
                            filetype = p.Substring(p.LastIndexOf('.')).TrimStart('.');
                            extension = p.Substring(p.LastIndexOf('.'));
                            virtualPath = string.Format("~/Content/glassimg/{0}", p);
                            fi = new FileInfoEntity
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FolderId = id,
                                RecId = id,
                                FileName = p,
                                FilePath = virtualPath,
                                FileType = filetype,
                                FileExtensions = extension,
                                FileSize = "",
                                DeleteMark = 0,
                                Description = "3"
                            };
                            fileBll.SaveForm(fi);

                            entity.Img = Url.Content(fi.FilePath);
                        }
                        string viedo = sheet.Cells[i, 6].StringValue;
                        if (viedo != "")
                        {

                            string p = viedo.TrimStart('{').TrimEnd('}');
                            filetype = p.Substring(p.LastIndexOf('.')).TrimStart('.');
                            extension = p.Substring(p.LastIndexOf('.'));
                            virtualPath = string.Format("~/Content/glassvideo/{0}", p);
                            fi = new FileInfoEntity
                            {
                                FileId = Guid.NewGuid().ToString(),
                                FolderId = id,
                                RecId = id,
                                FileName = p,
                                FilePath = virtualPath,
                                FileType = filetype,
                                FileExtensions = extension,
                                FileSize = "",
                                DeleteMark = 0,
                                Description = "1"
                            };
                            fileBll.SaveForm(fi);
                        }

                        dbll.SaveDrugGlassWare(entity.GlassWareId, entity);
                        fibll.SaveForm(this.BuildImage(entity.GlassWareId));
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

        public ActionResult expword(string ids, string type)
        {
            string n = "";
            if (type == "0") n = "化学药品";
            if (type == "1") n = "化学仪器";
            if (type == "2") n = "玻璃器皿";

            string filename = Server.MapPath("~/Content/export/药品二维码导出.docx");
            var doc = new Aspose.Words.Document(filename);
            FileInfo f = new FileInfo(filename);
            bool b = f.IsReadOnly;
            f.IsReadOnly = false;

            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = new Shape(doc, ShapeType.Image);

            string[] fids = ids.TrimEnd(',').Split(',');
            //根据书签找到单元格并输出图片
            for (int i = 0; i < fids.Length; i++)
            {
                if (type == "0")
                {
                    var entity = dbll.GetDrugInventoryEntity(fids[i]);
                    var data = fibll.GetFilesByRecIdNew(fids[i]).Where(x => x.Description.Contains("二维码")).FirstOrDefault();
                    if (data == null) continue;
                    string name = Server.UrlDecode(data.FileName);//返回客户端文件名称
                    string filepath = this.Server.MapPath(data.FilePath);
                    string marks = "img";

                    if (System.IO.File.Exists(filepath))
                    {
                        marks = marks + (i + 1);
                        builder.MoveToBookmark(marks);
                        builder.InsertImage(filepath, RelativeHorizontalPosition.Margin, 1, RelativeVerticalPosition.Margin, 1, 90, 90, WrapType.Square);
                        builder.InsertHtml("<label>" + entity.DrugName + "</label>");
                    }
                }
                else
                {
                    var entity = dbll.GetDrugGlassWareEntity(fids[i]);
                    var data = fibll.GetFilesByRecIdNew(fids[i]).Where(x => x.Description.Contains("二维码")).FirstOrDefault();
                    if (data == null) continue;
                    string name = Server.UrlDecode(data.FileName);//返回客户端文件名称
                    string filepath = this.Server.MapPath(data.FilePath);
                    string marks = "img";

                    if (System.IO.File.Exists(filepath))
                    {
                        marks = marks + (i + 1);
                        builder.MoveToBookmark(marks);
                        builder.InsertImage(filepath, RelativeHorizontalPosition.Margin, 1, RelativeVerticalPosition.Margin, 1, 90, 90, WrapType.Square);
                        builder.InsertHtml("<label style='margin-left:-10px;'>" + entity.GlassWareName + "</label>");
                    }
                }

            }
            // shape.ImageData.SetImage(photo)
            var path = Server.MapPath("~/Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            doc.Save(Path.Combine(path, n + "二维码信息.doc"), Aspose.Words.SaveFormat.Doc);
            ExcelHelper.DownLoadFile(Path.Combine(path, n + "二维码信息.doc"), n + "二维码信息.doc");
            return Success("导出成功。", new { });
        }
        public ActionResult delimg(string id, string drugid)
        {
            fibll.Delete(id);
            var filelist = fibll.GetFilesByRecIdNew(drugid).Where(x => x.Description == "2").ToList();
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = f.FilePath.TrimStart('~');
            }
            return Success("上传成功。", new { files = filelist });
        }
        public ActionResult UploadFileNew(string uptype, string id)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            if (uptype != "2")  //非警示图片，新上传时，删除旧数据及实体文件
            {
                IList<FileInfoEntity> fl = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description == uptype).ToList();
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
            if (uptype == "2" || uptype == "3" || uptype == "4")
            {
                if (!type.Contains("image"))
                {
                    return Success("1");
                }
            }
            if (uptype == "0" && !type.Contains("pdf"))
            {
                return Success("1");
            }
            if (uptype == "1" && !type.Contains("mp4"))
            {
                return Success("1");
            }
            string Id = OperatorProvider.Provider.Current().UserId;
            Id = Guid.NewGuid().ToString();
            string virtualPath = string.Format("~/Content/drugfile/{0}{1}", Id, FileEextension);
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
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = uptype
            };
            fileBll.SaveForm(fi);

            var filelist = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = Url.Content(f.FilePath);
            }
            return Success("上传成功。", new { path = virtualPath.TrimStart('~'), name = fi.FileName, files = filelist, newpath = Url.Content(virtualPath) });
        }

        public ActionResult GlassList()
        {
            Operator user = OperatorProvider.Provider.Current();
            ViewData["CurrUser"] = user.UserId;
            ViewData["CurrUserName"] = user.UserName;
            ViewData["NowTime"] = DateTime.Now;
            var data = dbll.GetGlassList().Where(x => x.BZId == user.DeptId);
            return View(data);
        }

        public ActionResult AddGlass(string glassData)
        {
            Operator user = OperatorProvider.Provider.Current();
            GlassEntity g = Newtonsoft.Json.JsonConvert.DeserializeObject<GlassEntity>(glassData);
            var glist = dbll.GetGlassList().Where(x => x.Name == g.Name && x.Spec == g.Spec).ToList();
            if (glist.Count() > 0)
            {
                return Success("1");
            }

            g.ID = Guid.NewGuid().ToString();
            g.BZId = user.DeptId;
            g.CreateDate = DateTime.Now;
            g.CreateUserId = user.UserId;
            dbll.SaveGlass(string.Empty, g);

            GlassStockEntity t = new GlassStockEntity();
            t.Name = g.Name;
            t.Spec = g.Spec;
            t.Remark = user.UserName;
            t.BZId = user.DeptId;
            t.Amount = g.Amount;
            t.CreateDate = DateTime.Now;
            t.CreateUserId = user.UserId;
            t.GlassId = g.ID;
            t.CurrentNumber = int.Parse(g.Amount);
            t.ID = Guid.NewGuid().ToString();
            t.Type = "in";
            dbll.SaveGlassStock(string.Empty, t);
            return Success("0");
        }
        [HttpPost]
        public ActionResult glassin(string amount, string tid)
        {
            Operator user = OperatorProvider.Provider.Current();

            GlassEntity g = dbll.GetGlass(tid);
            g.Amount = (int.Parse(g.Amount) + int.Parse(amount)).ToString();
            dbll.SaveGlass(tid, g);

            GlassStockEntity t = new GlassStockEntity();
            t.Name = g.Name;
            t.Spec = g.Spec;
            t.Remark = user.UserName;
            t.BZId = user.DeptId;
            t.Amount = amount;
            t.CreateDate = DateTime.Now;
            t.CreateUserId = user.UserId;
            t.GlassId = tid;
            t.CurrentNumber = int.Parse(g.Amount);
            t.ID = Guid.NewGuid().ToString();
            t.Type = "in";
            dbll.SaveGlassStock(string.Empty, t);
            return Success("0");
        }

        [HttpPost]
        public ActionResult glassout(string reason, string amount, string tid)
        {
            Operator user = OperatorProvider.Provider.Current();

            GlassEntity g = dbll.GetGlass(tid);
            g.Amount = (int.Parse(g.Amount) - int.Parse(amount)).ToString();
            dbll.SaveGlass(tid, g);

            GlassStockEntity t = new GlassStockEntity();
            t.Name = g.Name;
            t.Spec = g.Spec;
            t.Remark = user.UserName;
            t.BZId = user.DeptId;
            t.Amount = amount;
            t.CreateDate = DateTime.Now;
            t.CreateUserId = user.UserId;
            t.GlassId = tid;
            t.CurrentNumber = int.Parse(g.Amount);
            t.ID = Guid.NewGuid().ToString();
            t.Type = "out";
            t.Reason = reason;
            dbll.SaveGlassStock(string.Empty, t);
            return Success("0");
        }

        [HttpPost]
        public ActionResult showglass(string id)
        {
            GlassEntity t = dbll.GetGlass(id);

            return Success("修改成功！", new { name = t.Name, path = t.Path, wran = t.Warn, spec = t.Spec, amount = t.Amount });
        }

        public ActionResult UpdateGlass(string glassData, string id)
        {
            GlassEntity t = Newtonsoft.Json.JsonConvert.DeserializeObject<GlassEntity>(glassData);
            t.ID = id;
            dbll.SaveGlass(id, t);
            return Success("0");
        }
        public ActionResult GlassStockList(int page, int pagesize, FormCollection fc)
        {
            Operator user = OperatorProvider.Provider.Current();
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;

            var from = fc.Get("from");
            var to = fc.Get("to");
            var DrugName = fc.Get("DrugName");

            var total = 0;

            var dfrom = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var dto = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);
            var data = dbll.GetGlassStockPageList(dfrom, dto, DrugName, user.DeptId, "", page, pagesize, out total);

            List<GlassEntity> glist = new List<GlassEntity>();
            foreach (GlassStockEntity gs in data)
            {
                glist.Add(dbll.GetGlass(gs.GlassId));
            }

            ViewBag.pagecount = Math.Ceiling((decimal)total / pagesize);
            ViewData["TList"] = data;
            ViewData["GList"] = glist;

            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["DrugName"] = DrugName;
            return View(data);
        }
        public ActionResult DelGlass(string id)
        {
            try
            {

                dbll.DelGlass(id);
                return Success("0");
                //}
            }
            catch
            {
                return Success("1");
            }
        }

        public ActionResult StockManage(int page, int pagesize, FormCollection fc)
        {
            Operator user = OperatorProvider.Provider.Current();
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            //.Where(x => x.DutyName != null && x.DutyName.Contains("药品管理员"))
            List<UserEntity> userList = new UserBLL().GetDeptUsers(user.DeptId).ToList();
            //药品等级
            var DrugLevel = ditem.GetEntityByCode("DrugLevel");
            List<DataItemDetailEntity> DrugLevel_list = detail.GetList(DrugLevel.ItemId).ToList();
            //药品单位
            var DrugUnit = ditem.GetEntityByCode("DrugUnit");
            List<DataItemDetailEntity> DrugUnit_list = detail.GetList(DrugUnit.ItemId).ToList();
            //药品规格
            var DrugUSL = ditem.GetEntityByCode("DrugUSL");
            List<DataItemDetailEntity> DrugUSL_list = detail.GetList(DrugUSL.ItemId).ToList();
            //药品数量
            var DrugNum = ditem.GetEntityByCode("DrugNum");
            List<DataItemDetailEntity> DrugNum_list = detail.GetList(DrugNum.ItemId).ToList();

            var DrugName = fc.Get("DrugNameS");
            ViewData["DrugNumlist"] = DrugNum_list;
            ViewData["DrugUSLlist"] = DrugUSL_list;
            ViewData["DrugLevellist"] = DrugLevel_list;
            ViewData["DrugUnitlist"] = DrugUnit_list;


            ViewData["CurrUser"] = user.UserId;
            ViewData["CurrUserName"] = user.UserName;
            ViewData["NowTime"] = DateTime.Now;
            if (userList.Count > 0)
            {
                ViewData["GuarDianId"] = userList.FirstOrDefault().UserId;
                ViewData["GuarDianName"] = userList.FirstOrDefault().RealName;
            }
            else
            {
                ViewData["GuarDianId"] = null;
                ViewData["GuarDianName"] = null;
            }

            ViewData["DrugName"] = DrugName;

            List<DrugEntity> list = dbll.GetList(user.DeptId).Where(x => x.BZId == user.DeptId).ToList();
            //  list = list.Distinct(new DrugCompareByNameAndLevel()).ToList();


            if (!string.IsNullOrEmpty(DrugName))
            {
                list = list.Where(x => x.DrugName.Contains(DrugName)).ToList();
            }
            int total = list.Count();
            list = list.Skip(pagesize * (page - 1)).Take(pagesize).ToList();

            foreach (DrugEntity d in list)
            {
                var alist = list.Where(x => x.DrugName == d.DrugName && x.DrugLevel == d.DrugLevel).ToList();
                var all = 0f;
                for (int i = 0; i < alist.Count(); i++)
                {
                    all += alist[i].DrugNum;
                }
                //d.Total = all;
                if (int.Parse(d.StockWarn) >= all)
                {
                    d.Less = "y";
                }
                else
                {
                    d.Less = "n";
                }
            }

            string[] property = new string[] { "Less", "DrugName" };
            bool[] sort = new bool[] { false, false };

            //对 List 排序
            list = new IListSort<DrugEntity>(list, property, sort).Sort().ToList();
            ViewBag.pagecount = Math.Ceiling((decimal)total / pagesize);
            ViewData["DrugList"] = list;
            return View(list);
        }
        /// <summary>
        /// 新增种类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveDrug(string id, DrugEntity model)
        {
            try
            {
                var num = model.DrugNum;
                Operator user = OperatorProvider.Provider.Current();

                var dlist = dbll.GetList(user.DeptId).Where(x => x.DrugName == model.DrugName && x.DrugLevel == model.DrugLevel && x.Spec == model.Spec && x.BZId == user.DeptId).ToList();
                if (dlist.Count() > 0)
                {
                    return Json(new { success = false, message = "该类药品已存在！" });
                }

                model.BZId = user.DeptId;
                model.State = "0";
                if (string.IsNullOrEmpty(id)) model.Id = Guid.NewGuid().ToString(); //新增种类时，需要入库，id需生成
                model.Total = model.Surplus = 0;
                model.Warn = "0";
                model.DrugNum = 0;


                DrugInventoryEntity di = new DrugInventoryEntity();
                di.Id = Guid.NewGuid().ToString();
                di.DrugName = model.DrugName;
                var dilist = dbll.GetDrugInventoryList().Where(x => x.DrugName == model.DrugName);
                if (dilist.Count() == 0)
                {
                    dbll.SaveDrugInventory(di.Id, di);
                    fibll.SaveForm(this.BuildImage(di.Id));
                }

                if (string.IsNullOrEmpty(model.DrugInventoryId))
                {
                    model.DrugInventoryId = di.Id;
                }
                dbll.SaveDrug(id, model);


                //入库
                if (num > 0)
                {
                    DrugStockEntity ds = new DrugStockEntity();
                    ds.Id = Guid.NewGuid().ToString();
                    ds.DrugId = model.Id;
                    ds.DrugName = model.DrugName;
                    ds.DrugNum = num;
                    ds.DrugUSL = Convert.ToDecimal(model.Spec);
                    ds.CreateUserId = user.UserId;
                    ds.CreateUserName = user.UserName;
                    dbll.SaveDrugStock(string.Empty, ds);
                }

                //更新所有同种类预警值
                var list = dbll.GetList(user.DeptId).Where(x => x.DrugName == model.DrugName && x.DrugLevel == model.DrugLevel).ToList();
                foreach (DrugEntity d in list)
                {
                    d.StockWarn = model.StockWarn;
                    dbll.SaveDrug(d.Id, d);
                }

                return Json(new { success = true, message = "保存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message.ToString() });
            }

        }
        /// <summary>
        /// 药品入库
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stockData"></param>
        /// <param name="location"></param>
        /// <param name="drugid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDrugStock(string id, string stockData, string location, string drugid)
        {
            var user = OperatorProvider.Provider.Current();
            try
            {
                DrugStockEntity DrugStock = Newtonsoft.Json.JsonConvert.DeserializeObject<DrugStockEntity>(stockData);
                DrugStock.BZId = user.DeptId;
                DrugStock.BZName = user.DeptName;
                DrugStock.Monitor = user.UserName;
                DrugEntity obj = dbll.GetEntity(drugid);
                obj.Location = location;
                dbll.SaveDrug(drugid, obj);
                dbll.SaveDrugStock(id, DrugStock);
                return Json(new { success = true, message = "入库成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message.ToString() });
            }
        }
        /// <summary>
        /// 药品出库
        /// </summary>
        /// <param name="id"></param>
        /// <param name="outData"></param>
        /// <param name="drugid"></param>
        /// <param name="warn"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDrugOut(string id, string outData, string drugid, string warn)
        {
            var user = OperatorProvider.Provider.Current();
            try
            {
                DrugStockEntity DrugOut = Newtonsoft.Json.JsonConvert.DeserializeObject<DrugStockEntity>(outData);
                DrugEntity obj = dbll.GetEntity(drugid);
                obj.Warn = warn;
                dbll.SaveDrug(drugid, obj);
                //更新同种类药品出库预警值
                var list = dbll.GetList(user.DeptId).Where(x => x.DrugName == obj.DrugName && x.DrugLevel == obj.DrugLevel).ToList();
                foreach (DrugEntity d in list)
                {
                    d.Warn = obj.Warn;
                    dbll.SaveDrug(d.Id, d);
                }
                DrugOut.CreateUserName = user.UserName;
                DrugOut.CreateUserId = user.UserId;
                DrugOut.BZId = user.DeptId;
                DrugOut.BZName = user.DeptName;
                DrugOut.Monitor = user.UserName;

                dbll.SaveDrugOut(user.DeptId, id, DrugOut);
                return Json(new { success = true, message = "出库成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message.ToString() });
            }
        }

        [HttpPost]
        public ActionResult SaveDrugOutNew(string id, string num)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                DrugEntity drug = dbll.GetEntity(id);

                DrugStockEntity DrugOut = new DrugStockEntity();
                DrugOut.DrugName = drug.CreateUserName;
                DrugOut.DrugLevel = drug.DrugLevel;
                DrugOut.BZId = drug.BZId;
                DrugOut.DrugId = drug.Id;
                DrugOut.CreateUserName = user.UserName;
                DrugOut.CreateUserId = user.UserId;
                DrugOut.DrugUSL = Convert.ToDecimal(drug.Spec);

                DrugOut.Type = "1";
                DrugOut.DrugNum = int.Parse(num);

                if (int.Parse(num) > drug.DrugNum)
                {
                    return Json(new { success = false, message = "出库数量不能大于库存数量！" });
                }
                dbll.SaveDrugOut(user.DeptId, id, DrugOut);
                return Json(new { success = true, message = "出库成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message.ToString() });
            }
        }
        /// <summary>
        /// 药品取用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DrugOut"></param>
        /// <returns></returns>
        public ActionResult SaveDrugOutNew1(string id, DrugOutEntity DrugOut)
        {
            try
            {
                DrugOut.BZId = OperatorProvider.Provider.Current().DeptId;
                dbll.SaveDrugOutNew(id, DrugOut);
                return Json(new { success = true, message = "取用成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message.ToString() });
            }
        }
        /// <summary>
        /// 删除药品出库信息
        /// </summary>
        /// <returns></returns>
        [HandlerMonitor(6, "删除药品")]
        public ActionResult DelDrug()
        {
            try
            {
                string id = this.Request.Form.Get("drugId");
                var deptid = OperatorProvider.Provider.Current().DeptId;
                if (dbll.DelDrug(id, deptid))
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                    return Json(new { success = false, message = "删除失败" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message.ToString() });
            }
        }

        [HandlerMonitor(6, "删除药品")]
        public ActionResult DelDrugNew()
        {
            try
            {
                string id = this.Request.Form.Get("drugId");
                var deptid = OperatorProvider.Provider.Current().DeptId;
                if (dbll.DelDrugNew(id, deptid))
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                    return Json(new { success = false, message = "删除失败" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message.ToString() });
            }
        }
        /// <summary>
        /// 入库记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult StockList(int page, int pagesize, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;

            var from = fc.Get("from");
            var to = fc.Get("to");
            var DrugName = fc.Get("DrugName");

            var total = 0;

            var dfrom = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var dto = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);

            var data = dbll.GetStockList(new string[] { user.DeptId }, dfrom, dto, DrugName, 1, 10000, out total).Where(x => x.BZId == user.DeptId).ToList();
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();

            ViewBag.pagecount = Math.Ceiling((decimal)total / pagesize);
            ViewData["TList"] = data;

            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["DrugName"] = DrugName;
            return View(data);
        }
        /// <summary>
        /// 取用记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="DrugName"></param>
        /// <param name="fc"></param>
        /// <returns></returns>

        public ActionResult OutList(int page, int pagesize, string from, string to, string DrugName, FormCollection fc)
        {
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            var user = OperatorProvider.Provider.Current();
            if (string.IsNullOrEmpty(from)) from = fc.Get("from");
            if (string.IsNullOrEmpty(to)) to = fc.Get("to");
            if (string.IsNullOrEmpty(DrugName)) DrugName = fc.Get("DrugName");

            //var from = fc.Get("from");
            //var to = fc.Get("to");
            //var DrugName = fc.Get("DrugName");

            var total = 0;

            var dfrom = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var dto = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);
            var data = dbll.GetOutList(user.DeptId, dfrom, dto, DrugName, 1, 10000, out total).Where(x => x.BZId == user.DeptId).ToList();
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();

            ViewBag.pagecount = Math.Ceiling((decimal)total / pagesize);
            ViewData["TList"] = data;

            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["DrugName"] = DrugName;
            return View(data);
        }
        /// <summary>
        /// 药品详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">0-取用界面进入  1-库存界面进入</param>
        /// <returns></returns>
        public ActionResult Show(string id, string type)
        {
            ViewData["type"] = type;
            var user = OperatorProvider.Provider.Current();
            ViewData["user"] = user.UserName;
            ViewData["userid"] = user.UserId;
            ViewData["date"] = DateTime.Now;
            if (type == "0")
            {
                var model = dbll.GetStockOutEntity(id);
                ViewData["drugname"] = model.DrugName;
                ViewData["druglevel"] = model.DrugLevel;
                ViewData["warn"] = model.Warn;
                ViewData["total"] = model.Total;
                ViewData["id"] = model.Id;
                ViewData["unit"] = model.DrugUnit;
                var druginventory = dbll.GetDrugInventoryEntity(model.DrugInventoryId);
                druginventory = druginventory == null ? new DrugInventoryEntity() : druginventory;
                ViewData["inventory"] = druginventory;

                var files = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "2").ToList();
                ViewData["pic"] = files;

                var files1 = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "化学药品二维码").ToList();
                ViewData["qr"] = files1;

                string path = "x.pdf";
                FileInfoEntity f = new FileInfoEntity();
                f = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) path = Url.Content(f.FilePath);
                ViewBag.path = path;

                var file = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "1").FirstOrDefault();
                if (file != null)
                {
                    ViewData["video"] = file.FilePath;
                }
                else
                {
                    ViewData["video"] = "x.mp4";
                }

                var drugs = dbll.GetList(user.DeptId).Where(x => x.DrugName == model.DrugName && x.DrugLevel == model.DrugLevel && x.BZId == model.BZId).ToList();
                ViewData["drugs"] = drugs;

            }
            else if (type == "1")
            {
                var model = dbll.GetEntity(id);
                ViewData["drugname"] = model.DrugName;
                ViewData["druglevel"] = model.DrugLevel;
                ViewData["stockwarn"] = model.StockWarn;
                ViewData["location"] = model.Location == null ? "" : model.Location;
                var drugs = dbll.GetList(user.DeptId).Where(x => x.DrugName == model.DrugName && x.DrugLevel == model.DrugLevel && x.BZId == model.BZId).ToList(); //库存信息
                ViewData["drugs"] = drugs;
                ViewData["id"] = model.Id;
                decimal total = 0;
                foreach (DrugEntity d in drugs)
                {
                    total += (decimal)d.DrugNum * int.Parse(d.Spec);
                }
                ViewData["total"] = total; //库存总余量

                var druginventory = dbll.GetDrugInventoryEntity(model.DrugInventoryId);
                druginventory = druginventory == null ? new DrugInventoryEntity() : druginventory;
                ViewData["inventory"] = druginventory;
                var files = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "2").ToList();
                ViewData["pic"] = files;

                var files1 = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "化学药品二维码").ToList();
                ViewData["qr"] = files1;

                string path = "x.pdf";
                FileInfoEntity f = new FileInfoEntity();
                f = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) path = Url.Content(f.FilePath);
                ViewBag.path = path;

                var file = fibll.GetFilesByRecIdNew(druginventory.Id).Where(x => x.Description == "1").FirstOrDefault();
                if (file != null)
                {
                    ViewData["video"] = file.FilePath;
                }
                else
                {
                    ViewData["video"] = "x.mp4";
                }
            }


            return View();
        }
        [HttpPost]
        public object editWarn(string id, string warn)
        {
            var model = dbll.GetGlass(id);
            model.Warn = warn;
            dbll.SaveGlass(model.ID, model);
            return Success("修改成功！");
        }
        /// <summary>
        /// 修改库存预警值  type 1:修改使用预警值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warn"></param>
        /// <returns></returns>
        [HttpPost]
        public object editDrugWarn(string id, string warn, string type)
        {
            var user = OperatorProvider.Provider.Current();

            if (type == "0")
            {
                DrugStockOutEntity d = dbll.GetStockOutEntity(id);
                d.Warn = Convert.ToDecimal(warn);
                dbll.SaveStockOut(d.Id, d);
            }
            else
            {
                DrugEntity model = dbll.GetEntity(id);
                var list = dbll.GetList(user.DeptId).Where(x => x.DrugName == model.DrugName && x.DrugLevel == model.DrugLevel).ToList();
                foreach (DrugEntity d in list)
                {

                    d.StockWarn = warn;
                    dbll.SaveDrug(d.Id, d);
                }
            }


            return Success("修改成功！");
        }

        public ActionResult GlassDetail(string id)
        {
            var model = dbll.GetGlass(id);
            return View(model);
        }
        public ActionResult GlassWareDetail(string id)
        {
            DrugGlassWareEntity di = new DrugGlassWareEntity();
            if (!string.IsNullOrEmpty(id))
            {
                di = dbll.GetDrugGlassWareEntity(id);
                var pics = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "2").ToList();
                string msds = "";
                string video = "";
                FileInfoEntity f = new FileInfoEntity();
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "0").FirstOrDefault();
                if (f != null) msds = f.FilePath;
                f = fibll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
                if (f != null) video = f.FilePath;
                ViewBag.pics = pics;
                ViewBag.msds = msds;
                ViewBag.video = video;
            }
            var model = dbll.GetDrugGlassWareEntity(id);
            return View(model);
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string id = Guid.NewGuid().ToString();
            string FileEextension = Path.GetExtension(files[0].FileName);
            string name = Path.GetFileNameWithoutExtension(files[0].FileName);
            string virtualPath = string.Format("/Content/glasspic/{0}{1}", id, FileEextension);
            string fullFileName = Server.MapPath("~" + virtualPath);

            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);

            return Success("上传成功。", new { path = virtualPath, name = name });
        }

        [HttpPost]
        public ActionResult RemoveForm(string keyValue)
        {
            var deptid = OperatorProvider.Provider.Current().DeptId;
            string r = dbll.DelDrugNew1(keyValue, deptid);

            return Success(r);
        }
        private FileInfoEntity BuildImage(string activityid)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(activityid + "|化学药品", Encoding.UTF8);
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
                Description = "化学药品二维码",
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
        private FileInfoEntity BuildImageGlassWare(string activityid, string name)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(activityid + "|" + name, Encoding.UTF8);
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
                Description = name + "二维码",
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

        public ViewResult Import()
        {
            return View();
        }

        public JsonResult DoImport()
        {
            var success = true;
            var message = string.Empty;
            var user = OperatorProvider.Provider.Current();

            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) throw new Exception("请上传 Excel 文件");

                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];
                var list = new List<DrugEntity>();
                var error = new List<string>();

                var levels = new DataItemDetailBLL().GetDataItems("药品等级");
                var units = new DataItemDetailBLL().GetDataItems("药品单位");

                for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                {

                    if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue)) error.Add($"行{i + 1}药品名称不能为空");
                    if (string.IsNullOrEmpty(sheet.Cells[i, 2].StringValue)) error.Add($"行{i + 1}等级不能为空");
                    if (string.IsNullOrEmpty(sheet.Cells[i, 3].StringValue)) error.Add($"行{i + 1}规格不能为空");
                    if (string.IsNullOrEmpty(sheet.Cells[i, 4].StringValue)) error.Add($"行{i + 1}单位不能为空");
                    if (string.IsNullOrEmpty(sheet.Cells[i, 6].StringValue)) error.Add($"行{i + 1}数量不能为空");
                    if (string.IsNullOrEmpty(sheet.Cells[i, 7].StringValue)) error.Add($"行{i + 1}单位不能为空");

                    if (error.Count == 0)
                    {
                        var item = new DrugEntity();
                        item.DrugName = sheet.Cells[i, 1].StringValue;
                        item.DrugLevelName = sheet.Cells[i, 2].StringValue;
                        item.Spec = sheet.Cells[i, 3].StringValue;
                        var unit = sheet.Cells[i, 4].StringValue;
                        item.Unit = units.Find(x => x.ItemName == unit)?.ItemValue;
                        item.Location = sheet.Cells[i, 5].StringValue;
                        item.DrugNum = sheet.Cells[i, 6].IntValue;
                        item.Unit2 = sheet.Cells[i, 7].StringValue;
                        item.StockWarn = sheet.Cells[i, 8].StringValue;
                        if (string.IsNullOrEmpty(item.StockWarn)) item.StockWarn = "0";
                        item.CreateDate = DateTime.Now;
                        item.CreateUserId = user.UserId;
                        item.CreateUserName = user.UserName;
                        item.BZId = user.DeptId;
                        item.Id = Guid.NewGuid().ToString();
                        item.DrugLevel = levels.Find(x => x.ItemName == item.DrugLevelName)?.ItemValue;
                        item.Total = (decimal)item.DrugNum * int.Parse(item.Spec);
                        item.Surplus = (decimal)item.DrugNum * int.Parse(item.Spec);
                        item.OutSurplus = 0;
                        item.Warn = "0";
                        item.State = "0";
                        item.OutNum = 0;
                        item.Used = 0;

                        list.Add(item);
                    }
                }

                if (error.Count > 0)
                    throw new Exception(string.Join(Environment.NewLine, error));


                var drugbll = new DrugBLL();
                drugbll.Import(list);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                //message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }
    }

    public class DrugCompareByNameAndLevel : IEqualityComparer<DrugEntity>
    {
        public bool Equals(DrugEntity x, DrugEntity y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (x.DrugName == y.DrugName && x.DrugLevel == y.DrugLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(DrugEntity obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.DrugName.GetHashCode() ^ obj.DrugLevel.GetHashCode();
            }
        }

    }
}
