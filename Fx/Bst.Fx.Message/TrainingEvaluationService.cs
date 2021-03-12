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
    public class TrainingEvaluationService : BaseService
    {
        public TrainingEvaluationService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var evaluate = this.BusinessData as ActivityEvaluateEntity;
            var db = new RepositoryFactory().BaseRepository();

            var jobusers = (from q1 in db.IQueryable<JobUserEntity>()
                            join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                            where q2.Id == evaluate.Activityid
                            select q1).ToList();

            return string.Join(",", jobusers.Select(x => x.UserId));
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var evaluate = this.BusinessData as ActivityEvaluateEntity;

            var db = new RepositoryFactory().BaseRepository();
            var danger = db.FindEntity<DangerEntity>(evaluate.Activityid);

            return danger.JobName;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<ActivityEvaluateEntity>()
                        where q.ActivityEvaluateId == businessId
                        select q;
            var data = query.FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var evaluate = this.BusinessData as ActivityEvaluateEntity;
            var db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(evaluate.EvaluateId);
            return new string[] { user.DepartmentId };
        }

        public override string[] GetUserId()
        {
            var part1 = this.GetBusinessUserId();
            var part2 = this.GetRoleUserId();
            var list = new List<string>();
            if (!string.IsNullOrEmpty(part1))
                list.AddRange(part1.Split(','));
            if (!string.IsNullOrEmpty(part2))
                list.AddRange(part2.Split(','));
            return list.Distinct().ToArray();
        }

        public override string GetMessage()
        {
            if (this.BusinessData == null) return null;
            var data = this.BusinessData as ActivityEvaluateEntity;
            var id = Guid.Parse(data.Activityid);

            this.Config.Template = this.Config.Template.Replace("{dept}", data.DeptName);
            this.Config.Template = this.Config.Template.Replace("{name}", data.EvaluateUser);
            this.Config.Template = this.Config.Template.Replace("{score}", data.Score + "星");
            this.Config.Template = this.Config.Template.Replace("{evaluate}", data.EvaluateContent);

            if (this.BusinessData == null) return null;
            var evaluate = this.BusinessData as ActivityEvaluateEntity;

            var db = new RepositoryFactory().BaseRepository();
            var danger = db.FindEntity<DangerEntity>(evaluate.Activityid);

            var date = danger.JobTime.Value.ToString("yyyy-M-d");
            this.Config.Template = this.Config.Template.Replace("{date}", date);

            return base.GetMessage();
        }
    }
}
