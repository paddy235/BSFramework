using BSFramework.Data.EF.Extension;
using BSFramework.Util;
using BSFramework.Util.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using System.Data.Entity.Core.Objects;
using BSFramework.Util.WebControl;
//using MySql.Data.MySqlClient;
using BSFramework.Data;
using Oracle.ManagedDataAccess.Client;

namespace BSFramework.Data.EF
{
    /// <summary>
    /// 描 述：操作数据库
    /// </summary>
    public class Database : IDatabase
    {
        #region 构造函数
        ///// <summary>
        ///// 构造方法
        ///// </summary>
        public Database(string connString, string DbType)
        {
            if (DbType == "")
            {
                dbcontext = (DbContext)UnityIocHelper.DBInstance.GetService<IDbContext>(new ParameterOverride(
                          "connString", connString));
            }
            else
            {
                dbcontext = (DbContext)UnityIocHelper.DBInstance.GetService<IDbContext>(DbType, new ParameterOverride(
                           "connString", connString));
            }

        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取 当前使用的数据访问上下文对象
        /// </summary>
        public DbContext dbcontext { get; set; }
        /// <summary>
        /// 事务对象
        /// </summary>
        public DbTransaction dbTransaction { get; set; }
        #endregion

        #region 事物提交
        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns></returns>
        public IDatabase BeginTrans()
        {
            DbConnection dbConnection = ((IObjectContextAdapter)dbcontext).ObjectContext.Connection;
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            dbTransaction = dbConnection.BeginTransaction();
            return this;
        }
        /// <summary>
        /// 提交当前操作的结果
        /// </summary>
        public int Commit()
        {
            try
            {
                int returnValue = dbcontext.SaveChanges();
                if (dbTransaction != null)
                {
                    dbTransaction.Commit();
                    this.Close();
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException is SqlException)
                {
                    SqlException sqlEx = ex.InnerException.InnerException as SqlException;
                    string msg = ExceptionMessage.GetSqlExceptionMessage(sqlEx.Number);
                    throw DataAccessException.ThrowDataAccessException(sqlEx, msg);
                }
                throw;
            }
            finally
            {
                if (dbTransaction == null)
                {
                    this.Close();
                }
            }
        }
        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        public void Rollback()
        {
            this.dbTransaction.Rollback();
            this.dbTransaction.Dispose();
            this.Close();
        }
        /// <summary>
        /// 关闭连接 内存回收
        /// </summary>
        public void Close()
        {
            dbcontext.Dispose();
        }
        #endregion

        #region 执行 SQL 语句
        public int ExecuteBySql(string strSql)
        {
            if (dbTransaction == null)
            {
                return dbcontext.Database.ExecuteSqlCommand(strSql);
            }
            else
            {
                dbcontext.Database.ExecuteSqlCommand(strSql);
                return dbTransaction == null ? this.Commit() : 0;
            }
        }
        public int ExecuteBySql(string strSql, params DbParameter[] dbParameter)
        {
           
            DbParameter[] Params = new DbParameter[] { };
            Params = DbParameters.ToDbParameter(dbParameter);
            if (dbTransaction == null)
            {
                return dbcontext.Database.ExecuteSqlCommand(strSql, Params);
            }
            else
            {
                dbcontext.Database.ExecuteSqlCommand(strSql, Params);
                return dbTransaction == null ? this.Commit() : 0;
            }
        }
        public int ExecuteByProc(string procName)
        {
            if (dbTransaction == null)
            {
                return dbcontext.Database.ExecuteSqlCommand(DbContextExtensions.BuilderProc(procName));
            }
            else
            {
                dbcontext.Database.ExecuteSqlCommand(DbContextExtensions.BuilderProc(procName));
                return dbTransaction == null ? this.Commit() : 0;
            }
        }
        public int ExecuteByProc(string procName, params DbParameter[] dbParameter)
        {
            DbParameter[] Params = new DbParameter[] { };
            Params = DbParameters.ToDbParameter(dbParameter);
            if (dbTransaction == null)
            {
                return dbcontext.Database.ExecuteSqlCommand(DbContextExtensions.BuilderProc(procName, Params), Params);
            }
            else
            {

                dbcontext.Database.ExecuteSqlCommand(DbContextExtensions.BuilderProc(procName, Params), Params);
                return dbTransaction == null ? this.Commit() : 0;
            }
        }
        #endregion

        #region 对象实体 添加、修改、删除
        public int Insert<T>(T entity) where T : class
        {
            dbcontext.Entry<T>(entity).State = EntityState.Added;
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Insert<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                dbcontext.Entry<T>(entity).State = EntityState.Added;
            }
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Delete<T>() where T : class
        {
            EntitySet entitySet = DbContextExtensions.GetEntitySet<T>(dbcontext);
            if (entitySet != null)
            {
                string tableName = entitySet.MetadataProperties.Contains("Table") && entitySet.MetadataProperties["Table"].Value != null
                               ? entitySet.MetadataProperties["Table"].Value.ToString()
                               : entitySet.Name;
                return this.ExecuteBySql(DbContextExtensions.DeleteSql(tableName));
            }
            return -1;
        }
        public int Delete<T>(T entity) where T : class
        {
            dbcontext.Set<T>().Attach(entity);
            dbcontext.Set<T>().Remove(entity);
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Delete<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                dbcontext.Set<T>().Attach(entity);
                dbcontext.Set<T>().Remove(entity);
            }
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Delete<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            IEnumerable<T> entities = dbcontext.Set<T>().Where(condition).ToList();
            return entities.Count() > 0 ? Delete(entities) : 0;
        }
        public int Delete<T>(object keyValue) where T : class
        {
            EntitySet entitySet = DbContextExtensions.GetEntitySet<T>(dbcontext);
            if (entitySet != null)
            {
                string tableName = entitySet.MetadataProperties.Contains("Table") && entitySet.MetadataProperties["Table"].Value != null
                               ? entitySet.MetadataProperties["Table"].Value.ToString()
                               : entitySet.Name;
                string keyFlied = entitySet.ElementType.KeyMembers[0].Name;
                return this.ExecuteBySql(DbContextExtensions.DeleteSql(tableName, keyFlied, keyValue));
            }
            return -1;
        }
        public int Delete<T>(object[] keyValue) where T : class
        {
            EntitySet entitySet = DbContextExtensions.GetEntitySet<T>(dbcontext);
            if (entitySet != null)
            {
                string tableName = entitySet.MetadataProperties.Contains("Table") && entitySet.MetadataProperties["Table"].Value != null
                               ? entitySet.MetadataProperties["Table"].Value.ToString()
                               : entitySet.Name;
                string keyFlied = entitySet.ElementType.KeyMembers[0].Name;
                return this.ExecuteBySql(DbContextExtensions.DeleteSql(tableName, keyFlied, keyValue));
            }
            return -1;
        }
        public int Delete<T>(object propertyValue, string propertyName) where T : class
        {
            EntitySet entitySet = DbContextExtensions.GetEntitySet<T>(dbcontext);
            if (entitySet != null)
            {
                string tableName = entitySet.MetadataProperties.Contains("Table") && entitySet.MetadataProperties["Table"].Value != null
                               ? entitySet.MetadataProperties["Table"].Value.ToString()
                               : entitySet.Name;
                return this.ExecuteBySql(DbContextExtensions.DeleteSql(tableName, propertyName, propertyValue));
            }
            return -1;
        }
        public int Update<T>(T entity) where T : class
        {
            dbcontext.Set<T>().Attach(entity);
            Hashtable props = ConvertExtension.GetPropertyInfo<T>(entity);
            foreach (string item in props.Keys)
            {
                object value = dbcontext.Entry(entity).Property(item).CurrentValue;
                if (value != null)
                {
                    //if (value.ToString() == "&nbsp;")
                    //    dbcontext.Entry(entity).Property(item).CurrentValue = null;
                    dbcontext.Entry(entity).Property(item).IsModified = true;
                }
            }
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Update<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {  
                this.Update(entity);
            }
            return dbTransaction == null ? this.Commit() : 0;
        }
        public int Update<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return 0;
        }
        #endregion

        #region 对象实体 查询
        public T FindEntity<T>(object keyValue) where T : class
        {
            return dbcontext.Set<T>().Find(keyValue);
        }
        public T FindEntity<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return dbcontext.Set<T>().Where(condition).FirstOrDefault();
        }
        public IQueryable<T> IQueryable<T>() where T : class,new()
        {
            return dbcontext.Set<T>();
        }
        public IQueryable<T> IQueryable<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return dbcontext.Set<T>().Where(condition);
        }
        public IEnumerable<T> FindList<T>() where T : class,new()
        {
            return dbcontext.Set<T>().ToList();
        }
        public IEnumerable<T> FindList<T>(Func<T, object> keySelector) where T : class,new()
        {
            return dbcontext.Set<T>().OrderBy(keySelector).ToList();
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition) where T : class,new()
        {
            return dbcontext.Set<T>().Where(condition).ToList();
        }


        public IEnumerable<T> FindList<T>(string strSql) where T : class
        {
            return FindList<T>(strSql, null);
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
        {
            using (var dbConnection = dbcontext.Database.Connection)
            {
                DbParameter[] Params = new DbParameter[] { };
                Params = DbParameters.ToDbParameter(dbParameter);
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, strSql, Params);
                return ConvertExtension.IDataReaderToList<T>(IDataReader);
            }
        }

       
        public IEnumerable<T> FindList<T>(string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class,new()
        {
            string[] _order = orderField.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<T>().AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(T), "t");
                var property = typeof(T).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(T), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<T>(resultExp);
            total = tempData.Count();
            tempData = tempData.Skip<T>(pageSize * (pageIndex - 1)).Take<T>(pageSize).AsQueryable();
            return tempData.ToList();
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class,new()
        {
            string[] _order = orderField.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<T>().Where(condition);
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(T), "t");
                var property = typeof(T).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(T), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<T>(resultExp);
            total = tempData.Count();
            tempData = tempData.Skip<T>(pageSize * (pageIndex - 1)).Take<T>(pageSize).AsQueryable();
            return tempData.ToList();
        }
        public IEnumerable<T> FindList<T>(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class
        {
            return FindList<T>(strSql, null, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class
        {
            using (var dbConnection = dbcontext.Database.Connection)
            {
                StringBuilder sb = new StringBuilder();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                string OrderBy = "";

                if (!string.IsNullOrEmpty(orderField))
                {
                    if (orderField.ToUpper().IndexOf("ASC") + orderField.ToUpper().IndexOf("DESC") > 0)
                    {
                        OrderBy = "Order By " + orderField;
                    }
                    else
                    {
                        OrderBy = "Order By " + orderField + " " + (isAsc ? "ASC" : "DESC");
                    }
                }
                else
                {
                    OrderBy = "order by (select 0)";
                }
                var dbtype = DbHelper.DbType;
                if (dbtype == DatabaseType.SqlServer)
                {
                    sb.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
                    sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                }
                DbParameter[] Params = new DbParameter[] { };
                Params = DbParameters.ToDbParameter(dbParameter);
                if (dbtype == DatabaseType.Oracle)
                {
                    sb.Append("SELECT  * FROM (SELECT A.*, rownum r FROM (");
                    sb.Append(strSql + " " + OrderBy + ") A Where rownum<= " + num1 + " ) B where r>= " + num);
                }
                if (dbtype == DatabaseType.MySql)
                {
                    sb.Append(strSql + " " + OrderBy + "  limit " + num + "," + pageSize);
                }
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ")  t", Params));
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, sb.ToString(), Params);
                return ConvertExtension.IDataReaderToList<T>(IDataReader);
            }
        }

        /// <summary>
        /// 存储过程执行分页（目前支持oracle,sqlserver和mysql）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_kid"></param>
        /// <param name="p_fields"></param>
        /// <param name="p_tablename"></param>
        /// <param name="p_strwhere"></param>
        /// <param name="p_ordercolumn"></param>
        /// <param name="p_orderstyle"></param>
        /// <param name="p_curpage"></param>
        /// <param name="p_pagesize"></param>
        /// <param name="p_totalrecords"></param>
        /// <param name="p_totalpages"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public IEnumerable<T> FindListByProcPager<T>(Pagination pagination, DatabaseType dataType) where T : class, new()
        {
            IEnumerable<T> list = null;

            DbParameter[] param = null;

            string strSql = string.Empty;

            if (dataType == DatabaseType.Oracle)
            {
                param = new OracleParameter[11];

                param[0] = new OracleParameter("p_kid", OracleDbType.Varchar2, 50, pagination.p_kid, ParameterDirection.Input);
                param[1] = new OracleParameter("p_fields", OracleDbType.Varchar2, 4000, pagination.p_fields, ParameterDirection.Input);
                param[2] = new OracleParameter("p_tableName", OracleDbType.Varchar2, 4000, pagination.p_tablename, ParameterDirection.Input);
                param[3] = new OracleParameter("p_strWhere", OracleDbType.Varchar2, 4000, pagination.conditionJson, ParameterDirection.Input);
                param[4] = new OracleParameter("p_orderColumn", OracleDbType.Varchar2, 4000, pagination.sidx, ParameterDirection.Input);
                param[5] = new OracleParameter("p_orderStyle", OracleDbType.Varchar2, 50, pagination.sord, ParameterDirection.Input);
                param[6] = new OracleParameter("p_curPage", OracleDbType.Int32, pagination.page, ParameterDirection.InputOutput);
                param[7] = new OracleParameter("p_pageSize", OracleDbType.Int32, pagination.rows, ParameterDirection.InputOutput);
                param[8] = new OracleParameter("p_totalRecords", OracleDbType.Int32, 0, ParameterDirection.Output);
                param[9] = new OracleParameter("p_totalPages", OracleDbType.Int32, 0, ParameterDirection.Output);
                param[10] = new OracleParameter("v_cur", OracleDbType.RefCursor, ParameterDirection.Output);

                strSql = "prc_query";
            }
            if (dataType == DatabaseType.SqlServer) 
            {
                param = new SqlParameter[8];

                param[0] = new SqlParameter("@pageIndex",SqlDbType.Int); //当前页
                param[0].Value = pagination.page;
                param[1] = new SqlParameter("@pageSize",SqlDbType.Int); //每页记录数
                param[1].Value = pagination.rows;
                param[2] = new SqlParameter("@pkid",SqlDbType.VarChar,64); //主键
                param[2].Value = pagination.p_kid;
                param[3] = new SqlParameter("@fieldName",SqlDbType.VarChar,1024); //查询字段
                param[3].Value = pagination.p_fields;
                param[4] = new SqlParameter("@tableName",SqlDbType.VarChar,1024); //表名
                param[4].Value = pagination.p_tablename;
                param[5] = new SqlParameter("@whereStr",SqlDbType.VarChar,2000); //条件
                param[5].Value = pagination.conditionJson;
                param[6] = new SqlParameter("@orderStr", SqlDbType.VarChar, 64); //排序条件
                param[6].Value = pagination.sidx + " " + pagination.sord;
                param[7] = new SqlParameter("@recordTotal", SqlDbType.Int); //输出记录总数
                param[7].Value = 0;
                param[7].Direction = ParameterDirection.InputOutput;

                strSql = "prc_query";
            }
            if (dataType == DatabaseType.MySql)
            {
                //param = new MySqlParameter[9];
                //MySqlParameter par = new MySqlParameter("?p_kid", MySqlDbType.VarChar, 16);
                //par.Value = pagination.p_kid; par.Direction = ParameterDirection.Input;
                //param[0] = par;

                //par = new MySqlParameter("?p_fields", MySqlDbType.VarChar, 1024);
                //par.Value = pagination.p_fields; par.Direction = ParameterDirection.Input;
                //param[1] = par;

                //par = new MySqlParameter("?p_tableName", MySqlDbType.VarChar, 1024);
                //par.Value = pagination.p_tablename; par.Direction = ParameterDirection.Input;
                //param[2] = par;

                //par = new MySqlParameter("?p_strWhere", MySqlDbType.VarChar, 1024);
                //par.Value = pagination.conditionJson; par.Direction = ParameterDirection.Input;
                //param[3] = par;

                //par = new MySqlParameter("?p_orderColumn", MySqlDbType.VarChar, 128);
                //par.Value = pagination.sidx; par.Direction = ParameterDirection.Input;
                //param[4] = par;

                //par = new MySqlParameter("?p_orderStyle", MySqlDbType.VarChar, 4);
                //par.Value = pagination.sord; par.Direction = ParameterDirection.Input;
                //param[5] = par;

                //par = new MySqlParameter("?p_curPage", MySqlDbType.Int32);
                //par.Value = pagination.page; par.Direction = ParameterDirection.Input;
                //param[6] = par;

                //par = new MySqlParameter("?p_pageSize", MySqlDbType.Int32);
                //par.Value = pagination.rows; par.Direction = ParameterDirection.Input;
                //param[7] = par;

                //par = new MySqlParameter("?p_totalRecords", MySqlDbType.Int32);
                //par.Direction = ParameterDirection.Output;
                //param[8] = par;

                strSql = "prc_query";
            }
            using (var dbConnection = dbcontext.Database.Connection)
            {

               var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.StoredProcedure, strSql,true, param);
                list = ConvertExtension.IDataReaderToList<T>(IDataReader);
            }
            if (dataType == DatabaseType.Oracle)
            {
                pagination.records = int.Parse(param[8].Value.ToString());
            }
            if (dataType == DatabaseType.SqlServer) 
            {
                pagination.records = int.Parse(param[7].Value.ToString());
            }
            if (dataType == DatabaseType.MySql)
            {
                pagination.records = int.Parse(param[8].Value.ToString());
            }
            return list;
        }

        /// <summary>
        /// 存储过程执行分页（目前支持oracle,sqlserver和mysql）
        /// </summary>
        /// <param name="p_kid">表主键</param>
        /// <param name="p_fields">需要返回的列</param>
        /// <param name="p_tablename">表名</param>
        /// <param name="p_strwhere">查询条件</param>
        /// <param name="p_ordercolumn">排序字段</param>
        /// <param name="p_orderstyle">排序方式</param>
        /// <param name="p_curpage">当前页码</param>
        /// <param name="p_pagesize">每页记录数</param>
        /// <param name="p_totalrecords">总记录数（输出参数）</param>
        /// <param name="p_totalpages">总页数（输出参数）</param>
        /// <param name="dataType">数据库类型</param>
        /// <returns></returns>
        public DataTable FindTableByProcPager(Pagination pagination, DatabaseType dataType)
        {
            
            DataTable dt=new DataTable();
            DbParameter[] param = null;

            string strSql = string.Empty;

            if (dataType == DatabaseType.Oracle)
            {
                param = new OracleParameter[11];

                param[0] = new OracleParameter("p_kid", OracleDbType.Varchar2, 50, pagination.p_kid, ParameterDirection.Input);
                param[1] = new OracleParameter("p_fields", OracleDbType.Varchar2, 4000, pagination.p_fields, ParameterDirection.Input);
                param[2] = new OracleParameter("p_tableName", OracleDbType.Varchar2, 4000, pagination.p_tablename, ParameterDirection.Input);
                param[3] = new OracleParameter("p_strWhere", OracleDbType.Varchar2, 4000, pagination.conditionJson, ParameterDirection.Input);
                param[4] = new OracleParameter("p_orderColumn", OracleDbType.Varchar2, 4000, pagination.sidx, ParameterDirection.Input);
                param[5] = new OracleParameter("p_orderStyle", OracleDbType.Varchar2, 50, pagination.sord, ParameterDirection.Input);
                param[6] = new OracleParameter("p_curPage", OracleDbType.Int32, pagination.page, ParameterDirection.InputOutput);
                param[7] = new OracleParameter("p_pageSize", OracleDbType.Int32, pagination.rows, ParameterDirection.InputOutput);
                param[8] = new OracleParameter("p_totalRecords", OracleDbType.Int32, 0, ParameterDirection.Output);
                param[9] = new OracleParameter("p_totalPages", OracleDbType.Int32, 0, ParameterDirection.Output);
                param[10] = new OracleParameter("v_cur", OracleDbType.RefCursor, ParameterDirection.Output);

                strSql = "prc_query";
            }
            if (dataType == DatabaseType.SqlServer)
            {
                param = new SqlParameter[8];

                param[0] = new SqlParameter("@pageIndex", SqlDbType.Int); //当前页
                param[0].Value = pagination.page;
                param[1] = new SqlParameter("@pageSize", SqlDbType.Int); //每页记录数
                param[1].Value = pagination.rows;
                param[2] = new SqlParameter("@pkid", SqlDbType.VarChar, 64); //主键
                param[2].Value = pagination.p_kid;
                param[3] = new SqlParameter("@fieldName", SqlDbType.VarChar, 1024); //查询字段
                param[3].Value = pagination.p_fields;
                param[4] = new SqlParameter("@tableName", SqlDbType.VarChar, 1024); //表名
                param[4].Value = pagination.p_tablename;
                param[5] = new SqlParameter("@whereStr", SqlDbType.VarChar, 2000); //条件
                param[5].Value = pagination.conditionJson;
                param[6] = new SqlParameter("@orderStr", SqlDbType.VarChar, 64); //排序条件
                param[6].Value = pagination.sidx + " " + pagination.sord;
                param[7] = new SqlParameter("@recordTotal", SqlDbType.Int); //输出记录总数
                param[7].Value = 0;
                param[7].Direction = ParameterDirection.InputOutput;

                strSql = "prc_query";
            }
            if (dataType == DatabaseType.MySql)
            {
                //param = new MySqlParameter[9];
                //MySqlParameter par = new MySqlParameter("?p_kid", MySqlDbType.VarChar, 16);
                //par.Value = pagination.p_kid; par.Direction = ParameterDirection.Input;
                //param[0] = par;

                //par = new MySqlParameter("?p_fields", MySqlDbType.VarChar, 1024);
                //par.Value = pagination.p_fields; par.Direction = ParameterDirection.Input;
                //param[1] = par;

                //par = new MySqlParameter("?p_tableName", MySqlDbType.VarChar, 1024);
                //par.Value = pagination.p_tablename; par.Direction = ParameterDirection.Input;
                //param[2] = par;

                //par = new MySqlParameter("?p_strWhere", MySqlDbType.VarChar, 1024);
                //par.Value = pagination.conditionJson; par.Direction = ParameterDirection.Input;
                //param[3] = par;

                //par = new MySqlParameter("?p_orderColumn", MySqlDbType.VarChar, 128);
                //par.Value = pagination.sidx; par.Direction = ParameterDirection.Input;
                //param[4] = par;

                //par = new MySqlParameter("?p_orderStyle", MySqlDbType.VarChar, 4);
                //par.Value = pagination.sord; par.Direction = ParameterDirection.Input;
                //param[5] = par;

                //par = new MySqlParameter("?p_curPage", MySqlDbType.Int32);
                //par.Value = pagination.page; par.Direction = ParameterDirection.Input;
                //param[6] = par;

                //par = new MySqlParameter("?p_pageSize", MySqlDbType.Int32);
                //par.Value = pagination.rows; par.Direction = ParameterDirection.Input;
                //param[7] = par;

                //par = new MySqlParameter("?p_totalRecords", MySqlDbType.Int32);
                //par.Direction = ParameterDirection.Output;
                //param[8] = par;

                strSql = "prc_query";
            }
            using (var dbConnection = dbcontext.Database.Connection)
            {

                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.StoredProcedure, strSql, true, param);
                dt = ConvertExtension.IDataReaderToDataTable(IDataReader);
            }
            if (dataType == DatabaseType.Oracle)
            {
                pagination.records = int.Parse(param[8].Value.ToString());
            }
            if (dataType == DatabaseType.SqlServer)
            {
                pagination.records = int.Parse(param[7].Value.ToString());
            }
            if (dataType == DatabaseType.MySql)
            {
                pagination.records = int.Parse(param[8].Value.ToString());
            }
            return dt;
        }
        #endregion

        #region 数据源查询
        public DataTable FindTable(string strSql)
        {
            return FindTable(strSql, null);
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter)
        {
            using (var dbConnection = dbcontext.Database.Connection)
            {
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, strSql, dbParameter);
                return ConvertExtension.IDataReaderToDataTable(IDataReader);
            }
        }
        public DataTable FindTable(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            return FindTable(strSql, null, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            using (var dbConnection = dbcontext.Database.Connection)
            {
                StringBuilder sb = new StringBuilder();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                string OrderBy = "";
                strSql = DbParameters.FormatSql(strSql);
                if (!string.IsNullOrEmpty(orderField))
                {
                    if (orderField.ToUpper().IndexOf("ASC") + orderField.ToUpper().IndexOf("DESC") > 0)
                    {
                        OrderBy = "Order By " + orderField;
                    }
                    else
                    {
                        OrderBy = "Order By " + orderField + " " + (isAsc ? "ASC" : "DESC");
                    }
                }
                //else
                //{
                //    OrderBy = "order by (select 0)";
                //}
                DbParameter[] Params = new DbParameter[] { };
                Params = DbParameters.ToDbParameter(dbParameter);
                var dbtype = DbHelper.DbType;
                if (dbtype == DatabaseType.SqlServer)
                {
                    sb.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
                    sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                }
                if (dbtype == DatabaseType.Oracle)
                {
                    
                    sb.Append("SELECT  * FROM (SELECT A.*, rownum r FROM (");
                    sb.Append(strSql + " " + OrderBy + ") A Where rownum<= " + num1 + " ) B where r>= " + num);
                }
                if (dbtype == DatabaseType.MySql)
                {
                    
                    sb.Append(strSql + " " + OrderBy + "  limit " + num + "," + pageSize);
                }
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ") t", Params));

                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, sb.ToString(), Params);
                DataTable resultTable = ConvertExtension.IDataReaderToDataTable(IDataReader);
                //resultTable.Columns.Remove("rowNum");
                return resultTable;
            }
        }
        public object FindObject(string strSql)
        {
            return FindObject(strSql, null);
        }
        public object FindObject(string strSql, DbParameter[] dbParameter)
        {
            using (var dbConnection = dbcontext.Database.Connection)
            {
                    return new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, strSql, dbParameter);
            }
        }
        #endregion
    }
}
