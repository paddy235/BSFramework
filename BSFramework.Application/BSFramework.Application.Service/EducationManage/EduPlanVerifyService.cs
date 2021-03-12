using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EducationManage;

namespace BSFramework.Application.Service.EducationManage
{
    public class EduPlanVerifyService : RepositoryFactory<EduPlanVerifyEntity>, IEduPlanVerifyService
    {
        public IEnumerable<EduPlanVerifyEntity> GetVerifyList(string planid)
        {
            if (string.IsNullOrEmpty(planid))
            {
                var query = this.BaseRepository().IQueryable();
                return query;
            }
            else
            {
                var query = this.BaseRepository().IQueryable().Where(x => x.PlanId == planid);
                return query.ToList();

            }

        }

        public void RemoveEduPlanVerify(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        public void SaveEduPlanVerify(string keyValue, EduPlanVerifyEntity entity)
        {
            var entity1 = this.BaseRepository().FindEntity(keyValue);
            if (entity1 == null)
            {
                entity.CreateDate = DateTime.Now;

                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity1.Read = entity.Read;
                this.BaseRepository().Update(entity1);
            }
        }
    }
}
