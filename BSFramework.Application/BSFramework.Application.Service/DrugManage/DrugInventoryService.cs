using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Data;
using BSFramework.Util.WebControl;
using BSFramework.Data;
using BSFramework.Util;
using BSFramework.Util.Extension;

namespace BSFramework.Application.Service.DrugManage
{
    public class DrugInventoryService : RepositoryFactory<DrugInventoryEntity>, IDrugInventoryService
    {
        public IEnumerable<DrugInventoryEntity> GetList()
        {
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }
        public IEnumerable<DrugGlassWareEntity> GetDrugGlassWareList()
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<DrugGlassWareEntity>();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }


        public IEnumerable<DrugInventoryEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.DrugName.Contains(name));
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        public List<DrugInventoryEntity> GetDrugPageList(string usercode, Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable();

            //DatabaseType dataType = DbHelper.DbType;
            //Operator user = OperatorProvider.Provider.Current();

            if (usercode == null) usercode = "0";
            var queryParam = queryJson.ToJObject();
            //pagination.conditionJson += string.Format(" and (locate(deptcode,'{0}')) = 1 ", user.DeptCode);
            query = query.Where(x => x.DeptCode.StartsWith(usercode));
            if (!queryParam["keyword"].IsEmpty())
            {
                var keyword = queryParam["keyword"].ToString();
                //pagination.conditionJson += string.Format(" and drugname  like '%{0}%' or casno like '%{0}%' or englishname like '%{0}%' ", keyword);
                query = query.Where(x => x.DrugName.Contains(keyword) || x.CASNO.Contains(keyword) || x.EnglishName.Contains(keyword));
            }

            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

            //return dt;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DrugInventoryEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public DrugGlassWareEntity GetDrugGlassWareEntity(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<DrugGlassWareEntity>(keyValue);
        }
        public bool DelDrugInventory(DrugInventoryEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public bool DeleteGlassWare(DrugGlassWareEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            if (db.Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }

        public void SaveDrugInventory(string Id, DrugInventoryEntity entity)
        {
            DrugInventoryEntity drug = this.GetEntity(Id);
            if (drug == null)
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.Modify(Id);
                entity.Files = null;
                this.BaseRepository().Update(entity);
            }

        }
        public void SaveDrugGlassWare(string Id, DrugGlassWareEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                DrugGlassWareEntity drug = this.GetDrugGlassWareEntity(Id);
                if (drug == null)
                {
                    if (string.IsNullOrEmpty(entity.GlassWareId))
                    {
                        entity.GlassWareId = Guid.NewGuid().ToString();
                    }
                    entity.CreateDate = DateTime.Now;
                    db.Insert<DrugGlassWareEntity>(entity);
                    db.Commit();
                }
                else
                {
                    entity.Modify(Id);
                    //entity.Files = null;
                    db.Update<DrugGlassWareEntity>(entity);
                    db.Commit();
                }
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
    }
}
