using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Web.Mvc;
using System;
using System.Data;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using System.Text;
using System.IO;
using BSFramework.Application.Code;
using System.Reflection;
using System.Drawing.Imaging;
using System.Web;

namespace BSFramework.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public class PackageController : MvcControllerBase
    {
        private PackageBLL packagebll = new PackageBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CodeEncoder()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(Pagination pagination, string queryJson)
        {
            //pagination.p_kid = "ID";
            //pagination.p_fields = "CreateDate,AppName,PackType,ReleaseVersion,PublishVersion,ReleaseDate";
            //pagination.p_tablename = "wg_Package t";
            //pagination.conditionJson = " 1=1";
            var watch = CommonHelper.TimerStart();
            var data = packagebll.GetPageList(pagination, queryJson);
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
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = packagebll.GetEntity(keyValue);
            return ToJsonResult(data);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string keyValue)
        {
            packagebll.RemoveForm(keyValue);
            FileInfoBLL fileBll = new FileInfoBLL();
            var filelist = fileBll.GetFilesByRecIdNew(keyValue);
            foreach (var item in filelist)
            {
                string delPath = fileBll.Delete(item.FileId);
                System.IO.File.Delete(Server.MapPath(delPath));
            }
            return Success("删除成功。");
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
        public ActionResult SaveForm(string keyValue, PackageEntity entity)
        {
            try
            {
                packagebll.SaveForm(keyValue, entity);
                //this.BuildQRCode(keyValue);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult Bulid(string keyValue)
        {
            try
            {
                this.BuildQRCode(keyValue);
                return Success("生成成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        private void BuildQRCode(string keyValue)
        {
            PackageEntity packageEntity = packagebll.GetEntity(keyValue);
            FileInfoBLL fileBll = new FileInfoBLL();
            var filelist = fileBll.GetFilesByRecIdNew(keyValue);
            if (packageEntity == null)
                throw new ArgumentException("无APP应用程序信息");
            if (filelist == null || filelist.Count == 0)
                throw new ArgumentException("无APP应用程序文件");

            string staticFileName = string.Empty;
            staticFileName = packageEntity.PackType == 0 ? "bzapp.apk" : "bzzdapp.apk";

            string url = string.Format("http://{0}{1}/{2}", HttpContext.Request.Url.Host, HttpContext.Request.ApplicationPath, string.Format("/Resource/Package/{0}/{1}", packageEntity.PackType, staticFileName));
            //判断当前二维码路径存不存在
            string filePath = Server.MapPath(string.Format("~/Resource/Package/{0}", packageEntity.PackType));
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeVersion = 10;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeForegroundColor = Color.Black;
            Bitmap bmp = qrCodeEncoder.Encode(url, Encoding.UTF8);//指定utf-8编码， 支持中文
            bmp.Save(Path.Combine(filePath, "Download.jpg"));
            bmp.Dispose();
        }


        public ActionResult UploadFileNew(string keyValue, int packType)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string FileEextension = Path.GetExtension(files[0].FileName);
            if (!FileEextension.Contains("apk"))
            {
                return Success("1");
            }
            string Id = Guid.NewGuid().ToString();
            string virtualPath = string.Format("~/Resource/Package/{0}/{1}{2}", packType, Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            //保存一个固定的路径
            string staticFileName = string.Empty;
            staticFileName = packType == 0 ? "bzapp.apk" : "bzzdapp.apk";
            string staticFilePath = Server.MapPath(string.Format("~/Resource/Package/{0}/{1}", packType, staticFileName));
            files[0].SaveAs(staticFilePath);

            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = keyValue,
                RecId = keyValue,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = packType == 0 ? "安卓APP" : "安卓终端"
            };
            fileBll.SaveForm(fi);

            var filelist = fileBll.GetFilesByRecIdNew(keyValue);
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = Url.Content(f.FilePath);
            }
            return Success("上传成功。", new { path = virtualPath.TrimStart('~'), name = fi.FileName, files = filelist, newpath = Url.Content(virtualPath) });
        }

        #endregion
    }
}
