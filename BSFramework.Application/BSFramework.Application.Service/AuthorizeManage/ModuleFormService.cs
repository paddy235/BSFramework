using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace BSFramework.Application.Service.AuthorizeManage
{
    /// <summary>
    /// 描 述：系统表单
    /// </summary>
    public class ModuleFormService : RepositoryFactory, IModuleFormService
    {
        #region 获取数据
        /// <summary>
        /// 获取分页数据(管理页面调用)
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(@"SELECT
	                                m.FormId,
	                                m.ModuleId,
	                                m1.FullName as ModuleName,
	                                m.EnCode,
	                                m.FullName,
	                                m.SortCode,
	                                m.DeleteMark,
	                                m.EnabledMark,
	                                m.Description,
	                                m.CreateDate,
	                                m.CreateUserId,
	                                m.CreateUserName,
	                                m.ModifyDate,
	                                m.ModifyUserId,
	                                m.ModifyUserName
                                FROM
	                                Base_ModuleForm m
                                LEFT JOIN Base_Module m1 ON m1.ModuleId = m.ModuleId
                                WHERE m.DeleteMark = 0");

                var parameter = new List<DbParameter>();
                var queryParam = queryJson.ToJObject();
                if (!queryParam["Keyword"].IsEmpty())//关键字查询
                {
                    string keyord = queryParam["Keyword"].ToString();
                    var dbtype = DbHelper.DbType;
                    strSql.Append(@" AND ( m1.FullName LIKE @keyword 
                                        or m.FullName LIKE @keyword 
                                        or m.CreateUserName LIKE @keyword 
                    )");
                    parameter.Add(DbParameters.CreateDbParameter("@keyword", '%' + keyord + '%'));
                    return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray(), pagination);
                }
                else
                {
                    return this.BaseRepository().FindTable(strSql.ToString(),null, pagination);
                }
             
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 获取一个实体类
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ModuleFormEntity GetEntity(string keyValue)
        {
            try
            {
                return this.BaseRepository().FindEntity<ModuleFormEntity>(keyValue);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 通过模块Id获取系统表单
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public ModuleFormEntity GetEntityByModuleId(string moduleId)
        {
            try
            {
                var expression = LinqExtensions.True<ModuleFormEntity>();
                expression = expression.And(t => t.ModuleId.Equals(moduleId));
                return this.BaseRepository().FindEntity<ModuleFormEntity>(expression);
            }
            catch
            {
                throw;
            }
          
        }
        /// <summary>
        /// 判断模块是否已经有系统表单
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public bool IsExistModuleId(string keyValue,string moduleId)
        {
            var expression = LinqExtensions.True<ModuleFormEntity>();
            if(string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.ModuleId.Equals(moduleId));
            }
            else
            {
                expression = expression.And(t => t.ModuleId.Equals(moduleId) && t.FormId != keyValue);
            }
            ModuleFormEntity entity = this.BaseRepository().FindEntity<ModuleFormEntity>(expression);
            return entity == null ? false : true;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存一个实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int SaveEntity(string keyValue, ModuleFormEntity entity)
        {
            try
            {
                if (string.IsNullOrEmpty(keyValue))
                {
                    entity.Create();
                    return this.BaseRepository().Insert(entity);
                }
                else
                {
                    entity.Modify(keyValue);
                    return this.BaseRepository().Update(entity);
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 虚拟删除一个实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int VirtualDelete(string keyValue)
        {
            try
            {
                ModuleFormEntity entity = this.BaseRepository().FindEntity<ModuleFormEntity>(keyValue);
                if (entity != null)
                {
                    entity.DeleteMark = 1;
                    return this.BaseRepository().Update(entity);
                }
                else
                {
                    throw (new Exception("没有该记录无法删除"));
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
