using cn.jpush.api;
using cn.jpush.api.common;
using cn.jpush.api.common.resp;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class JPushClient
    {
        /// <summary>
        /// 应用标识：极光推送的用户名
        /// </summary>
        private static string AppKey = ConfigurationManager.AppSettings["AppKey"];
        /// <summary>
        /// 极光推送的密码
        /// </summary>
        private static string MasterSecret = ConfigurationManager.AppSettings["MasterSecret"];
        /// <summary>
        /// 是不使用极光推送
        /// </summary>
        private static string IsUseJPush = ConfigurationManager.AppSettings["IsUseJPush"];
        /// <summary>
        /// 移动端应用名称
        /// </summary>
        private static string AppName = ConfigurationManager.AppSettings["AppName"];
        /// <summary>
        /// 演示环境
        /// </summary>
        private static string DemoKey = ConfigurationManager.AppSettings["DemoKey"];

        /// <summary>
        /// 使用极光推送服务发消息
        /// </summary>
        /// <param name="alias">接收人账号</param>
        /// <param name="id">消息主键</param>
        /// <param name="msgtype">消息类型</param>
        /// <param name="title">消息标题</param>
        /// <param name="content">消息内容</param>
        /// <returns></returns>
        public static bool SendRequest(string[] alias, string id, string msgtype, string title, string content)
        {
            bool r = false;

            if (IsUseJPush == "1")
            {
                cn.jpush.api.JPushClient client = new cn.jpush.api.JPushClient(AppKey, MasterSecret);
                PushPayload payload = PushObject_All_All_Alert(alias, content, title, msgtype, id);
                try
                {
                    var result = client.SendPush(payload);
                    r = true;
                }
                catch (APIRequestException e)
                {
                    //string str = "Date: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n";
                    //str += "===============================================\r\n";
                    //str += "推送对象：[" + string.Join("|", alias) + "]。\r\n";
                    //str += "推送消息：" + content + "\r\n";
                    //str += "HTTP Status：" + e.Status + "\r\n";
                    //str += "Error Code：" + e.ErrorCode + "\r\n";
                    //str += "Error Message：" + e.Message + "\r\n";
                    //string logAddr = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpFile", "ErrLog", "极光推送" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                    //WriteLog(logAddr, str);

                    NLog.LogManager.GetCurrentClassLogger().Error("极光推送异常，{0}", e.Message);

                    r = false;
                }
                catch (APIConnectionException e)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error("极光推送异常，{0}", e.message);

                    //string str = "Date: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n";
                    //str += "===============================================\r\n";
                    //str += "推送对象：[" + string.Join("|", alias) + "]。\r\n";
                    //str += "推送消息：" + content + "\r\n";
                    //str += "Error Message：" + e.Message + "\r\n";
                    //string logAddr = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpFile", "ErrLog", "极光推送" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                    //WriteLog(logAddr, str);
                    r = false;
                }
            }

            return r;
        }

        private static PushPayload PushObject_All_All_Alert(string[] alias, string alert, string title, string moduleKey, string moduleValue)
        {
            if (!string.IsNullOrWhiteSpace(DemoKey))
            {
                var newAlias = new List<string>();
                Array.ForEach(alias, item => { newAlias.Add(DemoKey + item); });
                alias = newAlias.ToArray();
            }
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.s_alias(alias);
            var notification = new Notification();

            string ModuleKey = "ModuleKey";
            string ModuleValue = string.Format("{0}|{1}", moduleKey, moduleValue);

            if (!string.IsNullOrEmpty(ModuleKey) && !string.IsNullOrEmpty(ModuleValue))
            {
                notification.AndroidNotification = new AndroidNotification().setAlert(alert).AddExtra(ModuleKey, ModuleValue).setTitle(title);
                notification.IosNotification = new IosNotification().setAlert(alert).incrBadge(1).setSound("happy").AddExtra(ModuleKey, ModuleValue);
            }
            else
            {
                notification.AndroidNotification = new AndroidNotification().setAlert(alert).setTitle(title);
                notification.IosNotification = new IosNotification().setAlert(alert).incrBadge(1).setSound("happy");
            }
            pushPayload.notification = notification;

            string apns_production = ConfigurationManager.AppSettings["apns_production"];
            pushPayload.options.apns_production = (!string.IsNullOrEmpty(apns_production) && apns_production.ToLower() == "true") ? true : false;
            return pushPayload;
        }
    }
}
