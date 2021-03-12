using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.ToolManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Util;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        public object Test(string id)
        {
            //using (var factory = new ChannelFactory<IUploadService>("upload"))
            //{
            //    var channel = factory.CreateChannel();
            //    channel.Upload(id);
            //}
            return id;

            //var upload = new Uploader();
            //////upload.upload(id);
            //var url = "http://content-dev.bosafe.com/play.do?appid=001&srcFid=c511d865b1ae4e798558356538a0425c.mp4&expires=1543313959&token=00da100a13ea5fd8cdd6e300d05b03c1";
            //var status = upload.Query(url);
            //return status;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Upload()
        {
            Thread.Sleep(1800000);


            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, "ok");
        }
    }
}