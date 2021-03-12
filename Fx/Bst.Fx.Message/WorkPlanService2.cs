using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class WorkPlanService2 : BaseService
    {
        public WorkPlanService2(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var plan = this.BusinessData as WorkPlanEntity;

            return plan.CreateUserId;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var plan = this.BusinessData as WorkPlanEntity;
            return plan.PlanType;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<WorkPlanEntity>()
                        where q.ID == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var plan = this.BusinessData as WorkPlanEntity;
            return new string[] { plan.UseDeptId };
        }

    }
}
