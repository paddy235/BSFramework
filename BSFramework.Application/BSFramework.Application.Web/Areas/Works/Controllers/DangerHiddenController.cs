using BSFramework.Application.Code;
using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BSFramework.Application.Entity.WorkMeeting;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class DangerHiddenController : MvcControllerBase
    {
        //
        // GET: /Works/DangerHidden/
        #region 视图功能
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region 获取数据

        /// <summary>
        /// 调用隐患数量统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetHtLevelChart(string StrTime)
        {
            #region  请求
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            //存放
            StringBuilder Str = new StringBuilder();
            var user = OperatorProvider.Provider.Current();//用户 user.Account
            //Str.Append("fgsgly" + "^");
            Str.Append(user.Account + "^");
            var IpUrl = Config.GetValue("SyncIp");
            //var IpUrl = "http://10.36.1.70/erchtms";
            var strparm = "{year:" + StrTime + "}";
            var GetUrl = IpUrl + "/HtStat/GetHtLevelChart?queryJson=" + strparm;
            Str.Append(GetUrl + "^");
            Str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^");
            Str.Append("DLBZ");
            var dese = BSFramework.Util.DESEncrypt.Encrypt(Str.ToString());
            string urlStr = string.Empty;
            var goUrl = IpUrl + "/login/signin?args=" + dese;
            wc.Encoding = Encoding.UTF8;
            //[{"y":3,"name":"重大隐患"},{"y":3,"name":"一般隐患"}]
            string content = wc.DownloadString(goUrl);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-隐患统计-数量统计\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", goUrl, GetUrl, content);
            #endregion
            //var data1 = ['一般隐患', '重大隐患'];
            //var arr1 = [{ name: '一般隐患', value: 90, }, { name: '重大隐患', value: 10 }];
            content = content.Replace("y", "value");
            var dataList = content.ToList<GetHtLevelChartJsonEntity>();
            if (content.Count() > 0)
            {

                string[] nameList = new string[dataList.Count()];
                float sum = 0;
                for (int i = 0; i < dataList.Count(); i++)
                {
                    nameList[i] = dataList[i].name;
                    sum = sum + dataList[i].value;
                }
                var dataNew = new List<GetHtLevelChartJsonEntity>();
                for (int i = 0; i < 2; i++)
                {
                    var name = i == 1 ? "一般隐患" : "重大隐患";
                    var check = dataList.FirstOrDefault(row => row.name == name);
                    if (check == null)
                    {
                        GetHtLevelChartJsonEntity one = new GetHtLevelChartJsonEntity();
                        one.name = name;
                        one.value = 0;
                        dataNew.Add(one);
                    }
                    else
                    {
                        dataNew.Add(check);
                    }
                }
                List<GetHtLevelChartJsonEntity> arr = new List<GetHtLevelChartJsonEntity>();
                List<GetHtLevelChartSumJsonEntity> sumJson = new List<GetHtLevelChartSumJsonEntity>();
                foreach (var item in dataNew)
                {
                    GetHtLevelChartJsonEntity one = new GetHtLevelChartJsonEntity();
                    one.name = item.name;
                    one.value = (item.value / sum) * 100;
                    arr.Add(one);
                    GetHtLevelChartSumJsonEntity onesum = new GetHtLevelChartSumJsonEntity();
                    onesum.name = item.name;
                    onesum.value = (int)item.value;
                    sumJson.Add(onesum);

                }

                GetHtLevelChartSumJsonEntity osum = new GetHtLevelChartSumJsonEntity();
                osum.name = "合计";
                osum.value = (int)sum;
                sumJson.Add(osum);
                GetHtLevelChartReturnEntity returnRsult = new GetHtLevelChartReturnEntity();
                returnRsult.sumJson = sumJson;
                returnRsult.data = nameList;
                returnRsult.arr = arr;
                var context = BSFramework.Util.Json.ToJson(returnRsult);
                return Content(context);
            }
            else
            {
                GetHtLevelChartReturnEntity returnRsult = new GetHtLevelChartReturnEntity();
                var context = BSFramework.Util.Json.ToJson(returnRsult);
                return Content(context);
            }
        }

        /// <summary>
        /// 隐患数量趋势图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetHtNumChart(string StrTime, string level)
        {
            #region
            //存放

            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            StringBuilder Str = new StringBuilder();
            var user = OperatorProvider.Provider.Current();//用户 user.Account
                                                           //Str.Append("fgsgly" + "^");
            Str.Append(user.Account + "^");

            var IpUrl = Config.GetValue("SyncIp");
            //var IpUrl = "http://10.36.1.70/erchtms";
            var strparm = "";

            if (level == "==请选择==")
            {
                strparm = "{year:" + StrTime + "}";
            }
            else
            {
                strparm = "{year:" + StrTime + ",level:'" + level + "'}";
            }
            var GetUrl = IpUrl + "/HtStat/GetHtNumChart?queryJson=" + strparm;
            Str.Append(GetUrl + "^");
            Str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^");
            Str.Append("DLBZ");
            var dese = BSFramework.Util.DESEncrypt.Encrypt(Str.ToString());
            string urlStr = string.Empty;
            var goUrl = IpUrl + "/login/signin?args=" + dese;
            wc.Encoding = Encoding.UTF8;
            //[{"name":"一般隐患","data":[0,0,0,0,0,0,0,0,0,0,0,0]}]
            string content = wc.DownloadString(goUrl);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-隐患统计-数量趋势图\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", goUrl, GetUrl, content);
            #endregion
            //    var data1 = [2.6, 5.9, 9.0, 26.4, 28.7, 70.7, 175.6, 182.2, 48.7, 18.8, 6.0, 2.3];
            var dataList = content.ToList<GetHtNumChartJsonEntity>();
            if (dataList.Count > 0)
            {
                int[] Sum = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < 12; i++)
                {
                    foreach (var item in dataList)
                    {
                        Sum[i] = Sum[i] + item.data[i];
                    }
                }
                var context = BSFramework.Util.Json.ToJson(Sum);
                return Content(context);
            }
            else
            {
                int[] Sum = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                var context = BSFramework.Util.Json.ToJson(Sum);
                return Content(context);
            }
        }

        /// <summary>
        /// 隐患整改统计图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetHtNumReadjustChart(string StrTime, string level)
        {
            #region
            //存放
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            StringBuilder Str = new StringBuilder();
            var user = OperatorProvider.Provider.Current();//用户 user.Account
                                                           //Str.Append("fgsgly" + "^");
            Str.Append(user.Account + "^");
            var IpUrl = Config.GetValue("SyncIp");
            //var IpUrl = "http://10.36.1.70/erchtms";
            var strparm = "";

            if (level.Contains("="))
            {
                strparm = "{year:" + StrTime + "}";
            }
            else
            {
                strparm = "{year:" + StrTime + ",level:'" + level + "'}";
            }
            var GetUrl = IpUrl + "/HtStat/GetHtNumReadjustChart?queryJson=" + strparm;
            Str.Append(GetUrl + "^");
            Str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^");
            Str.Append("DLBZ");
            var dese = BSFramework.Util.DESEncrypt.Encrypt(Str.ToString());
            string urlStr = string.Empty;
            var goUrl = IpUrl + "/login/signin?args=" + dese;
            wc.Encoding = Encoding.UTF8;
            //{"tdata":[{"month":"01","yvalue":0.0,"wvalue":0.0},{"month":"02","yvalue":0.0,"wvalue":0.0},
            //{ "month":"03","yvalue":0.0,"wvalue":0.0},{"month":"04","yvalue":0.0,"wvalue":0.0},
            //{ "month":"05","yvalue":0.0,"wvalue":0.0},{"month":"06","yvalue":0.0,"wvalue":0.0},
            //{ "month":"07","yvalue":0.0,"wvalue":0.0},{"month":"08","yvalue":2.0,"wvalue":1.0},
            //{ "month":"09","yvalue":0.0,"wvalue":0.0},{"month":"10","yvalue":0.0,"wvalue":0.0},
            //{ "month":"11","yvalue":0.0,"wvalue":0.0},{"month":"12","yvalue":0.0,"wvalue":0.0}],
            //"sdata":[{"name":"已整改","data":[0,0,0,0,0,0,0,2,0,0,0,0]},{"name":"未整改","data":[0,0,0,0,0,0,0,1,0,0,0,0]}]}
            string content = wc.DownloadString(goUrl);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-隐患统计-整改统计图\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", goUrl, GetUrl, content);
            #endregion
            // var data2 = [[175, 220], [242, 250]];

            var dataList = BSFramework.Util.Json.ToObject<GetHtNumReadjustChartEntity>(content);
            if (dataList.sdata.Count() > 0)
            {
                int[][] sum = new int[dataList.sdata.Count()][];
                for (int i = 0; i < dataList.sdata.Count(); i++)
                {
                    sum[i] = dataList.sdata[i].data;
                }
                var context = BSFramework.Util.Json.ToJson(sum);
                return Content(context);
            }
            else
            {
                int[][] sum = new int[0][];
                var context = BSFramework.Util.Json.ToJson(sum);
                return Content(context);
            }


        }

        /// <summary>
        /// 隐患整改变化趋势图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetHtNumChangeChart(string StrTime, string level)
        {
            #region
            //存放
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            StringBuilder Str = new StringBuilder();
            var user = OperatorProvider.Provider.Current();//用户 user.Account
            //Str.Append("fgsgly" + "^");
            Str.Append(user.Account + "^");
            var IpUrl = Config.GetValue("SyncIp");
            //var IpUrl = "http://10.36.1.70/erchtms";
            var strparm = "";

            if (level.Contains("="))
            {
                strparm = "{year:" + StrTime + "}";
            }
            else
            {
                strparm = "{year:" + StrTime + ",level:'" + level + "'}";
            }
            var GetUrl = IpUrl + "/HtStat/GetHtNumChangeChart?queryJson=" + strparm;
            Str.Append(GetUrl + "^");
            Str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^");
            Str.Append("DLBZ");
            var dese = BSFramework.Util.DESEncrypt.Encrypt(Str.ToString());
            string urlStr = string.Empty;
            var goUrl = IpUrl + "/login/signin?args=" + dese;
            wc.Encoding = Encoding.UTF8;
            // [{"name":"所有隐患","data":[0.0,0.0,0.0,0.0,0.0,0.0,0.0,50.0,0.0,0.0,0.0,0.0]
            //},{"name":"一般隐患","data":[0.0,0.0,0.0,0.0,0.0,0.0,0.0,33.33,0.0,0.0,0.0,0.0]
            //},{"name":"重大隐患","data":[0.0,0.0,0.0,0.0,0.0,0.0,0.0,66.67,0.0,0.0,0.0,0.0]}]
            string content = wc.DownloadString(goUrl);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-隐患统计-整改变化趋势图\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", goUrl, GetUrl, content);
            // var data2 = [[2.6, 5.9, 9.0, 26.4], [28.7, 70.7, 175.6, 182.2], [48.7, 18.8, 6.0, 2.3]];
            #endregion

            var dataList = content.ToList<GetHtNumChangeChartData>();
            if (dataList.Count() > 0)
            {
                float[][] sum = new float[dataList.Count()][];
                for (int i = 0; i < dataList.Count(); i++)
                {
                    sum[i] = dataList[i].data;
                }
                var context = BSFramework.Util.Json.ToJson(sum);
                return Content(context);
            }
            else
            {
                float[][] sum = new float[0][];
                var context = BSFramework.Util.Json.ToJson(sum);
                return Content(context);
            }
        }

        #endregion
    }
}
