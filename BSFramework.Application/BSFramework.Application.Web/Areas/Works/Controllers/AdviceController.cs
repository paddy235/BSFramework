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
    /// 合理化建议
    /// </summary>
    public class AdviceController : MvcControllerBase
    {
        private DepartmentBLL deptBll = new DepartmentBLL();

        private AdviceBLL Bll = new AdviceBLL();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string searchtype=null)
        {
            var user = OperatorProvider.Provider.Current();
            //ViewBag.userId = user.UserId;
            var ck = false;
            if (user.DeptId == "0")
            {
                ck = true;
            }
            else
            {
                var dept = deptBll.GetEntity(user.DeptId);
                if (dept.Nature == "厂级")
                {
                    ck = true;
                }
            }
            var tree = deptBll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            ViewBag.DeptCk = ck;
            bool isSearchType = false;//是否是待办跳转过来的
            if (searchtype== "todo")
            {
                isSearchType = true;
            }
            ViewBag.IsSearchType = isSearchType;
            ViewBag.UserId = user.UserId;
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
            var user = OperatorProvider.Provider.Current();

            var keyValue = new Dictionary<string, string>();
            if (queryJson != null)
            {
                if (!string.IsNullOrEmpty(queryParam["advicetype"].ToString()))
                {
                    string advicetype = queryParam["advicetype"].ToString();
                    keyValue.Add("advicetype", advicetype);
                }
                if (!string.IsNullOrEmpty(queryParam["state"].ToString()))
                {
                    string state = queryParam["state"].ToString();
                    keyValue.Add("state", state);
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
            var data = Bll.getAdviceListIndex(keyValue, pagination, true);
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
        /// 获取数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public ActionResult GetAuditJson(Pagination pagination, string keyValue)
        {
            var watch = CommonHelper.TimerStart();
            var data = Bll.getAuditByid(keyValue).OrderBy(x => x.sort).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList(); ;
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
            var data = Bll.getAdviceByid(keyvalue)[0];
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
                if (audit.sort == 6&& !string.IsNullOrEmpty(audit.state))
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
            var audit = Bll.getAuditId(id)[0];
            int sort = audit.sort;
            ViewBag.sort = sort;
            return View(audit);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult saveAudit(AdviceAuditEntity entity)
        {
            var data = Bll.getAuditId(entity.auditid)[0];
            data.opinion = entity.opinion;
            data.state = entity.state;
            data.submintdate = entity.submintdate;
            var success = true;
            var message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(entity.userid))
                {
                    var audit = new AdviceAuditEntity();

                    //  audit.submintdate = entity.submintdate;
                    audit.userid = entity.userid;
                    audit.username = entity.username;
                    audit.adviceid = data.adviceid;

                    Bll.Operation(null, null, null, audit, data);
                   
                }
                else
                {
                    Bll.Operation(null, null, null, null, data);
                    //if(data != null)
                    //{
                    //    if (data.state == "审核通过")
                    //    {
                    //        msgBll.SendMessage("合理化建议审批通过", data.adviceid);
                    //    }
                    //    else if (data.state == "审核不通过")
                    //    {
                    //        msgBll.SendMessage("合理化建议审批不通过", data.adviceid);
                    //    }
                    //}
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
        public ActionResult SelectShowAudit(string keyvalue, int sort)
        {
            ViewBag.userid = keyvalue;
            ViewBag.sort = sort;
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
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexAdd()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptid = user.DeptId;
            ViewBag.deptname = user.DeptName;
            ViewBag.nowtime = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.username = user.UserName;
            ViewBag.userid = user.UserId;
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
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, AdviceEntity entity)
        {
            try
            {
                entity.adviceid = keyValue;
                var audit = new AdviceAuditEntity();
                audit.adviceid = entity.adviceid;
                audit.userid = entity.touserid;
                audit.username = entity.tousername;
                audit.auditid = Guid.NewGuid().ToString();
                audit.sort = 1;
                entity.touserid = null;
                entity.tousername = null;
                entity.audit = null;
                Bll.Operation(entity, null, "", audit, null);
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
                //ckselect = x.ItemType == "dept" ? "false" : "",
                value = x.ItemId,
                text = x.ItemName,
                ckselect = x.ItemType == "厂级" ? "不能选择厂级" : "",
                isexpand = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                hasChildren = data.Count(y => y.ParentItemId == x.ItemId) > 0,
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList();

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
                ChildNodes = GetChildren(data, x.ItemId)
            }).ToList(), JsonRequestBehavior.AllowGet);
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
                Bll.Operation(null, null, keyValue, null, null);

                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

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
