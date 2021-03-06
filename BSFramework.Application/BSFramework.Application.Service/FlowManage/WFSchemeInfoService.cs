using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.FlowManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.IService.FlowManage;
using BSFramework.Application.Service.BaseManage;
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

namespace BSFramework.Application.Service.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板信息表操作
    /// </summary>
    public class WFSchemeInfoService : RepositoryFactory, WFSchemeInfoIService
    {
        private IUserService userservice = new UserService();
        #region 获取数据
        /// <summary>
        /// 获取流程列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询条件</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(@"SELECT
	                            w.Id,
	                            w.SchemeCode,
	                            w.SchemeName,
	                            w.SchemeType,
                                w.SchemeVersion,
                                w.FrmType,
	                            t2.ItemName AS SchemeTypeName,
	                            w.SortCode,
	                            w.DeleteMark,
	                            w.EnabledMark,
	                            w.Description,
	                            w.CreateDate,
	                            w.CreateUserId,
	                            w.CreateUserName,
	                            w.ModifyDate,
	                            w.ModifyUserId,
	                            w.ModifyUserName,
                                w.AuthorizeType
                            FROM
	                            WF_SchemeInfo w
                            LEFT JOIN 
	                            Base_DataItemDetail t2 ON t2.ItemDetailId = w.SchemeType
                            WHERE w.DeleteMark = 0 ");
                var parameter = new List<DbParameter>();
                var queryParam = queryJson.ToJObject();
                if (!queryParam["SchemeType"].IsEmpty())
                {
                    strSql.Append(" AND w.SchemeType = @SchemeType ");
                    parameter.Add(DbParameters.CreateDbParameter("@SchemeType", queryParam["SchemeType"].ToString()));
                }
                else if (!queryParam["Keyword"].IsEmpty())//关键字查询
                {
                    string keyord = queryParam["Keyword"].ToString();
                    strSql.Append(@" AND ( w.SchemeCode LIKE @keyword 
                                        or w.SchemeName LIKE @keyword 
                                        or w.Description LIKE @keyword 
                    )");

                    parameter.Add(DbParameters.CreateDbParameter("@keyword", '%' + keyord + '%'));
                }
                return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray(), pagination);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取流程列表数据
        /// </summary>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public DataTable GetList(string userid, string objectid, string queryJson)
        {
            try
            {
                string dd = objectid;
                var strSql = new StringBuilder();
                strSql.Append(@"SELECT
	                            distinct w.Id,
	                            w.SchemeCode,
	                            w.SchemeName,
	                            w.SchemeType,
                                w.SchemeVersion,
                                w.FrmType,
	                            t2.ItemName AS SchemeTypeName,
	                            w.SortCode,
	                            w.DeleteMark,
	                            w.EnabledMark,
	                            w.Description,
	                            w.CreateDate,
	                            w.CreateUserId,
	                            w.CreateUserName,
	                            w.ModifyDate,
	                            w.ModifyUserId,
	                            w.ModifyUserName
                            FROM
	                            WF_SchemeInfo w
                            LEFT JOIN 
	                            Base_DataItemDetail t2 ON t2.ItemDetailId = w.SchemeType
                            LEFT JOIN 
	                            WF_SchemeInfoAuthorize w2 ON w2.SchemeInfoId = w.Id
                            WHERE w.DeleteMark = 0 AND w.EnabledMark = 1 AND w.FrmType = 0
                            AND ( w.AuthorizeType = 0  ");
                if (userid.ToLower() != "system")
                {
                    if (objectid != "")
                    {
                        strSql.Append(string.Format(" OR w2.ObjectId in ('{0}','{1}') )", objectid, userid));
                    }
                    else
                    {
                        strSql.Append(" ) ");
                    }
                }
                else
                {
                    strSql.Append(" OR w.AuthorizeType = 1 ) ");
                }


                var parameter = new List<DbParameter>();
                var queryParam = queryJson.ToJObject();

                if (!queryParam["SchemeType"].IsEmpty())
                {
                    strSql.Append(" AND w.SchemeType = @SchemeType ");
                    parameter.Add(DbParameters.CreateDbParameter("@SchemeType", queryParam["SchemeType"].ToString()));
                }
                if (!queryParam["Keyword"].IsEmpty())//关键字查询
                {
                    string keyord = queryParam["Keyword"].ToString();
                    strSql.Append(@" AND ( w.SchemeCode LIKE @keyword 
                                        or w.SchemeName LIKE @keyword 
                                        or w.Description LIKE @keyword 
                    )");

                    parameter.Add(DbParameters.CreateDbParameter("@keyword", '%' + keyord + '%'));

                }
                return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取流程模板列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetList()
        {
            try
            {
                var strSql = new StringBuilder();
                strSql.Append(@"SELECT
	                            w1.*,
	                            t2.ItemName,
                                t2.ItemDetailId as ItemId
                            FROM
                                WF_SchemeInfo w1
                            LEFT JOIN Base_DataItemDetail t2 ON t2.ItemDetailId = w1.SchemeType
                            ORDER BY t2.SortCode");
                return this.BaseRepository().FindTable(strSql.ToString());
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 设置流程
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public WFSchemeInfoEntity GetEntity(string keyValue)
        {
            try
            {
                return this.BaseRepository().FindEntity<WFSchemeInfoEntity>(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取权限列表数据
        /// </summary>
        /// <param name="schemeInfoId"></param>
        /// <returns></returns>
        public IEnumerable<WFSchemeInfoAuthorizeEntity> GetAuthorizeEntityList(string schemeInfoId)
        {
            try
            {
                var expression = LinqExtensions.True<WFSchemeInfoAuthorizeEntity>();
                expression = expression.And(t => t.SchemeInfoId == schemeInfoId);
                return this.BaseRepository().FindList<WFSchemeInfoAuthorizeEntity>(expression);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 更新流程模板状态（启用，停用）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="status">状态 1:启用;0.停用</param>
        public void UpdateState(string keyValue, int state)
        {
            try
            {
                WFSchemeInfoEntity entity = new WFSchemeInfoEntity();
                entity.Modify(keyValue);
                entity.EnabledMark = state;
                this.BaseRepository().Update(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region


        #endregion
    }
}
