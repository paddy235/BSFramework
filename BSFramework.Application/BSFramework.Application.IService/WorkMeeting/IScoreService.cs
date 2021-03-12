using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WorkMeeting
{
    public interface IScoreService
    {
        /// <summary>
        /// 个人年度分数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        List<JobScoreEntity> GetPersonScore(string userid, int year);

        /// <summary>
        /// 年度人平均分数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        List<JobScoreEntity> GetDeptScoreAvg(string deptid, int year);

        /// <summary>
        /// 月度人员分数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<UserScoreEntity> GetScore1(string deptid, int year, int month);

        /// <summary>
        /// 月度人员任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<UserScoreEntity> GetScore2(string deptid, int year, int month);

        /// <summary>
        /// 月度人员任务完成数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<UserScoreEntity> GetScore3(string deptid, int year, int month);

        /// <summary>
        /// 月度任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        decimal GetScore4(string deptid, int year, int month);

        /// <summary>
        /// 区间获取 班组任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        decimal GetScore4(string deptid, DateTime start, DateTime end);

        /// <summary>
        /// 区间获取 完成任务总数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        int GetFinishTaskCount(string deptid, DateTime start, DateTime end);
        /// <summary>
        /// 获取月度完成任务数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        int GetFinishTaskCount(string deptid, int year, int month);

        /// <summary>
        /// 区间获取 未完成任务数量
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        int GetUnfinishTaskCount(string deptid, DateTime start, DateTime end);
        /// <summary>
        /// 获取月度未完成任务数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        int GetUnfinishTaskCount(string deptid, int year, int month);

        /// <summary>
        /// 获取月度所有人员总分
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        int GetTotalScore(string deptid, int year, int month);

        /// <summary>
        /// 获取月度人员平均分
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        decimal GetAvgScore(string deptid, int year, int month);

        /// <summary>
        /// 获取月度人员平均任务娄
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        decimal GetAvgTaskCount(string deptid, int year, int month);
        List<UserScoreEntity> GetTaskHourStatistics(string deptId, int year, int month);

        /// <summary>
        /// 个人任务完成率
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        decimal PersonPercent(string userid, int year, int month);
        /// <summary>
        /// 个人总分
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        int PersonTotalScore(string userid, int year, int month);

        int PersonFinishCount(string userid, int year, int month);

        List<MeetingJobEntity> PersonJobs(string userid, int year, int month);

        List<PeopleEntity> GetData1(string deptid, int year, int month);
        List<MeetingJobEntity> PersonJobsObject(string userid, int year, int month);

    }
}
