using BSFramework.Application.Entity.Activity;
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
    public class EvaluateService2 : BaseService
    {
        public EvaluateService2(string messagekey, string businessId)
            : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as ToEvaluateEntity;

            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<UserEntity>()
                        where q.DepartmentId == entity.EvaluateDeptId
                        select q.UserId;

            var data = query.ToList();
            return string.Join(",", data);
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as ToEvaluateEntity;
            return entity.StartDate.ToString("yyyy/MM/dd") + "~" + entity.EndDate.ToString("yyyy/MM/dd");
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<ToEvaluateEntity>()
                        where q.ToEvaluateId == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as ToEvaluateEntity;
            if (entity == null) return null;
            return new string[] { entity.EvaluateDeptId };
        }
    }
}
