using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class EduPlanService : BaseService
    {
        public EduPlanService(string messagekey, string businessId)
            : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var entity = this.BusinessData as EduPlanEntity;
            return entity.CreateUserId;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as EduPlanEntity;
            var res = entity.VerifyState == "审核通过" ? "恭喜" : "抱歉";
            return string.Format("{2}您提交的{0}培训计划{1}", entity.Name.Substring(0,4), entity.VerifyState, res);
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<EduPlanEntity>()
                        where q.ID == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as EduPlanEntity;
            if (entity == null) return null;
            return new string[] { entity.BZID };
        }
    }
}
