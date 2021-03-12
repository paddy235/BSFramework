using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SevenSManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Application.Web.Areas.BaseManage.Models;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 7S
    /// </summary>
    public class SevenSController : MvcControllerBase
    {
        SevenSBLL ebll = new SevenSBLL();
        FileInfoBLL file = new FileInfoBLL();
        DepartmentBLL dept = new DepartmentBLL();
        #region 7S技术规范库
        //
        // GET: /Works/SevenS/
        /// <summary>
        /// 7S技术规范库
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var DeptConfig = Config.GetValue("EmergencyCard");
            if (DeptConfig == user.DeptName)
            {
                ViewBag.ck = 1;
            }
            else
            {
                ViewBag.ck = 0;
            }
            //if (user.UserName == "超级管理员")
            //{
            //    ViewBag.ck = 1;
            //}

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetItems(int rows, int page, string key, string typeid)
        {
            var user = OperatorProvider.Provider.Current();
            string deptCode = string.Empty;
            var company = dept.GetCompany(user.DeptId);
            var depts = dept.GetSubDepartments(company.DepartmentId, "");
            deptCode = string.Join(",", depts.Select(x => x.DepartmentId));
            var total = 0;
            var data = ebll.GetItems(key, typeid, rows, page, deptCode, out total).OrderBy(x => x.CreateDate).ThenBy(x => x.Name);

            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllType()
        {
            var user = OperatorProvider.Provider.Current();

            var company = dept.GetCompany(user.DeptId);
            var depts = dept.GetSubDepartments(company.DepartmentId, "集团,省级,厂级");
            var data = ebll.GetAllType(depts.Select(x => x.DepartmentId).ToArray()).ToList();

            var treeList = new List<TreeEntity>();

            foreach (var item in depts)
            {
                bool hasChild = depts.Where(x => x.ParentId == item.DepartmentId).Count() > 0 ? true : false || data.Where(x => x.deptid == item.DepartmentId).Count() > 0 ? true : false;
                TreeEntity tree = new TreeEntity();
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChild;
                tree.Attribute = "Code";
                tree.AttributeValue = item.EnCode;
                //tree.AttributeA = "Scope";
                //tree.AttributeValueA = item.Scope;
                //tree.AttributeB = "Dept";
                //tree.AttributeValueB = item.Scope;

                treeList.Add(tree);
            }

            foreach (var item in data)
            {
                //bool hasChild = data.Where(x => x.ParentId == item.ID).Count() > 0 ? true : false;
                TreeEntity tree = new TreeEntity();
                tree.id = item.TypeId;
                tree.text = item.TypeName;
                tree.value = item.TypeId;
                tree.parentId = string.IsNullOrEmpty(item.ParentCardId) ? depts.FirstOrDefault(x => x.DepartmentId == item.deptid).DepartmentId : item.ParentCardId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = data.Where(x => x.ParentCardId == tree.id).Count() > 0;
                tree.Attribute = "Code";
                tree.AttributeValue = item.TypeId;
                //tree.AttributeA = "Scope";
                //tree.AttributeValueA = item.Scope;
                //tree.AttributeB = "Dept";
                //tree.AttributeValueB = item.Scope;

                treeList.Add(tree);
            }

            return Content(treeList.TreeToJson(company.ParentId));







            //string deptCode = string.Empty;
            //deptCode = user.DeptCode;
            //return Json(data.Where(x => x.ParentCardId == null).Select(x => new TreeModel
            //{
            //    id = x.TypeId,
            //    value = x.TypeId,
            //    text = x.TypeName,
            //    isexpand = data.Count(y => y.ParentCardId == x.TypeId) > 0,
            //    hasChildren = data.Count(y => y.ParentCardId == x.TypeId) > 0,
            //    ChildNodes = GetChildren(data, x.TypeId)
            //}).ToList(),
            //    JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<SevenSTypeEntity> data, string id)
        {
            return data.Where(x => x.ParentCardId == id).Select(x => new TreeModel { id = x.TypeId, value = x.TypeId, text = x.TypeName, isexpand = data.Count(y => y.ParentCardId == x.TypeId) > 0, hasChildren = data.Count(y => y.ParentCardId == x.TypeId) > 0, ChildNodes = GetChildren(data, x.TypeId) }).ToList();
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

                ebll.DeleteType(id);
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
        public ActionResult Import()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userName = user.UserName;
            return View();
        }
        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditType(string id)
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
        public JsonResult EditType(string id, EmergencyCardModel model)
        {

            var success = true;
            var message = "保存成功";

            string deptId = string.Empty;
            string deptCode = string.Empty;
            string deptName = string.Empty;
            var user = OperatorProvider.Provider.Current();

            try
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
                if (string.IsNullOrEmpty(id))
                {
                    var data = ebll.GetAllType(deptCode).ToList();
                    if (data.Count > 0)
                    {
                        var ck = data.Where(x => x.TypeName == model.CardName);
                        if (ck.Count() > 0)
                        {
                            return Json(new AjaxResult { type = false ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode("名称不能重复") });

                        }
                        ebll.AddType(new SevenSTypeEntity() { deptcode = deptCode, deptid = deptId, deptname = deptName, TypeName = model.CardName, TypeId = model.CardId, CreateTime = DateTime.Now, ParentCardId = data.FirstOrDefault(x => x.ParentCardId == null).TypeId });

                    }
                    else
                    {
                        ebll.AddType(new SevenSTypeEntity() { deptcode = deptCode, deptid = deptId, deptname = deptName, TypeName = model.CardName, TypeId = Guid.NewGuid().ToString(), CreateTime = DateTime.Now, ParentCardId = null });

                    }
                }
                else
                {
                    var data = ebll.GetAllType(deptCode).ToList();
                    var ck = data.Where(x => x.TypeName == model.CardName);
                    if (ck.Count() > 0)
                    {
                        return Json(new AjaxResult { type = false ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode("名称不能重复") });

                    }
                    ebll.EditType(new SevenSTypeEntity() { TypeId = model.CardId, TypeName = model.CardName });
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
        public ActionResult Edit(string id)
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userName = user.UserName;
            var my = ebll.GetSevenSEntity(id);
            ViewBag.name = my.Name;
            ViewBag.Id = my.ID;
            ViewBag.path = my.Path;
            return View();
        }
        /// <summary>
        /// 选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Select()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ImportContext(string keyValue, SevenSEntity model)
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
                    SevenSEntity one = new SevenSEntity();
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
        [HttpPost]
        public JsonResult ImportEdit(string keyValue, bool ck, SevenSEntity model)
        {
            var success = true;
            var message = "修改成功";
            try
            {
                var user = OperatorProvider.Provider.Current();
                if (ck)
                {
                    FileInfoBLL fileInfoBLL = new FileInfoBLL();
                    var fileList = fileInfoBLL.GetFilesByRecIdNew(keyValue).OrderBy(x => x.CreateDate).ToList();
                    var fileListold = fileInfoBLL.GetFilesByRecIdNew(model.ID);
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (i > 0)
                        {
                            fileInfoBLL.DeleteFile(keyValue, fileList[i].FileName, fileList[i].FilePath);
                        }
                        else
                        {
                            fileList[i].RecId = model.ID;
                            fileInfoBLL.SaveFormEmergency(fileList[i].FileId, fileList[i]);
                        }

                    }
                    for (int i = 0; i < fileListold.Count; i++)
                    {
                        fileInfoBLL.DeleteFile(fileListold[i].RecId, fileListold[i].FileName, fileListold[i].FilePath);
                    }
                    model.CREATEUSERID = user.UserId;
                    model.CREATEUSERNAME = user.UserName;
                    ebll.SaveForm(model.ID, model);
                }
                else
                {
                    var one = ebll.GetSevenSEntity(model.ID);
                    one.Name = model.Name;
                    ebll.SaveSevenSEntity(one);

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
                if (!fileName.Contains(".pdf"))
                {
                    throw new Exception("请检查文件格式！");
                }
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = file.ContentLength;
                string FileEextension = Path.GetExtension(fileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string dir = string.Format("~/Resource/{0}/{1}", "SevenS", uploadDate);
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
                    fileInfoEntity.FolderId = "SevenS";
                    fileInfoEntity.FileId = fileGuid;
                    fileInfoEntity.RecId = keyvalue;
                    fileInfoEntity.FileName = fileName;
                    fileInfoEntity.FilePath = dir + "/" + newFileName;
                    fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                    fileInfoEntity.FileExtensions = FileEextension;
                    fileInfoEntity.FileType = FileEextension.TrimStart('.');
                    FileInfoBLL fileInfoBLL = new FileInfoBLL();
                    fileInfoBLL.SaveForm("", fileInfoEntity);
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



        public ActionResult Index1(FormCollection fc)
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

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            ViewData["Path"] = new List<string>();
            SevenSEntity model = ebll.GetSevenSEntity(id);
            FileInfoBLL fileInfoBLL = new FileInfoBLL();

            var fileList = fileInfoBLL.GetFilesByRecIdNew(id);
            var path = Url.Content("~").Substring(0, @Url.Content("~").Length - 1);
            var filePath = path + fileList[0].FilePath.Substring(1, fileList[0].FilePath.Length - 1);
            ViewBag.path = filePath;
            model.seenum = model.seenum > 0 ? model.seenum + 1 : 1;
            ebll.SaveSevenSEntity(model);
            return View(model);
        }
        #endregion


        #region 7S定点拍照
        /// <summary>
        /// 展示记录
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexPicture()
        {
            var user = OperatorProvider.Provider.Current();
            var tree = dept.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            return View();
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        //[HandlerMonitor(3, "分页查询用户信息!")]
        // DateTime? planeStart, DateTime? planeEnd, string state, string evaluationState, string space, Pagination pagination, bool ispage
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var queryParam = queryJson.ToJObject();
            DateTime? planeStart = null;
            DateTime? planeEnd = null;
            var state = "";
            var evaluationState = "";
            var deptId = "";
            if (queryJson != null)
            {


                if (!string.IsNullOrEmpty(queryParam["planeStart"].ToString()))
                {
                    planeStart = Convert.ToDateTime(queryParam["planeStart"].ToString());
                }
                if (!string.IsNullOrEmpty(queryParam["planeEnd"].ToString()))
                {
                    planeEnd = Convert.ToDateTime(queryParam["planeEnd"].ToString());
                }

                deptId = queryParam["deptId"].ToString();
                state = queryParam["state"].ToString();
                evaluationState = queryParam["evaluationState"].ToString();
            }
            var data = ebll.getList(planeStart, planeEnd, state, evaluationState, "", pagination, true, deptId, false);
            foreach (var item in data)
            {
                if (item.planeStartDate != null && item.planeEndDate != null)
                {
                    item.planeTime = Convert.ToDateTime(item.planeStartDate).ToString("yyyy-MM-dd") + "-" + Convert.ToDateTime(item.planeEndDate).ToString("yyyy-MM-dd");
                }
                else
                {
                    item.planeTime = null;
                }
                if (item.state == "未提交")
                {
                    item.ModifyDate = null;
                }
                if (string.IsNullOrEmpty(item.evaluation))
                {
                    item.evaluationState = "未评价";
                }
                else
                {
                    item.evaluationState = "已评价";
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

        /// <summary>
        /// 展示记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SpaceCycle()
        {

            return View();
        }

        /// <summary>
        /// 获取周期设置数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTimeSet()
        {
            var data = ebll.getCycle();
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取地点设置数据
        /// </summary>
        /// <returns></returns>
        public ActionResult getSet()
        {
            var data = ebll.getSet().OrderBy(x => x.createtime).ToList();
            return Content(data.ToJson());
        }

        /// <summary>
        /// 保存设置数据
        /// </summary>
        /// <returns></returns>   
        /// [HttpPost]

        public ActionResult SaveData(string deleteStr, string entity, string setTime, string regulation)
        {
            var del = deleteStr.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
            var data = entity.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
            List<SevenSPictureSetEntity> list = new List<SevenSPictureSetEntity>();
            if (data.Length >= 2)
            {
                for (int i = 0; i < data.Length - 1; i = i + 2)
                {
                    var one = new SevenSPictureSetEntity();
                    one.Id = Guid.NewGuid().ToString();
                    one.space = data[i];
                    one.remark = data[i + 1];
                    one.createtime = DateTime.Now.ToString();
                    list.Add(one);
                }
            }
            try
            {
                ebll.SaveFrom(list, del, setTime, regulation);
                return Success("保存成功。");
            }
            catch (Exception)
            {

                return Error("保存失败");
            }

        }
        /// <summary>
        /// 展示图片详情
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailPicture(string keyValue)
        {
            var Files = file.GetFilesByRecIdNew(keyValue).OrderBy(x => x.SortCode).ToList();
            var entity = ebll.getEntity(keyValue);
            ViewBag.evaluation = entity.evaluation;
            ViewBag.DeptName = entity.deptname;
            ViewBag.keyValue = keyValue;
            ViewBag.user = entity.evaluationUser;
            if (string.IsNullOrEmpty(entity.evaluationDate))
            {
                ViewBag.evaluationtime = "";
            }
            else
            {
                ViewBag.evaluationtime = DateTime.Parse(entity.evaluationDate).ToString("yyyy-MM-dd");
            }

            return View(Files);
        }
        /// <summary>
        /// 保存评价
        /// </summary>
        /// <returns></returns>   
        /// [HttpPost]

        public ActionResult SaveEvaluation(string keyValue, string evaluation)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                ebll.SaveEvaluation(keyValue, evaluation, user.UserName);



                return Success("保存成功。");
            }
            catch (Exception)
            {

                return Error("保存失败");
            }


        }


        #endregion

        #region 7S精益管理
        /// <summary>
        /// 展示记录
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexOffice(string searchtype = null)
        {
            var user = OperatorProvider.Provider.Current();
            bool isSearchType = false;//是否是待办跳转过来的
            if (searchtype == "todo")
            {
                isSearchType = true;
            }
            ViewBag.IsSearchType = isSearchType;
            ViewBag.UserId = user.UserId;
            return View();
        }
        /// <summary>
        /// 平台查询列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        public ActionResult GetOfficePageListJson(Pagination pagination, string queryJson)
        {
            var user = OperatorProvider.Provider.Current();
            var watch = CommonHelper.TimerStart();
            var queryParam = queryJson.ToJObject();
            string Start = string.Empty;
            string End = string.Empty;
            var aduitstate = string.Empty;
            var aduitresult = string.Empty;
            string name = string.Empty;
            var keyValue = new Dictionary<string, string>();
            if (queryJson != null)
            {
                if (!string.IsNullOrEmpty(queryParam["start"].ToString()))
                {
                    Start = queryParam["start"].ToString();
                    keyValue.Add("start", Start);
                }
                if (!string.IsNullOrEmpty(queryParam["end"].ToString()))
                {
                    End = queryParam["end"].ToString();
                    keyValue.Add("end", End);
                }
                if (!string.IsNullOrEmpty(queryParam["aduitstate"].ToString()))
                {
                    aduitstate = queryParam["aduitstate"].ToString();
                    keyValue.Add("aduitstate", aduitstate);
                }
                if (!string.IsNullOrEmpty(queryParam["aduitresult"].ToString()))
                {
                    aduitresult = queryParam["aduitresult"].ToString();
                    keyValue.Add("aduitresult", aduitresult);
                }
                if (!string.IsNullOrEmpty(queryParam["name"].ToString()))
                {
                    name = queryParam["name"].ToString();
                    keyValue.Add("name", name);
                }
            }
            var data = ebll.SelectOffice(user.UserId, keyValue, pagination);
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
        /// 根据id 获取数据
        /// </summary>
        public ActionResult getOfficeByid(string Strid)
        {
            var data = ebll.getOfficeByid(Strid);
            return Content(data.ToJson());
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexAudit(string keyvalue)
        {
            var data = ebll.getOfficeByid(keyvalue)[0];
            var user = OperatorProvider.Provider.Current();
            // 0无权限 1已经审核 2可以审核
            int ck = 0;
            var audit = data.audit.Last();

            if (audit.userid == user.UserId)
            {
                if (string.IsNullOrEmpty(audit.state))
                {
                    ck = 2;
                }
                if (audit.sort == 6)
                {
                    ck = 3;
                }
            }
            ViewBag.Id = audit.auditid;
            ViewBag.ck = ck;

            return View(data);
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexShow(string id)
        {

            ViewBag.time = DateTime.Now.ToString("yyyy-MM-dd");
            var audit = ebll.getAuditId(id)[0];
            int sort = audit.sort;
            ViewBag.sort = sort;
            return View(audit);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult saveAudit(SevenSOfficeAuditEntity entity)
        {
            var data = ebll.getAuditId(entity.auditid)[0];
            data.opinion = entity.opinion;
            data.state = entity.state;
            data.submintdate = entity.submintdate;
            var success = true;
            var message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(entity.userid))
                {
                    var audit = new SevenSOfficeAuditEntity();

                    audit.submintdate = entity.submintdate;
                    audit.userid = entity.userid;
                    audit.username = entity.username;
                    audit.officeid = entity.officeid;

                    ebll.Operation(null, null, null, audit, data);
                }
                else
                {
                    ebll.Operation(null, null, null, null, data);

                }
                return Json(new { success, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                success = false;
                return Json(new { success, message }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 人员管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectShow(string keyvalue, int sort)
        {
            ViewBag.userid = keyvalue;
            ViewBag.sort = sort;
            return View();
        }

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectUserByval(string keyvalue)
        {
            var user = OperatorProvider.Provider.Current();
            ReportBLL cbll = new ReportBLL();
            var data = cbll.GetSubmitPerson("") as List<ItemEntity>;
            if (string.IsNullOrEmpty(keyvalue))
            {
                return Json(data.Where(x => x.ParentItemId == "0").Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
                {
                    id = x.ItemId,
                    value = x.ItemId,
                    text = x.ItemName,
                    ckselect = x.ItemType == "dept" ? "不能选择部门" : x.ItemId == user.UserId ? "不能选择自己审核" : "",
                    isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                    hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                    ChildNodes = GetChildren(data, x.ItemId)
                }).ToList(), JsonRequestBehavior.AllowGet);

            }
            return Json(data.Where(x => x.ItemName.Contains(keyvalue)).Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                value = x.ItemId,
                text = x.ItemName,
                ckselect = x.ItemType == "dept" ? "不能选择部门" : x.ItemId == user.UserId ? "不能选择自己审核" : "",
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectUser()
        {
            var user = OperatorProvider.Provider.Current();
            ReportBLL cbll = new ReportBLL();
            var data = cbll.GetSubmitPerson("") as List<ItemEntity>;

            return Json(data.Where(x => x.ParentItemId == "0").Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                value = x.ItemId,
                text = x.ItemName,
                ckselect = x.ItemType == "dept" ? "不能选择部门" : x.ItemId == user.UserId ? "不能选择自己审核" : "",
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
                ckselect = x.ItemType == "dept" ? "不能选择部门" : x.ItemId == user.UserId ? "不能选择自己审核" : "",
                value = x.ItemId,
                text = x.ItemName,
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList();
        }


        [HttpPost]
        public JsonResult GetCount(string from, string to)
        {
            Dictionary<string, string> keyValue = new Dictionary<string, string>();
            string start = string.Empty;
            string end = string.Empty;
            if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
            {
                var Now = DateTime.Now;
                start = new DateTime(Now.Year, 1, 1).ToString("yyyy-MM-dd");
                end = new DateTime(Now.Year, 12, 31).ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(from)) start = from;
            if (!string.IsNullOrEmpty(to)) end = to;
            if (!string.IsNullOrEmpty(start))
            {
                keyValue.Add("start", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                keyValue.Add("end", end);
            }
            return Json(new { rows = ebll.GetCount(keyValue).ToJson() });
        }
        /// <summary>
        /// 统计图
        /// </summary>
        /// <returns></returns>
        public ActionResult SevenSCount()
        {

            return View();
        }


        #endregion

    }
}
