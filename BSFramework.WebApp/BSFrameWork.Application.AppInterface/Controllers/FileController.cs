using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class FileController : ApiController
    {
        public HttpResponseMessage Post(string dirName)
        {
            var content = Request.Content;
            var uploadDir = System.Configuration.ConfigurationManager.AppSettings["FileDir"];
            string dir = HttpUtility.UrlDecode(dirName).Replace("/", "\\");
            string[] arr = dir.Split('\\');
            StringBuilder sb = new StringBuilder();
            foreach (string str in arr)
            {
                sb.Append(str + "\\");
                if (!Directory.Exists(uploadDir + sb.ToString()))
                {
                    Directory.CreateDirectory(uploadDir + sb.ToString());
                }
            }
            var newFileName = "";
            var sp = new MultipartMemoryStreamProvider();
            Task.Run(async () => await Request.Content.ReadAsMultipartAsync(sp)).Wait();

            foreach (var item in sp.Contents)
            {
                if (item.Headers.ContentDisposition.FileName != null)
                {
                    var filename = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                    newFileName = uploadDir + dir + "\\" + HttpUtility.UrlDecode(filename);
                    var ms = item.ReadAsStreamAsync().Result;
                    using (var br = new BinaryReader(ms))
                    {
                        var data = br.ReadBytes((int)ms.Length);
                        File.WriteAllBytes(newFileName, data);
                    }
                }
            }
            var result = new Dictionary<string, string>();
            result.Add("result", newFileName);
            var resp = Request.CreateResponse(HttpStatusCode.OK, result);
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("text/json");
            resp.Content.Headers.ContentType.CharSet = "utf-8";
            return resp;
        }
        public string Get(string filePath)
        {
            string result = "0";
            filePath = HttpContext.Current.Server.MapPath("~/upfile/" + filePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                result = "1";
            }
            else
            {
                result = "0";
            }
            return result;
        }
    }
}
