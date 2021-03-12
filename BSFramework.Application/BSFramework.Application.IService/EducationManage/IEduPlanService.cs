using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;

namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduPlanService
    {
        IEnumerable<EduPlanEntity> GetPlanList();

        void RemoveEduPlan(string keyValue);

        void SaveEduPlan(string keyValue, EduPlanEntity entity);

        EduPlanEntity GetEduPlanEntity(string keyValue);

        int GetTodoList(string deptcode);
        
    }
}
