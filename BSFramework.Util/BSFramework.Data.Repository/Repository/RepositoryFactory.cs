using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
namespace BSFramework.Data.Repository
{
    /// <summary>
    /// 描 述：定义仓储模型工厂
    /// </summary>
    public class RepositoryFactory
    {
        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository BaseRepository(string connString)
        {
            return new Repository(DbFactory.Base(connString, DatabaseType.SqlServer));
        }
        /// <summary>
        /// 定义仓储（基础库）
        /// </summary>
        /// <returns></returns>
        public IRepository BaseRepository()
        {
            return new Repository(DbFactory.Base());
        }


        /// <summary>
        /// 定义仓储（采用Dapper）
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <param name="datatype">特定情况下的数据库类型</param>
        /// <returns></returns>
        public IRepository BaseRepository(string connString, DatabaseType datatype)
        {
            return new Repository(DbFactory.DapperBase(connString, datatype));
        }
        public IRepository BaseRepository(string connString, string datatype)
        {
            DatabaseType dbType = DatabaseType.MySql;
            if (datatype=="SqlServer")
            {
                dbType = DatabaseType.SqlServer;
            }
            if (datatype == "Oracle")
            {
                dbType = DatabaseType.Oracle;
            }
            return new Repository(DbFactory.DapperBase(connString, dbType));
        }
    }
}
