using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
  public  class JobEvaluateBLL 
    {

        /// <summary>
        /// 
        /// </summary>
        public IJobEvaluateService service;
        public JobEvaluateBLL()
        {
            service = new JobEvaluateService();
        }

        /// <summary>
        /// 判断用户是否是工作负责人
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="jobId">工作任务的ID</param>
        /// <returns></returns>
        public bool IsChecker(string userId, string jobId)
        {
            return service.IsChecker(userId, jobId);
        }

        /// <summary>
        /// 判断是否有图片
        /// </summary>
        /// <param name="jobId">工作任务的Id</param>
        /// <returns></returns>
        public bool CheckPhoto(string jobId)
        {
            return service.CheckPhoto(jobId);
        }

        /// <summary>
        /// 新增评分
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(JobEvaluateEntity entity)
        {
             service.Insert(entity);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <param name="evaluateState"></param>
        /// <param name="isMy"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList GetJobPageList(int pageIndex, int pageSize, string userId, string evaluateState, bool isMy,string startTime ,string endTime,string deptId,string keyWord, out int total)
        {
            return service.GetJobPageList(pageIndex, pageSize, userId, evaluateState, isMy, startTime, endTime,deptId, keyWord, out total);
        }

        /// <summary>
        /// 获取任务的详情  
        /// </summary>
        /// <param name="jobId">任务的Id</param>
        /// <returns></returns>
        public object GetJobDetail(string jobId)
        {
            return service.GetJobDetail(jobId);
        }

        /// <summary>
        /// 获取任务的评分信息
        /// </summary>
        /// <param name="jobId">任务的Id</param>
        /// <returns></returns>
        public JobEvaluateEntity GetJobEvaluat(string jobId)
        {
            return service.GetJobEvaluat(jobId);
        }

        public List<JobEvaluateEntity> GetAllByJobId(string jobId)
        {
            return service.GetAllByJobId(jobId);
        }

        public int CheckPhotoCount(string jobId)
        {
            return service.CheckPhotoCount(jobId);
        }
        /// <summary>
        /// 返回任务总分与成员平均分
        /// TotalScore  总分
        /// AvgScore 平均分
        /// </summary>
        /// <param name="deptId">部门的Id</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public Hashtable GetTotalScore(string deptId, int year, int month)
        {
            return service.GetTotalScore(deptId, year, month);
        }

        /// <summary>
        /// 获取班组人员的得分情况
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetPersonScore(string deptId, int year, int month)
        {
            return service.GetPersonScore(deptId, year, month);
        }

        /// <summary>
        /// 获取个人当前年 的任务得分与平均得分
        /// </summary>
        /// <param name="userKey"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<KeyValue> GetPersonScore(string userKey, string year)
        {
            return service.GetPersonScore(userKey, year );
        }

        public List<MeetingJobEntity> PersonJobsObject(string userKey, int year, int month)
        {
            return service.PersonJobsObject(userKey, year, month);
        }

        public List<PeopleEntity> GetData1(string deptId, int year, int month)
        {
            return service.GetData1(deptId, year, month);
        }

        public List<UserScoreEntity> GetScore2(string deptId, int year, int month)
        {
            return service.GetScore2(deptId, year, month);
        }

        public decimal GetAvgTaskCount(string deptId, int year, int month)
        {
            return service.GetAvgTaskCount(deptId, year, month);
        }

        public decimal GetScore4(string deptId, int year, int month)
        {
            return service.GetScore4(deptId, year, month);
        }

        public int GetFinishTaskCount(string deptId, int year, int month)
        {
            return service.GetFinishTaskCount(deptId, year, month);
        }

        public int GetUnfinishTaskCount(string deptId, int year, int month)
        {
            return service.GetUnfinishTaskCount(deptId, year, month);
        }
    }
}
