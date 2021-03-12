using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Data;
using BSFramework.Application.Service.BusinessExceptions;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Service.WorkMeeting;
using BSFramework.Application.Entity.PeopleManage;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Data.EF;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：安全预知训练
    /// </summary>
    public class DangerService : RepositoryFactory<DangerEntity>, DangerIService
    {


        private System.Data.Entity.DbContext _context;
        public DangerService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }



        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<DangerEntity> GetList(string queryJson)
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 获取需要预知训练的工作任务
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public DataTable GetJobList(string groupId)
        {
            //DataTable dt = this.BaseRepository().FindTable(string.Format("select id,jobname, a.jobid, cast(case when b.meetingjobid is null then 0 else 1 end as signed) as fromjob from wg_danger a left join wg_meetingandjob b on b.meetingjobid = a.jobid where (status = 0 || status = 1) and a.groupid = '{0}' order by jobtime ", groupId));
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<DangerEntity>()
                        join b in db.IQueryable<MeetingAndJobEntity>() on a.JobId equals b.MeetingJobId into t1
                        from tb1 in t1.DefaultIfEmpty()
                        where (a.Status == 0 || a.Status == 1) && a.GroupId == groupId
                        orderby a.JobTime descending
                        select new
                        {
                            id = a.Id,
                            jobname = a.JobName,
                            jobid = a.JobId,
                            fromjob = tb1.MeetingJobId == null ? 0 : 1
                        };
            var queryTalbe = DataHelper.ConvertToTable(query);
            return queryTalbe;
            //return dt;
        }
        /// <summary>
        /// 根据工作任务Id选择措施落实人员
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public DataTable GetJobUserList(string jobId)
        {
            // DataTable dt = this.BaseRepository().FindTable(string.Format("select userid,username from wg_jobuser  where  meetingjobid='{0}'", jobId));
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<JobUserEntity>()
                        where a.MeetingJobId == jobId
                        select new { userid = a.UserId, username = a.UserName };
            var queryTalbe = DataHelper.ConvertToTable(query);
            return queryTalbe;
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {

            var queryParam = queryJson.ToJObject();

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString().Trim();
                pagination.conditionJson += string.Format(" and jobname like '%{0}%'", keyword);
            }
            if (!queryParam["from"].IsEmpty())
            {
                string from = queryParam["from"].ToString().Trim();
                pagination.conditionJson += string.Format(" and jobtime> '{0}'", from);
            }
            if (!queryParam["to"].IsEmpty())
            {
                string to = queryParam["to"].ToString().Trim();
                pagination.conditionJson += string.Format(" and jobtime<='{0} 23:59:59'", to);
            }
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyord = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "Account":            //账户
                        pagination.conditionJson += string.Format(" and ACCOUNT  like '%{0}%'", keyord);
                        break;
                    case "RealName":          //姓名
                        pagination.conditionJson += string.Format(" and REALNAME  like '%{0}%'", keyord);
                        break;
                    case "Mobile":          //手机
                        pagination.conditionJson += string.Format(" and MOBILE like '%{0}%'", keyord);
                        break;
                    default:
                        break;
                }
            }
            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            return dt;
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPagesList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var query = from a in db.IQueryable<DangerEntity>()
                        select new
                        {
                            deptcode = a.DeptCode,
                            jobname = a.JobName,
                            jobtime = a.JobTime,
                            status = a.Status,
                            operdate = a.OperDate,
                            groupname = a.GroupName,
                            filepath = (from b in db.IQueryable<FileInfoEntity>()
                                        where b.RecId == a.Id && (b.FileType == "jpg" || b.FileType == "gif" || b.FileType == "bmp")
                                        orderby b.FilePath descending
                                        select b.FilePath
                                      ).FirstOrDefault()
                        };

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString().Trim();
                query = query.Where(x => x.jobname.Contains(keyword));
                // pagination.conditionJson += string.Format(" and jobname like '%{0}%'", keyword);
            }
            if (!queryParam["from"].IsEmpty())
            {
                var from = Convert.ToDateTime(queryParam["from"].ToString().Trim());
                query = query.Where(x => x.jobtime > from);
                // pagination.conditionJson += string.Format(" and jobtime> '{0}'", from);
            }
            if (!queryParam["to"].IsEmpty())
            {
                var to = Convert.ToDateTime(queryParam["to"].ToString().Trim() + " 23:59:59");
                query = query.Where(x => x.jobtime <= to);
                // pagination.conditionJson += string.Format(" and jobtime<='{0} 23:59:59'", to);
            }

            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            var data = query.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            pagination.records = query.Count();
            var queryTalbe = DataHelper.ConvertToTable(data);
            return queryTalbe;
        }

        /// <summary>
        /// 获取危险预知台账和评价
        /// </summary>
        ///<param name="pagination">分页公用类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|haveEvaluate是否查询评价|Depts部门id集合</param>
        /// <returns></returns>
        public List<DangerEntity> GetDangerCount(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<DangerEntity>()
                        select a;

            var queryParam = queryJson.ToJObject();
            //startTime开始时间
            if (!queryParam["startTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["startTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime startTime = time;
                query = query.Where(p => p.JobTime >= startTime);
            }
            //endTime结束时间
            if (!queryParam["endTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["endTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime endTime = time;
                query = query.Where(p => p.JobTime <= endTime);
            }
            //deptId部门id
            if (!queryParam["deptId"].IsEmpty())
            {
                string deptId = queryParam["deptId"].ToString();
                query = query.Where(p => p.GroupId == deptId);

            }
            // Depts部门id集合
            if (!queryParam["Depts"].IsEmpty())
            {
                string Depts = queryParam["Depts"].ToString();
                query = query.Where(p => Depts.Contains(p.GroupId));

            }
            int total = query.Count();
            pagination.records = total;
            var data = query.OrderByDescending(x=>x.JobTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();

            //haveEvaluate是否查询评价
            if (!queryParam["haveEvaluate"].IsEmpty())
            {
                var haveEvaluate = queryParam["haveEvaluate"].ToString();
                if (haveEvaluate == "1")
                {
                    data.ForEach(x => x.Evaluateions = db.IQueryable<ActivityEvaluateEntity>(p => p.Activityid == x.Id).ToList());

                }
            }
            return data;

        }

            /// <summary>
            /// 获取实体
            /// </summary>
            /// <param name="keyValue">主键值</param>
            /// <returns></returns>
            public DangerEntity GetEntity(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var data = db.FindEntity<DangerEntity>(keyValue);
            var meetinandjob = db.FindEntity<MeetingAndJobEntity>(data.JobId);

            data.FromJob = meetinandjob.StartMeetingId != null;
            return data;
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
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DangerEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
                this.BaseRepository().Update(entity);


            }
            else
            {
                entity.Create();
                this.BaseRepository().Insert(entity);
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public DangerEntity Save(MeetingJobEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {

                var dept = db.FindEntity<DepartmentEntity>(entity.GroupId);
                DangerEntity danger = new DangerEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = entity.GroupId,
                    JobId = entity.Relation.MeetingJobId,
                    JobName = entity.Job,
                    Persons = entity.Relation.JobUser,
                    JobUser = entity.Relation.JobUser,
                    JobTime = entity.StartTime,
                    Sno = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    DeptCode = dept.EnCode,
                    CreateDate = DateTime.Now,
                    GroupName = dept.FullName,
                    CreateUserId = entity.CreateUserId,
                    TicketId = entity.TicketCode
                };

                //danger.Create();

                var measures = new List<MeasuresEntity>();
                if (!string.IsNullOrEmpty(entity.TemplateId))
                {
                    var dangertemplates = db.IQueryable<DangerTemplateEntity>(x => x.JobId == entity.TemplateId).ToList();
                    measures.AddRange(dangertemplates.Select(x => new MeasuresEntity() { Id = Guid.NewGuid().ToString(), DangerSource = x.Dangerous, Measure = x.Measure, DangerId = danger.Id, CreateDate = DateTime.Now, CreateUserId = entity.CreateUserId }));
                }
                else
                {
                    //measures.Add(new MeasuresEntity() { Id = Guid.NewGuid().ToString(), DangerSource = entity.Dangerous, Measure = entity.Measure, DangerId = danger.Id, CreateDate = DateTime.Now });
                }

                db.Insert(danger);
                db.Insert(measures);

                db.Commit();

                return danger;
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

            //if(this.BaseRepository().Insert(danger)>0)
            //{
            //    string sql = string.Format("insert into wg_measures(id,dangersource,measure,dangerid,createdate) select uuid(),dangerous,measure,'{1}','{2}' from wg_dangertemplate where jobid='{0}'", entity.TemplateId, danger.Id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //    this.BaseRepository().ExecuteBySql(sql);
            //}
        }

        public MeasuresEntity EditItem(MeasuresEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity1 = db.FindEntity<MeasuresEntity>(entity.Id);
                entity1.Measure = entity.Measure;
                entity1.DangerSource = entity.DangerSource;
                db.Update(entity1);

                db.Commit();

                return entity;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="keyValue">实体主键</param>
        /// <param name="entity">危险预知训练实体</param>
        /// <param name="measures">防控措施</param>
        public void Update(string keyValue, DangerEntity entity, List<MeasuresEntity> measures)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            WorkmeetingService w = new WorkmeetingService();
            try
            {
                var dataEntity = db.FindEntity<DangerEntity>(keyValue);
                entity.Modify(keyValue);
                dataEntity.TicketId = entity.TicketId;
                dataEntity.Sno = entity.Sno;
                dataEntity.JobAddress = entity.JobAddress;
                dataEntity.Measure = entity.Measure;
                dataEntity.StopMeasure = entity.StopMeasure;
                dataEntity.Status = entity.Status;
                dataEntity.OperDate = entity.OperDate;
                dataEntity.OperUserId = entity.OperUserId;
                dataEntity.ScoreRemark = entity.ScoreRemark;
                dataEntity.AppraiseContent = entity.AppraiseContent;
                foreach (var item in measures)
                {
                    var dataitem = db.FindEntity<MeasuresEntity>(item.Id);

                    dataitem.DangerSource = item.DangerSource;
                    dataitem.Measure = item.Measure;
                    dataitem.DutyMan = item.DutyMan;
                    dataitem.IsOver = item.IsOver;
                    db.Update(dataitem);
                }

                db.Update(dataEntity);
                db.Commit();


            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public IList<DangerEntity> GetMyTrainings(string userId, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DangerEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.MeetingJobId into into1
                        from q2 in into1.DefaultIfEmpty()
                        join q3 in db.IQueryable<JobUserEntity>() on q1.JobId equals q3.MeetingJobId
                        where q3.UserId == userId && (q1.Status == 0 || q1.Status == 1)
                        select new { q1, q2 };

            total = query.Count();
            var data = query.ToList();
            var list = new List<DangerEntity>();

            foreach (var item in data)
            {
                var danger = item.q1;
                danger.Files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == danger.Id).ToList();
                danger.JobUsers = (from q1 in db.IQueryable<JobUserEntity>()
                                   join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                   where q2.MeetingJobId == danger.JobId
                                   select q1).ToList();
                var checker = danger.JobUsers.Find(x => x.JobType == "ischecker");
                if (checker != null)
                {
                    var user = db.IQueryable<PeopleEntity>().FirstOrDefault(x => x.ID == checker.UserId);
                    if (user != null) danger.Photo = string.IsNullOrEmpty(user.Photo) ? null : user.Photo;
                }
                danger.FromJob = item.q2 == null ? false : item.q2.StartMeetingId != null;
                list.Add(danger);
            }
            return list;
        }
        public List<JobUserEntity> GetUsersByDanger(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<JobUserEntity>()
                        where q1.MeetingJobId == id
                        select q1;
            return query.ToList();
        }
        public DangerEntity GetTrainingDetail(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query1 = from q1 in db.IQueryable<DangerEntity>()
                         where q1.Id == id
                         select q1;

            var users = (from q1 in db.IQueryable<DangerEntity>()
                             //join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                         join q3 in db.IQueryable<JobUserEntity>() on q1.JobId equals q3.MeetingJobId
                         where q1.Id == id
                         select q3).ToList();

            var query2 = from q1 in db.IQueryable<MeasuresEntity>()
                         where q1.DangerId == id
                         orderby q1.CreateDate
                         select q1;

            var query3 = from q1 in db.IQueryable<DangerEntity>()
                             //join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                         join q3 in db.IQueryable<JobUserEntity>() on q1.JobId equals q3.MeetingJobId
                         where q1.Id == id
                         orderby q3.CreateDate
                         select q3;

            var query4 = from q1 in db.IQueryable<FileInfoEntity>()
                         where q1.RecId == id
                         select q1;

            //if (users.Count > 0)
            //{
            //    if (users[0].JobType == "isdoperson")
            //    {
            //        query4 = query4.Where(x => x.CreateUserId == userid);
            //    }
            //}
            query4 = query4.OrderBy(x => x.CreateDate);

            var data = query1.FirstOrDefault();
            if (data != null)
            {
                data.TrainingItems = query2.ToList();
                data.JobUsers = query3.ToList();
                data.Files = query4.ToList();
            }

            return data;
        }


        public MeasuresEntity AddItem(MeasuresEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                    db.Insert(entity);
                }
                else
                {
                    var entity1 = db.FindEntity<MeasuresEntity>(entity.Id);
                    entity1.UserId = entity.UserId;
                    entity1.DutyMan = entity.DutyMan;
                    entity1.Measure = entity.Measure;
                    entity1.DangerSource = entity.DangerSource;
                    entity1.IsOver = entity.IsOver;
                    entity1.CreateUserId = entity.CreateUserId;
                    db.Update(entity1);
                }

                db.Commit();

                return entity;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void UploadTraining(DangerEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity1 = db.FindEntity<DangerEntity>(entity.Id);

                //entity1.Status = entity.Status;
                foreach (var item in entity.Files)
                {
                    if (item.State == 1)
                    {
                        db.Delete<FileInfoEntity>(item.FileId);
                    }
                    else
                    {
                        db.Insert(item);
                    }
                }

                db.Update(entity1);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public dynamic GetTrainingUsers(string id, string userid, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var data = default(IList<JobUserEntity>);
            var d1 = new object();
            if (string.IsNullOrEmpty(id))
            {
                var user = db.FindEntity<UserEntity>(userid);

                var query = from q1 in db.IQueryable<UserEntity>()
                                //join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                            orderby q1.RealName
                            where q1.DepartmentId == user.DepartmentId
                            select q1;

                d1 = query.Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList().Select(x => new JobUserEntity() { UserId = x.UserId, UserName = x.RealName, Photo = x.Photo }).ToList();
                total = query.Count();
            }
            else
            {
                var query = from q1 in db.IQueryable<DangerEntity>()
                            join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q3.MeetingJobId
                            join q2 in db.IQueryable<JobUserEntity>() on q3.MeetingJobId equals q2.MeetingJobId
                            where q1.Id == id
                            select q2;
                data = query.OrderBy(x => x.CreateDate).Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList();
                total = query.Count();

                d1 = from x1 in data
                     join x2 in db.IQueryable<PeopleEntity>() on x1.UserId equals x2.ID into into1
                     from t1 in into1.DefaultIfEmpty()
                     orderby t1.Planer, x1.UserName
                     select new { JobUserId = x1.JobUserId, UserId = x1.UserId, UserName = x1.UserName, JobType = x1.JobType, CreateDate = x1.CreateDate, JobId = x1.MeetingJobId, Score = x1.Score, ImageUrl = x1.ImageUrl, Photo = t1.Photo };
            }

            return d1;
        }

        public void PostTraining(DangerEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity1 = db.FindEntity<DangerEntity>(entity.Id);
                if (entity1.Status == 1) throw new TrainingExcuteException("危险预知训练进行中，无法再提交训练项");

                entity1.TicketId = entity.TicketId;
                entity1.JobAddress = entity.JobAddress;

                db.Update(entity1);

                foreach (var item in entity.TrainingItems)
                {
                    switch (item.State)
                    {
                        case 0:
                            item.CreateDate = DateTime.Now;
                            item.DangerId = entity1.Id;
                            item.Id = Guid.NewGuid().ToString();
                            db.Insert(item);
                            break;
                        case 1:
                            var trainitem = db.FindEntity<MeasuresEntity>(item.Id);
                            if (trainitem != null)
                            {
                                trainitem.Measure = item.Measure;
                                trainitem.DangerSource = item.DangerSource;
                                db.Update(trainitem);
                            }
                            break;
                        case 2:
                            db.Delete<MeasuresEntity>(item.Id);
                            break;
                        default:
                            break;
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void FinishTraining(DangerEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity1 = db.FindEntity<DangerEntity>(entity.Id);
                if (entity1.Status == 2) throw new TrainingExcuteException("危险预知训练已结束，无法再提交");

                entity1.Status = 2;

                db.Update(entity1);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public IList<DangerTemplateEntity> GetDangerous(string key, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = (from q in db.IQueryable<DangerTemplateEntity>()
                         where q.Dangerous.Contains(key)
                         select new { q.Dangerous, q.Measure }).Distinct();
            total = query.Count();
            return query.OrderBy(x => x.Dangerous).Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList().Select(x => new DangerTemplateEntity() { Dangerous = x.Dangerous, Measure = x.Measure }).ToList();
        }

        public IList<DangerTemplateEntity> GetMeasures(string key, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = (from q in db.IQueryable<DangerTemplateEntity>()
                         where q.Dangerous.Contains(key)
                         select new { q.Dangerous, q.Measure }).Distinct();
            total = query.Count();
            return query.OrderBy(x => x.Dangerous).Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList().Select(x => new DangerTemplateEntity() { Dangerous = x.Dangerous, Measure = x.Measure }).ToList();
        }

        public void BeginTraining(DangerEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity1 = db.FindEntity<DangerEntity>(entity.Id);
                if (entity1.Status == 2) throw new TrainingExcuteException("危险预知训练已结束，无法再提交");
                if (entity1.Status == 1) throw new TrainingExcuteException("危险预知训练已开始");

                entity1.Status = 1;

                db.Update(entity1);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public IList<DangerEntity> GetTrainings(string deptid, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<DangerEntity>()
                        join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.MeetingJobId into into1
                        from q1 in into1.DefaultIfEmpty()
                        where q.GroupId == deptid && (q.Status == 0 || q.Status == 1)
                        select new { q, q1 };

            total = query.Count();
            var data = query.OrderBy(x => x.q.CreateDate).Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList();
            var list = new List<DangerEntity>();

            foreach (var item in data)
            {
                var daner = item.q;
                daner.TrainingItems = db.IQueryable<MeasuresEntity>().Where(x => x.DangerId == daner.Id).ToList();
                daner.JobUsers = (from q1 in db.IQueryable<JobUserEntity>()
                                  join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                  where q2.MeetingJobId == daner.JobId
                                  select q1).ToList();
                daner.Files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == daner.Id).ToList();
                var checker = daner.JobUsers.Find(x => x.JobType == "ischecker");
                if (checker != null)
                {
                    var user = db.IQueryable<PeopleEntity>().FirstOrDefault(x => x.ID == checker.UserId);
                    if (user != null) daner.Photo = string.IsNullOrEmpty(user.Photo) ? null : user.Photo;
                }
                daner.FromJob = item.q1 == null ? false : item.q1.StartMeetingId != null;
                list.Add(daner);
            }

            return list;
        }

        public IList<DangerEntity> GetTrainingsApp(string deptid, DateTime fromtime, DateTime to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            to = to.AddDays(1);
            var query = from q in db.IQueryable<DangerEntity>()
                        where deptid.Contains(q.GroupId) && (q.Status == 0 || q.Status == 1)
                        && q.JobTime >= fromtime && q.JobTime <= to
                        select q;


            return query.ToList();
        }
        public IList<DangerEntity> GetTrainingsDo(string deptid, DateTime fromtime, DateTime to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            to = to.AddDays(1);
            var query = from q in db.IQueryable<DangerEntity>()
                        where deptid.Contains(q.GroupId) 
                        && q.JobTime >= fromtime && q.JobTime <= to
                        select q;


            return query.ToList();
        }
        public DangerEntity FinishTraining2(DangerEntity training)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (string.IsNullOrEmpty(training.Id))
                {
                    training.Id = Guid.NewGuid().ToString();
                    training.Status = training.Status;
                    db.Insert(training);
                }
                else
                {
                    var entity = db.FindEntity<DangerEntity>(training.Id);
                    if (entity.Status == 2) throw new Exception("已结束");

                    entity.Status = training.Status;
                    entity.TicketId = training.TicketId;
                    entity.JobAddress = training.JobAddress;
                    entity.StopMeasure = training.StopMeasure;
                    entity.Measure = training.Measure;

                    foreach (var item in training.TrainingItems)
                    {
                        switch (item.State)
                        {
                            case 0:
                                item.Id = Guid.NewGuid().ToString();
                                item.DangerId = training.Id;
                                item.CreateDate = DateTime.Now;
                                db.Insert(item);
                                break;
                            case 1:
                                var measure = db.FindEntity<MeasuresEntity>(item.Id);
                                measure.UserId = item.UserId;
                                measure.DutyMan = item.DutyMan;
                                measure.Measure = item.Measure;
                                measure.DangerSource = item.DangerSource;
                                measure.IsOver = item.IsOver;
                                db.Update(measure);
                                break;
                            case 2:
                                db.Delete<MeasuresEntity>(item.Id);
                                break;
                            default:
                                break;
                        }
                    }

                    db.Update(entity);

                }
                db.Commit();

                training.TrainingItems = training.TrainingItems.Where(x => x.State != 2).ToList();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

            return training;
        }


        public List<DangerEntity> FindTrainings(string key, int limit)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<JobTemplateEntity>()
                        join q2 in db.IQueryable<DangerTemplateEntity>() on q1.JobId equals q2.JobId into into1
                        where q1.DangerType == "job" && q1.JobType == "danger" && q1.JobContent.Contains(key)
                        orderby q1.CreateDate
                        select new { q1.JobId, q1.JobContent, q1.DeptCode, items = into1 };

            var data = query.Take(limit).ToList();

            return data.Select(x => new DangerEntity() { Id = x.JobId, DeptCode = x.DeptCode, JobName = x.JobContent, TrainingItems = x.items.Select(y => new MeasuresEntity() { Measure = y.Measure, DangerSource = y.Dangerous }).ToList() }
                ).ToList();
        }


        public DangerEntity AddTraining(DangerEntity training)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var user = db.FindEntity<UserEntity>(training.CreateUserId);
                var dept = db.FindEntity<DepartmentEntity>(user.DepartmentId);

                training.Id = Guid.NewGuid().ToString();
                training.GroupId = dept.DepartmentId;
                training.GroupName = dept.FullName;
                training.DeptCode = dept.EnCode;
                training.JobTime = DateTime.Now;

                if (training.TrainingItems == null)
                    training.TrainingItems = new List<MeasuresEntity>();
                else
                {
                    foreach (var item in training.TrainingItems)
                    {
                        item.Id = Guid.NewGuid().ToString();
                        item.DangerId = training.Id;
                        item.CreateDate = DateTime.Now;
                        item.CreateUserId = training.CreateUserId;
                    }
                }
                if (!string.IsNullOrEmpty(training.TemplateId))
                {
                    var template = (from q1 in db.IQueryable<JobTemplateEntity>()
                                    join q2 in db.IQueryable<DangerTemplateEntity>() on q1.JobId equals q2.JobId into into1
                                    where q1.JobId == training.TemplateId
                                    select new { q1, into1 }).FirstOrDefault();

                    if (template.q1.JobContent == training.JobName)
                    {
                        if (training.TrainingItems == null) training.TrainingItems = new List<MeasuresEntity>();
                        training.TrainingItems.AddRange(template.into1.Select(x => new MeasuresEntity() { Id = Guid.NewGuid().ToString(), Measure = x.Measure, DangerSource = x.Dangerous, DangerId = training.Id, CreateDate = DateTime.Now, CreateUserId = training.CreateUserId }));
                    }
                }

                var job = new MeetingJobEntity()
                {
                    JobId = Guid.NewGuid().ToString(),
                    CreateUserId = training.CreateUserId,
                    CreateDate = training.CreateDate.Value
                };
                job.Relation = new MeetingAndJobEntity()
                {
                    MeetingJobId = Guid.NewGuid().ToString(),
                    JobId = job.JobId,
                    IsFinished = "undo"
                };

                var jobusers = training.JobUsers;
                foreach (var item in jobusers)
                {
                    item.MeetingJobId = job.Relation.MeetingJobId;
                    item.JobUserId = Guid.NewGuid().ToString();
                    item.CreateDate = training.CreateDate.Value;
                }

                job.Relation.JobUsers = jobusers;
                job.Relation.JobUserId = string.Join(",", jobusers.Select(x => x.UserId));
                job.Relation.JobUser = string.Join(",", jobusers.Select(x => x.UserName));
                training.JobUsers = null;
                training.JobId = job.Relation.MeetingJobId;
                training.JobUser = string.Join(",", jobusers.Select(x => x.UserName));

                db.Insert(training);
                if (training.TrainingItems != null)
                    db.Insert(training.TrainingItems);
                db.Insert(job);
                db.Insert(job.Relation);
                db.Insert(jobusers);

                training.JobUsers = jobusers;

                db.Commit();

                return training;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }


        public void TrainingScore(DangerEntity dangerEntity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity = db.FindEntity<DangerEntity>(dangerEntity.Id);

                entity.Score = dangerEntity.Score;
                entity.ScoreRemark = dangerEntity.ScoreRemark;
                entity.OperUserId = dangerEntity.OperUserId;
                entity.OperDate = dangerEntity.OperDate;

                db.Update(entity);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }


        public List<DangerEntity> GetTrainingData(string userid, DateTime from, DateTime to, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var user = (from q in db.IQueryable<UserEntity>()
                        where q.UserId == userid
                        select q).FirstOrDefault();

            if (user == null)
            {
                total = 0;
                return new List<DangerEntity>();
            }

            var query = from q1 in db.IQueryable<DangerEntity>()
                        join q2 in db.IQueryable<FileInfoEntity>() on q1.Id equals q2.RecId into into1
                        where q1.GroupId == user.DepartmentId && q1.JobTime >= @from && q1.JobTime <= to && q1.Status == 2
                        orderby q1.JobTime descending
                        select new { q1.Id, q1.JobName, q1.JobTime, files = into1.Where(x => x.Description == "照片").ToList() };

            total = query.Count();
            var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return data.Select(x => new DangerEntity() { Id = x.Id, JobName = x.JobName, JobTime = x.JobTime, Files = x.files.ToList() }).ToList();
        }

        public void DeleteItem(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                db.Delete<MeasuresEntity>(id);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public string UpdateTraingItems(string dangerid, string jobid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var itemquery = from q in db.IQueryable<MeasuresEntity>()
                                where q.DangerId == dangerid
                                select q;

                var items = itemquery.ToList();

                db.Delete(items);

                var templatequery = from q in db.IQueryable<DangerTemplateEntity>()
                                    where q.JobId == jobid
                                    select q;

                var templates = templatequery.ToList();

                var trainitems = templates.Select(x => new MeasuresEntity() { Id = Guid.NewGuid().ToString(), DangerId = dangerid, DangerSource = x.Dangerous, Measure = x.Measure, IsOver = "0", CreateDate = DateTime.Now }).ToList();
                db.Insert(trainitems);

                db.Commit();

                return dangerid;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public MeasuresEntity UpdateTrainingPerson(string itemid, string userid, string users)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var trainingitem = db.FindEntity<MeasuresEntity>(itemid);
                trainingitem.DutyMan = users;
                trainingitem.UserId = userid;

                db.Update(trainingitem);

                db.Commit();

                return trainingitem;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public MeasuresEntity UpdateTrainingState(string itemid, bool isover)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var trainingitem = db.FindEntity<MeasuresEntity>(itemid);
                trainingitem.IsOver = isover ? "1" : "0";
                db.Update(trainingitem);

                db.Commit();

                return trainingitem;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public List<DangerEntity> GetRecords(string deptid, string name, DateTime? from, DateTime? to, int pagesize, int pageindex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<DangerEntity>()
                        join q1 in db.IQueryable<FileInfoEntity>() on q.Id equals q1.RecId into into1
                        where q.Status == 2
                        select new { q.Id, q.GroupId, q.JobName, q.JobTime, files = into1.Where(x => x.Description == "照片") };

            if (!string.IsNullOrEmpty(deptid)) query = query.Where(x => x.GroupId == deptid);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.JobName.Contains(name));
            if (from != null) query = query.Where(x => x.JobTime >= from.Value);
            if (to != null)
            {
                var too = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.JobTime <= too);
            }

            total = query.Count();

            var data = query.OrderByDescending(x => x.JobTime).Skip(pagesize * pageindex - pagesize).Take(pagesize).ToList();

            return data.Select(x => new DangerEntity() { Id = x.Id, JobName = x.JobName, JobTime = x.JobTime, Files = x.files.ToList() }).ToList();
        }

        public DataTable GetDangerPageList(string userid, Pagination pagination, string queryJson)
        {
            DatabaseType dataType = DbHelper.DbType;
            //Operator user = OperatorProvider.Provider.Current();
            var queryParam = queryJson.ToJObject();

            //查询条件
            //if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            //{
            //    string condition = queryParam["condition"].ToString();
            //    string keyord = queryParam["keyword"].ToString();

            //    switch (condition)
            //    {
            //        case "jobname":            //账户
            //            pagination.conditionJson += string.Format(" and jobname  like '%{0}%'", keyord);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //pagination.conditionJson += string.Format(" and deptcode  like '{0}%'", user.DeptCode);
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyord = queryParam["keyword"].ToString();
                pagination.conditionJson += string.Format(" and jobname  like '%{0}%'", keyord);
            }
            if (!queryParam["code"].IsEmpty())
            {
                string code = queryParam["code"].ToString();
                pagination.conditionJson += string.Format(" and deptcode  like '{0}%'", code);
            }
            if (!queryParam["from"].IsEmpty())
            {
                var from = queryParam["from"].ToString();
                pagination.conditionJson += string.Format(" and jobtime >= '{0}'", from);
            }
            if (!queryParam["to"].IsEmpty())
            {
                var to = queryParam["to"].ToString();
                var t = Convert.ToDateTime(to).AddDays(1).ToString();
                pagination.conditionJson += string.Format(" and jobtime < '{0}'", t);
            }
            if (!queryParam["appraise"].IsEmpty())
            {
                var appraise = queryParam["appraise"].ToString();
                if (appraise == "2")  //未评价
                {
                    //pagination.conditionJson += string.Format(" and appraisecontent is null");
                    pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate d where d.evaluateid = '{0}' and d.activityid = a.id)=0 ", userid);
                }
                else if (appraise == "1") //已评价
                {
                    //pagination.conditionJson += string.Format(" and appraisecontent is not null");
                    pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate d where d.evaluateid = '{0}' and d.activityid = a.id)>0 ", userid);
                }
            }

            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

            return dt;
        }

        public DataTable GetDangerJsonNew(Pagination pagination, string queryJson, string name, string from, string to, string userid)
        {
            DatabaseType dataType = DbHelper.DbType;
            IRepository db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var query = from a in db.IQueryable<DangerEntity>()
                        where a.Status == 2 && a.GroupName == name
                        select new
                        {
                            id = a.Id,
                            jobname = a.JobName,
                            jobuser = a.JobUser,
                            groupname = a.GroupName,
                            jobtime = a.JobTime,
                            status = a.Status,
                            scoreremark = a.ScoreRemark,
                            createdate = a.CreateDate,
                            deptcode = a.DeptCode,
                            score = a.Score,
                            operdate = a.OperDate,
                            appraisecontent = a.AppraiseContent,
                            ticketid = a.TicketId,
                            jobid = a.JobId
                        };

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyord = queryParam["keyword"].ToString();
                query = query.Where(x => x.jobname.Contains(keyord));
            }
            if (!queryParam["code"].IsEmpty())
            {
                string code = queryParam["code"].ToString();
                query = query.Where(x => x.deptcode.StartsWith(code));
            }

            if (!queryParam["from"].IsEmpty())
            {
                var start = Convert.ToDateTime(queryParam["from"].ToString());
                query = query.Where(x => x.jobtime >= start);
            }
            if (!queryParam["to"].IsEmpty())
            {
                var end = queryParam["to"].ToString();
                var t = Convert.ToDateTime(end).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.jobtime <= t);
            }
            if (!queryParam["appraise"].IsEmpty())
            {
                var appraise = queryParam["appraise"].ToString();
                if (appraise == "2")  //未评价
                {
                    query = from a in query
                            join b in db.IQueryable<ActivityEvaluateEntity>() on a.id equals b.Activityid
                            into t1
                            from tb1 in t1.DefaultIfEmpty()
                            where tb1.EvaluateId == null || tb1.EvaluateId != userid
                            select a;
                }
                else if (appraise == "1") //已评价
                {
                    query = from a in query
                            join b in db.IQueryable<ActivityEvaluateEntity>() on a.id equals b.Activityid
                            where b.EvaluateId == userid
                            select a;
                }
            }
            var data = query.OrderByDescending(x => x.createdate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            pagination.records = query.Count();
            var queryTalbe = DataHelper.ConvertToTable(data);
            return queryTalbe;
            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

            //return dt;
        }


        public DataTable GetDangerJson(Pagination pagination, string queryJson, string type, string[] depts, string userid)
        {
            //DatabaseType dataType = DbHelper.DbType;
            IRepository db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var query = from a in db.IQueryable<DangerEntity>()
                        where a.Status == 2 && depts.Contains(a.GroupId)
                        select new
                        {
                            id = a.Id,
                            jobname = a.JobName,
                            jobuser = a.JobUser,
                            groupname = a.GroupName,
                            jobtime = a.JobTime,
                            status = a.Status,
                            scoreremark = a.ScoreRemark,
                            createdate = a.CreateDate,
                            deptcode = a.DeptCode,
                            score = a.Score,
                            operdate = a.OperDate,
                            appraisecontent = a.AppraiseContent,
                            ticketid = a.TicketId,
                            jobid = a.JobId
                        };

            if (type == "undo")  //主页点击，只查询未完成的
            {
                query = query.Where(x => x.status != 2);
                // pagination.conditionJson += " and status != '2'";
                if (string.IsNullOrEmpty(queryJson)) //queryJson包含时间查询，所以只再首次加载时，筛选本季度
                {
                    int month = 1;
                    if (DateTime.Now.Month < 4) month = 1;
                    else if (DateTime.Now.Month < 7) month = 4;
                    else if (DateTime.Now.Month < 10) month = 7;
                    else if (DateTime.Now.Month <= 12) month = 10;
                    var sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
                    var edt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddMinutes(-1);
                    query = query.Where(x => x.jobtime >= sdt && x.jobtime < edt);
                    //pagination.conditionJson += string.Format(" and jobtime >= '{0}' and jobtime < '{1} 23:59:59'", sdt, edt);
                }
            }
            else
            {
                query = query.Where(x => x.status == 2);
                //pagination.conditionJson += " and status = '2'";
                if (type == "4") //首页工作台账，本月数据 
                {
                    if (string.IsNullOrEmpty(queryJson)) //queryJson包含时间查询，所以只再首次加载时，筛选本季度
                    {
                        int month = 1;
                        if (DateTime.Now.Month < 4) month = 1;
                        else if (DateTime.Now.Month < 7) month = 4;
                        else if (DateTime.Now.Month < 10) month = 7;
                        else if (DateTime.Now.Month <= 12) month = 10;
                        var sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
                        var edt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddMinutes(-1);
                        query = query.Where(x => x.jobtime >= sdt && x.jobtime < edt);
                        // pagination.conditionJson += string.Format(" and jobtime >= '{0}' and jobtime < '{1}'", sdt, edt);
                    }
                }
                else if (string.IsNullOrEmpty(queryJson))
                {
                    var sdt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);  //当前季度开始日期
                    var edt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddMinutes(-1);
                    query = query.Where(x => x.jobtime >= sdt && x.jobtime < edt);
                    // pagination.conditionJson += string.Format(" and jobtime >= '{0}' and jobtime < '{1}'", sdt, edt);
                }

            }


            if (!queryParam["keyword"].IsEmpty())
            {
                string keyord = queryParam["keyword"].ToString();
                query = query.Where(x => x.jobname.Contains(keyord));
            }
            if (!queryParam["code"].IsEmpty())
            {
                string code = queryParam["code"].ToString();
                query = query.Where(x => x.deptcode.StartsWith(code));
            }

            if (!queryParam["from"].IsEmpty())
            {
                var start = Convert.ToDateTime(queryParam["from"].ToString());
                query = query.Where(x => x.jobtime >= start);
            }
            if (!queryParam["to"].IsEmpty())
            {
                var end = queryParam["to"].ToString();
                var t = Convert.ToDateTime(end).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.jobtime <= t);
            }
            if (!queryParam["appraise"].IsEmpty())
            {
                var appraise = queryParam["appraise"].ToString();
                if (appraise == "2")  //未评价
                {
                    query = from a in query
                            join b in db.IQueryable<ActivityEvaluateEntity>() on a.id equals b.Activityid
                            into t1
                            from tb1 in t1.DefaultIfEmpty()
                            where tb1.EvaluateId == null || tb1.EvaluateId != userid
                            select a;
                }
                else if (appraise == "1") //已评价
                {
                    query = from a in query
                            join b in db.IQueryable<ActivityEvaluateEntity>() on a.id equals b.Activityid
                            where b.EvaluateId == userid
                            select a;
                }
            }
            var data = query.OrderByDescending(x => x.createdate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            pagination.records = query.Count();
            var queryTalbe = DataHelper.ConvertToTable(data);
            return queryTalbe;
            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

            //return dt;
        }
        private class newCount
        {
            public String Name { get; set; }
            public Int32 Count { get; set; }
            public String Code { get; set; }
        }
        public string GetCount(string deptid, DateTime f, DateTime t)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from item in (
                        from a in db.IQueryable<DepartmentEntity>()
                        join b in db.IQueryable<DangerEntity>() on a.EnCode equals b.DeptCode
                        into t1
                        from tb1 in t1.DefaultIfEmpty()
                        where tb1.JobTime >= f && tb1.JobTime < t
                        select new { a.DepartmentId, a.FullName, a.EnCode })
                        group item by item.DepartmentId into tb2
                        select new { name = tb2.Max(x=>x.FullName), count = tb2.Count(), code = tb2.Max(x=>x.EnCode) };
            var dt = DataHelper.ConvertToTable(query);

            //string from = f.ToString("yyyy-MM-dd");
            //string to = t.ToString("yyyy-MM-dd");
            //string sql = string.Format(" select a.fullname,count(b.id),a.encode from base_department a left join wg_danger b on a.encode = b.deptcode and b.jobtime >= '{0}' and b.jobtime < '{1}' and status = '2' where a.nature = '班组'group by a.departmentid ", from, to);
            //DataTable dt = this.BaseRepository().FindTable(sql);

            List<newCount> clist = new List<newCount>();
            var c = new newCount();
            foreach (DataRow row in dt.Rows)
            {
                c = new newCount();
                c.Name = row[0].ToString();
                c.Count = Convert.ToInt32(row[1]);
                c.Code = row[2].ToString();
                clist.Add(c);
            }
            //排序字段
            string[] property = new string[] { "Code", "Count" };
            //对应排序字段的排序方式
            bool[] sort = new bool[] { false, false };

            //对 List 排序
            clist = new IListSort<newCount>(clist, property, sort).Sort().ToList();

            string r = string.Empty;
            foreach (newCount o in clist)
            {
                r += "{" + string.Format("category:'{0}',value:'{1}'", o.Name, o.Count) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            return r;
        }
        public List<DangerEntity> GetRecordsApp(string username, string name, DateTime? from, DateTime? to, int pagesize, int pageindex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            //var query = from q in db.IQueryable<DangerEntity>()
            //            join q1 in db.IQueryable<FileInfoEntity>() on q.Id equals q1.RecId into into1
            //            where q.Status == 2
            //            select new { q.Id, q.GroupId, q.JobName, q.JobTime,q.OperDate, files = into1.Where(x => x.Description == "照片") };
            var query = from q in db.IQueryable<DangerEntity>()
                        where q.Status == 2 & q.Persons.Contains(username)
                        select new { q.Id, q.GroupId, q.JobName, q.JobTime, q.OperDate };
            //if (!string.IsNullOrEmpty(username)) query = query.Where(x => x.GroupId == deptid);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.JobName.Contains(name));
            if (from != null) query = query.Where(x => x.JobTime >= from.Value);
            if (to != null)
            {
                var too = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.JobTime <= too);
            }

            total = query.Count();
            var data = query.OrderByDescending(x => x.JobTime).ToList();
            if (pagesize > 0 && pageindex > 0)
            {
                data = data.Skip(pagesize * pageindex - pagesize).Take(pagesize).ToList();
            }

            return data.Select(x => new DangerEntity() { Id = x.Id, JobName = x.JobName, OperDate = x.OperDate, JobTime = x.JobTime }).ToList();
        }

        public DangerEntity GetTraining(string id, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var dataquery = from q in db.IQueryable<DangerEntity>()
                            where q.Id == id
                            select q;
            var data = dataquery.FirstOrDefault();

            var userquery = from q in db.IQueryable<JobUserEntity>()
                            where q.MeetingJobId == data.JobId
                            select q;
            data.JobUsers = userquery.ToList();

            var measurequery = from q in db.IQueryable<MeasuresEntity>()
                               where q.DangerId == data.Id
                               orderby q.CreateDate
                               select q;
            data.TrainingItems = measurequery.ToList();

            var jobuser = data.JobUsers.Find(x => x.UserId == userid);
            if (jobuser != null)
            {
                if (jobuser.JobType == "isdoperson")
                {
                    var filequery = from q in db.IQueryable<FileInfoEntity>()
                                    where q.RecId == id && q.CreateUserId == userid
                                    orderby q.CreateDate
                                    select q;

                    data.Files = filequery.ToList();
                }
                else
                {
                    var filequery = from q in db.IQueryable<FileInfoEntity>()
                                    where q.RecId == id
                                    orderby q.CreateDate
                                    select q;

                    data.Files = filequery.ToList();
                }
            }

            return data;
        }

        public List<DangerEntity> GetUserTrainings(string userid, string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var dataquery = from q1 in db.IQueryable<DangerEntity>()
                            join q2 in db.IQueryable<JobUserEntity>() on q1.JobId equals q2.MeetingJobId
                            join q3 in db.IQueryable<JobUserEntity>() on q1.JobId equals q3.MeetingJobId into into1
                            join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q4.MeetingJobId into into2
                            from q4 in into2.DefaultIfEmpty()
                            where (q1.Status == 0 || q1.Status == 1) && q2.UserId == userid
                            select new { q1.Id, q1.JobName, q1.CreateUserId, q1.Status, into1, q4 };
            if (!string.IsNullOrEmpty(id)) dataquery = dataquery.Where(x => x.Id == id);
            var data = dataquery.ToList();

            var result = new List<DangerEntity>();

            foreach (var item in data)
            {
                var entity = new DangerEntity() { Id = item.Id, JobName = item.JobName, CreateUserId = item.CreateUserId, FromJob = item.q4 == null ? false : item.q4.StartMeetingId != null, Status = item.Status, JobUsers = item.into1.OrderBy(x => x.UserName).ToList() };
                var jobuser = item.into1.FirstOrDefault(x => x.UserId == userid);
                switch (item.Status)
                {
                    case 0:
                        if (item.into1.Count(x => x.DangerStatus > 1) > 0)
                        {
                            if (jobuser.JobType == "ischecker")
                                entity.StatusDescription = "待确认";
                            else
                                entity.StatusDescription = "待负责人确认";
                        }
                        break;
                    case 1:
                        if (item.into1.Count(x => x.DangerStatus > 2) > 0)
                        {
                            if (jobuser.JobType == "ischecker") { }
                            //entity.StatusDescription = "待结束";
                            else
                                entity.StatusDescription = "已确认";
                        }
                        if (item.into1.Count(x => x.DangerStatus > 3) > 0)
                        {
                            if (jobuser.JobType == "ischecker")
                                entity.StatusDescription = "待结束";
                            else
                                entity.StatusDescription = "待负责人结束";
                        }
                        break;
                    default:
                        break;
                }
                result.Add(entity);
            }

            return result;
        }

        public List<MeasuresEntity> UpdateTraingingItem(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var newitems = danger.TrainingItems.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                foreach (var item in newitems)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.DangerId = danger.Id;
                    item.CreateDate = DateTime.Now;
                    item.CreateUserId = userid;
                }
                db.Insert(newitems);

                var deleteitems = danger.TrainingItems.Where(x => x.State == 2).Select(x => x.Id);
                var deletedata = (from q in db.IQueryable<MeasuresEntity>()
                                  where deleteitems.Any(x => q.Id == x)
                                  select q).ToList();
                db.Delete<MeasuresEntity>(deletedata);

                var updateitems = danger.TrainingItems.Where(x => !string.IsNullOrEmpty(x.Id) && x.State != 2);
                var ids = updateitems.Select(x => x.Id);
                var dangeritems = (from q in db.IQueryable<MeasuresEntity>()
                                   where q.DangerId == danger.Id && ids.Any(x => x == q.Id)
                                   select q).ToList();

                foreach (var item in dangeritems)
                {
                    var sourceitem = updateitems.FirstOrDefault(x => x.Id == item.Id);
                    if (sourceitem != null)
                    {
                        item.DangerSource = sourceitem.DangerSource;
                        item.Measure = sourceitem.Measure;
                    }
                }
                db.Update(dangeritems);

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 1;
                    db.Update(jobuser);
                }

                db.Commit();

                return danger.TrainingItems;

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<MeasuresEntity> SubmitTraingingItem(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (danger.TrainingItems != null)
                {
                    var newitems = danger.TrainingItems.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                    foreach (var item in newitems)
                    {
                        item.Id = Guid.NewGuid().ToString();
                        item.DangerId = danger.Id;
                        item.CreateDate = DateTime.Now;
                        item.CreateUserId = userid;
                    }
                    db.Insert(newitems);

                    var deleteitems = danger.TrainingItems.Where(x => x.State == 2).Select(x => x.Id);
                    var deletedata = (from q in db.IQueryable<MeasuresEntity>()
                                      where deleteitems.Any(x => q.Id == x)
                                      select q).ToList();
                    db.Delete<MeasuresEntity>(deletedata);

                    var updateitems = danger.TrainingItems.Where(x => !string.IsNullOrEmpty(x.Id) && x.State != 2);
                    var ids = updateitems.Select(x => x.Id);
                    var dangeritems = (from q in db.IQueryable<MeasuresEntity>()
                                       where q.DangerId == danger.Id && ids.Any(x => x == q.Id)
                                       select q).ToList();

                    foreach (var item in dangeritems)
                    {
                        var sourceitem = updateitems.FirstOrDefault(x => x.Id == item.Id);
                        if (sourceitem != null)
                        {
                            item.DangerSource = sourceitem.DangerSource;
                            item.Measure = sourceitem.Measure;
                        }
                    }
                    db.Update(dangeritems);
                }

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 2;
                    db.Update(jobuser);
                }

                db.Commit();

                return danger.TrainingItems;

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public DangerEntity UpdateTraingingItem2(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataquery = from q in db.IQueryable<DangerEntity>()
                                where q.Id == danger.Id
                                select q;
                var data = dataquery.FirstOrDefault();
                if (data == null) return null;

                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var deleteitems = danger.TrainingItems.Where(x => x.State == 2).Select(x => x.Id);
                var deletedata = (from q in db.IQueryable<MeasuresEntity>()
                                  where deleteitems.Any(x => q.Id == x)
                                  select q).ToList();
                db.Delete<MeasuresEntity>(deletedata);

                var newitems = danger.TrainingItems.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                foreach (var item in newitems)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.DangerId = data.Id;
                    item.CreateDate = DateTime.Now;
                    item.CreateUserId = userid;
                }
                db.Insert(newitems);

                var updateitems = danger.TrainingItems.Where(x => !string.IsNullOrEmpty(x.Id) && x.State != 2).ToList();
                var ids = updateitems.Select(x => x.Id);
                var dangeritems = (from q in db.IQueryable<MeasuresEntity>()
                                   where q.DangerId == data.Id && ids.Any(x => x == q.Id)
                                   select q).ToList();

                foreach (var item in dangeritems)
                {
                    var sourceitem = updateitems.FirstOrDefault(x => x.Id == item.Id);
                    if (sourceitem != null)
                    {
                        item.DangerSource = sourceitem.DangerSource;
                        item.Measure = sourceitem.Measure;
                        item.UserId = sourceitem.UserId;
                        item.DutyMan = sourceitem.DutyMan;
                    }
                }
                db.Update(dangeritems);

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 1;
                    db.Update(jobuser);
                }

                data.TicketId = danger.TicketId;
                data.JobAddress = danger.JobAddress;
                db.Update(data);

                db.Commit();

                data.TrainingItems = newitems.Concat(dangeritems).ToList();

                return data;

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public DangerEntity SubmitTraingingItem2(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataquery = from q in db.IQueryable<DangerEntity>()
                                where q.Id == danger.Id
                                select q;
                var data = dataquery.FirstOrDefault();
                if (data == null) return null;

                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var deleteitems = danger.TrainingItems.Where(x => x.State == 2).Select(x => x.Id);
                var deletedata = (from q in db.IQueryable<MeasuresEntity>()
                                  where deleteitems.Any(x => q.Id == x)
                                  select q).ToList();
                db.Delete<MeasuresEntity>(deletedata);

                var newitems = danger.TrainingItems.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
                foreach (var item in newitems)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.DangerId = data.Id;
                    item.CreateDate = DateTime.Now;
                    item.CreateUserId = userid;
                }
                db.Insert(newitems);

                var updateitems = danger.TrainingItems.Where(x => !string.IsNullOrEmpty(x.Id) && x.State != 2).ToList();
                var ids = updateitems.Select(x => x.Id);
                var dangeritems = (from q in db.IQueryable<MeasuresEntity>()
                                   where q.DangerId == data.Id && ids.Any(x => x == q.Id)
                                   select q).ToList();

                foreach (var item in dangeritems)
                {
                    var sourceitem = updateitems.FirstOrDefault(x => x.Id == item.Id);
                    if (sourceitem != null)
                    {
                        item.DangerSource = sourceitem.DangerSource;
                        item.Measure = sourceitem.Measure;
                        item.UserId = sourceitem.UserId;
                        item.DutyMan = sourceitem.DutyMan;
                    }
                }
                db.Update(dangeritems);

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 2;
                    db.Update(jobuser);
                }

                data.TicketId = danger.TicketId;
                data.JobAddress = danger.JobAddress;
                data.Status = 1;
                db.Update(data);

                db.Commit();

                data.TrainingItems = newitems.Concat(dangeritems).ToList();

                return data;

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 作业人训练时保存
        /// </summary>
        /// <returns></returns>
        public void DoTraining(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var itemids = danger.TrainingItems.Select(x => x.Id);

                var itemquery = from q in db.IQueryable<MeasuresEntity>()
                                where itemids.Any(x => x == q.Id)
                                select q;
                var items = itemquery.ToList();
                foreach (var item in items)
                {
                    var ss = danger.TrainingItems.Find(x => x.Id == item.Id);
                    item.IsOver = ss.IsOver;
                }
                db.Update(items);

                if (danger.Files == null) danger.Files = new List<FileInfoEntity>();
                var deletefiles = danger.Files.Where(x => x.State == 2).Select(x => x.FileId).ToArray();
                if (deletefiles.Length > 0)
                {
                    var deletedata = (from q in db.IQueryable<FileInfoEntity>()
                                      where deletefiles.Contains(q.FileId)
                                      select q).ToList();
                    db.Delete(deletedata);
                }

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 3;
                    db.Update(jobuser);
                }

                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 作业人训练时提交
        /// </summary>
        /// <param name="danger"></param>
        /// <param name="userid"></param>
        public void SubmitDoTraining(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var itemids = danger.TrainingItems.Select(x => x.Id);

                var itemquery = from q in db.IQueryable<MeasuresEntity>()
                                where itemids.Any(x => x == q.Id)
                                select q;
                var items = itemquery.ToList();
                foreach (var item in items)
                {
                    var ss = danger.TrainingItems.Find(x => x.Id == item.Id);
                    item.IsOver = ss.IsOver;
                }
                db.Update(items);

                if (danger.Files == null) danger.Files = new List<FileInfoEntity>();
                var deletefiles = danger.Files.Where(x => x.State == 2).Select(x => x.FileId).ToArray();
                if (deletefiles.Length > 0)
                {
                    var deletedata = (from q in db.IQueryable<FileInfoEntity>()
                                      where deletefiles.Contains(q.FileId)
                                      select q).ToList();
                    db.Delete(deletedata);
                }

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 4;
                    db.Update(jobuser);
                }

                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 负责人训练时保存
        /// </summary>
        /// <param name="danger"></param>
        /// <param name="userid"></param>
        public void DoTraining2(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var itemids = danger.TrainingItems.Select(x => x.Id);

                var itemquery = from q in db.IQueryable<MeasuresEntity>()
                                where itemids.Any(x => x == q.Id)
                                select q;
                var items = itemquery.ToList();
                foreach (var item in items)
                {
                    var ss = danger.TrainingItems.Find(x => x.Id == item.Id);
                    item.IsOver = ss.IsOver;
                }
                db.Update(items);

                if (danger.Files == null) danger.Files = new List<FileInfoEntity>();
                var deletefiles = danger.Files.Where(x => x.State == 2).Select(x => x.FileId).ToArray();
                if (deletefiles.Length > 0)
                {
                    var deletedata = (from q in db.IQueryable<FileInfoEntity>()
                                      where deletefiles.Contains(q.FileId)
                                      select q).ToList();
                    db.Delete(deletedata);
                }

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 3;
                    db.Update(jobuser);
                }

                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 负责人训练时提交
        /// </summary>
        /// <param name="danger"></param>
        /// <param name="userid"></param>
        public void SubmitDoTraining2(DangerEntity danger, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (danger.TrainingItems == null) danger.TrainingItems = new List<MeasuresEntity>();

                var dataquery = from q in db.IQueryable<DangerEntity>()
                                where q.Id == danger.Id
                                select q;
                var data = dataquery.FirstOrDefault();
                if (data == null) return;

                var itemids = danger.TrainingItems.Select(x => x.Id);
                var itemquery = from q in db.IQueryable<MeasuresEntity>()
                                where itemids.Any(x => x == q.Id)
                                select q;
                var items = itemquery.ToList();
                foreach (var item in items)
                {
                    var ss = danger.TrainingItems.Find(x => x.Id == item.Id);
                    item.IsOver = ss.IsOver;
                }
                db.Update(items);

                if (danger.Files == null) danger.Files = new List<FileInfoEntity>();
                var deletefiles = danger.Files.Where(x => x.State == 2).Select(x => x.FileId).ToArray();
                if (deletefiles.Length > 0)
                {
                    var deletedata = (from q in db.IQueryable<FileInfoEntity>()
                                      where deletefiles.Contains(q.FileId)
                                      select q).ToList();
                    db.Delete(deletedata);
                }

                var jobuser = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q2.JobId
                               where q1.UserId == userid && q2.Id == danger.Id
                               select q1).FirstOrDefault();
                if (jobuser != null)
                {
                    jobuser.DangerStatus = 4;
                    db.Update(jobuser);
                }
                data.Status = 2;
                db.Update(data);

                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<PeopleEntity> GetGroupTraining(int year, int month, string groupid)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            //IRepository<PeopleEntity> db = new RepositoryFactory<PeopleEntity>().BaseRepository();
            //var sql = string.Format(@"select a.name,a.id,count(job.jobid) as scores from wg_people a
            //left join (select b.userid,c.jobid from wg_danger c
            //join wg_jobuser b
            //on b.meetingjobid = c.jobid
            //where c.status='2'
            //and c.jobtime >= '{0}'
            //and c.jobtime < '{1}') as job
            //on a.id = job.userid
            //where a.bzid='{2}'
            //group by a.id
            //order by scores desc", from, to, groupid);

            //            return db.FindList(sql).ToList();

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from item in (
                                from a in db.IQueryable<PeopleEntity>()
                                join b in (from c in db.IQueryable<DangerEntity>()
                                           join d in db.IQueryable<JobUserEntity>() on c.JobId equals d.MeetingJobId
                                           where c.Status == 2 && c.JobTime >= @from && c.JobTime < to
                                           select new { d.UserId, c.JobId }
                                            ) on a.ID equals b.UserId into t1
                                from tb1 in t1.DefaultIfEmpty()
                                where a.BZID == groupid
                                select new { a.ID,a.Name,tb1 }
                        )
                        group item by item.ID into tb2
                        select new PeopleEntity()
                        {
                            ID = tb2.Max(x=>x.ID),
                            Name = tb2.Max(x => x.Name),
                            Scores =tb2.Count(x=>x.tb1!=null).ToString()
                        };
            return query.ToList();
         

        }

        public int GetTrainingTimes(string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DangerEntity>()
                        join q2 in db.IQueryable<JobUserEntity>() on q1.JobId equals q2.MeetingJobId
                        where q1.GroupId == deptid && q1.Status == 2
                        select q2;
            return query.Count();
        }

        public List<DangerEntity> GetHistory(string[] deptid, string key, DateTime? from, DateTime? to, bool? isEvaluate, string userid, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DangerEntity>()
                        join q2 in db.IQueryable<ActivityEvaluateEntity>() on q1.Id equals q2.Activityid into t2
                        where q1.Status != 0 && q1.Status != 1
                        select new { q1, q2 = t2 };

            if (deptid != null && deptid.Length > 0) query = query.Where(x => deptid.Contains(x.q1.GroupId));
            if (!string.IsNullOrEmpty(key)) query = query.Where(x => x.q1.JobName.Contains(key));
            if (from != null) query = query.Where(x => x.q1.JobTime >= from);
            if (to != null) query = query.Where(x => x.q1.JobTime <= to);
            if (isEvaluate != null)
            {
                if (isEvaluate.Value) query = query.Where(x => x.q2.Count(y => y.EvaluateId == userid) > 0);
                else query = query.Where(x => x.q2.Count() == 0);
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.q1.JobTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(userid)) item.q1.IsEvaluate = item.q2.Count(x => x.EvaluateId == userid) > 0;
            }
            return data.Select(x => x.q1).ToList();
        }

        public void Delete(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var query = from q in db.IQueryable<MeasuresEntity>()
                            where q.DangerId == id
                            select q;
                var measures = query.ToList();
                if (measures != null) db.Delete(measures);

                var entity = db.FindEntity<DangerEntity>(id);
                if (entity != null) db.Delete(entity);

                var meetandjob = db.FindEntity<MeetingAndJobEntity>(entity.JobId);
                if (meetandjob != null) db.Delete(meetandjob);

                var jobusers = (from q in db.IQueryable<JobUserEntity>()
                                where q.MeetingJobId == meetandjob.MeetingJobId
                                select q).ToList();

                var job = db.FindEntity<MeetingJobEntity>(meetandjob.JobId);
                var relations = (from q in db.IQueryable<MeetingAndJobEntity>()
                                 where q.JobId == job.Job
                                 select q).ToList();

                if (relations.Count == 0) db.Delete(job);

                var ctx = new DataContext();
                var todos = ctx.Messages.Where(x => x.MessageKey == "危险预知训练" && x.BusinessId == id).ToList();
                todos.ForEach(x => x.IsFinished = true);

                foreach (var item in jobusers)
                {
                    ctx.Messages.Add(new Message()
                    {
                        MessageId = Guid.NewGuid(),
                        BusinessId = id,
                        Content = string.Format("您有项危险预知训练任务已取消：{0}", entity.JobName),
                        Title = "危险预知训练取消",
                        UserId = item.UserId,
                        Category = MessageCategory.Message,
                        MessageKey = "危险预知训练取消",
                        CreateTime = DateTime.Now
                    });
                }
                ctx.SaveChanges();

                db.Delete(jobusers);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public List<DangerEntity> GetTimeCount(string deptid, DateTime f, DateTime t)
        {
            string from = f.ToString("yyyy-MM-dd");
            string to = t.ToString("yyyy-MM-dd");
//            string sql = string.Format(@" select  id as activityid,jobtime as starttime,operdate as endtime, deptcode,groupname as deptname,groupid as deptid,jobuser,persons FROM wg_danger 
//where deptcode like '{0}%' AND jobtime>='{1}' and jobtime<='{2}' and STATUS=2 ", deptid, from, to);
//            DataTable dt = this.BaseRepository().FindTable(sql);
//            return dt;
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<DangerEntity>()
                        where a.Status == 2 && a.DeptCode.StartsWith(deptid)
                        && a.JobTime <= t && a.JobTime >= f
                        select a;
            return query.ToList(); ;

        
        }

        public int Count(string[] depts, DateTime start, DateTime end)
        {
            var query = from q in _context.Set<DangerEntity>()
                        where depts.Contains(q.GroupId) && q.JobTime >= start && q.JobTime < end
                        select q;
            return query.Count();
        }
    }
}
