using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;

namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduPlanVerifyService
    {
        IEnumerable<EduPlanVerifyEntity> GetVerifyList(string planid);

        void RemoveEduPlanVerify(string keyValue);

        void SaveEduPlanVerify(string keyValue, EduPlanVerifyEntity entity);
    }
}
