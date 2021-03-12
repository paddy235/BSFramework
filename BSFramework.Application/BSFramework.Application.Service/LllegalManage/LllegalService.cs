using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.IService.LllegalManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Data;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Data;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Application.Service.BaseManage;

namespace BSFramework.Application.Service.LllegalManage
{
    public class LllegalService : RepositoryFactory<LllegalEntity>, IlllegalService
    {



        public LllegalEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public LllegalEntity GetLllegalDetail(string JobId)
        {
            LllegalEntity lllegal = this.BaseRepository().FindEntity(JobId);
            lllegal.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == JobId).ToList();
            return lllegal;
        }
        /// <summary>
        /// 条件查询，统计
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userid"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public IEnumerable<LllegalEntity> GetListNoPage(string type, string userid, string deptid, string level, string flowstate, string sub, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.ApproveResult == "0" || x.ApproveResult == null);
                if (type == "0") //我登记的
                {
                    query = query.Where(x => x.RegisterPersonId == userid);
                }
                if (type == "1") //待核准的
                {
                    if (sub == "Team")
                    {
                        query = query.Where(x => x.LllegalTeamId.Contains(deptid) && x.Sub == "Team");
                    }
                    else
                    {
                        query = query.Where(x => x.Sub == sub);
                    }
                    query = query.Where(x => x.FlowState == "待核准");
                }
                if (type == "2")
                {
                    query = query.Where(x => x.ReformPeopleId == userid);
                    query = query.Where(x => x.FlowState == "待整改");
                }
                if (type == "3")
                {
                    query = query.Where(x => x.ApprovePersonId == userid);
                    query = query.Where(x => x.FlowState == "待验收");
                }
                if (type == "4")
                {
                    query = query.Where(x => x.FlowState == "验收通过");
                }
            }
            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(x => x.LllegalLevel == level);
            }
            if (!string.IsNullOrEmpty(flowstate))
            {
                query = query.Where(x => x.FlowState == flowstate);
            }
            total = query.Count();
            return query.OrderByDescending(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        public IEnumerable<LllegalEntity> GetListPage(string type, string userid, string deptid, string level, string flowstate, string sub)
        {
            var query = this.BaseRepository().IQueryable();
            if (sub == "Team")
            {
                query = query.Where(x => x.LllegalTeamId.Contains(deptid));
            }
            else
            {
                query = query.Where(x => x.Sub == "Company");
            }
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "0") //我登记的
                {
                    query = query.Where(x => x.RegisterPersonId == userid);
                }
                if (type == "1") //待核准的
                {

                    query = query.Where(x => x.FlowState == "待核准");
                }
                if (type == "2")
                {
                    query = query.Where(x => x.ReformPeopleId == userid);
                    query = query.Where(x => x.FlowState == "待整改");
                }
                if (type == "3")
                {
                    query = query.Where(x => x.ApprovePersonId == userid);
                    query = query.Where(x => x.FlowState == "待验收");
                }
                if (type == "4")
                {
                    query = query.Where(x => x.FlowState == "验收通过");
                }
            }
            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(x => x.LllegalLevel == level);
            }
            if (!string.IsNullOrEmpty(flowstate))
            {
                query = query.Where(x => x.FlowState == flowstate);
            }
            return query.OrderByDescending(x => x.LllegalTime).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="category"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IEnumerable<LllegalEntity> GetLllegalList(string from, string to, int page, int pagesize, string category, string userid, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            // query = query.Where(x => x.ApproveResult == "0");
            //所属违章班组 . --- 整改人所在的班组

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.LllegalTeamId == category || x.RegisterPersonId == userid);
            }
            if (!string.IsNullOrEmpty(from))
            {
                DateTime time1 = DateTime.Parse(from);
                query = query.Where(x => x.LllegalTime >= time1);
            }

            if (!string.IsNullOrEmpty(to))
            {
                DateTime time2 = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.LllegalTime <= time2);
            }
            total = query.Count();
            return query.OrderByDescending(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        public IEnumerable<LllegalEntity> GetLllegalList(string from, string to, int page, int pagesize, string category, string userid, string approveResult, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(category)) query = query.Where(x => x.LllegalTeamId == category || x.RegisterPersonId == userid);
            if (!string.IsNullOrEmpty(from))
            {
                DateTime time1 = DateTime.Parse(from);
                query = query.Where(x => x.LllegalTime >= time1);
            }

            if (!string.IsNullOrEmpty(to))
            {
                DateTime time2 = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.LllegalTime <= time2);
            }
            //违章核准 
            query = query.Where(x => x.ApproveResult == approveResult);
            query = query.Where(x => x.RefromState != "0");
            total = query.Count();
            return query.OrderByDescending(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        public IEnumerable<LllegalEntity> GetListNew(string deptid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(deptid)) query = query.Where(x => x.LllegalPersonId.Contains(deptid) && x.ApproveResult == "0");

            total = query.Count();
            return query.OrderByDescending(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        public DataTable getExport(string deptid, string from, string to)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<LllegalEntity>(x => x.ApproveResult == "0" && x.ReportDept == "班组");

            var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  LllegalNumber,LllegalType, LllegalLevel , RegisterPerson , LllegalDepart ,LllegalTeam ,LllegalPerson ,LllegalDescribe,APPROVEPERSON,APPROVEDATE
            //                from wg_lllegalregister where approveresult='0' and reportdept = '班组'");
            if (!string.IsNullOrEmpty(deptid))
            {
                //strSql.Append(" and LllegalTeamId like '%" + deptid + "%'");
                query = query.Where(x => x.LllegalTeamId.Contains(deptid));
            }
            if (!string.IsNullOrEmpty(from))
            {
                DateTime time1 = DateTime.Parse(from);
                //strSql.Append(" and LllegalTime >='" + time1 + "'");
                query = query.Where(x => x.LllegalTime >= time1);
            }
            if (!string.IsNullOrEmpty(to))
            {
                DateTime time2 = DateTime.Parse(from);
                //strSql.Append(" and LllegalTime <='" + time2 + "'");
                query = query.Where(x => x.LllegalTime <= time2);

            }
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());

            return dt;
        }

        public DataTable GetLegalsList(Pagination pagination)
        {

            DatabaseType dataType = DbHelper.DbType;
            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void AddLllegalRegister(LllegalEntity entity)
        {

            this.BaseRepository().Insert(entity);
        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, LllegalEntity entity)
        {
            var entity1 = this.GetLllegalDetail(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                this.BaseRepository().Insert(entity);
                new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                //entity1.LllegalAddress = entity.LllegalAddress;
                //entity1.LllegalDepart = entity.LllegalDepart;
                //entity1.LllegalDepartCode = entity.LllegalDepartCode;
                //entity1.LllegalDescribe = entity.LllegalDescribe;
                //entity1.LllegalLevel = entity.LllegalLevel;
                //entity1.LllegalNumber = entity.LllegalNumber;
                //entity1.LllegalPerson = entity.LllegalPerson;
                //entity1.LllegalPersonId = entity.LllegalPersonId;
                //entity1.LllegalTeam = entity.LllegalTeam;
                //entity1.LllegalTeamId = entity.LllegalTeamId;
                //entity1.LllegalTime = entity.LllegalTime;
                //entity1.LllegalType = entity.LllegalType;
                //entity1.RegisterPerson = entity.RegisterPerson;
                //entity1.RegisterPersonId = entity.RegisterPersonId;
                //entity1.Remark = entity.Remark;
                entity1.ApproveDate = entity.ApproveDate;
                entity1.ApprovePerson = entity.ApprovePerson;
                entity1.ApprovePersonId = entity.ApprovePersonId;
                entity1.ApproveReason = entity.ApproveReason;
                entity1.ApproveResult = entity.ApproveResult;
                entity1.FlowState = entity.FlowState;
                entity1.AssessMoney = entity.AssessMoney;
                entity1.IsAssess = entity.IsAssess;
                entity1.Sub = entity.Sub;
                entity1.ReformDate = entity.ReformDate;
                entity1.ReformPeople = entity.ReformPeople;
                entity1.ReformPeopleId = entity.ReformPeopleId;
                entity1.Files = null;
                entity1.CheckContent = entity.CheckContent;
                this.BaseRepository().Update(entity1);


            }
        }
        /// <summary>
        /// 违章整改使用--修改违章整改状态和关联整改Id
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateLllegal(LllegalEntity entity)
        {
            try
            {
                this.BaseRepository().Update(entity);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public IEnumerable<LllegalEntity> GetListMonthLllegal(string userid, DateTime from, DateTime to, string deptid)
        {
            //Operator user = OperatorProvider.Provider.Current();
            from = new DateTime(from.Year, from.Month, from.Day);
            to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
            var query = this.BaseRepository().IQueryable();
            query = query.Where(x => x.LllegalTeamId == deptid || x.RegisterPersonId == userid);
            query = query.Where(x => x.LllegalTime >= from && x.LllegalTime <= to && (x.ApproveResult == null || x.ApproveResult == "0")).OrderBy(x => new { x.ApproveResult, x.LllegalTime });
            return query.ToList();
        }

        public string GetListMonthLllegal(string deptid, string userid)
        {
            //Operator user = OperatorProvider.Provider.Current();
            var query = this.BaseRepository().IQueryable();
            query = query.Where(x => x.LllegalTeamId == deptid || x.RegisterPersonId == userid);
            query = this.BaseRepository().IQueryable().Where(x => x.ApproveResult == null).OrderByDescending(x => x.LllegalTime);
            if (query.Count() > 0)
            {
                return query.First().ID;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="filtertype"></param>
        /// <param name="filtervalue"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IEnumerable<LllegalEntity> GetList(string deptid, string filtertype, string filtervalue, DateTime? from, DateTime? to, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.ReportDept != "Company" && x.LllegalTeamId.Contains(deptid));
            if (from != null) query = query.Where(x => x.LllegalTime >= from);
            if (to != null) query = query.Where(x => x.LllegalTime <= to);

            switch (filtertype)
            {
                case "违章级别":
                    if (filtervalue != "全部")
                        query = query.Where(x => x.LllegalLevel == filtervalue);
                    break;
                case "违章类型":
                    if (filtervalue != "全部")
                        query = query.Where(x => x.LllegalType == filtervalue);
                    break;
                case "违章人员":
                    query = query.Where(x => x.LllegalPerson.Contains(filtervalue));
                    break;
                case "核准未通过违章":
                    query = query.Where(x => x.ApproveResult == "1");
                    break;
                default:
                    break;
            }

            if (filtertype != "核准未通过违章")
                query = query.Where(x => x.ApproveResult == "0");

            total = query.Count();
            var nlist = query.OrderByDescending(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            foreach (LllegalEntity l in nlist)
            {
                l.refrom = new LllegalRefromService().GetEntityByLllegalId(l.ID);
                l.accept = new LllegalAcceptService().GetEntityByLllegalId(l.ID);
            }
            return nlist;
        }

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="no"></param>
        /// <param name="person"></param>
        /// <param name="level"></param>
        /// <param name="category"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<LllegalEntity> GetData(string deptid, string deptcode, string no, string person, string level, string category, string state, int pagesize, int page, out int total)
        {
            //Operator user = OperatorProvider.Provider.Current();
            //var deptid = user.DeptId;
            DepartmentEntity dept = new DepartmentService().GetEntity(deptid);
            if (dept != null)
            {


                if (dept.FullName == "HSE部")
                {
                    deptcode = "0";
                }
            }
            else
            {
                deptcode = "0";
            }
            //var sql = "select * from base_department where encode like '" + deptcode + "%'";

            var db = new RepositoryFactory().BaseRepository();

            var list = db.IQueryable<DepartmentEntity>(x => x.EnCode.Contains(deptcode));

            var query = db.IQueryable<LllegalEntity>();
            if (state != "全部")
            {
                if (state == "核准不通过")
                {
                    query = query.Where(x => x.ApproveResult == "1");
                }
                else
                {
                    query = query.Where(x => x.FlowState == state);
                }
            }
            if (!string.IsNullOrEmpty(category) && category != "全部") query = query.Where(x => x.LllegalType == category);
            if (!string.IsNullOrEmpty(level) && level != "全部") query = query.Where(x => x.LllegalLevel == level);
            if (!string.IsNullOrEmpty(no)) query = query.Where(x => x.LllegalNumber.Contains(no));
            if (!string.IsNullOrEmpty(person)) query = query.Where(x => x.LllegalPerson.Contains(person));


            var deptids = list.Select(x => x.DepartmentId).ToList();
            query = query.Where(x => deptids.Contains(x.LllegalTeamId) || x.LllegalTeamId == null);

            total = query.Count();
            var data = query.Select(x => new { ID = x.ID, LllegalNumber = x.LllegalNumber, LllegalLevel = x.LllegalLevel, LllegalType = x.LllegalType, LllegalTime = x.LllegalTime, LllegalPerson = x.LllegalPerson, LllegalAddress = x.LllegalAddress, FlowState = x.FlowState }).OrderBy(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return data.Select(x => new LllegalEntity { ID = x.ID, LllegalNumber = x.LllegalNumber, LllegalLevel = x.LllegalLevel, LllegalType = x.LllegalType, LllegalTime = x.LllegalTime, LllegalPerson = x.LllegalPerson, LllegalAddress = x.LllegalAddress, FlowState = x.FlowState }).OrderBy(x => x.LllegalTime).ToList();
        }

        /// <summary>
        /// 待核准
        /// </summary>
        /// <param name="no"></param>
        /// <param name="person"></param>
        /// <param name="level"></param>
        /// <param name="category"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<LllegalEntity> GetApproving(string no, string person, string level, string category, int pagesize, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<LllegalEntity>().Where(x => x.Sub == "Company" && x.FlowState == "待核准");
            if (!string.IsNullOrEmpty(category) && category != "全部") query = query.Where(x => x.LllegalType == category);
            if (!string.IsNullOrEmpty(level) && level != "全部") query = query.Where(x => x.LllegalLevel == level);
            if (!string.IsNullOrEmpty(no)) query = query.Where(x => x.LllegalNumber.Contains(no));
            if (!string.IsNullOrEmpty(person)) query = query.Where(x => x.LllegalPerson.Contains(person));
            total = query.Count();
            var data = query.Select(x => new { ID = x.ID, LllegalNumber = x.LllegalNumber, LllegalLevel = x.LllegalLevel, LllegalType = x.LllegalType, LllegalTime = x.LllegalTime, LllegalPerson = x.LllegalPerson, LllegalAddress = x.LllegalAddress, FlowState = x.FlowState }).OrderBy(x => x.LllegalTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return data.Select(x => new LllegalEntity { ID = x.ID, LllegalNumber = x.LllegalNumber, LllegalLevel = x.LllegalLevel, LllegalType = x.LllegalType, LllegalTime = x.LllegalTime, LllegalPerson = x.LllegalPerson, LllegalAddress = x.LllegalAddress, FlowState = x.FlowState }).OrderBy(x => x.LllegalTime).ToList();
        }

        /// <summary>
        /// 核准
        /// </summary>
        /// <param name="model"></param>
        public void Approve(LllegalEntity model)
        {
            var entity = this.BaseRepository().FindEntity(model.ID);
            entity.ReformDate = model.ReformDate;
            entity.ReformPeople = model.ReformPeople;
            entity.ReformPeopleId = model.ReformPeopleId;
            entity.ApproveResult = model.ApproveResult;
            entity.ApproveReason = model.ApproveReason;
            entity.ApprovePerson = model.ApprovePerson;
            entity.ApprovePersonId = model.ApprovePersonId;
            entity.ApproveDate = model.ApproveDate;
            entity.CheckType = model.CheckType;
            entity.CheckContent = model.CheckContent;
            entity.AssessMoney = model.AssessMoney;
            if (model.ApproveResult == "0")
            {
                entity.FlowState = "待整改";
            }
            if (model.ApproveResult == "1")
            {
                entity.FlowState = "核准不通过";
            }

            this.BaseRepository().Update(entity);
        }

        private class newCount
        {
            public String Name { get; set; }
            public Int32 Count { get; set; }
            public String Code { get; set; }
        }
        public string GetCount(string deptid)
        {
            string sql = " select departmentid,fullname,encode from base_department where find_in_set(departmentid, fn_recursive('" + deptid + "')) > 0 and nature = '班组' group by encode desc;";
            DataTable dt = this.BaseRepository().FindTable(sql);
            string r = string.Empty;
            var m = DateTime.Now.Month;
            List<newCount> clist = new List<newCount>();
            var c = new newCount();
            foreach (DataRow row in dt.Rows)
            {
                c = new newCount();
                sql = "select count(*) from wg_lllegalregister where ApproveResult ='0' and month(LllegalTime)='" + m + "' and LllegalTeamId like '%" + row[0].ToString() + "%'";
                dt = this.BaseRepository().FindTable(sql);
                c.Name = row[1].ToString();
                c.Count = Convert.ToInt32(dt.Rows[0][0].ToString());
                c.Code = row[2].ToString();
                clist.Add(c);
            }
            //排序字段
            string[] property = new string[] { "Code", "Count" };
            //对应排序字段的排序方式
            bool[] sort = new bool[] { false, false };

            //对 List 排序
            clist = new IListSort<newCount>(clist, property, sort).Sort().ToList();

            foreach (newCount o in clist)
            {
                r += "{" + string.Format("category:'{0}',value:'{1}'", o.Name, o.Count) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            return r;
        }
        private class newObj
        {
            public String Name { get; set; }
            public Int32 Number { get; set; }
        }
        public string GetFinish(string deptid)
        {
            string sql = " select departmentid,fullname from base_department where find_in_set(departmentid, fn_recursive('" + deptid + "')) > 0 and nature = '班组';";
            DataTable dt = this.BaseRepository().FindTable(sql);
            string r = string.Empty;
            var m = DateTime.Now.Month;
            List<newObj> nlist = new List<newObj>();
            newObj n = new newObj();
            foreach (DataRow row in dt.Rows)
            {
                n = new newObj();
                sql = "select count(*) from wg_meetingjob where IsFinished ='finish' and month(endtime)='" + m + "' and groupid ='" + row[0].ToString() + "'";
                dt = this.BaseRepository().FindTable(sql);
                n.Name = row[1].ToString();
                n.Number = Convert.ToInt32(dt.Rows[0][0].ToString());
                nlist.Add(n);

            }
            var sortedlist =
                (from a in nlist
                 orderby a.Number descending
                 select a).ToList();
            sortedlist = sortedlist.Take(6).ToList();
            foreach (newObj o in sortedlist)
            {
                r += "{" + string.Format("category:'{0}',value:'{1}'", o.Name, o.Number) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            return r;
        }

        public DataTable GetMore(string deptid)
        {
            //Operator user = OperatorProvider.Provider.Current();
            //string deptid = user.DeptId;
            string sql = " select departmentid,fullname from base_department where find_in_set(departmentid, fn_recursive('" + deptid + "')) > 0 and nature = '班组';";
            DataTable dt = this.BaseRepository().FindTable(sql);
            string r = string.Empty;
            var m = DateTime.Now.Month;
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Bz");
            dt1.Columns.Add("Num");
            //foreach (DataRow row in dt.Rows)
            //{
            //    DataRow newrow = dt1.NewRow();

            //    sql = "select count(*) from wg_meetingjob where IsFinished ='finish' and month(endtime)='" + m + "' and groupid ='" + row[0].ToString() + "'";
            //    dt = this.BaseRepository().FindTable(sql);
            //    newrow[0] = row[1].ToString();
            //    newrow[1] = Convert.ToInt32(dt.Rows[0][0].ToString());
            //    dt1.Rows.Add(newrow);

            //}
            List<newObj> nlist = new List<newObj>();
            newObj n = new newObj();
            foreach (DataRow row in dt.Rows)
            {
                n = new newObj();
                sql = "select count(*) from wg_meetingjob where IsFinished ='finish' and month(endtime)='" + m + "' and groupid ='" + row[0].ToString() + "'";
                dt = this.BaseRepository().FindTable(sql);
                n.Name = row[1].ToString();
                n.Number = Convert.ToInt32(dt.Rows[0][0].ToString());
                nlist.Add(n);

            }
            var sortedlist =
                (from a in nlist
                 orderby a.Number descending
                 select a).ToList();
            foreach (newObj o in sortedlist)
            {
                DataRow newrow = dt1.NewRow();
                newrow[0] = o.Name;
                newrow[1] = o.Number;
                dt1.Rows.Add(newrow);
            }

            return dt1;
        }

        /// <summary>
        /// 根据用户id查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<LllegalEntity> GetLllegalDetailByUser(string userId, DateTime? start, DateTime? end, bool allowPaging, int page, int pageSize)
        {
            List<LllegalEntity> lllegal = this.BaseRepository().IQueryable().Where(x => x.LllegalPersonId.Contains(userId)
            ).OrderBy(x => x.LllegalTime).ToList();
            if (start != null) lllegal = lllegal.Where(x => x.LllegalTime >= start).ToList();
            if (end != null) lllegal = lllegal.Where(x => x.LllegalTime >= start).ToList();
            if (allowPaging) lllegal = lllegal.OrderByDescending(x => x.LllegalTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            foreach (var item in lllegal)
            {
                item.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == item.ID).ToList();

            }
            return lllegal;
        }
    }

}
