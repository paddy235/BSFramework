using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Busines.SystemManage;
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

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public class AppPackageController : MvcControllerBase
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
            pagination.p_kid = "ID";
            pagination.p_fields = "ID,CreateDate,AppName,ReleaseVersion,PublishVersion,ReleaseDate";
            pagination.p_tablename = "bis_Package t";
            pagination.conditionJson = " 1=1";
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
        #region 页面组件初始化
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetInitDataJson()
        {
            string FileName = Guid.NewGuid().ToString();
            //返回值
            var josnData = new
            {
                FileName = FileName
            };

            return Content(josnData.ToJson());
        }
        #endregion
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
        public ActionResult SaveForm(string keyValue, AppPackageEntity entity)
        {
            packagebll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult Bulid(string keyValue)
        {
            //二维码路径固定
            string appName = Request.ApplicationPath;
            if (appName == "/" || appName == @"\") appName = "/ERCHTMS";
            string url = string.Format("http://{0}{1}{2}", HttpContext.Request.Url.Host, appName, "/Resource/AppFile");
            //判断当前二维码路径存不存在
            string filePath = Server.MapPath("~/Resource/AppFile/");
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

            }
            catch (Exception e)
            {
                return Error("路径有误。");
            }

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeVersion = 10;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeForegroundColor = Color.Black;
            Bitmap bmp = qrCodeEncoder.Encode(keyValue, Encoding.UTF8);//指定utf-8编码， 支持中文
            bmp.Save(Path.Combine(filePath, "Download.jpg"));

            return Success("生成成功。");
        }
        /// <summary>
        /// 获取默认路径
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDefaltPath()
        {
            string appName = Request.ApplicationPath;
            if (appName == "/" || appName == @"\") appName = "/ERCHTMS";
            var url = string.Format("http://{0}{1}{2}", HttpContext.Request.Url.Host, appName, "/Resource/AppFile/双控宝.apk");
            return Content(url);
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVersion()
        {
            var version = "";
            object[] att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (att.Length > 0)
            {
                version = ((AssemblyFileVersionAttribute)att[0]).Version;
            }
            return Content(version);
        }
        #endregion
    }
}
