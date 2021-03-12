using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using BSFramework.Data;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.AuthorizeManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Data.EF;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 描 述：角色管理
    /// </summary>
    public class RoleService : RepositoryFactory<RoleEntity>, IRoleService
    {
        //private IAuthorizeService<RoleEntity> iauthorizeservice = new AuthorizeService<RoleEntity>();
        private System.Data.Entity.DbContext _context;

        public RoleService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        #region 获取数据
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetList()
        {
            var expression = LinqExtensions.True<RoleEntity>();
            expression = expression.And(t => t.Category == 1).And(t => t.EnabledMark == 1).And(t => t.DeleteMark == 0);
            return this.BaseRepository().IQueryable(expression).OrderByDescending(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable(x => x.Category == 1);

            //StringBuilder sb = new StringBuilder("select t.OrganizeId,t.RoleId,t.EnCode,t.FullName,o.FullName OrgName,t.CreateDate,t.IsPublic,t.EnabledMark,t.description from BASE_ROLE t left join base_organize o on t.organizeid=o.organizeid where t.Category = 1");
            //var param = new List<DbParameter>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "EnCode":            //角色编号
                        //sb.AppendFormat(" and t.EnCode like @EnCode");
                        query = query.Where(x => x.EnCode.Contains(keyword));
                        //param.Add(DbParameters.CreateDbParameter("@EnCode", '%' + keyword + '%'));
                        break;
                    case "FullName":          //角色名称
                        //sb.AppendFormat(" and t.FullName like @FullName");
                        query = query.Where(x => x.FullName.Contains(keyword));
                        //param.Add(DbParameters.CreateDbParameter("@FullName", '%' + keyword + '%'));

                        break;
                    default:
                        break;
                }
            }
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.EnCode), out count);
            pagination.records = count;
            return data;
        }
        /// <summary>
        /// 角色列表all
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetAllList()
        {
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  r.RoleId ,
            //            o.FullName AS OrganizeId ,
            //            r.Category ,
            //            r.EnCode ,
            //            r.FullName ,
            //            r.SortCode ,
            //            r.EnabledMark ,
            //            r.Description ,
            //            r.CreateDate
            //        FROM    Base_Role r
            //            LEFT JOIN Base_Organize o ON o.OrganizeId = r.OrganizeId
            //        WHERE   o.FullName is not null and r.Category = 1 and r.EnabledMark =1
            //        ORDER BY o.FullName, r.SortCode");
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<RoleEntity>()
                        join q2 in db.IQueryable<OrganizeEntity>() on q1.OrganizeId equals q2.OrganizeId into t
                        from t1 in t.DefaultIfEmpty()
                        where !string.IsNullOrWhiteSpace(t1.FullName) && q1.Category == 1 && q1.EnabledMark == 1
                        orderby t1.FullName, q1.SortCode descending
                        select new
                        {
                            q1,
                            t1.FullName
                        };
            var data = query.ToList().Select(x => new RoleEntity
            {
                RoleId = x.q1.RoleId,
                OrganizeId = x.FullName,
                Category = x.q1.Category,
                EnCode = x.q1.EnCode,
                FullName = x.q1.FullName,
                SortCode = x.q1.SortCode,
                EnabledMark = x.q1.EnabledMark,
                Description = x.q1.Description,
                CreateDate = x.q1.CreateDate,
            }).ToList();


            return data;
        }
        /// <summary>
        /// 角色实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public RoleEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 角色编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var expression = LinqExtensions.True<RoleEntity>();
            expression = expression.And(t => t.EnCode == enCode).And(t => t.Category == 1);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.RoleId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 角色名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var expression = LinqExtensions.True<RoleEntity>();
            expression = expression.And(t => t.FullName == fullName).And(t => t.Category == 1);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.RoleId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存角色表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="roleEntity">角色实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, RoleEntity roleEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.Modify(keyValue);
                this.BaseRepository().Update(roleEntity);
            }
            else
            {
                roleEntity.Create();
                roleEntity.Category = 1;
                this.BaseRepository().Insert(roleEntity);
            }
        }

        public void Save(List<RoleEntity> roles)
        {
            var db = DbFactory.Base() as Database;
            var dbset = db.dbcontext.Set<RoleEntity>();

            foreach (var item in roles)
            {
                var entity = dbset.Find(item.RoleId);
                if (entity == null) dbset.Add(item);
                else
                {
                    db.dbcontext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    entity.EnCode = item.EnCode;
                    entity.FullName = item.FullName;
                    entity.ModifyDate = item.ModifyDate;
                    entity.ModifyUserName = item.ModifyUserName;
                }
            }
            db.dbcontext.SaveChanges();
        }

        public RoleEntity Delete(string id)
        {
            var db = DbFactory.Base() as Database;
            var dbset = db.dbcontext.Set<RoleEntity>();

            var entity = dbset.Find(id);
            if (entity == null) return null;
            else
                db.dbcontext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

            db.dbcontext.SaveChanges();
            return entity;
        }

        public List<RoleEntity> GetPageList(string rolename, int pagesize, int pageindex, out int total)
        {
            var query = from q in _context.Set<RoleEntity>()
                        where q.Category == 1
                        select q;

            if (!string.IsNullOrEmpty(rolename))
            {
                query = query.Where(x => x.FullName.Contains(rolename));
            }

            total = query.Count();
            return query.OrderBy(x => x.EnCode).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
        }
        #endregion
    }
}
