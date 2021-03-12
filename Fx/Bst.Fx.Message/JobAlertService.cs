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
    public class JobAlertService : BaseService
    {
        public JobAlertService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var user = this.BusinessData as UserEntity;
            return user.UserId;
        }

        public override string GetContent()
        {
            var user = this.BusinessData as UserEntity;

            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                        where q2.UserId == user.UserId && q1.IsFinished == "finish"
                        select q1;

            var data = query.Count();
            return data.ToString();
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var user = (from q in db.IQueryable<UserEntity>()
                        where q.UserId == businessId
                        select q).FirstOrDefault();

            return user;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var user = this.BusinessData as UserEntity;
            return new string[] { user.DepartmentId };
        }

        public override string[] GetUserId()
        {
            var list = new List<string>();
            var user = this.BusinessData as UserEntity;

            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                        where q2.UserId == user.UserId && q1.IsFinished == "finish"
                        select q1;

            var data = query.Count();
            if (data % 2 == 0) list.Add(user.UserId);

            return list.ToArray();
        }
    }
}
