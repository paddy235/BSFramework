using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WorkMeeting
{
    public interface IDangerAnalysisService
    {
        DangerAnalysisEntity GetLast(string deptid);
        void Init(DangerAnalysisEntity entity);
        List<RiskFactorSetEntity> FindDanger(string key, int limit, string deptid);
        JobDangerousEntity GetDanger(string id);
        void EditDanger(JobDangerousEntity danger);
        void DeleteDanger(string id);
        DangerAnalysisEntity Prev(string deptId, string id);
        DangerAnalysisEntity GetByMeeting(string meetingid);
        void Edit(DangerAnalysisEntity analysis);
        void Copy(JobDangerousEntity model);
    }
}
