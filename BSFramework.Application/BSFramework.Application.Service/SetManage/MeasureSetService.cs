using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.IService.SetManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BSFramework.Application.Service.SetManage
{
    public class MeasureSetService : RepositoryFactory<MeasureSetEntity>, IMeasureSetService
    {
        /// <summary>
        /// 得到危险因素下的防范措施列表
        /// </summary>
        /// <param name="riskFactorId">危险因素ID</param>
        /// <returns></returns>
        public IEnumerable<MeasureSetEntity> GetList(string riskFactorId)
        {
            if (string.IsNullOrEmpty(riskFactorId))
            {
                throw new ArgumentNullException("危险因素ID必传");
            }

            return this.BaseRepository().IQueryable().Where(t => t.RiskFactorId == riskFactorId).OrderBy(t => t.Sort).ToList();
        }

        /// <summary>
        /// 删除防范措施
        /// </summary>
        /// <param name="keyValue">防范措施ID</param>
        public void Delete(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        /// <summary>
        /// 新增一条防范措施
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(MeasureSetEntity entity)
        {
            int sort = this.BaseRepository().IQueryable().Where(t => t.RiskFactorId == entity.RiskFactorId).OrderByDescending(x => x.Sort).Take(1).ToList().DefaultIfEmpty(new MeasureSetEntity() { Sort = 0 }).Max(x => x.Sort).Value;
            entity.ID = Guid.NewGuid().ToString();
            entity.CreateDate = DateTime.Now;
            entity.Sort = sort + 1;

            this.BaseRepository().Insert(entity);
        }
    }
}
