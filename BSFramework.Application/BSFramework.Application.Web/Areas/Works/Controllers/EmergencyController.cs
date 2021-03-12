using BSFramework.Application.Busines.ToolManage;
using BSFramework.Application.Entity.ToolManage;
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
using BSFramework.Application.Busines.AttendManage;
using BSFramework.Application.Entity.AttendManage;
using BSFramework.Application.Busines.EmergencyManage;
using BSFramework.Application.Entity.EmergencyManage;
using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Busines.PublicInfoManage;
using System.Collections;
using System.Data;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Busines.WorkMeeting;
using System.Net;
using BSFramework.Application.Web.Areas.EvaluateAbout.Models;
using BSFramework.Application.Web.Areas.Works.Models;
using System.Threading.Tasks;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 描述应急
    /// </summary>
    public class EmergencyController : MvcControllerBase
    {
        //
        // GET: /Works/Emergency/
        EmergencyBLL ebll = new EmergencyBLL();
        DepartmentBLL dept = new DepartmentBLL();

        public ActionResult Index(FormCollection fc)
        {

            var user = OperatorProvider.Provider.Current();
            string deptCode = string.Empty;
            deptCode = user.DeptCode;
            var name = fc.Get("name");
            ViewData["list"] = ebll.GetIndex(name, deptCode, "");
            //ViewData["list1"] = ebll.GetList("1", name);
            //ViewData["list2"] = ebll.GetList("2", name);
            //ViewData["list3"] = ebll.GetList("3", name);
            //ViewData["list4"] = ebll.GetList("4", name);
            ViewData["name"] = name;
            return View();
        }
        #region  old
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            ViewData["Path"] = new List<string>();
            EmergencyEntity model = ebll.GetEntity(id);
            FileInfoBLL fileInfoBLL = new FileInfoBLL();

            var fileList = fileInfoBLL.GetFilesByRecIdNew(id);
            var path = Url.Content("~").Substring(0, @Url.Content("~").Length - 1);
            var filePath = path + fileList[0].FilePath.Substring(1, fileList[0].FilePath.Length - 1);
            ViewBag.path = filePath;
            model.seenum = model.seenum > 0 ? model.seenum + 1 : 1;
            ebll.SaveEmergencyEntity(model);
            return View(model);
        }
        public ActionResult saveobj(string id, string name, string path)
        {
            EmergencyEntity model = ebll.GetEntity(id);
            model.Name = name;
            if (!string.IsNullOrEmpty(model.Path))
            {
                model.Path += "," + path;
            }
            else
            {
                model.Path = path;
            }
            ebll.SaveForm(id, model);
            return Success("0");

        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            ViewData["src"] = new List<string>();
            EmergencyEntity model = ebll.GetEntity(id);
            if (model != null && model.Path != "")
            {
                ViewData["src"] = model.Path.Split(',').ToList();
            }
            return View(model);
        }
        /// <summary>
        /// delImg
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult delImg(string id, string path)
        {
            EmergencyEntity model = ebll.GetEntity(id);
            if (model != null)
            {
                var srcs = model.Path.Split(',').ToList();
                srcs.Remove(path);
                string strsrc = "";
                foreach (string src in srcs)
                {
                    strsrc += src + ',';
                }
                if (strsrc.Length > 0)
                {
                    strsrc = strsrc.Substring(0, strsrc.Length - 1);
                }
                model.Path = strsrc;
                ebll.SaveForm(model.ID, model);
                ViewData["src"] = srcs;
            }
            return Success("0");

        }
        /// <summary>
        /// 删除应急处置卡
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HandlerMonitor(6, "删除应急处置卡")]
        public ActionResult Del(string id)
        {
            try
            {
                ebll.RemoveForm(id);
                return Success("0");
            }
            catch
            {
                return Success("1");
            }
        }
        public ActionResult Add(string type, string name, string path)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                EmergencyEntity obj = new EmergencyEntity();
                obj.ID = Guid.NewGuid().ToString();
                obj.CreateDate = DateTime.Now;
                obj.BZId = user.DeptId;
                obj.BZName = user.DeptName;
                obj.Name = name;
                obj.Path = path;
                obj.Remark = type;
                ebll.SaveForm(string.Empty, obj);
                return Success("新增成功。");
            }
            catch (Exception e)
            {
                return Success("新增失败。", new { info = e.Message });
            }
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="emid"></param>
        /// <returns></returns>
        public ActionResult UploadFile(string emid)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string id = Guid.NewGuid().ToString();
            string FileEextension = Path.GetExtension(files[0].FileName);
            string[] exts = new string[] { ".bmp", ".gif", ".jpg", ".png", ".jpeg" };
            if (!exts.Contains(FileEextension.ToLower()))
            {
                return Success("上传失败。", new { code = 1 });
            }
            string name = Path.GetFileNameWithoutExtension(files[0].FileName);
            string virtualPath = string.Format("/Content/emergencyfile/{0}{1}", id, FileEextension);
            string fullFileName = Server.MapPath("~" + virtualPath);

            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            //if (!string.IsNullOrEmpty(emid))   //应急处置卡更换图片
            //{

            //    EmergencyEntity e = ebll.GetEntity(emid);
            //    if (e.Path != "")
            //    {
            //        e.Path += ',' + Request.ApplicationPath + virtualPath;
            //    }
            //    else 
            //    {
            //        e.Path += Request.ApplicationPath + virtualPath;
            //    }
            //    ebll.SaveForm(emid, e);
            //}

            return Success("上传成功。", new { code = 0, path = virtualPath, name = files[0].FileName });
        }
        #endregion
        #region new 应急处置卡
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexCard()
        {
            var user = OperatorProvider.Provider.Current();
            var useuser = new UserBLL().GetEntity(user.UserId);

            var DeptConfig = Config.GetValue("EmergencyCard");
            if (DeptConfig == user.DeptName)
            {
                ViewBag.ck = 1;
            }
            else
            {
                ViewBag.ck = 0;
            }
            if (user.UserName == "超级管理员")
            {
                ViewBag.ck = 1;
            }
            if ((useuser.RoleName.Contains("部门级用户") && useuser.RoleName.Contains("安全管理员")) || useuser.RoleName.Contains("公司管理员"))
            {
                ViewBag.ck = 1;
            }

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetCarkItems(int rows, int page, string key, string typeid)
        {
            var user = OperatorProvider.Provider.Current();
            string deptCode = string.Empty;
            deptCode = user.DeptCode;
            var total = 0;
            var data = ebll.GetCarkItems(key, typeid, rows, page, deptCode, out total);

            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllCardType()
        {
            var user = OperatorProvider.Provider.Current();
            string deptCode = string.Empty;
            deptCode = user.DeptCode;
            var data = ebll.GetAllCardType(deptCode).ToList();
            return Json(data.Where(x => x.ParentCardId == null).Select(x => new TreeModel { id = x.TypeId, value = x.TypeId, text = x.TypeName, isexpand = data.Count(y => y.ParentCardId == x.TypeId) > 0, hasChildren = data.Count(y => y.ParentCardId == x.TypeId) > 0, ChildNodes = GetChildren(data, x.TypeId) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<EmergencyCardTypeEntity> data, string id)
        {
            return data.Where(x => x.ParentCardId == id).Select(x => new TreeModel { id = x.TypeId, value = x.TypeId, text = x.TypeName, isexpand = data.Count(y => y.ParentCardId == x.TypeId) > 0, hasChildren = data.Count(y => y.ParentCardId == x.TypeId) > 0, ChildNodes = GetChildren(data, x.TypeId) }).ToList();
        }
        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditCardType(string id)
        {
            return View();
        }
        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditCardType(string id, EmergencyCardModel model)
        {

            var success = true;
            var message = "保存成功";

            try
            {
                var user = OperatorProvider.Provider.Current();
                string deptId = string.Empty;
                string deptCode = string.Empty;
                string deptName = string.Empty;
                if (user.UserName == "超级管理员")
                {
                    deptId = user.DeptId;
                    deptCode = user.DeptCode;
                    deptName = user.DeptName;
                }
                else
                {
                    var userDept = dept.GetEntity(user.DeptId);
                    var pDept = dept.GetEntity(userDept.ParentId);
                    if (pDept == null)
                    {
                        deptId = userDept.DepartmentId;
                        deptCode = userDept.EnCode;
                        deptName = userDept.FullName;
                    }
                    else
                    {
                        if (pDept.Nature == "厂级")
                        {
                            deptId = pDept.DepartmentId;
                            deptCode = pDept.EnCode;
                            deptName = pDept.FullName;
                        }
                    }
                }

                if (string.IsNullOrEmpty(id))
                {
                    var data = ebll.GetAllCardType(deptCode).ToList();
                    if (data.Count > 0)
                    {
                        var ck = data.Where(x => x.TypeName == model.CardName);
                        if (ck.Count() > 0)
                        {
                            return Json(new AjaxResult { type = false ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode("名称不能重复") });

                        }
                        ebll.AddCardType(new EmergencyCardTypeEntity() { deptcode = deptCode, deptid = deptId, deptname = deptName, TypeName = model.CardName, TypeId = model.CardId, CreateTime = DateTime.Now, ParentCardId = data.FirstOrDefault(x => x.ParentCardId == null).TypeId });

                    }
                    else
                    {
                        ebll.AddCardType(new EmergencyCardTypeEntity() { deptcode = deptCode, deptid = deptId, deptname = deptName, TypeName = model.CardName, TypeId = Guid.NewGuid().ToString(), CreateTime = DateTime.Now, ParentCardId = null });

                    }
                }
                else
                {
                    var data = ebll.GetAllCardType(deptCode).ToList();
                    var ck = data.Where(x => x.TypeName == model.CardName);
                    if (ck.Count() > 0)
                    {
                        return Json(new AjaxResult { type = false ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode("名称不能重复") });

                    }
                    ebll.EditCardType(new EmergencyCardTypeEntity() { TypeId = model.CardId, TypeName = model.CardName });
                }
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
        public JsonResult Delete(string id)
        {

            var success = true;
            var message = "删除成功";

            try
            {

                ebll.DeleteCardType(id);
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
        public JsonResult DeleteItem(string id)
        {


            var success = true;
            var message = "删除成功";

            try
            {
                var user = OperatorProvider.Provider.Current();
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                var fileList = fileInfoBLL.GetFilesByRecIdNew(id);
                for (int i = 0; i < fileList.Count; i++)
                {
                    fileInfoBLL.DeleteFile(fileList[i].RecId, fileList[i].FileName, Server.MapPath(fileList[i].FilePath));
                }
                ebll.DeleteItem(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
        /// <summary>
        /// 选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Select()
        {
            return View();
        }
        public ActionResult ImportCard()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userName = user.UserName;
            return View();
        }
        [HttpPost]
        public JsonResult ImportCardContext(string keyValue, EmergencyEntity model)
        {
            var success = true;
            var message = "新增成功";
            try
            {
                var user = OperatorProvider.Provider.Current();
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                var fileList = fileInfoBLL.GetFilesByRecIdNew(keyValue);
                for (int i = 0; i < fileList.Count; i++)
                {
                    EmergencyEntity one = new EmergencyEntity();
                    one.ID = Guid.NewGuid().ToString();
                    one.BZId = user.DeptId;
                    one.BZName = user.DeptName;
                    one.CREATEUSERID = user.UserId;
                    one.CREATEUSERNAME = user.UserName;
                    one.seenum = 0;
                    one.CreateDate = model.CreateDate;
                    one.TypeId = model.TypeId;
                    one.Name = fileList[i].FileName.Split('.')[0];
                    one.Path = fileList[i].FileName;
                    fileList[i].RecId = one.ID;
                    ebll.SaveForm("", one);
                    fileInfoBLL.SaveFormEmergency(fileList[i].FileId, fileList[i]);

                }

            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });

        }

        public ActionResult EditCard(string id)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userName = user.UserName;
            var my = ebll.GetEntity(id);
            ViewBag.name = my.Name;
            ViewBag.Id = my.ID;
            ViewBag.path = my.Path;
            return View();
        }
        [HttpPost]
        public JsonResult ImportEditCard(string keyValue, bool ck, EmergencyEntity model)
        {
            var success = true;
            var message = "修改成功";
            try
            {
                var user = OperatorProvider.Provider.Current();
                if (ck)
                {
                    FileInfoBLL fileInfoBLL = new FileInfoBLL();
                    var fileList = fileInfoBLL.GetFilesByRecIdNew(keyValue);
                    var fileListold = fileInfoBLL.GetFilesByRecIdNew(model.ID);
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        fileList[i].RecId = model.ID;
                        ebll.SaveForm(model.ID, model);
                        fileInfoBLL.DeleteFile(fileListold[i].RecId, fileListold[i].FileName, fileListold[i].FilePath);
                        fileInfoBLL.SaveFormEmergency(fileList[i].FileId, fileList[i]);
                    }
                }
                else
                {
                    var one = ebll.GetEntity(model.ID);
                    one.Name = model.Name;
                    ebll.SaveEmergencyEntity(one);

                }


            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });

        }
        /// <summary>
        /// FileDrop组件以流的方式实现文件上传
        /// </summary>
        /// <param name="filePath">文件存储路径</param>
        /// <param name="recId">关联记录Id</param>
        /// <param name="isDate">是否按日期目录存储文件</param>
        /// <returns></returns>
        [HttpPost]
        public string PostFile([System.Web.Http.FromUri]string filePath, [System.Web.Http.FromUri]string recId, [System.Web.Http.FromUri]int isDate = 0)
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
                        #region 如果是word文件，则转成pdf以供在线预览
                        if (FileEextension.ToLower().Contains("doc") || FileEextension.ToLower().Contains("docx"))
                        {
                            string pathDic = "~/Resource/EmergencyPDF/";
                            string pdfFileName = fileInfoEntity.FileName.Substring(0, fileInfoEntity.FileName.LastIndexOf(".")) + ".pdf";
                            this.DocConvertPDF(Server.MapPath(newFilePath), Server.MapPath(pathDic + fileInfoEntity.FileId + "_" + pdfFileName));
                        }
                        #endregion
                    }
                }

            }
            return newFilePath;
        }

        public JsonResult ImportEditPush(string keyvalue)
        {
            var success = true;
            var message = string.Empty;
            try
            {
                string newFilePath = "";
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                var file = this.Request.Files[0];
                string fileName = System.IO.Path.GetFileName(file.FileName);
                if (!(fileName.ToLower().Contains(".pdf") || fileName.ToLower().Contains(".doc") || fileName.ToLower().Contains(".docx") || fileName.ToLower().Contains(".jpg") || fileName.ToLower().Contains(".jpeg")))
                {
                    throw new Exception("请检查文件格式！");
                }
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = file.ContentLength;
                string FileEextension = Path.GetExtension(fileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string dir = string.Format("~/Resource/{0}/{1}", "Emergency", uploadDate);
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
                    fileInfoEntity.FolderId = "Emergency";
                    fileInfoEntity.FileId = fileGuid;
                    fileInfoEntity.RecId = keyvalue;
                    fileInfoEntity.FileName = fileName;
                    fileInfoEntity.FilePath = dir + "/" + newFileName;
                    fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                    fileInfoEntity.FileExtensions = FileEextension;
                    fileInfoEntity.FileType = FileEextension.TrimStart('.');
                    FileInfoBLL fileInfoBLL = new FileInfoBLL();
                    fileInfoBLL.SaveForm("", fileInfoEntity);

                    #region 如果是word文件，则转成pdf以供在线预览
                    if (FileEextension.ToLower().Contains("doc") || FileEextension.ToLower().Contains("docx"))
                    {
                        string pathDic = "~/Resource/EmergencyPDF/";
                        string pdfFileName = fileInfoEntity.FileName.Substring(0, fileInfoEntity.FileName.LastIndexOf(".")) + ".pdf";
                        this.DocConvertPDF(Server.MapPath(newFilePath), pathDic + Server.MapPath(fileInfoEntity.FileId + "_" + pdfFileName));
                    }
                    #endregion

                }
                message = fileInfoEntity.FileName;
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });

        }
        #endregion
        #region 终端
        #region 页面
        /// <summary>
        /// 演练第一步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillObjective(string EmergencyReportId, string EmergencyId)
        {

            var content = string.Empty;
            if (!string.IsNullOrEmpty(EmergencyId) && !string.IsNullOrEmpty(EmergencyReportId))
            {
                //不判断是或否为空  因为按业务此时数据不可能为空
                var cklist = ebll.GetEmergencyReportList(null, EmergencyReportId);
                //演练目的为空
                if (string.IsNullOrEmpty(cklist[0].purpose))
                {
                    var list = ebll.GetEmergencyWorkList(null, null, cklist[0].EmergencyId, null);
                    content = list[0].Purpose;
                }
                else
                {
                    content = cklist[0].purpose;
                }

            }
            ViewBag.content = content;
            ViewBag.EmergencyReportId = EmergencyReportId;
            ViewBag.EmergencyId = EmergencyId;
            return View();
        }
        /// <summary>
        /// 演练第二步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillScene(string EmergencyReportId, string EmergencyId)
        {

            var content = string.Empty;
            if (!string.IsNullOrEmpty(EmergencyId) && !string.IsNullOrEmpty(EmergencyReportId))
            {
                //不判断是或否为空  因为按业务此时数据不可能为空
                var cklist = ebll.GetEmergencyReportList(null, EmergencyReportId);
                //演练目的为空
                if (string.IsNullOrEmpty(cklist[0].rehearsescenario))
                {
                    var list = ebll.GetEmergencyWorkList(null, null, cklist[0].EmergencyId, null);
                    content = list[0].RehearseScenario;
                }
                else
                {
                    var list = ebll.GetEmergencyReportList(null, EmergencyReportId);
                    content = list[0].rehearsescenario;
                }

            }
            ViewBag.content = content;
            ViewBag.EmergencyReportId = EmergencyReportId;
            ViewBag.EmergencyId = EmergencyId;
            return View();
        }
        /// <summary>
        /// 演练第三步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillResponse(string EmergencyReportId, string EmergencyId)
        {
            //获取步骤数据 并进行修改
            object content = null;

            if (!string.IsNullOrEmpty(EmergencyId) && !string.IsNullOrEmpty(EmergencyReportId))
            {
                var steps = ebll.GetEmergencyReportStepsList(null, EmergencyReportId).OrderBy(x => x.EmergencySort).ToList();
                if (steps.Count == 0)
                {
                    var list = ebll.GetEmergencyStepsList(EmergencyId, null).OrderBy(x => x.EmergencySort).ToList();
                    content = list;

                }
                else
                {
                    foreach (var items in steps)
                    {
                        //演练步骤为空
                        if (string.IsNullOrEmpty(items.EmergencyUser))
                        {
                            var list = ebll.GetEmergencyStepsList(EmergencyId, null).OrderBy(x => x.EmergencySort).ToList();
                            content = list;
                            break;
                        }
                        else
                        {
                            content = steps;
                            break;

                        }
                    }
                }
            }
            UserBLL userBLL = new UserBLL();
            var user = OperatorProvider.Provider.Current();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            var bzaqy = users.FirstOrDefault(x => x.DutyName == "安全员");
            var person = users.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName }).ToList();
            //排除未参与的成员
            var personglist = ebll.GetEmergencyReportList(null, EmergencyReportId);
            var myperson = personglist[0].userperson;
            var contentlist = new List<SelectListItem>();
            foreach (var item in person)
            {
                if (myperson.Contains(item.Text))
                {
                    contentlist.Add(item);
                }
            }
            ViewBag.person = contentlist;
            ViewBag.content = content;
            ViewBag.EmergencyReportId = EmergencyReportId;
            ViewBag.EmergencyId = EmergencyId;
            return View();


        }
        /// <summary>
        /// 演练第四步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillStep(string EmergencyReportId, string EmergencyId)
        {
            //获取步骤数据 并进行修改
            object content = null;
            //第四部为肯定有reportsteps数据
            var steps = ebll.GetEmergencyReportStepsList(null, EmergencyReportId).OrderBy(x => x.EmergencySort).ToList();
            content = steps;
            ViewBag.content = content;
            ViewBag.EmergencyReportId = EmergencyReportId;
            ViewBag.EmergencyId = EmergencyId;
            return View();
        }
        /// <summary>
        /// 演练第五步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillPoints(string EmergencyReportId, string EmergencyId)
        {
            var content = string.Empty;

            if (!string.IsNullOrEmpty(EmergencyId) && !string.IsNullOrEmpty(EmergencyReportId))
            {
                //不判断是或否为空  因为按业务此时数据不可能为空
                var cklist = ebll.GetEmergencyReportList(null, EmergencyReportId);
                //演练目的为空
                if (string.IsNullOrEmpty(cklist[0].mainpoints))
                {
                    var list = ebll.GetEmergencyWorkList(null, null, cklist[0].EmergencyId, null);
                    content = list[0].MainPoints;
                }
                else
                {
                    content = cklist[0].mainpoints;
                }

            }
            ViewBag.content = content;
            ViewBag.EmergencyReportId = EmergencyReportId;
            ViewBag.EmergencyId = EmergencyId;
            return View();
        }
        /// <summary>
        /// 演练第六步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillAssess(string EmergencyReportId, string EmergencyId)
        {
            //var cklist = ebll.GetEmergencyReportList(null, EmergencyReportId);
            //ViewBag.content = content;
            var entity = ebll.GetReportEntity(EmergencyReportId);
            ViewBag.EmergencyReportId = EmergencyReportId;
            ViewBag.EmergencyId = EmergencyId;
            return View(entity);
        }

        //public ActionResult emergencyHistoryRecord(string from, string to)
        //{
        //    ViewData["from"] = from;
        //    ViewData["to"] = to;
        //    return View();
        //}
        /// <summary>
        /// 应急演练列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult emergencyHistoryRecord(int page, int pagesize, string from, string to, FormCollection fc)
        {
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            if (string.IsNullOrEmpty(from)) from = fc.Get("StartTime");
            if (string.IsNullOrEmpty(to)) to = fc.Get("EndTime");
            var name = fc.Get("name");
            if (name == null)
            {
                name = "";
            }
            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["name"] = name;
            var user = OperatorProvider.Provider.Current();

            var total = 0;
            var data = ebll.EmergencyReportGetPageList(user.DeptId, name, string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), page, pagesize, out total);
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            return View(data);
        }

        /// <summary>
        /// 应急演练详情
        /// </summary>
        /// <param name="EmergencyReportId"></param>
        /// <returns></returns>
        public ActionResult drillHistoryDetails(string EmergencyReportId)
        {
            if (string.IsNullOrEmpty(EmergencyReportId))
            {
                return View();
            }
            var entity = ebll.GetReportEntity(EmergencyReportId);
            var list = ebll.GetEmergencyReportStepsList(null, entity.EmergencyReportId).OrderBy(x => x.EmergencySort);
            ViewBag.list = list.ToList();
            return View(entity);
        }
        /// <summary>
        /// 应急预案详情
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult drillProgramme(string EmergencyId)
        {
            if (string.IsNullOrEmpty(EmergencyId))
            {
                return View();
            }
            var entity = ebll.GetEmergencyWorkList(null, null, EmergencyId, null);
            if (entity.Count() != 1)
            {
                return View();
            }
            var list = ebll.GetEmergencyStepsList(entity[0].EmergencyId, entity[0].CREATEUSERNAME).OrderBy(x => x.EmergencySort);
            var user = OperatorProvider.Provider.Current();
            DepartmentBLL departmentBLL = new DepartmentBLL();
            UserBLL userBLL = new UserBLL();
            var userList = userBLL.GetUserList();
            var deptList = departmentBLL.GetList();
            var userid = entity[0].ToCompileUserid;
            var deptId = entity[0].ToCompileDeptId;
            var CkUser = userList.FirstOrDefault(row => row.UserId == userid);
            var CkDept = deptList.FirstOrDefault(row => row.DepartmentId == deptId);
            if (CkUser == null)
            {
                return Success("不存在该用户。");
            }
            else
            {
                entity[0].ToCompileUser = CkUser.RealName;
            }
            if (CkDept == null)
            {
                return Success("不存在该部门。");
            }
            else
            {
                entity[0].ToCompileDept = CkDept.FullName;
            }
            ViewBag.list = list.ToList();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            var bzaqy = users.FirstOrDefault(x => x.DutyName == "安全员");
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("应急演练");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            //应急预案类型
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            var emergencytype = contentType.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName });
            var rehearsetype = content.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName });
            ViewBag.emergencytype = emergencytype;
            ViewBag.rehearsetype = rehearsetype;
            var userlist = users.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName });
            ViewBag.userlist = userlist;
            return View(entity[0]);
        }
        /// <summary>
        /// 应急预案
        /// </summary>
        /// <returns></returns>
        public ActionResult drillPlan(string from, string to)
        {
            var user = OperatorProvider.Provider.Current();
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("应急预案");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            content.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            contentDepart.Insert(0, new Entity.BaseManage.DepartmentEntity { DepartmentId = "全部", FullName = "全部" });
            ViewBag.Depart = contentDepart;
            ViewBag.content = content;
            ViewBag.deptName = user.DeptName;
            ViewData["from"] = from;
            ViewData["to"] = to;
            var model = ebll.getEmergencyWorkList(string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), user.DeptName == "全部" ? "" : user.DeptName, content[0].ItemName == "全部" ? "" : content[0].ItemName).ToList();
            UserBLL userBLL = new UserBLL();
            var userList = userBLL.GetUserList();
            foreach (var item in model)
            {
                var CkUser = userList.FirstOrDefault(row => row.UserId == item.ToCompileUserid);
                var CkDept = contentDepart.FirstOrDefault(row => row.DepartmentId == item.ToCompileDeptId);
                if (CkUser == null)
                {
                    return Success("不存在该用户。");
                }
                else
                {
                    item.ToCompileUser = CkUser.RealName;
                }
                if (CkDept == null)
                {
                    return Success("不存在该部门。");
                }
                else
                {
                    item.ToCompileDept = CkDept.FullName;
                }
            }

            return View(model);
        }
        /// <summary>
        /// 终端页面应急预案
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="deptName"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        public ActionResult getEmergencyWorkList(DateTime? from, DateTime? to, string deptName, string EmergencyType)
        {
            var model = ebll.getEmergencyWorkList(from, to, deptName == "全部" ? "" : deptName, EmergencyType == "全部" ? "" : EmergencyType).ToList();


            return Content(model.ToJson());
        }
        /// <summary>
        /// 演练准备
        /// </summary>
        /// <returns></returns>
        public ActionResult drillReady()
        {
            var user = OperatorProvider.Provider.Current();
            var cklist = ebll.GetEmergencyReportList(user.UserName, null);
            //是否存在未完成的演练
            foreach (var item in cklist)
            {
                //演练目的为空
                if (string.IsNullOrEmpty(item.purpose))
                {
                    return Redirect("drillObjective?EmergencyReportId=" + item.EmergencyReportId + "&&EmergencyId=" + item.EmergencyId);
                }
                //演练情景为空
                if (string.IsNullOrEmpty(item.rehearsescenario))
                {
                    return Redirect("drillScene?EmergencyReportId=" + item.EmergencyReportId + "&&EmergencyId=" + item.EmergencyId);
                }
                var steps = ebll.GetEmergencyReportStepsList(null, item.EmergencyReportId).OrderBy(x => x.EmergencySort).ToList();
                //演练步骤为空
                if (steps.Count() == 0)
                {
                    return Redirect("drillResponse?EmergencyReportId=" + item.EmergencyReportId + "&&EmergencyId=" + item.EmergencyId);
                }
                foreach (var items in steps)
                {
                    //演练步骤为空
                    if (string.IsNullOrEmpty(items.EmergencyUser))
                    {
                        return Redirect("drillResponse?EmergencyReportId=" + item.EmergencyReportId + "&&EmergencyId=" + item.EmergencyId);
                    }
                }
                //演练要点为空
                if (string.IsNullOrEmpty(item.mainpoints))
                {
                    return Redirect("drillPoints?EmergencyReportId=" + item.EmergencyReportId + "&&EmergencyId=" + item.EmergencyId);
                }
                //演练要点为空
                if (item.evaluationscore == 10)
                {
                    return Redirect("drillAssess?EmergencyReportId=" + item.EmergencyReportId + "&&EmergencyId=" + item.EmergencyId);
                }
            }
            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;
            var bzaqy = users.FirstOrDefault(x => x.DutyName == "安全员");
            //初始化默认数据
            var model = new EmergencyReportEntity()
            {
                EmergencyReportId = Guid.NewGuid().ToString(),
                planstarttime = DateTime.Now,
                emergencyplace = "班组办公室",
                chairperson = user.UserName,
                alerttype = "提前15分钟",
                Persons = string.Join(",", users.Select(x => x.RealName)),
                PersonId = string.Join(",", users.Select(x => x.UserId))
            };
            //获取班组人员  供人员选择
            model.EmergencyPersons = users.Select(x => new EmergencyPersonEntity()
            {
                EmergencyReportId = model.EmergencyReportId,
                EmergencyPersonId = Guid.NewGuid().ToString(),
                PersonId = x.UserId,
                Person = x.RealName,
                IsSigned = true
            }).ToList();
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("应急演练");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            //演练类型
            var rehearsetype = content.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName });
            //获取班组下的演练预案
            var list = ebll.GetEmergencyWorkList(user.DeptId, null, null, null);
            var emergencyName = list.Select(x => new SelectListItem() { Value = x.EmergencyId, Text = x.Name });
            var emergencyplan = list.Select(x => new SelectListItem() { Value = x.EmergencyId, Text = x.RehearseName });
            ViewBag.EmergencyReportId = model.EmergencyReportId;
            ViewBag.emergencyName = emergencyName;

            ViewBag.emergencyplan = emergencyplan;
            ViewBag.rehearsetype = rehearsetype;
            return View(model);
        }
        #endregion
        #region 提交
        /// <summary>
        /// 提交演练准备数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult drillReadyGo(string entity)
        {

            try
            {
                var emergencyReport = BSFramework.Util.Json.ToObject<EmergencyReportEntity>(entity);
                var Persons = emergencyReport.Persons;
                var PersonId = emergencyReport.PersonId;
                var PersonsSplit = Persons.Split(',');
                var PersonIdSplit = PersonId.Split(',');
                var worklist = ebll.GetEmergencyWorkList(null, null, emergencyReport.EmergencyId, null);
                var user = OperatorProvider.Provider.Current();
                emergencyReport.userperson = Persons;
                emergencyReport.deptname = user.DeptName;
                emergencyReport.deptid = user.DeptId;
                emergencyReport.emergencytype = worklist[0].EmergencyType;
                emergencyReport.emergencytypeid = worklist[0].EmergencyTypeId;
                emergencyReport.emergencyname = worklist[0].Name;
                List<EmergencyPersonEntity> list = new List<EmergencyPersonEntity>();
                for (int i = 0; i < PersonsSplit.Count(); i++)
                {
                    EmergencyPersonEntity one = new EmergencyPersonEntity();
                    one.PersonId = PersonIdSplit[i];
                    one.Person = PersonsSplit[i];
                    one.EmergencyId = emergencyReport.EmergencyId;
                    list.Add(one);
                }
                ebll.InsertEmergencyReport(emergencyReport, list);
                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new { info = e.Message });
            }


        }
        /// <summary>
        /// 提交演练步骤
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult EmergencyReportSteps(string list)
        {
            try
            {
                var entityList = list.ToList<EmergencyReportStepsEntity>();
                foreach (var item in entityList)
                {
                    item.Create();
                }
                ebll.saveReportSteps(entityList);
                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new
                {
                    info = e.Message
                });
            }
        }

        /// <summary>
        /// 提交演练目的
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult EmergencyReportPurpose(string EmergencyReportId, string Purpose)
        {
            try
            {
                ebll.updateEmergencyReport(EmergencyReportId, Purpose, null, null, false, null, null, null);

                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new
                {
                    info = e.Message
                });
            }
        }


        /// <summary>
        /// 提交演练目的
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult EmergencyReportMainPoints(string EmergencyReportId, string MainPoints)
        {
            try
            {
                ebll.updateEmergencyReport(EmergencyReportId, null, null, MainPoints, false, null, null, null);

                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new
                {
                    info = e.Message
                });
            }
        }
        /// <summary>
        /// 提交演练情景
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult EmergencyRehearsesceNario(string EmergencyReportId, string RehearsesceNario)
        {
            try
            {
                ebll.updateEmergencyReport(EmergencyReportId, null, RehearsesceNario, null, false, null, null, null);

                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new
                {
                    info = e.Message
                });
            }
        }

        /// <summary>
        /// 提交自我评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult EmergencyScore(string EmergencyReportId, string radio, string score, string effectreport, string planreport)
        {
            try
            {
                var getbool = false;
                if (radio == "是")
                {
                    getbool = true;
                }
                ebll.updateEmergencyReport(EmergencyReportId, null, null, null, getbool, score, effectreport, planreport);

                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new
                {
                    info = e.Message
                });
            }
        }
        #endregion

        #endregion
        #region 管理平台



        #region 应急预案导入
        public ActionResult Import()
        {

            return View();
        }

        /// <summary>
        /// 管理平台应急预案导入
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Importxlsx()
        {
            DepartmentBLL departmentBLL = new DepartmentBLL();
            UserBLL userBLL = new UserBLL();

            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var success = true;
            var message = string.Empty;
            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) throw new Exception("请上传 Excel 文件");
                //读取文件
                var book = new Workbook(this.Request.Files[0].InputStream);
                //获取第一个sheet
                var sheet = book.Worksheets[0];
                //列表实体
                var templates = new List<EmergencyWorkEntity>();
                var dtemplates = new List<EmergencyStepsEntity>();
                //应急主表
                DateTime dtDate;
                var userList = userBLL.GetUserList();
                var deptList = departmentBLL.GetList();
                for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new EmergencyWorkEntity();
                    entity.EmergencyId = Guid.NewGuid().ToString();
                    entity.Name = sheet.Cells[i, 1].StringValue;

                    if (!string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                    {
                        var EmergencyType = sheet.Cells[i, 2].StringValue.Trim();
                        var main = itembll.GetEntityByName("应急预案");
                        var content = itemdetialbll.GetList(main.ItemId).ToList().FirstOrDefault(x => x.ItemName == EmergencyType);
                        if (content == null)
                        {
                            throw new Exception("第" + (i + 1) + "行,系统不存在该应急预案类型！");
                        }
                        else
                        {
                            entity.EmergencyType = content.ItemName;
                            entity.EmergencyTypeId = content.ItemId;
                        }
                        var name = sheet.Cells[i, 3].StringValue.Trim();

                        var CkUser = userList.FirstOrDefault(row => row.RealName == name);

                        if (CkUser == null)
                        {
                            throw new Exception("第" + (i + 1) + "行,系统不存在该用户！");
                        }
                        else
                        {
                            entity.ToCompileUser = sheet.Cells[i, 3].StringValue;
                            entity.ToCompileUserid = CkUser.UserId;

                        }
                        var depatName = sheet.Cells[i, 4].StringValue.Trim();
                        var CkDept = deptList.FirstOrDefault(row => row.FullName == depatName);
                        if (CkDept == null)
                        {
                            throw new Exception("第" + (i + 1) + "行,系统不存在该部门！");

                        }
                        else
                        {
                            entity.ToCompileDept = CkDept.FullName;
                            entity.ToCompileDeptId = CkDept.DepartmentId;

                        }
                        if (CkDept.DepartmentId != CkUser.DepartmentId)
                        {
                            throw new Exception("人员和部门不匹配！");
                        }
                        //entity.ToCompileDate = sheet.Cells[i, 5].StringValue;

                        if (DateTime.TryParse(sheet.Cells[i, 5].StringValue, out dtDate))
                        {
                            entity.ToCompileDate = dtDate;
                        }
                        else
                        {
                            throw new Exception("第" + (i + 1) + "行,编制时间格式错误！");
                        }

                        //entity.Attachment= sheet.Cells[i, 6].StringValue;
                        if (string.IsNullOrEmpty(sheet.Cells[i, 7].StringValue))
                        {
                            throw new Exception("第" + (i + 1) + "行,演练方案名称不能为空！");
                        }
                        entity.RehearseName = sheet.Cells[i, 7].StringValue;
                        entity.EmergencyPlan = sheet.Cells[i, 7].StringValue;
                        if (string.IsNullOrEmpty(sheet.Cells[i, 8].StringValue))
                        {
                            throw new Exception("第" + (i + 1) + "行,演练目的名称不能为空！");
                        }
                        entity.Purpose = sheet.Cells[i, 8].StringValue;
                        //entity.RehearseDate = sheet.Cells[i, 9].StringValue;

                        if (!string.IsNullOrEmpty(sheet.Cells[i, 9].StringValue))
                        {
                            if (DateTime.TryParse(sheet.Cells[i, 9].StringValue, out dtDate))
                            {
                                entity.RehearseDate = dtDate;
                            }
                            else
                            {
                                throw new Exception("第" + (i + 1) + "行,演练时间格式错误！");
                            }
                        }

                        entity.RehearsePlace = sheet.Cells[i, 10].StringValue;
                        if (!string.IsNullOrEmpty(sheet.Cells[i, 11].StringValue))
                        {
                            var RehearseType = sheet.Cells[i, 11].StringValue.Trim();
                            main = itembll.GetEntityByName("应急演练");
                            content = itemdetialbll.GetList(main.ItemId).ToList().FirstOrDefault(x => x.ItemName == RehearseType);
                            if (content == null)
                            {
                                throw new Exception("第" + (i + 1) + "行,系统不存在该演练类型！");
                            }
                            else
                            {
                                entity.RehearseType = content.ItemName;
                                entity.RehearseTypeId = content.ItemId;
                            }
                        }
                        if (string.IsNullOrEmpty(sheet.Cells[i, 12].StringValue))
                        {
                            throw new Exception("第" + (i + 1) + "行,情景模拟不能为空！");
                        }
                        entity.RehearseScenario = sheet.Cells[i, 12].StringValue;
                        if (string.IsNullOrEmpty(sheet.Cells[i, 14].StringValue))
                        {
                            throw new Exception("第" + (i + 1) + "行,演练要点不能为空！");
                        }
                        entity.MainPoints = sheet.Cells[i, 14].StringValue;
                    }

                    //单元格合并 其他数据为空
                    if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                    {
                        if (templates.Count >= 1)
                        {
                            var ck = true;
                            for (int j = 1; j < 15; j++)
                            {
                                //全部是否为空
                                if (!string.IsNullOrEmpty(sheet.Cells[i, j].StringValue) && j != 13)
                                {
                                    ck = false;
                                    break;
                                }

                            }
                            if (!ck)
                            {
                                throw new Exception("第" + (i + 1) + "行,应急预案名称不能为空！");
                            }
                            entity.EmergencyId = templates[i - 2].EmergencyId;
                        }
                        else
                        {

                            throw new Exception("第" + (i + 1) + "行,应急预案名称不能为空！");

                        }

                    }
                    templates.Add(entity);
                }
                int sort = 0;
                //应急步骤表
                for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new EmergencyStepsEntity();
                    entity.EmergencyId = templates[i - 1].EmergencyId;
                    var ck = false;
                    for (int j = 1; j < 15; j++)
                    {
                        if (!string.IsNullOrEmpty(sheet.Cells[i, j].StringValue))
                        {
                            ck = true;
                            break;
                        }
                    }
                    if (ck && string.IsNullOrEmpty(sheet.Cells[i, 13].StringValue))
                    {
                        throw new Exception("第" + (i + 1) + "行,演实施步骤不能为空！");
                    }
                    entity.EmergencyContext = sheet.Cells[i, 13].StringValue;
                    entity.EmergencySort = sort;
                    dtemplates.Add(entity);
                    //如果不相同 新的应急预案
                    if (i > 1)
                    {
                        if (entity.EmergencyId != dtemplates[i - 2].EmergencyId)
                        {
                            sort = 0;
                        }
                        else
                        {
                            sort++;
                        }
                    }
                    else { sort++; }
                }
                var templatesList = new List<EmergencyWorkEntity>();
                //清理templates中垃圾数据
                foreach (var item in templates)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        templatesList.Add(item);
                    }
                }
                var dtemplatesList = new List<EmergencyStepsEntity>();
                //清理templates中垃圾数据
                foreach (var item in dtemplates)
                {
                    if (!string.IsNullOrEmpty(item.EmergencyContext))
                    {
                        dtemplatesList.Add(item);
                    }
                }
                ebll.SaveImportList(templatesList, dtemplatesList);
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });
        }
        #region 应急预案
        /// <summary>
        /// 应急预案
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult Index1()
        {
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();



            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            contentDepart.Insert(0, new Entity.BaseManage.DepartmentEntity { DepartmentId = "全部", FullName = "全部" });
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName });
            //应急预案类型
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            contentType.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            ViewData["EmergencyType"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            return View();
        }
        /// <summary>
        /// 公告表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            var id = this.Request.QueryString.Get("keyValue");
            if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            return View();
        }
        /// <summary>
        /// 删除应急预案
        /// </summary>
        /// <param name="emergencyId"></param>
        public ActionResult deleteEmergency(string emergencyId)
        {
            try
            {
                ebll.deleteEmergency(emergencyId);
                return Success("删除成功。");
            }
            catch (Exception e)
            {
                return Success("删除失败。", new { info = e.Message });
            }
        }

        #endregion
        #endregion

        #region 应急预案
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail1()
        {
            var id = this.Request.QueryString.Get("keyValue");
            //if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            //应急预案类型
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            ViewData["EmergencyType"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //演练方式
            var mainManoeuvre = itembll.GetEntityByName("应急演练");
            var contentManoeuvre = itemdetialbll.GetList(mainManoeuvre.ItemId).ToList();
            ViewData["Manoeuvre"] = contentManoeuvre.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //编制人
            var userBll = new UserBLL();
            var contentUser = userBll.GetList().ToList();
            ViewData["ToCompileUser"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName });
            //编制人对应部门
            ViewData["UserIdDeptId"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.DepartmentId });
            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName });
            return View();
        }
        /// <summary>
        /// 应急预案
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form1()
        {
            var id = this.Request.QueryString.Get("keyValue");
            //if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            var user = OperatorProvider.Provider.Current();
            if (user.UserId != "System" && string.IsNullOrEmpty(id))
            {
                ViewBag.UserId = user.UserId;
                ViewBag.DeptId = user.DeptId;
            }
            //应急预案类型
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            ViewData["EmergencyType"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //演练方式
            var mainManoeuvre = itembll.GetEntityByName("应急演练");
            var contentManoeuvre = itemdetialbll.GetList(mainManoeuvre.ItemId).ToList();
            ViewData["Manoeuvre"] = contentManoeuvre.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //编制人
            var userBll = new UserBLL();
            var contentUser = userBll.GetList().Where(x => x.UserId != "System").ToList();
            ViewData["ToCompileUser"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName });
            //编制人对应部门
            ViewData["UserIdDeptId"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.DepartmentId });
            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().Where(x => x.DepartmentId != "0").ToList();
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName });

            return View();

        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="workEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        [HandlerMonitor(5, "保存应急预案(新增、修改)")]
        public ActionResult SaveForm(string keyValue, string FilePath, string FileExtensions, EmergencyWorkEntity workEntity)
        {
            var user = OperatorProvider.Provider.Current();
            FileInfoBLL fibll = new FileInfoBLL();
            var fie = new FileInfoEntity();
            var fieEntity = fibll.GetEntity(workEntity.AttachmentId);
            fie.FileId = Guid.NewGuid().ToString();
            fie.FileName = workEntity.Attachment;
            if (fieEntity != null && (string.IsNullOrEmpty(FilePath) || FilePath == ""))
            {
                fie.FilePath = fieEntity.FilePath;
            }
            else
            {
                fie.FilePath = FilePath;
            }
            fie.FileExtensions = FileExtensions;
            fie.FileType = FileExtensions.Replace(".", "");
            fie.RecId = keyValue;
            fibll.SaveFormEmergency(workEntity.AttachmentId, fie);
            workEntity.AttachmentId = fie.FileId;
            workEntity.CREATEDATE = workEntity.MODIFYDATE = DateTime.Now;
            workEntity.CREATEUSERID = workEntity.MODIFYUSERID = user.UserId;
            workEntity.CREATEUSERNAME = workEntity.MODIFYUSERNAME = user.UserName;
            ebll.SaveFormWork(keyValue, workEntity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetData(string name, int rows)
        {

            var user = OperatorProvider.Provider.Current();
            var bll = new EmergencyBLL();


            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var ToCompileDeptIdSearch = this.Request.QueryString.Get("ToCompileDeptIdSearch");
            var EmergencyTypeSearch = this.Request.QueryString.Get("EmergencyTypeSearch");
            var total = 0;
            //var data = bll.GetData(no, person, level, category, string.IsNullOrEmpty(state) ? "全部" : state, pagesize, page, out total);


            var data = bll.GetEvaluations(user.DeptId, name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, out total);

            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var bll = new EmergencyBLL();
            var data = bll.GetWorkEntity(keyValue);
            FileInfoBLL fi = new FileInfoBLL();
            IList list = fi.GetFilesByRecId(keyValue);
            return ToJsonResult(new { formData = data, files = list });
        }
        public JsonResult DoImport()
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

                //workmeetingbll.AddJobTemplates(templates);
                //var path = "~/Resource/Emergency/";
                string userId = OperatorProvider.Provider.Current().UserId;
                string fileGuid = Guid.NewGuid().ToString();
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                var file = this.Request.Files[0];
                filename = file.FileName;
                string FileEextension = Path.GetExtension(file.FileName);
                string virtualPath = string.Format("~/Resource/Emergency/{0}/{1}/{2}{3}", userId, uploadDate, fileGuid, FileEextension);
                string fullFileName = this.Server.MapPath(virtualPath);

                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);

                file.SaveAs(fullFileName);
                fie.FileId = Guid.NewGuid().ToString();
                fie.FileName = file.FileName;
                fie.FilePath = virtualPath;
                fie.FileExtensions = FileEextension;
                fie.FileType = FileEextension.Replace(".", "");
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message, filename, fie });
        }
        /// <summary>
        /// 删除应急预案
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除应急预案")]
        public ActionResult DelEmergency(string keyValue)
        {
            var bll = new EmergencyBLL();
            bll.DelEmergency(keyValue);
            return Success("删除成功。");
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="filename"></param>
        /// <param name="recId"></param>
        public void DownloadFile(string keyValue, string filename, string recId)
        {
            var fileFolderBLL = new FileFolderBLL();
            var fileInfoBLL = new FileInfoBLL();
            if (string.IsNullOrEmpty(keyValue)) { return; }
            FileInfoEntity data = null;
            if (!string.IsNullOrEmpty(filename))
            {
                data = fileInfoBLL.GetEntity(recId, Server.UrlDecode(filename));
            }
            else
            {
                data = fileInfoBLL.GetEntity(keyValue);
            }
            if (data != null)
            {
                string name = string.IsNullOrEmpty(filename) ? Server.UrlDecode(data.FileName) : Server.UrlDecode(filename);//返回客户端文件名称
                string filepath = this.Server.MapPath(data.FilePath);
                if (FileDownHelper.FileExists(filepath))
                {
                    FileDownHelper.DownLoadold(filepath, name);
                }

            }
        }
        #endregion
        #region 应急演练
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index2()
        {
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();



            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            contentDepart.Insert(0, new Entity.BaseManage.DepartmentEntity { DepartmentId = "全部", FullName = "全部" });
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName });
            //应急预案类型
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            contentType.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            ViewData["EmergencyType"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetDataManoeuvre(string name, int rows)
        {
            var bll = new EmergencyBLL();

            var user = OperatorProvider.Provider.Current();

            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var ToCompileDeptIdSearch = this.Request.QueryString.Get("ToCompileDeptIdSearch");
            var EmergencyTypeSearch = this.Request.QueryString.Get("EmergencyTypeSearch");
            var meetingstarttime = this.Request.QueryString.Get("meetingstarttime");
            var meetingendtime = this.Request.QueryString.Get("meetingendtime");
            var total = 0;
            //var data = bll.GetData(no, person, level, category, string.IsNullOrEmpty(state) ? "全部" : state, pagesize, page, out total);


            var data = bll.GetEvaluationsManoeuvre(user.DeptId, name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
            foreach (EmergencyReportEntity ere in data)
            {
                if (ere.state == "True" || ere.state == "1")
                {
                    ere.state = "已评价";
                }
                else
                {
                    ere.state = "未评价";
                }
            }

            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail2()
        {
            var id = this.Request.QueryString.Get("keyValue");
            //if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            //应急预案类型
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            ViewData["EmergencyType"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //演练方式
            var mainManoeuvre = itembll.GetEntityByName("应急演练");
            var contentManoeuvre = itemdetialbll.GetList(mainManoeuvre.ItemId).ToList();
            ViewData["Manoeuvre"] = contentManoeuvre.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //编制人
            var userBll = new UserBLL();
            var contentUser = userBll.GetList().ToList();
            ViewData["ToCompileUser"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName });
            //编制人对应部门
            ViewData["UserIdDeptId"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.DepartmentId });
            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName });

            var bll = new EmergencyBLL();
            var data = bll.GetReportEntity(id);
            return View(data);
        }
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail3()
        {
            var id = this.Request.QueryString.Get("keyValue");
            //if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            //应急预案类型
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var mainType = itembll.GetEntityByName("应急预案");
            var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            ViewData["EmergencyType"] = contentType.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //演练方式
            var mainManoeuvre = itembll.GetEntityByName("应急演练");
            var contentManoeuvre = itemdetialbll.GetList(mainManoeuvre.ItemId).ToList();
            ViewData["Manoeuvre"] = contentManoeuvre.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            //编制人
            var userBll = new UserBLL();
            var contentUser = userBll.GetList().ToList();
            ViewData["ToCompileUser"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName });
            //编制人对应部门
            ViewData["UserIdDeptId"] = contentUser.Select(x => new SelectListItem() { Value = x.UserId, Text = x.DepartmentId });
            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName });

            var bll = new EmergencyBLL();
            var data = bll.GetReportEntity(id);
            data.evaluationuser = OperatorProvider.Provider.Current().UserName;
            return View(data);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="workEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        [HandlerMonitor(5, "保存应急演练(新增、修改)")]
        public ActionResult SaveFormReport(string keyValue, EmergencyReportEntity workEntity)
        {
            ebll.updateEmergencyReportEvaluate(keyValue, workEntity);
            return Success("操作成功。");
        }
        #endregion

        #endregion
        /// <summary>
        /// Word转换成pdf
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <returns>true=转换成功</returns>
        private void DocConvertPDF(string sourcePath, string targetPath)
        {
            Task.Run(() => {

                BSFramework.Util.Offices.Office2PDFHelper.DOCConvertToPDF(sourcePath, targetPath);
            });
        }
    }
}
