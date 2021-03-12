using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class DangerAnalysisBLL
    {
        private IDangerAnalysisService _service;

        public DangerAnalysisBLL()
        {
            _service = new DangerAnalysisService();
        }

        public DangerAnalysisEntity GetLast(string deptid)
        {
            return _service.GetLast(deptid);
        }

        public void Init(DangerAnalysisEntity model)
        {
            _service.Init(model);
        }

        public List<RiskFactorSetEntity> FindDanger(string key, int limit, string deptid)
        {
            return _service.FindDanger(key, limit, deptid);
        }

        public JobDangerousEntity GetDanger(string id)
        {
            return _service.GetDanger(id);
        }

        public void EditDanger(JobDangerousEntity danger)
        {
            _service.EditDanger(danger);
        }

        public void DeleteDanger(string id)
        {
            _service.DeleteDanger(id);
        }

        public DangerAnalysisEntity Prev(string deptId, string id)
        {
            return _service.Prev(deptId, id);
        }

        public DangerAnalysisEntity GetByMeeting(string meetingid)
        {
            return _service.GetByMeeting(meetingid);
        }

        public void Copy(JobDangerousEntity model)
        {
            _service.Copy(model);
        }
    }
}
