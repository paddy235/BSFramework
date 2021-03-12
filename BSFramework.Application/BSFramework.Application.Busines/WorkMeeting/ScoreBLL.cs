using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using BSFramework.Application.Entity.BaseManage;
using System.Linq;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data;
using BSFramework.Application.Code;
using System.Text;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using BSFramework.Application.Entity.PeopleManage;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class ScoreBLL
    {
        IScoreService service = new ScoreService();
        public List<JobScoreEntity> GetPersonScore(string userid, int year)
        {
            return service.GetPersonScore(userid, year);
        }
        public List<JobScoreEntity> GetDeptScoreAvg(string deptid, int year)
        {
            return service.GetDeptScoreAvg(deptid, year);
        }
        public List<UserScoreEntity> GetScore1(string deptid, int year, int month)
        {
            return service.GetScore1(deptid, year, month);
        }
        public List<UserScoreEntity> GetScore2(string deptid, int year, int month)
        {
            return service.GetScore2(deptid, year, month);
        }
        public List<UserScoreEntity> GetScore3(string deptid, int year, int month)
        {
            return service.GetScore3(deptid, year, month);
        }

        public decimal GetAvgScore(string deptid, int year, int month)
        {
            return service.GetAvgScore(deptid, year, month);
        }

        public decimal GetAvgTaskCount(string deptid, int year, int month)
        {
            return service.GetAvgTaskCount(deptid, year, month);
        }

        public int GetFinishTaskCount(string deptid, int year, int month)
        {
            return service.GetFinishTaskCount(deptid, year, month);
        }
        public int GetFinishTaskCount(string deptid, DateTime start, DateTime end)
        {
            return service.GetFinishTaskCount(deptid, start, end);
        }

        public decimal GetScore4(string deptid, int year, int month)
        {
            return service.GetScore4(deptid, year, month);
        }
        public decimal GetScore4(string deptid, DateTime start, DateTime end)
        {
            return service.GetScore4(deptid, start, end);
        }
        public int GetTotalScore(string deptid, int year, int month)
        {
            return service.GetTotalScore(deptid, year, month);
        }

        public int GetUnfinishTaskCount(string deptid, int year, int month)
        {
            return service.GetUnfinishTaskCount(deptid, year, month);
        }

        public int GetUnfinishTaskCount(string deptid, DateTime start, DateTime end)
        {
            return service.GetUnfinishTaskCount(deptid, start, end);
        }
        public int PersonFinishCount(string userid, int year, int month)
        {
            return service.PersonFinishCount(userid, year, month);
        }

        public decimal PersonPercent(string userid, int year, int month)
        {
            return service.PersonPercent(userid, year, month);
        }

        public int PersonTotalScore(string userid, int year, int month)
        {
            return service.PersonTotalScore(userid, year, month);
        }

        public List<MeetingJobEntity> PersonJobs(string userid, int year, int month)
        {
            return service.PersonJobs(userid, year, month);
        }
        public List<MeetingJobEntity> PersonJobsObject(string userid, int year, int month)
        {
            return service.PersonJobsObject(userid, year, month);
        }

        public List<PeopleEntity> GetData1(string deptId, int year, int month)
        {
            return service.GetData1(deptId, year, month);
        }

        /// <summary>
        /// 查询班组下所有的工时情况
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetTaskHourStatistics(string deptId, int year, int month)
        {
            return service.GetTaskHourStatistics(deptId, year, month);
        }
    }

}
