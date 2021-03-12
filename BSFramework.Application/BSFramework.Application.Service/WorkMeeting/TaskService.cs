using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Common;
using BSFramework.Data;
using System.Text;
using System.Configuration;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.Activity;

namespace BSFramework.Service.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class TaskService : ITaskService
    {
        public void Delete(string jobid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                where q.MeetingJobId == jobid
                                select q).FirstOrDefault();

                if (relation != null)
                {
                    var job = (from q in db.IQueryable<MeetingJobEntity>()
                               where q.JobId == relation.JobId
                               select q).FirstOrDefault();

                    if (job != null) db.Delete(job);

                    db.Delete(relation);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<MeetingJobEntity> GetListByDeptId(string deptid, int pagesize, int page, out int total)
        {
            var result = new List<MeetingJobEntity>();
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<WorkmeetingEntity>() on q2.EndMeetingId equals q3.MeetingId into into1
                        from q3 in into1.DefaultIfEmpty()
                        where q1.GroupId == deptid && q1.JobType == "app" && (q3 == null || q3.IsOver == false || (q3.IsOver == true && q2.IsFinished == "finish"))
                        orderby q1.CreateDate descending
                        select new { q1, q2, q3 };
            total = query.Count();
            var data = query.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            foreach (var item in data)
            {
                item.q1.Relation = item.q2;
                item.q1.Relation.EndMeeting = item.q3;
                item.q1.Relation.JobUsers = db.IQueryable<JobUserEntity>().Where(x => x.MeetingJobId == item.q2.MeetingJobId).ToList();
                result.Add(item.q1);
            }
            return result;
        }


    }
}
