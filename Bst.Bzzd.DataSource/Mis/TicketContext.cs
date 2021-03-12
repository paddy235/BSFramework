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
    public class TicketContext
    {
        public DataTable GetUnits()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = @"
                                select distinct
                                    a.jz
                                from
                                    (
                                        select
                                            jz
                                        from
                                            maximo.v_wxgzpdqdyz
                                        union all
                                        select
                                            jz
                                        from
                                            maximo.v_wxgzpdqdez
                                        union all
                                        select
                                            jzxx as jz
                                        from
                                            maximo.v_wxrljx
                                        union all
                                        select
                                            jzxx as jz
                                        from
                                            maximo.v_wxrggzp
                                    ) a
                                order by
                                    a.jz";
                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }

        public DataTable HyGetUnits()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = @"
                                select distinct ( case jz
                          when '00' then '公用系统'
                           when '01' then '#1机组'
                            when '02' then '#2机组'
                          else jz end ) as jz from ( select distinct
                                     unit_code as jz
                                from
                                  wo_wt_view where unit_code is not null

                                order by
                                   unit_code ) a  ";

                var command = new OracleCommand(query, connection);
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
                var query = "";

                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }

        public DataTable HyGetCategories()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = @" select distinct
                                    WT_TYPE_NAME as lx
                                from
                                  wo_wt_view where WT_CODE is not null
                                order by
                                   WT_TYPE_NAME";

                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }
        public DataTable GetList(string deptname, string[] units, string dutyperson, string category, string status, bool includecode, string keyword, int pagesize, int pageindex, out int total)
        {
            var queryString = @"
select
    a.*
from
    (
        select
            'gzp01' || to_char(wxgzpdqdyzid) as gzpid, gzpbh, jz,gzdd, pergzfzr, pergzfzrx, xkgzdate, gznr, perxkgzxkr, pzjsdate, yqdate
            , gzpstatus，bz, cy, perqfr, '电气第一种工作票' as category
        from
            maximo.v_wxgzpdqdyz
        union all
        select
            'gzp02' || to_char(wxgzpdqdezid) as gzpid, gzpbh, jz,gzdd, pergzfzr, null as pergzfzrx, xkgzdate, gznr, perxkgzxkr, gzzjdate
            , yqdate, gzpstatus, bz, cy, perqfr, '电气第二种工作票' as category
        from
            maximo.v_wxgzpdqdez
        union all
        select
            'gzp03' || to_char(wxrljxid) as gzpid, lcbh, jzxx as jz,gzdd, pergzfzr, null as pergzfzrx, xkgzksdate, gznr, perxkgzxkr, pzjsdate
            , gzpyqdate, gzpstatus, bz, cjry, pergzpqfr, '热力机械工作票' as category
        from
            maximo.v_wxrljx
        union all
        select
            'gzp04' || to_char(wxrggzpid) as gzpid, lcbh, jzxx as jz,gzdd, pergzfzr, perbgfzr, xkdate, gznr, perxkxkr, pzjsdate, yqdate
            , gzpstatus, bz, gzbzry, pergzpqfr, '热工工作票' as category
        from
            maximo.v_wxrggzp
    ) a
where
    a.gzpstatus <> '已终结'";
            var countString = @"
select
    count(1)
from
    (
        select
            'gzp01' || to_char(wxgzpdqdyzid) as gzpid, gzpbh, jz, pergzfzr,gzdd, pergzfzrx, xkgzdate, gznr, perxkgzxkr, pzjsdate, yqdate
            , gzpstatus，bz, cy, perqfr, '电气第一种工作票' as category
        from
            maximo.v_wxgzpdqdyz
        union all
        select
            'gzp02' || to_char(wxgzpdqdezid) as gzpid, gzpbh, jz,gzdd, pergzfzr, null as pergzfzrx, xkgzdate, gznr, perxkgzxkr, gzzjdate
            , yqdate, gzpstatus, bz, cy, perqfr, '电气第二种工作票' as category
        from
            maximo.v_wxgzpdqdez
        union all
        select
            'gzp03' || to_char(wxrljxid) as gzpid, lcbh, jzxx as jz, pergzfzr,gzdd, null as pergzfzrx, xkgzksdate, gznr, perxkgzxkr, pzjsdate
            , gzpyqdate, gzpstatus, bz, cjry, pergzpqfr, '热力机械工作票' as category
        from
            maximo.v_wxrljx
        union all
        select
            'gzp04' || to_char(wxrggzpid) as gzpid, lcbh, jzxx as jz,gzdd, pergzfzr, perbgfzr, xkdate, gznr, perxkxkr, pzjsdate, yqdate
            , gzpstatus, bz, gzbzry, pergzpqfr, '热工工作票' as category
        from
            maximo.v_wxrggzp
    ) a
where
    a.gzpstatus <> '已终结'";

            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(deptname))
            {
                queryString += " and bz like :bz";
                countString += " and bz like :bz";
                parameters.Add(new OracleParameter("bz", "%" + deptname + "%"));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                queryString += " and (bz like :kw or pergzfzr like :kw or pergzfzrx like :kw or pergzfzrx like :kw or gzpbh like :kw or gzdd like :kw )";
                countString += " and (bz like :kw or pergzfzr like :kw or pergzfzrx like :kw or pergzfzrx like :kw or gzpbh like :kw or gzdd like :kw )";
                parameters.Add(new OracleParameter("kw", "%" + keyword + "%"));
            }
            if (units != null && units.Length > 0)
            {
                var p1 = new OracleParameter[units.Length];
                for (int i = 0; i < units.Length; i++)
                {
                    p1[i] = new OracleParameter("jz" + i, units[i]);
                }
                queryString += string.Format(" and jz in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                countString += string.Format(" and jz in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                parameters.AddRange(p1);
            }

            if (!string.IsNullOrEmpty(dutyperson))
            {
                queryString += " and (pergzfzr = :pergzfzr or pergzfzrx = :pergzfzrx)";
                countString += " and (pergzfzr = :pergzfzr or pergzfzrx = :pergzfzrx)";
                parameters.Add(new OracleParameter("pergzfzr", dutyperson));
                parameters.Add(new OracleParameter("pergzfzrx", dutyperson));
            }

            if (!string.IsNullOrEmpty(category))
            {
                queryString += " and category = :category";
                countString += " and category = :category";
                parameters.Add(new OracleParameter("category", category));
            }

            if (!string.IsNullOrEmpty(status))
            {
                queryString += " and gzpstatus = :gzpstatus";
                countString += " and gzpstatus = :gzpstatus";
                parameters.Add(new OracleParameter("gzpstatus", status));
            }

            if (includecode)
            {
                queryString += " and gzpbh is not null";
                countString += " and gzpbh is not null";
            }

            queryString += " order by pzjsdate desc";

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

        public DataTable HyGetList(string deptname, string[] units, string dutyperson, string category, string status, bool includecode, int pagesize, int pageindex, out int total)
        {


            var queryString = @"
                        select *  from（
                        select
                              distinct （ID）  as gzpid,
                            WT_CODE as gzpbh,
                            unit_code as jz,
                            WT_TYPE_NAME  as category,
                            WORK_LEADER_NAME as pergzfzr,
                            NOW_WORK_LEADER_NAME as pergzfzrx,
                            permit_Start_Time  as xkgzdate,
                            CONTENT  as gznr,
                            PERMIT_BY_NAME as perxkgzxkr,
                            WT_SIGNER_NAME  as perqfr,
                            act_End_Time     as pzjsdate,
                            DELAY_TIME  as yqdate,
                            STATUS_NAME as gzpstatus,
                            MAINT_ORG_NAME as bz,
                            WORK_CLASS_PERSON as cy,
                            create_place as gzdd
                        from
                           wo_wt_view where WT_CODE is not null and main_ticket_id is null ）a
                        where
                           gzpstatus not like '%终结%'";
            var countString = @"
                              select count(1) from（
                        select
                            distinct （ID）  as gzpid,
                            WT_CODE as gzpbh,
                            unit_code as jz,
                            WT_TYPE_NAME  as category,
                            WORK_LEADER_NAME as pergzfzr,
                            NOW_WORK_LEADER_NAME as pergzfzrx,
                            permit_Start_Time  as xkgzdate,
                            CONTENT  as gznr,
                            PERMIT_BY_NAME as perxkgzxkr,
                            WT_SIGNER_NAME  as perqfr,
                            act_End_Time     as pzjsdate,
                            DELAY_TIME  as yqdate,
                            STATUS_NAME as gzpstatus,
                            MAINT_ORG_NAME as bz,
                            WORK_CLASS_PERSON as cy,
                            create_place as gzdd
                        from
                           wo_wt_view  where WT_CODE is not null and main_ticket_id is null ）a
                        where
                          gzpstatus not like '%终结%'";

            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(deptname))
            {
                queryString += " and bz = :bz";
                countString += " and bz = :bz";
                parameters.Add(new OracleParameter("bz", deptname));
            }

            if (units != null && units.Length > 0)
            {
                var p1 = new OracleParameter[units.Length];
                for (int i = 0; i < units.Length; i++)
                {
                    var unitsCode = "";
                    switch (units[i])
                    {
                        case "#1机组":
                            unitsCode = "01"; break;
                        case "#2机组":
                            unitsCode = "02"; break;
                        case "公用系统":
                            unitsCode = "00"; break;
                        default:
                            unitsCode = units[i];
                            break;
                    }
                    p1[i] = new OracleParameter("jz" + i, unitsCode);
                }
                queryString += string.Format(" and jz in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                countString += string.Format(" and jz in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));
                parameters.AddRange(p1);
            }

            if (!string.IsNullOrEmpty(dutyperson))
            {
                queryString += " and (pergzfzr = :pergzfzr or pergzfzrx = :pergzfzrx)";
                countString += " and (pergzfzr = :pergzfzr or pergzfzrx = :pergzfzrx)";
                parameters.Add(new OracleParameter("pergzfzr", dutyperson));
                parameters.Add(new OracleParameter("pergzfzrx", dutyperson));
            }

            if (!string.IsNullOrEmpty(category))
            {
                queryString += " and category = :category";
                countString += " and category = :category";
                parameters.Add(new OracleParameter("category", category));
            }

            if (!string.IsNullOrEmpty(status))
            {
                queryString += " and gzpstatus = :gzpstatus";
                countString += " and gzpstatus = :gzpstatus";
                parameters.Add(new OracleParameter("gzpstatus", status));
            }

            if (includecode)
            {
                queryString += " and gzpbh is not null";
                countString += " and gzpbh is not null";
            }

            queryString += " order by pzjsdate desc";

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand(string.Format("select * from (select a.*, ROW_NUMBER() over(order by a.gzpbh) as  rn from ({0}) a where rownum <= {2}) where rn > {1}", queryString, pagesize * (pageindex - 1), pagesize * pageindex), connection);
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
        public DataTable GetStatistical(string deptname, string[] units, DateTime Start, DateTime End, string TeamType)
        {
            var queryString = @"
select
    a.*
from
    (
        select
            'gzp01' || to_char(wxgzpdqdyzid) as gzpid, gzpbh, jz, pergzfzr, pergzfzrx, xkgzdate, gznr, perxkgzxkr, pzjsdate, yqdate
            , gzpstatus，bz, cy, perqfr, '电气第一种工作票' as category
        from
            maximo.v_wxgzpdqdyz
        union all
        select
            'gzp02' || to_char(wxgzpdqdezid) as gzpid, gzpbh, jz, pergzfzr, null as pergzfzrx, xkgzdate, gznr, perxkgzxkr, gzzjdate
            , yqdate, gzpstatus, bz, cy, perqfr, '电气第二种工作票' as category
        from
            maximo.v_wxgzpdqdez
        union all
        select
            'gzp03' || to_char(wxrljxid) as gzpid, lcbh, jzxx as jz, pergzfzr, null as pergzfzrx, xkgzksdate, gznr, perxkgzxkr, pzjsdate
            , gzpyqdate, gzpstatus, bz, cjry, pergzpqfr, '势力机械工作票' as category
        from
            maximo.v_wxrljx
        union all
        select
            'gzp04' || to_char(wxrggzpid) as gzpid, lcbh, jzxx as jz, pergzfzr, perbgfzr, xkdate, gznr, perxkxkr, pzjsdate, yqdate
            , gzpstatus, bz, gzbzry, pergzpqfr, '热工工作票' as category
        from
            maximo.v_wxrggzp
    ) a
where
    a.gzpstatus = '已终结'";

            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(deptname) && (TeamType == "02" || TeamType == "03"))
            {
                queryString += " and bz like :bz";

                parameters.Add(new OracleParameter("bz", "%" + deptname + "%"));
            }


            if (units != null && units.Length > 0)
            {
                var p1 = new OracleParameter[units.Length];
                for (int i = 0; i < units.Length; i++)
                {
                    p1[i] = new OracleParameter("jz" + i, units[i]);
                }
                queryString += string.Format(" and jz in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));

                parameters.AddRange(p1);
            }
            queryString += string.Format(" and pzjsdate  between to_date('{0}', 'yyyy-mm-dd hh24:mi:ss') and to_date('{1}', 'yyyy-mm-dd hh24:mi:ss') ", Start.ToString(), End.ToString());

            //queryString += " and pzjsdate => :Start";
            //parameters.Add(new OracleParameter("Start", Start));
            //queryString += " and pzjsdate <= :end";
            //parameters.Add(new OracleParameter("End", End));

            queryString += " order by pzjsdate desc";

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand(queryString, connection);
                queryCommand.CommandType = CommandType.Text;
                queryCommand.Parameters.AddRange(parameters.ToArray());

                var reader = queryCommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                connection.Close();
            }

            return dt;
        }

        public DataTable HyGetStatistical(string deptname, string[] units, DateTime Start, DateTime End, string TeamType)
        {

            var queryString = @"
                        select *  from（
                        select
                            distinct （ID）  as gzpid,
                            WT_CODE as gzpbh,
                            unit_code as jz,
                            WT_TYPE_NAME  as category,
                            WORK_LEADER_NAME as pergzfzr,
                            NOW_WORK_LEADER_NAME as pergzfzrx,
                            permit_Start_Time  as xkgzdate,
                            CONTENT  as gznr,
                            PERMIT_BY_NAME as perxkgzxkr,
                            WT_SIGNER_NAME  as perqfr,
                            act_End_Time     as pzjsdate,
                            DELAY_TIME  as yqdate,
                            STATUS_NAME as gzpstatus,
                            MAINT_ORG_NAME as bz,
                            WORK_CLASS_PERSON as cy,
                            create_place as gzdd
                        from
                           wo_wt_view where WT_CODE is not null and main_ticket_id is null ）a
                        where
                           gzpstatus like '%终结%'";


            var parameters = new List<OracleParameter>();
            if (!string.IsNullOrEmpty(deptname) && (TeamType == "02" || TeamType == "03"))
            {
                queryString += " and bz = :bz";

                parameters.Add(new OracleParameter("bz", deptname));
            }


            if (units != null && units.Length > 0)
            {
                var p1 = new OracleParameter[units.Length];
                for (int i = 0; i < units.Length; i++)
                {
                    var unitsCode = "";
                    switch (units[i])
                    {
                        case "#1机组":
                            unitsCode = "01"; break;
                        case "#2机组":
                            unitsCode = "02"; break;
                        case "公用系统":
                            unitsCode = "00"; break;
                        default:
                            unitsCode = units[i];
                            break;
                    }
                    p1[i] = new OracleParameter("jz" + i, unitsCode);

                }
                queryString += string.Format(" and jz in ({0})", string.Join(",", p1.Select(x => ":" + x.ParameterName)));

                parameters.AddRange(p1);
            }
            queryString += string.Format(" and pzjsdate  between to_date('{0}', 'yyyy-mm-dd hh24:mi:ss') and to_date('{1}', 'yyyy-mm-dd hh24:mi:ss') ", Start.ToString(), End.ToString());

            //queryString += " and pzjsdate => :Start";
            //parameters.Add(new OracleParameter("Start", Start));
            //queryString += " and pzjsdate <= :end";
            //parameters.Add(new OracleParameter("End", End));

            queryString += " order by pzjsdate desc";

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var queryCommand = new OracleCommand(queryString, connection);
                queryCommand.CommandType = CommandType.Text;
                queryCommand.Parameters.AddRange(parameters.ToArray());

                var reader = queryCommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                connection.Close();
            }

            return dt;
        }
        /// <summary>
        /// 一级动火
        /// </summary>
        /// <param name="ticketcode"></param>
        /// <returns></returns>
        public int GetNum1(string ticketcode)
        {
            var result = 0;
            if (string.IsNullOrEmpty(ticketcode)) return result;

            var countString = "select count(1) from maximo.v_wxgzpyjdh where gzpbh = :gzpbh";
            var parameters = new List<OracleParameter>() { new OracleParameter("gzpgh", ticketcode) };

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                result = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 二级动火
        /// </summary>
        /// <param name="ticketcode"></param>
        /// <returns></returns>
        public int GetNum2(string ticketcode)
        {
            var result = 0;
            if (string.IsNullOrEmpty(ticketcode)) return result;

            var countString = "select count(1) from maximo.v_wxgzpejdh where gzpbh = :gzpbh";
            var parameters = new List<OracleParameter>() { new OracleParameter("gzpgh", ticketcode) };

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                result = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 风险作业审批单
        /// </summary>
        /// <param name="ticketcode"></param>
        /// <returns></returns>
        public int GetNum3(string ticketcode)
        {
            var result = 0;
            if (string.IsNullOrEmpty(ticketcode)) return result;

            var countString = "select count(1) from maximo.v_gzfspgl where gzpbh = :gzpbh";
            var parameters = new List<OracleParameter>() { new OracleParameter("gzpgh", ticketcode) };

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                result = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 热控保护措施票
        /// </summary>
        /// <param name="ticketcode"></param>
        /// <returns></returns>
        public int GetNum4(string ticketcode)
        {
            var result = 0;
            if (string.IsNullOrEmpty(ticketcode)) return result;

            var countString = "select count(1) from maximo.v_rkbhcsp where gzpbh = :gzpbh";
            var parameters = new List<OracleParameter>() { new OracleParameter("gzpgh", ticketcode) };

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                result = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 继电保护措施票
        /// </summary>
        /// <param name="ticketcode"></param>
        /// <returns></returns>
        public int GetNum5(string ticketcode)
        {
            var result = 0;
            if (string.IsNullOrEmpty(ticketcode)) return result;

            var countString = "select count(1) from maximo.v_jdbhaqcsp where bh = :gzpbh";
            var parameters = new List<OracleParameter>() { new OracleParameter("gzpgh", ticketcode) };

            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                var countCommand = new OracleCommand(countString, connection);
                countCommand.CommandType = CommandType.Text;
                countCommand.Parameters.AddRange(parameters.ToArray());
                result = int.Parse(countCommand.ExecuteScalar().ToString());

                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// 作业安全措施票
        /// </summary>
        /// <param name="ticketcode"></param>
        /// <returns></returns>
        public int GetNum6(string ticketcode)
        {
            var result = 1;
            //var countString = "select count(1) from jdbhaqcsp where bh = :gzpbh";
            //var parameters = new List<OracleParameter>() { new OracleParameter("gzpgh", ticketcode) };

            //var dt = new DataTable();
            //var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            //using (var connection = new OracleConnection(connectionString))
            //{
            //    connection.Open();

            //    var countCommand = new OracleCommand(countString, connection);
            //    countCommand.CommandType = CommandType.Text;
            //    countCommand.Parameters.AddRange(parameters.ToArray());
            //    result = int.Parse(countCommand.ExecuteScalar().ToString());

            //    connection.Close();
            //}

            return result;
        }
        /// <summary>
        /// 红雁获取数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> HYgetNum(string ticketid)
        {
            var typeNum = new Dictionary<string, int>();
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = @"select  type,count(ID) as num from (
                                 select     distinct （ID）,WT_TYPE_NAME as type from wo_wt_view where  WT_CODE is not null and main_ticket_id is null and ID = '" + ticketid + "'  )  GROUP BY  type ";
                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
                foreach (DataRow item in dt.Rows)
                {
                    typeNum.Add(item["type"].ToString(), Convert.ToInt32(item["num"].ToString()));
                }
            }
            return typeNum;
        }


        public DataTable GetDetail(string ticketid)
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();


            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = @"
                        select
                            a.*
                        from
                            (
                                select
                                    'gzp01' || to_char(wxgzpdqdyzid) as gzpid, gzpbh, jz, pergzfzr, pergzfzrx, xkgzdate, gznr, perxkgzxkr, pzjsdate, yqdate
                                    , gzpstatus，bz, cy, perqfr, '电气第一种工作票' as category,gzdd
                                from
                                    maximo.v_wxgzpdqdyz
                                union all
                                select
                                    'gzp02' || to_char(wxgzpdqdezid) as gzpid, gzpbh, jz, pergzfzr, null as pergzfzrx, xkgzdate, gznr, perxkgzxkr, gzzjdate
                                    , yqdate, gzpstatus, bz, cy, perqfr, '电气第二种工作票' as category,gzdd
                                from
                                    maximo.v_wxgzpdqdez
                                union all
                                select
                                    'gzp03' || to_char(wxrljxid) as gzpid, lcbh, jzxx as jz, pergzfzr, null as pergzfzrx, xkgzksdate, gznr, perxkgzxkr, pzjsdate
                                    , gzpyqdate, gzpstatus, bz, cjry, pergzpqfr, '势力机械工作票' as category,gzdd
                                from
                                    maximo.v_wxrljx
                                union all
                                select
                                    'gzp04' || to_char(wxrggzpid) as gzpid, lcbh, jzxx as jz, pergzfzr, perbgfzr, xkdate, gznr, perxkxkr, pzjsdate, yqdate
                                    , gzpstatus, bz, gzbzry, pergzpqfr, '热工工作票' as category,gzdd
                                from
                                    maximo.v_wxrggzp
                            ) a where gzpid = '" + ticketid + "'";


                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }
        public DataTable HyGetDetail(string ticketid)
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                var query = @"
                         select *  from（
                        select
                              distinct （ID）  as gzpid,
                            WT_CODE as gzpbh,
                            unit_code as jz,
                            WT_TYPE_NAME  as category,
                            WORK_LEADER_NAME as pergzfzr,
                            NOW_WORK_LEADER_NAME as pergzfzrx,
                            permit_Start_Time  as xkgzdate,
                            CONTENT  as gznr,
                            PERMIT_BY_NAME as perxkgzxkr,
                            WT_SIGNER_NAME  as perqfr,
                            act_End_Time     as pzjsdate,
                            DELAY_TIME  as yqdate,
                            STATUS_NAME as gzpstatus,
                            MAINT_ORG_NAME as bz,
                            WORK_CLASS_PERSON as cy,
                            create_place as gzdd
                        from
                           wo_wt_view  where WT_CODE is not null and main_ticket_id is null ）a
                            where gzpid = '" + ticketid + "'";

                var command = new OracleCommand(query, connection);
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


                var query = @"
                                select distinct
                                    a.gzpstatus
                                from
                                    (
                                        select
                                            gzpstatus
                                        from
                                            maximo.v_wxgzpdqdyz
                                        union all
                                        select
                                            gzpstatus
                                        from
                                            maximo.v_wxgzpdqdez
                                        union all
                                        select
                                            gzpstatus
                                        from
                                            maximo.v_wxrljx
                                        union all
                                        select
                                            gzpstatus
                                        from
                                            maximo.v_wxrggzp
                                    ) a
                                where
                                    a.gzpstatus <> '已终结'
                                order by
                                    a.gzpstatus";

                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }
        public DataTable HyGetStatus()
        {
            var dt = new DataTable();
            var connectionString = ConfigurationManager.ConnectionStrings["MisConnection"].ToString();
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = @"
                                select distinct  a.gzpstatus  from（
                                select 
                                    STATUS_NAME as gzpstatus
                                from
                                    wo_wt_view 
                                where STATUS_NAME not like '%终结%' and STATUS_NAME  is not null and WT_CODE is not null
                                    order by
                                    STATUS_NAME） a
                                    order by
                                    a.gzpstatus";

                var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(reader);
            }

            return dt;
        }
    }
}
