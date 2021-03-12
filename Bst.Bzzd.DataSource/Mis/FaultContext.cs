using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Bzzd.DataSource.Mis
{
    public class FaultContext
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
        public DataTable GetUnits()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand("select distinct ssjz from maximo.v_yzblszb where ssjz is not null", connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }

        public DataTable GetFaults(string[] deptname, string[] units, string specialty, string[] categories, string status, int pagesize, int pageindex, out int total)
        {
            var queryString = "select SBFAULTID,FAULTNUM,SSJZ,ZY,QXFL,FXDATE,QXMC,YQXQWCDATE,FAULTSTATUS,XQBM,DESCRIPTION from maximo.v_yzblszb where 1=1 and STATUS <> 'ZJ'";
            var countString = "select count(1) from maximo.v_yzblszb where 1=1 and STATUS <> 'ZJ'";
            var parameters = new List<OracleParameter>();
            //if (!string.IsNullOrEmpty(deptname))
            //{
            //    queryString += " and ZRBM = :ZRBM";
            //    countString += " and ZRBM = :ZRBM";
            //    parameters.Add(new OracleParameter("ZRBM", deptname));
            //}
            if (deptname != null && deptname.Length > 0)
            {
                queryString += " and (";
                countString += " and (";
                for (int i = 0; i < deptname.Length; i++)
                {
                    if (i == deptname.Length - 1)
                    {
                        queryString += string.Format(" ZRBM = '{0}' ", deptname[i]);
                        countString += string.Format(" ZRBM = '{0}' ", deptname[i]);
                    }
                    else
                    {
                        queryString += string.Format(" ZRBM = '{0}' or ", deptname[i]);
                        countString += string.Format(" ZRBM = '{0}' or ", deptname[i]);
                    }

                }
                queryString += " ) ";
                countString += " ) ";


            }
            if (units != null && units.Length > 0)
            {
                var p1 = new OracleParameter[units.Length];
                for (int i = 0; i < units.Length; i++)
                {
                    p1[i] = new OracleParameter("SSJZ" + i, units[i]);
                }
                queryString += string.Format(" and SSJZ in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                countString += string.Format(" and SSJZ in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                parameters.AddRange(p1);
            }

            if (!string.IsNullOrEmpty(specialty))
            {
                queryString += " and ZY = :ZY";
                countString += " and ZY = :ZY";
                parameters.Add(new OracleParameter("ZY", specialty));
            }

            if (categories != null && categories.Length > 0)
            {
                var p2 = new OracleParameter[categories.Length];
                for (int i = 0; i < categories.Length; i++)
                {
                    p2[i] = new OracleParameter("QXFL" + i, categories[i]);
                }
                queryString += string.Format(" and QXFL in ({0})", string.Join(",", p2.Select(x => ":" + x.ParameterName)));
                countString += string.Format(" and QXFL in ({0})", string.Join(",", p2.Select(x => ":" + x.ParameterName)));
                parameters.AddRange(p2);
            }

            if (!string.IsNullOrEmpty(status))
            {
                queryString += " and FAULTSTATUS = :FAULTSTATUS";
                countString += " and FAULTSTATUS = :FAULTSTATUS";
                parameters.Add(new OracleParameter("FAULTSTATUS", status));
            }

            queryString += " order by FXDATE desc";

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand(string.Format("select * from (select a.*, rownum as rn from ({0}) a where rownum <= {2}) where rn > {1}", queryString, pagesize * (pageindex - 1), pagesize * pageindex), connection);
                queryCommand.CommandType = CommandType.Text;
                queryCommand.Parameters.AddRange(parameters.ToArray());

                var reader = queryCommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                total = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return dt;
        }
        public DataTable GetFaultsByClass(string[] deptname, string[] units, string specialty, int pagesize, int pageindex, out int total)
        {
            var queryString = "select SBFAULTID,FAULTNUM,SSJZ,ZY,QXFL,FXDATE,QXMC,YQXQWCDATE,FAULTSTATUS,XQBM,DESCRIPTION,FXDFX,ASSESSMENT,PREVENTIVE from maximo.v_yzblszb where 1=1 and STATUS <> 'ZJ'";
            var countString = "select count(1) from maximo.v_yzblszb where 1=1 and STATUS <> 'ZJ'";
            var parameters = new List<OracleParameter>();
            //if (!string.IsNullOrEmpty(deptname))
            //{
            //    queryString += " and ZRBM = :ZRBM";
            //    countString += " and ZRBM = :ZRBM";
            //    parameters.Add(new OracleParameter("ZRBM", deptname));
            //}

            if (units != null && units.Length > 0)
            {
                var p1 = new OracleParameter[units.Length];
                for (int i = 0; i < units.Length; i++)
                {
                    p1[i] = new OracleParameter("SSJZ" + i, units[i]);
                }
                queryString += string.Format(" and SSJZ in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                countString += string.Format(" and SSJZ in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                parameters.AddRange(p1);
            }

            if (!string.IsNullOrEmpty(specialty))
            {
                queryString += " and ZY = :ZY";
                countString += " and ZY = :ZY";
                parameters.Add(new OracleParameter("ZY", specialty));
            }
            if (deptname != null && deptname.Length > 0)
            {
                queryString += " and (";
                countString += " and (";
                for (int i = 0; i < deptname.Length; i++)
                {
                    if (i == deptname.Length - 1)
                    {
                        queryString += string.Format(" ZRBM = '{0}'  ", deptname[i]);
                        countString += string.Format(" ZRBM = '{0}'  ", deptname[i]);
                    }
                    else
                    {
                        queryString += string.Format(" ZRBM = '{0}' or ", deptname[i]);
                        countString += string.Format(" ZRBM = '{0}' or ", deptname[i]);
                    }

                }
                queryString += " ) ";
                countString += " ) ";


            }

            //if (categories != null && categories.Length > 0)
            //{
            //    var p2 = new OracleParameter[categories.Length];
            //    for (int i = 0; i < categories.Length; i++)
            //    {
            //        p2[i] = new OracleParameter("QXFL" + i, categories[i]);
            //    }
            //    queryString += string.Format(" and QXFL in ({0})", string.Join(",", p2.Select(x => ":" + x.ParameterName)));
            //    countString += string.Format(" and QXFL in ({0})", string.Join(",", p2.Select(x => ":" + x.ParameterName)));
            //    parameters.AddRange(p2);
            //}
            queryString += " and (QXFL like '%一类%' or QXFL like '%二类%')  ";
            countString += " and (QXFL like '%一类%' or QXFL like '%二类%')  ";
            queryString += " and FAULTSTATUS = :FAULTSTATUS";
            countString += " and FAULTSTATUS = :FAULTSTATUS";
            parameters.Add(new OracleParameter("FAULTSTATUS", "未消除"));


            queryString += " order by FXDATE desc";

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand(string.Format("select * from (select a.*, rownum as rn from ({0}) a where rownum <= {2}) where rn > {1}", queryString, pagesize * (pageindex - 1), pagesize * pageindex), connection);
                queryCommand.CommandType = CommandType.Text;
                queryCommand.Parameters.AddRange(parameters.ToArray());

                var reader = queryCommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                total = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return dt;
        }
        public List<string> GetFaultsDept()
        {

            List<string> dept = new List<string>();
            var parameters = new List<OracleParameter>();
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand("select ZRBM from maximo.v_yzblszb", connection);
                queryCommand.CommandType = CommandType.Text;
                queryCommand.Parameters.AddRange(parameters.ToArray());

                var reader = queryCommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                //var countCommand = new OracleCommand(countString, connection);
                //countCommand.CommandType = CommandType.Text;
                //countCommand.Parameters.AddRange(parameters.ToArray());
                //total = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }
            foreach (DataRow item in dt.Rows)
            {
                var deptname = item["ZRBM"].ToString();
                if (!dept.Contains(deptname))
                {
                    dept.Add(deptname);
                }

            }
            return dept;

        }


        public DataTable GetStatistical(string deptname, DateTime Start, DateTime End, string TeamType)
        {
            var queryString = "select SBFAULTID,FAULTNUM,SSJZ,ZY,QXFL,FXDATE,QXMC,XQJSX,YQXQWCDATE,FAULTSTATUS,XQBM,DESCRIPTION from maximo.v_yzblszb where 1=1 ";
            var parameters = new List<OracleParameter>();


            queryString += string.Format(" and FXDATE  between to_date('{0}', 'yyyy-mm-dd hh24:mi:ss') and to_date('{1}', 'yyyy-mm-dd hh24:mi:ss') ", Start.ToString(), End.ToString());
            // parameters.Add(new OracleParameter("Start", Start));
            //  parameters.Add(new OracleParameter("End", End));
            if (TeamType == "02" || TeamType == "03")
            {
                queryString += string.Format(" and XQBM like '%{0}%'", deptname);

            }
            if (TeamType == "01")
            {
                queryString += string.Format(" and FXBM like '%{0}%'", deptname);

            }

            queryString += " order by FXDATE desc";

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand(queryString, connection);
                queryCommand.CommandType = CommandType.Text;
                //queryCommand.Parameters.AddRange(parameters.ToArray());

                var reader = queryCommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();

                connection.Close();
            }

            return dt;
        }
        public DataTable GetDetail(decimal faultid)
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand("select SBFAULTID,FAULTNUM,FAULTSTATUS,SSJZ,ZY,QXMC,DESCRIPTION,QXFL,ZRBM,YQXQWCDATE,XQBM,PERFXR,FXBM,FXDATE,PERYSR,YSBM,YSDATE,XQJSX,YSSFHG,UNTREATED from maximo.v_yzblszb where SBFAULTID = " + faultid, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }

        public DataTable GetSpecialties()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand("select distinct zy from maximo.v_yzblszb where zy is not null", connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }

        public DataTable GetCategories()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand("select distinct qxfl from maximo.v_yzblszb where qxfl is not null", connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }

        public DataTable GetStatus()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand("select distinct faultstatus from maximo.v_yzblszb where faultstatus is not null", connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }
        /// <summary>
        /// DataTable查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExcuteDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// 首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExcuteScalar(string sql)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    object reuslt = cmd.ExecuteScalarAsync().Result;
                    return reuslt;
                }
            }
        }
    }
}
