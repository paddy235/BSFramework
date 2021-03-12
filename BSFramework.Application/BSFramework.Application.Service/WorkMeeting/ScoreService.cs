using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WorkMeeting
{
    /// <summary>
    /// 
    /// </summary>
    public class ScoreService : IScoreService
    {
        /// <summary>
        /// 个人评分统计  年度人员任务评分
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<JobScoreEntity> GetPersonScore(string userid, int year)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingStartTime.Year == year && q3.UserId == userid && q1.MeetingType == "班后会"
                        select new { q1.MeetingStartTime, q3.Score };

            var month = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var query2 = from q1 in month
                         join q2 in query on q1 equals q2.MeetingStartTime.Month into into1
                         select new { q1, Score = into1.Sum(x => x.Score ?? 0) };

            var data = query2.ToList();

            return data.Select(x => new JobScoreEntity() { Month = x.q1, Score = x.Score }).ToList();
        }

        /// <summary>
        /// 个人评分统计  年度人员任务平均分
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<JobScoreEntity> GetDeptScoreAvg(string deptid, int year)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var usertotal = db.IQueryable<UserEntity>().Count(x => x.DepartmentId == deptid);

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<UserEntity>() on q3.UserId equals q4.UserId
                        where q1.MeetingStartTime.Year == year && q4.DepartmentId == deptid && q1.MeetingType == "班后会"
                        select new { q1.MeetingStartTime, q3.Score };

            var month = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var query2 = from q1 in month
                         join q2 in query on q1 equals q2.MeetingStartTime.Month into into1
                         select new { q1, Score = into1.Sum(x => x.Score ?? 0) / usertotal };

            return query2.ToList().Select(x => new JobScoreEntity() { Month = x.q1, Score = x.Score }).ToList();
        }



        /// <summary>
        /// 月度数据统计  成员平均得分
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetAvgScore(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);

            IRepository db = new RepositoryFactory().BaseRepository();

            var usercount = db.IQueryable<UserEntity>().Count(x => x.DepartmentId == deptid);

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingType == "班后会" && q2.EndTime >= @from && q2.StartTime <= to && q2.GroupId == deptid
                        select q3;

            if (query.Count() == 0) return 0;

            return Math.Round((decimal)query.Sum(x => (x.Score ?? 0)) / usercount, 1);
        }
        /// <summary>
        /// 月度数据统计  平均完成任务数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetAvgTaskCount(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);

            IRepository db = new RepositoryFactory().BaseRepository();

            var usercount = db.IQueryable<UserEntity>().Count(x => x.DepartmentId == deptid);

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid && q2.IsFinished == "finish"
                        select q3;

            return Math.Round((decimal)query.Count() / usercount, 1);
        }

        /// <summary>
        /// 月度数据统计  完成任务总数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetFinishTaskCount(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid && q2.IsFinished == "finish"
                        select q2;

            return query.Count();
        }
        /// <summary>
        /// 区间获取 完成任务总数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int GetFinishTaskCount(string deptid, DateTime start, DateTime end)
        {
            var from = start;
            var to = end.AddDays(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid && q2.IsFinished == "finish"
                        select q2;

            return query.Count();
        }

        /// <summary>
        /// 月度数据统计 未完成任务数量
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetUnfinishTaskCount(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid && q2.IsFinished == "undo"
                        select q2;

            return query.Count();
        }
        /// <summary>
        /// 区间获取 未完成任务数量
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int GetUnfinishTaskCount(string deptid, DateTime start, DateTime end)
        {
            var from = start;
            var to = end.AddDays(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid && q2.IsFinished == "undo"
                        select q2;

            return query.Count();
        }

        /// <summary>
        /// 月度数据统计  人员任务评分
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetScore1(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<UserEntity>() on q3.UserId equals q4.UserId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q4.DepartmentId == deptid
                        select q3;

            var query2 = from q1 in db.IQueryable<UserEntity>()
                         join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID into into2
                         join q2 in query on q1.UserId equals q2.UserId into into1
                         from d1 in into2.DefaultIfEmpty()
                         where q1.DepartmentId == deptid
                         select new { q1.RealName, Sort = d1.Planer, Score = into1.Count() == 0 ? 0 : into1.Sum(x => (x.Score ?? 0)) };

            if (query2.Count() == 0) return new List<UserScoreEntity>();

            return query2.OrderBy(x => x.Sort != null).ThenBy(x => x.RealName).ToList().Select(x => new UserScoreEntity() { UserName = x.RealName, Score = x.Score }).ToList();
        }
        /// <summary>
        /// 月度数据统计  人员任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetScore2(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<UserEntity>() on q3.UserId equals q4.UserId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q4.DepartmentId == deptid
                        select new { q2.IsFinished, q3.UserId };

            var query2 = from q1 in db.IQueryable<UserEntity>()
                         join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID into into2
                         join q2 in query on q1.UserId equals q2.UserId into into1
                         from d1 in into2.DefaultIfEmpty()
                         where q1.DepartmentId == deptid
                         select new { q1.RealName, Sort = d1.Planer, Score = (decimal)(into1.Count(x => x.IsFinished == "finish") == 0 ? 0 : into1.Count(x => x.IsFinished == "finish") / (into1.Count() == 0 ? 1 : into1.Count())) };

            return query2.OrderBy(x => x.Sort != null).ThenBy(x => x.RealName).ToList().Select(x => new UserScoreEntity() { UserName = x.RealName, Score = Math.Round(x.Score, 4) * 100 }).ToList();
        }
        /// <summary>
        /// 月度数据统计  人员任务完成数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetScore3(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<UserEntity>() on q3.UserId equals q4.UserId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q4.DepartmentId == deptid
                        select new { q2.IsFinished, q3.UserId };

            var query2 = from q1 in db.IQueryable<UserEntity>()
                         join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID into into2
                         join q2 in query on q1.UserId equals q2.UserId into into1
                         from d1 in into2.DefaultIfEmpty()
                         where q1.DepartmentId == deptid
                         select new { q1.RealName, Sort = d1.Planer, Score = into1.Count(x => x.IsFinished == "finish") };

            return query2.OrderBy(x => x.Sort != null).ThenBy(x => x.RealName).ToList().Select(x => new UserScoreEntity() { UserName = x.RealName, Score = x.Score }).ToList();
        }
        /// <summary>
        /// 月度数据统计 班组任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetScore4(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid
                        select q2;
            if (query.Count() > 0)
            {
                return (decimal)query.Count(x => x.IsFinished == "finish") / (query.Count() == 0 ? 1 : query.Count());
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 区间获取 班组任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public decimal GetScore4(string deptid, DateTime start, DateTime end)
        {
            var from = start;
            var to = end.AddDays(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid
                        select q2;
            if (query.Count() > 0)
            {
                return (decimal)query.Count(x => x.IsFinished == "finish") / (query.Count() == 0 ? 1 : query.Count());
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 月度数据统计  任务总分值
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetTotalScore(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q1.GroupId == deptid
                        select q3;
            if (query.Count() == 0) return 0;
            return query.Sum(x => x.Score ?? 0);


        }

        /// <summary>
        /// 个人完成任务总数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int PersonFinishCount(string userid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q3.UserId == userid && q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.IsFinished == "finish"
                        select q2;
            return query.Count();
        }

        /// <summary>
        /// 个人完成任务总分
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int PersonTotalScore(string userid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q3.UserId == userid && q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to
                        select q3;
            if (query.Count() == 0) return 0;
            return query.Sum(x => x.Score ?? 0);
        }

        public List<PeopleEntity> GetPeopleScore(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.IsFinished == "finish"
                        select q2;
            var query1 = from q1 in db.IQueryable<PeopleEntity>()
                             // join 
                         where q1.BZID == deptid
                         select q1;
            return query1.ToList();
        }

        public List<MeetingJobEntity> PersonJobs(string userid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                        where q1.StartTime > @from && q1.EndTime < to && q3.UserId == userid
                        select new { q1.JobId, q1.Job, q2.JobUser, q1.Measure, q1.Remark, q1.StartTime, q2.JobUserId, q1.CreateDate, q1.Dangerous, q1.Description, q1.EndTime, q1.IsFinished, q3.Score };

            var data = query.OrderByDescending(x => x.CreateDate).ToList().Select(x => new MeetingJobEntity() { JobId = x.JobId, Job = x.Job, Measure = x.Measure, Remark = x.Remark, StartTime = x.StartTime, /*UserId = x.JobUserId,*/ CreateDate = x.CreateDate, Dangerous = x.Dangerous, Description = x.Description, EndTime = x.EndTime, IsFinished = x.IsFinished, Score = (x.Score ?? 0).ToString() }).ToList();

            return data;
        }
        //评分任务统计返回
        public List<MeetingJobEntity> PersonJobsObject(string userid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingType == "班后会" && q2.StartTime >= @from && q2.EndTime <= to && q3.UserId == userid
                        select new { q2.JobId, q2.Job, /*q2.JobUsers,*/ q2.Measure, q2.Remark, q4.StartMeetingId, q2.StartTime, /*q2.UserId, */q2.CreateDate, q2.Dangerous, q2.Description, q4.EndMeetingId, q2.EndTime, q4.IsFinished, q3.Score, q4 };
            var data = query.OrderByDescending(x => x.CreateDate).ToList().Select(x => new MeetingJobEntity() { JobId = x.JobId, Job = x.Job, Measure = x.Measure, Remark = x.Remark, StartTime = x.StartTime, /*UserId = x.UserId, JobUsers = x.JobUsers, */CreateDate = x.CreateDate, Dangerous = x.Dangerous, Description = x.Description, EndTime = x.EndTime, IsFinished = x.IsFinished, JobUsers = x.q4.JobUser, Score = x.Score.ToString(), Relation = new MeetingAndJobEntity() { MeetingJobId = x.q4.MeetingJobId } }).ToList();

            //return query.OrderByDescending(x=>x.CreateDate).ToList();
            return data;
        }
        /// <summary>
        /// 个人任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal PersonPercent(string userid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q3.UserId == userid
                        select q2;
            if (query.Count() > 0)
            {
                return (decimal)query.Count(x => x.IsFinished == "finish") / (query.Count() == 0 ? 1 : query.Count()) * 100;
            }
            else
            {
                return 100;
            }

        }

        public List<PeopleEntity> GetData1(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);

            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q2.EndMeetingId
                        join q3 in db.IQueryable<MeetingJobEntity>() on q2.JobId equals q3.JobId
                        join q4 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q4.MeetingJobId
                        join q5 in db.IQueryable<PeopleEntity>() on q4.UserId equals q5.ID
                        where q5.BZID == deptid && q1.MeetingType == "班后会" && q3.EndTime >= @from && q3.StartTime <= to && q3.IsFinished != "cancel"
                        group new { q5.ID, q5.Name, q2.MeetingJobId, q2.IsFinished, q4.Score, q5.Planer, q4.TaskHour } by new { q5.ID, q5.Name, q5.Planer } into g
                        select new { g.Key.ID, g.Key.Name, g.Key.Planer, s1 = g.Count(x => x.IsFinished == "finish"), s2 = g.Sum(x => x.Score ?? 0), s3 = (decimal)(g.Count(x => x.IsFinished == "finish") / (g.Count() == 0 ? 1 : g.Count())), TotalHour = g.Sum(x => x.TaskHour ?? 0) };

            var data = query.OrderBy(x => x.Planer).ThenBy(x => x.Name).ToList();

            return data.Select(x => new PeopleEntity() { ID = x.ID, Name = x.Name, Jobs = x.s1.ToString(), Percent = x.s3.ToString("p"), Scores = x.s2.ToString(), Age = x.TotalHour.ToString() }).ToList();

        }

        /// <summary>
        /// 查询班组下所所有人的工时情况
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetTaskHourStatistics(string deptId, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<UserEntity>() on q3.UserId equals q4.UserId
                        where q1.MeetingType == "班后会" && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q4.DepartmentId == deptId && q2.IsFinished != "cancel"
                        //group q3.TaskHour by new { q3.UserId } into g
                        select new { q3.UserId, q3.TaskHour };

            var query2 = from q1 in db.IQueryable<UserEntity>()
                         join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID into into2
                         join q2 in query on q1.UserId equals q2.UserId into into1
                         from d1 in into2.DefaultIfEmpty()
                         where q1.DepartmentId == deptId
                         select new { q1.RealName, Sort = d1.Planer, TaskHour = into1.Sum(x => x.TaskHour) };

            return query2.OrderBy(x => x.Sort != null).ThenBy(x => x.RealName).ToList().Select(x => new UserScoreEntity() { UserName = x.RealName, Score = x.TaskHour.HasValue ? x.TaskHour.Value : 0 }).ToList();
        }
    }
}
