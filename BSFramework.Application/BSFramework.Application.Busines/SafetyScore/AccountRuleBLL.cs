using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.IService.SafetyScore;
using BSFramework.Application.Service.SafetyScore;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SafetyScore
{
   public class AccountRuleBLL
    {
        private readonly IAccountRuleService _ruleService;
        public AccountRuleBLL()
        {
            _ruleService = new AccountRuleService();
        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public IEnumerable<AccountRuleEntity> GetPagedList(Pagination pagination, string queryJson)
        {
            return _ruleService.GetPagedList(pagination, queryJson);
        }

        /// <summary>
        /// 根据主键获取单个实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public AccountRuleEntity GetEntity(string keyValue)
        {
            return _ruleService.GetEntity(keyValue);
        }

        /// <summary>
        /// 保存数据（新增、修改）
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveForm(string keyValue, AccountRuleEntity entity)
        {
            return _ruleService.SaveForm(keyValue, entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void Remove(string keyValue)
        {
            _ruleService.Remove(keyValue);
        }

        public void Insert( List<AccountRuleEntity> importList)
        {
            _ruleService.Insert(importList);
        }
    }
}
