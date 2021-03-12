using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using System;
using System.IO;
using System.Net;


namespace BSFramework.Application.Web.Areas
{
    /// <summary>
    /// 描 述：职位管理
    /// </summary>
    public class Erchtms : MvcControllerBase
    {

        /// <summary>
        /// 同步用户
        /// </summary>
        public bool ErchtmsSynchronoous(string type, object o, string account)
        {
            var baseUrl = Config.GetValue("ErchtmsApiUrl");

            if (Config.GetValue("SyncBool") == "f")
            {
                return true;
            }

            string id = "";
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            //发送请求到web api并获取返回值，默认为post方式
            try
            {
                System.Collections.Specialized.NameValueCollection nc = new System.Collections.Specialized.NameValueCollection();
                if (type == "SaveUser")
                {
                    UserEntity user = (UserEntity)o;
                    id = user.UserId;
                    nc.Add("account", account);
                    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                }
                else if (type == "SaveDept")
                {
                    DepartmentEntity d = (DepartmentEntity)o;
                    id = d.DepartmentId;
                    nc.Add("account", BSFramework.Application.Code.OperatorProvider.Provider.Current().Account);
                    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(d));
                }
                else if (type == "SaveRole")
                {
                    RoleEntity r = (RoleEntity)o;
                    id = r.RoleId;
                    nc.Add("account", BSFramework.Application.Code.OperatorProvider.Provider.Current().Account);
                    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(r));
                }
                if (type == "DeleteUser")
                {
                    UserEntity user = (UserEntity)o;
                    id = user.UserId;
                    nc.Add("account", account);
                    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                }
                else if (type == "DeleteDept")
                {
                    DepartmentEntity d = (DepartmentEntity)o;
                    id = d.DepartmentId;
                    nc.Add("account", BSFramework.Application.Code.OperatorProvider.Provider.Current().Account);
                    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(d));
                }
                else if (type == "DeleteRole")
                {
                    RoleEntity r = (RoleEntity)o;
                    id = r.RoleId;
                    nc.Add("account", BSFramework.Application.Code.OperatorProvider.Provider.Current().Account);
                    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(r));
                }
                else if (type == "UpdatePwd")
                {
                    string[] r = (string[])o;
                    nc.Add("userId", r[0]);
                    nc.Add("pwd", r[1]);
                    string path = baseUrl + "syncdata/" + type + "?userId=" + r[0] + "&pwd=" + r[1];
                    wc.UploadValuesCompleted += wc_UploadValuesCompleted;
                    wc.UploadValuesAsync(new Uri(path), nc);
                    return true;
                }


                wc.UploadValuesCompleted += wc_UploadValuesCompleted;
                string a = baseUrl + "syncdata/" + type + "?keyValue=" + id;
                wc.UploadValuesAsync(new Uri(baseUrl + "syncdata/" + type + "?keyValue=" + id), nc);

            }
            catch (Exception ex)
            {
                //将同步结果写入日志文件
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/logs")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/logs"));
                }

                System.IO.File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息" + Newtonsoft.Json.JsonConvert.SerializeObject(o) + ",异常信息：" + ex.Message + "\r\n");
                return false;
            }
            return true;
        }

        void wc_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            //将同步结果写入日志文件
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/logs/" + fileName);
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/logs")))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/logs"));
            }
            if (e.Error != null)
            {
                System.IO.File.AppendAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：" + e.Error.ToString() + "\r\n");
            }

        }
    }
}
