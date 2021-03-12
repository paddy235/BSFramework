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
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Service.PeopleManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Service.WorkMeeting;

namespace BSFramework.Application.Service.ToolManage
{
    public class ToolBorrowService : RepositoryFactory<ToolBorrowEntity>, IToolBorrowService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<ToolBorrowEntity> GetList(string userid, string deptid, DateTime? from, DateTime? to, string name)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            //string userid = OperatorProvider.Provider.Current().UserId;
            PeopleEntity p = new PeopleService().GetEntity(userid);
            var query = this.BaseRepository().IQueryable();
            //运行班组共享数据
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

            if (p != null && p.Quarters != "班长" && p.Quarters != "班组长")
            {
                query = query.Where(x => x.BorrwoPersonId == userid);
            }
            if (from != null) query = query.Where(x => x.BorrwoDate >= from);
            if (to != null) { to = to.Value.AddDays(1); query = query.Where(x => x.BorrwoDate < to); }
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.ToolName.Contains(name));
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public List<ToolBorrowEntity> GetToolBorrowPageList(Pagination pagination, string queryJson)
        {
            DatabaseType dataType = DbHelper.DbType;
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<ToolBorrowEntity>();
            var queryParam = queryJson.ToJObject();
            //部门主键

            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyord = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "Name":
                        //账户
                        //  pagination.conditionJson += string.Format(" and TOOLNAME  like '%{0}%'", keyord);
                        expression = expression.And(x => x.ToolName.Contains(keyord));
                        break;

                    default:
                        break;
                }
            }
            var data = db.IQueryable(expression);
            pagination.records = data.Count();
            var result = data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();

            // DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

            return result;
        }
        public IEnumerable<ToolBorrowEntity> GetListByUser(string userid, string tid)
        {
            return this.BaseRepository().IQueryable().Where(x => x.BorrwoPersonId == userid && x.TypeId == tid && x.BackDate == null).ToList();
        }

        public IEnumerable<ToolBorrowEntity> GetListByUserId(string userid, string deptid, string tid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(userid))
            {
                query = query.Where(x => x.BorrwoPersonId == userid);
            }
            else
            {
                //string deptid = OperatorProvider.Provider.Current().DeptId;
                //运行班组共享数据
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

                if (!string.IsNullOrEmpty(tid)) query = query.Where(x => x.TypeId == tid);


            }
            total = query.Count();
            return query.OrderByDescending(x => x.BorrwoDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        #endregion

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ToolBorrowEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

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
        public void SaveForm(string keyValue, ToolBorrowEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                //new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.IsGood = entity.IsGood;
                entity1.BackDate = entity.BackDate;
                entity1.Remark = entity.Remark;

                this.BaseRepository().Update(entity1);
            }
        }
        #endregion
    }
}
