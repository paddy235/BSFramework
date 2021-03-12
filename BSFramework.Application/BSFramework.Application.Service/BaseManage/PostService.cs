using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.AuthorizeManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 描 述：岗位管理
    /// </summary>
    public class PostService : RepositoryFactory<RoleEntity>, IPostService
    {
        //private IAuthorizeService<RoleEntity> iauthorizeservice = new AuthorizeService<RoleEntity>();

        #region 获取数据
        /// <summary>
        /// 岗位列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetList()
        {
            var expression = LinqExtensions.True<RoleEntity>();
            expression = expression.And(t => t.Category == 2).And(t => t.EnabledMark == 1).And(t => t.DeleteMark == 0);
            return this.BaseRepository().IQueryable(expression).OrderByDescending(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 岗位列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable(t=>t.Category==2);
            //var expression = LinqExtensions.True<RoleEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "EnCode":            //岗位编号
                        query = query.Where(t => t.EnCode.Contains(keyword));
                        //expression = expression.And(t => t.EnCode.Contains(keyword));
                        break;
                    case "FullName":          //岗位名称
                        //expression = expression.And(t => t.FullName.Contains(keyword));
                        query = query.Where(t => t.FullName.Contains(keyword));
                        break;
                    default:
                        break;
                }
            }
            //expression = expression.And(t => t.Category == 2);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.EnCode), out count);
            pagination.records = count;
            return data;
            //return this.BaseRepository().FindList(expression, pagination);
        }

        public IEnumerable<RoleEntity> GetQuartersList(string deptid)
        {
            var data = BaseRepository().IQueryable(t => t.Category == 2 && t.OrganizeId == deptid).OrderBy(x => x.EnCode).ToList();
            return data;
            //return this.BaseRepository().FindList("select * from base_role where Category = '2 ' and OrganizeId ='" + deptid + "' order by encode");
        }
        /// <summary>
        /// 岗位列表(ALL)
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
            //        WHERE   o.FullName is not null and r.Category = 2 and r.EnabledMark =1
            //        ORDER BY o.FullName, r.SortCode");
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<RoleEntity>()
                        join q2 in db.IQueryable<OrganizeEntity>() on q1.OrganizeId equals q2.OrganizeId into t
                        from t1 in t.DefaultIfEmpty()
                        where !string.IsNullOrWhiteSpace(t1.FullName) && q1.Category == 2 && q1.EnabledMark == 1
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
        /// 岗位实体
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
        /// 岗位编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var expression = LinqExtensions.True<RoleEntity>();
            expression = expression.And(t => t.EnCode == enCode).And(t => t.Category == 2);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.RoleId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 岗位名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var expression = LinqExtensions.True<RoleEntity>();
            expression = expression.And(t => t.FullName == fullName).And(t => t.Category == 2);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.RoleId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存岗位表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="postEntity">岗位实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, RoleEntity postEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                postEntity.Modify(keyValue);
                this.BaseRepository().Update(postEntity);
            }
            else
            {
                postEntity.Create();
                postEntity.Category = 2;
                this.BaseRepository().Insert(postEntity);
            }
        }
        #endregion
    }
}
