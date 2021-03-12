using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Data.Repository;

namespace BSFramework.Application.Service.DrugManage
{
    public class DrugStockService : RepositoryFactory<DrugStockEntity>, IDrugStockService
    {
        /// <summary>
        /// 保存入库记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        public void SaveDrugStock(string Id, DrugStockEntity entity)
        {
            DrugStockEntity drugStock = this.GetEntity(Id);
            if (drugStock == null)
            {
                entity.Create();

                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.Modify(Id);
                this.BaseRepository().Update(entity);
            }

        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DrugStockEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 查询入库记录
        /// </summary>
        /// <param name="from">起始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="name">药品名称</param>
        /// <param name="page">当前页数</param>
        /// <param name="pagesize">一页显示条数</param>
        /// <param name="total">查询总数</param>
        /// <returns></returns>
        public IEnumerable<DrugStockEntity> GetStockList(string[] deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (deptid != null && deptid.Length > 0) query = query.Where(x => deptid.Contains(x.BZId));
            if (from != null) query = query.Where(x => x.CreateDate >= from);
            if (to != null) { to = to.Value.AddDays(1); query = query.Where(x => x.CreateDate < to); }
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.DrugName.Contains(name));
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
    }
}
