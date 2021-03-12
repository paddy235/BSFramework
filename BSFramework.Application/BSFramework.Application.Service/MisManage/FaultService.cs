using BSFramework.Application.IService.MisManage;
using Bst.Bzzd.DataSource.Mis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.MisManage;
using BSFramework.Data.Repository;

namespace BSFramework.Application.Service.MisManage
{
    public class FaultService : IFaultService
    {
        public List<string> GetCategories()
        {
            var db = new FaultContext();
            var dt = db.GetCategories();
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("QXFL"));
            }
            return result.OrderBy(x => x).ToList();
        }

        public FaultEntity GetDetail(decimal faultid)
        {
            var db = new FaultContext();
            var dt = db.GetDetail(faultid);
            if (dt == null && dt.Rows.Count == 0) return null;

            var result = new FaultEntity()
            {
                FaultId = dt.Rows[0].Field<decimal>("SBFAULTID"),
                FaultNum = dt.Rows[0].Field<string>("FAULTNUM"),
                Status = dt.Rows[0].Field<string>("FAULTSTATUS"),
                Unit = dt.Rows[0].Field<string>("SSJZ"),
                Specialty = dt.Rows[0].Field<string>("ZY"),
                FaultName = dt.Rows[0].Field<string>("QXMC"),
                DeviceName = dt.Rows[0].Field<string>("DESCRIPTION"),
                Category = dt.Rows[0].Field<string>("QXFL"),
                ResponsibleDepartment = dt.Rows[0].Field<string>("ZRBM"),
                PlanCompleteTime = dt.Rows[0].Field<DateTime?>("YQXQWCDATE"),
                ResolveGroup = dt.Rows[0].Field<string>("XQBM"),
                FoundPerson = dt.Rows[0].Field<string>("PERFXR"),
                FoundDepartment = dt.Rows[0].Field<string>("FXBM"),
                FoundTime = dt.Rows[0].Field<DateTime>("FXDATE"),
                Acceptor = dt.Rows[0].Field<string>("PERYSR"),
                AcceptanceDepartment = dt.Rows[0].Field<string>("YSBM"),
                AcceptanceTime = dt.Rows[0].Field<DateTime?>("YSDATE"),
                ResolveInTime = dt.Rows[0].Field<string>("XQJSX"),
                Qualified = dt.Rows[0].Field<string>("YSSFHG"),
                UndoReason = dt.Rows[0].Field<string>("UNTREATED")
            };

            return result;
        }

        public List<FaultEntity> GetFaults(string[] deptname, string[] units, string specialty, string[] categories, string status, int pagesize, int pageindex, out int total)
        {
            var db = new FaultContext();
            var dt = db.GetFaults(deptname, units, specialty, categories, status, pagesize, pageindex, out total);
            var result = new List<FaultEntity>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                var entity = new FaultEntity()
                {
                    FaultId = item.Field<decimal>("SBFAULTID"),
                    FaultNum = item.Field<string>("FAULTNUM"),
                    Unit = item.Field<string>("SSJZ"),
                    Specialty = item.Field<string>("ZY"),
                    Category = item.Field<string>("QXFL"),
                    FoundTime = item.Field<DateTime>("FXDATE"),
                    FaultName = item.Field<string>("QXMC"),
                    PlanCompleteTime = item.Field<DateTime?>("YQXQWCDATE"),
                    Status = item.Field<string>("FAULTSTATUS"),
                    ResolveGroup = item.Field<string>("XQBM"),
                    DeviceName = item.Field<string>("DESCRIPTION")
                };
                result.Add(entity);
            }

            var faultids = result.Select(x => x.FaultId).ToList();
            IRepository rep = new RepositoryFactory().BaseRepository();
            var query = from q in rep.IQueryable<FaultRelationEntity>()
                        where faultids.Contains(q.FaultId)
                        select q.FaultId;
            var allocated = query.ToList();
            result.ForEach(x => x.Allocated = allocated.Any(y => y == x.FaultId));

            return result;
        }

        public List<FaultEntity> GetFaultsByClass(string[] deptname, string[] units, string specialty, int pagesize, int pageindex, out int total)
        {
            var db = new FaultContext();
            var dt = db.GetFaultsByClass(deptname, units, specialty, pagesize, pageindex, out total);
            var result = new List<FaultEntity>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                var entity = new FaultEntity()
                {
                    FaultId = item.Field<decimal>("SBFAULTID"),
                    FaultNum = item.Field<string>("FAULTNUM"),
                    Unit = item.Field<string>("SSJZ"),
                    Specialty = item.Field<string>("ZY"),
                    Category = item.Field<string>("QXFL"),
                    FoundTime = item.Field<DateTime>("FXDATE"),
                    FaultName = item.Field<string>("QXMC"),
                    PlanCompleteTime = item.Field<DateTime?>("YQXQWCDATE"),
                    Status = item.Field<string>("FAULTSTATUS"),
                    ResolveGroup = item.Field<string>("XQBM"),
                    DeviceName = item.Field<string>("DESCRIPTION"),
                    RiskAnalysis= item.Field<string>("FXDFX"),
                    RiskAssessment = item.Field<string>("ASSESSMENT"),
                    Preventive = item.Field<string>("PREVENTIVE")
                };
                result.Add(entity);
            }

            var faultids = result.Select(x => x.FaultId).ToList();
            IRepository rep = new RepositoryFactory().BaseRepository();
            var query = from q in rep.IQueryable<FaultRelationEntity>()
                        where faultids.Contains(q.FaultId)
                        select q.FaultId;
            var allocated = query.ToList();
            result.ForEach(x => x.Allocated = allocated.Any(y => y == x.FaultId));

            return result;
        }
        public List<string> GetFaultsDept()
        {
            var db = new FaultContext();
            var list = db.GetFaultsDept();
            return list;
        }
        public List<FaultEntity> GetStatistical(string deptname, DateTime Start, DateTime End,string TeamType)
        {
            var db = new FaultContext();
            var dt = db.GetStatistical(deptname, Start, End, TeamType);
            var result = new List<FaultEntity>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                var entity = new FaultEntity()
                {
                    FaultId = item.Field<decimal>("SBFAULTID"),
                    FaultNum = item.Field<string>("FAULTNUM"),
                    Unit = item.Field<string>("SSJZ"),
                    Specialty = item.Field<string>("ZY"),
                    Category = item.Field<string>("QXFL"),
                    FoundTime = item.Field<DateTime>("FXDATE"),
                    FaultName = item.Field<string>("QXMC"),
                    PlanCompleteTime = item.Field<DateTime?>("YQXQWCDATE"),
                    Status = item.Field<string>("FAULTSTATUS"),
                    TimeStatus= item.Field<string>("XQJSX"),
                    ResolveGroup = item.Field<string>("XQBM"),
                    DeviceName = item.Field<string>("DESCRIPTION")
                };
                result.Add(entity);
            }

            //var faultids = result.Select(x => x.FaultId).ToList();
            //IRepository rep = new RepositoryFactory().BaseRepository();
            //var query = from q in rep.IQueryable<FaultRelationEntity>()
            //            where faultids.Contains(q.FaultId)
            //            select q.FaultId;
            //var allocated = query.ToList();
            //result.ForEach(x => x.Allocated = allocated.Any(y => y == x.FaultId));

            return result;
        }
        public List<string> GetSpecialties()
        {
            var db = new FaultContext();
            var dt = db.GetSpecialties();
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("ZY"));
            }
            return result.OrderBy(x => x).ToList();
        }

        public List<string> GetStatus()
        {
            var db = new FaultContext();
            var dt = db.GetStatus();
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("FAULTSTATUS"));
            }
            return result.OrderBy(x => x).ToList();
        }

        public List<string> GetUnits()
        {
            var db = new FaultContext();
            var dt = db.GetUnits();
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("SSJZ"));
            }
            return result.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// 分页查询班长值班交接日志
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每个个数</param>
        /// <param name="bZMC">班组，班号</param>
        /// <param name="fL">分类</param>
        /// <param name="kEYWORD">关键字</param>
        /// <param name="sTARTDATE">开始时间</param>
        /// <param name="eNDDATE">结束时间</param>
        /// <param name="totalCount">返回的总条数</param>
        /// <returns></returns>
        public DataTable GetJJRZ(int pageIndex, int pageSize, string bZMC, string fL, string kEYWORD, string sTARTDATE, string eNDDATE, ref int totalCount)
        {
            string where = " 1=1 ";
            if (!string.IsNullOrWhiteSpace(bZMC)) where += $" AND T.XT='{bZMC}'";
            if (!string.IsNullOrWhiteSpace(fL))
            {
                string[] flArry = fL.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string flSql = "'" + string.Join("','", flArry) + "'";
                where += $" AND T.FL IN ({flSql})";
            }
            if (!string.IsNullOrWhiteSpace(kEYWORD)) where += $" AND T.CZNR like '%{kEYWORD}%'";
            if (!string.IsNullOrWhiteSpace(sTARTDATE))
            {
                DateTime start;
                if (DateTime.TryParse(sTARTDATE, out start))
                {
                    where += $" AND T.FSDATE>=to_date('{start.ToString("yyyy-MM-dd")}','yyyy-MM-dd')";
                }
            }
            if (!string.IsNullOrWhiteSpace(eNDDATE))
            {
                DateTime end;
                if (DateTime.TryParse(eNDDATE, out end))
                {
                    end = end.AddDays(1).Date;
                    where += $" AND T.FSDATE<to_date('{end.ToString("yyyy-MM-dd")}','yyyy-MM-dd')";
                }
            }

            string countSql = $"SELECT COUNT(*) FROM ( select rownum as ROWNO,T.* from ( select * from V_YXJJBRZ order by FSDATE desc ) T where {where} ) tt";
            string selectSql = $"SELECT tt.*  FROM ( select rownum as ROWNO,T.* from ( select * from V_YXJJBRZ order by FSDATE desc ) T where {where} ) tt where tt.ROWNO>{(pageIndex - 1) * pageSize} and tt.ROWNO <{(pageSize * pageIndex) + 1}";
            var db = new FaultContext();
            object obj = db.ExcuteScalar(countSql);
            totalCount = Convert.ToInt32(obj);

            DataTable dt = db.ExcuteDataTable(selectSql);
            return dt;
        }
        /// <summary>
        /// 查询值班记录分类
        /// </summary>
        /// <returns></returns>
        public DataTable GetFL()
        {
            var db = new FaultContext();
            string sql = "select DISTINCT FL from MAXIMO.V_YXJJBRZ ";
            DataTable dt = db.ExcuteDataTable(sql);
            return dt;
        }
    }
}
