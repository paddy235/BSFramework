using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace BSFramework.Data
{
    /// <summary>
    /// 描 述：SQL参数化
    /// </summary>
    public class DbParameters
    {
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来获取命令参数中的参数符号oracle为":",sqlserver为"@"
        /// </summary>
        /// <returns></returns>
        public static string CreateDbParmCharacter()
        {
            string character = string.Empty;
            switch (DbHelper.DbType)
            {
                case DatabaseType.SqlServer:
                    character = "@";
                    break;
                case DatabaseType.Oracle:
                    character = ":";
                    break;
                case DatabaseType.Dm:
                    character = ":";
                    break;
                case DatabaseType.MySql:
                    character = "?";
                    break;
                case DatabaseType.Access:
                    character = "@";
                    break;
                case DatabaseType.SQLite:
                    character = "@";
                    break;
                default:
                    throw new Exception("不支持当前数据库类型！");
            }
            return character;
        }
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter()
        {
            DbParameter param = null;
            Type type = null;
            switch (DbHelper.DbType)
            {
                case DatabaseType.SqlServer:
                    param = new SqlParameter();
                    break;
                case DatabaseType.Oracle:
                    param = new OracleParameter();

                    type = Type.GetType("Oracle.ManagedDataAccess.Client.OracleParameter, Oracle.ManagedDataAccess");
                    param = Activator.CreateInstance(type) as DbParameter;

                    break;
                case DatabaseType.MySql:
                    param = new MySqlParameter();

                    type = Type.GetType("MySql.Data.MySqlClient.MySqlParameter, MySql.Data");
                    param = Activator.CreateInstance(type) as DbParameter;

                    break;
                case DatabaseType.Access:
                    param = new OleDbParameter();
                    break;
                case DatabaseType.SQLite:
                    param = new SQLiteParameter();
                    break;
                case DatabaseType.Dm:
                    param = new Dm.DmParameter();
                    break;
                default:
                    throw new Exception("不支持当前数据库类型！");
            }
            return param;

        }
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value)
        {
            DbParameter param = DbParameters.CreateDbParameter();
            Regex reg = new Regex("^[\\?:@]{1}");
            paramName = reg.Replace(paramName, "");
            param.ParameterName = CreateDbParmCharacter() + paramName;
            param.Value = value;
            return param;
        }
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value, DbType dbType)
        {
            DbParameter param = DbParameters.CreateDbParameter();
            Regex reg = new Regex("^[\\?:@]{1}");
            paramName = reg.Replace(paramName, "");
            param.DbType = dbType;
            param.ParameterName = DbHelper.DbType == DatabaseType.Oracle ? paramName : CreateDbParmCharacter() + paramName;
            param.Value = value;
            return param;
        }
        /// <summary>
        /// 根据数据库类型转换sql语句参数格式
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public static string FormatSql(string strSql)
        {
            if (strSql.Contains("'@"))
            {
                return strSql;
            }
            else
            {
                Regex reg = new Regex("[\\?:@]{1}");
                return reg.Replace(strSql, CreateDbParmCharacter());
            }

        }
        /// <summary>
        /// 转换对应的数据库参数
        /// </summary>
        /// <param name="dbParameter">参数</param>
        /// <returns></returns>
        public static DbParameter[] ToDbParameter(DbParameter[] dbParameter)
        {
            if (dbParameter == null)
            {
                return null;
            }
            else
            {
                int i = 0;
                int size = dbParameter.Length;
                DbParameter[] _dbParameter = new DbParameter[dbParameter.Length];
                while (i < size)
                {
                    _dbParameter[i] = CreateDbParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                    i++;
                }
                return _dbParameter;
            }

        }
    }
}
