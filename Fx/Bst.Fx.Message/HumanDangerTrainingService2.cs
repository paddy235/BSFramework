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
    public class HumanDangerTrainingService2 : BaseService
    {
        public HumanDangerTrainingService2(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var report = this.BusinessData as HumanDangerTrainingUser;
            return report.TrainingUserId.ToString();
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var data = this.BusinessData as HumanDangerTrainingUser;
            using (var ctx = new DataContext())
            {
                var training = ctx.HumanDangerTrainings.Find(data.TrainingId);
                return training.TrainingTask;
            }
        }

        public override object GetData(string businessId)
        {
            var id = Guid.Parse(businessId);
            using (var ctx = new DataContext())
            {
                var data = ctx.HumanDangerTrainingUsers.Find(id);
                return data;
            }
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var report = this.BusinessData as HumanDangerTrainingUser;

            if (report == null) return null;

            var db = new RepositoryFactory().BaseRepository();
            var user = (from q in db.IQueryable<UserEntity>()
                        where q.UserId == report.UserId
                        select q).FirstOrDefault();

            if (user == null) return null;
            return new string[] { user.DepartmentId };
        }
    }
}
