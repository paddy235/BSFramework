using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.WorkPlan;

namespace BSFramework.Application.IService.WorkPlanManage
{
    public interface IWorkPlanService
    {
        IEnumerable<WorkPlanEntity> GetPlanList();

        IEnumerable<WorkPlanContentEntity> GetContentList(string planid);

        void RemoveWorkPlan(string keyValue);

        void RemoveWorkPlanContent(string keyValue);

        void SaveWorkPlan(string keyValue, WorkPlanEntity entity);

        void SaveWorkPlanContent(string keyValue, WorkPlanContentEntity entity);

        WorkPlanEntity GetWorkPlanEntity(string keyValue);

        WorkPlanContentEntity GetWorkPlanContentEntity(string keyValue);
    }
}
