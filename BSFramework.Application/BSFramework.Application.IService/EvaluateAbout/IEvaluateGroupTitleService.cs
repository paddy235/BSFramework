using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EvaluateAbout;

namespace BSFramework.Application.IService.EvaluateAbout
{
    public interface IEvaluateGroupTitleService
    {
        void Remove(string groupId, string bK1);
        EvaluateGroupTitleEntity GetEntity(string titleId);
        void Update(EvaluateGroupTitleEntity oldEntity);
        void Insert(EvaluateGroupTitleEntity entity);
        string GetTitleNameByGroupId(string deptId,string evaluateId);
        void Remove(string titleId);
    }
}
