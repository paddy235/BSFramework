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
    public class GlassService : RepositoryFactory<GlassEntity>, IGlassService
    {
        public IEnumerable<GlassEntity> GetList()
        {
            //Operator user = OperatorProvider.Provider.Current();
            //string deptid = user.DeptId;
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public IEnumerable<GlassEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            query = query.Where(x => x.BZId == deptid);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Name.Contains(name));
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public GlassEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public bool DelGlass(GlassEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public void SaveGlass(string Id, GlassEntity entity)
        {
            GlassEntity drug = this.GetEntity(Id);
            if (drug == null)
            {
                entity.Create();
                this.BaseRepository().Insert(entity);
            }
            else
            {
                drug.Name = entity.Name;
                drug.Location = entity.Location;
                drug.Spec = entity.Spec;
                drug.GlassWareId = entity.GlassWareId;
                drug.Path = entity.Path;
                drug.Remark = entity.Remark;
                drug.State = entity.State;
                drug.Amount = entity.Amount;
                drug.BGPath = entity.BGPath;

                drug.Warn = entity.Warn;
                entity.Modify(Id);
                this.BaseRepository().Update(drug);
            }

        }
    }
}
