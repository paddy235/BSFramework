using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.HuamDanger;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    public class HumanDangerBLL
    {
        public List<HumanDangerEntity> GetData(string key, int pagesize, int page, string deptId, out int total)
        {
            IHumanDangerService service = new HumanDangerService();
            return service.GetData(key, pagesize, page, deptId, out total);
        }

        public HumanDangerEntity GetDetail(string id)
        {
            IHumanDangerService service = new HumanDangerService();
            return service.GetDetail(id);
        }

        public void Save(HumanDangerEntity model)
        {
            IHumanDangerService service = new HumanDangerService();
            service.Save(model);
        }

        public void Delete(string id)
        {
            IHumanDangerService service = new HumanDangerService();
            service.Delete(id);
        }

        public void Add(List<HumanDangerEntity> data)
        {
            IHumanDangerService service = new HumanDangerService();
            service.Add(data);
        }

        public List<HumanDangerEntity> Find(string key, string deptid, int pageSize, int pageIndex, out int total)
        {
            IHumanDangerService service = new HumanDangerService();
            return service.Find(key, deptid, pageSize, pageIndex, out total);
        }

        public void Evaluate(ActivityEvaluateEntity model)
        {
            IHumanDangerService service = new HumanDangerService();
            service.Evaluate(model);
        }
        /// <summary>
        /// 验证数据是否重复
        /// </summary>
        /// <param name="task">任务名称</param>
        /// <param name="deptId">班组ID，逗号隔开</param>
        /// <returns>bool</returns>
        public bool CheckUnique(string humanDangerId, string task, string deptId)
        {
            IHumanDangerService service = new HumanDangerService();
            return service.CheckUnique(humanDangerId, task, deptId);
        }

        public bool CheckUnique(List<TaskDeptModel> dic)
        {
            IHumanDangerService service = new HumanDangerService();
            return service.CheckUnique(dic);
        }

        public void Association()
        {
            IHumanDangerService service = new HumanDangerService();
            service.Association();
        }

        public List<HumanDangerEntity> GetTemplates(string deptid, string name, int status, int pagesize, int pageindex, out int total)
        {
            IHumanDangerService service = new HumanDangerService();
            return service.GetTemplates(deptid, name, status, pagesize, pageindex, out total);
        }

        public void Approve(HumanDangerEntity model, ApproveRecordEntity approve)
        {
            IHumanDangerService service = new HumanDangerService();
            service.Approve(model, approve);
        }
    }
}
