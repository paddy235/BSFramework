using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Entity.WorkMeeting;

namespace BSFramework.Application.IService.WorkMeeting
{
    public interface IJobEvaluateService
    {
        bool IsChecker(string userId, string jobId);
        bool CheckPhoto(string jobId);
        void Insert(JobEvaluateEntity entity);
        IList GetJobPageList(int pageIndex, int pageSize, string userId, string evaluateState, bool isMy, string startTime, string endTime,string deptid,string keyWord, out int total);
        object GetJobDetail(string jobId);
        JobEvaluateEntity GetJobEvaluat(string jobId);
        List<JobEvaluateEntity> GetAllByJobId(string jobId);
        int CheckPhotoCount(string jobId);
        Hashtable GetTotalScore(string deptId, int year, int month);
        List<UserScoreEntity> GetPersonScore(string deptId, int year, int month);
        List<KeyValue> GetPersonScore(string userKey, string year);
        List<MeetingJobEntity> PersonJobsObject(string userKey, int year, int month);
        List<PeopleEntity> GetData1(string deptId, int year, int month);
        List<UserScoreEntity> GetScore2(string deptId, int year, int month);
        decimal GetAvgTaskCount(string deptId, int year, int month);
        decimal GetScore4(string deptId, int year, int month);
        int GetFinishTaskCount(string deptId, int year, int month);
        int GetUnfinishTaskCount(string deptId, int year, int month);
    }
}
