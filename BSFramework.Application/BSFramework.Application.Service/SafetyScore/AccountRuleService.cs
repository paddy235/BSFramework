using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.IService.SafetyScore;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SafetyScore
{
    public class AccountRuleService : RepositoryFactory<AccountRuleEntity>, IAccountRuleService
    {
        /// <summary>
        /// 根据主键获取单个实体
        /// </summary>
        /// <param name="keyValu
        public AccountRuleEntity GetEntity(string keyValue)
        {
            return BaseRepository().FindEntity(keyValue);
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public IEnumerable<AccountRuleEntity> GetPagedList(Pagination pagination, string queryJson)
        {
            var expression = LinqExtensions.True<AccountRuleEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(x => x.Standard.Contains(keyword));
            }

            var query = BaseRepository().IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = query.Count();
            var list = query.OrderBy(p=>p.Sort).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            return list;
        }


        /// <summary>
        /// 保存数据（新增、修改）
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SaveForm(string keyValue, AccountRuleEntity entity)
        {
            int success = 0;
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                //新增
                entity.Create();
                success = BaseRepository().Insert(entity);
            }
            else
            {
                //修改
                entity.Modify();
                entity.Id = keyValue;
                success = BaseRepository().Update(entity);
            }
            return success > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void Remove(string keyValue)
        {
            BaseRepository().Delete(keyValue);
        }

        public void Insert(List<AccountRuleEntity> importList)
        {
            BaseRepository().Insert(importList);
        }
    }
}