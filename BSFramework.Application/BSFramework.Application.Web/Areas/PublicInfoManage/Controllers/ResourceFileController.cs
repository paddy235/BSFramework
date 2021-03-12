using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 描 述：文件管理
    /// </summary>
    public class ResourceFileController : MvcControllerBase
    {
        private FileFolderBLL fileFolderBLL = new FileFolderBLL();
        private FileInfoBLL fileInfoBLL = new FileInfoBLL();

        #region 视图功能
        /// <summary>
        /// 文件管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult UploadifyForm()
        {
            return View();
        }
        /// <summary>
        /// 文件夹表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult FolderForm()
        {
            return View();
        }
        /// <summary>
        /// 文件表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult FileForm()
        {
            return View();
        }
        /// <summary>
        /// 文件（夹）移动表单  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult MoveForm()
        {
            return View();
        }
        #endregion

        #region 获取数据


        /// <summary>
        /// 获取java平台视频地址
        /// </summary>
        /// <returns>返回列表Json</returns>
        public ContentResult GetJavaVideoJson(string file_id)
        {
            var url = "api/v1.0/course/selectUrlByFileId";
            string userId = OperatorProvider.Provider.Current().UserId;
            var client = new HttpClient();
            HttpContent httpheard = new StringContent("{\"file_id\":\""+file_id+"\"}");
            httpheard.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpheard.Headers.Add("userId", userId);
            httpheard.Headers.Add("source", userId);
            var res = client.PostAsync(Config.GetValue("javaApi") + url, httpheard).Result;
            var content = res.Content.ReadAsStringAsync().Result;
            return Content(content);
        }
        /// <summary>
        /// 文件夹列表 
        /// </summary>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetTreeJson()
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileFolderBLL.GetList(userId);
            var treeList = new List<TreeEntity>();
            foreach (FileFolderEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.FolderId) == 0 ? false : true;
                tree.id = item.FolderId;
                tree.text = item.FolderName;
                tree.value = item.FolderId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                if (hasChildren == false)
                {
                    tree.img = "fa fa-folder";
                }
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 所有文件（夹）列表
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(string folderId)
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileInfoBLL.GetList(folderId, userId);
            foreach(FileInfoEntity fi in data)
            {
                if (fi.FileType != "folder")
                {
                    if (!System.IO.File.Exists(Server.MapPath(fi.FilePath)))
                    {
                        fi.FilePath = "404";
                    }
                    else
                    {
                        fi.FilePath = "200";
                    }
                }
            }
            return ToJsonResult(data);
        }
        /// <summary>
        /// 文档列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetDocumentListJson()
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileInfoBLL.GetDocumentList(userId);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 图片列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetImageListJson()
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileInfoBLL.GetImageList(userId);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 回收站文件（夹）列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetRecycledListJson()
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileInfoBLL.GetRecycledList(userId);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 我的文件（夹）共享列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetMyShareListJson()
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileInfoBLL.GetMyShareList(userId);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 他人文件（夹）共享列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetOthersShareListJson()
        {
            string userId = OperatorProvider.Provider.Current().UserId;
            var data = fileInfoBLL.GetOthersShareList(userId);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 文件夹实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFolderFormJson(string keyValue)
        {
            var data = fileFolderBLL.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 文件实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFileFormJson(string keyValue)
        {
            var data = fileInfoBLL.GetEntity(keyValue);
            return ToJsonResult(data);
        }

        /// <summary>
        /// 获取文件apk列表
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetApkListJsonByFolder(string folderId)
        {
            var data = fileInfoBLL.GetApkListByObject(folderId);
            return ToJsonResult(data);
        }

        /// <summary>
        /// 根据业务记录Id获取附件信息
        /// </summary>
        /// <param name="recId">记录Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFilesByRecId(string recId)
        {
            var data = fileInfoBLL.GetFilesByRecIdNew(recId).ToList();
            data.ForEach(p => {
                p.FilePath = Url.Content(p.FilePath);
            });
            return ToJsonResult(data);
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="fileName">附件名称</param>
        /// <param name="recId">业务记录Id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveFile(string fileName, string recId)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            FileInfoEntity entity = fileBll.GetEntity(recId, fileName);
            int res = 0;
            if (entity != null)
            {
                res = fileBll.DeleteFile(recId, fileName, Server.MapPath(entity.FilePath));
            }
            return res > 0 ? Success("操作成功。") : Error("操作失败");
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 还原文件（夹）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RestoreFile(string keyValue, string fileType)
        {
            if (fileType == "folder")
            {
                fileFolderBLL.RestoreFile(keyValue);
            }
            else
            {
                fileInfoBLL.RestoreFile(keyValue);
            }
            return Success("还原成功。");
        }
        /// <summary>
        /// 删除文件（夹）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除文件(夹)信息")]
        public ActionResult RemoveForm(string keyValue, string fileType)
        {
            if (fileType == "folder")
            {
                fileFolderBLL.RemoveForm(keyValue);
            }
            else
            {
                fileInfoBLL.RemoveForm(keyValue);
            }
            return Success("删除成功。");
        }
        /// <summary>
        /// 彻底删除文件（夹）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "彻底删除文件(夹)")]
        public ActionResult ThoroughRemoveForm(string keyValue, string fileType)
        {
            if (fileType == "folder")
            {
                fileFolderBLL.ThoroughRemoveForm(keyValue);
            }
            else
            {
                fileInfoBLL.ThoroughRemoveForm(keyValue);
            }
            return Success("删除成功。");
        }

        /// <summary>
        /// 保存文件夹表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileFolderEntity">文件夹实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存文件夹表单(新增、修改)")]
        public ActionResult SaveFolderForm(string keyValue, FileFolderEntity fileFolderEntity)
        {
            fileFolderBLL.SaveForm(keyValue, fileFolderEntity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 保存文件表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileInfoEntity">文件实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存文件表单(新增、修改)")]
        public ActionResult SaveFileForm(string keyValue, FileInfoEntity fileInfoEntity)
        {
            fileInfoBLL.SaveForm(keyValue, fileInfoEntity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 保存文件（夹）移动位置
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="moveFolderId">要移动文件夹Id</param>
        /// <param name="FileType">文件类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存文件(夹)移动位置")]
        public ActionResult SaveMoveForm(string keyValue, string moveFolderId, string fileType)
        {
            if (fileType == "folder")
            {
                FileFolderEntity fileFolderEntity = new FileFolderEntity();
                fileFolderEntity.FolderId = keyValue;
                fileFolderEntity.ParentId = moveFolderId;
                fileFolderBLL.SaveForm(keyValue, fileFolderEntity);
            }
            else
            {
                FileInfoEntity fileInfoEntity = new FileInfoEntity();
                fileInfoEntity.FileId = keyValue;
                fileInfoEntity.FolderId = moveFolderId;
                fileInfoBLL.SaveForm(keyValue, fileInfoEntity);
            }
            return Success("操作成功。");
        }
        /// <summary>
        /// 共享文件（夹）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="IsShare">是否共享：1-共享 0取消共享</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(0, "共享文件(夹)")]
        public ActionResult ShareFile(string keyValue, int IsShare, string fileType)
        {
            if (fileType == "folder")
            {
                fileFolderBLL.ShareFolder(keyValue, IsShare);
            }
            else
            {
                fileInfoBLL.ShareFile(keyValue, IsShare);
            }
            return Success("共享成功。");
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="Filedata">文件对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadifyFile(string folderId, HttpPostedFileBase Filedata)
        {
            try
            {
                Thread.Sleep(500);////延迟500毫秒
                //没有文件上传，直接返回
                if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                {
                    return HttpNotFound();
                }
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/ResourceFile/{userId}{data}/{guid}.{后缀名}
                string userId = OperatorProvider.Provider.Current().UserId;
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = Filedata.ContentLength;
                string FileEextension = Path.GetExtension(Filedata.FileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string virtualPath = string.Format("~/Resource/DocumentFile/{0}/{1}/{2}{3}", userId, uploadDate, fileGuid, FileEextension);
                string fullFileName = this.Server.MapPath(virtualPath);
                //创建文件夹
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);
                FileInfoEntity fileInfoEntity = new FileInfoEntity();
                if (!System.IO.File.Exists(fullFileName))
                {
                    //保存文件
                    Filedata.SaveAs(fullFileName);
                    //文件信息写入数据库
                 
                    fileInfoEntity.Create();
                    fileInfoEntity.FileId = fileGuid;
                    if (!string.IsNullOrEmpty(folderId))
                    {
                        fileInfoEntity.FolderId = folderId;
                    }
                    else
                    {
                        fileInfoEntity.FolderId = "0";
                    }
                    fileInfoEntity.FileName = Filedata.FileName;
                    fileInfoEntity.FilePath = virtualPath;
                    fileInfoEntity.FileSize = filesize.ToString();
                    fileInfoEntity.FileExtensions = FileEextension;
                    fileInfoEntity.FileType = FileEextension.Replace(".", "");
                    fileInfoBLL.SaveForm("", fileInfoEntity);
                }
                return Success("上传成功。", fileInfoEntity.FileId);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        //[HttpPost]
        //[HttpGet]
        //[HandlerAuthorize(PermissionMode.Ignore)]
        public void DownloadFile(string keyValue, string filename, string recId)
        {
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
               
                if (name.ToLower().Contains(".gw")) name = name.ToLower().Replace(".gw", ".pdf");
                if (filepath.ToUpper().LastIndexOf(".GW")>0) filepath = filepath.ToUpper().Replace(".GW", ".pdf");

                if (FileDownHelper.FileExists(filepath))
                {
                    FileDownHelper.DownLoadold(filepath, name);
                }
                else
                {
                    if (this.Server.MapPath(data.FilePath).ToUpper().LastIndexOf(".GW") > 0) 
                          HttpContext.Response.Write("GW文件未找到或正在转换成PDF文件");
                    else
                        HttpContext.Response.Write("文件未找到");
                }
                 
            }
        }
        #endregion
    }
}
