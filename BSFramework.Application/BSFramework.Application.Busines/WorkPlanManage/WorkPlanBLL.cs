using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Application.IService.WorkPlanManage;
using BSFramework.Application.Service.WorkPlanManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkPlanManage
{
    public class WorkPlanBLL
    {
        private IWorkPlanService service = new WorkPlanService();

        public IEnumerable<WorkPlanEntity> GetPlanList() 
        {
            return service.GetPlanList();
        }

        public IEnumerable<WorkPlanContentEntity> GetContentList(string planid)
        {
            return service.GetContentList(planid);
        }

        public void RemoveWorkPlan(string keyValue)
        {
            service.RemoveWorkPlan(keyValue);
        }

        public void RemoveWorkPlanContent(string keyValue)
        {
            service.RemoveWorkPlanContent(keyValue);
        }

        public void SaveWorkPlan(string keyValue, WorkPlanEntity entity)
        {
            service.SaveWorkPlan(keyValue, entity);
        }

        public void SaveWorkPlanContent(string keyValue, WorkPlanContentEntity entity)
        {
            service.SaveWorkPlanContent(keyValue, entity);
        }

        public WorkPlanEntity GetWorkPlanEntity(string keyValue)
        {
            return service.GetWorkPlanEntity(keyValue);
        }

        public WorkPlanContentEntity GetWorkPlanContentEntity(string keyValue)
        {
            return service.GetWorkPlanContentEntity(keyValue);
        }
    }
}
