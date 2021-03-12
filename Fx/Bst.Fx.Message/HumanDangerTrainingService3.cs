using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class HumanDangerTrainingService3 : BaseService
    {
        public HumanDangerTrainingService3(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var evaluate = this.BusinessData as ActivityEvaluateEntity;
            var id = Guid.Parse(evaluate.Activityid);
            using (var ctx = new DataContext())
            {
                var training = ctx.HumanDangerTrainingUsers.Find(id);
                return training.UserId;
            }
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var evaluate = this.BusinessData as ActivityEvaluateEntity;
            using (var ctx = new DataContext())
            {
                var training = ctx.HumanDangerTrainingUsers.Include("Training").FirstOrDefault(x => x.TrainingUserId == evaluate.Activityid);
                return training == null ? string.Empty : training.Training.TrainingTask;
            }
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
            var report = this.BusinessData as ActivityEvaluateEntity;

            if (report == null) return null;
            return new string[0];
        }

        public override string GetMessage()
        {
            if (this.BusinessData == null) return null;
            var data = this.BusinessData as ActivityEvaluateEntity;

            this.Config.Template = this.Config.Template.Replace("{dept}", data.DeptName);
            this.Config.Template = this.Config.Template.Replace("{name}", data.EvaluateUser);
            this.Config.Template = this.Config.Template.Replace("{score}", data.Score + "星");
            this.Config.Template = this.Config.Template.Replace("{evaluate}", data.EvaluateContent);

            using (var ctx = new DataContext())
            {
                var training = ctx.HumanDangerTrainingUsers.Include("Training").FirstOrDefault(x => x.TrainingUserId == data.Activityid);
                var date = training.TrainingTime.Value.ToString("yyyy-M-d");
                this.Config.Template = this.Config.Template.Replace("{date}", date);
            }

            return base.GetMessage();
        }
    }
}
