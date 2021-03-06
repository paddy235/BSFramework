using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 描 述：数据库管理
    /// </summary>
    public class DataBaseTableService : RepositoryFactory, IDataBaseTableService
    {
        private IDataBaseLinkService dataBaseLinkService = new DataBaseLinkService();

        #region 获取数据
        /// <summary>
        /// 数据表列表
        /// </summary>
        /// <param name="dataBaseLinkId">库连接Id</param>
        /// <param name="tableName">表明</param>
        /// <returns></returns>
        public DataTable GetTableList(string dataBaseLinkId, string tableName)
        {
            DataBaseLinkEntity dataBaseLinkEntity = dataBaseLinkService.GetEntity(dataBaseLinkId);
            StringBuilder strSql = new StringBuilder();
            if (dataBaseLinkEntity != null)
            {
                //SqlServer条件下的查询
                //if (DbHelper.DbType == DatabaseType.SqlServer)
                if (dataBaseLinkEntity.DbType =="SqlServer")
                {
                    strSql.Append(@"DECLARE @TableInfo TABLE ( name VARCHAR(50) , sumrows VARCHAR(11) , reserved VARCHAR(50) , data VARCHAR(50) , index_size VARCHAR(50) , unused VARCHAR(50) , pk VARCHAR(50) )
                            DECLARE @TableName TABLE ( name VARCHAR(50) )
                            DECLARE @name VARCHAR(50)
                            DECLARE @pk VARCHAR(50)
                            INSERT INTO @TableName ( name ) SELECT o.name FROM sysobjects o , sysindexes i WHERE o.id = i.id AND o.Xtype = 'U' AND i.indid < 2 ORDER BY i.rows DESC , o.name
                            WHILE EXISTS ( SELECT 1 FROM @TableName ) BEGIN SELECT TOP 1 @name = name FROM @TableName DELETE @TableName WHERE name = @name DECLARE @objectid INT SET @objectid = OBJECT_ID(@name) SELECT @pk = COL_NAME(@objectid, colid) FROM sysobjects AS o INNER JOIN sysindexes AS i ON i.name = o.name INNER JOIN sysindexkeys AS k ON k.indid = i.indid WHERE o.xtype = 'PK' AND parent_obj = @objectid AND k.id = @objectid INSERT INTO @TableInfo ( name , sumrows , reserved , data , index_size , unused ) EXEC sys.sp_spaceused @name UPDATE @TableInfo SET pk = @pk WHERE name = @name END
                            SELECT F.name , F.reserved , F.data , F.index_size , RTRIM(F.sumrows) AS sumrows , F.unused , ISNULL(p.tdescription, f.name) AS tdescription , F.pk
                            FROM @TableInfo F LEFT JOIN ( SELECT name = CASE WHEN A.COLORDER = 1 THEN D.NAME ELSE '' END , tdescription = CASE WHEN A.COLORDER = 1 THEN ISNULL(F.VALUE, '') ELSE '' END FROM SYSCOLUMNS A LEFT JOIN SYSTYPES B ON A.XUSERTYPE = B.XUSERTYPE INNER JOIN SYSOBJECTS D ON A.ID = D.ID AND D.XTYPE = 'U' AND D.NAME <> 'DTPROPERTIES' LEFT JOIN sys.extended_properties F ON D.ID = F.major_id WHERE a.COLORDER = 1 AND F.minor_id = 0 ) P ON F.name = p.name
                            WHERE 1 = 1 ");
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        strSql.Append(" AND f.name like '" + tableName + "%'");
                    }
                    strSql.Append(" ORDER BY f.name");
                    return this.BaseRepository(dataBaseLinkEntity.DbConnection).FindTable(strSql.ToString());
                }
                else if (dataBaseLinkEntity.DbType == "Oracle")
                {
                    strSql.Append(@"select a.table_name as name,a.num_rows as sumrows , a.last_analyzed as updatetime , d.column_name as pk ,e.comments as tdescription ,0 as reserved  from  user_all_tables a
                                left join user_constraints c on a.table_name =c.TABLE_NAME
                                left join user_cons_columns d on c.constraint_name= d.constraint_name 
                                left join user_tab_comments e on a.table_name = e.table_name
                                where c.constraint_type ='P' 
                                ");
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        strSql.Append(" and a.name like '" + tableName + "%'");
                    }
                    strSql.Append(" order by a.table_name");
                    return this.BaseRepository(dataBaseLinkEntity.DBAlias, DatabaseType.Oracle).FindTable(strSql.ToString());
                }
                else if (dataBaseLinkEntity.DbType == "MySql")
                {
                    strSql.Append(@"select a.table_name as name from information_schema.tables a where table_schema = 'fd' and table_type = 'BASE TABLE'
                                ");
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        strSql.Append(" and a.name like '" + tableName + "%'");
                    }
                    strSql.Append(" order by a.table_name");
                    return this.BaseRepository(dataBaseLinkEntity.DBAlias, DatabaseType.MySql).FindTable(strSql.ToString());
                }

            }
            return null;
        }
        /// <summary>
        /// 数据表字段列表
        /// </summary>
        /// <param name="dataBaseLinkId">库连接Id</param>
        /// <param name="tableName">表明</param>
        /// <returns></returns>
        public IEnumerable<DataBaseTableFieldEntity> GetTableFiledList(string dataBaseLinkId, string tableName)
        {
            DataBaseLinkEntity dataBaseLinkEntity = dataBaseLinkService.GetEntity(dataBaseLinkId);
            if (dataBaseLinkEntity != null)
            {
                if (dataBaseLinkEntity.DbType == "SqlServer")
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT [column_id] = a.colorder , [column_name] = a.name , [datatype] = b.name , [length_data] = COLUMNPROPERTY(a.id, a.name, 'PRECISION') , [identity_data] = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1 THEN '1' ELSE '' END , [key] = CASE WHEN EXISTS ( SELECT 1 FROM sysobjects WHERE xtype = 'PK' AND parent_obj = a.id AND name IN ( SELECT name FROM sysindexes WHERE indid IN ( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid = a.colid ) ) ) THEN '1' ELSE '' END , [isnullable] = CASE WHEN a.isnullable = 1 THEN '1' ELSE '' END , [defaults] = ISNULL(e.text, '') , [remark] = ISNULL(g.[value], a.name)
                                FROM syscolumns a LEFT JOIN systypes b ON a.xusertype = b.xusertype INNER JOIN sysobjects d ON a.id = d.id AND d.xtype = 'U' AND d.name <> 'dtproperties' LEFT JOIN syscomments e ON a.cdefault = e.id LEFT JOIN sys.extended_properties g ON a.id = g.major_id AND a.colid = g.minor_id LEFT JOIN sys.extended_properties f ON d.id = f.major_id AND f.minor_id = 0
                                WHERE d.name = @tableName
                                ORDER BY a.id , a.colorder");
                        DbParameter[] parameter = 
                    {
                        DbParameters.CreateDbParameter("@tableName",tableName)
                    };
                        return this.BaseRepository(dataBaseLinkEntity.DbConnection).FindList<DataBaseTableFieldEntity>(strSql.ToString(), parameter);
                }
                else if (dataBaseLinkEntity.DbType == "Oracle")
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.AppendFormat(@"select a.column_id  , a.column_name ,a.data_type as datatype,a.data_length as length_data ,'' identity_data, (case when length(c.column_name)>0 then '1' else '' end)   as key, (case when a.nullable ='N' then '' else '1' end) as isnullable, '' defaults,b.comments as remark  from user_tab_columns a 
                                            left join  user_col_comments b on a.table_name = b.table_name and a.column_name = b.column_name
                                            left join (
                                                select b.table_name , b.column_name from  user_constraints a
                                                left join user_cons_columns b on a.constraint_name= b.constraint_name 
                                                where a.table_name='{0}' and a.constraint_type ='P'
                                            ) c on a.table_name = c.table_name and a.column_name = c.column_name
                                            where a.table_name='{0}' ", tableName);
                    return this.BaseRepository(dataBaseLinkEntity.DBAlias, DatabaseType.Oracle).FindList<DataBaseTableFieldEntity>(strSql.ToString());
                }
            }
            return null;
        }
        /// <summary>
        /// 数据库表数据列表
        /// </summary>
        /// <param name="dataBaseLinkId">库连接</param>
        /// <param name="tableName">表明</param>
        /// <param name="switchWhere">条件</param>
        /// <param name="logic">逻辑</param>
        /// <param name="keyword">关键字</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public DataTable GetTableDataList(string dataBaseLinkId, string tableName, string switchWhere, string logic, string keyword, Pagination pagination)
        {
            DataBaseLinkEntity dataBaseLinkEntity = dataBaseLinkService.GetEntity(dataBaseLinkId);
            if (dataBaseLinkEntity != null)
            {
                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append("SELECT * FROM " + tableName + " WHERE 1=1");
                if (!string.IsNullOrEmpty(keyword))
                {
                    strSql.Append(" AND " + switchWhere + "");
                    switch (logic)
                    {
                        case "Equal":           //等于
                            strSql.Append(" = ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "NotEqual":        //不等于
                            strSql.Append(" <> ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "Greater":         //大于
                            strSql.Append(" > ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "GreaterThan":     //大于等于
                            strSql.Append(" >= ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "Less":            //小于
                            strSql.Append(" < ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "LessThan":        //小于等于
                            strSql.Append(" >= ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "Null":            //为空
                            strSql.Append(" is null ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "NotNull":         //不为空
                            strSql.Append(" is not null ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, keyword));
                            break;
                        case "Like":            //包含
                            strSql.Append(" like ");
                            parameter.Add(DbParameters.CreateDbParameter("@" + switchWhere, '%' + keyword + '%'));
                            break;
                        default:
                            break;
                    }
                    strSql.Append("@" + switchWhere + "");
                }
                if (dataBaseLinkEntity.DbType == "SqlServer")
                {
                    return this.BaseRepository(dataBaseLinkEntity.DbConnection).FindTable(strSql.ToString(), parameter.ToArray(), pagination);
                }
                else if (dataBaseLinkEntity.DbType == "Oracle")
                {
                    return this.BaseRepository(dataBaseLinkEntity.DBAlias, DatabaseType.Oracle).FindTable(strSql.ToString(), parameter.ToArray(), pagination);
                }

            }
            return null;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据库表表单（新增、修改）
        /// </summary>
        /// <param name="dataBaseLinkId">库连接Id</param>
        /// <param name="tableName">表名称</param>
        /// <param name="tableDescription">表说明</param>
        /// <param name="fieldList">字段列表</param>
        public void SaveForm(string dataBaseLinkId, string tableName, string tableDescription, IEnumerable<DataBaseTableFieldEntity> fieldList)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("if exists (select 1");
            strSql.Append("            from  sysobjects");
            strSql.Append("            where  id = object_id('" + tableName + "')");
            strSql.Append("            and   type = 'U')");
            strSql.Append("   drop table " + tableName + "");
            strSql.Append("go");

            strSql.Append("/*==============================================================*/");
            strSql.Append("/* Table: " + tableName + "                                     */");
            strSql.Append("/*==============================================================*/");
            strSql.Append("create table " + tableName + " (");


            strSql.Append("   LogId                varchar(50)        not null,");
            strSql.Append("   SourceObjectId       varchar(50)        null,");
            strSql.Append("   SourceContentJson    text               null,");
            strSql.Append("   IPAddress            varchar(50)        null,");
            strSql.Append("   IPAddressName        varchar(200)       null,");
            strSql.Append("   Category             int                null,");



            strSql.Append("   constraint PK_BASE_LOG primary key nonclustered (LogId)");
            strSql.Append(")");
            strSql.Append("go");

            //declare @CurrentUser sysname
            //select @CurrentUser = user_name()
            //execute sp_addextendedproperty 'MS_Description', 
            //   '系统日志表',
            //   'user', @CurrentUser, 'table', 'Base_Log'
            //go

            //declare @CurrentUser sysname
            //select @CurrentUser = user_name()
            //execute sp_addextendedproperty 'MS_Description', 
            //   '日志主键',
            //   'user', @CurrentUser, 'table', 'Base_Log', 'column', 'LogId'
            //go
        }
        #endregion
    }
}
