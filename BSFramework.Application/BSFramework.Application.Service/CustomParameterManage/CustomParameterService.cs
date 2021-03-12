using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CustomParameterManage;
using BSFramework.Application.IService.CustomParameterManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.CustomParameterManage
{
    /// <summary>
    /// 自定义台账
    /// </summary>
    public class CustomParameterService : RepositoryFactory<CustomParameterEntity>, ICustomParameterService
    {
        #region 查询数据
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<CustomParameterEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var Expression = LinqExtensions.True<CustomParameterEntity>();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId == "System")
            {
                user.DepartmentCode = "0";
            }
            if (!queryParam["TName"].IsEmpty())
            {
                var TName = queryParam["TName"].ToString();
                Expression = Expression.And(x => x.CTId == TName);
            }
            if (!queryParam["StartTime"].IsEmpty())
            {
                var StartTIme = Convert.ToDateTime(queryParam["StartTime"].ToString());
                Expression = Expression.And(x => x.CreateDate >= StartTIme);
            }
            if (!queryParam["EndTime"].IsEmpty())
            {
                var EndTime = Convert.ToDateTime(queryParam["EndTime"].ToString()).AddDays(1).AddMilliseconds(-1);
                Expression = Expression.And(x => x.CreateDate <= EndTime);
            }
            if (!queryParam["CTId"].IsEmpty())
            {
                var CTId = queryParam["CTId"].ToString();
                Expression = Expression.And(x => x.CTId == CTId);
            }
            if (!queryParam["DeptId"].IsEmpty())
            {
                var DeptId = queryParam["DeptId"].ToString();
                var dept = db.FindEntity<DepartmentEntity>(DeptId);
                Expression = Expression.And(x => x.DeptCode.StartsWith(dept.EnCode));
            }
            else
            {
                Expression = Expression.And(x => x.DeptCode.StartsWith(user.DepartmentCode));
            }
            var entity = db.FindList(Expression).OrderByDescending(x => x.CreateDate);
            pagination.records = entity.Count();
            var entityList = entity.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return entityList.ToList();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CustomParameterEntity getEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<CustomParameterEntity>(keyvalue);
            return entity;
        }

        /// <summary>
        /// 根据模板id获取实际数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public IEnumerable<CustomParameterEntity> getListbyCTId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindList<CustomParameterEntity>(x => x.CTId == keyvalue);
            return entity;
        }


        #endregion
        #region 操作数据

        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(CustomParameterEntity entity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);

                var oldentity = db.FindEntity<CustomParameterEntity>(entity.CPId);
                if (oldentity != null)
                {
                    oldentity.TitleContent = entity.TitleContent;
                    oldentity.TemplateName = entity.TemplateName;
                    oldentity.FormContentText = entity.FormContentText;
                    oldentity.FormContent = entity.FormContent;
                    oldentity.ModifyDate = DateTime.Now;
                    oldentity.ModifyUserName = user.RealName;
                    oldentity.ModifyUserId = user.UserId;
                    oldentity.FormText = null;
                    oldentity.FormContentList = null;
                    oldentity.TitleContentList = null;
                    //oldentity.CTId = entity.CTId;
                    db.Update(oldentity);

                }
                else
                {
                    if (string.IsNullOrEmpty(entity.CPId))
                    {
                        entity.CPId = Guid.NewGuid().ToString();
                    }
                    entity.DeptCode = user.DepartmentCode;
                    entity.DeptId = user.DepartmentId;
                    entity.Dept = user.DepartmentName;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserName = user.RealName;
                    entity.ModifyUserId = user.UserId;
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserName = user.RealName;
                    entity.CreateUserId = user.UserId;
                    db.Insert(entity);

                }


                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<CustomParameterEntity>(keyvalue);
                if (entity != null)
                {
                    db.Delete(entity);
                }
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        #endregion

    }
}
