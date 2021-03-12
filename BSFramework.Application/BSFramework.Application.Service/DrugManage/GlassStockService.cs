using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Data.Repository;
using System.Collections.Generic;
using System;

namespace BSFramework.Application.Service.DrugManage
{
    public class GlassStockService : RepositoryFactory<GlassStockEntity>, IGlassStockService
    {
        public IEnumerable<GlassStockEntity> GetList(string deptid)
        {
            //Operator user = OperatorProvider.Provider.Current();
            //string deptid = user.DeptId;
            var query = this.BaseRepository().IQueryable().Where(x => x.BZId == deptid);
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public IEnumerable<GlassStockEntity> GetPageList(DateTime? from, DateTime? to, string name, string deptid, string glassid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            query = query.Where(x => x.BZId == deptid);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Name.Contains(name));
            if (from != null) query = query.Where(x => x.CreateDate >= from);
            if (to != null) { to = to.Value.AddDays(1); query = query.Where(x => x.CreateDate < to); }
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public GlassStockEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public void SaveGlassStock(string Id, GlassStockEntity entity)
        {
            GlassStockEntity drug = this.GetEntity(Id);
            if (drug == null)
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
    }
}
