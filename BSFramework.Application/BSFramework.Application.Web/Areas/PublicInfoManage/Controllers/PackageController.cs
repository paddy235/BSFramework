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
    /// �� ����app�汾
    /// </summary>
    public class PackageController : MvcControllerBase
    {
        private PackageBLL packagebll = new PackageBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CodeEncoder()
        {
            return View();
        }
        #endregion

        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�Json</returns>
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
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = packagebll.GetEntity(keyValue);
            return ToJsonResult(data);
        }

        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
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
            return Success("ɾ���ɹ���");
        }
        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
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
                return Success("�����ɹ���");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// ���ɶ�ά��
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult Bulid(string keyValue)
        {
            try
            {
                this.BuildQRCode(keyValue);
                return Success("���ɳɹ���");
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
                throw new ArgumentException("��APPӦ�ó�����Ϣ");
            if (filelist == null || filelist.Count == 0)
                throw new ArgumentException("��APPӦ�ó����ļ�");

            string staticFileName = string.Empty;
            staticFileName = packageEntity.PackType == 0 ? "bzapp.apk" : "bzzdapp.apk";

            string url = string.Format("http://{0}{1}/{2}", HttpContext.Request.Url.Host, HttpContext.Request.ApplicationPath, string.Format("/Resource/Package/{0}/{1}", packageEntity.PackType, staticFileName));
            //�жϵ�ǰ��ά��·���治����
            string filePath = Server.MapPath(string.Format("~/Resource/Package/{0}", packageEntity.PackType));
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeVersion = 10;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeForegroundColor = Color.Black;
            Bitmap bmp = qrCodeEncoder.Encode(url, Encoding.UTF8);//ָ��utf-8���룬 ֧������
            bmp.Save(Path.Combine(filePath, "Download.jpg"));
            bmp.Dispose();
        }


        public ActionResult UploadFileNew(string keyValue, int packType)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
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
            //�����ļ��У������ļ�
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            //����һ���̶���·��
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
                Description = packType == 0 ? "��׿APP" : "��׿�ն�"
            };
            fileBll.SaveForm(fi);

            var filelist = fileBll.GetFilesByRecIdNew(keyValue);
            foreach (FileInfoEntity f in filelist)
            {
                f.FilePath = Url.Content(f.FilePath);
            }
            return Success("�ϴ��ɹ���", new { path = virtualPath.TrimStart('~'), name = fi.FileName, files = filelist, newpath = Url.Content(virtualPath) });
        }

        #endregion
    }
}
