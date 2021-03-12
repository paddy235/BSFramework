using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BSFramework.Util.Ioc;
using System;
using BSFramework.Data.Dapper;

namespace BSFramework.Data.Repository
{
    /// <summary>
    /// 描 述：数据库建立工厂
    /// </summary>
    public class DbFactory
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <param name="DbType">数据库类型</param>
        /// <returns></returns>
        public static IDatabase Base(string connString, DatabaseType DbType)
        {
            DbHelper.DbType = DbType;
            return UnityIocHelper.DBInstance.GetService<IDatabase>(new ParameterOverride(
              "connString", connString), new ParameterOverride(
              "DbType", DbType.ToString()));
        }
        /// <summary>
        /// 连接基础库
        /// </summary>
        /// <returns></returns>
        public static IDatabase Base()
        {
            DbHelper.DbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), UnityIocHelper.GetmapToByName("DBcontainer", "IDbContext"));
            return UnityIocHelper.DBInstance.GetService<IDatabase>(new ParameterOverride(
             "connString", "BaseDb"), new ParameterOverride(
              "DbType", ""));
        }



        public static IDatabase Base(string connString, DatabaseType DbType, bool isIoc)
        {
            DbHelper.DbType = DbType;
            return UnityIocHelper.DBInstance.GetService<IDatabase>(new ParameterOverride(
              "connString", connString), new ParameterOverride(
              "DbType", ""));
        }
        /// <summary>
        /// 采用Dapper进行数据操作
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="DbType">数据库类型</param>
        /// <returns></returns>
        public static IDatabase DapperBase(string connString, DatabaseType DbType)
        {
            IDatabase db = null;
            if (DbType == DatabaseType.MySql)
            {
                db = new MySqlDatabase(connString);
            }
            if (DbType == DatabaseType.Oracle)
            {
                db = new BSFramework.Data.Dapper.OracleDatabase(connString);
            }
            if (DbType == DatabaseType.SqlServer)
            {
                db = new BSFramework.Data.Dapper.SqlDatabase(connString);
            }
            return db;
        }
    }
}
