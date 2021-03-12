using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFrameWork.Application.AppInterface.Models;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Busines.EvaluateAbout;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Util;
using System.Threading.Tasks;
using System.IO;
using BSFramework.Application.Busines.SevenSManage;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class HomeController : BaseApiController
    {

        PeopleBLL pbll = new PeopleBLL();
        WorkmeetingBLL wbll = new WorkmeetingBLL();
        [HttpPost]
        public object GetJobList([FromBody]JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string uid = dy.userId;
            FileInfoBLL bll = new FileInfoBLL();
            var data = wbll.GetMyJobs(uid, DateTime.Now);
            if (data == null) data = new List<MeetingJobEntity>();
            foreach (var item in data)
            {
                var file = bll.GetFilesByRecIdNew(item.Relation.MeetingJobId);
                item.Files = file.Where(x => x.Description == "照片" && x.CreateUserId == uid).ToList();
                item.FileList1 = file.Where(x => x.Description == "音频" && x.CreateUserId == uid).ToList();
            }
            return new
            {
                info = "成功",
                code = 0,
                count = data.Count,
                data = data.Select(t => new
                {
                    t.Job,
                    t.JobId,
                    JobUsers = t.Relation.JobUser,
                    t.Dangerous,
                    t.GroupId,
                    t.IsFinished,
                    t.Measure,
                    t.PlanId,
                    piccount = t.Files.Count(),
                    audiocount = t.FileList1.Count(),
                    t.StartTime,
                    t.EndTime,
                    t.Relation,
                    UserId = t.CreateUserId,
                    t.NeedTrain
                }).ToList()
            };
        }

        [HttpPost]
        public object GetUserDeptJobs(BaseDataModel dy)
        {
            string userId = dy.userId;
            int pagesize = (int)dy.pageSize;
            int pageindex = (int)dy.pageIndex;
            var total = 0;
            UserEntity user = new UserBLL().GetEntity(userId);
            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            FileInfoBLL bll = new FileInfoBLL();
            var data = new List<MeetingJobEntity>();
            if (user.RoleName.Contains("班组长") || (user.RoleName.Contains("班组级用户") && user.RoleName.Contains("负责人")))
            {
                data = meetBll.GetDeptJobs(user.DepartmentId, pagesize, pageindex, out total);

            }
            else
            {
                data = wbll.GetMyJobs(userId, DateTime.Now);
            }
            if (data == null) data = new List<MeetingJobEntity>();
            var bll2 = new HumanDangerTrainingBLL();
            var trainingtype = Config.GetValue("TrainingType");
            foreach (var item in data)
            {
                var file = bll.GetFilesByRecIdNew(item.Relation.MeetingJobId);
                item.Files = file.Where(x => x.Description == "照片").ToList();
                item.FileList1 = file.Where(x => x.Description == "音频").ToList();
                if (trainingtype == "人身风险预控")
                {
                    var trainings = bll2.GetListByUserIdJobId(item.Relation.MeetingJobId, userId);

                    //item.NeedTrain = trainings.Count > 0 ? true : false;
                    if (item.NeedTrain)
                    {
                        //判断风险预控完成情况获取所有人员
                        var alltrainings = bll2.GetListByJobId(item.Relation.MeetingJobId);
                        var finish = alltrainings.Where(x => x.IsDone && x.IsMarked).ToList();
                        if (alltrainings.Count == finish.Count)
                        {
                            item.TrainingDone = true;
                        }
                        else
                        {
                            item.TrainingDone = false;
                        }
                    }
                }
            }

            var data1 = data.Select(t => new
            {
                t.Job,
                t.JobId,
                JobUsers = t.Relation.JobUser,
                t.Dangerous,
                t.GroupId,
                IsFinished = string.IsNullOrEmpty(t.IsFinished) ? "undo" : t.IsFinished,
                t.Measure,
                t.PlanId,
                piccount = t.Files.Count(),
                audiocount = t.FileList1.Count(),
                t.StartTime,
                t.EndTime,
                t.Relation,
                UserId = t.CreateUserId,
                t.NeedTrain,
                t.TrainingDone,
                t.JobType,
                ischecker = t.Relation.JobUsers == null ? false : t.Relation.JobUsers.FirstOrDefault(x => x.JobType == "ischecker") == null ? false : t.Relation.JobUsers.FirstOrDefault(x => x.JobType == "ischecker").UserId == userId ? true : false,
                order = t.Relation.JobUsers == null ? 1 : t.Relation.JobUsers.Any(x => x.UserId == userId) ? 0 : 1,
                Status = t.IsFinished == "finish" ? "已完成" : "进行中"
            }).OrderBy(x => x.order).ToList();
            return new { info = string.Empty, code = 0, count = data1.Count, data = data1 };
        }
        private UserEduncationFileBLL bll = new UserEduncationFileBLL();

        ///<summary>
        ///首页（管理层界面-班组）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetTotalWork(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel>(json);
                var total = bll.GetTotalWork(dy.data, dy.userId
                       );
                return new
                {
                    info = "操作成功",
                    code = result,
                    data = total
                };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
        ///<summary>
        ///人均培训学时
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetedDecimal(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel>(json);
                var total = bll.GetTotalWork(dy.data, dy.userId
                       );
                return new
                {
                    info = "操作成功",
                    code = result,
                    data = total
                };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
        //public object GetJobList([FromBody]JObject json)
        //{
        //    try
        //    {
        //        string res = json.Value<string>("json");
        //        dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
        //        string uid = dy.userId;
        //        List<MeetingJobEntity> m_list = wbll.GetJobList(uid);
        //        for (int i = 0; i < m_list.Count; i++)
        //        {
        //            WorkmeetingEntity wentity = new WorkmeetingBLL().GetEntity(m_list[i].MeetingId);
        //            if (wentity == null) continue;
        //            //班前会为结束不显示班前会任务
        //            if (wentity.MeetingType == "班前会" && wentity.IsOver == false)
        //            {
        //                m_list.RemoveAt(i);
        //                i--;
        //                continue;
        //            }
        //            if (wentity.MeetingType == "班前会"&&!(string.IsNullOrEmpty(wentity.OtherMeetingId))&&wentity.IsOver==true)
        //            {
        //                m_list.RemoveAt(i);
        //                i--;
        //                continue;
        //            }
        //            else if (wentity.MeetingType == "班后会" && wentity.IsOver==true)
        //            {
        //                m_list.RemoveAt(i);
        //                i--;
        //                continue;
        //            }
        //        }
        //        IList rlist = m_list.Select(t => new
        //        {
        //            t.Job,
        //            t.JobId,
        //            t.JobUsers,
        //            t.Dangerous,
        //            t.GroupId,
        //            t.IsFinished,
        //            t.Measure,
        //            t.MeetingId,
        //            t.PlanId,
        //            t.UserId,
        //            StartTime = t.StartTime.ToString("HH:mm:ss"),
        //            EndTime = t.EndTime.ToString("HH:mm:ss")
        //        }).ToList();
        //        return new { info = "成功", code = 0, count = rlist.Count, data = rlist };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new { info = "查询失败：" + ex.Message, code = 1, count = 0, data = new List<Object>() };
        //    }

        //}
        [HttpPost]
        public object GetUpdateJosState([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string jobId = dy.data.JobId;
                string isFinish = dy.data.isFinish;
                string userid = dy.userId;
                var trainingtype = Config.GetValue("TrainingType");

                wbll.UpdateState(jobId, isFinish, trainingtype);
                var messagebll = new MessageBLL();
                messagebll.SendMessage("任务满百", userid);
                //if (wbll.UpdateJosState(jobId, isFinish) > 0)
                //{
                //    var messagebll = new MessageBLL();

                //    return new { info = "修改成功", code = 0 };
                //}
                //else
                //    return new { info = "修改失败", code = 1 };
                return new { info = "修改成功", code = 0 };
            }
            catch (Exception ex)
            {

                return new { info = "修改失败:" + ex.Message, code = 1 };
            }

        }

        [HandlerErrorAttribute]
        public string GetTask(string id)
        {
            throw new Exception("xxxxxxxxxxx");
        }

        [HttpPost]
        public ModelBucket<object> GetSummary(ParamBucket<string> args)
        {
            var safetydays = new DepartmentBLL().GetSafeDays(args.Data);
            var traingingtimes = new DangerBLL().GetTrainingTimes(args.Data);
            var qctimes = new QcActivityBLL().GetQcTimes(args.Data);
            var suggestions = new AdviceBLL().GetSuggestions(args.Data);
            var innovation = new WorkInnovationBLL().GetInnovation(args.Data);
            var meetings = new WorkmeetingBLL().GetMeetingNumber(args.Data);
            var tasks = new WorkmeetingBLL().GetTaskNumber(args.Data);
            var taskpct = new WorkmeetingBLL().GetTaskPct(args.Data);
            var n1 = new EducationBLL().GetNum1(args.Data);
            var n2 = new EducationBLL().GetNum2(args.Data);
            var n3 = new EducationBLL().GetNum3(args.Data);
            var activities = new ActivityBLL().GetLastest(args.Data);
            var evaluations = new EvaluateBLL().GetMainData(args.Data);
            foreach (var item in evaluations)
            {
                if (string.IsNullOrEmpty(item.Reason)) item.Reason = item.EvaluateContent;
            }
            var evaluateionresult = new EvaluateBLL().GetEvaluationResult();
            var totalscore = 0d;
            var avgscore = 0d;
            var seq = 1;
            if (evaluateionresult != null)
            {
                var dept = new DepartmentBLL().GetEntity(args.Data);
                var current = evaluateionresult.FirstOrDefault(x => x.GroupName == dept.FullName);
                if (evaluateionresult.Count > 0)
                {
                    var lastScore = evaluateionresult[0].Pct;
                    for (int i = 0; i < evaluateionresult.Count; i++)
                    {
                        if (evaluateionresult[i].Pct != lastScore)
                        {
                            lastScore = evaluateionresult[i].Pct;
                            seq++;
                        }
                        if (evaluateionresult[i].GroupName == current.GroupName)
                            break;
                    }
                }
                if (current != null)
                    totalscore = current.ActualScore.HasValue ? (double)current.ActualScore.Value : 0d;

                var avgscoretemp = evaluateionresult.Average(x => x.ActualScore);
                avgscore = (double)Math.Round(avgscoretemp.HasValue ? (double)avgscoretemp.Value : 0d);
            }

            return new ModelBucket<object> { code = 0, Data = new { safetydays, traingingtimes, qctimes, suggestions, innovation, meetings, tasks, taskpct, n1, n2, n3, activities, seq, totalscore, avgscore, evaluations } };
        }
        /// <summary>
        /// 国家能源集团版本 统计指标
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetIndexCountries(ParamBucket<IndexCountries> args)
        {

            //统计指标
            var activies = 0;
            var trainings = 0;
            decimal eduncation = 0;
            var nowTime = DateTime.Now;
            var startTime = new DateTime(nowTime.Year, nowTime.Month, 1);
            var endTime = new DateTime(nowTime.Year, nowTime.Month, 1).AddMonths(1).AddMinutes(-1);
            if (args.Data.startTime.HasValue)
            {
                startTime = args.Data.startTime.Value;
                endTime = args.Data.endTime.Value;
            }
            //班前班后会
            if (args.Data.switchValue.Contains("1"))
            {
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                // var list = meetBll.GetList(startTime, endTime, args.Data.DeptId);
                activies = meetBll.GetMeetCountries(args.Data.DeptId, startTime, endTime);
            }
            //培训学时
            if (args.Data.switchValue.Contains("3"))
            {
                UserEduncationFileBLL edBll = new UserEduncationFileBLL();
                eduncation = edBll.GetedDecimal(args.Data.DeptId, args.UserId);
            }
            //人身风险预控
            if (args.Data.switchValue.Contains("2"))
            {
                var HumanDangerbll = new HumanDangerTrainingBLL();
                HumanDangerbll.GetTrainings(args.UserId, new string[] { args.Data.DeptId }, string.Empty, startTime, endTime, "", 50000, 1, null, null, null, out trainings);
            }


            return new { code = 0, info = "获取数据成功", data = new { activies = activies, trainings = trainings, eduncation = eduncation } };

        }



    }
}
