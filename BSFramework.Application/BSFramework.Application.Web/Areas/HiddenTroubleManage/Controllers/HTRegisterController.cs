using BSFramework.Application.Code;
using BSFramework.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.HiddenTroubleManage.Controllers
{
    public class HTRegisterController : Controller
    {
        //
        // GET: /HiddenTroubleManage/HTRegister/

        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            return View();
        }

        /// <summary>
        /// 首页分布页
        /// </summary>
        /// <returns></returns>
        public ViewResult IndexPartView()
        {



            Operator user = OperatorProvider.Provider.Current();
            ViewBag.UserId = user.UserId;
            return View();
        }

        /// <summary>
        /// 获取首页违章统计图的数据
        /// </summary>
        /// <param name="dateType"></param>
        /// <returns></returns>
        public JsonResult GetIndexData(string dateType)
        {
            //正式

            //测试
            //string Value = "{ userid:\"9098221b-6bda-41bd-84d1-79c31877f6fb\", data:{ themetype:0,PaltformType:0} }";
       
            string url = Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Hidden", "GetWzPieStatis");
            if (Debugger.IsAttached)
            {
                url = "http://10.36.1.72/erchtmsapp/api/Hidden/GetWzPieStatis ";
            }
            var requestParams = new
            {
                business = "GetWzPieStatis",
                userid = OperatorProvider.Provider.Current().UserId,
                tokenid = "",
                data = new
                {
                    seltype = dateType,
                    stattype = "0"
                }
            };
            var para = "json=" + Url.Encode(JsonConvert.SerializeObject(requestParams));
            string responseStr = HttpMethods.HttpPost(url, para);
            return Json(responseStr, JsonRequestBehavior.AllowGet);
        }
    }
}
