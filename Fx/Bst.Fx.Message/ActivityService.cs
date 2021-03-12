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
    public class ActivityService : BaseService
    {
        public ActivityService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var evaluate = this.BusinessData as ActivityEvaluateEntity;
            var db = new RepositoryFactory().BaseRepository();
            var activity = (from q in db.IQueryable<ActivityEntity>() where q.ActivityId == evaluate.Activityid select q).FirstOrDefault();
            var query = from q in db.IQueryable<UserEntity>()
                        where q.DepartmentId == activity.GroupId && q.RealName == activity.ChairPerson
                        select q;
            var data = query.ToList();
            return string.Join(",", data.Select(x => x.UserId));
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var evaluate = this.BusinessData as ActivityEvaluateEntity;
            var db = new RepositoryFactory().BaseRepository();
            var activity = (from q in db.IQueryable<ActivityEntity>()
                            where q.ActivityId == evaluate.Activityid
                            select q).FirstOrDefault();
            string type = activity != null ? activity.ActivityType : "";
            return string.Format("{4}{5}台账有一条新的评价记录：{0} {1} {2}分 {3}", evaluate.DeptName, evaluate.EvaluateUser, evaluate.Score, evaluate.EvaluateContent, activity.PlanStartTime.ToString("yyyy-MM-dd"), type);
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<ActivityEvaluateEntity>()
                        where q.ActivityEvaluateId == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as ActivityEvaluateEntity;
            if (entity == null) return null;
            var db = new RepositoryFactory().BaseRepository();
            var acuentity = (from q in db.IQueryable<ActivityEntity>()
                             where q.ActivityId == entity.Activityid
                             select q).FirstOrDefault();
            return new string[] { acuentity.GroupId };
        }
     
    }
}
