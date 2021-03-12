using Aspose.Cells;
using Aspose.Slides;
using Bst.Fx.Uploading.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{
    public class Uploader
    {
        public void UploadVideo(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Trace("开始上传 {0}", id);

            var entity = default(AttachmentEntity);
            using (var ctx = new DataContext())
            {
                entity = ctx.Attachments.Find(id);
            }

            if (entity == null) return;

            var time = DateTime.Now.AddYears(10);
            var appkey = ConfigurationManager.AppSettings["appkey2"].ToString();
            string code = Md5(appkey + GetTimeStamp(time));

            var dict = new Dictionary<string, string>();
            dict.Add("userKey", "user");
            dict.Add("fileName", entity.FileName);
            dict.Add("fileType", entity.Extention.Trim('.'));
            dict.Add("crypto", "0");
            dict.Add("timeStamp", GetTimeStamp(time).ToString());
            dict.Add("sign", code);
            dict.Add("wkpFlag", (ConfigurationManager.AppSettings["wkpFlag"] ?? "0").ToString());

            var urlparam = string.Join("&", dict.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
            var videosystem = ConfigurationManager.AppSettings["videosystem"].ToString();
            var initurl = "/api/v1/file/init";

            var webclient = new WebClientPro();
            var json = webclient.DownloadString(string.Format("{0}{1}?{2}", videosystem, initurl, urlparam));
            logger.Trace(json);
            var initdata = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage>(json);
            if (initdata.status != "200")
            {
                logger.Trace("初始化失败 {1}，{0}", initdata.msg, id);
            }

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var path = Path.Combine(filepath, entity.FilePath.Replace("~/Resource/", "").Replace('/', '\\'));
            if (!File.Exists(path))
            {
                throw new IOException(string.Format("未找到文件 {1}，{0}", path, id));
            }

            //time = DateTime.Now.AddSeconds(180);
            dict["timeStamp"] = GetTimeStamp(time).ToString();
            dict.Add("token", initdata.result.token);
            dict.Add("offset", "0");
            urlparam = string.Join("&", dict.Select(x => string.Format("{0}={1}", x.Key, x.Value)));

            var uploadurl = "/api/v1/file/upload";
            byte[] buffer = null;
            try
            {
                buffer = webclient.UploadFile(string.Format("{0}{1}?{2}", videosystem, uploadurl, urlparam), path);
            }
            catch (Exception ex)
            {
                logger.Error("上传异常 {1}，{0}", ex.Message, id);
                return;
            }
            json = Encoding.Default.GetString(buffer);
            logger.Trace(json);
            var uploaddata = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonMessage>(json);
            if (uploaddata.status != "200")
            {
                logger.Trace("上传失败 {1}，{0}", uploaddata.msg, id);
                return;
            }

            //time = DateTime.Now.AddSeconds(180);

            dict = new Dictionary<string, string>();
            dict.Add("appid", "001");
            dict.Add("srcFid", uploaddata.result.fid);
            dict.Add("expires", GetTimeStamp(time).ToString());
            var appcode = ConfigurationManager.AppSettings["appcode"].ToString();
            var enckey = Md5(appcode + uploaddata.result.fid + GetTimeStamp(time));
            dict.Add("token", enckey);
            dict.Add("wkpFlag", (ConfigurationManager.AppSettings["wkpFlag"] ?? "0").ToString());
            urlparam = string.Join("&", dict.Select(x => string.Format("{0}={1}", x.Key, x.Value)));

            using (var ctx = new DataContext())
            {
                var playurl = ConfigurationManager.AppSettings["playsystem"].ToString();
                entity = ctx.Attachments.Find(id);
                entity.OtherUrl = string.Format("{0}{1}?{2}", playurl, "/play.do", urlparam);
                ctx.SaveChanges();
            }
            logger.Trace("上传成功，{0}", id);
        }

        public bool Query(string url)
        {
            url = url.Replace("play", "queryPublishFileInfo");

            var webclient = new WebClient();
            byte[] buffer = null;
            buffer = webclient.DownloadData(url);
            var json = Encoding.UTF8.GetString(buffer);
            JObject jobject = null;
            try
            {
                jobject = Newtonsoft.Json.Linq.JObject.Parse(json);
            }
            catch (Exception)
            {
                return false;
            }
            if (jobject.SelectToken("success").Value<bool>())
            {
                var code = jobject.SelectToken("dataObj.srcFileStatusName").Value<string>();
                return code == "24" || code == "12";
            }
            else
                return false;
        }

        private static string Md5(string key)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var ary = md5.ComputeHash(Encoding.UTF8.GetBytes((key)));
            var sb = new StringBuilder();
            for (int i = 0; i < ary.Length; i++)
            {
                sb.Append(ary[i].ToString("x2"));
            }
            var code = sb.ToString();
            return code;
        }

        private int GetTimeStamp(DateTime time)
        {
            var start = new DateTime(1970, 1, 1);
            return (int)(time - start).TotalSeconds;
        }

        #region Office转PDF
        /// <summary>
        /// office文件转PDF文件
        /// </summary>
        /// <param name="sourcePath">要转换的文件的地址</param>
        /// <param name="targetPath">转换后文件存放的地址</param>
        public void ConvertPdf(string sourcePath, string targetPath)
        {

            //1、获取文件名，判断文件的类型
            var extension = sourcePath.Substring(sourcePath.LastIndexOf('.') + 1).ToLower();
            switch (extension)
            {
                case "doc":
                case "docx":
                    DOCConvertToPDF(sourcePath, targetPath);
                    break;
                case "xls":
                case "xlsx":
                    XLSConvertToPDF(sourcePath, targetPath);
                    break;
                case "ppt":
                case "pptx":
                    PPTConvertToPDF(sourcePath, targetPath);
                    break;
                default:
                    throw new Exception($"传的文件不是Office文件{sourcePath}");
            }
        }

        public static void DOCConvertToPDF(string sourcePath, string targetPath)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(sourcePath);
            doc.Save(targetPath, Aspose.Words.SaveFormat.Pdf);
        }


        public static void XLSConvertToPDF(string sourcePath, string targetPath)
        {
            var book = new Workbook(sourcePath);
            book.Save(targetPath, SaveFormat.Pdf);
        }


        public static void PPTConvertToPDF(string sourcePath, string targetPath)
        {
            Presentation ppt = new Presentation(sourcePath);
            ppt.Save(targetPath, Aspose.Slides.Export.SaveFormat.Pdf);
        }
        #endregion
    }
}
