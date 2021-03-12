using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using System.IO;
using BSFramework.Application.Code;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Busines.WorkMeeting;
using System.Text.RegularExpressions;
using BSFramework.Application.Entity.PeopleManage;
using BSFrameWork.Application.AppInterface.Models;
using BSFramework.Application.Busines.Activity;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class MainController : ApiController
    {
        DepartmentBLL dtbll = new DepartmentBLL();
        PeopleBLL pbll = new PeopleBLL();
        UserBLL ubll = new UserBLL();
        RoleBLL rbll = new RoleBLL();
        FileInfoBLL fileBll = new FileInfoBLL();
        WorkmeetingBLL workmeetingbll = new WorkmeetingBLL();
        NoticeBLL noticeBLL = new NoticeBLL();
        /// <summary>
        /// 今日工作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetJobs([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                var data = workmeetingbll.GetGroupJobs(user.DepartmentId).OrderByDescending(x => x.IsFinished);
                var filebll = new FileInfoBLL();
                var trainingtype = Config.GetValue("TrainingType");
                var bll2 = new HumanDangerTrainingBLL();
                foreach (MeetingJobEntity n in data)
                {
                    n.Status = n.IsFinished == "finish" ? "已完成" : n.IsFinished == "undo" ? "进行中" : "已取消";
                    n.Files = filebll.GetFileList(n.Relation.MeetingJobId);
                    n.FileList1 = n.Files.Where(x => x.Description == "音频").Select(x => new FileInfoEntity() { FileId = x.FileId, FileName = x.FileName, FilePath = url + x.FilePath.Replace("~/", string.Empty) }).ToList();
                    n.FileList2 = n.Files.Where(x => x.Description == "照片").Select(x => new FileInfoEntity() { FileId = x.FileId, FileName = x.FileName, FilePath = url + x.FilePath.Replace("~/", string.Empty) }).ToList();
                    var jobusers = workmeetingbll.GetJobUsers(n.JobId, null);
                    var checkeruesr = jobusers.FirstOrDefault(x => x.JobType == "ischecker");
                    if (checkeruesr == null) continue;
                    PeopleEntity p = pbll.GetEntity(checkeruesr.UserId);
                    if (p != null)
                    {
                        n.Remark = p.Photo;
                    }

                    if (trainingtype == "人身风险预控")
                    {
                        var trainings = bll2.GetListByUserIdJobId(n.Relation.MeetingJobId, userId);

                        //item.NeedTrain = trainings.Count > 0 ? true : false;
                        if (n.NeedTrain)
                        {
                            //判断风险预控完成情况获取所有人员
                            var alltrainings = bll2.GetListByJobId(n.Relation.MeetingJobId);
                            var finish = alltrainings.Where(x => x.IsDone && x.IsMarked).ToList();
                            if (alltrainings.Count == finish.Count)
                            {
                                n.TrainingDone = true;
                            }
                            else
                            {
                                n.TrainingDone = false;
                            }
                        }
                    }

                }

                return new { code = 0, info = "获取数据成功", count = data.Count(), data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 通知公告
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetNotice([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                var data = noticeBLL.GetCurrentNotice(user.DepartmentId, DateTime.Today.AddDays(-7));
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                url = url.Substring(0, url.Length - 1);
                foreach (NewsEntity n in data)
                {
                    n.fileList = new FileInfoBLL().GetFilesByRecId(n.NewsId, url);
                }
                return new { code = 0, info = "获取数据成功", count = data.Count(), data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 获取公司名称
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetCompanyName()
        {
            try
            {
                var name = Config.GetValue("CustomerCompanyName");

                return new { code = 0, info = "获取数据成功", data = name };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetPrefix()
        {
            var name = Config.GetValue("TrainingPrefix");
            return new { code = 0, info = "获取数据成功", data = name };
        }

        /// <summary>
        /// 获取工作标准列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetWorkStandardList()
        {
            WorkStandardBLL workStandardBLL = new WorkStandardBLL();
            FileInfoBLL fileInfoBLL = new FileInfoBLL();
            var dataList = workStandardBLL.GetAllList();
            return new { code = 0, info = "获取数据成功", data = dataList };
        }
    }
}
