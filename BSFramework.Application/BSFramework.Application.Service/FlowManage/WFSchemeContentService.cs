using BSFramework.Application.IService.FlowManage;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.FlowManage;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.Common;
using BSFramework.Data;

namespace BSFramework.Application.Service.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板内容表操作
    /// </summary>
    public class WFSchemeContentService : RepositoryFactory, WFSchemeContentIService
    {
        #region 获取数据
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="wfSchemeInfoId">工作流模板信息表主键</param>
        /// <param name="schemeVersion">模板版本号</param>
        /// <returns></returns>
        public WFSchemeContentEntity GetEntity(string wfSchemeInfoId, string schemeVersion)
        {
            try
            {
                var expression = LinqExtensions.True<WFSchemeContentEntity>();
                expression = expression.And(t => t.WFSchemeInfoId == wfSchemeInfoId).And(t => t.SchemeVersion == schemeVersion);
                return  this.BaseRepository().FindEntity<WFSchemeContentEntity>(expression);
            }
            catch {
                throw;            
            }
        }
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="wfSchemeInfoId">工作流模板信息表Id</param>
        /// <returns></returns>
        public IEnumerable<WFSchemeContentEntity> GetEntityList(string wfSchemeInfoId)
        {
            var expression = LinqExtensions.True<WFSchemeContentEntity>();
            expression = expression.And(t => t.WFSchemeInfoId == wfSchemeInfoId);
            return this.BaseRepository().FindList<WFSchemeContentEntity>(expression);
        }
        /// <summary>
        /// 获取对象列表（不包括模板内容）
        /// </summary>
        /// <param name="wfSchemeInfoId">工作流模板信息表Id</param>
        /// <returns></returns>
        public DataTable GetTableList(string wfSchemeInfoId)
        {
            try
            {
                var parameter = new List<DbParameter>();
                var strSql = new StringBuilder();
            
                    strSql.Append(@"SELECT
	                            w.Id,
                                w.WFSchemeInfoId,
                                w.SchemeVersion,
	                            w.CreateDate,
	                            w.CreateUserId,
	                            w.CreateUserName
                            FROM
	                            WF_SchemeContent w
                            where w.WFSchemeInfoId = @wfSchemeInfoId
                            Order by  w.SchemeVersion DESC ");

                 parameter.Add(DbParameters.CreateDbParameter("@wfSchemeInfoId", wfSchemeInfoId));
                return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray());
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="entity">类</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public int SaveEntity(WFSchemeContentEntity entity, string keyValue)
        {
            try
            {
                if (string.IsNullOrEmpty(keyValue))
                {
                    entity.Create();
                    return this.BaseRepository().Insert<WFSchemeContentEntity>(entity);
                }
                else
                {
                    entity.Modify(keyValue);
                    return this.BaseRepository().Update<WFSchemeContentEntity>(entity);
                }
            }
            catch {
                throw;
            }
        }
        #endregion
    }
}
