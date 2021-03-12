using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.SafetyScore
{
    public interface IAccountRuleService
    {
        IEnumerable<AccountRuleEntity> GetPagedList(Pagination pagination, string queryJson);
        bool SaveForm(string keyValue, AccountRuleEntity entity);
        AccountRuleEntity GetEntity(string keyValue);
        void Remove(string keyValue);
        void Insert(List<AccountRuleEntity> importList);
    }
}
