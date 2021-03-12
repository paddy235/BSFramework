using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.HuamDanger;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 描 述：人身风险预控库
    /// </summary>
    public interface IHumanDangerService
    {
        List<HumanDangerEntity> GetData(string key, int pagesize, int page, string deptId, out int total);
        void Save(HumanDangerEntity model);
        HumanDangerEntity GetDetail(string id);
        void Delete(string id);
        void Add(List<HumanDangerEntity> data);
        List<HumanDangerEntity> Find(string key,string deptid, int pageSize, int pageIndex, out int total);
        void Evaluate(ActivityEvaluateEntity model);
        bool CheckUnique(string humanDangerId, string task, string deptId);
        bool CheckUnique(List<TaskDeptModel> dic);
        void Association();
        List<HumanDangerEntity> GetTemplates(string deptid, string name, int status, int pagesize, int pageindex, out int total);
        void Approve(HumanDangerEntity model, ApproveRecordEntity approve);
    }
}
