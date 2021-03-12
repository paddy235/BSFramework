using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class MeetingService2 : BaseService
    {
        public MeetingService2(string messagekey, string businessId) : base(messagekey, businessId)
        { }
        public override string GetBusinessUserId()
        {
            var relation = this.BusinessData as MeetingAndJobEntity;

            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<JobUserEntity>()
                        where q.MeetingJobId == relation.MeetingJobId
                        select q.UserId;

            var data = query.ToList();
            return string.Join(",", data);
        }

        public override string GetContent()
        {
            var relation = this.BusinessData as MeetingAndJobEntity;

            var db = new RepositoryFactory().BaseRepository();
            var job = (from q in db.IQueryable<MeetingJobEntity>()
                       where q.JobId == relation.JobId
                       select q).FirstOrDefault();

            return job.Job;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<MeetingAndJobEntity>()
                        where q.MeetingJobId == businessId
                        select q;
            var data = query.FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            var relation = this.BusinessData as MeetingAndJobEntity;

            var db = new RepositoryFactory().BaseRepository();
            var job = (from q in db.IQueryable<MeetingJobEntity>()
                       where q.JobId == relation.JobId
                       select q).FirstOrDefault();


            return new string[] { job.GroupId };
        }
    }
}
