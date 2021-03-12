using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.IService.ToolManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using System.Text;
using System.Data;
using BSFramework.Data;
using System;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Service.WorkMeeting;

namespace BSFramework.Application.Service.ToolManage
{
    public class ToolTypeService : RepositoryFactory<ToolTypeEntity>, IToolTypeService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<ToolTypeEntity> GetList(string deptid)
        {
            var query = this.BaseRepository().IQueryable();
            //Operator user = OperatorProvider.Provider.Current();
            if (deptid != null)
            {
                //string deptid = user.DeptId;
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
            return query.OrderBy(x => x.CreateDate).ToList();
        }
        ///// <summary>
        ///// 获取列表
        ///// </summary>
        ///// <param name="deptId">部门Id</param>
        ///// <returns>返回列表</returns>
        //public IEnumerable<ToolTypeEntity> GetList(string deptId)
        //{
        //    var query = this.BaseRepository().IQueryable();
        //    //Operator user = OperatorProvider.Provider.Current();
        //    var bzids = new WorkOrderService().GetWorkOrderGroup(deptId).Select(x => x.departmentid).ToList();
        //    if (bzids.Count == 0)
        //    {
        //        query = query.Where(x => x.BZId == deptId);
        //    }
        //    else
        //    {
        //        query = query.Where(x => bzids.Contains(x.BZId));
        //    }
        //    return query.OrderBy(x => x.CreateDate).ToList();
        //}

        public IEnumerable<ToolTypeEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<ToolTypeEntity>();
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
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Name.Contains(name));
            var finalQuery = from q1 in query
                             join q2 in db.IQueryable<ToolInventoryEntity>() on q1.InventoryId equals q2.ID into t
                             from t1 in t.DefaultIfEmpty()
                             select new { q1, t1.Type };

                             



            total = finalQuery.Count();
            if (page < 0)
            {
                return finalQuery.OrderByDescending(x => x.q1.CreateDate).ToList().Select(x => new ToolTypeEntity()
                {
                    Type = x.Type,
                    BZId = x.q1.BZId,
                    CreateDate = x.q1.CreateDate,
                    DeptId = x.q1.DeptId,
                    ID = x.q1.ID,
                    InventoryId = x.q1.InventoryId,
                    Name = x.q1.Name,
                    Numbers = x.q1.Numbers,
                    Path = x.q1.Path,
                    Remark = x.q1.Remark,
                    Remind = x.q1.Remind,
                    State = x.q1.State
                });
            }
            return finalQuery.OrderByDescending(x => x.q1.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList().Select(x => new ToolTypeEntity()
            {
                Type = x.Type,
                BZId = x.q1.BZId,
                CreateDate = x.q1.CreateDate,
                DeptId = x.q1.DeptId,
                ID = x.q1.ID,
                InventoryId = x.q1.InventoryId,
                Name = x.q1.Name,
                Numbers = x.q1.Numbers,
                Path = x.q1.Path,
                Remark = x.q1.Remark,
                Remind = x.q1.Remind,
                State = x.q1.State
            });
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ToolTypeEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }


        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ToolTypeEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (entity1 == null)
            {
                // entity.ID = Guid.NewGuid().ToString();
                // entity.BZId = OperatorProvider.Provider.Current().DeptId;
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.Name = entity.Name;
                entity1.Path = entity.Path;
                this.BaseRepository().Update(entity1);
            }
        }
        #endregion
    }
}
