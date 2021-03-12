using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Entity.BaseManage;

namespace BSFramework.Application.Service.EducationManage
{
    public class EduPlanService : RepositoryFactory<EduPlanEntity>, IEduPlanService
    {
        public IEnumerable<EduPlanEntity> GetPlanList()
        {
            var query = this.BaseRepository().IQueryable();
            return query.ToList();
        }
        public int GetTodoList(string deptcode)
        {
            var db = new RepositoryFactory().BaseRepository();
            var depts = db.IQueryable<DepartmentEntity>().Where(x => x.EnCode.StartsWith(deptcode)).ToList();
            var ids = depts.Select(x => x.DepartmentId);
            var query = this.BaseRepository().IQueryable().Where(x => ids.Contains(x.BZID) && x.VerifyState == "待审核");
            return query.Count();
        }
        public void RemoveEduPlan(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        public EduPlanEntity GetEduPlanEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public void SaveEduPlan(string keyValue, EduPlanEntity entity)
        {
            var entity1 = this.BaseRepository().FindEntity(keyValue);
            if (entity1 == null)
            {
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity1.VerifyState = entity.VerifyState;
                entity1.SubmitDate = entity.SubmitDate;
                entity1.SubmitState = entity.SubmitState;

                this.BaseRepository().Update(entity1);
            }
        }
    }
}
