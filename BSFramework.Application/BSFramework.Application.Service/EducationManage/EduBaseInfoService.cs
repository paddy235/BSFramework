using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Application.Service.PeopleManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BSFramework.Application.Service.EducationManage
{
    /// <summary>
    /// 
    /// </summary>
    public class EduBaseInfoService : RepositoryFactory<EduBaseInfoEntity>, IEduBaseInfoService
    {
        private System.Data.Entity.DbContext _context;
        private IQueryable<EduBaseInfoEntity> _query;
        private DbSet<EduBaseInfoEntity> eduBaseInfoEntities;
        private DbSet<EduAnswerEntity> eduAnswerEntities;
        private readonly IEduAnswerService eduAnswerService;

        /// <summary>
        /// 
        /// </summary>
        public EduBaseInfoService()
        {
            _context = (DbFactory.Base() as Data.EF.Database).dbcontext;
            eduBaseInfoEntities = _context.Set<EduBaseInfoEntity>();
            eduAnswerEntities = _context.Set<EduAnswerEntity>();
            _query = eduBaseInfoEntities.AsNoTracking().AsQueryable();
            eduAnswerService = new EduAnswerService();
        }
        public IEnumerable<EduBaseInfoEntity> GetListApp(string deptId)
        {

            var query = this.BaseRepository().IQueryable().Where(x => x.BZId == deptId);

            return query.OrderByDescending(x => x.CreateDate).ToList();
        }
        public IEnumerable<EduBaseInfoEntity> GetAllList()
        {
            var query = this.BaseRepository().IQueryable();

            return query.OrderByDescending(x => x.CreateDate).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<EduBaseInfoEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var Expression = LinqExtensions.True<EduBaseInfoEntity>();
            var queryParam = queryJson.ToJObject();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId != "System")
            {
                Expression = Expression.And(x => x.BZId == user.DepartmentId);
            }
            Expression = Expression.And(x => x.Category != "安全学习日");
            if (!queryParam["eduType"].IsEmpty())
            {
                var type = queryParam["eduType"].ToString();
                if (type != "0")
                {
                    Expression = Expression.And(x => type.Contains(x.EduType));
                }
            }
            if (!queryParam["edutype"].IsEmpty())
            {
                var type = queryParam["edutype"].ToString();
                if (type != "0")
                {
                    Expression = Expression.And(x => type.Contains(x.EduType));
                }
            }
            if (!queryParam["type"].IsEmpty())
            {
                var type = queryParam["type"].ToString();
                if (type == "1")
                {
                    Expression = Expression.And(x => (x.TeacherId == userid && x.AnswerFlow != "2") || (x.RegisterPeopleId == userid && x.AnswerFlow == "0"));
                }
                if (type == "2")
                {
                    Expression = Expression.And(x => x.AnswerFlow == "2");
                }
                if (type == "3")
                {
                    Expression = Expression.And(x => x.AnswerFlow != "2" && x.TeacherId == userid);
                }
            }
            if (!queryParam["teacherid"].IsEmpty())
            {
                var teacherid = queryParam["teacherid"].ToString();
                Expression = Expression.And(x => x.TeacherId == teacherid);
            }
            if (!queryParam["regid"].IsEmpty())
            {
                var regid = queryParam["regid"].ToString();
                Expression = Expression.And(x => x.RegisterPeopleId == regid);
            }

            if (!queryParam["from"].IsEmpty())
            {
                var from = queryParam["from"].ToString();
                DateTime f = DateTime.Parse(from);
                Expression = Expression.And(x => x.ActivityDate >= f);
            }
            if (!queryParam["to"].IsEmpty())
            {
                var to = queryParam["to"].ToString();
                DateTime t = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                Expression = Expression.And(x => x.ActivityDate < t);
            }
            if (!queryParam["flow"].IsEmpty())
            {
                var flow = queryParam["flow"].ToString();
                if (flow == "1")
                {
                    Expression = Expression.And(x => x.Flow == flow);
                }
                else
                {
                    Expression = Expression.And(x => x.Flow != "1");

                }
            }

            pagination.records = db.IQueryable<EduBaseInfoEntity>().Count(Expression);
            var entityList = db.IQueryable<EduBaseInfoEntity>().Where(Expression).OrderByDescending(x => x.CreateDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return entityList.ToList();
        }


        /// <summary>
        /// 教育培训连接安全学习日
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<EduBaseInfoEntity> GetPageListEdAndAc(Pagination pagination, string queryJson, string userid)
        {

            var db = new RepositoryFactory().BaseRepository();
            IQueryable<EduBaseInfoEntity> query = from a in db.IQueryable<EduBaseInfoEntity>()
                                                  select a;
            var queryac = from a in db.IQueryable<EdActivityEntity>()
                          select a;
            var queryParam = queryJson.ToJObject();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId != "System")
            {
                query = query.Where(x => x.BZId == user.DepartmentId);
                queryac = queryac.Where(x => x.GroupId == user.DepartmentId);
            }
            if (!queryParam["eduType"].IsEmpty())
            {
                var type = queryParam["eduType"].ToString();
                if (type != "0")
                {
                    query = query.Where(x => type.Contains(x.EduType));
                    queryac = queryac.Where(x => type.Contains(x.ActivityType));
                }
            }
            if (!queryParam["edutype"].IsEmpty())
            {
                var type = queryParam["edutype"].ToString();
                if (type != "0")
                {
                    query = query.Where(x => type.Contains(x.EduType));
                    queryac = queryac.Where(x => type.Contains(x.ActivityType));
                }
            }
            if (!queryParam["flow"].IsEmpty())
            {
                var flow = queryParam["flow"].ToString();
                if (flow == "1")
                {
                    query = query.Where(x => x.Flow == flow);
                    queryac = queryac.Where(x => x.State == "Finish");
                }
                else
                {
                    query = query.Where(x => x.Flow != "1");
                    queryac = queryac.Where(x => x.State != "Finish");

                }
            }
            if (!queryParam["teacherid"].IsEmpty())
            {
                var teacherid = queryParam["teacherid"].ToString();
                query = query.Where(x => x.TeacherId == teacherid);
            }
            if (!queryParam["regid"].IsEmpty())
            {
                var regid = queryParam["regid"].ToString();
                query = query.Where(x => x.RegisterPeopleId == regid);
            }

            if (!queryParam["from"].IsEmpty())
            {
                var from = queryParam["from"].ToString();
                DateTime f = DateTime.Parse(from);
                query = query.Where(x => x.ActivityDate >= f);
                queryac = queryac.Where(x => x.StartTime >= f);
            }
            if (!queryParam["to"].IsEmpty())
            {
                var to = queryParam["to"].ToString();
                DateTime t = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.ActivityDate < t);
                queryac = queryac.Where(x => x.EndTime < t);
            }

            var queryacList = new List<EduBaseInfoEntity>();
            queryac.ToList().ForEach(a =>
            queryacList.Add(new EduBaseInfoEntity()
            {
                ID = a.ActivityId,
                CreateDate = a.CreateDate,
                Theme = a.Subject,
                ActivityDate = a.StartTime,
                ActivityEndDate = a.EndTime,
                EduType = a.ActivityType,
                BZId = a.GroupId,
                BZName = a.GroupName,
                Flow = a.State == "Finish" ? "1" : a.State == "Ready" ? "2" : "0"

            })
           );
            var all = queryacList.Concat(query).ToList();
            pagination.records = all.Count();
            var entityList = all.OrderByDescending(x => x.ActivityDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return entityList.ToList();

        }

        /// <summary>
        /// 获取教育培训任务
        /// </summary>
        /// <param name="pagination">分页公共类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id|Flow状态</param>
        /// <returns></returns>
        public List<EduBaseInfoEntity> GetEdJobList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            IQueryable<EduBaseInfoEntity> query = from a in db.IQueryable<EduBaseInfoEntity>()
                                                  select a;
            var queryParam = queryJson.ToJObject();
            //startTime开始时间
            if (!queryParam["startTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["startTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime startTime = time;
                query = query.Where(p => p.ActivityDate >= startTime);
            }
            //endTime结束时间
            if (!queryParam["endTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["endTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime endTime = time;
                query = query.Where(p => p.ActivityDate <= endTime);
            }
            //deptId部门id
            if (!queryParam["deptId"].IsEmpty())
            {
                string deptId = queryParam["deptId"].ToString();
                query = query.Where(p => deptId.Contains(p.BZId));

            }
            //userId用户id
            if (!queryParam["userId"].IsEmpty())
            {
                var userId = queryParam["userId"].ToString();
                var user = db.FindEntity<UserEntity>(userId);
                query = query.Where(p => p.BZId == user.DepartmentId);
            }

            //Flow状态
            if (!queryParam["Flow"].IsEmpty())
            {
                var Flow = queryParam["Flow"].ToString();
                query = query.Where(p => p.Flow == Flow);
            }
            int total = query.Count();
            pagination.records = total;
            var data = query.OrderByDescending(x => x.ActivityDate).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            //haveEvaluate是否查询评价
            if (!queryParam["haveEvaluate"].IsEmpty())
            {
                var haveEvaluate = queryParam["haveEvaluate"].ToString();
                if (haveEvaluate == "1")
                {
                    data.ForEach(x => x.Appraises = db.IQueryable<ActivityEvaluateEntity>(p => p.Activityid == x.ID).ToList());

                }
            }
            return data;

        }



        /// <summary>
        /// 未使用 注释
        /// </summary>
        /// <param name="from"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<EduBaseInfoEntity> GetAllList(DateTime from, string code)
        {
            //            var query = this.BaseRepository();
            //            var sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_edubaseinfo a
            //left join wg_activityevaluate b on a.ID = b.activityid
            //where activitydate > '{0}' and a.bzid in (select departmentid from base_department where encode like '{1}%' and nature = '班组')
            //group by a.id) a where total = 0", from, code);
            //            return query.FindList(sql);
            return new List<EduBaseInfoEntity>();
        }

        public IEnumerable<EduBaseInfoEntity> GetListBySql(string StrSql)
        {
            //var query = this.BaseRepository();
            //var sql = string.Format(@"select * from   wg_edubaseinfo  where 1=1 {0}", StrSql);
            //return query.FindList(sql);
            return new List<EduBaseInfoEntity>();
        }
        public IEnumerable<EduBaseInfoEntity> GetListBySql(string deptid, DateTime start, DateTime end)
        {
            var query = this.BaseRepository().IQueryable().Where(x => deptid.Contains(x.BZId) && x.ActivityDate >= start && x.ActivityDate <= end);
            return query;

        }
        public IEnumerable<EduBaseInfoEntity> GetPageList(string deptid, int page, int pagesize, out int total)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable().Where(x => x.BZId == deptid);
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        //public DataTable GetEducationPageList(Pagination pagination, string queryJson)
        //{
        //    DatabaseType dataType = DbHelper.DbType;

        //    var queryParam = queryJson.ToJObject();
        //    //查询条件
        //    //if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
        //    //{
        //    //    string condition = queryParam["condition"].ToString();
        //    //    string keyord = queryParam["keyword"].ToString();
        //    //    switch (condition)
        //    //    {
        //    //        case "Name":            //账户
        //    //            pagination.conditionJson += string.Format(" and theme  like '%{0}%'", keyord);
        //    //            break;

        //    //        default:
        //    //            break;
        //    //    }
        //    //}
        //    if (!queryParam["keyword"].IsEmpty())
        //    {
        //        var keyword = queryParam["keyword"].ToString();
        //        pagination.conditionJson += string.Format(" and theme  like '%{0}%'", keyword);
        //    }
        //    if (!queryParam["type"].IsEmpty())
        //    {
        //        var type = queryParam["type"].ToString();
        //        if (type != "0")
        //        {
        //            if (type == "2" || type == "5")
        //            {
        //                pagination.conditionJson += string.Format(" and (edutype = '2' || edutype = '5')");
        //            }
        //            else if (type == "3" || type == "6")
        //            {
        //                pagination.conditionJson += string.Format(" and (edutype = '3' || edutype = '6')");
        //            }
        //            else
        //            {
        //                pagination.conditionJson += string.Format(" and edutype = '{0}'", type);
        //            }

        //        }
        //    }
        //    if (!queryParam["appraise"].IsEmpty())
        //    {
        //        var appraise = queryParam["appraise"].ToString();
        //        if (appraise == "2")  //未评价
        //        {
        //            pagination.conditionJson += string.Format(" and (appraisecontent != '1' or appraisecontent is null)");
        //        }
        //        else if (appraise == "1") //已评价
        //        {
        //            pagination.conditionJson += string.Format(" and appraisecontent = '1'");
        //        }
        //    }
        //    if (!queryParam["code"].IsEmpty())
        //    {
        //        var code = queryParam["code"].ToString();
        //        pagination.conditionJson += string.Format(" and bzid in (select departmentid from base_department where encode like '{0}%')", code);
        //    }
        //    if (!queryParam["from"].IsEmpty())
        //    {
        //        var from = queryParam["from"].ToString();
        //        pagination.conditionJson += string.Format(" and activitydate > '{0}'", from);
        //    }
        //    if (!queryParam["to"].IsEmpty())
        //    {
        //        var to = queryParam["to"].ToString() + " 23:59:59";
        //        pagination.conditionJson += string.Format(" and activitydate <= '{0}'", to);
        //    }
        //    DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

        //    return dt;
        //}
        public EduBaseInfoEntity GetEntity(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            EduBaseInfoEntity entity = db.FindEntity<EduBaseInfoEntity>(keyValue);
            if (entity != null)
            {
                entity.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == entity.ID).ToList();
                entity.Answers = db.IQueryable<EduAnswerEntity>(x => x.EduId == entity.ID).OrderByDescending(x => x.CreateDate).ToList();
                foreach (var item in entity.Answers)
                {
                    item.Files = db.IQueryable<FileInfoEntity>(x => x.RecId == item.ID).ToList();
                }
            }


            return entity;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        public void update(EduBaseInfoEntity entity)
        {
            entity.Files = null;
            entity.Answers = null;
            entity.hasSign = null;
            this.BaseRepository().Update(entity);

        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EduBaseInfoEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (entity1 == null)
            {
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                // entity1.Theme = entity.Theme == null ? entity1.Theme : entity.Theme;
                entity.Files = null;
                entity.Files1 = null;
                entity.Answers = null;
                entity.Appraises = null;
                entity.Files2 = null;
                entity.BgImage = null;
                entity.AttendNumber = entity.AttendNumber == 0 ? entity1.AttendNumber : entity.AttendNumber;
                entity.SignNumber = entity.SignNumber == 0 ? entity1.SignNumber : entity.SignNumber;
                entity.DefaultNumber = entity.DefaultNumber == 0 ? entity1.DefaultNumber : entity.DefaultNumber;
                entity.LearnTime = entity.LearnTime == 0 ? entity1.LearnTime : entity.LearnTime;
                //entity1.DefaultPeople = entity.DefaultPeople == null ? entity1.Theme : entity.Theme;
                //entity1.DefaultPeopleId = entity.DefaultPeopleId == null ? entity1.DefaultPeopleId : entity.DefaultPeopleId;
                //entity1.SignPeople = entity.SignPeople == null ? entity1.SignPeople : entity.SignPeople;
                //entity1.SignPeopleId = entity.SignPeopleId == null ? entity1.SignPeopleId : entity.SignPeopleId;
                //entity1.SignNumber = entity.SignNumber == null || entity.SignNumber == 0 ? entity1.SignNumber : entity.SignNumber;
                //entity1.ImpartPeople = entity.ImpartPeople == null ? entity1.ImpartPeople : entity.ImpartPeople;
                //entity1.ImpartPeopleId = entity.ImpartPeopleId == null ? entity1.ImpartPeopleId : entity.ImpartPeopleId;
                //entity1.RegisterDate = entity.RegisterDate == null ? entity1.RegisterDate : entity.RegisterDate;
                //entity1.RegisterPeople = entity.RegisterPeople == null ? entity1.RegisterPeople : entity.RegisterPeople;
                //entity1.RegisterPeopleId = entity.RegisterPeopleId == null ? entity1.RegisterPeopleId : entity.RegisterPeopleId;
                //entity1.IsSaved = entity.IsSaved == null ? entity1.IsSaved : entity.IsSaved;
                //entity1.AppraiseContent = entity.AppraiseContent == null ? entity1.AppraiseContent : entity.AppraiseContent;
                //entity1.AppraiseDate = entity.AppraiseDate == null ? entity1.AppraiseDate : entity.AppraiseDate;
                //entity1.AppraisePeople = entity.AppraisePeople == null ? entity1.AppraisePeople : entity.AppraisePeople;
                //entity1.AppraisePeopleId = entity.AppraisePeopleId == null ? entity1.AppraisePeopleId : entity.AppraisePeopleId;
                //entity1.DefaultNumber = entity.DefaultNumber == null||entity.DefaultNumber==0 ? entity1.DefaultNumber : entity.DefaultNumber;
                //entity1.LearnTime = entity.LearnTime == null ? entity1.LearnTime : entity.LearnTime;
                //entity1.StartDate = entity.StartDate == null ? entity1.StartDate : entity.StartDate;
                //entity1.Flow = entity.Flow == null ? entity1.Flow : entity.Flow;
                //entity1.AttendNumber = entity.AttendNumber == null || entity.AttendNumber == 0 ? entity1.AttendNumber : entity.AttendNumber;
                //entity1.AttendPeople = entity.AttendPeople == null ? entity1.AttendPeople : entity.AttendPeople;
                //entity1.AttendPeopleId = entity.AttendPeopleId == null ? entity1.AttendPeopleId : entity.AttendPeopleId;
                //entity1.ActivityLocation = entity.ActivityLocation == null ? entity1.ActivityLocation : entity.ActivityLocation;
                //entity1.ActivityEndDate = entity.ActivityEndDate == null ? entity1.ActivityEndDate : entity.ActivityEndDate;
                //entity1.Teacher = entity.Teacher == null ? entity1.Teacher : entity.Teacher;
                //entity1.TeacherId = entity.TeacherId == null ? entity1.TeacherId : entity.TeacherId;
                //entity1.Describe = entity.Describe == null ? entity1.Describe : entity.Describe;
                //entity1.RunWay = entity.RunWay == null ? entity1.RunWay : entity.RunWay;
                //entity1.AnswerFlow = entity.AnswerFlow == null ? entity1.AnswerFlow : entity.AnswerFlow;
                //entity1.NewAppraiseContent = entity.NewAppraiseContent == null ? entity1.NewAppraiseContent : entity.NewAppraiseContent;
                //entity1.ActivityDate = entity.ActivityDate == null ? entity1.ActivityDate : entity.ActivityDate;
                if (string.IsNullOrEmpty(entity.Grade))
                {
                    entity1.Grade = "0";
                }
                else
                {
                    entity1.Grade = entity.Grade;
                }
                this.BaseRepository().Update(entity);


                List<string> pdf = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" };
                var fileBll = new FileInfoService();
                var files = fileBll.GetFilesByRecIdNew(entity1.ID).Where(x => x.Description != "教育培训二维码" && pdf.Contains(x.FileExtensions));
                //using (var factory = new ChannelFactory<IQueueService>("upload"))
                //{
                //    var channel = factory.CreateChannel();
                //    foreach (FileInfoEntity f in files)
                //    {
                //        if (string.IsNullOrEmpty(f.OtherUrl))
                //        {
                //            channel.Upload(f.FileId);
                //        }
                //    }

                //}
            }
        }

        /// <summary>
        /// 结束教育培训
        /// </summary>
        /// <param name="eduId"></param>
        public void Finsh(string eduId)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            EduBaseInfoEntity entity = db.FindEntity<EduBaseInfoEntity>(eduId);

            entity.ActivityEndDate = DateTime.Now;

            entity.Answers = null;
            entity.Files = null;
            entity.Flow = "1";


            db.Update(entity);
        }
        private class newCount
        {
            public String Name { get; set; }
            public decimal Count { get; set; }
            public String Code { get; set; }
        }
        private class newCount1
        {
            public String Name { get; set; }
            public int jsjk { get; set; }
            public int jswd { get; set; }

            public int sgyx { get; set; }

            public int fsgyx { get; set; }

            public int njsjk { get; set; }

            public int total { get; set; }
        }
        /// <summary>
        /// 培训学时统计
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="f"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetCount(string deptid, DateTime f, DateTime t)
        {
            var db = new RepositoryFactory().BaseRepository();
            PeopleService ps = new PeopleService();
            //string from = f.ToString("yyyy-MM-dd");
            //string to = t.ToString("yyyy-MM-dd");
            //string sql = string.Format(@" select a.fullname,sum(b.learntime),sum(b.attendnumber),a.departmentid,sum(b.learntime*b.attendnumber) from base_department a left join wg_edubaseinfo b on a.departmentid = b.bzid  and b.activitydate > '{0}' and b.activitydate < '{1}' and b.flow = '1' where a.nature = '班组' and a.ENCODE like '{2}%' group by a.departmentid", from, to, deptid);
            //DataTable dt = this.BaseRepository().FindTable(sql);

            var query = (from a in db.IQueryable<DepartmentEntity>()
                         join b in db.IQueryable<EduBaseInfoEntity>()
                         on a.DepartmentId equals b.BZId into t1
                         from tb1 in t1.DefaultIfEmpty()
                             //join c in db.IQueryable<EdActivityPersonEntity>()
                             //on a.ID equals c.ActivityId into t2
                             //from tb2 in t2.DefaultIfEmpty()
                         where a.EnCode.StartsWith(deptid) && tb1.ActivityDate >= f && tb1.ActivityEndDate <= t && tb1.Flow == "1" && a.Nature == "班组"
                         select new { a.DepartmentId, a.FullName, tb1.LearnTime, tb1.AttendNumber } into tb2
                         group tb2 by tb2.DepartmentId into tb3
                         select new
                         {
                             fullname = tb3.Max(x => x.FullName),
                             learntime = tb3.Sum(x => x.LearnTime),
                             attendnumber = tb3.Sum(x => x.AttendNumber),
                             departmentid = tb3.Key,
                             learntimeandattendnumber = tb3.Sum(x => x.LearnTime * x.AttendNumber)
                         });
            var dt = DataHelper.ConvertToTable(query);
            var dtUser = GetGroupCount(deptid, f, t);
            List<newCount> clist = new List<newCount>();
            var c = new newCount();
            foreach (DataRow row in dt.Rows)
            {
                var dtUserLinq = dtUser.AsEnumerable();
                var result = dtUserLinq.Where(x => x.Field<string>("departmentid") == row[3].ToString());
                //int num = ps.GetListByDept(row[3].ToString()).Count();
                int num = 0;
                foreach (DataRow rows in result)
                {
                    num += Convert.ToInt32(rows["persons"].ToString());
                }
                c = new newCount();
                c.Name = row[0].ToString();
                if (row[1].ToString() == "" || row[2].ToString() == "")
                {
                    c.Count = 0;
                }
                else
                {
                    if (num != 0)
                    {
                        c.Count = Math.Round(Convert.ToDecimal(row[4]) / num, 2);
                    }
                    else
                    {
                        c.Count = 0;
                    }
                }
                c.Code = row[2].ToString();

                clist.Add(c);
            }
            //排序字段
            string[] property = new string[] { "Code", "Count" };
            //对应排序字段的排序方式
            bool[] sort = new bool[] { false, false };

            //对 List 排序
            clist = new IListSort<newCount>(clist, property, sort).Sort().ToList();

            StringBuilder r = new StringBuilder();
            foreach (newCount o in clist)
            {
                r.AppendFormat("{{category:'{0}',value:'{1}'}},", o.Name, o.Count);
            }
            return "[" + r.ToString().TrimEnd(',') + "]";
        }


        /// <summary>
        /// 培训学时统计
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="f"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataTable GetCountTable(string deptid, DateTime f, DateTime t)
        {

            var db = new RepositoryFactory().BaseRepository();


            var query = (from a in db.IQueryable<DepartmentEntity>()
                         join b in db.IQueryable<EduBaseInfoEntity>()
                         on a.DepartmentId equals b.BZId into t1
                         from tb1 in t1.DefaultIfEmpty()

                         where a.EnCode.StartsWith(deptid) && tb1.ActivityDate >= f && tb1.ActivityEndDate <= t && tb1.Flow == "1" && a.Nature == "班组"
                         select new { a.DepartmentId, a.FullName, tb1.LearnTime, tb1.AttendNumber } into tb2
                         group tb2 by tb2.DepartmentId into tb3
                         select new
                         {
                             fullname = tb3.Max(x => x.FullName),
                             learntime = tb3.Sum(x => x.LearnTime),
                             attendnumber = tb3.Sum(x => x.AttendNumber),
                             departmentid = tb3.Key,
                             learntimeandattendnumber = tb3.Sum(x => x.LearnTime * x.AttendNumber)
                         });
            var dt = DataHelper.ConvertToTable(query);
            return dt;
        }
        /// <summary>
        /// 培训次数统计
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="f"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataTable GetLearnCountTable(string deptid, DateTime f, DateTime t)
        {

            var db = new RepositoryFactory().BaseRepository();


            var query = (from a in db.IQueryable<DepartmentEntity>()
                         join b in db.IQueryable<EduBaseInfoEntity>()
                         on a.DepartmentId equals b.BZId into t1
                         from tb1 in t1.DefaultIfEmpty()

                         where a.EnCode.StartsWith(deptid) && tb1.ActivityDate >= f && tb1.ActivityEndDate <= t && tb1.Flow == "1" && a.Nature == "班组"
                         select new { a.DepartmentId, a.FullName, tb1.LearnTime, tb1.AttendNumber, tb1.EduType }
                         );

            var dt = DataHelper.ConvertToTable(query);
            return dt;
        }

        /// <summary>
        /// 培训次数统计
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="f"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetLearnCount(string deptid, DateTime f, DateTime t)
        {
            PeopleService ps = new PeopleService();

            //string sql = string.Format(@" select a.fullname,a.departmentid from base_department a  where a.nature = '班组' and a.ENCODE like '{0}%'", deptid);
            //DataTable dt = this.BaseRepository().FindTable(sql);
            StringBuilder r = new StringBuilder();
            // var allList = this.GetAllList();
            var bzTable = GetGroupCount(deptid, f, t);
            var allTable = GetLearnCountTable(deptid, f, t);
            foreach (DataRow row in bzTable.Rows)
            {
                var allTableLinq = allTable.AsEnumerable();

                int jsjk = allTableLinq.Count(x => x.Field<string>("EduType") == "1" && x.Field<string>("DepartmentId") == row[0].ToString());
                int jswd = allTableLinq.Count(x => x.Field<string>("DepartmentId") == row[0].ToString() && (x.Field<string>("EduType") == "2" || x.Field<string>("EduType") == "5"));
                int sgyx = allTableLinq.Count(x => x.Field<string>("DepartmentId") == row[0].ToString() && (x.Field<string>("EduType") == "3" || x.Field<string>("EduType") == "6"));
                int fsgyx = allTableLinq.Count(x => x.Field<string>("DepartmentId") == row[0].ToString() && x.Field<string>("EduType") == "4");
                int njswd = allTableLinq.Count(x => x.Field<string>("DepartmentId") == row[0].ToString() && (x.Field<string>("EduType") == "2" || x.Field<string>("EduType") == "5"));
                int nsgyx = allTableLinq.Count(x => x.Field<string>("DepartmentId") == row[0].ToString() && (x.Field<string>("EduType") == "3" || x.Field<string>("EduType") == "6"));
                int kwjj = allTableLinq.Count(x => x.Field<string>("DepartmentId") == row[0].ToString() && (x.Field<string>("EduType") == "7" || x.Field<string>("EduType") == "8"));
                int total = jswd + jswd + sgyx;
                r.AppendFormat("{{category:'{0}',value:'{1}',value1:'{2}',value2:'{3}',value3:'{4}',value4:'{5}',value5:'{6}',value6:'{7}'}},", row[1], jsjk, jswd, sgyx, fsgyx, njswd, nsgyx, kwjj);
            }
            return "[" + r.ToString().TrimEnd(',') + "]";
        }
        public DataTable GetGroupCount(string deptid, DateTime f, DateTime t)
        {
            PeopleService ps = new PeopleService();
            string from = f.ToString("yyyy-MM-dd");
            string to = t.ToString("yyyy-MM-dd");
            //            string sql = string.Format(@" select a.departmentid,a.fullname,count(b.id) as persons,sum(c.learntime) as total from base_department a
            //left join wg_people b on  a.departmentid = b.BZID
            //left join wg_edubaseinfo c on a.departmentid = c.bzid and c.activitydate >= '{0}' and c.activitydate < '{1}' 
            //where a.nature = '班组' and a.encode like '{2}%'
            //group by a.departmentid", from, to, deptid);
            //            string sql = string.Format(@" select a.departmentid,a.fullname,count(b.id) as persons from base_department a
            //left join wg_people b on  a.departmentid = b.BZID
            //where a.nature = '班组' and a.encode like '{2}%'
            //group by a.departmentid", from, to, deptid);
            //            DataTable dt = this.BaseRepository().FindTable(sql);
            //            return dt;
            var db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<DepartmentEntity>()
                        join b in db.IQueryable<PeopleEntity>()
                        on a.DepartmentId equals b.BZID into t1
                        from tb1 in t1.DefaultIfEmpty()
                        where a.Nature == "班组" && a.EnCode.StartsWith(deptid)
                        select new { a.DepartmentId, a.FullName, tb1.ID } into tb2
                        group tb2 by tb2.DepartmentId into tb3
                        select new
                        {
                            departmentid = tb3.Max(x => x.DepartmentId),
                            fullname = tb3.Max(x => x.FullName),
                            persons = tb3.Count(x => x.ID != null)
                        };
            var queryTalbe = DataHelper.ConvertToTable(query);
            return queryTalbe;
        }

        public DataTable GetTimeCount(string deptid, DateTime f, DateTime t, string category)
        {
            string from = f.ToString("yyyy-MM-dd");
            string to = t.ToString("yyyy-MM-dd");

            //            string sql = string.Format(@" select  a.id as activityid,a.activitydate as starttime,a.activityenddate as endtime,b.ENCODE as deptcode,b.FULLNAME as deptname,b.DEPARTMENTID as deptid,attendnumber as sum  FROM wg_edubaseinfo a 
            //LEFT JOIN base_department b on a.bzid=b.DEPARTMENTID
            //where b.ENCODE like '{0}%' AND a.activitydate>='{1}' and a.activityenddate<='{2}' and a.flow=1 and a.edutype='{3}'
            //union all
            //select a.activityid,a.starttime,a.endtime,b.ENCODE as deptcode,b.FULLNAME as deptname,b.DEPARTMENTID as deptid,(select count(activityid) 
            //from wg_activityperson where activityid=a.activityid) as sum FROM wg_edactivity a 
            //LEFT JOIN base_department b on a.groupid=b.DEPARTMENTID
            //where b.ENCODE like '{0}%' AND a.starttime>='{1}' and a.endtime<='{2}' and a.state='Finish' and a.activitytype='{3}'", deptid, from, to, category);
            //            DataTable dt = this.BaseRepository().FindTable(sql);
            //            return dt;
            var db = new RepositoryFactory().BaseRepository();
            //var query = (from a in db.IQueryable<EduBaseInfoEntity>()
            //             join b in db.IQueryable<DepartmentEntity>()
            //             on a.BZId equals b.DepartmentId into t1
            //             from tb1 in t1.DefaultIfEmpty()
            //             where tb1.EnCode.StartsWith(deptid) && a.ActivityDate >= f && a.ActivityEndDate <= t && a.Flow == "1" && a.EduType == category
            //             select new { activityid = a.ID, starttime = a.ActivityDate.HasValue? a.ActivityDate.Value.ToString("yyyy-MM-dd HH:mm:ss"):"", endtime = a.ActivityEndDate.HasValue? a.ActivityEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss"):"", deptcode = tb1.EnCode, deptname = tb1.FullName, deptid = tb1.DepartmentId, sum = a.AttendNumber })
            //            .Concat
            if (category == "安全学习日")
            {
                var query = (from a in db.IQueryable<EdActivityEntity>()
                             join b in db.IQueryable<DepartmentEntity>()
                             on a.GroupId equals b.DepartmentId into t1
                             from tb1 in t1.DefaultIfEmpty()
                             join c in db.IQueryable<EdActivityPersonEntity>()
                             on a.ActivityId equals c.ActivityId into t2
                             from tb2 in t2.DefaultIfEmpty()
                             where tb1.EnCode.StartsWith(deptid) && a.StartTime >= f && a.EndTime <= t && a.State == "Finish" && a.ActivityType == category
                             select new
                             {
                                 activityid = a.ActivityId,
                                 starttime = a.StartTime,
                                 endtime = a.EndTime,
                                 deptcode = tb1.EnCode,
                                 deptname = tb1.FullName,
                                 deptid = tb1.DepartmentId,
                                 sum = t2.Count(x => x.ActivityId == a.ActivityId)
                             }
                       );
                var queryTalbe = DataHelper.ConvertToTable(query);
                return queryTalbe;
            }
            else
            {

                var query = (from a in db.IQueryable<EduBaseInfoEntity>()
                             join b in db.IQueryable<DepartmentEntity>()
                             on a.BZId equals b.DepartmentId into t1
                             from tb1 in t1.DefaultIfEmpty()
                                 //join c in db.IQueryable<EdActivityPersonEntity>()
                                 //on a.ID equals c.ActivityId into t2
                                 //from tb2 in t2.DefaultIfEmpty()
                             where tb1.EnCode.StartsWith(deptid) && a.ActivityDate >= f && a.ActivityEndDate <= t && a.Flow == "1" && a.EduType == category
                             select new
                             {
                                 activityid = a.ID,
                                 starttime = a.ActivityDate,
                                 endtime = a.ActivityEndDate,
                                 deptcode = tb1.EnCode,
                                 deptname = tb1.FullName,
                                 deptid = tb1.DepartmentId,
                                 sum = a.AttendNumber
                             }
            );

                var queryTalbe = DataHelper.ConvertToTable(query);
                return queryTalbe;



            }



        }

        public int GetNum1(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = @from.AddYears(1).AddMinutes(-1);

            var query = from q in this.BaseRepository().IQueryable()
                        where q.BZId == deptid && q.EduType == "2" && q.ActivityDate >= @from && q.ActivityDate <= to
                        select q.AttendNumber;
            var cnt = query.Count();
            if (cnt > 0) return query.Sum(x => x);
            else return 0;
        }

        public int GetNum2(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = @from.AddYears(1).AddMinutes(-1);

            var query = from q in this.BaseRepository().IQueryable()
                        where q.BZId == deptid && q.EduType == "3" && q.ActivityDate >= @from && q.ActivityDate <= to
                        select q.AttendNumber;
            var cnt = query.Count();
            if (cnt > 0) return query.Sum(x => x);
            else return 0;
        }

        public int GetNum3(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = @from.AddYears(1).AddMinutes(-1);

            var query = from q in this.BaseRepository().IQueryable()
                        where q.BZId == deptid && q.EduType == "1" && q.ActivityDate >= @from && q.ActivityDate <= to
                        select q.AttendNumber;
            var cnt = query.Count();
            if (cnt > 0) return query.Sum(x => x);
            else return 0;
        }

        public EduBaseInfoEntity GetDetail(string id)
        {
            var eduset = _context.Set<EduBaseInfoEntity>();
            var answerset = _context.Set<EduAnswerEntity>();
            var fileset = _context.Set<FileInfoEntity>();
            var entity = eduset.Find(id);
            if (entity != null)
            {
                entity.Answers = answerset.Where(x => x.EduId == id).ToList();
                if (entity.Answers != null)
                {
                    foreach (var item in entity.Answers)
                    {
                        item.Files = fileset.Where(x => x.RecId == item.ID).ToList();
                    }
                }
                entity.Files2 = fileset.Where(x => x.RecId == id && x.Description == "课件").ToList();
                entity.Files = fileset.Where(x => x.RecId == id && x.Description == "技术讲课图片").ToList();
            }
            return entity;
        }

        public void End(string subActivityId)
        {
            var eduset = _context.Set<EduBaseInfoEntity>();
            var entity = eduset.Find(subActivityId);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.Flow = "1";
                entity.ActivityEndDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void EditEducation(EduBaseInfoEntity data)
        {
            var eduset = _context.Set<EduBaseInfoEntity>();
            var entity = eduset.Find(data.ID);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                entity.Theme = data.Theme;
                entity.RunWay = data.RunWay;

                _context.SaveChanges();
            }
        }

        public List<EduBaseInfoEntity> GetList(string name, string category, DateTime? start, DateTime? end, string appraise, string[] depts, int pagesize, int pageindex, out int total)
        {
            var query = from q1 in _context.Set<EduBaseInfoEntity>()
                        join q3 in _context.Set<UserEntity>() on q1.BZId equals q3.DepartmentId into into3
                        //from t3 in into3.DefaultIfEmpty()
                        join q4 in _context.Set<ActivityEvaluateEntity>() on q1.ID equals q4.Activityid into into4
                        //from t4 in into4.DefaultIfEmpty()
                        where depts.Contains(q1.BZId) && q1.Flow == "1"
                        select new
                        {
                            q1.ID,
                            q1.EduType,
                            q1.BZName,
                            q1.CreateDate,
                            q1.Theme,
                            q1.LearnTime,
                            q1.ActivityDate,
                            q1.ActivityEndDate,
                            q1.AttendNumber,
                            Total = into3.Count(),
                            Evaluate = into4.Count()
                        };

            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Theme.Contains(name));
            if (!string.IsNullOrEmpty(category) && category != "0") query = query.Where(x => x.EduType == category);
            if (start != null) query = query.Where(x => x.ActivityDate >= start);
            if (end != null) query = query.Where(x => x.ActivityEndDate <= end);
            if (appraise == "1") query = query.Where(x => x.Evaluate > 0);

            total = query.Count();
            var data = query.OrderByDescending(x => x.ActivityDate).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
            return data.Select(x => new EduBaseInfoEntity
            {
                ID = x.ID,
                EduType = x.EduType,
                BZName = x.BZName,
                Theme = x.Theme,
                LearnTime = x.LearnTime,
                ActivityDate = x.ActivityDate,
                ActivityEndDate = x.ActivityEndDate,
                DefaultNumber = x.Total,
                SignNumber = x.AttendNumber
            }).ToList();
        }

        public int Count(string[] depts, string[] category, DateTime start, DateTime end)
        {
            var query = from q in _context.Set<EduBaseInfoEntity>()
                        where depts.Contains(q.BZId) && category.Contains(q.EduType) && q.Flow == "1" && q.ActivityDate >= start && q.ActivityDate < end
                        select q;

            return query.Count();
        }

        public List<EduBaseInfoEntity> GetList()
        {
            return _query.ToList();
        }

        public List<EduBaseInfoEntity> GetList(int pageSize, int pageIndex, out int total)
        {
            total = _query.Count();
            return _query.OrderByDescending(x => x.ActivityDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        public List<EduBaseInfoEntity> GetList(string[] depts)
        {
            _query = _query.Where(x => depts.Contains(x.BZId));
            return GetList();
        }
        public List<EduBaseInfoEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total)
        {
            _query = _query.Where(x => depts.Contains(x.BZId));
            return GetList(pageSize, pageIndex, out total);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? fromtime, DateTime? to)
        {
            if (fromtime != null) _query = _query.Where(x => x.ActivityDate >= fromtime);
            if (to != null) _query = _query.Where(x => x.ActivityDate <= to);

            return GetList(depts);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? fromtime, DateTime? to, int pageSize, int pageIndex, out int total)
        {
            if (fromtime != null) _query = _query.Where(x => x.ActivityDate >= fromtime);
            if (to != null) _query = _query.Where(x => x.ActivityDate <= to);

            return GetList(depts, pageSize, pageIndex, out total);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow)
        {
            if (!string.IsNullOrEmpty(flow)) _query = _query.Where(x => x.Flow == flow);
            return GetList(depts, from, to);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, int pageSize, int pageIndex, out int total)
        {
            if (!string.IsNullOrEmpty(flow)) _query = _query.Where(x => x.Flow == flow);
            return GetList(depts, from, to, pageSize, pageIndex, out total);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, string edutype)
        {
            if (!string.IsNullOrEmpty(edutype)) _query = _query.Where(x => x.EduType == edutype);
            return GetList(depts, from, to, flow);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, string edutype, int pageSize, int pageIndex, out int total)
        {
            if (!string.IsNullOrEmpty(edutype)) _query = _query.Where(x => x.EduType == edutype);
            return GetList(depts, from, to, flow, pageSize, pageIndex, out total);
        }

        public void Add(EduBaseInfoEntity entity)
        {
            var ids = entity.Answers.Select(x => x.ID).ToList();
            var entities = eduAnswerEntities.Where(x => ids.Contains(x.ID)).ToList();
            foreach (var item in entities)
            {
                _context.Entry(item).State = EntityState.Modified;
                item.EduId = entity.ID;
            }

            _context.Entry(entity).State = EntityState.Added;
            _context.SaveChanges();
        }

        public EduBaseInfoEntity Get(string id)
        {
            var entity = eduBaseInfoEntities.Find(id);
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public List<EduBaseInfoEntity> List(int pageSize, int pageIndex)
        {
            return _query.OrderByDescending(x => x.ActivityDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        public List<EduBaseInfoEntity> FilterByMeeting(string id)
        {
            return _query.Where(x => x.MeetingId == id).ToList();
        }

        public void Modify(EduBaseInfoEntity entity)
        {
            var ids = entity.Answers.Select(x => x.ID).ToList();
            var entities = eduAnswerEntities.Where(x => ids.Contains(x.ID)).ToList();
            foreach (var item in entities)
            {
                _context.Entry(item).State = EntityState.Modified;
                item.EduId = entity.ID;
            }

            _context.SaveChanges();
        }
    }
}
