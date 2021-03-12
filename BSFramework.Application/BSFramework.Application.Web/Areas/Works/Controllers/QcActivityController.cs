using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{

    /// <summary>
    /// qc活动
    /// </summary>
    public class QcActivityController : MvcControllerBase
    {
        private DepartmentBLL deptBll = new DepartmentBLL();

        private QcActivityBLL qcbll = new QcActivityBLL();
        /// <summary>
        /// 展示页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            //ViewBag.userId = user.UserId;
            var ck = false;
            if (user.DeptId=="0")
            {
                ck = true;
            }
            else
            {
              var dept=  deptBll.GetEntity(user.DeptId);
                if (dept.Nature=="厂级")
                {
                    ck = true;
                }
            }
            var tree = deptBll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            ViewBag.userId = user.UserId;
            ViewBag.DeptCk = ck;
            return View();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var queryParam = queryJson.ToJObject();
            var keyValue = new Dictionary<string, string>();
            if (queryJson != null)
            {
                if (!string.IsNullOrEmpty(queryParam["subjectstate"].ToString()))
                {
                    string subjectstate = queryParam["subjectstate"].ToString();
                    keyValue.Add("subjectstate", subjectstate);
                }
                if (!string.IsNullOrEmpty(queryParam["year"].ToString()))
                {
                    string year = queryParam["year"].ToString();
                    keyValue.Add("year", year);
                }
                if (!string.IsNullOrEmpty(queryParam["name"].ToString()))
                {
                    string name = queryParam["name"].ToString();
                    keyValue.Add("name", name);
                }
                if (!string.IsNullOrEmpty(queryParam["deptid"].ToString()))
                {
                    string deptid = queryParam["deptid"].ToString();
                    keyValue.Add("deptid", deptid);
                }
                else
                {
                    keyValue.Add("deptid", "0");
                }
            }
            else
            {
                keyValue.Add("deptid", "0");
            }
            var data = qcbll.getQcList(keyValue, pagination, true);
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
        /// 查看
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexDetail(string keyvalue)
        {
            var data = qcbll.getQcById(keyvalue);
            return View(data);
        }
        /// <summary>
        ///编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexEdit(string keyvalue)
        {
            var data = qcbll.getQcById(keyvalue);
            //var itemdetialbll = new DataItemDetailBLL();
            //var itembll = new DataItemBLL();
            //var mainType = itembll.GetEntityByName("QC活动");
            //var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            //var grouptype = new List<SelectListItem>();
            //var detail = contentType.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName });
            //foreach (var item in detail)
            //{
            //    if (item.Value==data.grouptype)
            //    {
            //        item.Selected = true;
            //    }
            //}
            //grouptype.AddRange(detail);
            //ViewData["grouptype"] = grouptype;
            return View(data);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexAdd()
        {
            //var itemdetialbll = new DataItemDetailBLL();
            //var itembll = new DataItemBLL();
            //var mainType = itembll.GetEntityByName("QC活动");
            //var contentType = itemdetialbll.GetList(mainType.ItemId).ToList();
            //var grouptype = new List<SelectListItem>();
            //var detail = contentType.Select(x => new SelectListItem() { Value = x.ItemId, Text = x.ItemName });
            //grouptype.AddRange(detail);
            //ViewData["grouptype"] = grouptype;
            return View();
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ActionResult Select()
        {

            return View();
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectShow()
        {

            return View();
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectDept()
        {

            return View();
        }

        /// <summary>
        /// 查看
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFiles(string keyvalue)
        {
            var data = qcbll.getQcById(keyvalue);
            foreach (var item in data.Photos)
            {
                item.FilePath = Url.Content(item.FilePath);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
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
            var data = deptbll.GetSubDepartments(rootdpet.DepartmentId, "厂级,部门,班组").Select(x => new ItemEntity { ItemType = x.Nature, ItemId = x.DepartmentId, ItemName = x.FullName, ParentItemId = x.ParentId }).ToList();

            return Json(data.Where(x => x.ParentItemId == "0").Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.ItemId,
                value = x.ItemId,
                text = x.ItemName,
                ckselect = x.ItemType == "厂级" ? "不能选择厂级" : "",
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList(), JsonRequestBehavior.AllowGet);
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
                    showcheck = true,
                    ckselect = x.ItemType == "dept" ? "false" : "",
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
                showcheck = true,
                ckselect = x.ItemType == "dept" ? "false" : "",
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
                ckselect = x.ItemType == "厂级" ? "不能选择厂级" : "",
                value = x.ItemId,
                text = x.ItemName,
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
        public string PostFile([System.Web.Http.FromUri]string filePath, [System.Web.Http.FromUri]string recId, [System.Web.Http.FromUri]string Description, [System.Web.Http.FromUri]int isDate = 0)
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
                        fileInfoEntity.Description = Description;
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
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, QcActivityEntity entity)
        {
            try
            {
                entity.qcid = keyValue;
                qcbll.addEntity(entity);

                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult EditForm(QcActivityEntity entity)
        {
            try
            {
                qcbll.EditEntity(entity);

                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult delEntity(string keyValue)
        {
             FileInfoBLL fileBll = new FileInfoBLL();
            try
            {
                string getDel = string.Empty;
                #region 删除数据
                if (!string.IsNullOrEmpty(keyValue))
                {
                    var fileList = fileBll.GetFilesByRecIdNew(keyValue).ToList();
                    getDel = string.Join(",", fileList.Select(x => x.FileId));
                }
                #endregion
                #region 修改删除图片
                var DelKeys = getDel.Split(',');
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Length; i++)
                {
                    if (string.IsNullOrEmpty(DelKeys[i]))
                    {
                        continue;
                    }
                    FileInfoEntity fileList = fileBll.GetEntity(DelKeys[i]);
                    string url = Server.MapPath(fileList.FilePath);
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                #endregion
                qcbll.delEntity(keyValue);

                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        
        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFilePath(string keyvalue, string filename)
        {
            FileInfoBLL fileInfoBLL = new FileInfoBLL();
            var data = fileInfoBLL.GetEntity(keyvalue, filename);
            if (data == null)
            {
                return Success("");
            }
            var path = Url.Content(data.FilePath);
            return Success(path);


        }
    }
}
