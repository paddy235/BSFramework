using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class JobService : BaseService
    {
        public JobService(string messagekey, string businessId) : base(messagekey, businessId)
        { }
        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var job = this.BusinessData as MeetingJobEntity;
            return string.Join(",", job.Relation.JobUsers.Select(x => x.UserId));
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var job = this.BusinessData as MeetingJobEntity;
            return job.Job;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var job = (from q1 in db.IQueryable<MeetingJobEntity>()
                       join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                       where q2.MeetingJobId == businessId
                       select q1).FirstOrDefault();

            job.Relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                            where q.MeetingJobId == businessId
                            select q).FirstOrDefault();

            job.Relation.JobUsers = (from q in db.IQueryable<JobUserEntity>()
                                     where q.MeetingJobId == businessId
                                     select q).ToList();

            return job;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var job = this.BusinessData as MeetingJobEntity;
            return new string[] { job.GroupId };
        }
    }
}
