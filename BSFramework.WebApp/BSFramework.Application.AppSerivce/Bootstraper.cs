using Nancy;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using BSFramework.Util;
using System.IO;

namespace BSFramework.Application.AppSerivce
{
    /// <summary>
    /// 描 述:初始化
    /// </summary>
    public class Bootstraper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                                .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
            //INSERT INTO BASE_AREA (AREAID, PARENTID, AREACODE, AREANAME, QUICKQUERY, SIMPLESPELLING, LAYER, SORTCODE, DELETEMARK, ENABLEDMARK, DESCRIPTION, CREATEDATE, CREATEUSERID, CREATEUSERNAME, MODIFYDATE, MODIFYUSERID, MODIFYUSERNAME)
        }
    }
}