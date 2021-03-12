using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;

namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduPlanInfoService
    {
        IEnumerable<EduPlanInfoEntity> GetPlanInfoList(string planid);
        IEnumerable<EduPlanInfoEntity> GetPlanInfoList(string edutype, string verifyhtml, string month, string year, string state, string txt_Keyword, string deptCode, int page, int pagesize, out int total);
        EduPlanInfoEntity GetPlanInfoEntity(string keyValue);

        void RemoveEduPlanInfo(string keyValue);


        void SaveEduPlanInfo(string keyValue, EduPlanInfoEntity entity);
    }
}
