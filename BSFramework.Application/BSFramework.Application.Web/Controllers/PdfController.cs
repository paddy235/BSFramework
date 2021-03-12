using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Controllers
{
    public class PdfController : Controller
    {
        //
        // GET: /Pdf/

        public ActionResult Index()
        {
            return View();
        }

        public ViewResult Detail(string id)
        {
            var bll = new FileInfoBLL();
            var entity = bll.GetEntity(id);
            if (entity != null)
            {
                ViewBag.url = this.Request.Url.Scheme + "://" + this.Request.Url.Host + Url.Content(entity.FilePath);
                //ViewBag.Ip = this.Request.Url.Scheme + "://" + this.Request.Url.Host;
                ViewBag.Ip = Config.GetValue("AppUrl");
                if (entity.FileExtensions.ToLower().Contains("doc")
                    || entity.FileExtensions.ToLower().Contains("docx"))
                {
                    string pdfFileName = entity.FileName.Substring(0, entity.FileName.LastIndexOf(".")) + ".pdf";
                    if (!System.IO.File.Exists(Server.MapPath(entity.FilePath)))
                    {
                        bool success = BSFramework.Util.Offices.Office2PDFHelper.DOCConvertToPDF(Server.MapPath(entity.FilePath), Server.MapPath("~/Resource/EmergencyPDF/" + entity.FileId + "_" + pdfFileName));
                        if (!success) throw new Exception("doc文件转换PDF文件失败，无法在线实现在线预览");
                    }
                    ViewBag.filePath = Url.Content("~/Resource/EmergencyPDF/" + entity.FileId + "_" + pdfFileName);

                }
                else
                {
                    ViewBag.filePath = Url.Content(entity.FilePath);
                }

            }

            return View();
        }

        public ViewResult ViewPDFPage(string httpUrl)
        {
            return View();
        }

    }
}
