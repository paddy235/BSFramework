using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.EducationManage
{
    public class EduPlanBLL
    {
        private IEduPlanInfoService planinfoservice = new EduPlanInfoService();
        private IEduPlanService planservice = new EduPlanService();
        private IEduPlanVerifyService planverifyservice = new EduPlanVerifyService();

        public int GetTodoCount(string code)
        {
            return planservice.GetTodoList(code);
        }
        public int GetTodoInfoCount(string code)
        {
            var list = planinfoservice.GetPlanInfoList("").Where(x => !string.IsNullOrEmpty(x.GroupCode));
            list = list.Where(x => x.GroupCode.StartsWith(code) && x.VerifyState == "待审核");
            return list.Count();
        }

        public IEnumerable<EduPlanEntity> GetPlanList()
        {
            return planservice.GetPlanList();
        }
        public EduPlanEntity GetEduPlanEntity(string keyValue)
        {
            return planservice.GetEduPlanEntity(keyValue);
        }
        public void RemoveEduPlan(string keyValue)
        {
            planservice.RemoveEduPlan(keyValue);
        }

        public void SaveEduPlan(string keyValue, EduPlanEntity entity)
        {
            planservice.SaveEduPlan(keyValue, entity);
        }

        public IEnumerable<EduPlanInfoEntity> GetPlanInfoList(string planid)
        {
            return planinfoservice.GetPlanInfoList(planid);
        }
        public IEnumerable<EduPlanInfoEntity> GetPlanInfoList(string edutype, string verifyhtml, string month, string year, string state, string txt_Keyword, string deptCode, int page, int pagesize, out int total)
        {
            return planinfoservice.GetPlanInfoList(edutype, verifyhtml, month, year, state, txt_Keyword, deptCode, page, pagesize,out total);
        }
        public EduPlanInfoEntity GetPlanInfoEntity(string keyValue)
        {
            return planinfoservice.GetPlanInfoEntity(keyValue);
        }

        public void RemoveEduPlanInfo(string keyValue)
        {
            planinfoservice.RemoveEduPlanInfo(keyValue);
        }

        public void SaveEduPlanInfo(string keyValue, EduPlanInfoEntity entity)
        {
            planinfoservice.SaveEduPlanInfo(keyValue, entity);
        }

        public IEnumerable<EduPlanVerifyEntity> GetVerifyList(string planid)
        {
            return planverifyservice.GetVerifyList(planid);
        }

        public void RemoveEduPlanVerify(string keyValue)
        {
            planverifyservice.RemoveEduPlanVerify(keyValue);
        }

        public void SaveEduPlanVerify(string keyValue, EduPlanVerifyEntity entity)
        {
            planverifyservice.SaveEduPlanVerify(keyValue, entity);
        }

    }
}
