using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.SevenSManage;

namespace BSFramework.Application.IService.SevenSManage
{
    public interface ISevensPictureEvaluationService
    {
        void Insert(SevensPictureEvaluationEntity evaluationEntity);
        List<SevensPictureEvaluationEntity> GetList(string dataid);
        List<SevensPictureEvaluationEntity> GetHistory(string userId, string deptid);
        IList GetActivityEvaluateList(string dataid);
        IList GetHistory(string userId);
    }
}
