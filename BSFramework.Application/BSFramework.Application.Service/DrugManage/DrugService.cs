using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Data.Repository;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Service.WorkMeeting;
using BSFramework.Data.EF;

namespace BSFramework.Application.Service.DrugManage
{
    public class DrugService : RepositoryFactory<DrugEntity>, IDrugService
    {

        public IEnumerable<DrugEntity> GetList(string deptid)
        {
            var query = this.BaseRepository().IQueryable();
            //Operator user = OperatorProvider.Provider.Current();
            if (deptid != null)
            {
                //string deptid = user.DeptId;
                //query = query.Where(x => x.BZId == deptid);
                var dpservice = new DepartmentService();
                var dept = new DepartmentEntity();

                var bzids = new WorkOrderService().GetWorkOrderGroup(deptid).Select(x => x.departmentid).ToList();

                if (bzids.Count == 0)
                {
                    query = query.Where(x => x.BZId == deptid);
                }
                else
                {
                    query = query.Where(x => bzids.Contains(x.BZId));
                }
            }
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public IEnumerable<DrugEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            //query = query.Where(x => x.BZId == deptid);
            var dpservice = new DepartmentService();
            var dept = new DepartmentEntity();

            var bzids = new WorkOrderService().GetWorkOrderGroup(deptid).Select(x => x.departmentid).ToList();

            if (bzids.Count == 0)
            {
                query = query.Where(x => x.BZId == deptid);
            }
            else
            {
                query = query.Where(x => bzids.Contains(x.BZId));
            }
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.DrugName.Contains(name));
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DrugEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public bool DelDrug(DrugEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public void SaveDrug(string Id, DrugEntity entity)
        {
            DrugEntity drug = this.GetEntity(Id);
            if (drug == null)
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }
                entity.Specs = null;
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.Modify(Id);
                entity.Specs = null;
                this.BaseRepository().Update(entity);
            }

        }

        public void AddDrugStockOut2(DrugOutEntity data, float used)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var drug = (from q in db.IQueryable<DrugEntity>()
                            where q.Id == data.DrugId
                            select q).FirstOrDefault();

                var all = drug.Used + used;

                var ss = (int)(all / float.Parse(drug.Spec));
                drug.DrugNum -= ss;

                drug.Used = all - int.Parse(drug.Spec) * ss;
                data.Surplus = (decimal)(drug.DrugNum * int.Parse(drug.Spec) - drug.Used);

                db.Update(drug);
                db.Insert(data);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void Update(List<DrugEntity> drugs)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            var list = drugs.Select(x => x.Id).ToList();
            var entites = (from q in db.IQueryable<DrugEntity>()
                           where list.Contains(q.Id)
                           select q).ToList();

            foreach (var item in drugs)
            {
                var entity = entites.Find(x => x.Id == item.Id);
                entity.Surplus = item.Surplus;
                var spec = int.Parse(entity.Spec);
                entity.DrugNum = (float)Math.Ceiling(entity.Surplus.Value / spec);
                entity.Used = spec * entity.DrugNum - (float)entity.Surplus.Value;
            }

            try
            {
                db.Update(entites);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void Import(List<DrugEntity> list)
        {
            var context = (DbFactory.Base() as Database).dbcontext;
            var drugset = context.Set<DrugEntity>();
            var drugstockset = context.Set<DrugStockEntity>();

            foreach (var item in list)
            {
                var entity = drugset.FirstOrDefault(x => x.BZId == item.BZId && x.DrugName == item.DrugName && x.DrugLevelName == item.DrugLevelName && x.Spec == item.Spec);
                if (entity == null)
                {
                    entity = item;
                    drugset.Add(entity);
                }
                else
                {
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    entity.Surplus = entity.Surplus + item.Surplus;
                    entity.Total = entity.Total + item.Total;
                    entity.DrugNum = entity.DrugNum + item.DrugNum;
                }

                drugstockset.Add(new DrugStockEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    DrugId = item.Id,
                    DrugName = item.DrugName,
                    DrugNum = item.DrugNum,
                    DrugUnit = item.Unit,
                    DrugUSL = decimal.Parse(item.Spec),
                    CreateUserId = item.CreateUserId,
                    CreateDate = item.CreateDate,
                    CreateUserName = item.CreateUserName,
                    DrugLevel = item.DrugLevel,
                    Type = "0",
                    BZId = item.BZId,
                    StockNum = (float)entity.Surplus.Value,
                    Monitor = item.CreateUserName
                });
            }

            context.SaveChanges();
        }
    }
}
