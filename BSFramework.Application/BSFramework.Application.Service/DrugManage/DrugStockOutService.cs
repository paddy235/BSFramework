using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Service.WorkMeeting;

namespace BSFramework.Application.Service.DrugManage
{
    public class DrugStockOutService : RepositoryFactory<DrugStockOutEntity>, IDrugStockOutService
    {
        /// <summary>
        /// 保存入库记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        public void SaveDrugStockOut(string Id, DrugStockOutEntity entity)
        {
            DrugStockOutEntity drugStock = this.GetEntity(Id);
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
        public bool DelDrugStcokOut(DrugStockOutEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public IEnumerable<DrugStockOutEntity> GetStockOutList(string deptid, string name, string level)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.DrugName.Contains(name));
            if (!string.IsNullOrEmpty(level)) query = query.Where(x => x.DrugLevel == level);
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
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DrugStockOutEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
    }
}
