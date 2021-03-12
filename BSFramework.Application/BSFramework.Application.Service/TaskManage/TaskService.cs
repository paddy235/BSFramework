using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.TaskManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.TaskManage;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.TaskManage
{
    /// <summary>
    /// 任务相关
    /// </summary>
    public class TaskService : ITaskService
    {
        private System.Data.Entity.DbContext _context;

        public TaskService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        /// <summary>
        /// 统计未签到记录
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="date"></param>
        public void CalculateUnSignin(string cycle, DateTime date)
        {
            DateTime from;
            DateTime to;

            if (cycle == "每天")
            {
                from = date.AddDays(-1);
                to = from.AddDays(1).AddSeconds(-1);
            }
            else if (cycle == "每周")
            {
                from = date.AddDays(7);
                to = from.AddDays(7).AddSeconds(-1);
            }
            else if (cycle == "每月")
            {
                from = date.AddMonths(-1);
                to = from.AddMonths(1).AddSeconds(-1);
            }
            else return;

            var districtPersons = _context.Set<DistrictPersonEntity>().Where(x => x.Cycle == cycle).ToList();
            var signins = _context.Set<DistrictSignInEntity>().Where(x => x.SigninDate >= @from && x.SigninDate < to).ToList();
            var unsignin = districtPersons.Where(x => !signins.Any(y => y.DutyDepartmentId == x.DutyDepartmentId)).Select(x => new UnSigninEntity
            {
                UnSigninId = Guid.NewGuid().ToString(),
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                DutyDepartmentId = x.DutyDepartmentId,
                DutyDepartmentName = x.DutyDepartmentName,
                UnSigninDate = date.AddDays(-1),
                UserId = x.DutyUserId,
                UserName = x.DutyUser,
                DistrictId = x.DistrictId,
                DistrictName = x.DistrictName
            }).ToList();
            if (unsignin.Count > 0)
            {
                _context.Set<UnSigninEntity>().AddRange(unsignin);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 签到状态
        /// </summary>
        /// <param name="meetingJob"></param>
        /// <param name="districtId"></param>
        /// <param name="date"></param>
        public void EnsureSignin(MeetingJobEntity meetingJob, string districtId, DateTime date)
        {
            var query = from q in _context.Set<DistrictSignInEntity>()
                        where q.SigninDate == date && q.DistrictId == districtId
                        select q;

            var data = query.ToList();
            foreach (var item in meetingJob.Relation.JobUsers)
            {
                item.IsSignin = data.Any(x => x.UserId == item.UserId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int FetchTodayTotal(string deptId, DateTime date)
        {
            var from = date;
            var to = from.AddDays(1).AddSeconds(-1);

            var query = from q1 in _context.Set<WorkmeetingEntity>()
                        join q2 in _context.Set<MeetingAndJobEntity>() on q1.MeetingId equals q2.StartMeetingId
                        join q3 in _context.Set<MeetingJobEntity>() on q2.JobId equals q3.JobId
                        where q1.GroupId == deptId && q1.MeetingStartTime >= @from && q1.MeetingStartTime <= to && q1.MeetingType == "班前会"
                        select q3;
            return query.Count();
        }

        public int FetchTodayTotal(string deptId, DateTime date, bool isFinished)
        {
            var from = date;
            var to = from.AddDays(1).AddSeconds(-1);

            if (isFinished)
            {
                var query = from q1 in _context.Set<WorkmeetingEntity>()
                            join q2 in _context.Set<MeetingAndJobEntity>() on q1.MeetingId equals q2.StartMeetingId
                            join q3 in _context.Set<MeetingJobEntity>() on q2.JobId equals q3.JobId
                            where q1.GroupId == deptId && q1.MeetingStartTime >= @from && q1.MeetingStartTime <= to && q1.MeetingType == "班前会" && q2.IsFinished == "finish"
                            select q3;
                return query.Count();
            }
            else
            {
                var query = from q1 in _context.Set<WorkmeetingEntity>()
                            join q2 in _context.Set<MeetingAndJobEntity>() on q1.MeetingId equals q2.StartMeetingId
                            join q3 in _context.Set<MeetingJobEntity>() on q2.JobId equals q3.JobId
                            where q1.GroupId == deptId && q1.MeetingStartTime >= @from && q1.MeetingStartTime <= to && q1.MeetingType == "班前会" && q2.IsFinished != "finish"
                            select q3;
                return query.Count();
            }
        }

        /// <summary>
        /// 区域未签到记录
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="categoryId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<UnSigninEntity> GetUnSignin(string districtId, string categoryId, int pageSize, int pageIndex, out int total)
        {
            var query = _context.Set<UnSigninEntity>().AsNoTracking().Where(x => x.DistrictId == districtId);
            if (!string.IsNullOrEmpty(categoryId)) query = query.Where(x => x.CategoryId == categoryId);
            total = query.Count();
            return query.OrderByDescending(x => x.UnSigninDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 今天工作列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="date"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<MeetingJobEntity> List(string userId, string districtId, DateTime date, int pageSize, int pageIndex, out int total)
        {
            var day = date.Date;

            var query = from q1 in _context.Set<MeetingJobEntity>()
                        join q2 in _context.Set<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in _context.Set<WorkmeetingEntity>() on q2.StartMeetingId equals q3.MeetingId
                        where q3.MeetingStartTime >= day && q3.MeetingType == "班前会" && q3.IsOver == true && q1.StartTime <= date
                        select new { q1, q2 };
            if (!string.IsNullOrEmpty(userId))
            {
                query = from q1 in query
                        join q2 in _context.Set<JobUserEntity>() on q1.q2.MeetingJobId equals q2.MeetingJobId
                        where q2.UserId == userId
                        select q1;
            }

            if (!string.IsNullOrEmpty(districtId))
            {
                query = from q1 in query
                        join q2 in _context.Set<DistrictJobEntity>() on q1.q2.MeetingJobId equals q2.MeetingJobId
                        where q2.DistrictId == districtId
                        select q1;

            }

            var dataquery = from q1 in query
                            join q2 in _context.Set<JobUserEntity>() on q1.q2.MeetingJobId equals q2.MeetingJobId into into2
                            select new { q1.q1, q1.q2, q3 = into2 };

            total = dataquery.Count();

            var data = dataquery.OrderBy(x => x.q1.CreateDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            data.ForEach(x =>
            {
                x.q1.Relation = x.q2;
                x.q1.Relation.JobUsers = x.q3.ToList();
            });

            return data.Select(x => x.q1).ToList();
        }

        public List<MeetingJobEntity> List(string deptId, int year, int month, string status, int pageSize, int pageIndex, out int total)
        {
            //var finish = 

            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            var query = from q1 in _context.Set<MeetingJobEntity>()
                        join q2 in _context.Set<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in _context.Set<WorkmeetingEntity>() on q2.StartMeetingId equals q3.MeetingId
                        join q4 in _context.Set<JobUserEntity>() on q2.MeetingJobId equals q4.MeetingJobId into g1
                        where q3.GroupId == deptId && q1.StartTime >= @from && q1.StartTime < to && q3.IsOver == true
                        select new { q1, q2, q3, g1 };
            switch (status)
            {
                case "已完成":
                    query = query.Where(x => x.q1.IsFinished == "finish");
                    break;
                case "未完成":
                    var ary = new string[] { string.Empty, "undo" };
                    query = query.Where(x => ary.Contains(x.q1.IsFinished));
                    break;
                case "已取消":
                    query = query.Where(x => x.q1.IsFinished == "cancel");
                    break;
                default:
                    break;
            }

            var queryable = query.Select(x => new { x.q1, x.g1 });
            total = queryable.Count();
            var data = queryable.OrderBy(x => x.q1.StartTime).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            var list = new List<MeetingJobEntity>();
            foreach (var item in data)
            {
                item.q1.Relation = new MeetingAndJobEntity { JobUsers = item.g1.ToList() };
                list.Add(item.q1);
            }
            return list;
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="date"></param>
        public void SignIn(string userId, string districtId, string districtName, DateTime date)
        {
            var query = from q in _context.Set<DistrictSignInEntity>()
                        where q.DistrictId == districtId && q.UserId == userId && q.SigninDate == date
                        select q;

            var entity = query.FirstOrDefault();
            if (entity == null)
            {
                entity = new DistrictSignInEntity { SigninId = Guid.NewGuid().ToString(), DistrictId = districtId, DistrictName = districtName, UserId = userId, SigninDate = date };
                _context.Set<DistrictSignInEntity>().Add(entity);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 选择今日工作
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="tasks"></param>
        public void UpdateDistrictTask(string districtId, string userId, string[] tasks)
        {
            var query = from q1 in _context.Set<DistrictJobEntity>()
                        join q2 in _context.Set<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                        where q1.DistrictId == districtId && q2.UserId == userId
                        select q1;

            var exists = query.ToList();
            foreach (var item in exists)
            {
                _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
            }
            var entities = tasks.Select(x => new DistrictJobEntity { DistrictJobId = Guid.NewGuid().ToString(), DistrictId = districtId, MeetingJobId = x }).ToList();
            if (entities.Count > 0)
            {
                _context.Set<DistrictJobEntity>().AddRange(entities);
            }
            _context.SaveChanges();
        }
    }
}
