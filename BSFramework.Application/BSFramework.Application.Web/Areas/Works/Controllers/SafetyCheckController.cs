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
    public class SafetyCheckController : MvcControllerBase
    {
        //
        // GET: /Works/SafetyCheck/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StrTime"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCheckNumChart(string StrTime, string ctype)
        {
            #region
            //存放

            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            StringBuilder Str = new StringBuilder();
            var user = OperatorProvider.Provider.Current();//用户 user.Account
            //Str.Append("rlb" + "^");
            Str.Append(user.Account + "^");

            var IpUrl = Config.GetValue("SyncIp");
            //var IpUrl = "http://10.36.1.70/erchtms";
            var strparm = "";
            string GetUrl = string.Empty;
            if (ctype == "全部")
            {
                GetUrl = IpUrl + "/HtStat/GetCheckNumChart?year=" + StrTime;
            }
            else
            {
                GetUrl = IpUrl + "/HtStat/GetCheckNumChart?year=" + StrTime + "&ctype='" + ctype + "'";
            }
            Str.Append(GetUrl + "^");
            Str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^");
            Str.Append("DLBZ");
            var dese = BSFramework.Util.DESEncrypt.Encrypt(Str.ToString());
            string urlStr = string.Empty;
            var goUrl = IpUrl + "/login/signin?args=" + dese;
            wc.Encoding = System.Text.Encoding.UTF8;
            // {"total":1,"page":1,"records":12,"rows":[{"month":"1月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0}
            // ,{"month":"2月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0}
            //,{ "month":"3月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0}
            //,{ "month":"4月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0}
            //,{ "month":"5月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0},
            //{ "month":"6月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0},
            // { "month":"7月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0},
            //{ "month":"8月","rc":3,"zx":6,"jj":0,"jjr":1,"sum":10,"zh":0},
            //{ "month":"9月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0},
            //{ "month":"10月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0},
            //{ "month":"11月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0},
            //{ "month":"12月","rc":0,"zx":0,"jj":0,"jjr":0,"sum":0,"zh":0}]}
            string content = wc.DownloadString(goUrl);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-安全检查\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", goUrl, GetUrl, content);
            #endregion
            // var data2 = [[175, 220], [242, 250]];        
            var dataList = BSFramework.Util.Json.ToObject<SafetyHazardChart>(content);
            if (dataList.rows.Count() > 0)
            {
                int[][] sum = new int[6][];
                for (int j = 0; j < 6; j++)
                {
                    int[] one = new int[12];
                    for (int i = 0; i < dataList.rows.Count(); i++)
                    {
                        switch (j)
                        {
                            case 0:
                                one[i] = dataList.rows[i].rc;
                                break;
                            case 1:
                                one[i] = dataList.rows[i].zx;
                                break;
                            case 2:
                                one[i] = dataList.rows[i].jj;
                                break;
                            case 3:
                                one[i] = dataList.rows[i].jjr;
                                break;
                            case 4:
                                one[i] = dataList.rows[i].zh;
                                break;
                            default:
                                one[i] = dataList.rows[i].sum;
                                break;
                        }



                    }
                    sum[j] = one;
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
    }
}
