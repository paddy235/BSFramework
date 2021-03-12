using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Bzzd.Sync
{
    public class TrainingSystem
    {
        public bool SyncDept(Dictionary<DepartmentEntity, DepartmentEntity> depts)
        {
            var prefix = (ConfigurationManager.AppSettings["TrainingPrefix"] ?? string.Empty).ToString();

            string sync = Config.GetValue("TrainingSync");
            if (sync != "1") return true;

            var jarray = new JArray();
            foreach (var item in depts)
            {
                JObject jobject = new JObject();
                jobject.Add(new JProperty("Id", prefix + item.Key.DepartmentId));
                jobject.Add(new JProperty("deptName", item.Key.FullName));
                jobject.Add(new JProperty("deptCode", prefix + item.Key.EnCode));
                jobject.Add(new JProperty("parentCode", prefix + (item.Value ?? new DepartmentEntity() { EnCode = "00" }).EnCode));
                jobject.Add(new JProperty("parentID", item.Value == null ? "-1" : (prefix + item.Value.DepartmentId)));
                jarray.Add(jobject);
            }

            JObject jobj = new JObject();
            jobj.Add(new JProperty("Business", "SavedeptInfro"));
            jobj.Add(new JProperty("DpetInfo", jarray));

            var json = jobj.ToString();
            var url = Config.GetValue("TrainingSystem");
            try
            {
                var result = HttpMethods.HttpPost(url, "json=" + json);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncData>(result);
                if (data.meta.success == "true")
                {
                    string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                    }

                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据成功，同步信息\r\n" + json + ",结果：" + data.meta.message + "\r\n\r\n");

                }
                else
                {
                    string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                    }

                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息\r\n" + json + ",异常信息：" + data.meta.message + "\r\n\r\n");

                }
            }
            catch (Exception ex)
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                }

                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息\r\n" + json + ",异常信息：" + ex.Message + "\r\n\r\n");
            }

            return true;
        }

        public bool SyncUser(List<UserEntity> users)
        {
            var prefix = (ConfigurationManager.AppSettings["TrainingPrefix"] ?? string.Empty).ToString();

            string sync = Config.GetValue("TrainingSync");
            if (sync != "1") return true;

            var jarray = new JArray();
            foreach (var item in users)
            {
                JObject jobject = new JObject();
                jobject.Add(new JProperty("Id", item.UserId));
                jobject.Add(new JProperty("userName", item.RealName));
                jobject.Add(new JProperty("userAccount", item.Account));
                jobject.Add(new JProperty("pwd", "abc1234"));
                jobject.Add(new JProperty("IdCard", item.IDENTIFYID));
                jobject.Add(new JProperty("departid", prefix + item.DepartmentId));
                jobject.Add(new JProperty("role", "0"));
                jobject.Add(new JProperty("userkind", "一般人员"));
                jobject.Add(new JProperty("telephone", item.Telephone));
                jarray.Add(jobject);
            }

            JObject jobj = new JObject();
            jobj.Add(new JProperty("Business", "saverUser"));
            jobj.Add(new JProperty("UserInfo", jarray));

            var json = jobj.ToString();
            var url = Config.GetValue("TrainingSystem");
            try
            {
                var result = HttpMethods.HttpPost(url, "json=" + json);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncData>(result);
                //if (data.meta.success == "true")
                //    return true;
                //else
                //{
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                }

                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息" + json + ",异常信息：" + data.meta.message + "\r\n");

                //}
            }
            catch (Exception ex)
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                }

                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息" + json + ",异常信息：" + ex.Message + "\r\n");
            }

            return true;
        }
    }

    public class SyncData
    {
        public MetaData meta { get; set; }
    }

    public class MetaData
    {
        public string success { get; set; }
        public string message { get; set; }
    }
}
