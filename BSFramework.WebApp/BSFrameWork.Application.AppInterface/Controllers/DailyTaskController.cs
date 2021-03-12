using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.TaskManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.TaskManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using BSFrameWork.Application.AppInterface.Models.Meeting;
using BSFrameWork.Application.AppInterface.Models.QueryModels;
using System;
using System.Linq;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 现场终端今日工作
    /// </summary>
    public class DailyTaskController : BaseApiController
    {
        private HumanDangerTrainingBLL trainingBLL = new HumanDangerTrainingBLL();
        private TaskBLL taskBLL = new TaskBLL();

        /// <summary>
        /// 今日工作列表
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<MeetingJobEntity> GetList(ParamBucket<TaskQueryModel> paramBucket)
        {
            var user = OperatorProvider.Provider.Current();
            var now = DateTime.Now;
            var total = 0;
            var data = taskBLL.List(paramBucket.Data.UserId, paramBucket.Data.DistrictId, now, paramBucket.PageSize, paramBucket.PageIndex, out total);

            var trainingtype = Config.GetValue("TrainingType");
            foreach (var item in data)
            {
                item.Status = item.IsFinished == "finish" ? "已完成" : item.IsFinished == "undo" ? "进行中" : "已取消";

                if (trainingtype == "人身风险预控")
                    trainingBLL.EnsureStatus(item, user.UserId);
                if (!string.IsNullOrEmpty(paramBucket.Data.DistrictId))
                    taskBLL.EnsureSignin(item, paramBucket.Data.DistrictId, now.Date);
            }

            return new ListBucket<MeetingJobEntity> { Data = data, Success = true };
        }

        /// <summary>
        /// 选择今日工作
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket PostDistrictTask(ParamBucket<DistrictTaskModel> paramBucket)
        {
            taskBLL.UpdateDistrictTask(paramBucket.Data.DistrictId, paramBucket.UserId, paramBucket.Data.Tasks);
            return new ResultBucket { Success = true };
        }

        /// <summary>
        /// 现场终端签到
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SignIn(ParamBucket<DistrictTaskModel> paramBucket)
        {
            taskBLL.SignIn(paramBucket.UserId, paramBucket.Data.DistrictId, paramBucket.Data.DistrictName, DateTime.Today);
            return new ResultBucket { Success = true };
        }

        /// <summary>
        /// 统计未签到记录
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CalculateUnSignin(string cycle)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"统计未签到记录：{cycle}");

            taskBLL.CalculateUnSignin(cycle, DateTime.Today);

            return Ok();
        }

        /// <summary>
        /// 未签到记录
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<UnSigninEntity> GetUnSignin(ParamBucket<UnSigninQuery> paramBucket)
        {
            var total = 0;
            var data = taskBLL.GetUnSignin(paramBucket.Data.DistrictId, paramBucket.Data.CategoryId, paramBucket.PageSize, paramBucket.PageIndex, out total);
            return new ListBucket<UnSigninEntity> { Success = true, Data = data };
        }

        /// <summary>
        /// 首页今日工作
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<DailyTaskSummaryModel> Summary(ParamBucket paramBucket)
        {
            var userContext = OperatorProvider.Provider.Current();

            var total = taskBLL.FetchTodayTotal(userContext.DeptId, DateTime.Today);
            var finished = taskBLL.FetchTodayTotal(userContext.DeptId, DateTime.Today, true);
            var unfinished = taskBLL.FetchTodayTotal(userContext.DeptId, DateTime.Today, false);

            return new ModelBucket<DailyTaskSummaryModel> { Success = true, Data = new DailyTaskSummaryModel { TodayTotal = total, FinishedTotal = finished, UnFinishedTotal = unfinished } };
        }

        /// <summary>
        /// 任务统计
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="status">状态</param>
        /// <param name="pageSize">记录数</param>
        /// <param name="pageIndex">页</param>
        /// <returns></returns>
        [Route("api/DailyTask")]
        public ListBucket<DailyTaskModel> Get(int year, int month, string status, int pageSize = 10, int pageIndex = 1)
        {
            var currentUser = OperatorProvider.Provider.Current();

            var total = 0;
            var data = taskBLL.List(currentUser.DeptId, year, month, status, pageSize, pageIndex, out total);

            var list = data.Select(x => new DailyTaskModel
            {
                Id = x.JobId,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Job = x.Job,
                IsFinished = TranslateStatu(x.IsFinished),
                Score = x.Score,
                TaskPersons = x.Relation == null || x.Relation.JobUsers == null ? null : x.Relation.JobUsers.Select(y => new DailyTaskPersonModel
                {
                    UserId = y.UserId,
                    Person = y.UserName,
                    Category = y.JobType
                }).ToList()
            }).ToList();

            return new ListBucket<DailyTaskModel> { Data = list, Total = total };
        }

        private string TranslateStatu(string isFinished)
        {
            var status = string.Empty;
            switch (isFinished)
            {
                case "finish":
                    status = "已完成";
                    break;
                case "undo":
                    status = "未完成";
                    break;
                case "cancel":
                    status = "已取消";
                    break;
                default:
                    status = "未完成";
                    break;
            }
            return status;
        }
    }
}