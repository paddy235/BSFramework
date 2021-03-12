using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Application.Service.AuthorizeManage;
using System;

namespace BSFramework.Application.Service.BaseManage
{
    /// 描 述：用户管理
    /// </summary>
    public class UserInfoService : RepositoryFactory<UserInfoEntity>, IUserInfoService
    {
        
        #region 获取数据

       
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
       //未使用
        public IEnumerable<UserInfoEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable();
            //var expression = LinqExtensions.True<UserInfoEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
           ;
                string condition = queryParam["condition"].ToString();
                string keyord = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "Account":            //账户
                        //expression = expression.And(t => t.Account.Contains(keyord));
                        query = query.Where(t => t.Account.Contains(keyord));
                        break;
                    case "RealName":          //姓名
                        //expression = expression.And(t => t.RealName.Contains(keyord));
                        query = query.Where(t => t.RealName.Contains(keyord));
                        break;
                    case "Mobile":          //手机
                        //expression = expression.And(t => t.Mobile.Contains(keyord));
                        query = query.Where(t => t.Mobile.Contains(keyord));
                        break;
                    default:
                        break;
                }
            }
            //expression = expression.And(t => t.UserId != "System");
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderBy(x => x.EnCode), out count);
            pagination.records = count;
            return data;
            //return this.BaseRepository().FindList(expression, pagination);
        }


        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserInfoEntity GetUserInfoEntity(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        join q3 in db.IQueryable<DepartmentEntity>() on q2.ParentId equals q3.DepartmentId into into3
                        from q3 in into3.DefaultIfEmpty()
                        where q1.UserId == keyValue
                        select new { q1, q2, q3 };

            var data = query.ToList();
            var resultData = data.Select(x => new UserInfoEntity() { UserId = x.q1.UserId, Account = x.q1.Account, RealName = x.q1.RealName, Gender = x.q1.Gender, Mobile = x.q1.Mobile, DeptName = x.q2.Nature == "班组" ? x.q3.FullName + "/" + x.q2.FullName : x.q2.FullName, DutyName = x.q1.DutyName, PostName = x.q1.PostName, RoleName = x.q1.RoleName, Manager = x.q1.Manager }).ToList();
            if (resultData.Count == 1)
            {
                return resultData.First();
            }
            else
            {
                if (resultData.Count > 1)
                {
                    throw new Exception("账户数据重复");
                }
                return new UserInfoEntity();
            }
            //return this.BaseRepository().FindEntity(keyValue);
        }


        /// <summary>
        /// 根据用户账号获取用户信息
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        public UserInfoEntity GetUserInfoByAccount(string account)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        join q3 in db.IQueryable<DepartmentEntity>() on q2.ParentId equals q3.DepartmentId into into3
                        from q3 in into3.DefaultIfEmpty()
                        where q1.Account==account
                        select new { q1, q2, q3 };

            var data = query.ToList();
            var resultData = data.Select(x => new UserInfoEntity() { UserId = x.q1.UserId, Account = x.q1.Account, RealName = x.q1.RealName, Gender = x.q1.Gender, Mobile = x.q1.Mobile, DeptName = x.q2.Nature == "班组" ? x.q3.FullName + "/" + x.q2.FullName : x.q2.FullName, DutyName = x.q1.DutyName, PostName = x.q1.PostName, RoleName = x.q1.RoleName, Manager = x.q1.Manager }).ToList();
            if (resultData.Count == 1)
            {
                return resultData.First();
            }
            else
            {
                if (resultData.Count > 1)
                {
                    throw new Exception("账户数据重复");
                }
                return new UserInfoEntity();
            }
            //var expression = LinqExtensions.True<UserInfoEntity>();
            //expression = expression.And(t => t.Account == account);
            //return this.BaseRepository().FindEntity(expression);
        }
        #endregion

    }
}
