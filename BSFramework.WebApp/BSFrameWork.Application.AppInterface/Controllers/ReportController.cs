using Aspose.Cells;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.IWarning;
using Bst.Fx.Warning;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class ReportController : BaseApiController
    {

        private ReportBLL _reportBLL;

        public ReportController() : base()
        {
            _reportBLL = new ReportBLL();
        }

        /// <summary>
        /// 生成工作总结
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public string Build(ReportEntity args)
        {
            var bll = new ReportBLL();
            var data = bll.Build(args.ReportType);

            foreach (var item in data)
            {
                var user = new UserBLL().GetEntity(item.ReportUserId);
                IWarningService service = new WarningService();
                service.SendWarning(item.ReportType, item.ReportId.ToString());
                //if (user != null)
                //MessageClient.SendRequest(new string[] { user.Account }, item.ReportId.ToString(), "工作总结", item.ReportType, "新工作总结");
            }

            return "ssss";
        }

        /// <summary>
        /// 获取我相关的工作总结
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetReports()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<ReportEntity>>(json);
            var bll = new ReportBLL();
            var total = 0;
            var data = bll.GetReports(args.Data.Category, args.Data.ReportType, args.Data.ReportUserId, args.Data.StartTime, args.Data.EndTime, null, args.PageSize, args.PageIndex, out total, args.UserId, args.Data.UnRead);
            foreach (var item in data)
            {
                if (item.EndTime.HasValue) item.EndTime = item.EndTime.Value.AddDays(-1);
            }
            return new { code = 0, info = string.Empty, data, total };
        }

        /// <summary>
        /// 获取某人的工作总结
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetReportsByUser()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var bll = new ReportBLL();
            var total = 0;
            var data = bll.GetReportsByUser(args.Data, args.PageSize, args.PageIndex, out total);
            return new { code = 0, info = string.Empty, data, total };
        }

        /// <summary>
        /// 提交工作总结
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SubmitReport()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<ReportEntity>>(json);
            args.Data.ReportUserId = args.UserId;
            var bll = new ReportBLL();
            var data = bll.Submit(args.Data);
            var users = args.Data.ToUserId.Split(',');

            var messagebll = new MessageBLL();
            messagebll.SendMessage("评价工作总结", args.Data.ReportId.ToString());
            //foreach (var item in users)
            //{
            //    //var user = new UserBLL().GetEntity(item);
            //    //if (user != null)
            //    //    MessageClient.SendRequest(new string[] { user.Account }, data.ReportId.ToString(), "工作总结", data.ReportType, "提交工作总结");
            //}

            return new { code = 0, info = string.Empty };
        }

        /// <summary>
        /// 获取某人工作总结
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetReport()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var bll = new ReportBLL();
            var data = bll.GetReport(args.Data, args.UserId);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            if (!string.IsNullOrEmpty(data.FilePath))
                data.FilePath = url + data.FilePath.Replace("~/", string.Empty);
            if (data.EndTime.HasValue) data.EndTime = data.EndTime.Value.AddDays(-1);
            return new { code = 0, info = string.Empty, data };
        }

        /// <summary>
        /// 分享
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Share()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<ReportEntity>>(json);
            var users = args.Data.ShareId.Split(',');
            var bll = new ReportBLL();
            var userdata = bll.Share(args.Data.ReportId.ToString(), users);
            var report = bll.GetDetail(args.Data.ReportId.ToString());
            //foreach (var item in userdata)
            //{
            //    var user = new UserBLL().GetEntity(item);
            //    if (user != null)
            //        MessageClient.SendRequest(new string[] { user.Account }, report.ReportId.ToString(), "工作总结", report.ReportType, "分享工作总结");
            //}

            return new { code = 0, info = string.Empty };
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Comment()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<CommentEntity>>(json);
            var bll = new ReportBLL();
            bll.Comment(args.Data);
            return new { code = 0, info = string.Empty };
        }

        [HttpPost]
        public object GetSubmitPerson()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var bll = new ReportBLL();
            var data = bll.GetSubmitPerson(args.Data);
            return new { code = 0, info = string.Empty, data };
        }

        [HttpPost]
        public ModelBucket<ReportEntity> GetDetail(ParamBucket<string> paramBucket)
        {
            var data = _reportBLL.GetDetail(paramBucket.Data);
            return new ModelBucket<ReportEntity> { Data = data, Success = true };
        }

        public ResultBucket EditReport(ParamBucket<ReportEntity> paramBucket)
        {
            if (paramBucket.Data.TaskList != null)
            {
                foreach (var item in paramBucket.Data.TaskList)
                {
                    if (string.IsNullOrEmpty(item.ReportTaskId)) item.ReportTaskId = Guid.NewGuid().ToString();
                    item.ReportId = paramBucket.Data.ReportId;
                }
            }
            _reportBLL.EditReport(paramBucket.Data);
            return new ResultBucket { Success = true };
        }
    }
}
