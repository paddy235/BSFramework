using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Bzzd.Sync
{
    public class SkSystem
    {
        public bool SyncTask(MeetingJobEntity job, string userid, string state)
        {
            string url = Config.GetValue("ErchtmsApiUrl");
            if (string.IsNullOrEmpty(url)) return true;

            JObject jobj = new JObject();
            jobj.Add(new JProperty("JobId", job.RecId));
            jobj.Add(new JProperty("StartTime", job.StartTime));
            jobj.Add(new JProperty("EndTime", job.EndTime));
            jobj.Add(new JProperty("TaskUserName", string.Join(",", job.Relation.JobUsers.Select(x => x.UserName))));
            jobj.Add(new JProperty("TaskUserId", string.Join(",", job.Relation.JobUsers.Select(x => x.UserId))));
            jobj.Add(new JProperty("SuperviseState", job.IsFinished == "finish" ? "3" : "2"));
            jobj.Add(new JProperty("state", state));
            jobj.Add(new JProperty("userid", userid));

            var json = jobj.ToString();
            try
            {
                var result = HttpMethods.HttpPost(url + "SyncData/SupervisionTask", "json=" + json);
                NLog.LogManager.GetCurrentClassLogger().Info("同步信息 {0}", json);
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncData>(result);
                //if (data.meta.success == "true")
                //    return true;
                //else
                //{
                //string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                //{
                //    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                //}

                //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步信息\r\n" + json + "|" + result + "\r\n\r\n");

                //}
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Info("同步信息 {0}, 异常 {1}", json, ex.Message);

                //string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                //{
                //    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                //}

                //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息\r\n" + json + ",异常信息：" + ex.Message + "\r\n\r\n");
            }

            return true;
        }
        public bool DelTask(string jobid)
        {
            string url = Config.GetValue("ErchtmsApiUrl");
            if (string.IsNullOrEmpty(url)) return true;

            JObject jobj = new JObject();
            jobj.Add(new JProperty("JobId", jobid));
            //jobj.Add(new JProperty("StartTime", job.StartTime));
            //jobj.Add(new JProperty("EndTime", job.EndTime));
            //jobj.Add(new JProperty("TaskUserName", string.Join(",", job.Relation.JobUsers.Select(x => x.UserName))));
            //jobj.Add(new JProperty("TaskUserId", string.Join(",", job.Relation.JobUsers.Select(x => x.UserId))));
            //jobj.Add(new JProperty("SuperviseState", job.IsFinished == "finish" ? "3" : "2"));
            //jobj.Add(new JProperty("state", state));
            //jobj.Add(new JProperty("userid", userid));

            var json = jobj.ToString();
            try
            {
                var result = HttpMethods.HttpPost(url + "SyncData/DelTask", "json=" + json);
                NLog.LogManager.GetCurrentClassLogger().Info("同步信息 {0}", json);
                //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncData>(result);
                //if (data.meta.success == "true")
                //    return true;
                //else
                //{
                //string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                //{
                //    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                //}

                //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步信息\r\n" + json + "|" + result + "\r\n\r\n");

                //}
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Info("同步信息 {0}, 异常 {1}", json, ex.Message);

                //string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\logs"))
                //{
                //    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\logs");
                //}

                //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + fileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息\r\n" + json + ",异常信息：" + ex.Message + "\r\n\r\n");
            }

            return true;
        }
    }
}
