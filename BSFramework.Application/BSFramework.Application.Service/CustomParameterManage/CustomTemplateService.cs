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
    /// 自定义模板
    /// </summary>
    public class CustomTemplateService : RepositoryFactory<CustomTemplateEntity>, ICustomTemplateService
    {
        #region 查询数据
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<CustomTemplateEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var Expression = LinqExtensions.True<CustomTemplateEntity>();
            var user = db.FindEntity<UserEntity>(userid);
           
            Expression = Expression.And(x => !x.IsDel);
            if (user.UserId == "System")
            {
                Expression = Expression.And(x => x.CreateUserName == "超级管理员");
            }
            else
            if (!queryParam["istemplate"].IsEmpty())
            {
                Expression = Expression.And(x =>x.CreateUserName== "超级管理员");
            }
            else
            {
                Expression = Expression.And(x => x.DeptCode.StartsWith(user.DepartmentCode));
            }
            if (!queryParam["TName"].IsEmpty())
            {
                var TName = queryParam["TName"].ToString();
                Expression = Expression.And(x => x.TemplateName.Contains(TName));
            }
            var entity = db.FindList(Expression).OrderByDescending(x=>x.CreateDate);

            pagination.records = entity.Count();
            var  entityList = entity.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            return entityList;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CustomTemplateEntity getEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<CustomTemplateEntity>(keyvalue);
            return entity;
        }

        /// <summary>
        /// 获取部门班组
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public List<CustomTemplateEntity> setSelect(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindList<CustomTemplateEntity>(x => !x.IsDel).OrderByDescending(x => x.CreateDate);

            if (string.IsNullOrEmpty(keyvalue))
            {
                return entity.ToList();
            }
            else
            {

                var dept = db.FindEntity<DepartmentEntity>(keyvalue);
                if (dept != null)
                {
                    entity = entity.Where(x=>!string.IsNullOrEmpty(x.UserDpetCode)).Where(x=>x.UserDpetCode.Split(',').Any(y=>y.StartsWith(dept.EnCode))).OrderByDescending(x => x.CreateDate);
                    return entity.ToList();
                }
                return new List<CustomTemplateEntity>();
            }

        }


        #endregion

        #region 操作数据

        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(CustomTemplateEntity entity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);

                var oldentity = db.FindEntity<CustomTemplateEntity>(entity.CTId);
                if (oldentity != null)
                {
                    oldentity.TitleContent = entity.TitleContent;
                    oldentity.TemplateName = entity.TemplateName;
                    oldentity.FormContent = entity.FormContent;
                    oldentity.ModifyDate = DateTime.Now;
                    oldentity.ModifyUserName = user.RealName;
                    oldentity.ModifyUserId = user.UserId;
                    oldentity.UserDpet = entity.UserDpet;
                    oldentity.UserDpetId = entity.UserDpetId;
                    oldentity.UserDpetCode = entity.UserDpetCode;
                    oldentity.TitleContentList = null;
                    oldentity.FormContentList = null;
                    db.Update(oldentity);

                }
                else
                {
                    if (string.IsNullOrEmpty(entity.CTId))
                    {
                        entity.CTId = Guid.NewGuid().ToString();
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
                var entity = db.FindEntity<CustomTemplateEntity>(keyvalue);
                if (entity != null)
                {
                    entity.IsDel = true;
                    entity.TitleContentList = null;
                    entity.FormContentList = null;
                    //db.Delete(entity);
                    db.Update(entity);
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
