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

namespace BSFramework.Application.Service.DrugManage
{
    public class DrugOutService : RepositoryFactory<DrugOutEntity>, IDrugOutService
    {
        /// <summary>
        /// 保存取用记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        public void SaveDrugOut(string Id, DrugOutEntity entity)
        {
            DrugOutEntity DrugOut = this.GetEntity(Id);
            if (DrugOut == null)
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
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public DrugOutEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 获取取用记录条数
        /// </summary>
        /// <param name="from">起始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="DrugName">药品名称</param>
        /// <returns></returns>
        public int GetOutListCount(string deptid, DateTime? from, DateTime? to, string DrugName)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable();

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
            if (from != null) query = query.Where(x => x.CreateDate >= from);
            if (to != null) { to = to.Value.AddDays(1); query = query.Where(x => x.CreateDate < to); }
            if (!string.IsNullOrEmpty(DrugName)) query = query.Where(x => x.DrugName.Contains(DrugName));
            return query.Count();
        }
        /// <summary>
        /// 获取取用记录
        /// </summary>
        /// <param name="from">起始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="DrugName">药品名称or创建人名称</param>
        /// <param name="page">当前页数</param>
        /// <param name="pagesize">一页显示条数</param>
        /// <param name="total">查询总数</param>
        /// <returns></returns>
        public IEnumerable<DrugOutEntity> GetOutList(string deptid, DateTime? from, DateTime? to, string DrugName, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();

            //var user = OperatorProvider.Provider.Current();
            
            if (deptid != null)
            {
                //var deptid = user.DeptId;
                //query = query.Where(x => x.BZId == user.DeptId);
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

            if (from != null) query = query.Where(x => x.CreateDate >= from);
            if (to != null) { to = to.Value.AddDays(1); query = query.Where(x => x.CreateDate < to); }
            if (!string.IsNullOrEmpty(DrugName)) query = query.Where(x => x.DrugName.Contains(DrugName) || x.CreateUserName.Contains(DrugName));
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
    }
}
