using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.EmergencyManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Service.BusinessExceptions;
//using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TaskScheduler;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class LogController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> PostLog()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "log";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    if (File.Exists(Path.Combine(path, filename)))
                        File.Delete(Path.Combine(path, filename));

                    File.Move(file.LocalFileName, Path.Combine(path, filename));
                }

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
