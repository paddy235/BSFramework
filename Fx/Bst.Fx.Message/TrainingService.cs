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
    public class TrainingService : BaseService
    {
        public TrainingService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var danger = this.BusinessData as DangerEntity;

            return string.Join(",", danger.JobUsers.Select(x => x.UserId));
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var training = this.BusinessData as DangerEntity;
            return training.JobName;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<DangerEntity>()
                        where q.Id == businessId
                        select q).FirstOrDefault();
            data.JobUsers = (from q in db.IQueryable<JobUserEntity>()
                             where q.MeetingJobId == data.JobId
                             select q).ToList();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var danger = this.BusinessData as DangerEntity;
            return new string[] { danger.GroupId };
        }
    }
}
