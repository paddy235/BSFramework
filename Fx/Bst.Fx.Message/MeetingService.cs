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
    public class MeetingService : BaseService
    {
        public MeetingService(string messagekey, string businessId) : base(messagekey, businessId)
        { }
        public override string GetBusinessUserId()
        {
            var evaluate = this.BusinessData as ActivityEvaluateEntity;

            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.DepartmentId equals q2.GroupId
                        where q2.MeetingId == evaluate.Activityid
                        select q1.UserId;

            var data = query.ToList();
            return string.Join(",", data);
        }

        public override string GetContent()
        {
            var evaluate = this.BusinessData as ActivityEvaluateEntity;

            var db = new RepositoryFactory().BaseRepository();
            var meeting = (from q1 in db.IQueryable<WorkmeetingEntity>()
                           join q2 in db.IQueryable<WorkmeetingEntity>() on q1.MeetingId equals q2.OtherMeetingId
                           where q2.MeetingId == evaluate.Activityid
                           select q1).FirstOrDefault();

            return meeting.MeetingStartTime.ToString("yyyy-MM-dd");
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
            var evaluate = this.BusinessData as ActivityEvaluateEntity;

            var db = new RepositoryFactory().BaseRepository();
            var meeting = (from q1 in db.IQueryable<WorkmeetingEntity>()
                           join q2 in db.IQueryable<WorkmeetingEntity>() on q1.MeetingId equals q2.OtherMeetingId
                           where q2.MeetingId == evaluate.Activityid
                           select q1).FirstOrDefault();

            return new string[] { meeting.GroupId };
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

            return base.GetMessage();
        }
    }
}
