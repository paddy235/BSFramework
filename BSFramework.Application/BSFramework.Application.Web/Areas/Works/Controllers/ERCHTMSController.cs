using BSFramework.Application.Code;
using BSFramework.Util;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Web.Mvc;
namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class ERCHTMSController : MvcControllerBase
    {


        string path;
        /// <summary>
        ///查询
        /// </summary>
        public ActionResult List(int page, int pagesize, FormCollection fc)
        {
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;
            var WorkStream = fc.Get("WorkStream");
            var HidType = fc.Get("HidType");
            var HidRank = fc.Get("HidRank");
            var SaftyCheckType = fc.Get("SaftyCheckType");
            var ChangeStatus = fc.Get("ChangeStatus");
            var StartTime = fc.Get("from");
            var EndTime = fc.Get("to");
            var IsBreakRule= fc.Get("IsBreakRule");
            var IsExposureState = fc.Get("IsExposureState");

            string useAccountr = BSFramework.Application.Code.OperatorProvider.Provider.Current().Account;
            string url = Config.GetValue("SyncIp") + "/HiddenTroubleManage/HTBaseInfo/GetListJson?queryJson={code:\"001\",WorkStream:\"" + WorkStream + "\",HidType:\"" + HidType + "\",HidRank:\"" + HidRank + "\",SaftyCheckType:\"" + SaftyCheckType + "\",ChangeStatus:\"" + ChangeStatus + "\",StartTime:\"" + StartTime + "\",EndTime:\"" + EndTime + "\",HidDescribe:\"\",IsBreakRule:\""+ IsBreakRule + "\",IsExposureState:\""+ IsExposureState + "\"}&rows="+pagesize+"&page="+ page + "&sidx=createdate&sord=desc";

            string args = BSFramework.Util.DESEncrypt.Encrypt(useAccountr + "^" +url + "^" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^DLBZ");

            path = Config.GetValue("SyncIp") + "/login/signin?args=" + args;
            HttpWebRequest wrq = (HttpWebRequest)HttpWebRequest.Create(Config.GetValue("SyncIp") + "/login/signin?args=" + args);
            wrq.Method = "GET";
            System.Net.WebResponse wrp = wrq.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(wrp.GetResponseStream());
            string json = sr.ReadToEnd();
            BSFramework.Application.Web.Areas.Works.Models.HTBaseInfoModel entity=Newtonsoft.Json.JsonConvert.DeserializeObject<BSFramework.Application.Web.Areas.Works.Models.HTBaseInfoModel>(json);

            foreach (BSFramework.Application.Web.Areas.Works.Models.HTBaseInforows a in entity.rows)
            {
                a.Id = Config.GetValue("SyncIpName") + "/HiddenTroubleManage/HTApproval/Form?keyValue=" +a.Id+ "&actiontype=view";
            }
            //view-source:http://10.36.1.70/ERCHTMS/HiddenTroubleManage/HTApproval/Form?keyValue=7c47cca7-e61e-4b56-ab46-bf786e84b5ba&actiontype=view
            //ViewBag.path = Config.GetValue("SyncIp") + "/erchtms/login/signin?args=" + args;
            // ViewBag.getList = "jQuery(function(){getList(path);});";

            var data = entity.rows;
            ViewBag.pages =  Math.Ceiling((decimal) entity.records / pagesize);;
            ViewBag.current = page;
            ViewBag.pagesize = pagesize;
            return View(data);
        }


        private static object webClient(string url, string val) 
        {
            var webclient = new WebClient();
            var ApiIp = Config.GetValue("ErchtmsApiUrl");
            NameValueCollection postVal = new NameValueCollection();
            postVal.Add("json", val);
            var getData = webclient.UploadValues(ApiIp + url, postVal);
            var result = System.Text.Encoding.UTF8.GetString(getData);
            return result;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string HomeIndex(string mode)
        {
            var user = OperatorProvider.Provider.Current();
            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"mode\":\"{0}\"", mode);
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            var dyresult = new object();
            if (mode == "4" || mode == "6")
                dyresult = webClient("Home/getBZPlatformStatisticsInfo", valueStr);
            else
                dyresult = webClient("Home/getBZPlatformStatisticsInfo", valueStr);


            return dyresult.ToString();

        }

     
    }
}
