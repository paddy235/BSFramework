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
using System.Configuration;

namespace BSFramework.Application.Service.MisManage
{
    public class TicketService : ITicketService
    {
        //perqfr 第二许可人对不上  jz 没有 地点没有
        public TicketEntity GetDetail(string ticketid, string ticketStr)
        {
            var db = new TicketContext();
            DataTable dt;
            switch (ticketStr)
            {
                case "红雁池":
                    dt = db.HyGetDetail(ticketid);
                    break;
                default:
                    dt = db.GetDetail(ticketid);
                    break;
            }
            if (dt == null || dt.Rows.Count == 0) return null;
            var result = new TicketEntity()
            {
                TicketId = dt.Rows[0].Field<string>("gzpid"),
                TicketCode = dt.Rows[0].Field<string>("gzpbh"),
                Unit = dt.Rows[0].Field<string>("jz"),
                Category = dt.Rows[0].Field<string>("category"),
                DutyPerson = dt.Rows[0].Field<string>("pergzfzr"),
                DutyPerson2 = dt.Rows[0].Field<string>("pergzfzrx"),
                StartTime = dt.Rows[0].Field<DateTime?>("xkgzdate"),
                Content = dt.Rows[0].Field<string>("gznr"),
                ApprovePerson = dt.Rows[0].Field<string>("perxkgzxkr"),
                ApprovePerson2 = dt.Rows[0].Field<string>("perqfr"),
                EndTime = dt.Rows[0].Field<DateTime?>("pzjsdate"),
                TicketStatus = dt.Rows[0].Field<string>("gzpstatus"),
                EndTime2 = dt.Rows[0].Field<DateTime?>("yqdate"),
                DeptName = dt.Rows[0].Field<string>("bz"),
                WorkMate = dt.Rows[0].Field<string>("cy"),
                Place= dt.Rows[0].Field<string>("gzdd"),
                OtherTickets = new Dictionary<string, int>()
            };
            switch (ticketStr)
            {
                case "红雁池":
                    var OtherTickets = db.HYgetNum(ticketid);
                    result.OtherTickets.Add("一级动火证", OtherTickets.FirstOrDefault(x => x.Key == "一级动火工作票").Value);
                    result.OtherTickets.Add("二级动火证", OtherTickets.FirstOrDefault(x => x.Key == "二级动火工作票").Value);
                    result.OtherTickets.Add("风险作业审批单", OtherTickets.FirstOrDefault(x => x.Key == "").Value);
                    result.OtherTickets.Add("热控保护措施票", OtherTickets.FirstOrDefault(x => x.Key == "热控保护措施票").Value);
                    result.OtherTickets.Add("继电保护措施票", OtherTickets.FirstOrDefault(x => x.Key == "继电保护措施票").Value);
                    result.OtherTickets.Add("作业安全措施票", OtherTickets.FirstOrDefault(x => x.Key == "作业安全措施票").Value);
                    break;
                default:
                    result.OtherTickets.Add("一级动火证", db.GetNum1(result.TicketCode));
                    result.OtherTickets.Add("二级动火证", db.GetNum2(result.TicketCode));
                    result.OtherTickets.Add("风险作业审批单", db.GetNum3(result.TicketCode));
                    result.OtherTickets.Add("热控保护措施票", db.GetNum4(result.TicketCode));
                    result.OtherTickets.Add("继电保护措施票", db.GetNum5(result.TicketCode));
                    result.OtherTickets.Add("作业安全措施票", db.GetNum6(result.TicketCode));
                    break;
            }





            return result;
        }

        public List<TicketEntity> GetList(string deptname, string[] units, string dutyperson, string category, string status, bool includecode,string keyword, int pagesize, int pageindex, out int total, string ticketStr)
        {
            var db = new TicketContext();
            DataTable dt;
            switch (ticketStr)
            {
                case "红雁池":
                    dt = db.HyGetList(deptname, units, dutyperson, category, status, includecode, pagesize, pageindex, out total);
                    break;
                default:
                    dt = db.GetList(deptname, units, dutyperson, category, status, includecode, keyword, pagesize, pageindex, out total);
                    break;
            }
            var result = new List<TicketEntity>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                var entity = new TicketEntity()
                {
                    TicketId = item.Field<string>("gzpid"),
                    TicketCode = item.Field<string>("gzpbh"),
                    Unit = item.Field<string>("jz"),
                    Category = item.Field<string>("category"),
                    DutyPerson = item.Field<string>("pergzfzr"),
                    DutyPerson2 = item.Field<string>("pergzfzrx"),
                    StartTime = item.Field<DateTime?>("xkgzdate"),
                    Content = item.Field<string>("gznr"),
                    ApprovePerson = item.Field<string>("perxkgzxkr"),
                    EndTime = item.Field<DateTime?>("pzjsdate"),
                    EndTime2 = item.Field<DateTime?>("yqdate"),
                    TicketStatus = item.Field<string>("gzpstatus"),
                    DeptName = item.Field<string>("bz"),
                    WorkMate = item.Field<string>("cy"),
                    Place = item.Field<string>("gzdd")
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

        public List<TicketEntity> GetStatistical(string deptname, string[] units, DateTime Start, DateTime End, string TeamType, string ticketStr)
        {
            var db = new TicketContext();
            var result = new List<TicketEntity>();
            DataTable dt;
            switch (ticketStr)
            {
                case "红雁池":
                    dt = db.HyGetStatistical(deptname, units, Start, End, TeamType);
                    break;
                default:
                    dt = db.GetStatistical(deptname, units, Start, End, TeamType);
                    break;
            }

            if (dt == null && dt.Rows.Count == 0) return result;



            foreach (DataRow item in dt.Rows)
            {
                var entity = new TicketEntity()
                {
                    TicketId = item.Field<string>("gzpid"),
                    TicketCode = item.Field<string>("gzpbh"),
                    Unit = item.Field<string>("jz"),
                    Category = item.Field<string>("category"),
                    DutyPerson = item.Field<string>("pergzfzr"),
                    DutyPerson2 = item.Field<string>("pergzfzrx"),
                    StartTime = item.Field<DateTime?>("xkgzdate"),
                    Content = item.Field<string>("gznr"),
                    OtherTickets = new Dictionary<string, int>(),
                    ApprovePerson = item.Field<string>("perxkgzxkr"),
                    EndTime = item.Field<DateTime?>("pzjsdate"),
                    TicketStatus = item.Field<string>("gzpstatus")
                };
                switch (ticketStr)
                {
                    case "红雁池":
                        var OtherTickets = db.HYgetNum(entity.TicketId);
                        entity.OtherTickets.Add("一级动火证", OtherTickets.FirstOrDefault(x => x.Key == "一级动火工作票").Value);
                        entity.OtherTickets.Add("二级动火证", OtherTickets.FirstOrDefault(x => x.Key == "二级动火工作票").Value);
                        entity.OtherTickets.Add("风险作业审批单", OtherTickets.FirstOrDefault(x => x.Key == "").Value);
                        entity.OtherTickets.Add("热控保护措施票", OtherTickets.FirstOrDefault(x => x.Key == "热控保护措施票").Value);
                        entity.OtherTickets.Add("继电保护措施票", OtherTickets.FirstOrDefault(x => x.Key == "继电保护措施票").Value);
                        entity.OtherTickets.Add("作业安全措施票", OtherTickets.FirstOrDefault(x => x.Key == "作业安全措施票").Value);
                        break;
                    default:
                        entity.OtherTickets.Add("一级动火证", db.GetNum1(entity.TicketCode));
                        entity.OtherTickets.Add("二级动火证", db.GetNum2(entity.TicketCode));
                        entity.OtherTickets.Add("风险作业审批单", db.GetNum3(entity.TicketCode));
                        entity.OtherTickets.Add("热控保护措施票", db.GetNum4(entity.TicketCode));
                        entity.OtherTickets.Add("继电保护措施票", db.GetNum5(entity.TicketCode));
                        entity.OtherTickets.Add("作业安全措施票", db.GetNum6(entity.TicketCode));
                        break;
                }

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
        public List<string> GetStatus(string ticketStr)
        {
            var db = new TicketContext();
            DataTable dt;
            switch (ticketStr)
            {
                case "红雁池":
                    dt = db.HyGetStatus();
                    break;
                default:
                    dt = db.GetStatus();
                    break;
            }
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("GZPSTATUS"));
            }
            return result.OrderBy(x => x).ToList();
        }

        public List<string> GetUnits(string ticketStr)
        {
            var db = new TicketContext();
            DataTable dt;
            switch (ticketStr)
            {
                case "红雁池":
                    dt = db.HyGetUnits();
                    break;
                default:
                    dt = db.GetUnits();
                    break;
            }
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("JZ"));
            }
            return result.OrderBy(x => x).ToList();
        }
        public List<string> GetCategories(string ticketStr)
        {
            var db = new TicketContext();
            DataTable dt;
            switch (ticketStr)
            {
                case "红雁池":
                    dt = db.HyGetCategories();
                    break;
                default:
                    dt = db.GetCategories();
                    break;
            }
            var result = new List<string>();
            if (dt == null && dt.Rows.Count == 0) return result;

            foreach (DataRow item in dt.Rows)
            {
                result.Add(item.Field<string>("lx"));
            }
            return result.OrderBy(x => x).ToList();
        }
    }
}
