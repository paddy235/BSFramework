using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.MisManage;
using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Application.Service.CarcOrCardManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Application.Service.WebApp;
using BSFramework.Application.Service.WorkPlanManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using Bst.Bzzd.Sync;
using Bst.ServiceContract.MessageQueue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BSFramework.Service.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class WorkmeetingService : RepositoryFactory<WorkmeetingEntity>, WorkmeetingIService
    {
        private System.Data.Entity.DbContext _context;
        private DbSet<EduBaseInfoEntity> eduBaseInfos;
        private DbSet<MeetingSigninEntity> meetingSignins;
        private DbSet<WorkmeetingEntity> workMeetings;

        /// <summary>
        /// ctor
        /// </summary>
        public WorkmeetingService()
        {
            _context = (DbFactory.Base() as Data.EF.Database).dbcontext;
            workMeetings = _context.Set<WorkmeetingEntity>();
            eduBaseInfos = _context.Set<EduBaseInfoEntity>();
            meetingSignins = _context.Set<MeetingSigninEntity>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public WorkmeetingEntity PrepareWorkMeeting(string deptid, DateTime date)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<WorkmeetingEntity>()
                        where q.GroupId == deptid && q.MeetingType == "班前会"
                        orderby q.MeetingStartTime descending
                        select q;

            var meeting = default(WorkmeetingEntity);
            var last = query.FirstOrDefault();
            if (last == null)
            {
                meeting = this.BuildingStartMeeting(deptid, date);
                return meeting;
            }
            else
            {
                if (last.IsOver)
                {
                    var endmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                      where q.MeetingId == last.OtherMeetingId
                                      select q).FirstOrDefault();

                    if (endmeeting == null)
                        meeting = this.BuildingEndMeeting(last.MeetingId, date);
                    else
                    {
                        if (endmeeting.IsOver) meeting = this.BuildingStartMeeting(deptid, date);
                        else meeting = endmeeting;
                    }
                }
                else meeting = last;
            }

            return meeting;
        }
        public IEnumerable<WorkmeetingEntity> GetAllList(DateTime from, string code)
        {
            //var query = this.BaseRepository().IQueryable().Where(x => x.MeetingStartTime > from);

            //return query.OrderByDescending(x => x.MeetingStartTime).ToList();
            var db = new RepositoryFactory().BaseRepository();
            //            var query = this.BaseRepository();
            //            var sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_workmeeting a
            //left join wg_activityevaluate b on a.meetingid = b.activityid
            //where meetingstarttime > '{0}' and a.meetingtype='班后会' and a.groupid in(select departmentid from base_department where encode like '{1}%' and nature = '班组')
            //group by a.meetingid) a where total = 0", from, code);
            //            return query.FindList(sql);
            var deptIds = db.IQueryable<DepartmentEntity>(p => p.EnCode.StartsWith(code) && p.Nature == "班组").Select(x => x.DepartmentId).ToList();
            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<ActivityEvaluateEntity>() on q1.MeetingId equals q2.Activityid into t
                        from t1 in t.DefaultIfEmpty()
                        where q1.MeetingStartTime > @from && q1.MeetingType == "班后会" && deptIds.Contains(q1.GroupId)
                        group t1.ActivityEvaluateId by new { q1 } into g
                        select new
                        {
                            g.Key.q1.MeetingId,
                            g.Key.q1.MeetingType,
                            g.Key.q1.MeetingStartTime,
                            g.Key.q1.MeetingEndTime,
                            g.Key.q1.ShouldJoin,
                            g.Key.q1.ActuallyJoin,
                            g.Key.q1.Remark,
                            g.Key.q1.GroupId,
                            g.Key.q1.GroupName,
                            g.Key.q1.OtherMeetingId,
                            g.Key.q1.IsOver,
                            g.Key.q1.MeetingPerson,
                            g.Key.q1.PersonState,
                            g.Key.q1.IsStarted,
                            g.Key.q1.ShouldStartTime,
                            g.Key.q1.ShouldEndTime,
                            g.Key.q1.MeetingCode,
                            total = g.Count()
                        };
            var dataList = query.Where(x => x.total == 0).ToList().Select(x => new WorkmeetingEntity()
            {
                MeetingId = x.MeetingId,
                MeetingType = x.MeetingType,
                MeetingStartTime = x.MeetingStartTime,
                MeetingEndTime = x.MeetingEndTime,
                ShouldJoin = x.ShouldJoin,
                ActuallyJoin = x.ActuallyJoin,
                Remark = x.Remark,
                GroupId = x.GroupId,
                GroupName = x.GroupName,
                OtherMeetingId = x.OtherMeetingId,
                IsOver = x.IsOver,
                MeetingPerson = x.MeetingPerson,
                PersonState = x.PersonState,
                IsStarted = x.IsStarted,
                ShouldStartTime = x.ShouldStartTime,
                ShouldEndTime = x.ShouldEndTime,
                MeetingCode = x.MeetingCode,
            });
            return dataList;
        }
        private WorkmeetingEntity BuildingEndMeeting(string startmeetingid, DateTime date)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var startmeeting = db.IQueryable<WorkmeetingEntity>().FirstOrDefault(x => x.MeetingId == startmeetingid);
                var endmeeting = new WorkmeetingEntity() { MeetingId = Guid.NewGuid().ToString(), GroupId = startmeeting.GroupId, MeetingStartTime = date, MeetingEndTime = date, PersonState = "正常", MeetingType = "班后会", OtherMeetingId = startmeeting.MeetingId, ActuallyJoin = startmeeting.ActuallyJoin, ShouldJoin = startmeeting.ShouldJoin };
                startmeeting.OtherMeetingId = endmeeting.MeetingId;
                startmeeting.Jobs = null;
                startmeeting.Signins = null;
                startmeeting.Files = null;
                var startSignins = (from q in db.IQueryable<MeetingSigninEntity>()
                                    where q.MeetingId == startmeeting.MeetingId
                                    select q).ToList();
                var signins = startSignins.Select(x => new MeetingSigninEntity() { MeetingId = endmeeting.MeetingId, SigninId = Guid.NewGuid().ToString(), IsSigned = x.IsSigned, UserId = x.UserId, PersonName = x.PersonName, ClosingCondition = x.ClosingCondition, MentalCondition = x.MentalCondition, CreateDate = date }).ToList();
                var startJobs = (from q1 in db.IQueryable<MeetingJobEntity>()
                                 join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                 where q2.StartMeetingId == startmeeting.MeetingId
                                 select q2).ToList();
                var startJobUsers = (from q1 in db.IQueryable<JobUserEntity>()
                                     join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                     where q2.StartMeetingId == startmeeting.MeetingId
                                     select q1).ToList();

                foreach (var item in startJobs)
                {
                    item.EndMeetingId = endmeeting.MeetingId;
                }
                //
                //var jobs = new List<MeetingJobEntity>();
                //var jobusers = new List<JobUserEntity>();
                //foreach (var item in startJobs)
                //{
                //    var job = new MeetingJobEntity() { MeetingId = endmeeting.MeetingId, JobId = Guid.NewGuid().ToString(), Job = item.Job, UserId = item.UserId, JobUsers = item.JobUsers, StartTime = item.StartTime, EndTime = item.EndTime, Dangerous = item.Dangerous, Measure = item.Measure, IsFinished = "undo", TemplateId = item.TemplateId, CreateDate = DateTime.Now, NeedTrain = item.NeedTrain, GroupId = endmeeting.GroupId };
                //    jobusers.AddRange(startJobUsers.Where(x => x.JobId == item.JobId).Select(x => new JobUserEntity() { JobId = job.JobId, CreateDate = date, JobType = x.JobType, JobUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName }));
                //    jobs.Add(job);
                //}

                db.Update(startmeeting);
                db.Insert(endmeeting);
                db.Insert(signins);
                db.Update(startJobs);
                //db.Insert(jobs);
                //db.Insert(jobusers);

                db.Commit();
                return endmeeting;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }

        private WorkmeetingEntity BuildingStartMeeting(string deptid, DateTime date)
        {
            var dayofmonth = ',' + date.Day.ToString() + ',';


            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var meeting = new WorkmeetingEntity() { MeetingId = Guid.NewGuid().ToString(), GroupId = deptid, MeetingStartTime = date, MeetingEndTime = date, PersonState = "正常", MeetingType = "班前会" };
                var persons = (from q in db.IQueryable<PeopleEntity>()
                               where q.BZID == deptid && q.FingerMark == "yes"
                               select q).ToList();
                meeting.ShouldJoin = persons.Count;
                meeting.ActuallyJoin = persons.Count;

                var signins = persons.Select(x => new MeetingSigninEntity() { SigninId = Guid.NewGuid().ToString(), MeetingId = meeting.MeetingId, UserId = x.ID, PersonName = x.Name, IsSigned = true, MentalCondition = "正常", ClosingCondition = "正常", CreateDate = date }).ToList();

                var meetingjobs = new List<MeetingJobEntity>();
                var jobusers = new List<JobUserEntity>();

                var endmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                  where q.GroupId == deptid && q.MeetingType == "班后会"
                                  orderby q.MeetingStartTime descending
                                  select q).FirstOrDefault();
                // 取消任务结转
                //if (endmeeting != null && endmeeting.IsOver)
                //{
                //    var lastjobs = (from q in db.IQueryable<MeetingJobEntity>()
                //                    where q.EndMeetingId == endmeeting.MeetingId && q.IsFinished == "undo"
                //                    orderby q.StartTime
                //                    select q).ToList();

                //    var lastjobusers = (from q1 in db.IQueryable<JobUserEntity>()
                //                        join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                //                        where q2.EndMeetingId == endmeeting.MeetingId && q2.IsFinished == "undo"
                //                        select q1).ToList();

                //    foreach (var item in lastjobs)
                //    {
                //        var job = new MeetingJobEntity() { StartMeetingId = meeting.MeetingId, JobId = Guid.NewGuid().ToString(), Job = item.Job, UserId = item.UserId, JobUsers = item.JobUsers, StartTime = item.StartTime, EndTime = item.EndTime, Dangerous = item.Dangerous, Measure = item.Measure, IsFinished = "undo", TemplateId = item.TemplateId, CreateDate = DateTime.Now, NeedTrain = item.NeedTrain, GroupId = meeting.GroupId };
                //        jobusers.AddRange(lastjobusers.Where(x => x.JobId == item.JobId).Select(x => new JobUserEntity() { JobId = job.JobId, CreateDate = date, JobType = x.JobType, JobUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName }));
                //        meetingjobs.Add(job);
                //    }
                //}

                var templatesQuery = (from q in db.IQueryable<JobTemplateEntity>()
                                      where q.DangerType == "job" && q.DeptId == deptid
                                      orderby q.CreateDate
                                      select q).ToList();
                //任务库任务
                var templates = GetWorkDateJob(deptid, DateTime.Now);


                foreach (var item in templates)
                {
                    var users = new List<PeopleEntity>();
                    if (!string.IsNullOrEmpty(item.JobPerson))
                    {
                        users = persons.Where(x => item.JobPerson.Split(',').Contains(x.Name)).ToList();
                    }
                    if (!string.IsNullOrEmpty(item.otherperson))
                    {
                        var more = persons.Where(x => item.otherperson.Split(',').Contains(x.Name)).ToList();
                        users.AddRange(more);
                    }
                    var job = new MeetingJobEntity()
                    {
                        JobId = Guid.NewGuid().ToString(),
                        PlanId = item.JobId,
                        Job = item.JobContent,
                        StartTime = item.JobStartTime.HasValue ? item.JobStartTime.Value : new DateTime(date.Year, date.Month, date.Day, 8, 30, 0),
                        EndTime = item.JobEndTime.HasValue ? item.JobEndTime.Value : new DateTime(date.Year, date.Month, date.Day, 17, 30, 0),
                        Dangerous = item.Dangerous,
                        Measure = item.Measure,
                        IsFinished = "undo",
                        TemplateId = item.JobId,
                        CreateDate = DateTime.Now,
                        NeedTrain = item.EnableTraining,
                        GroupId = meeting.GroupId
                    };
                    job.Relation = new MeetingAndJobEntity() { MeetingJobId = Guid.NewGuid().ToString(), StartMeetingId = meeting.MeetingId, IsFinished = "undo", JobId = item.JobId };
                    job.Relation.JobUserId = string.Join(",", users.Select(x => x.ID));
                    job.Relation.JobUser = string.Join(",", users.Select(x => x.Name));
                    jobusers.AddRange(users.Select(x => new JobUserEntity() { CreateDate = date, JobType = "isdoperson", JobUserId = Guid.NewGuid().ToString(), UserId = x.ID, UserName = x.Name }));
                    meetingjobs.Add(job);
                }

                db.Insert(meeting);
                db.Insert(signins);
                db.Insert(meetingjobs);
                db.Insert(jobusers);

                db.Commit();
                return meeting;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }


        #region 获取数据
        public DataTable getExport(string content)
        {
            var query = new RepositoryFactory().BaseRepository().IQueryable<JobTemplateEntity>().Where(x => x.JobType == "danger");
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  JobContent,WorkQuarters, Usetime , RedactionDate , RedactionPerson 
            //                from wg_jobtemplate where 1=1 and jobtype='danger'");
            if (!string.IsNullOrEmpty(content))
            {
                query = query.Where(x => x.JobContent.Contains(content));
                //strSql.Append(" and JobContent like '%" + content + "%'");
            }

            //DataTable dt = new RepositoryFactory().BaseRepository().FindTable(strSql.ToString());
            var datalist = query.Select(x => new JobTemplateEntity()
            {
                JobContent = x.JobContent,
                WorkQuarters = x.WorkQuarters,
                Usetime = x.Usetime,
                RedactionDate = x.RedactionDate,
                RedactionPerson = x.RedactionPerson
            }).ToList();
            DataTable dt = new DataTable("wg_jobtemplate");
            dt.Columns.Add("JobContent");
            dt.Columns.Add("WorkQuarters");
            dt.Columns.Add("Usetime");
            dt.Columns.Add("RedactionDate");
            dt.Columns.Add("RedactionPerson");
            if (datalist != null && datalist.Count > 0)
            {
                datalist.ForEach(x =>
                {
                    dt.Rows.Add(new object[] { x.JobContent, x.WorkQuarters, x.Usetime, x.RedactionDate, x.RedactionPerson });
                });
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depts"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="isEvaluate"></param>
        /// <param name="userid"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IEnumerable<WorkmeetingEntity> GetList(string[] depts, DateTime? from, DateTime? to, bool? isEvaluate, string userid, int page, int pagesize, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                            //join q2 in db.IQueryable<WorkmeetingEntity>() on q1.MeetingId equals q2.OtherMeetingId
                        join q3 in db.IQueryable<ActivityEvaluateEntity>() on q1.OtherMeetingId equals q3.Activityid into t3
                        join q4 in db.IQueryable<FileInfoEntity>() on q1.MeetingId equals q4.RecId into t4
                        join q5 in db.IQueryable<FileInfoEntity>() on q1.OtherMeetingId equals q5.RecId into t5
                        where depts.Contains(q1.GroupId) && q1.MeetingType == "班前会" && q1.IsOver == true
                        select new { q1, q3 = t3, q4 = t4, q5 = t5 };

            if (from != null) query = query.Where(x => x.q1.MeetingStartTime >= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.q1.MeetingStartTime <= to);
            }
            if (isEvaluate != null)
            {
                if (isEvaluate.Value) query = query.Where(x => x.q3.Count(y => y.EvaluateId == userid) > 0);
                else query = query.Where(x => x.q3.Count(y => y.EvaluateId == userid) == 0);
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.q1.MeetingStartTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            foreach (var item in data)
            {
                if (userid != null)
                    item.q1.IsEvaluate = item.q3.Count(x => x.EvaluateId == userid) > 0;
                item.q1.Files = item.q4.Where(x => x.Description == "照片").OrderByDescending(x => x.CreateDate).ToList();
                var files = item.q5.Where(x => x.Description == "照片").OrderByDescending(x => x.CreateDate).ToList();
                if (files != null) item.q1.Files.AddRange(files);
            }
            return data.Select(x => x.q1).ToList();
        }

        public IEnumerable<WorkmeetingEntity> GetAllList()
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 获取班组活动列表
        /// </summary>
        /// <param name="page">请求页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="total">记录总数</param>
        /// <param name="status">状态（0：未开展，1：已结束，2：所有）</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns></returns>
        public System.Collections.IList GetList(int page, int pageSize, out int total, int status, string startTime, string endTime, string bzDepart)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(startTime))
            {
                DateTime t1 = DateTime.Parse(startTime);
                query = query.Where(x => x.MeetingStartTime >= t1);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime t1 = DateTime.Parse(endTime).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.MeetingStartTime <= t1);
            }
            if (status == 0)
            {
                query = query.Where(t => !t.IsOver);
            }
            if (status == 1)
            {
                query = query.Where(t => t.IsOver);
            }
            if (!string.IsNullOrEmpty(bzDepart))
            {
                query = query.Where(t => t.GroupId == bzDepart);
            }
            total = query.Count();
            return query.OrderByDescending(x => x.MeetingStartTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList().Select(t => new
            {
                t.MeetingId,
                PlanTime = t.MeetingStartTime.ToString("yyyy.MM.dd HH:mm") + "-" + t.MeetingEndTime.ToString("HH:mm"),
                MeetName = t.MeetingStartTime.ToString("MM月dd日") + t.MeetingType
            }).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        public WorkmeetingEntity GetDetail(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            IRepository db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<WorkmeetingEntity>(id);
            if (entity == null) return entity;

            var meetingid = entity.MeetingId;
            if (entity.MeetingType == "班后会")
                meetingid = entity.OtherMeetingId;

            var relations = (from q in db.IQueryable<MeetingAndJobEntity>()
                             join q1 in db.IQueryable<DangerEntity>() on q.MeetingJobId equals q1.JobId into into1
                             from q1 in into1.DefaultIfEmpty()
                             join q2 in db.IQueryable<MeetingJobEntity>() on q.JobId equals q2.JobId
                             join q3 in db.IQueryable<JobUserEntity>() on q.MeetingJobId equals q3.MeetingJobId into into2
                             join q4 in db.IQueryable<FileInfoEntity>() on q.MeetingJobId equals q4.RecId into into3
                             join q5 in (
                             from q1 in db.IQueryable<JobDangerousEntity>()
                             join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into1
                             select new { q1, q2 = into1 }
                             ) on q.JobId equals q5.q1.JobId into into4
                             join q6 in (
                             from q1 in db.IQueryable<HumanDangerTrainingBaseEntity>()
                             join q2 in db.IQueryable<HumanDangerTrainingUserEntity>() on q1.TrainingId equals q2.TrainingId into t2
                             select new { q1, q2 = t2 }
                             ) on q.MeetingJobId equals q6.q1.MeetingJobId into t6
                             from q6 in t6.DefaultIfEmpty()
                             where q.StartMeetingId == meetingid
                             orderby q2.CreateDate
                             select new { q, q1, q2, q3 = into2, q4 = into3, q5 = into4, q6 }).ToList();

            var signinquery = from q in db.IQueryable<MeetingSigninEntity>()
                              join q1 in db.IQueryable<PeopleEntity>() on q.UserId equals q1.ID
                              where q.MeetingId == entity.MeetingId
                              select new { q, q1.Planer };

            var filequery = from q in db.IQueryable<FileInfoEntity>()
                            where q.RecId == entity.MeetingId
                            select q;

            var start = entity.MeetingStartTime.Date;
            var end = entity.MeetingStartTime.Date.AddDays(1).AddMinutes(-1);

            var dutypersonquery = from q1 in db.IQueryable<UnSignRecordEntity>()
                                  join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                                  where q1.UnSignDate >= start && q1.UnSignDate <= end && q1.Reason == "值班" && q2.DepartmentId == entity.GroupId
                                  select q1;

            entity.Signins = signinquery.OrderBy(x => x.Planer).ToList().Select(x => x.q).ToList();
            entity.Files = filequery.OrderBy(x => x.CreateDate).ToList();
            entity.DutyPerson = dutypersonquery.ToList();
            entity.Jobs = new List<MeetingJobEntity>();

            foreach (var item in relations)
            {
                var job = item.q2;
                job.Relation = item.q;
                job.Relation.JobUsers = item.q3.ToList();
                job.Files = item.q4.ToList();
                job.DangerousList = new List<JobDangerousEntity>();
                foreach (var item2 in item.q5)
                {
                    job.DangerousList.Add(item2.q1);
                    item2.q1.MeasureList = item2.q2.ToList();
                }
                job.Dangerous = string.Join(",", job.DangerousList.Select(x => x.Content));
                job.Measure = string.Join(",", job.DangerousList.SelectMany(x => x.MeasureList).Select(x => x.Content));

                if (item.q6 != null)
                {
                    job.HumanDangerTraining = item.q6.q1;
                    if (item.q6.q2 != null)
                    {
                        job.HumanDangerTraining.TrainingUsers = item.q6.q2.ToList();
                        job.HumanDangerTraining.TrainingUsers.ForEach(x => x.Training = null);
                    }
                }

                if (item.q1 != null)
                {
                    job.Training = item.q1;
                    job.TrainingDone = item.q1.Status == 2;
                }
                else if (item.q6 != null)
                {
                    job.Training = new DangerEntity() { Id = item.q6.q2.First().TrainingUserId.ToString() };
                    job.TrainingDone = item.q6.q2.All(x => x.IsDone == true && x.IsMarked == true);
                }

                entity.Jobs.Add(job);
            }

            return entity;
        }

        private WorkmeetingEntity BuildStartMeeting(string deptid, DateTime date)
        {
            var dayofmonth = ',' + date.Day.ToString() + ',';


            IRepository db = new RepositoryFactory().BaseRepository();
            var date1 = DateTime.Parse((ConfigurationManager.AppSettings["StartDate"] ?? DateTime.Now.ToString()).ToString());
            var cnt = new TimeSpan(DateTime.Today.Ticks).Subtract(new TimeSpan(date1.Ticks)).Duration().Days;
            var model = new WorkmeetingEntity() { MeetingType = "班前会", MeetingStartTime = DateTime.Now, GroupId = deptid, PersonState = "正常" };
            // 取消任务结转
            //var last = db.IQueryable<WorkmeetingEntity>().Where(x => x.GroupId == deptid && x.IsOver).OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
            //if (last != null)
            //{
            //    var lastjobs = new Repository<MeetingJobEntity>(DbFactory.Base()).IQueryable().Where(x => x.EndMeetingId == last.MeetingId && x.IsFinished == "undo").ToList();
            //    foreach (var item in lastjobs)
            //    {
            //        model.Jobs.Add(new MeetingJobEntity() { JobId = Guid.NewGuid().ToString(), Job = item.Job, UserId = item.UserId, JobUsers = item.JobUsers, StartTime = item.StartTime, EndTime = item.EndTime, Dangerous = item.Dangerous, Measure = item.Measure, IsFinished = "undo", Persons = db.IQueryable<JobUserEntity>(x => x.JobId == item.JobId).ToList() });
            //    }
            //}


            //任务库任务

            var templates = GetWorkDateJob(deptid, date);
            //var templates = query.ToList();
            templates.ForEach(x => model.Jobs.Add(new MeetingJobEntity() { JobId = Guid.NewGuid().ToString(), Job = x.JobContent, /*JobUsers = x.JobPerson,*/ StartTime = x.JobStartTime ?? DateTime.Now, EndTime = x.JobEndTime ?? DateTime.Now, Dangerous = x.Dangerous, Measure = x.Measure, NeedTrain = x.EnableTraining, IsFinished = "undo" }));
            var groupmages = new Repository<UserEntity>(DbFactory.Base()).IQueryable().Where(x => x.DepartmentId == deptid).ToList();
            model.Signins = groupmages.Select(x => new MeetingSigninEntity() { MeetingId = model.MeetingId, PersonName = x.RealName, SigninId = Guid.NewGuid().ToString(), UserId = x.UserId, IsSigned = true, MentalCondition = "正常", ClosingCondition = "正常", CreateDate = date }).ToList();
            model.ShouldJoin = model.ActuallyJoin = model.Signins.Count;

            return model;
        }

        private WorkmeetingEntity BuildEndMeeting(string deptid)
        {
            var model = new WorkmeetingEntity() { MeetingType = "班后会", MeetingStartTime = DateTime.Now, GroupId = deptid };
            var templates = new Repository<JobTemplateEntity>(DbFactory.Base()).IQueryable().Where(x => x.DangerType == "job" && x.DeptId == deptid).OrderBy(x => x.CreateDate).ToList();
            templates.ForEach(x => model.Jobs.Add(new MeetingJobEntity() { JobId = Guid.NewGuid().ToString(), Job = x.JobContent, /*JobUsers = x.JobPerson,*/ StartTime = x.JobStartTime ?? DateTime.Now, EndTime = x.JobEndTime ?? DateTime.Now, Dangerous = x.Dangerous, Measure = x.Measure, IsFinished = "finish" }));
            var groupmages = new Repository<UserEntity>(DbFactory.Base()).IQueryable().Where(x => x.DepartmentId == deptid).ToList();
            model.Signins = groupmages.Select(x => new MeetingSigninEntity() { MeetingId = model.MeetingId, PersonName = x.RealName, SigninId = Guid.NewGuid().ToString(), UserId = x.UserId, IsSigned = true, CreateDate = DateTime.Now }).ToList();
            model.ShouldJoin = model.ActuallyJoin = model.Signins.Count;
            return model;
        }

        public WorkmeetingEntity GetStartMeeting(string deptid, string meetingtype, DateTime date)
        {
            IRepository db = new RepositoryFactory().BaseRepository();


            //var deptid = OperatorProvider.Provider.Current().DeptId;
            if (meetingtype == "班后会")
                return this.BuildEndMeeting(deptid);
            else
            {
                var last = db.IQueryable<WorkmeetingEntity>().Where(x => x.GroupId == deptid && x.MeetingType == "班前会").OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
                if (last == null)
                {
                    return this.BuildStartMeeting(deptid, date);
                }
                else
                {
                    if (last.IsOver)
                    {
                        var endmeeting = db.IQueryable<WorkmeetingEntity>().Where(x => x.GroupId == deptid && x.MeetingType == "班后会").OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
                        if (endmeeting == null || endmeeting.OtherMeetingId != last.MeetingId)
                            return new WorkmeetingEntity() { MeetingType = "班后会", OtherMeetingId = last.MeetingId };
                        else
                        {
                            if (endmeeting.IsOver)
                                return this.BuildStartMeeting(deptid, date);
                            else
                                return new WorkmeetingEntity() { MeetingId = endmeeting.MeetingId, MeetingType = "班后会" };
                        }
                    }
                    else
                    {
                        return last;
                    }
                }
            }
        }

        public WorkmeetingEntity GetEndMeeting(string startmeetingid)
        {
            if (string.IsNullOrEmpty(startmeetingid)) return null;

            var startmeeting = this.GetDetail(startmeetingid);
            var model = new WorkmeetingEntity() { MeetingType = "班后会", MeetingStartTime = DateTime.Now, GroupId = startmeeting.GroupId, ShouldJoin = startmeeting.ShouldJoin, ActuallyJoin = startmeeting.ActuallyJoin };
            model.Signins = startmeeting.Signins;
            model.Jobs = startmeeting.Jobs;
            foreach (var item in model.Signins)
            {
                item.SigninId = Guid.NewGuid().ToString();
                item.CreateDate = DateTime.Now;
            }
            foreach (var item in model.Jobs)
            {
                item.PlanId = item.JobId;
                item.JobId = Guid.NewGuid().ToString();
            }
            return model;
        }

        public IList<MeetingJobEntity> GetGroupJobs(string groupid)
        {
            //var from = DateTime.Now.Date;
            //var to = from.AddDays(1).AddMinutes(-1);
            var date = DateTime.Today;
            var db = new RepositoryFactory().BaseRepository();

            var meetingquery = from q in db.IQueryable<WorkmeetingEntity>()
                               where q.GroupId == groupid && q.MeetingStartTime > date && q.MeetingType == "班前会" && q.IsOver == true
                               select q;

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in meetingquery on q2.StartMeetingId equals q3.MeetingId
                        join q4 in db.IQueryable<DangerEntity>() on q2.MeetingJobId equals q4.JobId into into3
                        from q4 in into3.DefaultIfEmpty()
                        select new { q1, q2, q4 };

            var data = query.ToList();
            var result = data.Select(x => new MeetingJobEntity() { JobId = x.q1.JobId, Job = x.q1.Job, Relation = x.q2, StartTime = x.q1.StartTime, EndTime = x.q1.EndTime, JobType = x.q1.JobType, IsFinished = x.q2.IsFinished, NeedTrain = x.q1.NeedTrain, TrainingDone = x.q4 == null ? false : x.q4.Status == 2, TaskType = x.q1.TaskType, RiskLevel = x.q1.RiskLevel }).ToList();

            return result;
        }
        //public IList<MeetingJobEntity> GetGroupJobs(string groupid)
        //{
        //    var from = DateTime.Now.Date;
        //    var to = from.AddDays(1).AddMinutes(-1);

        //    var db = new RepositoryFactory().BaseRepository();

        //    var query = from q in db.IQueryable<WorkmeetingEntity>()
        //                where q.GroupId == groupid && q.MeetingStartTime > @from && q.MeetingStartTime < to
        //                select q;

        //    var meetings = query.ToList();
        //    if (meetings.Count == 0) return null;
        //    else
        //    {
        //        var startmeeting = meetings.Where(x => x.MeetingType == "班前会").OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
        //        var endmeeting = meetings.Where(x => x.MeetingType == "班后会").OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
        //        var meeting = default(WorkmeetingEntity);

        //        if (startmeeting == null) return null;
        //        else
        //        {
        //            if (endmeeting == null)
        //            {
        //                if (startmeeting.IsOver) meeting = startmeeting;
        //            }
        //            else
        //            {
        //                if (endmeeting.OtherMeetingId == startmeeting.MeetingId)
        //                {
        //                    if (!endmeeting.IsOver) meeting = endmeeting;
        //                }
        //                else
        //                {
        //                    if (startmeeting.IsOver) meeting = startmeeting;
        //                }
        //            }
        //        }
        //        if (meeting == null) return null;

        //        var query1 = from q1 in db.IQueryable<MeetingJobEntity>()
        //                     where (meeting.MeetingType == "班前会" ? q1.StartMeetingId == meeting.MeetingId : q1.EndMeetingId == meeting.MeetingId) && q1.IsFinished != "cancel"
        //                     select q1;

        //        return query1.ToList();
        //    }
        //}

        public IList GetList(string from, string to, int page, int pagesize, out int total, int status)
        {
            var query = new Repository<MeetingJobEntity>(DbFactory.Base()).IQueryable();
            if (status == 0)
            {
                query = query.Where(t => t.IsFinished == "undo");
            }
            if (status == 1)
            {
                query = query.Where(t => t.IsFinished == "finish");
            }
            if (from != null) query = query.Where(x => x.StartTime >= DateTime.Parse(from));
            if (to != null)
            {
                query = query.Where(x => x.StartTime <= DateTime.Parse(to).AddDays(1).AddMinutes(-1));
            }
            total = query.Count();
            return query.OrderByDescending(x => x.StartTime).Skip(pagesize * (page - 1)).Take(pagesize).Select(t => new
            {
                t.JobId,
                t.Job,
                //t.JobUsers,
                PlanTime = t.StartTime.ToString("yyyy-MM-dd HH:mm") + "-" + t.EndTime.ToString("HH:mm")
            }).ToList();
        }

        #endregion

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        /// <summary>
        /// 后台管理修改数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void ManagerSaveForm(string keyValue, WorkmeetingEntity entity)
        {
            try
            {
                var Mode = this.GetEntity(keyValue);
                if (Mode != null)
                {
                    Mode.MeetingStartTime = entity.MeetingStartTime;
                    Mode.MeetingEndTime = entity.MeetingEndTime;
                    this.BaseRepository().Update(Mode);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        ///// <summary>
        ///// 保存表单（新增、修改）
        ///// </summary>
        ///// <param name="keyValue">主键值</param>
        ///// <param name="entity">实体对象</param>
        ///// <returns></returns>
        //public void SaveForm(string keyValue, WorkmeetingEntity entity)
        //{
        //    try
        //    {
        //        this.BaseRepository().BeginTrans();
        //        if (string.IsNullOrEmpty(keyValue))
        //        {
        //            if (entity.MeetingType == "班后会")
        //            {
        //                var startmeeting = this.BaseRepository().FindEntity(entity.OtherMeetingId);
        //                startmeeting.OtherMeetingId = entity.MeetingId;
        //                startmeeting.Jobs = null;
        //                startmeeting.Signins = null;
        //                startmeeting.Files = null;
        //                this.BaseRepository().Update(startmeeting);
        //            }
        //            this.BaseRepository().Insert(entity);
        //            foreach (var item in entity.Jobs)
        //            {
        //                if (string.IsNullOrEmpty(item.JobId)) item.JobId = Guid.NewGuid().ToString();
        //                if (entity.MeetingType == "班前会")
        //                    item.StartMeetingId = entity.MeetingId;
        //                item.GroupId = entity.GroupId;
        //                if (entity.MeetingType == "班前会") item.IsFinished = "undo";
        //            }
        //            foreach (var item in entity.Signins)
        //            {
        //                item.MeetingId = entity.MeetingId;
        //            }
        //            new Repository<MeetingJobEntity>(DbFactory.Base()).Insert(entity.Jobs.ToList());
        //            new Repository<MeetingSigninEntity>(DbFactory.Base()).Insert(entity.Signins.ToList());
        //            new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

        //            if (entity.MeetingType == "班后会")
        //            {
        //                var startmeeting = this.GetEntity(entity.OtherMeetingId);
        //                foreach (var item in startmeeting.Jobs)
        //                {
        //                    item.IsFinished = entity.Jobs.FirstOrDefault(x => x.PlanId == item.JobId).IsFinished;
        //                    item.Score = entity.Jobs.FirstOrDefault(x => x.PlanId == item.JobId).Score;
        //                    new Repository<MeetingJobEntity>(DbFactory.Base()).Update(item);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var data = this.GetEntity(keyValue);
        //            data.MeetingType = entity.MeetingType;
        //            data.ShouldJoin = entity.ShouldJoin;
        //            data.ActuallyJoin = entity.ActuallyJoin;
        //            data.Remark = entity.Remark;
        //            data.IsOver = entity.IsOver;
        //            data.MeetingEndTime = entity.MeetingEndTime;
        //            data.PersonState = entity.PersonState;
        //            var jobs = data.Jobs;
        //            var signins = data.Signins;
        //            data.Jobs = null;
        //            data.Signins = null;
        //            data.Files = null;
        //            for (int i = 0; i < signins.Count; i++)
        //            {
        //                signins[i].IsSigned = entity.Signins.Single(x => x.SigninId == signins[i].SigninId).IsSigned;
        //                signins[i].MentalCondition = entity.Signins.Single(x => x.SigninId == signins[i].SigninId).MentalCondition;
        //                signins[i].ClosingCondition = entity.Signins.Single(x => x.SigninId == signins[i].SigninId).ClosingCondition;
        //                new Repository<MeetingSigninEntity>(DbFactory.Base()).Update(signins[i]);
        //            }

        //            foreach (var item in entity.Jobs)
        //            {
        //                if (string.IsNullOrEmpty(item.JobId))
        //                {
        //                    item.JobId = Guid.NewGuid().ToString();
        //                    if (entity.MeetingType == "班前会")
        //                        item.StartMeetingId = data.MeetingId;
        //                    item.GroupId = data.GroupId;
        //                    new Repository<MeetingJobEntity>(DbFactory.Base()).Insert(item);
        //                }
        //                else
        //                {
        //                    var dataitem = new Repository<MeetingJobEntity>(DbFactory.Base()).FindEntity(item.JobId);
        //                    dataitem.Job = item.Job;
        //                    dataitem.UserId = item.UserId;
        //                    dataitem.JobUsers = item.JobUsers;
        //                    dataitem.StartTime = item.StartTime;
        //                    dataitem.EndTime = item.EndTime;
        //                    dataitem.Dangerous = item.Dangerous;
        //                    dataitem.Measure = item.Measure;
        //                    dataitem.IsFinished = item.IsFinished;
        //                    dataitem.Score = item.Score;
        //                    new Repository<MeetingJobEntity>(DbFactory.Base()).Update(dataitem);
        //                }
        //            }

        //            new Repository<MeetingJobEntity>(DbFactory.Base()).Delete(jobs.Where(x => !entity.Jobs.Select(y => y.JobId).Contains(x.JobId)).ToList());

        //            if (entity.MeetingType == "班后会")
        //            {
        //                var startmeeting = this.GetEntity(data.OtherMeetingId);
        //                foreach (var item in startmeeting.Jobs)
        //                {
        //                    item.IsFinished = entity.Jobs.FirstOrDefault(x => x.PlanId == item.JobId).IsFinished;
        //                    new Repository<MeetingJobEntity>(DbFactory.Base()).Update(item);
        //                }
        //            }

        //            this.BaseRepository().Update(data);
        //        }

        //        this.BaseRepository().Commit();
        //    }
        //    catch (System.Exception e)
        //    {
        //        this.BaseRepository().Rollback();
        //        throw e;
        //    }
        //    //if (!string.IsNullOrEmpty(keyValue))
        //    //{
        //    //    entity.Modify(keyValue);
        //    //    this.BaseRepository().Update(entity);
        //    //}
        //    //else
        //    //{
        //    //    entity.Create();
        //    //    this.BaseRepository().Insert(entity);
        //    //}
        //}

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        public int GetIndex(string deptId, DateTime? from, DateTime? to, string name)
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.GroupId == deptId && x.MeetingType == "班前会");
            if (from != null) query = query.Where(x => x.MeetingStartTime >= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.MeetingStartTime <= to);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Remark.Contains(name));
            }
            return query.Count();
        }
        /// <summary>
        /// 国家能源集团版本
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public int GetMeetCountries(string deptId, DateTime from, DateTime to)
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.GroupId == deptId && x.IsOver);
            if (from != null) query = query.Where(x => x.MeetingStartTime >= from);
            if (to != null) query = query.Where(x => x.MeetingStartTime <= to);
            var count = 0;
            var go = true;
            while (go)
            {
                if (from > to)
                {
                    go = false;
                    continue;
                }
                var end = new DateTime(from.Year, from.Month, from.Day).AddDays(1).AddMinutes(-1);
                var startmeet = query.Where(g => g.MeetingType == "班前会" && g.MeetingStartTime >= from && g.MeetingStartTime <= end).Count();
                var endmeet = query.Where(g => g.MeetingType == "班后会" && g.MeetingStartTime >= from && g.MeetingStartTime <= end).Count();
                if (startmeet > 0 && endmeet > 0)
                {
                    count++;
                }
                from = from.AddDays(1);
            }

            return count;
        }
        /// <summary>
        /// 获取当天的任务列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<MeetingJobEntity> GetJobList(string UserId)
        {
            var from = DateTime.Now.Date;
            var to = from.AddDays(1).AddMinutes(-1);
            //string sql = string.Format(@"SELECT
            //                                j.jobid,
            //                                j.meetingid,
            //                                j.planid,
            //                                j.job,
            //                                j.userid,
            //                                j.jobusers,
            //                                j.starttime,
            //                                j.endtime,
            //                                j.dangerous,
            //                                j.measure,
            //                                j.isfinished,
            //                                j.groupid
            //                                FROM
            //                                wg_meetingjob j where userid like'%{0}%'
            //                                ", UserId);
            //return new Repository<MeetingJobEntity>(DbFactory.Base()).FindList(sql).ToList();
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<MeetingJobEntity>()
                        join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.JobId
                        where q1.JobUserId.Contains(UserId) && q.StartTime > @from && q.StartTime < to && q.IsFinished != "cancel"
                        orderby q.StartTime
                        select q;
            return query.ToList();
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="isFinish"></param>
        /// <returns></returns>
        public int UpdateJosState(string JobId, string isFinish)
        {
            var result = 1;
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = (from q1 in db.IQueryable<MeetingJobEntity>()
                              join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                              where q2.MeetingJobId == JobId
                              select q1).FirstOrDefault();
                entity.IsFinished = isFinish;
                db.Update(entity);

                var relations = (from q in db.IQueryable<MeetingAndJobEntity>()
                                 where q.JobId == entity.JobId
                                 select q).ToList();
                foreach (var item in relations)
                {
                    item.IsFinished = isFinish;
                }
                db.Update(relations);

                db.Commit();
            }
            catch (Exception ex)
            {
                result = 0;
                db.Rollback();
                throw ex;
            }

            return result;
            //if (isFinish == "1")
            //{
            //    isFinish = "finish";
            //}
            //else
            //{
            //    isFinish = "Nofinish";
            //}
            //string sql = string.Format("update wg_meetingjob set isfinished='{0}' where jobid='{1}'", isFinish, JobId);
            //return new Repository<MeetingJobEntity>(DbFactory.Base()).ExecuteBySql(sql);
        }

        /// <summary>
        /// 添加新任务
        /// </summary>
        /// <param name="entity"></param>
        public MeetingJobEntity AddNewJob(MeetingJobEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(entity.Relation.StartMeetingId))
                {
                    var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                   where q.MeetingId == entity.Relation.StartMeetingId
                                   select q).FirstOrDefault();

                    if (meeting.MeetingType == "班前会")
                    {
                        if (meeting.MeetingStartTime.Date == entity.StartTime.Date)
                        {
                            entity.Relation.StartMeetingId = meeting.MeetingId;
                            entity.Relation.EndMeetingId = meeting.OtherMeetingId;
                        }
                        else
                        {
                            entity.Relation.StartMeetingId = null;
                            entity.Relation.EndMeetingId = null;
                        }
                    }
                    else
                    {
                        if (meeting.MeetingStartTime.Date == entity.StartTime.Date)
                        {
                            entity.Relation.StartMeetingId = meeting.OtherMeetingId;
                            entity.Relation.EndMeetingId = meeting.MeetingId;
                        }
                        else
                        {
                            entity.Relation.StartMeetingId = null;
                            entity.Relation.EndMeetingId = null;
                        }
                    }
                }

                db.Insert(entity);
                db.Insert(entity.Relation);
                db.Insert(entity.Relation.JobUsers);
                db.Insert(entity.DangerousList);
                db.Insert(entity.DangerousList.SelectMany(x => x.MeasureList).ToList());

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

            return entity;
        }

        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="job"></param>
        public void UpdateJob(MeetingJobEntity job)
        {
            var db = DbFactory.Base() as Data.EF.Database;

            var entity = (from q1 in db.IQueryable<MeetingJobEntity>()
                          join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                          where q2.MeetingJobId == job.Relation.MeetingJobId
                          select q1).FirstOrDefault();
            if (entity != null)
            {
                entity.TicketCode = job.TicketCode;
                entity.TicketId = job.TicketId;
                entity.Description = job.Description;

                db.dbcontext.Entry(entity).State = EntityState.Modified;
            }

            if (job.NeedTrain)
            {
                var training = (from q in db.IQueryable<HumanDangerTrainingBaseEntity>()
                                where q.MeetingJobId == job.Relation.MeetingJobId
                                select q).FirstOrDefault();

                var dept = (from q in db.IQueryable<DepartmentEntity>()
                            where q.DepartmentId == entity.GroupId
                            select q).FirstOrDefault();

                if (training == null)
                {
                    var jobusers = (from q in db.IQueryable<JobUserEntity>()
                                    where q.MeetingJobId == job.Relation.MeetingJobId
                                    select q).ToList();

                    training = new HumanDangerTrainingBaseEntity() { TrainingId = Guid.NewGuid().ToString(), CreateTime = DateTime.Now, CreateUserId = job.CreateUserId, DeptId = entity.GroupId, MeetingJobId = job.Relation.MeetingJobId, DeptName = dept.FullName, TrainingTask = job.Job, TrainingUsers = jobusers.Select(x => new HumanDangerTrainingUserEntity { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, No = entity.TicketCode, TrainingRole = x.JobType == "ischecker" ? 1 : 0, TrainingPlace = job.JobAddr }).ToList() };
                    if (!string.IsNullOrEmpty(entity.TemplateId))
                        training.HumanDangerId = entity.TemplateId;

                    db.dbcontext.Set<HumanDangerTrainingBaseEntity>().Add(training);

                    using (var factory = new ChannelFactory<IMsgService>("message"))
                    {
                        foreach (var item in training.TrainingUsers)
                        {
                            var proxy = factory.CreateChannel();
                            proxy.Send("人身风险预控", item.TrainingUserId.ToString());
                        }
                    }
                }
            }

            var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                            where q.RecId == job.Relation.MeetingJobId & q.CreateUserId == job.CreateUserId
                            select q).ToList();

            try
            {
                var deletefiles = oldfiles.Except(job.Files).ToList();
                foreach (var item in deletefiles)
                {
                    db.dbcontext.Entry(item).State = EntityState.Deleted;
                }

                var newfiles = job.Files.Except(oldfiles).ToList();
                foreach (var item in newfiles)
                {
                    db.dbcontext.Set<FileInfoEntity>().Add(item);
                }

                db.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// 任务变更
        /// </summary>
        /// <param name="job"></param>
        public void ChangeJob(MeetingJobEntity job)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var jobusers = (from q in db.IQueryable<JobUserEntity>()
                                where q.MeetingJobId == job.Relation.MeetingJobId
                                select q).ToList();

                //var deleteusers = jobusers.Where(x => !job.Relation.JobUsers.Select(y => y.UserId).Contains(x.UserId)).ToList();
                var newusers = job.Relation.JobUsers.ToList();

                var dangerous = (from q1 in db.IQueryable<JobDangerousEntity>()
                                 join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into t2
                                 where q1.JobId == job.JobId
                                 select new { q1, q2 = t2 }).ToList();
                foreach (var item in job.DangerousList)
                {
                    var dan = dangerous.Find(x => x.q1.JobDangerousId == item.JobDangerousId);
                    if (dan == null)
                    {
                        db.Insert(item);
                        db.Insert(item.MeasureList);
                    }
                    else
                    {
                        dan.q1.Content = item.Content;
                        dan.q1.DangerousId = item.DangerousId;
                        db.Update(dan.q1);

                        foreach (var item1 in item.MeasureList)
                        {
                            var mea = dan.q2.FirstOrDefault(x => x.JobMeasureId == item1.JobMeasureId);
                            if (mea == null) db.Insert(item1);
                            else
                            {
                                mea.Content = item1.Content;
                                mea.MeasureId = item1.MeasureId;
                                db.Update(mea);
                            }
                        }

                        var deletemeasures = dan.q2.Where(x => !item.MeasureList.Any(y => y.JobMeasureId == x.JobMeasureId)).ToList();
                        db.Delete(deletemeasures);
                    }

                    var deletedangerous = dangerous.Where(x => !job.DangerousList.Any(y => y.JobDangerousId == x.q1.JobDangerousId)).ToList();
                    db.Delete(deletedangerous.Select(x => x.q1).ToList());
                    db.Delete(deletedangerous.SelectMany(x => x.q2).ToList());
                }

                db.Delete(jobusers);
                db.Insert(newusers);

                job.Relation.JobUsers = null;
                db.Update(job.Relation);

                job.Relation = null;
                job.Files = null;
                job.Training = null;
                job.DangerousList = null;
                job.HumanDangerTraining = null;

                db.Update(job);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="Id"></param>
        public void CancelJob(string Id, string meetingjobid)
        {
            StringBuilder strRemark = new StringBuilder();

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.IQueryable<MeetingJobEntity>(x => x.JobId == Id).FirstOrDefault();
                var meetingandjob = db.FindEntity<MeetingAndJobEntity>(meetingjobid);

                entity.IsFinished = "cancel";
                meetingandjob.IsFinished = entity.IsFinished;
                entity.Description = string.Format("已于 {0} 取消", DateTime.Now.ToString("yyyy/MM/dd H:mm"));

                //如果有备注了需要换行
                if (!string.IsNullOrEmpty(entity.Remark))
                {
                    strRemark.Append("\r\n");
                    strRemark.AppendFormat("{0} 任务取消", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    strRemark.AppendFormat("{0} 任务取消", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                entity.Remark = strRemark.ToString();

                db.Update(entity);
                db.Update(meetingandjob);

                db.Commit();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public dynamic GetData(int pagesize, int page, out int total, Dictionary<string, string> dict)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q2.DepartmentId
                        join q3 in db.IQueryable<DepartmentEntity>() on q2.ParentId equals q3.DepartmentId
                        where q1.MeetingType == "班前会" && q1.IsOver == true
                        orderby q1.MeetingStartTime descending
                        select new
                        {
                            q1.GroupId,
                            q1.MeetingStartTime,
                            q1.MeetingPerson,
                            team = q2.FullName,
                            department = q3.FullName,
                            q1.ActuallyJoin,
                            q1.ShouldJoin,
                            q1.MeetingEndTime,
                        };

            var where = new StringBuilder();
            var parameters = new List<DbParameter>();
            foreach (var item in dict)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    if (item.Key == "meetingstarttime")
                    {
                        //where.Append(string.Format(" and t.meetingstarttime >= @{0}", item.Key));
                        //parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, DateTime.Parse(item.Value)));
                        DateTime startTIme = DateTime.Parse(item.Value);
                        query = query.Where(x => x.MeetingStartTime >= startTIme);
                    }
                    else if (item.Key == "meetingendtime")
                    {
                        //where.Append(string.Format(" and t.meetingendtime <= @{0}", item.Key));
                        //parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, DateTime.Parse(item.Value).AddDays(1).AddMinutes(-1)));
                        DateTime endtime = DateTime.Parse(item.Value).AddDays(1);
                        query = query.Where(x => x.MeetingEndTime <= endtime);
                    }
                    else if (item.Key == "departmentid")
                    {
                        var dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == item.Value);
                        if (dept == null) dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.ParentId == "0");
                        //parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, dept.DepartmentId));
                        query = query.Where(x => x.GroupId == dept.DepartmentId);
                    }
                    else if (item.Key == "team")
                    {
                        //where.Append(string.Format(" and t.{0} like @{0}", item.Key));
                        //parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, "%" + item.Value + "%"));
                        query = query.Where(x => x.team.Contains(item.Value));
                    }
                }
            }
            total = query.Count();
            var datalist = query.Skip(pagesize * (page - 1)).Take(pagesize * page).ToList().Select(x => new
            {
                x.GroupId,
                x.MeetingStartTime,
                x.MeetingPerson,
                x.team,
                x.department,
                members = $"{x.ActuallyJoin}/{x.ShouldJoin}"
            });

            return datalist;
            //            var querysql = @"
            //SELECT 
            //    t.*
            //FROM
            //    (SELECT 
            //        a.meetingid,
            //            a.groupid,
            //            a.meetingstarttime,
            //            a.meetingperson,
            //            b.FULLNAME AS team,
            //            c.FULLNAME AS department,
            //            CONCAT(a.actuallyjoin, '/', a.shouldjoin) AS members
            //    FROM
            //        wg_workmeeting a
            //    LEFT JOIN base_department b ON b.DEPARTMENTID = a.groupid
            //    LEFT JOIN base_department c ON c.DEPARTMENTID = b.PARENTID
            //    WHERE
            //        FIND_IN_SET(groupid, FN_RECURSIVE(@departmentid)) > 0
            //            AND a.meetingtype = '班前会'
            //            AND a.isover = 1) t
            //            where 1 = 1 {2}
            //ORDER BY t.meetingstarttime DESC
            //LIMIT {0} , {1}
            //";


            //            var countsql = @"
            //SELECT 
            //    COUNT(1)
            //FROM
            //    (SELECT 
            //        a.meetingid,
            //            a.groupid,
            //            a.meetingstarttime,
            //            a.meetingperson,
            //            b.FULLNAME AS team,
            //            c.FULLNAME AS department,
            //            CONCAT(a.actuallyjoin, '/', a.shouldjoin) AS members
            //    FROM
            //        wg_workmeeting a
            //    LEFT JOIN base_department b ON b.DEPARTMENTID = a.groupid
            //    LEFT JOIN base_department c ON c.DEPARTMENTID = b.PARENTID
            //    WHERE
            //        FIND_IN_SET(groupid, FN_RECURSIVE(@departmentid)) > 0
            //            AND a.meetingtype = '班前会'
            //            AND a.isover = 1) t
            //WHERE
            //    1 = 1 {0};
            //";

            //            total = int.Parse(this.BaseRepository().FindObject(string.Format(countsql, where), parameters.ToArray()).ToString());
            //            return this.BaseRepository().FindTable(string.Format(querysql, pagesize * (page - 1), pagesize * page, where), parameters.ToArray());
        }

        public dynamic GetDataNew(int pagesize, int page, out int total, Dictionary<string, string> dict, string[] depts)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q2.DepartmentId into tb1
                        from t1 in tb1.DefaultIfEmpty()
                        join q3 in db.IQueryable<DepartmentEntity>() on t1.ParentId equals q3.DepartmentId into tb2
                        from t2 in tb2.DefaultIfEmpty()
                        join q4 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q4.MeetingId into tb3
                        from t3 in tb3.DefaultIfEmpty()
                        where q1.MeetingType == "班前会" && q1.IsOver == true
                        select new
                        {
                            aftermeetingid = q1.MeetingId,
                            q1.GroupId,
                            q1.MeetingStartTime,
                            afterstarttime = q1.MeetingStartTime,
                            afterendtime = q1.MeetingEndTime,
                            q1.IsOver,
                            team = t1.FullName,
                            department = t2.FullName,
                            beforemeetingid = t3.MeetingId,
                            beforestarttime = t3.MeetingStartTime,
                            beforeendtime = t3.MeetingEndTime,
                            afterpic = 0,
                            aftervideo = "",
                            beforepic = 0,
                            beforevideo = "",
                        };


            var where = new StringBuilder();
            var parameters = new List<DbParameter>();
            //var deptwhere = string.Empty;
            if (depts != null)
            {
                //deptwhere = string.Format(" and a.groupid in ('{0}')", string.Join("','", depts));
                query = query.Where(x => depts.Contains(x.GroupId));
            }

            var finalQuery = from q1 in query
                             join q2 in db.IQueryable<ActivityEvaluateEntity>() on q1.beforemeetingid equals q2.Activityid into tb1
                             from t1 in tb1.DefaultIfEmpty()
                             select new { q1, t1.EvaluateContent };
            foreach (var item in dict)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    if (item.Key == "meetingstarttime")
                    {
                        //where.Append(string.Format(" and t.afterstarttime >= @{0}", "meetingstarttime"));
                        //parameters.Add(DbParameters.CreateDbParameter("@meetingstarttime", DateTime.Parse(item.Value)));
                        DateTime starttime = DateTime.Parse(item.Value);
                        finalQuery = finalQuery.Where(x => x.q1.afterstarttime >= starttime);
                    }


                    else if (item.Key == "meetingendtime")
                    {
                        //where.Append(string.Format(" and t.afterendtime <= @{0}", "afterendtime"));
                        //parameters.Add(DbParameters.CreateDbParameter("@afterendtime", DateTime.Parse(item.Value).AddDays(1).AddMinutes(-1)));
                        DateTime endtime = DateTime.Parse(item.Value).AddDays(1);
                        finalQuery = finalQuery.Where(x => x.q1.afterendtime < endtime);
                    }
                    else if (item.Key == "departmentid")
                    {
                        //var dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == item.Value);
                        //if (dept == null) dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.ParentId == "0");
                        //parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, dept.DepartmentId));
                    }
                    else
                    {
                        if (item.Key != "orderby")
                        {
                            //where.Append(string.Format(" and ( t.afterisover like @{0}", item.Key));
                            //where.Append(string.Format(" or t.beforeisover like @{0}", item.Key));
                            //where.Append(string.Format(" or t.department like @{0}  ", item.Key));
                            //where.Append(string.Format(" or t.team like @{0}  )", item.Key));
                            //parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, "%" + item.Value + "%"));
                        }
                    }


                    if (item.Key == "meetingstarttimetwo")
                    {
                        //where.Append(string.Format(" and t.afterstarttime <= @{0}", "meetingstarttimetwo"));
                        //parameters.Add(DbParameters.CreateDbParameter("@meetingstarttimetwo", DateTime.Parse(item.Value)));
                        DateTime timetwo = DateTime.Parse(item.Value);
                        finalQuery = finalQuery.Where(x => x.q1.afterstarttime <= timetwo);
                    }
                    if (item.Key == "appraise")
                    {
                        var appraise = item.Value.ToString();
                        if (appraise == "2")  //未评价
                        {
                            //where.Append("and  evaluatecontent is null");
                            finalQuery = finalQuery.Where(x => string.IsNullOrWhiteSpace(x.EvaluateContent));
                        }
                        else if (appraise == "1") //已评价
                        {
                            //where.Append(" and evaluatecontent is not null ");
                            finalQuery = finalQuery.Where(x => !string.IsNullOrWhiteSpace(x.EvaluateContent));
                        }
                    }
                    if (item.Key == "orderby")
                    {
                        if (item.Value == "DESC")
                        {
                            //where.Append(string.Format(" ORDER BY t.meetingstarttime DESC"));
                            finalQuery = finalQuery.OrderByDescending(x => x.q1.MeetingStartTime);
                        }
                        else
                        {
                            //where.Append(string.Format(" ORDER BY t.meetingstarttime ASC"));
                            finalQuery = finalQuery.OrderBy(x => x.q1.MeetingStartTime);
                        }
                    }
                }
            }



            var querysql = @"
SELECT 
    t.*
FROM
    (SELECT 
            a.meetingid as aftermeetingid,
            a.groupid,
            a.meetingstarttime,
            a.meetingstarttime as afterstarttime,
						a.meetingendtime as afterendtime,
						( case when a.isover is NULL then '未开始' 
							WHEN a.isover=0 then '进行中'
							ELSE '已完成' END
             )  as afterisover,
            b.FULLNAME AS team,
            c.FULLNAME AS department,
			d.meetingid as beforemeetingid,
            d.meetingstarttime as beforestarttime,
						d.meetingendtime as beforeendtime,
						( case when d.isover is NULL then '未开始' 
							WHEN d.isover=0 then '进行中'
							ELSE '已完成' END
             ) as beforeisover,
0 as afterpic,
'' as aftervideo,
0 as beforepic,
'' as beforevideo
    FROM
        wg_workmeeting a
    LEFT JOIN base_department b ON b.DEPARTMENTID = a.groupid
    LEFT JOIN base_department c ON c.DEPARTMENTID = b.PARENTID
		LEFT JOIN wg_workmeeting d on a.othermeetingid=d.meetingid
    WHERE 1 = 1 and a.isover = 1
        {3}
            AND a.meetingtype = '班前会'
            ) t   LEFT JOIN wg_activityevaluate e on t.beforemeetingid=e.activityid
            where 1 = 1 {2}

LIMIT {0} , {1}
";
            var countsql = @"
SELECT 
    COUNT(1)
FROM
    (SELECT 
            a.meetingid as aftermeetingid,
            a.groupid,
            a.meetingstarttime,
            a.meetingstarttime as afterstarttime,
						a.meetingendtime as afterendtime,
						( case when a.isover is NULL then '未开始' 
							WHEN a.isover=0 then '进行中'
							ELSE '已完成' END
             )  as afterisover,
            b.FULLNAME AS team,
            c.FULLNAME AS department,
			d.meetingid as beforemeetingid,
            d.meetingstarttime as beforestarttime,
						d.meetingendtime as beforeendtime,
						( case when d.isover is NULL then '未开始' 
							WHEN d.isover=0 then '进行中'
							ELSE '已完成' END
             ) as beforeisover,
0 as afterpic,
'' as aftervideo,
0 as beforepic,
'' as beforevideo

    FROM
        wg_workmeeting a
    LEFT JOIN base_department b ON b.DEPARTMENTID = a.groupid
    LEFT JOIN base_department c ON c.DEPARTMENTID = b.PARENTID
		LEFT JOIN wg_workmeeting d on a.othermeetingid=d.meetingid
    WHERE 1 = 1 and a.isover = 1
        {1}
            AND a.meetingtype = '班前会'
            ) t   LEFT JOIN wg_activityevaluate e on t.beforemeetingid=e.activityid
WHERE
    1 = 1 {0};
";

            //total = int.Parse(this.BaseRepository().FindObject(string.Format(countsql, where, deptwhere), parameters.ToArray()).ToString());
            total = finalQuery.Count();
            var data = finalQuery.OrderBy(x => x.q1.beforestarttime).Skip(pagesize * (page - 1)).Take(pagesize).ToList().Select(x => new
            {
                x.q1.aftermeetingid,
                x.q1.GroupId,
                x.q1.MeetingStartTime,
                x.q1.afterstarttime,
                x.q1.afterendtime,
                afterisover = x.q1.IsOver == null ? "未开始" : (x.q1.IsOver == false ? "进行中" : "已完成"),
                x.q1.team,
                x.q1.department,
                x.q1.beforemeetingid,
                x.q1.beforestarttime,
                x.q1.beforeendtime,
                beforeisover = x.q1.IsOver == null ? "未开始" : (x.q1.IsOver == false ? "进行中" : "已完成"),
                x.q1.afterpic,
                x.q1.aftervideo,
                x.q1.beforepic,
                x.q1.beforevideo,
            }).ToList();
            return data;
            //return this.BaseRepository().FindTable(string.Format(querysql, pagesize * (page - 1), pagesize * page, where, deptwhere), parameters.ToArray());
        }


        /// <summary>
        /// 获取评论实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public List<ActivityEvaluateEntity> GetEntityEvaluate(string keyValue)
        {
            var query = new RepositoryFactory<ActivityEvaluateEntity>().BaseRepository().IQueryable();
            query = query.Where(x => x.Activityid == keyValue);
            return query.ToList();
        }
        public dynamic GetdeptId(Dictionary<string, string> dict)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var where = new StringBuilder();
            var parameters = new List<DbParameter>();
            foreach (var item in dict)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    if (item.Key == "departmentid")
                    {
                        var dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == item.Value);
                        if (dept == null) dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.ParentId == "0");
                        parameters.Add(DbParameters.CreateDbParameter("@get" + item.Key, dept.DepartmentId));
                    }
                    else
                    {

                        where.Append(string.Format(" and (a.FULLNAME like @{0}", item.Key));
                        where.Append(string.Format(" or c.FULLNAME like @{0}", item.Key));
                        where.Append(string.Format(" or a.workname like @{0}  )", item.Key));
                        parameters.Add(DbParameters.CreateDbParameter("@" + item.Key, "%" + item.Value + "%"));

                    }
                }
            }
            var querysql = @"  select t.departmentid,t.team,t.workname from
                  (SELECT   a.departmentid, c.FULLNAME AS team,a.workname  FROM  wg_workorder a 
  LEFT JOIN base_department b ON b.DEPARTMENTID = a.departmentid 
 LEFT JOIN base_department c ON c.DEPARTMENTID = b.PARENTID             
     WHERE   FIND_IN_SET(a.departmentid, FN_RECURSIVE(@getdepartmentid)) > 0  {0}
 ) t GROUP BY t.departmentid,t.team,t.workname";
            return this.BaseRepository().FindTable(string.Format(querysql, where), parameters.ToArray());
        }

        public List<DepartmentEntity> GetOverView(string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.ParentId == deptid);
            if (dept == null) dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.ParentId == "0");

            var query1 = from q in db.IQueryable<DepartmentEntity>()
                         where q.DepartmentId == dept.DepartmentId
                         select q;

            var query2 = from q in db.IQueryable<DepartmentEntity>()
                         where q.ParentId == dept.DepartmentId
                         select q;

            while (query2.Count() > 0)
            {
                query1 = query1.Concat(query2);

                query2 = from q1 in query2
                         join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                         select q2;
            }

            var query3 = from q in query1
                         where q.Nature == "班组"
                         select q;

            var query4 = from q1 in query3
                         join q2 in
                             (
                                 from q1 in db.IQueryable<WorkmeetingEntity>()
                                 join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.StartMeetingId
                                 join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                                 where q1.MeetingType == "班前会"
                                 select new { q1.GroupId, q2.JobId, q3.IsFinished }
                                 ) on q1.DepartmentId equals q2.GroupId into t1
                         select new { q1.DepartmentId, q1.FullName, layer = t1.Count(x => x.IsFinished == "finish"), sortcode = t1.Count() };


            //            var sql = @"
            //SELECT 
            //    a.departmentid,
            //    a.FULLNAME,
            //    SUM(CASE b.isfinished
            //        WHEN 'finish' THEN 1
            //        ELSE 0
            //    END) AS layer,
            //    CASE COUNT(1)
            //        WHEN 0 THEN 1
            //        ELSE COUNT(1)
            //    END AS sortcode
            //FROM
            //    base_department a
            //        LEFT JOIN
            //    (SELECT 
            //        b.groupid, a.jobid, a.isfinished
            //    FROM
            //        wg_meetingjob a
            //    INNER JOIN wg_workmeeting b ON b.meetingid = a.meetingid
            //    INNER JOIN (SELECT 
            //        a.groupid, MAX(a.meetingendtime) AS meetingendtime
            //    FROM
            //        wg_workmeeting a
            //    INNER JOIN base_department b ON b.DEPARTMENTID = a.groupid
            //    WHERE
            //        FIND_IN_SET(departmentid, FN_RECURSIVE(@deptid)) >= 0 and nature = '班组'
            //    GROUP BY a.groupid) c ON c.groupid = b.groupid
            //        AND c.meetingendtime = b.meetingendtime) b ON b.groupid = a.departmentid
            //WHERE
            //    FIND_IN_SET(departmentid,
            //            FN_RECURSIVE(@deptid)) >= 0 and nature = '班组'
            //GROUP BY a.departmentid , a.FULLNAME
            //ORDER BY a.ENCODE desc;
            //";

            //            var parameters = new List<DbParameter>() { DbParameters.CreateDbParameter("@deptid", dept.DepartmentId) };
            //            var data = db.FindList<DepartmentEntity>(sql, parameters.ToArray()).ToList();

            var data = query4.ToList();

            return data.Select(x => new DepartmentEntity() { DepartmentId = x.DepartmentId, FullName = x.FullName, Layer = x.layer, SortCode = x.sortcode }).ToList();
        }

        public List<JobTemplateEntity> Find(string key, string deptid, int limit)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            //var query = from q in db.IQueryable<JobTemplateEntity>()
            //            where (q.DangerType == "job" || q.DangerType == "tmp") && (q.DeptId == deptid || q.DeptId == null) && q.JobContent.Contains(key)
            //            orderby q.CreateDate descending
            //            select q;

            var query = from q1 in db.IQueryable<JobTemplateEntity>()
                        join q2 in (from q1 in db.IQueryable<JobDangerousEntity>()
                                    join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                    select new { q1, q2 = into2 }) on q1.JobId equals q2.q1.JobId into into2
                        where (q1.DangerType == "job" || q1.DangerType == "tmp") && (q1.DeptId == deptid || q1.DeptId == null) && q1.JobContent.Contains(key)
                        orderby q1.CreateDate descending
                        select new { q1, q2 = into2 };

            var data = query.Take(limit).ToList();
            foreach (var item in data)
            {
                if (item.q2 != null)
                {
                    foreach (var item1 in item.q2)
                    {
                        item1.q1.MeasureList = item1.q2.ToList();
                    }
                }
                item.q1.DangerousList = item.q2.Select(x => x.q1).ToList();
            }


            //var data = db.IQueryable<JobTemplateEntity>().Where(x => x.DangerType == "job" && (x.DeptId == deptid || x.DeptId == null) && x.JobContent.Contains(key)).ToList();
            //foreach (var item in data)
            //{
            //    item.Dangers = db.IQueryable<DangerTemplateEntity>().Where(x => x.JobId == item.JobId).ToList();
            //    item.Dangerous = string.Join("\r\n", item.Dangers.Select(x => x.Dangerous));
            //    item.Measure = string.Join("\r\n", item.Dangers.Select(x => x.Measure));
            //}

            return data.Select(x => x.q1).ToList();
        }

        public WorkmeetingEntity AddStartMeeting(WorkmeetingEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var lastquery = from q in db.IQueryable<WorkmeetingEntity>()
                                where q.GroupId == model.GroupId
                                orderby q.MeetingStartTime descending
                                select q;

                var last = lastquery.FirstOrDefault();
                if (last != null && !last.IsOver)
                {
                    return this.GetDetail(last.MeetingId);
                }

                db.Insert(model);
                db.Insert(model.Files.ToList());
                db.Insert(model.Signins.ToList());
                foreach (var item in model.Jobs)
                {
                    var job = (from q in db.IQueryable<MeetingJobEntity>()
                               where q.JobId == item.JobId
                               select q).FirstOrDefault();
                    if (job == null)
                    {
                        db.Insert(item);
                        db.Insert(item.Relation);
                        db.Insert(item.Relation.JobUsers);
                        db.Insert(item.DangerousList);
                        db.Insert(item.DangerousList.SelectMany(x => x.MeasureList).ToList());
                    }
                    else
                    {
                        var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                        where q.MeetingJobId == item.Relation.MeetingJobId
                                        select q).FirstOrDefault();
                        if (relation == null)
                        {
                            db.Insert(item.Relation);
                            db.Insert(item.Relation.JobUsers);
                        }
                        else
                        {
                            db.Delete<JobUserEntity>(x => x.MeetingJobId == relation.MeetingJobId);
                            db.Insert(item.Relation.JobUsers);
                        }

                        job.StartTime = item.StartTime;
                        job.EndTime = item.EndTime;
                        job.Job = item.Job;
                        job.Dangerous = item.Dangerous;
                        job.Measure = item.Measure;
                        job.NeedTrain = item.NeedTrain;
                        db.Update(job);
                    }
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }

            return model;
        }

        public void AddEndMeeting(WorkmeetingEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                var lastquery = from q in db.IQueryable<WorkmeetingEntity>()
                                where q.GroupId == model.GroupId
                                orderby q.MeetingStartTime descending
                                select q;

                var last = lastquery.FirstOrDefault();
                if (last != null && !last.IsOver)
                {
                    throw new Exception("meeting is not over");
                }

                db.Insert(model);
                db.Insert(model.Files.ToList());

                var relations = (from q in db.IQueryable<MeetingAndJobEntity>()
                                 where q.StartMeetingId == model.OtherMeetingId
                                 select q).ToList();

                var jobs = (from q1 in db.IQueryable<MeetingJobEntity>()
                            join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                            where q2.StartMeetingId == model.OtherMeetingId
                            select q1).ToList();

                foreach (var item in relations)
                {
                    var newjob = model.Jobs.FirstOrDefault(x => x.JobId == item.JobId);
                    if (newjob != null)
                    {
                        item.EndMeetingId = model.MeetingId;
                        item.IsFinished = newjob.Relation.IsFinished;

                        var job = jobs.Find(x => x.JobId == item.JobId);
                        job.IsFinished = newjob.IsFinished;
                    }
                }

                db.Update(relations);
                db.Update(jobs);

                db.Insert(model.Signins.ToList());

                var startmeeting = db.FindEntity<WorkmeetingEntity>(model.OtherMeetingId);

                startmeeting.OtherMeetingId = model.MeetingId;
                db.Update(startmeeting);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public List<JobTemplateEntity> GetBaseData(string deptid, string empty, string jobplantype, int pagesize, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<JobTemplateEntity>()
                        where q.DangerType == "job" && q.DeptId == deptid && !string.IsNullOrEmpty(q.JobContent)
                        select q;

            if (!string.IsNullOrEmpty(empty))
            {
                query = query.Where(x => x.JobContent.Contains(empty));
            }

            if (!string.IsNullOrEmpty(jobplantype))
            {
                query = query.Where(x => x.jobplantype == jobplantype);
            }

            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * page - pagesize).Take(pagesize).ToList();
        }

        public List<JobTemplateEntity> GetBaseDataNew(string content, int pagesize, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<JobTemplateEntity>()
                        where q.JobType == "danger" && !string.IsNullOrEmpty(q.JobContent)
                        select q;

            if (!string.IsNullOrEmpty(content))
            {
                query = from q in query
                        where q.JobContent.Contains(content)
                        select q;
            }
            query = from q in query
                    orderby q.Usetime descending
                    select q;

            total = query.Count();
            return query.Skip(pagesize * page - pagesize).Take(pagesize).ToList();
        }

        public IEnumerable<JobTemplateEntity> GetPageList(string deptcode, Pagination pagination, string queryJson)
        {
            //Operator user = OperatorProvider.Provider.Current();
            if (deptcode == null) deptcode = "0";
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<JobTemplateEntity>();
            var queryParam = queryJson.ToJObject();
            expression = expression.And(t => deptcode.StartsWith(t.DeptCode));
            if (!queryParam["jobcontent"].IsEmpty())
            {
                string jobcontent = queryParam["jobcontent"].ToString();
                expression = expression.And(t => t.JobContent.Contains(jobcontent) || t.WorkQuarters.Contains(jobcontent));
            }
            expression = expression.And(t => t.JobType == "danger");
            //if (!queryParam["Category"].IsEmpty())
            //{
            //    string Category = queryParam["Category"].ToString();
            //    expression = expression.And(t => t.Category == Category);
            //}
            //expression = expression.And(t => t.TypeId == 2);
            return db.FindList(expression, pagination);
        }

        public void AddJobTemplates(List<JobTemplateEntity> templates)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(templates);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void AddDangerTemplates(List<DangerTemplateEntity> templates)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(templates);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public bool ExistQrImage(WorkmeetingEntity model)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<FileInfoEntity>()
                        where q.RecId == model.MeetingId && q.Description == "二维码"
                        select q;

            return query.Count() > 0;
        }

        public void SaveQrImage(FileInfoEntity file)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(file);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void AddMeasure(JobTemplateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var measurequery = from q in db.IQueryable<JobTemplateEntity>()
                                   where q.DangerType == entity.DangerType && q.DeptId == entity.DeptId && q.Measure == entity.Measure
                                   select q;

                if (measurequery.Count() > 0)
                    throw new Exception("已存在该防范措施！");

                db.Insert(entity);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public List<JobTemplateEntity> GetDangerous(string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<JobTemplateEntity>()
                        where q.DangerType == "dangerous" && q.DeptId == deptId && string.IsNullOrEmpty(q.JobContent)
                        orderby q.CreateDate
                        select q;

            return query.ToList();
        }

        public void DeleteMeasure(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<JobTemplateEntity>(id);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        public void DeleteJobTemplate(List<string> dept)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var delList = new List<JobTemplateEntity>();
                foreach (var item in dept)
                {
                    var job = db.IQueryable<JobTemplateEntity>(x => x.DeptId == item).ToList();
                    delList.AddRange(job);
                }
                db.Delete(delList);
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        public void DeleteJobTemplateByDept(List<string> deptList)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var list = new List<JobTemplateEntity>();
                foreach (var item in deptList)
                {
                    var job = db.IQueryable<JobTemplateEntity>(x => x.DeptId == item && x.isworkgroup).ToList();
                    list.AddRange(job);
                }
                db.Delete(list);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }


        public void DeleteJobTemplate(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                // db.Delete<JobTemplateEntity>(id);
                var list = new List<JobTemplateEntity>();
                //获取班组排班绑定
                WorkOrderService order = new WorkOrderService();
                var obj = GetJobTemplate(id);
                var group = order.GetWorkOrderGroup(obj.DeptId);
                if (group.Count() > 1)
                {
                    foreach (var item in group)
                    {
                        var job = db.IQueryable<JobTemplateEntity>().FirstOrDefault(x => x.DeptId == item.departmentid && x.JobContent == obj.JobContent);
                        if (job != null)
                        {
                            //job.Dangers = getdangertemplate(job.JobId);
                            var jobDanger = db.IQueryable<JobDangerousEntity>().Where(x => x.JobId == job.JobId).ToList();
                            foreach (var Measure in jobDanger)
                            {
                                var Measurelist = db.IQueryable<JobMeasureEntity>().Where(x => x.JobDangerousId == Measure.JobDangerousId).ToList();
                                db.Delete(Measurelist);
                            }
                            db.Delete(jobDanger);
                            //foreach (DangerTemplateEntity d in job.Dangers)
                            //{
                            //    db.Delete<DangerTemplateEntity>(d.DangerId);
                            //}
                            list.Add(job);

                        }
                    }
                }
                else
                {
                    //obj.Dangers = getdangertemplate(obj.JobId);
                    //foreach (DangerTemplateEntity d in obj.Dangers)
                    //{
                    //    db.Delete<DangerTemplateEntity>(d.DangerId);
                    //}
                    var jobDanger = db.IQueryable<JobDangerousEntity>().Where(x => x.JobId == id).ToList();
                    foreach (var Measure in jobDanger)
                    {
                        var Measurelist = db.IQueryable<JobMeasureEntity>().Where(x => x.JobDangerousId == Measure.JobDangerousId).ToList();
                        db.Delete(Measurelist);
                    }
                  
                    db.Delete(jobDanger);
                    list.Add(obj);

                }
                db.Delete(list);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        public MeetingJobEntity GetJobDetail(string id, string meetingjobid, string trainingtype)
        {
            var db = new RepositoryFactory().BaseRepository();

            var data = (from q in db.IQueryable<MeetingJobEntity>()
                        join q1 in (from q1 in db.IQueryable<JobDangerousEntity>()
                                    join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into t1
                                    select new { q1, q2 = t1 }) on q.JobId equals q1.q1.JobId into t1
                        where q.JobId == id
                        select new { q, q1 = t1 }).FirstOrDefault();

            if (data != null)
            {
                data.q.Relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                   where q.MeetingJobId == meetingjobid
                                   select q).FirstOrDefault();

                if (data.q.Relation != null)
                {
                    data.q.Relation.JobUsers = (from q in db.IQueryable<JobUserEntity>()
                                                where q.MeetingJobId == meetingjobid
                                                select q).ToList();
                }

                data.q.Files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == meetingjobid).ToList();

                data.q.Training = (from q1 in db.IQueryable<DangerEntity>()
                                   where q1.JobId == meetingjobid
                                   select q1).FirstOrDefault();
                foreach (var item in data.q1)
                {
                    item.q1.MeasureList = item.q2.ToList();
                }
                data.q.DangerousList = data.q1.Select(x => x.q1).ToList();

                //var humandanger = (from q in (from q1 in db.IQueryable<HumanDangerTrainingBaseEntity>()
                //                              join q2 in db.IQueryable<HumanDangerTrainingUserEntity>() on q1.TrainingId equals q2.TrainingId into into2
                //                              select new { q1, q2 = into2 })
                //                   where q.q1.MeetingJobId == meetingjobid
                //                   select q).FirstOrDefault();

                var ctx = db.IQueryable<HumanDangerTrainingBaseEntity>() as DbQuery<HumanDangerTrainingBaseEntity>;
                if (ctx != null) data.q.HumanDangerTraining = (from q in ctx.Include("TrainingUsers").AsNoTracking()
                                                               where q.MeetingJobId == meetingjobid
                                                               select q).FirstOrDefault();

                if (data.q.HumanDangerTraining != null)
                    data.q.HumanDangerTraining.TrainingUsers.ForEach(x => x.Training = null);
                //if (humandanger != null)
                //{
                //    data.q.HumanDangerTraining = humandanger.q1;
                //    data.q.HumanDangerTraining.TrainingUsers = humandanger.q2.ToList();
                //}

                if (trainingtype == "人身风险预控" && data.q.HumanDangerTraining != null)
                {
                    data.q.Training = new DangerEntity() { Id = data.q.HumanDangerTraining.TrainingUsers.FirstOrDefault().TrainingUserId.ToString() };
                }
            }

            return data.q;
        }

        public MeetingJobEntity PostJob(MeetingJobEntity job)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var startmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                    where q.GroupId == job.GroupId && q.MeetingType == "班前会"
                                    orderby q.MeetingStartTime descending
                                    select q).FirstOrDefault();

                if (job.JobType == "加班任务")
                {
                    var endmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                      where q.MeetingId == startmeeting.OtherMeetingId
                                      select q).FirstOrDefault();
                    if (endmeeting == null)
                        throw new Exception("班后会未进行");

                    if (endmeeting.MeetingStartTime.Date != job.StartTime.Date)
                        throw new Exception("班后会未进行");
                }

                if (startmeeting != null)
                {
                    var query1 = from q in db.IQueryable<WorkmeetingEntity>()
                                 where q.MeetingId == startmeeting.OtherMeetingId
                                 select q;
                    var endmeeting = query1.FirstOrDefault();
                    if (endmeeting == null)
                    {
                        if (job.StartTime.Date == startmeeting.MeetingStartTime.Date)
                            job.Relation.StartMeetingId = startmeeting.MeetingId;
                    }
                    else
                    {
                        if (endmeeting.MeetingStartTime.Date == DateTime.Today)
                        {
                            if (endmeeting.IsOver)
                            {
                                job.Relation.StartMeetingId = startmeeting.MeetingId;
                                job.Relation.EndMeetingId = startmeeting.OtherMeetingId;
                            }
                            else
                            {
                                if (job.StartTime.Date == startmeeting.MeetingStartTime.Date)
                                {
                                    job.Relation.EndMeetingId = endmeeting.MeetingId;
                                    job.Relation.StartMeetingId = endmeeting.OtherMeetingId;
                                }
                            }
                        }
                    }
                }

                var jobusers = job.Relation.JobUsers.ToList<JobUserEntity>();
                foreach (var item in jobusers)
                {
                    item.JobUserId = Guid.NewGuid().ToString();
                    item.MeetingJobId = job.Relation.MeetingJobId;
                }

                if (job.FaultId.HasValue)
                {
                    var fault = new FaultRelationEntity()
                    {
                        FaultId = job.FaultId.Value,
                        MeetingJobId = job.Relation.MeetingJobId,
                        RelationId = Guid.NewGuid().ToString()
                    };
                    db.Insert(fault);
                }

                db.Insert(job);
                db.Insert(job.Relation);
                db.Insert(job.Relation.JobUsers);
                db.Insert(job.DangerousList);
                db.Insert(job.DangerousList.SelectMany(x => x.MeasureList).ToList());
                db.Commit();

                return job;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }


        public List<MeetingSigninEntity> GetJobUser(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<WorkmeetingEntity>()
                        where q.GroupId == deptid && q.MeetingType == "班前会"
                        orderby q.MeetingStartTime descending
                        select q;
            var startmeeting = query.FirstOrDefault();
            if (startmeeting == null) throw new Exception("班前会未开始");

            var query1 = from q in db.IQueryable<WorkmeetingEntity>()
                         where q.MeetingId == startmeeting.OtherMeetingId
                         select q;
            var endmeeting = query1.FirstOrDefault();
            if (endmeeting == null)
            {
                var userquery = from q1 in db.IQueryable<MeetingSigninEntity>()
                                join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID into into1
                                from d1 in into1.DefaultIfEmpty()
                                where q1.MeetingId == startmeeting.MeetingId
                                orderby d1.Planer, d1.Name
                                select q1;
                return userquery.ToList();
            }
            else
            {
                if (endmeeting.IsOver)
                {
                    throw new Exception("班前会未开始");
                }
                else
                {
                    var userquery = from q1 in db.IQueryable<MeetingSigninEntity>()
                                    join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID into into1
                                    from d1 in into1.DefaultIfEmpty()
                                    where q1.MeetingId == endmeeting.MeetingId
                                    orderby d1.Planer, d1.Name
                                    select q1;

                    return userquery.ToList();
                }
            }

        }

        /// <summary>
        /// 获取部门任务
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<MeetingJobEntity> GetDeptJobs(string deptid, int pagesize, int pageindex, out int total)
        {
            total = 0;
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<WorkmeetingEntity>()
                        where q.GroupId == deptid && q.MeetingType == "班前会"
                        orderby q.MeetingStartTime descending
                        select q;

            var startmeeting = query.FirstOrDefault();
            if (startmeeting == null) return null;

            var query1 = from q in db.IQueryable<WorkmeetingEntity>()
                         where q.MeetingId == startmeeting.OtherMeetingId
                         select q;
            var endmeeting = query1.FirstOrDefault();
            if (endmeeting == null)
            {
                if (!startmeeting.IsOver) return null;

                var jobquery = from q1 in db.IQueryable<MeetingJobEntity>()
                               join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                               join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId into into1
                               join q4 in db.IQueryable<DangerEntity>() on q2.MeetingJobId equals q4.JobId into into2
                               from q4 in into2.DefaultIfEmpty()
                               where q2.StartMeetingId == startmeeting.MeetingId && q2.IsFinished != "cancel"
                               select new { q1, q2, q4, into1 };
                total = jobquery.Count();

                var data = jobquery.OrderBy(x => x.q1.CreateDate).Skip(pagesize * pageindex - pagesize).Take(pagesize).ToList();

                return data.Select(x => new MeetingJobEntity() { JobId = x.q1.JobId, IsFinished = x.q1.IsFinished, Job = x.q1.Job, StartTime = x.q1.StartTime, EndTime = x.q1.EndTime, NeedTrain = x.q1.NeedTrain, JobType = x.q1.JobType, TrainingDone = x.q4 == null ? false : x.q4.Status == 2, Relation = new MeetingAndJobEntity() { JobUserId = x.q2.JobUserId, JobUser = x.q2.JobUser, IsFinished = x.q2.IsFinished, MeetingJobId = x.q2.MeetingJobId, JobUsers = x.into1.ToList() }, CreateDate = x.q1.CreateDate }).ToList();
            }
            else
            {
                if (endmeeting.IsOver)
                {
                    if (endmeeting.MeetingStartTime.Date == DateTime.Today)
                    {
                        var jobquery = from q1 in db.IQueryable<MeetingJobEntity>()
                                       join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                       join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId into into1
                                       join q4 in db.IQueryable<DangerEntity>() on q2.MeetingJobId equals q4.JobId into into2
                                       from q4 in into2.DefaultIfEmpty()
                                       where q2.EndMeetingId == endmeeting.MeetingId && q2.IsFinished != "cancel"
                                       select new { q1, q2, q4, into1 };
                        total = jobquery.Count();

                        var data = jobquery.OrderBy(x => x.q1.CreateDate).Skip(pagesize * pageindex - pagesize).Take(pagesize).ToList();

                        return data.Select(x => new MeetingJobEntity() { JobId = x.q1.JobId, IsFinished = x.q1.IsFinished, Job = x.q1.Job, StartTime = x.q1.StartTime, EndTime = x.q1.EndTime, NeedTrain = x.q1.NeedTrain, JobType = x.q1.JobType, TrainingDone = x.q4 == null ? false : x.q4.Status == 2, Relation = new MeetingAndJobEntity() { JobUserId = x.q2.JobUserId, JobUser = x.q2.JobUser, IsFinished = x.q2.IsFinished, MeetingJobId = x.q2.MeetingJobId, JobUsers = x.into1.ToList() }, CreateDate = x.q1.CreateDate }).ToList();
                    }
                    else
                        return null;
                }
                else
                {
                    var jobquery = from q1 in db.IQueryable<MeetingJobEntity>()
                                   join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                   join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId into into1
                                   join q4 in db.IQueryable<DangerEntity>() on q2.MeetingJobId equals q4.JobId into into2
                                   from q4 in into2.DefaultIfEmpty()
                                   where q2.EndMeetingId == endmeeting.MeetingId && q2.IsFinished != "cancel"
                                   select new { q1, q2, q4, into1 };
                    total = jobquery.Count();

                    var data = jobquery.OrderBy(x => x.q1.CreateDate).Skip(pagesize * pageindex - pagesize).Take(pagesize).ToList();


                    return data.Select(x => new MeetingJobEntity() { JobId = x.q1.JobId, IsFinished = x.q1.IsFinished, Job = x.q1.Job, StartTime = x.q1.StartTime, EndTime = x.q1.EndTime, NeedTrain = x.q1.NeedTrain, JobType = x.q1.JobType, TrainingDone = x.q4 == null ? false : x.q4.Status == 2, Relation = new MeetingAndJobEntity() { JobUserId = x.q2.JobUserId, JobUser = x.q2.JobUser, IsFinished = x.q2.IsFinished, MeetingJobId = x.q2.MeetingJobId, JobUsers = x.into1.ToList() }, CreateDate = x.q1.CreateDate }).ToList();
                }
            }
        }

        public JobTemplateEntity GetJobTemplate(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<JobTemplateEntity>()
                        join q2 in (
                            from q1 in db.IQueryable<JobDangerousEntity>()
                            join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                            select new { q1, q2 = into2 }
                            ) on q1.JobId equals q2.q1.JobId into into2
                        where q1.JobId == id
                        select new { q1, q2 = into2 };
            var data = query.FirstOrDefault();
            if (data == null)
            {
                return null;
            }
            foreach (var item in data.q2)
            {
                item.q1.MeasureList = item.q2.ToList();
            }
            data.q1.DangerousList = data.q2.Select(x => x.q1).ToList();
            return data.q1;
        }

        /// <summary>
        /// 绑定班组修改
        /// </summary>
        /// <param name="model"></param>
        /// <param name="DeptList"></param>
        /// <returns></returns>
        public string UpdateJobTemplateGroup(JobTemplateEntity model, List<string> DeptList)
        {
            //Operator user = OperatorProvider.Provider.Current();
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                List<JobTemplateEntity> JobList = new List<JobTemplateEntity>();
                var entity = db.FindEntity<JobTemplateEntity>(model.JobId);
                if (entity != null)
                {
                    var jobcontent = entity.JobContent;
                    var query = from q in db.IQueryable<JobTemplateEntity>()
                                where q.JobContent == model.JobContent && q.DeptId == model.DeptId && q.DangerType == "job"
                                select q;
                    if (query.Count() > 0)
                    {
                        if (!query.Select(x => x.JobId).Contains(entity.JobId))
                        {
                            throw new Exception("该任务已存在");
                        }
                    }
                    foreach (var item in DeptList)
                    {

                        var jobs = db.IQueryable<JobTemplateEntity>().FirstOrDefault(q => q.JobContent == jobcontent && q.DeptId == item);
                        //当前修改班组
                        if (item == model.DeptId)
                        {

                            jobs.JobPerson = string.IsNullOrEmpty(model.JobPerson) ? "" : model.JobPerson;
                            jobs.JobPersonId = string.IsNullOrEmpty(model.JobPersonId) ? "" : model.JobPersonId;
                        }
                        else
                        {
                            //其他班组修改

                            if (jobs == null)
                            {
                                continue;
                            }
                        }
                        jobs.JobContent = string.IsNullOrEmpty(model.JobContent) ? "" : model.JobContent;
                        jobs.JobStartTime = model.JobStartTime;
                        jobs.JobEndTime = model.JobEndTime;
                        jobs.Device = string.IsNullOrEmpty(model.Device) ? "" : model.Device;
                        jobs.EnableTraining = model.EnableTraining;
                        jobs.Dangerous = string.IsNullOrEmpty(model.Dangerous) ? "" : model.Dangerous;
                        jobs.Measure = string.IsNullOrEmpty(model.Measure) ? "" : model.Measure;
                        jobs.Cycle = string.IsNullOrEmpty(model.Cycle) ? "" : model.Cycle;
                        jobs.CycleDate = string.IsNullOrEmpty(model.CycleDate) ? "" : model.CycleDate;
                        jobs.WorkArea = string.IsNullOrEmpty(model.WorkArea) ? "" : model.WorkArea;
                        jobs.WorkDescribe = string.IsNullOrEmpty(model.WorkDescribe) ? "" : model.WorkDescribe;
                        jobs.WorkQuarters = string.IsNullOrEmpty(model.WorkQuarters) ? "" : model.WorkQuarters;
                        jobs.WorkType = string.IsNullOrEmpty(model.WorkType) ? "" : model.WorkType;
                        jobs.Usetime = model.Usetime;
                        jobs.EditTime = model.EditTime;
                        jobs.ResPrepare = string.IsNullOrEmpty(model.ResPrepare) ? "" : model.ResPrepare;
                        jobs.RedactionPerson = string.IsNullOrEmpty(model.RedactionPerson) ? "" : model.RedactionPerson;
                        jobs.RedactionDate = DateTime.Now;
                        jobs.PicNumber = string.IsNullOrEmpty(model.PicNumber) ? "" : model.PicNumber;
                        jobs.JobType = string.IsNullOrEmpty(model.JobType) ? "" : model.JobType;
                        jobs.isweek = model.isweek;
                        jobs.worksetid = model.worksetid;
                        jobs.isend = model.isend;
                        jobs.isexplain = model.isexplain;
                        jobs.jobplantypeid = string.IsNullOrEmpty(model.jobplantypeid) ? "" : model.jobplantypeid;
                        jobs.jobplantype = string.IsNullOrEmpty(model.jobplantype) ? "" : model.jobplantype;
                        jobs.jobstandard = string.IsNullOrEmpty(model.jobstandard) ? "" : model.jobstandard;
                        jobs.islastday = model.islastday;
                        jobs.worksetname = string.IsNullOrEmpty(model.worksetname) ? "" : model.worksetname;
                        jobs.otherperson = string.IsNullOrEmpty(model.otherperson) ? "" : model.otherperson;
                        jobs.otherpersonid = string.IsNullOrEmpty(model.otherpersonid) ? "" : model.otherpersonid;
                        jobs.TaskType = string.IsNullOrEmpty(model.TaskType) ? "" : model.TaskType;
                        jobs.RiskLevel = string.IsNullOrEmpty(model.RiskLevel) ? "" : model.RiskLevel;
                        jobs.isworkgroup = true;

                        var dans = (from q1 in db.IQueryable<JobDangerousEntity>()
                                    join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                    where q1.JobId == jobs.JobId
                                    select new { q1, q2 = into2 }).ToList();
                       // var deleteitems = dans.Where(x => !model.DangerousList.Any(y => y.DangerousId == x.q1.DangerousId)).ToList();
                        db.Delete(dans.Select(x => x.q1).ToList());
                        db.Delete(dans.SelectMany(x => x.q2).ToList()); 

                        foreach (var item1 in model.DangerousList)
                        {
                            var dan = dans.Find(x => x.q1.JobDangerousId == item1.JobDangerousId);
                            //  var dan = dans.Find(x => x.q1.DangerousId == item1.DangerousId);
                            if (dan == null)
                            {
                                var newdan = new JobDangerousEntity()
                                {
                                    JobDangerousId = Guid.NewGuid().ToString(),
                                    Content = item1.Content,
                                    CreateTime = DateTime.Now,
                                    DangerousId = item1.DangerousId,
                                    JobId = jobs.JobId
                                };
                                var newmea = item1.MeasureList.Select(x => new JobMeasureEntity()
                                {
                                    JobMeasureId = Guid.NewGuid().ToString(),
                                    Content = x.Content,
                                    CreateTime = DateTime.Now,
                                    JobDangerousId = newdan.JobDangerousId,
                                    MeasureId = x.MeasureId
                                }).ToList();
                                db.Insert(newdan);
                                db.Insert(newmea);
                            }
                            else
                            {
                                dan.q1.Content = item1.Content;
                                db.Update(dan.q1);

                                var deletemeas = dan.q2.Where(x => !item1.MeasureList.Any(y => y.MeasureId == x.MeasureId)).ToList();
                                db.Delete(deletemeas);

                                foreach (var item2 in item1.MeasureList)
                                {
                                    var mea = dan.q2.FirstOrDefault(x => x.MeasureId == item2.MeasureId);
                                    if (mea == null)
                                    {
                                        var newmea = new JobMeasureEntity()
                                        {
                                            JobMeasureId = Guid.NewGuid().ToString(),
                                            Content = item2.Content,
                                            CreateTime = DateTime.Now,
                                            JobDangerousId = dan.q1.JobDangerousId,
                                            MeasureId = item2.MeasureId
                                        };
                                        db.Insert(newmea);
                                    }
                                    else
                                    {
                                        mea.Content = item2.Content;
                                        db.Update(mea);
                                    }
                                }
                            }
                        }


                        model.Files = null;
                        //if (string.IsNullOrEmpty(model.CreateUserId))
                        //{
                        //    jobs.CreateDate = DateTime.Now;
                        //    jobs.CreateUserId = model.CreateUserId;
                        //    jobs.CreateUser = model.CreateUser;
                        //}
                        //else
                        //{
                        jobs.CreateDate = model.CreateDate;
                        jobs.CreateUserId = model.CreateUserId;
                        jobs.CreateUser = model.CreateUser;
                        //}
                        JobList.Add(jobs);
                    }
                    db.Update(JobList);
                    db.Commit();
                    return ("修改成功！");
                }
                else
                {
                    var mydept = model.DeptId;
                    var mypeople = model.JobPerson;
                    var mypeopleid = model.JobPersonId;
                    foreach (var item in DeptList)
                    {
                        var jobs = new JobTemplateEntity();
                        var query = from q in db.IQueryable<JobTemplateEntity>()
                                    where q.JobContent == model.JobContent && q.DeptId == item && q.DangerType == "job"
                                    select q;
                        if (query.Count() > 0) throw new Exception("该任务已存在");
                        if (string.IsNullOrEmpty(model.JobId))
                        {
                            jobs.JobId = Guid.NewGuid().ToString();
                        }
                        jobs.JobContent = string.IsNullOrEmpty(model.JobContent) ? "" : model.JobContent;
                        jobs.JobStartTime = model.JobStartTime;
                        jobs.JobEndTime = model.JobEndTime;
                        jobs.Device = string.IsNullOrEmpty(model.Device) ? "" : model.Device;
                        jobs.EnableTraining = model.EnableTraining;
                        jobs.Dangerous = string.IsNullOrEmpty(model.Dangerous) ? "" : model.Dangerous;
                        jobs.Measure = string.IsNullOrEmpty(model.Measure) ? "" : model.Measure;
                        jobs.Cycle = string.IsNullOrEmpty(model.Cycle) ? "" : model.Cycle;
                        jobs.CycleDate = string.IsNullOrEmpty(model.CycleDate) ? "" : model.CycleDate;
                        jobs.WorkArea = string.IsNullOrEmpty(model.WorkArea) ? "" : model.WorkArea;
                        jobs.WorkDescribe = string.IsNullOrEmpty(model.WorkDescribe) ? "" : model.WorkDescribe;
                        jobs.WorkQuarters = string.IsNullOrEmpty(model.WorkQuarters) ? "" : model.WorkQuarters;
                        jobs.WorkType = string.IsNullOrEmpty(model.WorkType) ? "" : model.WorkType;
                        jobs.EditTime = model.EditTime;
                        jobs.ResPrepare = string.IsNullOrEmpty(model.ResPrepare) ? "" : model.ResPrepare;
                        jobs.PicNumber = string.IsNullOrEmpty(model.PicNumber) ? "" : model.PicNumber;
                        jobs.JobType = string.IsNullOrEmpty(model.JobType) ? "" : model.JobType;
                        jobs.isweek = model.isweek;
                        jobs.worksetid = model.worksetid;
                        jobs.isend = model.isend;
                        jobs.isexplain = model.isexplain;
                        jobs.jobplantypeid = string.IsNullOrEmpty(model.jobplantypeid) ? "" : model.jobplantypeid;
                        jobs.jobplantype = string.IsNullOrEmpty(model.jobplantype) ? "" : model.jobplantype;
                        jobs.jobstandard = string.IsNullOrEmpty(model.jobstandard) ? "" : model.jobstandard;
                        jobs.islastday = model.islastday;
                        jobs.worksetname = string.IsNullOrEmpty(model.worksetname) ? "" : model.worksetname;
                        jobs.otherperson = string.IsNullOrEmpty(model.otherperson) ? "" : model.otherperson;
                        jobs.otherpersonid = string.IsNullOrEmpty(model.otherpersonid) ? "" : model.otherpersonid;
                        jobs.TaskType = string.IsNullOrEmpty(model.TaskType) ? "" : model.TaskType;
                        jobs.RiskLevel = string.IsNullOrEmpty(model.RiskLevel) ? "" : model.RiskLevel;
                        jobs.isworkgroup = true;
                        model.Files = null;

                        if (model.DangerousList != null)
                        {
                            var newdan = model.DangerousList.Select(x => new JobDangerousEntity()
                            {
                                JobDangerousId = Guid.NewGuid().ToString(),
                                Content = x.Content,
                                CreateTime = DateTime.Now,
                                DangerousId = x.DangerousId,
                                JobId = jobs.JobId,
                                MeasureList = x.MeasureList.Select(y => new JobMeasureEntity()
                                {
                                    JobMeasureId = Guid.NewGuid().ToString(),
                                    Content = y.Content,
                                    CreateTime = DateTime.Now,
                                    MeasureId = y.MeasureId
                                }).ToList()
                            }).ToList();
                            foreach (var item1 in newdan)
                            {
                                foreach (var item2 in item1.MeasureList)
                                {
                                    item2.JobDangerousId = item1.JobDangerousId;
                                }
                            }
                            db.Insert(newdan);
                            db.Insert(newdan.SelectMany(x => x.MeasureList).ToList());
                        }

                        //if (string.IsNullOrEmpty(model.CreateUserId))
                        //{
                        //    jobs.RedactionPerson = model.CreateUser;
                        //    jobs.CreateUserId = model.CreateUserId;
                        //    jobs.CreateUser = model.CreateUser;
                        //}
                        //else
                        //{
                        jobs.RedactionPerson = model.RedactionPerson;
                        jobs.CreateUserId = model.CreateUserId;
                        jobs.CreateUser = model.CreateUser;
                        //}
                        if (mydept == item)
                        {
                            jobs.JobPerson = mypeople;
                            jobs.JobPersonId = mypeopleid;

                        }
                        else
                        {
                            jobs.JobPerson = null;
                            jobs.JobPersonId = null;
                        }
                        jobs.DeptId = item;
                        jobs.CreateDate = DateTime.Now;
                        jobs.DangerType = "job";
                        jobs.RedactionDate = DateTime.Now;
                        jobs.Usetime = 0;
                        //bool b = savedangertemplate(model);
                        //if (b == false) return ("1");
                        JobList.Add(jobs);
                    }
                    db.Insert(JobList);
                    db.Commit();
                    return ("新增成功！");

                }
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }


        }
        public string UpdateJobTemplate(JobTemplateEntity model)
        {
            //Operator user = OperatorProvider.Provider.Current();
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity = db.FindEntity<JobTemplateEntity>(model.JobId);
                if (entity != null)
                {
                    var query = from q in db.IQueryable<JobTemplateEntity>()
                                where q.JobContent == model.JobContent && q.DeptId == model.DeptId && q.DangerType == "job"
                                select q;
                    if (query.Count() > 0)
                    {
                        if (!query.Select(x => x.JobId).Contains(entity.JobId))
                        {
                            throw new Exception("该任务已存在");
                        }
                    }
                    //bool b = savedangertemplate(model);
                    //if (b == false) return ("1");
                    entity.JobContent = string.IsNullOrEmpty(model.JobContent) ? "" : model.JobContent;
                    entity.JobPerson = string.IsNullOrEmpty(model.JobPerson) ? "" : model.JobPerson;
                    entity.JobPersonId = string.IsNullOrEmpty(model.JobPersonId) ? "" : model.JobPersonId;
                    entity.JobStartTime = model.JobStartTime;
                    entity.JobEndTime = model.JobEndTime;
                    entity.Device = string.IsNullOrEmpty(model.Device) ? "" : model.Device;
                    entity.EnableTraining = model.EnableTraining;
                    entity.Dangerous = string.IsNullOrEmpty(model.Dangerous) ? "" : model.Dangerous;
                    entity.Measure = string.IsNullOrEmpty(model.Measure) ? "" : model.Measure;
                    entity.Cycle = string.IsNullOrEmpty(model.Cycle) ? "" : model.Cycle;
                    entity.CycleDate = string.IsNullOrEmpty(model.CycleDate) ? "" : model.CycleDate;
                    entity.WorkArea = string.IsNullOrEmpty(model.WorkArea) ? "" : model.WorkArea;
                    entity.WorkDescribe = string.IsNullOrEmpty(model.WorkDescribe) ? "" : model.WorkDescribe;
                    entity.WorkQuarters = string.IsNullOrEmpty(model.WorkQuarters) ? "" : model.WorkQuarters;
                    entity.WorkType = string.IsNullOrEmpty(model.WorkType) ? "" : model.WorkType;
                    entity.Usetime = model.Usetime;
                    entity.EditTime = model.EditTime;
                    entity.ResPrepare = string.IsNullOrEmpty(model.ResPrepare) ? "" : model.ResPrepare;
                    entity.RedactionPerson = string.IsNullOrEmpty(model.RedactionPerson) ? "" : model.RedactionPerson;
                    entity.RedactionDate = DateTime.Now;
                    entity.PicNumber = string.IsNullOrEmpty(model.PicNumber) ? "" : model.PicNumber;
                    entity.JobType = string.IsNullOrEmpty(model.JobType) ? "" : model.JobType;
                    entity.isweek = model.isweek;
                    entity.worksetid = model.worksetid;
                    entity.isend = model.isend;
                    entity.isexplain = model.isexplain;
                    entity.jobplantypeid = string.IsNullOrEmpty(model.jobplantypeid) ? "" : model.jobplantypeid;
                    entity.jobplantype = string.IsNullOrEmpty(model.jobplantype) ? "" : model.jobplantype;
                    entity.jobstandard = string.IsNullOrEmpty(model.jobstandard) ? "" : model.jobstandard;
                    entity.islastday = model.islastday;
                    entity.worksetname = string.IsNullOrEmpty(model.worksetname) ? "" : model.worksetname;
                    entity.otherperson = string.IsNullOrEmpty(model.otherperson) ? "" : model.otherperson;
                    entity.otherpersonid = string.IsNullOrEmpty(model.otherpersonid) ? "" : model.otherpersonid;
                    entity.TaskType = string.IsNullOrEmpty(model.TaskType) ? "" : model.TaskType;
                    entity.RiskLevel = string.IsNullOrEmpty(model.RiskLevel) ? "" : model.RiskLevel;
                    model.Files = null;

                    var dans = (from q1 in db.IQueryable<JobDangerousEntity>()
                                join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                where q1.JobId == entity.JobId
                                select new { q1, q2 = into2 }).ToList();
                   // var deleteitems = dans.Where(x => !model.DangerousList.Any(y => y.JobDangerousId == x.q1.DangerousId)).ToList();
                    db.Delete(dans.Select(x => x.q1).ToList());
                    db.Delete(dans.SelectMany(x => x.q2).ToList());
                    foreach (var item1 in model.DangerousList)
                    {
                        var dan = dans.Find(x => x.q1.JobDangerousId == item1.JobDangerousId);
                        // var dan = dans.Find(x => x.q1.DangerousId == item1.DangerousId);
                        if (dan == null)
                        {
                            var newdan = new JobDangerousEntity()
                            {
                                JobDangerousId = Guid.NewGuid().ToString(),
                                Content = item1.Content,
                                CreateTime = DateTime.Now,
                                DangerousId = item1.DangerousId,
                                JobId = entity.JobId
                            };
                            var newmea = item1.MeasureList.Select(x => new JobMeasureEntity()
                            {
                                JobMeasureId = Guid.NewGuid().ToString(),
                                Content = x.Content,
                                CreateTime = DateTime.Now,
                                JobDangerousId = newdan.JobDangerousId,
                                MeasureId = x.MeasureId
                            }).ToList();
                            db.Insert(newdan);
                            db.Insert(newmea);
                        }
                        else
                        {
                            dan.q1.Content = item1.Content;
                            db.Update(dan.q1);

                            var deletemeas = dan.q2.Where(x => !item1.MeasureList.Any(y => y.MeasureId == x.MeasureId)).ToList();
                            db.Delete(deletemeas);

                            foreach (var item2 in item1.MeasureList)
                            {
                                var mea = dan.q2.FirstOrDefault(x => x.MeasureId == item2.MeasureId);
                                if (mea == null)
                                {
                                    var newmea = new JobMeasureEntity()
                                    {
                                        JobMeasureId = Guid.NewGuid().ToString(),
                                        Content = item2.Content,
                                        CreateTime = DateTime.Now,
                                        JobDangerousId = dan.q1.JobDangerousId,
                                        MeasureId = item2.MeasureId
                                    };
                                    db.Insert(newmea);
                                }
                                else
                                {
                                    mea.Content = item2.Content;
                                    db.Update(mea);
                                }
                            }
                        }
                    }

                    db.Update(entity);
                    db.Commit();
                    return ("修改成功！");
                }
                else
                {
                    if (string.IsNullOrEmpty(model.JobId))
                    {
                        model.JobId = Guid.NewGuid().ToString();
                        foreach (var item in model.DangerousList)
                        {
                            item.JobId = model.JobId;
                        }
                    }
                    //if (string.IsNullOrEmpty(model.CreateUserId))
                    //{
                    //    model.RedactionPerson = user.UserName;
                    //    model.CreateUserId = user.UserId;
                    //    model.CreateUser = user.UserName;
                    //}

                    var query = from q in db.IQueryable<JobTemplateEntity>()
                                where q.JobContent == model.JobContent && q.DeptId == model.DeptId && q.DangerType == "job"
                                select q;
                    if (query.Count() > 0) throw new Exception("该任务已存在");


                    model.CreateDate = DateTime.Now;

                    model.DangerType = "job";


                    model.RedactionDate = DateTime.Now;
                    model.Usetime = 0;
                    //bool b = savedangertemplate(model);
                    //if (b == false) return ("1");

                    db.Insert(model);
                    if (model.DangerousList != null)
                    {
                        db.Insert(model.DangerousList);
                        db.Insert(model.DangerousList.SelectMany(x => x.MeasureList).ToList());
                    }
                    db.Commit();
                    return ("新增成功！");

                }
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }


        }
        public bool savedangertemplate(JobTemplateEntity model)
        {
            bool b = true;
            //Operator user = OperatorProvider.Provider.Current();
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(model.d) && !string.IsNullOrEmpty(model.m))
                {
                    model.d = model.d.Replace("\r\n", "").Trim();
                    model.m = model.m.Replace("\r\n", "").Trim();
                    if (model.d.EndsWith("。")) { model.d = model.d.Substring(0, model.d.Length - 1); }
                    if (model.m.EndsWith("。")) { model.m = model.m.Substring(0, model.m.Length - 1); }
                    var dangers = model.d.Split('。');
                    var measures = model.m.Split('。');

                    if (dangers.Count() == measures.Count())
                    {
                        //删除原危险及防范措施
                        var list = getdangertemplate(model.JobId);
                        foreach (DangerTemplateEntity dan in list)
                        {
                            db.Delete(dan);
                        }
                        if (dangers.Count() > 0)
                        {

                            for (int i = 0; i < dangers.Count(); i++)
                            {
                                DangerTemplateEntity obj = new DangerTemplateEntity();
                                obj.DangerId = Guid.NewGuid().ToString();
                                obj.Dangerous = dangers[i] + "。";
                                obj.Measure = measures[i] + "。";
                                obj.JobId = model.JobId;
                                obj.CreateUserId = model.CreateUserId;
                                obj.CreateTime = DateTime.Now;
                                db.Insert(obj);
                            }
                        }

                    }
                    else
                    {
                        b = false;
                        //throw new Exception("危险与防范措施必须保持一致！");
                    }
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
            return b;
            //else
            //{
            //    var list = getdangertemplate(model.JobId);
            //    foreach (DangerTemplateEntity dan in list)
            //    {
            //        db.Delete(dan);

            //    }
            //    db.Commit();
            //}
        }

        /// <summary>
        /// 添加危险因素
        /// </summary>
        /// <param name="entity"></param>
        public void AddDangerous(JobTemplateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var measurequery = from q in db.IQueryable<JobTemplateEntity>()
                                   where q.DangerType == entity.DangerType && q.DeptId == entity.DeptId && q.Dangerous == entity.Dangerous
                                   select q;
                if (measurequery.Count() > 0)
                    throw new Exception("已存在该危险因素！");

                db.Insert(entity);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// 添加防范措施
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public List<JobTemplateEntity> GetMeasures(string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<JobTemplateEntity>()
                        where q.DangerType == "measure" && q.DeptId == deptId && string.IsNullOrEmpty(q.JobContent)
                        orderby q.CreateDate
                        select q;

            return query.ToList();
        }

        public List<DangerTemplateEntity> getdangertemplate(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<DangerTemplateEntity>()
                        where q.JobId == id
                        select q;
            return query.ToList();
        }
        public DangerTemplateEntity getdangtemplateentity(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<DangerTemplateEntity>(id);
        }
        public void deldangertemplateentity(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            db.Delete<DangerTemplateEntity>(id);
        }

        /// <summary>
        /// 删除班前班后会
        /// </summary>
        /// <param name="id"></param>
        public void DeltetMeeting(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            db.Delete<WorkmeetingEntity>(id);
        }

        public void savedangertemplateentity(DangerTemplateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository();
            var d = this.getdangtemplateentity(entity.DangerId);
            if (d == null)
            {
                db.Insert<DangerTemplateEntity>(entity);
            }
            else
            {
                db.Update<DangerTemplateEntity>(entity);
            }
        }
        public DataTable GetAttendanceData3(Pagination pagination, string name, string deptid, DateTime from, DateTime to, out decimal total, out int records, bool[] isMenu)
        {
            var db = new RepositoryFactory().BaseRepository();

            //排序
            var sortquery = from q1 in db.IQueryable<UserEntity>()
                            join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                            join q3 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q3.DepartmentId
                            where deptid.Contains(q1.DepartmentId)
                            orderby q2.BZCode
                            select new { q2.ID, q2.Name, q2.Photo, q2.Planer, q2.EntryDate, BZName = q3.FullName, BZID = q3.DepartmentId };
            if (!string.IsNullOrEmpty(name))
            {
                sortquery = sortquery.Where(x => x.BZName.Contains(name) || x.Name.Contains(name));
            }
            records = sortquery.Count();
            total = Math.Ceiling((decimal)records / pagination.rows);
            sortquery = sortquery.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var userdata = sortquery.ToList();
            var bzIDStr = string.Join(",", userdata.Select(x => x.BZID));
            var bzUser = string.Join(",", userdata.Select(x => x.ID));
            List<DateTime> workTime = new List<DateTime>();
            //是否启用模块
            if (isMenu[1])
            {
                //获取签到考情情况
                var Face = from q in db.IQueryable<FaceAttendanceTimeEntity>()
                           where bzUser.Contains(q.userid) && q.worktime >= @from && q.worktime <= to
                           select q;
                if (Face.Count() > 0)
                {
                    var getworkTime = Face.Select(x => x.worktime).ToList();
                    foreach (var item in getworkTime)
                    {
                        workTime.Add(item.Date);
                    }
                }

            }
            var bqsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                              where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to && bzUser.Contains(ss.UserId)
                              orderby ms.MeetingStartTime ascending
                              select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned };
            List<string> clear = new List<string>();
            List<DateTime> cleartime = new List<DateTime>();
            foreach (var item in bqsignquery)
            {
                if (!cleartime.Contains(item.MeetingStartTime.Date))
                {
                    clear.Add(item.MeetingId);
                }
                else
                {
                    cleartime.Add(item.MeetingStartTime.Date);
                }
            }
            var clearbqsignquery = bqsignquery.Where(x => clear.Contains(x.MeetingId));

            var bhsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                              where clear.Contains(ms.MeetingId)
                              select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned };
            //出勤
            var signquery = from q1 in
                                clearbqsignquery
                            join q2 in bhsignquery
        on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
                            select g;
            //出勤
            //var signquery = from q1 in
            //                    (from ms in db.IQueryable<WorkmeetingEntity>()
            //                     join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
            //                     where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
            //                     select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
            //                join q2 in
            //                    (from ms in db.IQueryable<WorkmeetingEntity>()
            //                     join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
            //                     where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
            //                     select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
            //                where bzUser.Contains(q1.UserId)
            //                group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
            //                select g;
            ////出勤统计
            //var query = from q1 in userquery
            //            join q2 in signquery on q1.UserId equals q2.Key into into1
            //            from d1 in into1.DefaultIfEmpty()
            //            select new { q1.UserId, d1 };
            //出勤统计
            var signquerys = signquery.ToList();

            var signdata = isMenu[1] ? signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => (n.IsSigned1 || n.IsSigned2) && workTime.Contains(n.MeetingStartTime.Date)) > 0) }) : signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => n.IsSigned1 || n.IsSigned2) > 0) });
            //缺勤类型
            var unsigntypequery = from q1 in db.IQueryable<DataItemEntity>()
                                  join q2 in db.IQueryable<DataItemDetailEntity>() on q1.ItemId equals q2.ItemId
                                  where q1.ItemName == "缺勤原因" && q2.ItemName != "不参会"
                                  select q2;
            //缺勤类型
            var unsigntypedata = isMenu[0] ? unsigntypequery.Where(x => x.ItemName != "值班").ToList() : unsigntypequery.ToList();
            //值班
            var zhiban = unsigntypequery.FirstOrDefault(x => x.ItemName == "值班");
            //缺勤统计
            var unsignquery = from q1 in userdata
                              join q2 in
                                  (from q1 in userdata
                                   join q2 in db.IQueryable<UnSignRecordEntity>() on q1.ID equals q2.UserId
                                   where q2.UnSignDate >= @from && q2.UnSignDate <= to
                                   select q2
                                          ) on q1.ID equals q2.UserId into into1
                              select new { q1.ID, unsign = into1.ToList() };

            //缺勤统计
            var unsigndata = unsignquery.ToList().Select(x => new { UserId = x.ID, Data = unsigntypedata.GroupJoin(x.unsign.GroupBy(y => y.Reason).Select(y => new { Reason = y.Key, Times = y.Count(), Hours = y.Sum(z => z.Hours) }), m => m.ItemName, n => n.Reason, (m, n) => n.DefaultIfEmpty(new { Reason = string.Empty, Times = 0, Hours = 0f }).Select(z => new AttendanceTypeEntity { Category = m.ItemName, Times = z.Times, Hours = z.Hours })).SelectMany(z => z) }).Select(x => x).ToList();


            //缺勤排序整合
            var data1 = userdata.GroupJoin(unsigndata, x => x.ID, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() : a.Data })).SelectMany(z => z).ToList();
            //出勤排序整合
            var data2 = userdata.GroupJoin(signdata, x => x.ID, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = 0 } } : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = a.SignCount } } })).SelectMany(z => z).ToList();


            //排班
            //获取区间数据
            int startYear = from.Year;
            int endYear = to.Year;
            int startMonth = from.Month;
            int endMonth = to.Month;
            var getList = (from q in db.IQueryable<WorkTimeSortEntity>()
                           where q.year >= startYear && q.year <= endYear && q.month >= startMonth && q.month <= endMonth && bzIDStr.Contains(q.departmentid)
                           select q).ToList();
            var pb = new List<UserAttendanceEntity>();
            foreach (var item in userdata)
            {
                var pbUser = new UserAttendanceEntity();
                pbUser.UserId = item.ID;
                var signUser = signquerys.FirstOrDefault(x => x.Key == item.ID);
                var pbUserData = new List<AttendanceTypeEntity>();
                var pbUserOneData = new AttendanceTypeEntity();
                pbUserOneData.Hours = 0;
                pbUserOneData.Times = 0;
                pbUserOneData.Category = "出勤班次";
                var ckTime = string.Empty;
                //个人考勤数据
                if (signUser != null)
                {
                    foreach (var sign in signUser.GroupBy(x => x.MeetingStartTime))
                    {
                        if (!workTime.Contains(sign.Key.Date) && isMenu[1])
                        {
                            continue;
                        }
                        var year = sign.Key.Year;
                        var month = sign.Key.Month;
                        var day = sign.Key.Day;
                        var start = new DateTime(year, month, day);
                        var end = new DateTime(year, month, day + 1);
                        var bc = getList.FirstOrDefault(x => x.departmentid == item.BZID && x.year == year && x.month == month);
                        if (bc != null)
                        {
                            var dayData = bc.timedata.Split(',');
                            if (dayData[day - 1].Length > 5)
                            {
                                if (sign.Count(x => x.IsSigned1 && x.IsSigned2) > 0)
                                {
                                    if (!ckTime.Contains(sign.Key.ToString("yyyy-MM-dd")))
                                    {
                                        ckTime += sign.Key.ToString("yyyy-MM-dd");
                                        pbUserOneData.Times++;
                                    }

                                }

                            }
                        }
                    }
                }
                pbUserData.Add(pbUserOneData);
                pbUser.Data = pbUserData;
                pb.Add(pbUser);
            }

            //整合
            var data = data2.Join(pb, x => x.ID, y => y.UserId, (x, y) => new UserAttendanceEntity() { UserId = x.ID, Data = x.Data.Concat(y.Data).ToList() });


            if (isMenu[0] && zhiban != null)
            {
                //获取值班情况
                var onduty = from q in db.IQueryable<OndutyEntity>()
                             where bzUser.Contains(q.ondutyuserid) && q.ondutytime >= @from && q.ondutytime <= to
                             group q by q.ondutyuserid into n
                             select n;

                var ondutylist = onduty.ToList();
                //var ondutyData = onduty.Select(x => new UserAttendanceEntity() { UserId = x.Key, Data =new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = x.Count() } } }).ToList();

                var data3 = userdata.GroupJoin(onduty, x => x.ID, y => y.Key, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = 0 } } : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = y.Count() } } })).SelectMany(z => z).ToList();

                data = data.Join(data3, x => x.UserId, y => y.ID, (x, y) => new UserAttendanceEntity() { UserId = x.UserId, Data = x.Data.Concat(y.Data).ToList() });
            }
            data = data.Join(data1, x => x.UserId, y => y.ID, (x, y) => new UserAttendanceEntity() { UserId = x.UserId, Data = x.Data.Concat(y.Data).ToList() });

            var dataquery = userdata.Join(data, x => x.ID, y => y.UserId, (x, y) => new { x.ID, x.Name, x.BZID, x.BZName, x.Photo, x.Planer, x.EntryDate, y.Data }).OrderByDescending(x => x.BZName).ToList();

            //转化datatable
            var ListTabel = new DataTable();
            ListTabel.Columns.Add("userid", Type.GetType("System.String"));
            ListTabel.Columns.Add("bzid", Type.GetType("System.String"));
            ListTabel.Columns.Add("bz", Type.GetType("System.String"));
            ListTabel.Columns.Add("mc", Type.GetType("System.String"));
            ListTabel.Columns.Add("cqts", Type.GetType("System.String"));
            ListTabel.Columns.Add("cqbc", Type.GetType("System.String"));
            if (isMenu[0] && zhiban != null)
            {
                ListTabel.Columns.Add(zhiban.ItemValue, Type.GetType("System.String"));
            }
            foreach (var item in unsigntypedata)
            {
                ListTabel.Columns.Add(item.ItemValue, Type.GetType("System.String"));
            }

            foreach (var item in dataquery)
            {
                DataRow rows = ListTabel.NewRow();
                rows[0] = item.ID;
                rows[1] = item.BZID;
                rows[2] = item.BZName;
                rows[3] = item.Name;
                int i = 4;
                foreach (var ItemValue in item.Data)
                {
                    if (ItemValue.Category == "值班" || ItemValue.Category == "出勤" || ItemValue.Category == "出勤班次")
                    {
                        rows[i] = ItemValue.Times;
                    }
                    else
                    {
                        rows[i] = ItemValue.Hours;
                    }

                    i++;
                }
                ListTabel.Rows.Add(rows);
            }


            return ListTabel;
        }

        public DataTable GetAttendanceUserData3(string UserId, string deptid, DateTime from, DateTime to, bool[] isMenu)
        {
            var db = new RepositoryFactory().BaseRepository();

            //排序
            var sortquery = from q in db.IQueryable<PeopleEntity>()
                            where deptid.Contains(q.BZID) && q.FingerMark == "yes" && q.ID == UserId
                            orderby q.BZCode
                            select new { q.ID, q.Name, q.Photo, q.Planer, q.EntryDate, q.BZName, q.BZID };

            var userdata = sortquery.ToList();
            var bzIDStr = string.Join(",", userdata.Select(x => x.BZID));
            var bzUser = string.Join(",", userdata.Select(x => x.ID));
            var bqsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                              where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to && bzUser.Contains(ss.UserId)
                              orderby ms.MeetingStartTime ascending
                              select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned };
            List<string> clear = new List<string>();
            List<DateTime> cleartime = new List<DateTime>();
            foreach (var item in bqsignquery)
            {
                if (!cleartime.Contains(item.MeetingStartTime.Date))
                {
                    clear.Add(item.MeetingId);
                }
                else
                {
                    cleartime.Add(item.MeetingStartTime.Date);
                }
            }
            var clearbqsignquery = bqsignquery.Where(x => clear.Contains(x.MeetingId));

            var bhsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                              where clear.Contains(ms.MeetingId)
                              select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned };
            //出勤
            var signquery = from q1 in
                                clearbqsignquery
                            join q2 in bhsignquery
        on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
                            select g;
            //出勤
            //var signquery = from q1 in
            //                    (from ms in db.IQueryable<WorkmeetingEntity>()
            //                     join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
            //                     where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
            //                     select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
            //                join q2 in
            //                    (from ms in db.IQueryable<WorkmeetingEntity>()
            //                     join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
            //                     where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
            //                     select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
            //                where bzUser.Contains(q1.UserId)
            //                group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
            //                select g;

            ////出勤统计
            //var query = from q1 in userquery
            //            join q2 in signquery on q1.UserId equals q2.Key into into1
            //            from d1 in into1.DefaultIfEmpty()
            //            select new { q1.UserId, d1 };
            //出勤统计
            var signquerys = signquery.ToList();
            //var signdata = signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => n.IsSigned1&&n.IsSigned2) > 0) });
            var signdata = signquerys.Select(x => new { UserId = x.Key, Sign = x }).ToList();

            //缺勤类型
            var unsigntypequery = from q1 in db.IQueryable<DataItemEntity>()
                                  join q2 in db.IQueryable<DataItemDetailEntity>() on q1.ItemId equals q2.ItemId
                                  where q1.ItemName == "缺勤原因"
                                  select q2;
            //缺勤类型
            var unsigntypedata = unsigntypequery.ToList();

            //值班统计
            var unsignquery = (from q1 in userdata
                               join q2 in db.IQueryable<UnSignRecordEntity>() on q1.ID equals q2.UserId
                               where q2.UnSignDate >= @from && q2.UnSignDate <= to
                               select q2).ToList();
            //是否值班
            var Isunsignquery = isMenu[0] ? (from q1 in userdata
                                             join q2 in db.IQueryable<OndutyEntity>() on q1.ID equals q2.ondutyuserid
                                             where q2.ondutytime >= @from && q2.ondutytime <= to
                                             select q2).ToList() : new List<OndutyEntity>();
            //是否签到
            var IsFace = isMenu[1] ? (from q1 in userdata
                                      join q2 in db.IQueryable<FaceAttendanceTimeEntity>() on q1.ID equals q2.userid
                                      where q2.worktime >= @from && q2.worktime <= to
                                      select q2).ToList() : new List<FaceAttendanceTimeEntity>();
            //考勤详情
            var IsFaceData = isMenu[1] ? (from q1 in userdata
                                          join q2 in db.IQueryable<FaceAttendanceEntity>() on q1.ID equals q2.ondutyuserid
                                          where q2.ondutytime >= @from && q2.ondutytime <= to
                                          select q2).ToList() : new List<FaceAttendanceEntity>();


            //排班
            //获取区间数据
            int startYear = from.Year;
            int endYear = to.Year;
            int startMonth = from.Month;
            int endMonth = to.Month;
            var getList = (from q in db.IQueryable<WorkTimeSortEntity>()
                           where q.year >= startYear && q.year <= endYear && q.month >= startMonth && q.month <= endMonth && bzIDStr.Contains(q.departmentid)
                           select q).ToList();

            //转化datatable
            var ListTabel = new DataTable();
            ListTabel.Columns.Add("bz", Type.GetType("System.String"));
            ListTabel.Columns.Add("mc", Type.GetType("System.String"));
            ListTabel.Columns.Add("cqtime", Type.GetType("System.String"));
            ListTabel.Columns.Add("bc", Type.GetType("System.String"));
            ListTabel.Columns.Add("cq", Type.GetType("System.String"));
            ListTabel.Columns.Add("sb", Type.GetType("System.String"));
            ListTabel.Columns.Add("xb", Type.GetType("System.String"));
            ListTabel.Columns.Add("zb", Type.GetType("System.String"));
            ListTabel.Columns.Add("remark", Type.GetType("System.String"));
            string TimeCk = string.Empty;
            foreach (var item in signquerys)
            {

                var user = userdata.FirstOrDefault(x => x.ID == item.Key);
                foreach (var DTime in item.OrderBy(x => x.MeetingStartTime))
                {
                    var year = DTime.MeetingStartTime.Year;
                    var month = DTime.MeetingStartTime.Month;
                    var day = DTime.MeetingStartTime.Day;
                    if (TimeCk.Contains(DTime.MeetingStartTime.ToString("yyyy-MM-dd")))
                    {
                        continue;
                    }
                    TimeCk += DTime.MeetingStartTime.ToString("yyyy-MM-dd");
                    DataRow rows = ListTabel.NewRow();
                    rows[0] = user.BZName;
                    rows[1] = user.Name;
                    rows[2] = DTime.MeetingStartTime.ToString("yyyy-MM-dd");
                    var Start = new DateTime(year, month, day);
                    var End = new DateTime(year, month, day + 1);
                    if (isMenu[1])
                    {
                        var qd = IsFaceData.Where(x => x.ondutytime >= Start && x.ondutytime <= End).ToList();
                        var bc = getList.FirstOrDefault(x => x.departmentid == user.BZID && x.year == year && x.month == month);
                        if (bc != null)
                        {
                            var bcDetail = bc.timedata.Split(',');
                            if (bcDetail[day - 1].Length > 5)
                            {
                                var bcData = bcDetail[day - 1].Split('(');
                                var Time = bcData[0].Split('-');
                                var Detail = bcData[1].Replace(")", "");
                                rows[3] = Detail;

                            }
                            else
                            {
                                rows[3] = "";

                            }
                        }
                        else
                        {
                            rows[3] = "";
                        }
                        var sb = qd.Where(x => x.ondutyshift == "上班打卡" || x.ondutyshift == "迟到").OrderBy(x => x.ondutytime).ToList();
                        var xb = qd.Where(x => x.ondutyshift == "下班打卡" || x.ondutyshift == "早退").OrderBy(x => x.ondutytime).ToList();
                        if (sb.Count > 0)
                        {
                            rows[5] = sb[0].ondutytime.ToString("HH:mm") + "考勤签到";
                        }
                        else
                        {
                            rows[5] = "";
                        }
                        if (xb.Count > 0)
                        {
                            rows[6] = xb[0].ondutytime.ToString("HH:mm") + "考勤签到";
                        }
                        else
                        {
                            rows[6] = "";
                        }
                    }
                    else
                    {
                        var bc = getList.FirstOrDefault(x => x.departmentid == user.BZID && x.year == year && x.month == month);

                        if (bc != null)
                        {
                            var bcDetail = bc.timedata.Split(',');
                            if (bcDetail[day - 1].Length > 5)
                            {
                                var bcData = bcDetail[day - 1].Split('(');
                                var Time = bcData[0].Split('-');
                                var Detail = bcData[1].Replace(")", "");
                                rows[3] = Detail;
                                rows[5] = Time[0];
                                rows[6] = Time[1];
                            }
                            else
                            {
                                rows[3] = "";
                                rows[5] = "";
                                rows[6] = "";
                            }
                        }
                        else
                        {
                            rows[3] = "";
                            rows[5] = "";
                            rows[6] = "";
                        }
                    }

                    if (isMenu[1])
                    {
                        var cq = IsFace.Where(x => x.worktime >= Start && x.worktime <= End);
                        if (DTime.IsSigned1 && DTime.IsSigned2 && cq.Count() > 0)
                        {
                            rows[4] = "出勤";
                        }
                        else
                        {
                            var Nounsign = unsignquery.Where(x => x.StartTime >= Start && x.StartTime <= End && x.Reason != "值班");
                            var NounsignS = string.Join(",", Nounsign.Select(x => x.Reason));
                            rows[4] = NounsignS.Length > 2 ? NounsignS : "异常";
                        }
                    }
                    else
                    {
                        if (DTime.IsSigned1 && DTime.IsSigned2)
                        {
                            rows[4] = "出勤";
                        }
                        else
                        {
                            var Nounsign = unsignquery.Where(x => x.StartTime >= Start && x.StartTime <= End && x.Reason != "值班");
                            var NounsignS = string.Join(",", Nounsign.Select(x => x.Reason));
                            rows[4] = NounsignS.Length > 2 ? NounsignS : "异常";
                        }
                    }

                    if (isMenu[0])
                    {
                        var xd = Isunsignquery.Where(x => x.ondutytime >= Start && x.ondutytime <= End).OrderByDescending(x => x.ondutytime).ToList();
                        if (xd.Count > 0)
                        {
                            rows[7] = xd[0].ondutyshift + "(" + xd[0].ondutytime.ToString("HH:mm") + ")";
                        }
                        else
                        {
                            rows[7] = "";
                        }
                    }
                    else
                    {
                        var unsign = unsignquery.Where(x => x.UnSignDate >= Start && x.UnSignDate <= End && x.Reason == "值班");
                        var zbDetail = string.Empty;
                        foreach (var zbData in unsign)
                        {
                            var HourData = zbData.EndTime.HasValue ? zbData.EndTime.Value.ToString("HH:mm") : "";
                            if (!zbDetail.Contains(zbData.ReasonRemark))
                            {
                                if (zbDetail.Length >= 2)
                                {
                                    zbDetail += "," + zbData.ReasonRemark;
                                }
                                else
                                {
                                    zbDetail += zbData.ReasonRemark;
                                }

                            }
                        }
                        rows[7] = zbDetail;
                    }




                    rows[8] = "";
                    ListTabel.Rows.Add(rows);

                }

            }


            return ListTabel;
        }
        public DataTable GetAttendanceExportData3(string name, string deptid, DateTime from, DateTime to, bool[] isMenu)
        {
            var db = new RepositoryFactory().BaseRepository();

            //排序
            var sortquery = from q in db.IQueryable<PeopleEntity>()
                            where deptid.Contains(q.BZID) && q.FingerMark == "yes"
                            orderby q.BZCode
                            select new { q.ID, q.Name, q.Photo, q.Planer, q.EntryDate, q.BZName, q.BZID };
            if (!string.IsNullOrEmpty(name))
            {
                sortquery = sortquery.Where(x => x.BZName.Contains(name) || x.Name.Contains(name));
            }

            var userdata = sortquery.ToList();
            var bzIDStr = string.Join(",", userdata.Select(x => x.BZID));
            var bzUser = string.Join(",", userdata.Select(x => x.ID));
            List<DateTime> workTime = new List<DateTime>();
            //是否启用模块
            if (isMenu[1])
            {
                //获取签到考情情况
                var Face = from q in db.IQueryable<FaceAttendanceTimeEntity>()
                           where bzUser.Contains(q.userid) && q.worktime >= @from && q.worktime <= to
                           select q;
                if (Face.Count() > 0)
                {
                    var getworkTime = Face.Select(x => x.worktime).ToList();
                    foreach (var item in getworkTime)
                    {
                        workTime.Add(item.Date);
                    }
                }

            }
            var bqsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                              where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to && bzUser.Contains(ss.UserId)
                              orderby ms.MeetingStartTime ascending
                              select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned };
            List<string> clear = new List<string>();
            List<DateTime> cleartime = new List<DateTime>();
            foreach (var item in bqsignquery)
            {
                if (!cleartime.Contains(item.MeetingStartTime.Date))
                {
                    clear.Add(item.MeetingId);
                }
                else
                {
                    cleartime.Add(item.MeetingStartTime.Date);
                }
            }
            var clearbqsignquery = bqsignquery.Where(x => clear.Contains(x.MeetingId));

            var bhsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                              where clear.Contains(ms.MeetingId)
                              select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned };
            //出勤
            var signquery = from q1 in
                                clearbqsignquery
                            join q2 in bhsignquery
        on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
                            select g;
            //出勤
            //var signquery = from q1 in
            //                    (from ms in db.IQueryable<WorkmeetingEntity>()
            //                     join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
            //                     where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
            //                     select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
            //                join q2 in
            //                    (from ms in db.IQueryable<WorkmeetingEntity>()
            //                     join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
            //                     where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
            //                     select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
            //                where bzUser.Contains(q1.UserId)
            //                group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
            //                select g;
            ////出勤统计
            //var query = from q1 in userquery
            //            join q2 in signquery on q1.UserId equals q2.Key into into1
            //            from d1 in into1.DefaultIfEmpty()
            //            select new { q1.UserId, d1 };
            //出勤统计
            var signquerys = signquery.ToList();

            var signdata = isMenu[1] ? signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => (n.IsSigned1 || n.IsSigned2) && workTime.Contains(n.MeetingStartTime.Date)) > 0) }) : signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => n.IsSigned1 || n.IsSigned2) > 0) });
            //缺勤类型
            var unsigntypequery = from q1 in db.IQueryable<DataItemEntity>()
                                  join q2 in db.IQueryable<DataItemDetailEntity>() on q1.ItemId equals q2.ItemId
                                  where q1.ItemName == "缺勤原因" && q2.ItemName != "不参会"
                                  select q2;
            //缺勤类型
            var unsigntypedata = isMenu[0] ? unsigntypequery.Where(x => x.ItemName != "值班").ToList() : unsigntypequery.ToList();
            //值班
            var zhiban = unsigntypequery.FirstOrDefault(x => x.ItemName == "值班");
            //缺勤统计
            var unsignquery = from q1 in userdata
                              join q2 in
                                  (from q1 in userdata
                                   join q2 in db.IQueryable<UnSignRecordEntity>() on q1.ID equals q2.UserId
                                   where q2.UnSignDate >= @from && q2.UnSignDate <= to
                                   select q2
                                          ) on q1.ID equals q2.UserId into into1
                              select new { q1.ID, unsign = into1.ToList() };

            //缺勤统计
            var unsigndata = unsignquery.ToList().Select(x => new { UserId = x.ID, Data = unsigntypedata.GroupJoin(x.unsign.GroupBy(y => y.Reason).Select(y => new { Reason = y.Key, Times = y.Count(), Hours = y.Sum(z => z.Hours) }), m => m.ItemName, n => n.Reason, (m, n) => n.DefaultIfEmpty(new { Reason = string.Empty, Times = 0, Hours = 0f }).Select(z => new AttendanceTypeEntity { Category = m.ItemName, Times = z.Times, Hours = z.Hours })).SelectMany(z => z) }).Select(x => x).ToList();


            //缺勤排序整合
            var data1 = userdata.GroupJoin(unsigndata, x => x.ID, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() : a.Data })).SelectMany(z => z).ToList();
            //出勤排序整合
            var data2 = userdata.GroupJoin(signdata, x => x.ID, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = 0 } } : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = a.SignCount } } })).SelectMany(z => z).ToList();


            //排班
            //获取区间数据
            int startYear = from.Year;
            int endYear = to.Year;
            int startMonth = from.Month;
            int endMonth = to.Month;
            var getList = (from q in db.IQueryable<WorkTimeSortEntity>()
                           where q.year >= startYear && q.year <= endYear && q.month >= startMonth && q.month <= endMonth && bzIDStr.Contains(q.departmentid)
                           select q).ToList();
            var pb = new List<UserAttendanceEntity>();
            foreach (var item in userdata)
            {
                var pbUser = new UserAttendanceEntity();
                pbUser.UserId = item.ID;
                var signUser = signquerys.FirstOrDefault(x => x.Key == item.ID);
                var pbUserData = new List<AttendanceTypeEntity>();
                var pbUserOneData = new AttendanceTypeEntity();
                pbUserOneData.Hours = 0;
                pbUserOneData.Times = 0;
                pbUserOneData.Category = "出勤班次";
                var ckTime = string.Empty;
                //个人考勤数据
                if (signUser != null)
                {
                    foreach (var sign in signUser.GroupBy(x => x.MeetingStartTime))
                    {
                        if (!workTime.Contains(sign.Key.Date) && isMenu[1])
                        {
                            continue;
                        }
                        var year = sign.Key.Year;
                        var month = sign.Key.Month;
                        var day = sign.Key.Day;
                        var start = new DateTime(year, month, day);
                        var end = new DateTime(year, month, day + 1);
                        var bc = getList.FirstOrDefault(x => x.departmentid == item.BZID && x.year == year && x.month == month);
                        if (bc != null)
                        {
                            var dayData = bc.timedata.Split(',');
                            if (dayData[day - 1].Length > 5)
                            {
                                if (sign.Count(x => x.IsSigned1 && x.IsSigned2) > 0)
                                {
                                    if (!ckTime.Contains(sign.Key.ToString("yyyy-MM-dd")))
                                    {
                                        ckTime += sign.Key.ToString("yyyy-MM-dd");
                                        pbUserOneData.Times++;
                                    }

                                }

                            }
                        }
                    }
                }
                pbUserData.Add(pbUserOneData);
                pbUser.Data = pbUserData;
                pb.Add(pbUser);
            }

            //整合
            var data = data2.Join(pb, x => x.ID, y => y.UserId, (x, y) => new UserAttendanceEntity() { UserId = x.ID, Data = x.Data.Concat(y.Data).ToList() });
            if (isMenu[0] && zhiban != null)
            {
                //获取值班情况
                var onduty = from q in db.IQueryable<OndutyEntity>()
                             where bzUser.Contains(q.ondutyuserid) && q.ondutytime >= @from && q.ondutytime <= to
                             group q by q.ondutyuserid into n
                             select n;

                var ondutylist = onduty.ToList();
                //var ondutyData = onduty.Select(x => new UserAttendanceEntity() { UserId = x.Key, Data =new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = x.Count() } } }).ToList();

                var data3 = userdata.GroupJoin(onduty, x => x.ID, y => y.Key, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = 0 } } : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = y.Count() } } })).SelectMany(z => z).ToList();

                data = data.Join(data3, x => x.UserId, y => y.ID, (x, y) => new UserAttendanceEntity() { UserId = x.UserId, Data = x.Data.Concat(y.Data).ToList() });
            }
            data = data.Join(data1, x => x.UserId, y => y.ID, (x, y) => new UserAttendanceEntity() { UserId = x.UserId, Data = x.Data.Concat(y.Data).ToList() });

            var dataquery = userdata.Join(data, x => x.ID, y => y.UserId, (x, y) => new { x.ID, x.Name, x.BZID, x.BZName, x.Photo, x.Planer, x.EntryDate, y.Data }).OrderByDescending(x => x.BZName).ToList();
            //转化datatable
            var ListTabel = new DataTable();
            ListTabel.Columns.Add("班组", Type.GetType("System.String"));
            ListTabel.Columns.Add("名称", Type.GetType("System.String"));
            ListTabel.Columns.Add("出勤天数", Type.GetType("System.String"));
            ListTabel.Columns.Add("出勤班次", Type.GetType("System.String"));
            foreach (var item in unsigntypequery)
            {
                ListTabel.Columns.Add(item.ItemName, Type.GetType("System.String"));
            }
            foreach (var item in dataquery)
            {
                DataRow rows = ListTabel.NewRow();
                rows[0] = item.BZName;
                rows[1] = item.Name;
                int i = 2;
                foreach (var ItemValue in item.Data)
                {
                    if (ItemValue.Category == "值班")
                    {
                        rows[i] = ItemValue.Times;
                    }
                    else
                    {
                        rows[i] = ItemValue.Hours;
                    }

                    i++;
                }
                ListTabel.Rows.Add(rows);
            }


            return ListTabel;
        }
        public List<UserAttendanceEntity> GetAttendanceData2(string deptid, DateTime from, DateTime to, bool[] isMenu)
        {
            var db = new RepositoryFactory().BaseRepository();

            //排序
            var sortquery = from q in db.IQueryable<PeopleEntity>()
                            where deptid.Contains(q.BZID) && q.FingerMark == "yes"
                            orderby q.BZCode
                            select new { q.ID, q.Name, q.Photo, q.Planer, q.EntryDate, q.BZName, q.BZID };


            var userdata = sortquery.ToList();
            var bzIDStr = string.Join(",", userdata.Select(x => x.BZID));
            var bzUser = string.Join(",", userdata.Select(x => x.ID));
            List<DateTime> workTime = new List<DateTime>();
            //是否启用模块
            if (isMenu[1])
            {
                //获取签到考情情况
                var Face = from q in db.IQueryable<FaceAttendanceTimeEntity>()
                           where bzUser.Contains(q.userid) && q.worktime >= @from && q.worktime <= to
                           select q;
                if (Face.Count() > 0)
                {
                    var getworkTime = Face.Select(x => x.worktime).ToList();
                    foreach (var item in getworkTime)
                    {
                        workTime.Add(item.Date);
                    }
                }

            }
            var bqsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                              where ms.MeetingType == "班前会" && bzIDStr.Contains(ms.GroupId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to && bzUser.Contains(ss.UserId)
                              orderby ms.MeetingStartTime ascending
                              select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned };
            List<string> clear = new List<string>();
            List<DateTime> cleartime = new List<DateTime>();
            foreach (var item in bqsignquery)
            {
                if (!cleartime.Contains(item.MeetingStartTime.Date))
                {
                    clear.Add(item.MeetingId);
                }
                else
                {
                    cleartime.Add(item.MeetingStartTime.Date);
                }
            }
            var clearbqsignquery = bqsignquery.Where(x => clear.Contains(x.MeetingId));

            var bhsignquery = from ms in db.IQueryable<WorkmeetingEntity>()
                              join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                              where clear.Contains(ms.MeetingId)
                              select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned };
            //出勤
            var signquery = from q1 in
                                clearbqsignquery
                            join q2 in bhsignquery
        on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            group new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned } by q1.UserId into g
                            select g;
            ////出勤统计
            //var query = from q1 in userquery
            //            join q2 in signquery on q1.UserId equals q2.Key into into1
            //            from d1 in into1.DefaultIfEmpty()
            //            select new { q1.UserId, d1 };
            //出勤统计
            var signquerys = signquery.ToList();

            var signdata = isMenu[1] ? signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => (n.IsSigned1 || n.IsSigned2) && workTime.Contains(n.MeetingStartTime.Date)) > 0) }) : signquerys.Select(x => new { UserId = x.Key, SignCount = x.Count() == 0 ? 0 : x.GroupBy(y => y.MeetingStartTime.Date).Count(z => z.Count(n => n.IsSigned1 || n.IsSigned2) > 0) });
            //缺勤类型
            var unsigntypequery = from q1 in db.IQueryable<DataItemEntity>()
                                  join q2 in db.IQueryable<DataItemDetailEntity>() on q1.ItemId equals q2.ItemId
                                  where q1.ItemName == "缺勤原因" && q2.ItemName != "不参会"
                                  select q2;
            //缺勤类型
            var unsigntypedata = isMenu[0] ? unsigntypequery.Where(x => x.ItemName != "值班").ToList() : unsigntypequery.ToList();
            //值班
            var zhiban = unsigntypequery.FirstOrDefault(x => x.ItemName == "值班");
            //缺勤统计
            var unsignquery = from q1 in userdata
                              join q2 in
                                  (from q1 in userdata
                                   join q2 in db.IQueryable<UnSignRecordEntity>() on q1.ID equals q2.UserId
                                   where q2.UnSignDate >= @from && q2.UnSignDate <= to
                                   select q2
                                          ) on q1.ID equals q2.UserId into into1
                              select new { q1.ID, unsign = into1.ToList() };

            //缺勤统计
            var unsigndata = unsignquery.ToList().Select(x => new { UserId = x.ID, Data = unsigntypedata.GroupJoin(x.unsign.GroupBy(y => y.Reason).Select(y => new { Reason = y.Key, Times = y.Count(), Hours = y.Sum(z => z.Hours) }), m => m.ItemName, n => n.Reason, (m, n) => n.DefaultIfEmpty(new { Reason = string.Empty, Times = 0, Hours = 0f }).Select(z => new AttendanceTypeEntity { Category = m.ItemName, Times = z.Times, Hours = z.Hours })).SelectMany(z => z) }).Select(x => x).ToList();


            //缺勤排序整合
            var data1 = userdata.GroupJoin(unsigndata, x => x.ID, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() : a.Data })).SelectMany(z => z).ToList();
            //出勤排序整合
            var data2 = userdata.GroupJoin(signdata, x => x.ID, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = 0 } } : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = a.SignCount } } })).SelectMany(z => z).ToList();


            //排班
            //获取区间数据
            int startYear = from.Year;
            int endYear = to.Year;
            int startMonth = from.Month;
            int endMonth = to.Month;
            var getList = (from q in db.IQueryable<WorkTimeSortEntity>()
                           where q.year >= startYear && q.year <= endYear && q.month >= startMonth && q.month <= endMonth && bzIDStr.Contains(q.departmentid)
                           select q).ToList();
            var pb = new List<UserAttendanceEntity>();
            foreach (var item in userdata)
            {
                var pbUser = new UserAttendanceEntity();
                pbUser.UserId = item.ID;
                var signUser = signquerys.FirstOrDefault(x => x.Key == item.ID);
                var pbUserData = new List<AttendanceTypeEntity>();
                var pbUserOneData = new AttendanceTypeEntity();
                pbUserOneData.Hours = 0;
                pbUserOneData.Times = 0;
                pbUserOneData.Category = "出勤班次";
                var ckTime = string.Empty;
                //个人考勤数据
                if (signUser != null)
                {
                    foreach (var sign in signUser.GroupBy(x => x.MeetingStartTime))
                    {
                        if (!workTime.Contains(sign.Key.Date) && isMenu[1])
                        {
                            continue;
                        }
                        var year = sign.Key.Year;
                        var month = sign.Key.Month;
                        var day = sign.Key.Day;
                        var start = new DateTime(year, month, day);
                        var end = new DateTime(year, month, day).AddDays(1);
                        var bc = getList.FirstOrDefault(x => x.departmentid == item.BZID && x.year == year && x.month == month);
                        if (bc != null)
                        {
                            var dayData = bc.timedata.Split(',');
                            if (dayData[day - 1].Length > 5)
                            {
                                if (sign.Count(x => x.IsSigned1 && x.IsSigned2) > 0)
                                {
                                    if (!ckTime.Contains(sign.Key.ToString("yyyy-MM-dd")))
                                    {
                                        ckTime += sign.Key.ToString("yyyy-MM-dd");
                                        pbUserOneData.Times++;
                                    }

                                }

                            }
                        }
                    }
                }
                pbUserData.Add(pbUserOneData);
                pbUser.Data = pbUserData;
                pb.Add(pbUser);
            }

            //整合
            var data = data2.Join(pb, x => x.ID, y => y.UserId, (x, y) => new UserAttendanceEntity() { UserId = x.ID, Data = x.Data.Concat(y.Data).ToList() });
            if (isMenu[0] && zhiban != null)
            {
                //获取值班情况
                var onduty = from q in db.IQueryable<OndutyEntity>()
                             where bzUser.Contains(q.ondutyuserid) && q.ondutytime >= @from && q.ondutytime <= to
                             group q by q.ondutyuserid into n
                             select n;

                var ondutylist = onduty.ToList();
                //var ondutyData = onduty.Select(x => new UserAttendanceEntity() { UserId = x.Key, Data =new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = x.Count() } } }).ToList();

                var data3 = userdata.GroupJoin(onduty, x => x.ID, y => y.Key, (x, y) => y.DefaultIfEmpty().Select(a => new { x.ID, x.Name, x.BZName, Data = a == null ? new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = 0 } } : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "值班", Times = y.Count() } } })).SelectMany(z => z).ToList();

                data = data.Join(data3, x => x.UserId, y => y.ID, (x, y) => new UserAttendanceEntity() { UserId = x.UserId, Data = x.Data.Concat(y.Data).ToList() });
            }
            data = data.Join(data1, x => x.UserId, y => y.ID, (x, y) => new UserAttendanceEntity() { UserId = x.UserId, Data = x.Data.Concat(y.Data).ToList() });

            var dataquery = userdata.Join(data, x => x.ID, y => y.UserId, (x, y) => new { x.ID, x.Name, x.Photo, x.Planer, x.EntryDate, y.Data }).OrderBy(x => x.Planer).ThenBy(x => x.EntryDate).Select(x => new UserAttendanceEntity() { UserId = x.ID, UserName = x.Name, Data = x.Data, Photo = x.Photo });

            return dataquery.ToList();
        }



        /// <summary>
        /// web终端统计出勤，与android终端统计方式不一样
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<AttendanceEntity> GetAttendanceData(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var userquery = from q1 in db.IQueryable<UserEntity>()
                            join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID into into1
                            from d1 in into1.DefaultIfEmpty()
                            where q1.DepartmentId == deptid
                            select new { q1.UserId, q1.RealName, Sort1 = d1 == null ? "99" : d1.Planer, Sort2 = q1.CreateDate };

            var users = userquery.OrderBy(x => new { x.Sort1, x.Sort2 }).ToList();

            var dataquery = from q in
                                (
                                    from q1 in db.IQueryable<WorkmeetingEntity>()
                                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
                                    where q1.MeetingType == "班前会" && q1.IsOver && q1.GroupId == deptid && q1.MeetingStartTime > @from && q1.MeetingStartTime < to
                                    select new { q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                    )
                                    .Concat(
                                    from q1 in db.IQueryable<WorkmeetingEntity>()
                                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.OtherMeetingId equals q2.MeetingId
                                    where q1.MeetingType == "班前会" && q1.IsOver && q1.GroupId == deptid && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.MeetingId != null
                                    select new { q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                    )
                            select q;

            var data = dataquery.ToList();

            var itemquery = from q1 in db.IQueryable<DataItemDetailEntity>()
                            join q2 in db.IQueryable<DataItemEntity>() on q1.ItemId equals q2.ItemId
                            where q2.ItemName == "缺勤原因"
                            select q1;


            var result = new List<AttendanceEntity>();
            foreach (var item in users)
            {
                var g = data.Where(x => x.UserId == item.UserId).GroupBy(x => x.MeetingStartTime.Date);
                var chuqin = 0d;
                var tiaoxiu = 0d;
                var gongxiu = 0d;
                var bingjia = 0d;
                var shijia = 0d;
                var chuchai = 0d;
                var qita = 0d;

                foreach (var gitem in g)
                {
                    switch (gitem.Count())
                    {
                        case 0:
                            break;
                        case 1:
                            tiaoxiu += gitem.Sum(x => !x.IsSigned && x.Reason == "调休" ? 0.5 : 0);
                            gongxiu += gitem.Sum(x => !x.IsSigned && x.Reason == "公休" ? 0.5 : 0);
                            bingjia += gitem.Sum(x => !x.IsSigned && x.Reason == "病假" ? 0.5 : 0);
                            shijia += gitem.Sum(x => !x.IsSigned && x.Reason == "事假" ? 0.5 : 0);
                            chuchai += gitem.Sum(x => !x.IsSigned && x.Reason == "出差" ? 0.5 : 0);
                            qita += gitem.Sum(x => !x.IsSigned && x.Reason == "其他" ? 0.5 : 0);
                            chuqin += gitem.Sum(x => x.IsSigned ? 0.5 : 0);
                            break;
                        case 2:
                            goto case 1;
                        default:
                            var signins = gitem.OrderByDescending(x => x.CreateDate).ToList();
                            if (signins[0].IsSigned)
                                chuqin += 0.5;
                            else
                            {
                                switch (signins[0].Reason)
                                {
                                    case "调休":
                                        tiaoxiu += 0.5;
                                        break;
                                    case "公休":
                                        gongxiu += 0.5;
                                        break;
                                    case "病假":
                                        bingjia += 0.5;
                                        break;
                                    case "事假":
                                        shijia += 0.5;
                                        break;
                                    case "出差":
                                        chuchai += 0.5;
                                        break;
                                    case "其他":
                                        qita += 0.5;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (signins[1].IsSigned)
                                chuqin += 0.5;
                            else
                            {
                                switch (signins[1].Reason)
                                {
                                    case "调休":
                                        tiaoxiu += 0.5;
                                        break;
                                    case "公休":
                                        gongxiu += 0.5;
                                        break;
                                    case "病假":
                                        bingjia += 0.5;
                                        break;
                                    case "事假":
                                        shijia += 0.5;
                                        break;
                                    case "出差":
                                        chuchai += 0.5;
                                        break;
                                    case "其他":
                                        qita += 0.5;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                    }
                }

                result.Add(new AttendanceEntity() { UserId = item.UserId, UserName = item.RealName, Chuqin = (decimal)chuqin, Tiaoxiu = (decimal)tiaoxiu, Gongxiu = (decimal)gongxiu, Bingjia = (decimal)bingjia, Shijia = (decimal)shijia, Chuchai = (decimal)chuchai, Qita = (decimal)qita });
            }


            //var query = from q1 in db.IQueryable<UserEntity>()
            //            join q2 in (
            //                from q in (
            //                    from q1 in db.IQueryable<WorkmeetingEntity>()
            //                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
            //                    where q1.MeetingType == "班前会" && q1.IsOver && q1.GroupId == deptid && q1.MeetingStartTime > @from && q1.MeetingStartTime < to
            //                    select new { MeetingStartTime = q1.MeetingStartTime.ToString("yyyyMMdd"), q2.UserId, q2.SigninId, q2.IsSigned, q2.CreateDate }).Concat(
            //                    from q1 in db.IQueryable<WorkmeetingEntity>()
            //                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.OtherMeetingId equals q2.MeetingId
            //                    where q1.MeetingType == "班前会" && q1.IsOver && q1.GroupId == deptid && q1.MeetingStartTime > @from && q1.MeetingStartTime < to
            //                    select new { MeetingStartTime = q1.MeetingStartTime.ToString("yyyyMMdd"), q2.UserId, q2.SigninId, q2.IsSigned, q2.CreateDate })
            //                group q by q.UserId into g
            //                select new { g.Key, Chuqin = g.GroupBy(x => x.MeetingStartTime).Count(x => x.Count(y => y.IsSigned) > 0), Tiaoxiu = 0, Gongxiu = 0, Bingjia = 0, Shijia = 0, Chuchai = 0, Qita = 0 }) on q1.UserId equals q2.Key into into1
            //            from q3 in into1.DefaultIfEmpty()
            //            select new { q1.UserId, q1.RealName, Chuqin = q3 == null ? 0 : q3.Chuqin, Tiaoxiu = q3 == null ? 0 : q3.Tiaoxiu, Gongxiu = q3 == null ? 0 : q3.Gongxiu, Bingjia = q3 == null ? 0 : q3.Bingjia, Shijia = q3 == null ? 0 : q3.Shijia, Chuchai = q3 == null ? 0 : q3.Chuchai, Qita = q3 == null ? 0 : q3.Qita };




            //var query = from q1 in db.IQueryable<UserEntity>()
            //            join q2 in
            //                (from q1 in db.IQueryable<WorkmeetingEntity>()
            //                 join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
            //                 where q1.IsOver && q1.GroupId == deptid && q1.MeetingStartTime > @from && q1.MeetingStartTime < to
            //                 group q2 by q2.UserId into g
            //                 select new { g.Key, Chuqin = g.Sum(x => x.IsSigned ? 0.5 : 0), Tiaoxiu = g.Sum(x => (!x.IsSigned && x.Reason == "调休") ? 0.5 : 0), Gongxiu = g.Sum(x => (!x.IsSigned && x.Reason == "公休") ? 0.5 : 0), Bingjia = g.Sum(x => (!x.IsSigned && x.Reason == "病假") ? 0.5 : 0), Shijia = g.Sum(x => (!x.IsSigned && x.Reason == "事假") ? 0.5 : 0), Chuchai = g.Sum(x => (!x.IsSigned && x.Reason == "出差") ? 0.5 : 0), Qita = g.Sum(x => (!x.IsSigned && x.Reason == "其他") ? 0.5 : 0) }) on q1.UserId equals q2.Key into into1
            //            from q3 in into1.DefaultIfEmpty()
            //            where q1.DepartmentId == deptid
            //            select new { q1.UserId, q1.RealName, Chuqin = q3 == null ? 0 : q3.Chuqin, Tiaoxiu = q3 == null ? 0 : q3.Tiaoxiu, Gongxiu = q3 == null ? 0 : q3.Gongxiu, Bingjia = q3 == null ? 0 : q3.Bingjia, Shijia = q3 == null ? 0 : q3.Shijia, Chuchai = q3 == null ? 0 : q3.Chuchai, Qita = q3 == null ? 0 : q3.Qita };


            //var data = query.ToList();

            //return data.Select(x => new AttendanceEntity() { UserId = x.UserId, UserName = x.RealName, Chuqin = (decimal)x.Chuqin, Tiaoxiu = (decimal)x.Tiaoxiu, Gongxiu = (decimal)x.Gongxiu, Bingjia = (decimal)x.Bingjia, Shijia = (decimal)x.Shijia, Chuchai = (decimal)x.Chuchai, Qita = (decimal)x.Qita }).ToList();
            return result;
        }

        public AttendanceEntity GetMonthAttendance(string userid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month + 1, 1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(userid);

            var dataquery = from q in
                                (
                                    from q1 in db.IQueryable<WorkmeetingEntity>()
                                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
                                    where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.UserId == userid
                                    select new { q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                    )
                                    .Concat(
                                    from q1 in db.IQueryable<WorkmeetingEntity>()
                                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.OtherMeetingId equals q2.MeetingId
                                    where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.MeetingId != null && q2.UserId == userid
                                    select new { q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                    )
                            select q;

            var data = dataquery.ToList();

            var g = data.GroupBy(x => x.MeetingStartTime.Date);
            var chuqin = 0d;
            var tiaoxiu = 0d;
            var gongxiu = 0d;
            var bingjia = 0d;
            var shijia = 0d;
            var chuchai = 0d;
            var qita = 0d;

            foreach (var gitem in g)
            {

                switch (gitem.Count())
                {
                    case 0:
                        break;
                    case 1:
                        tiaoxiu += gitem.Sum(x => !x.IsSigned && x.Reason == "调休" ? 0.5 : 0);
                        gongxiu += gitem.Sum(x => !x.IsSigned && x.Reason == "公休" ? 0.5 : 0);
                        bingjia += gitem.Sum(x => !x.IsSigned && x.Reason == "病假" ? 0.5 : 0);
                        shijia += gitem.Sum(x => !x.IsSigned && x.Reason == "事假" ? 0.5 : 0);
                        chuchai += gitem.Sum(x => !x.IsSigned && x.Reason == "出差" ? 0.5 : 0);
                        qita += gitem.Sum(x => !x.IsSigned && x.Reason == "其他" ? 0.5 : 0);
                        chuqin += gitem.Sum(x => x.IsSigned ? 0.5 : 0);
                        break;
                    case 2:
                        goto case 1;
                    default:
                        var signins = gitem.OrderByDescending(x => x.CreateDate).ToList();
                        if (signins[0].IsSigned)
                            chuqin += 0.5;
                        else
                        {
                            switch (signins[0].Reason)
                            {
                                case "调休":
                                    tiaoxiu += 0.5;
                                    break;
                                case "公休":
                                    gongxiu += 0.5;
                                    break;
                                case "病假":
                                    bingjia += 0.5;
                                    break;
                                case "事假":
                                    shijia += 0.5;
                                    break;
                                case "出差":
                                    chuchai += 0.5;
                                    break;
                                case "其他":
                                    qita += 0.5;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (signins[1].IsSigned)
                            chuqin += 0.5;
                        else
                        {
                            switch (signins[1].Reason)
                            {
                                case "调休":
                                    tiaoxiu += 0.5;
                                    break;
                                case "公休":
                                    gongxiu += 0.5;
                                    break;
                                case "病假":
                                    bingjia += 0.5;
                                    break;
                                case "事假":
                                    shijia += 0.5;
                                    break;
                                case "出差":
                                    chuchai += 0.5;
                                    break;
                                case "其他":
                                    qita += 0.5;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
            }

            return new AttendanceEntity() { UserId = user.UserId, UserName = user.RealName, Chuqin = (decimal)chuqin, Tiaoxiu = (decimal)tiaoxiu, Gongxiu = (decimal)gongxiu, Bingjia = (decimal)bingjia, Shijia = (decimal)shijia, Chuchai = (decimal)chuchai, Qita = (decimal)qita };
        }

        public string GetAttendance(string id, DateTime date)
        {
            var from = date;
            var to = date.AddDays(1);
            var db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(id);

            var query = from q in
                            (
                                from q1 in db.IQueryable<WorkmeetingEntity>()
                                join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
                                where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.UserId == id
                                select new { q1.MeetingId, q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                )
                                .Concat(
                                from q1 in db.IQueryable<WorkmeetingEntity>()
                                join q2 in db.IQueryable<MeetingSigninEntity>() on q1.OtherMeetingId equals q2.MeetingId
                                where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.MeetingId != null && q2.UserId == id
                                select new { q1.MeetingId, q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                )
                        select q;

            var data = query.ToList();
            var chuqin = 0d;
            var tiaoxiu = 0d;
            var gongxiu = 0d;
            var bingjia = 0d;
            var shijia = 0d;
            var chuchai = 0d;
            var qita = 0d;

            switch (data.Count())
            {
                case 0:
                    break;
                case 1:
                    tiaoxiu += data.Sum(x => !x.IsSigned && x.Reason == "调休" ? 0.5 : 0);
                    gongxiu += data.Sum(x => !x.IsSigned && x.Reason == "公休" ? 0.5 : 0);
                    bingjia += data.Sum(x => !x.IsSigned && x.Reason == "病假" ? 0.5 : 0);
                    shijia += data.Sum(x => !x.IsSigned && x.Reason == "事假" ? 0.5 : 0);
                    chuchai += data.Sum(x => !x.IsSigned && x.Reason == "出差" ? 0.5 : 0);
                    qita += data.Sum(x => !x.IsSigned && x.Reason == "其他" ? 0.5 : 0);
                    chuqin += data.Sum(x => x.IsSigned ? 0.5 : 0);
                    break;
                default:
                    var g = data.GroupBy(x => new { x.MeetingId, x.MeetingStartTime }).OrderByDescending(x => x.Key.MeetingStartTime).First();

                    switch (g.Count(x => !x.IsSigned))
                    {
                        case 1:
                            switch (g.First(x => !x.IsSigned).Reason)
                            {
                                case "调休":
                                    tiaoxiu += 0.5;
                                    break;
                                case "公休":
                                    gongxiu += 0.5;
                                    break;
                                case "病假":
                                    bingjia += 0.5;
                                    break;
                                case "事假":
                                    shijia += 0.5;
                                    break;
                                case "出差":
                                    chuchai += 0.5;
                                    break;
                                case "其他":
                                    qita += 0.5;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2:
                            switch (g.First().Reason)
                            {
                                case "调休":
                                    tiaoxiu += 0.5;
                                    break;
                                case "公休":
                                    gongxiu += 0.5;
                                    break;
                                case "病假":
                                    bingjia += 0.5;
                                    break;
                                case "事假":
                                    shijia += 0.5;
                                    break;
                                case "出差":
                                    chuchai += 0.5;
                                    break;
                                case "其他":
                                    qita += 0.5;
                                    break;
                                default:
                                    break;
                            }
                            switch (g.Last().Reason)
                            {
                                case "调休":
                                    tiaoxiu += 0.5;
                                    break;
                                case "公休":
                                    gongxiu += 0.5;
                                    break;
                                case "病假":
                                    bingjia += 0.5;
                                    break;
                                case "事假":
                                    shijia += 0.5;
                                    break;
                                case "出差":
                                    chuchai += 0.5;
                                    break;
                                case "其他":
                                    qita += 0.5;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            chuqin += g.Sum(x => x.IsSigned ? 0.5 : 0);
                            break;
                    }
                    break;
            }

            if (tiaoxiu == 1)
                return "调休";
            else if (tiaoxiu == 0.5)
                return "调休 0.5";

            if (gongxiu == 1)
                return "公休";
            else if (gongxiu == 0.5)
                return "公休 0.5";

            if (bingjia == 1)
                return "病假";
            else if (bingjia == 0.5)
                return "病假 0.5";

            if (shijia == 1)
                return "事假";
            else if (shijia == 0.5)
                return "事假 0.5";

            if (chuchai == 1)
                return "出差";
            else if (chuchai == 0.5)
                return "出差 0.5";

            if (qita == 1)
                return "其他";
            else if (qita == 0.5)
                return "其他 0.5";

            if (chuqin > 0)
                return "出勤";

            return string.Empty;
        }

        public List<JobUserEntity> GetJobUsers(string jobid, string meetingjobid)
        {
            var db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(meetingjobid))
            {
                var query = from q in db.IQueryable<JobUserEntity>()
                            join q1 in db.IQueryable<MeetingAndJobEntity>() on q.MeetingJobId equals q1.MeetingJobId
                            where q1.JobId == jobid
                            select q;

                return query.ToList();
            }
            else
            {
                var query = from q in db.IQueryable<JobUserEntity>()
                            join q1 in db.IQueryable<MeetingAndJobEntity>() on q.MeetingJobId equals q1.MeetingJobId
                            where q1.MeetingJobId == meetingjobid
                            select q;

                return query.ToList();
            }
        }
        public List<JobUserEntity> GetDangerJobUsers(string jobid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<JobUserEntity>()
                        where q.MeetingJobId == jobid
                        select q;

            return query.ToList();
        }
        /// <summary>
        /// 更新评分
        /// </summary>
        /// <param name="models"></param>
        public void PostScore(List<JobUserEntity> models)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                for (int i = 0; i < models.Count; i++)
                {
                    var entity = db.FindEntity<JobUserEntity>(models[i].JobUserId);
                    entity.Score = models[i].Score;

                    db.Update(entity);

                    if (i == 0)
                    {
                        var job = (from q1 in db.IQueryable<MeetingJobEntity>()
                                   join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                   join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                                   where q3.JobUserId == entity.JobUserId
                                   select q1).FirstOrDefault();
                        if (job != null)
                        {
                            job.Score = "ok";
                            db.Update(job);
                        }
                    }
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public DayAttendanceEntity GetDayAttendance(string id, DateTime date)
        {
            var from = date;
            var to = date.AddDays(1);
            var db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(id);

            var query = from q in
                            (
                                from q1 in db.IQueryable<WorkmeetingEntity>()
                                join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
                                where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.UserId == id
                                select new { q1.MeetingId, q1.MeetingType, q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                )
                                .Concat(
                                from q1 in db.IQueryable<WorkmeetingEntity>()
                                join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId
                                join q3 in db.IQueryable<MeetingSigninEntity>() on q2.MeetingId equals q3.MeetingId
                                where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.MeetingId != null && q3.UserId == id
                                select new { q1.MeetingId, q2.MeetingType, q1.MeetingStartTime, q3.UserId, q3.SigninId, q3.IsSigned, q3.Reason, q3.CreateDate }
                                )
                        select q;

            var data = query.OrderByDescending(x => x.CreateDate).ToList();

            var state = "出勤";
            var day = "全天";

            if (data.Count == 0)
            {
                state = string.Empty;
                day = string.Empty;
            }
            else
            {
                var g = data.GroupBy(x => new { x.MeetingId, x.MeetingStartTime }).OrderByDescending(x => x.Key.MeetingStartTime).First();

                switch (g.Count(x => !x.IsSigned))
                {
                    case 0:
                        break;
                    case 1:
                        day = g.First(x => !x.IsSigned).MeetingType == "班前会" ? "上午" : "下午";
                        state = g.First(x => !x.IsSigned).Reason;
                        break;
                    case 2:
                        var start = g.OrderBy(x => x.CreateDate).First();
                        var end = g.OrderBy(x => x.CreateDate).Last();

                        state = start.Reason;
                        day = start.Reason == end.Reason ? "全天" : "上午";
                        break;
                    default:
                        break;
                }
            }

            return new DayAttendanceEntity() { DayType = day, State = state };
        }

        public void PostUserState(string id, DateTime date, DayAttendanceEntity model)
        {
            var from = date;
            var to = date.AddDays(1);
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                var user = db.FindEntity<UserEntity>(id);

                var query = from q in
                                (
                                    from q1 in db.IQueryable<WorkmeetingEntity>()
                                    join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
                                    where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.UserId == id
                                    select new { q1.MeetingId, q1.MeetingType, q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                    )
                                    .Concat(
                                    from q1 in db.IQueryable<WorkmeetingEntity>()
                                    join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId
                                    join q3 in db.IQueryable<MeetingSigninEntity>() on q2.MeetingId equals q3.MeetingId
                                    where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.MeetingId != null && q3.UserId == id
                                    select new { q2.MeetingId, q2.MeetingType, q2.MeetingStartTime, q3.UserId, q3.SigninId, q3.IsSigned, q3.Reason, q3.CreateDate }
                                    )
                            select q;

                var list = query.ToList();

                if (model.DayType == "全天")
                {
                    var signids = list.Select(x => x.SigninId);
                    var entities = db.IQueryable<MeetingSigninEntity>().Where(x => signids.Contains(x.SigninId)).ToList();
                    foreach (var item in entities)
                    {
                        //item.MentalCondition = model.State == "出勤" ? "正常" : "异常";
                        item.IsSigned = model.State == "出勤" ? true : false;
                        item.Reason = model.State == "出勤" ? null : model.State;
                        db.Update(item);
                    }
                }
                else
                {
                    var meetingtype = model.DayType == "上午" ? "班前会" : "班后会";
                    var signids = list.Where(x => x.MeetingType == meetingtype).Select(x => x.SigninId);
                    var entities = db.IQueryable<MeetingSigninEntity>().Where(x => signids.Contains(x.SigninId)).ToList();
                    foreach (var item in entities)
                    {
                        //item.MentalCondition = model.State == "出勤" ? "正常" : "异常";
                        item.IsSigned = model.State == "出勤" ? true : false;
                        item.Reason = model.State == "出勤" ? null : model.State;
                        db.Update(item);
                    }
                    var signids2 = list.Where(x => x.MeetingType != meetingtype).Select(x => x.SigninId);
                    var entities2 = db.IQueryable<MeetingSigninEntity>().Where(x => signids2.Contains(x.SigninId)).ToList();
                    foreach (var item in entities2)
                    {
                        //item.MentalCondition = model.State == "出勤" ? "正常" : "异常";
                        item.IsSigned = true;
                        item.Reason = null;
                        db.Update(item);
                    }
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        /// <summary>
        /// 评分统计
        /// </summary>
        /// <param name="userid">成员id</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="bzid">班组id</param>
        /// <param name="isfinish">完成情况</param>
        /// <returns></returns>
        public IEnumerable<MeetingJobEntity> GetJobs(string userid, string year, string month, string bzid, string isfinish)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<MeetingJobEntity>()
                        join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.JobId
                        select new { q, q1 };

            //var query = new Repository<MeetingJobEntity>(DbFactory.Base()).IQueryable();
            if (!string.IsNullOrEmpty(userid)) query = query.Where(x => x.q1.JobUserId.Contains(userid));
            if (!string.IsNullOrEmpty(month)) { int m = int.Parse(month); query = query.Where(x => x.q.CreateDate.Month == m); }
            if (!string.IsNullOrEmpty(year)) { int y = int.Parse(year); query = query.Where(x => x.q.CreateDate.Year == y); }
            if (!string.IsNullOrEmpty(bzid)) query = query.Where(x => x.q.GroupId == bzid);
            if (!string.IsNullOrEmpty(isfinish)) query = query.Where(x => x.q.IsFinished == isfinish);
            return query.OrderByDescending(x => x.q.StartTime).Select(x => x.q).ToList();
        }

        public List<SinginUserEntity> GetSigninData(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in
                            (
                                from q1 in db.IQueryable<WorkmeetingEntity>()
                                join q2 in db.IQueryable<MeetingSigninEntity>() on q1.MeetingId equals q2.MeetingId
                                where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to
                                select new { q1.MeetingId, q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                )
                                .Concat(
                                from q1 in db.IQueryable<WorkmeetingEntity>()
                                join q2 in db.IQueryable<MeetingSigninEntity>() on q1.OtherMeetingId equals q2.MeetingId
                                where q1.MeetingType == "班前会" && q1.IsOver && q1.MeetingStartTime > @from && q1.MeetingStartTime < to && q2.MeetingId != null
                                select new { q1.MeetingId, q1.MeetingStartTime, q2.UserId, q2.SigninId, q2.IsSigned, q2.Reason, q2.CreateDate }
                                ) on q1.UserId equals q2.UserId into into1
                        where q1.DepartmentId == deptid
                        select new { q1.UserId, q1.RealName, Signins = into1.ToList() };

            var data = query.ToList().Select(x => new SinginUserEntity() { UserId = x.UserId, UserName = x.RealName, Signins = x.Signins.Select(y => new SigninEntity() { MeetingId = y.MeetingId, MeetingStartTime = y.MeetingStartTime, IsSignin = y.IsSigned, Reason = y.Reason, CreateDate = y.CreateDate }).ToList() }).ToList();
            return data;
        }

        public List<MeetingJobEntity> GetMyJobs(string userid, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository();

            var userquery = from q in db.IQueryable<UserEntity>()
                            where q.UserId == userid
                            select q;
            var user = userquery.FirstOrDefault();
            if (user == null) return null;

            var meetingquery = from q in db.IQueryable<WorkmeetingEntity>()
                               where q.GroupId == user.DepartmentId
                               orderby q.MeetingStartTime descending
                               select q;
            var meeting = meetingquery.FirstOrDefault();
            if (meeting == null) return null;

            var query1 = from q in db.IQueryable<MeetingAndJobEntity>()
                         where q.IsFinished != "cancel"
                         select q;

            if (meeting.MeetingType == "班前会" && meeting.IsOver)
            {
                query1 = from q in query1
                         where q.StartMeetingId == meeting.MeetingId
                         select q;
            }
            else if (meeting.MeetingType == "班后会")
            {
                if (meeting.IsOver)
                {
                    query1 = from q in query1
                             where q.EndMeetingId == meeting.MeetingId
                             select q;
                }
                else
                {
                    if (meeting.MeetingStartTime.Date < date.Date)
                        return null;
                    else
                    {
                        query1 = from q in query1
                                 where q.EndMeetingId == meeting.MeetingId && q.IsFinished == "undo"
                                 select q;
                    }
                }
            }
            else
                return null;

            var query2 = from q1 in query1
                         join q3 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q3.JobId
                         join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                         join q4 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q4.MeetingJobId into into1
                         join q5 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q5.JobId into t2
                         from q5 in t2.DefaultIfEmpty()
                         where q2.UserId == userid
                         select new { JobId = q1.JobId, JobUser = q1.JobUser, q1.JobUserId, UserId = q2.JobType == "ischecker" ? q2.UserId : null, q3.StartTime, q3.EndTime, q3.IsFinished, q3.Job, q1.MeetingJobId, q3.NeedTrain, into1, q5 };

            var data = query2.ToList();

            return data.Select(x => new MeetingJobEntity() { JobId = x.JobId, StartTime = x.StartTime, EndTime = x.EndTime, Job = x.Job, IsFinished = x.IsFinished, CreateUserId = x.UserId, NeedTrain = x.NeedTrain, TrainingDone = x.q5 == null ? false : x.q5.Status == 2, Relation = new MeetingAndJobEntity() { JobUser = x.JobUser, JobUserId = x.JobUserId, MeetingJobId = x.MeetingJobId, JobUsers = x.into1.ToList() } }).ToList();
        }

        //public List<MeetingJobEntity> GetMyJobs(string uid, DateTime dateTime)
        //{
        //    var from = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        //    var to = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        //    var db = new RepositoryFactory().BaseRepository();

        //    var user = (from q in db.IQueryable<UserEntity>()
        //                where q.UserId == uid
        //                select q).FirstOrDefault();
        //    if (user == null) return null;


        //    var query = from q in db.IQueryable<WorkmeetingEntity>()
        //                where q.GroupId == user.DepartmentId && q.MeetingStartTime > @from && q.MeetingStartTime < to
        //                select q;

        //    var meetings = query.ToList();
        //    if (meetings.Count == 0) return null;
        //    else
        //    {
        //        var startmeeting = meetings.Where(x => x.MeetingType == "班前会").OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
        //        var endmeeting = meetings.Where(x => x.MeetingType == "班后会").OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
        //        var meeting = default(WorkmeetingEntity);

        //        if (startmeeting == null) return null;
        //        else
        //        {
        //            if (endmeeting == null)
        //            {
        //                if (startmeeting.IsOver) meeting = startmeeting;
        //            }
        //            else
        //            {
        //                if (endmeeting.OtherMeetingId == startmeeting.MeetingId)
        //                {
        //                    if (!endmeeting.IsOver) meeting = endmeeting;
        //                }
        //                else
        //                {
        //                    if (startmeeting.IsOver) meeting = startmeeting;
        //                }
        //            }
        //        }
        //        if (meeting == null) return null;

        //        var query1 = from q in db.IQueryable<MeetingJobEntity>()
        //                     where q.IsFinished != "cancel"
        //                     select q;

        //        if (meeting.MeetingType == "班前会")
        //            query1 = from q in query1
        //                     where q.StartMeetingId == meeting.MeetingId
        //                     select q;
        //        else
        //            query1 = from q in query1
        //                     where q.EndMeetingId == meeting.MeetingId
        //                     select q;

        //        var query2 = from q1 in query1
        //                     join q2 in db.IQueryable<JobUserEntity>() on q1.JobId equals q2.JobId
        //                     where q2.UserId == uid
        //                     select new { JobId = q1.JobId, JobUsers = q1.JobUsers, UserId = q2.JobType == "ischecker" ? q2.UserId : null, q1.StartTime, q1.EndTime, q1.IsFinished, q1.Job };

        //        //var query1 = from q1 in db.IQueryable<MeetingJobEntity>()
        //        //             join q2 in db.IQueryable<JobUserEntity>() on q1.JobId equals q2.JobId
        //        //             where meeting.MeetingType == "班前会" ? q1.StartMeetingId == meeting.MeetingId : q1.EndMeetingId == meeting.MeetingId && q1.IsFinished != "cancel" && q2.UserId == uid
        //        //             select new { JobId = q1.JobId, JobUsers = q1.JobUsers, UserId = q2.JobType == "ischecker" ? null : q2.UserId, q1.StartTime, q1.EndTime, q1.IsFinished };

        //        return query2.ToList().Select(x => new MeetingJobEntity() { JobId = x.JobId, JobUsers = x.JobUsers, UserId = x.UserId, StartTime = x.EndTime, EndTime = x.EndTime, Job = x.Job, IsFinished = x.IsFinished }).ToList();
        //    }
        //}

        public void UpdateJob2(MeetingJobEntity model, string trainingtype, string trainingtype2)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            var dept = db.FindEntity<DepartmentEntity>(model.GroupId);

            try
            {
                var meeting = default(WorkmeetingEntity);
                if (!string.IsNullOrEmpty(model.Relation.StartMeetingId))
                    meeting = db.FindEntity<WorkmeetingEntity>(model.Relation.StartMeetingId);

                var entity = db.FindEntity<MeetingJobEntity>(model.JobId);

                entity.Job = model.Job;
                entity.StartTime = model.StartTime;
                entity.EndTime = model.EndTime;
                entity.Dangerous = model.Dangerous;
                entity.Measure = model.Measure;
                entity.IsFinished = model.IsFinished;
                entity.Remark = model.Remark;
                entity.Description = model.Description;
                entity.NeedTrain = model.NeedTrain;
                entity.Relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                   where q.MeetingJobId == model.Relation.MeetingJobId
                                   select q).FirstOrDefault();
                entity.Relation.JobUser = string.Join(",", model.Relation.JobUsers.Select(x => x.UserName));
                entity.Relation.JobUserId = string.Join(",", model.Relation.JobUsers.Select(x => x.UserId));
                entity.Relation.IsFinished = model.IsFinished;
                if (model.StartTime.Date > DateTime.Today)
                    entity.Relation.StartMeetingId = string.Empty;

                entity.TicketId = model.TicketId ?? string.Empty;
                entity.TicketCode = model.TicketCode;
                entity.TaskType = model.TaskType;
                entity.RiskLevel = model.RiskLevel;

                var danger = (from q in db.IQueryable<DangerEntity>()
                              where q.JobId == model.Relation.MeetingJobId
                              select q).FirstOrDefault();

                var ctx = new DataContext();
                var training = default(HumanDangerTraining);
                if (!string.IsNullOrEmpty(model.Relation.MeetingJobId))
                {
                    training = ctx.HumanDangerTrainings.Include("TrainingUsers").FirstOrDefault(x => x.MeetingJobId == model.Relation.MeetingJobId);
                }


                if (meeting != null && meeting.IsOver)
                {
                    //班前会结束了才能变更危险预知训练数据和人身风险预控数据
                    if (model.IsFinished == "cancel")
                    {
                        if (trainingtype == "人身风险预控" && training != null)
                        {
                            if (training.TrainingUsers.Count == training.TrainingUsers.Count(x => !(x.IsDone == true && x.IsMarked == true)))
                            {
                                ctx.HumanDangerTrainings.Remove(training);
                                foreach (var item in training.TrainingUsers)
                                {
                                    var todos = ctx.Messages.Where(x => x.MessageKey == "人身风险预控" && x.BusinessId == item.TrainingUserId.ToString()).ToList();
                                    todos.ForEach(x => x.IsFinished = true);
                                    ctx.Messages.Add(new Message()
                                    {
                                        MessageId = Guid.NewGuid(),
                                        BusinessId = item.TrainingUserId.ToString(),
                                        Content = string.Format("您有条人身风险预控任务已取消：{0}", training.TrainingTask),
                                        Title = "人身风险预控取消",
                                        UserId = item.UserId,
                                        Category = MessageCategory.Message,
                                        MessageKey = "人身风险预控取消",
                                        CreateTime = DateTime.Now
                                    });
                                }
                            }
                            else
                            {
                                var deleteitems = training.TrainingUsers.Where((x => !(x.IsDone == true && x.IsMarked == true))).ToList();
                                foreach (var item in deleteitems)
                                {
                                    ctx.HumanDangerTrainingUsers.Remove(item);
                                    var todos = ctx.Messages.Where(x => x.MessageKey == "人身风险预控" && x.BusinessId == item.TrainingUserId.ToString()).ToList();
                                    todos.ForEach(x => x.IsFinished = true);
                                    ctx.Messages.Add(new Message()
                                    {
                                        MessageId = Guid.NewGuid(),
                                        BusinessId = item.TrainingUserId.ToString(),
                                        Content = string.Format("您有条人身风险预控任务已取消：{0}", training.TrainingTask),
                                        Title = "人身风险预控取消",
                                        UserId = item.UserId,
                                        Category = MessageCategory.Message,
                                        MessageKey = "人身风险预控取消",
                                        CreateTime = DateTime.Now
                                    });
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (model.NeedTrain)
                        {
                            if (trainingtype == "人身风险预控")
                            {
                                if (training == null)
                                {
                                    training = new HumanDangerTraining() { TrainingId = Guid.NewGuid().ToString(), CreateTime = DateTime.Now, CreateUserId = model.CreateUserId, DeptId = model.GroupId, MeetingJobId = model.Relation.MeetingJobId, DeptName = dept.FullName, TrainingTask = model.Job, TrainingUsers = model.Relation.JobUsers.Select(x => new HumanDangerTrainingUser { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, No = entity.TicketCode, TrainingRole = x.JobType == "ischecker" ? 1 : 0, TrainingPlace = model.JobAddr }).ToList() };
                                    if (!string.IsNullOrEmpty(entity.TemplateId))
                                        training.HumanDangerId = entity.TemplateId;

                                    ctx.HumanDangerTrainings.Add(training);
                                    using (var factory = new ChannelFactory<IMsgService>("message"))
                                    {
                                        foreach (var item in training.TrainingUsers)
                                        {
                                            var proxy = factory.CreateChannel();
                                            proxy.Send("人身风险预控", item.TrainingUserId.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    var trainingusers = ctx.HumanDangerTrainingUsers.Where(x => x.TrainingId == training.TrainingId).ToList();
                                    var deleteitems = trainingusers.Where(x => !model.Relation.JobUsers.Any(y => y.UserId == x.UserId) && x.IsDone == false && x.IsMarked == false).ToList();
                                    ctx.HumanDangerTrainingUsers.RemoveRange(deleteitems);

                                    var newitems = model.Relation.JobUsers.Where(x => !training.TrainingUsers.Any(y => y.UserId == x.UserId)).Select(x => new HumanDangerTrainingUser { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, TrainingRole = 0, TrainingPlace = model.JobAddr, TrainingId = training.TrainingId, No = entity.TicketCode, TicketId = entity.TicketId });
                                    ctx.HumanDangerTrainingUsers.AddRange(newitems);
                                    using (var factory = new ChannelFactory<IMsgService>("message"))
                                    {
                                        foreach (var item in newitems)
                                        {
                                            var proxy = factory.CreateChannel();
                                            proxy.Send("人身风险预控", item.TrainingUserId.ToString());
                                        }
                                    }

                                    foreach (var item in trainingusers)
                                    {
                                        if (item.TrainingRole == 1) item.TrainingRole = 0;

                                        if (string.IsNullOrEmpty(item.No))
                                        {
                                            item.No = entity.TicketCode;
                                            item.TicketId = entity.TicketId;
                                        }

                                        if (model.IsFinished == "cancel")
                                        {
                                            var todos = ctx.Messages.Where(x => x.MessageKey == "人身风险预控" && x.BusinessId == item.TrainingUserId.ToString()).ToList();
                                            todos.ForEach(x => x.IsFinished = true);
                                            ctx.Messages.Add(new Message()
                                            {
                                                MessageId = Guid.NewGuid(),
                                                BusinessId = item.TrainingUserId.ToString(),
                                                Content = string.Format("您有条人身风险预控任务已取消：{0}", training.TrainingTask),
                                                Title = "人身风险预控取消",
                                                UserId = item.UserId,
                                                Category = MessageCategory.Message,
                                                MessageKey = "人身风险预控取消",
                                                CreateTime = DateTime.Now
                                            });
                                        }
                                    }
                                    var checker = model.Relation.JobUsers.Find(x => x.JobType == "ischecker");
                                    var traininguser = trainingusers.Find(x => x.UserId == checker.UserId);
                                    if (traininguser != null) traininguser.TrainingRole = 1;

                                    //humandanger.TrainingUsers.RemoveAll(x => !model.Relation.JobUsers.Any(y => y.UserId == x.UserId));
                                    //humandanger.TrainingUsers.AddRange(model.Relation.JobUsers.Where(x => !humandanger.TrainingUsers.Any(y => y.UserId == x.UserId)).Select(x => new HumanDangerTrainingUser { TrainingUserId = Guid.NewGuid(), UserId = x.UserId, UserName = x.UserName, TrainingRole = 0, TrainingPlace = model.JobAddr }).ToList());
                                    //humandanger.TrainingUsers.ForEach(x => x.TrainingRole = 0);
                                    //humandanger.TrainingUsers.Find(x => x.UserId == model.Relation.JobUsers.Find(y => y.JobType == "ischecker").UserId).TrainingRole = 1;
                                }
                            }
                            else
                            {
                                if (danger == null)
                                {
                                    var user1 = model.Relation.JobUsers.FirstOrDefault(x => x.JobType == "ischecker");
                                    var user2 = model.Relation.JobUsers.Where(x => x.JobType == "isdoperson");
                                    danger = new DangerEntity()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Sno = DateTime.Now.ToString("yyyyMMddHHmmss"),
                                        JobId = entity.Relation.MeetingJobId,
                                        JobName = entity.Job,
                                        JobUser = user1 == null ? null : user1.UserName,
                                        JobAddress = entity.JobAddr,
                                        JobTime = DateTime.Now,
                                        Persons = string.Join(",", user2.Select(x => x.UserName)),
                                        GroupId = entity.GroupId,
                                        GroupName = dept == null ? null : dept.FullName,
                                        CreateDate = DateTime.Now,
                                        CreateUserId = model.CreateUserId,
                                        DeptCode = dept == null ? null : dept.EnCode,
                                        OperDate = DateTime.Now,
                                        Status = 0,
                                        TicketId = entity.TicketCode
                                    };
                                    db.Insert(danger);
                                }
                                else
                                {
                                    if (model.IsFinished == "cancel")
                                    {
                                        var jobusers = (from q in db.IQueryable<JobUserEntity>()
                                                        where q.MeetingJobId == entity.Relation.MeetingJobId
                                                        select q).ToList();

                                        var todos = ctx.Messages.Where(x => x.MessageKey == trainingtype2 && x.BusinessId == danger.Id).ToList();
                                        todos.ForEach(x => x.IsFinished = true);

                                        foreach (var item in jobusers)
                                        {
                                            ctx.Messages.Add(new Message()
                                            {
                                                MessageId = Guid.NewGuid(),
                                                BusinessId = danger.Id,
                                                Content = string.Format("您有项{1}任务已取消：{0}", danger.JobName, trainingtype2),
                                                Title = trainingtype2 + "取消",
                                                UserId = item.UserId,
                                                Category = MessageCategory.Message,
                                                MessageKey = trainingtype2 + "取消",
                                                CreateTime = DateTime.Now
                                            });
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (trainingtype == "人身风险预控")
                            {
                                if (training != null)
                                {
                                    if (training.TrainingUsers.Count(x => x.IsMarked == true && x.IsMarked == true) > 0)
                                    {
                                        var trainingusers = ctx.HumanDangerTrainingUsers.Where(x => x.TrainingId == training.TrainingId).ToList();
                                        trainingusers.ForEach(x => x.Status = 1);
                                        var deleteitems = trainingusers.Where(x => !(x.IsMarked == true && x.IsMarked == true)).ToList();
                                        ctx.HumanDangerTrainingUsers.RemoveRange(deleteitems);

                                        foreach (var item in deleteitems)
                                        {
                                            var bid = item.TrainingUserId.ToString();
                                            var todos = ctx.Messages.Where(x => x.MessageKey == "人身风险预控" && x.BusinessId == bid).ToList();
                                            todos.ForEach(x => x.IsFinished = true);

                                            ctx.Messages.Add(new Message()
                                            {
                                                MessageId = Guid.NewGuid(),
                                                BusinessId = bid,
                                                Content = string.Format("您有条人身风险预控任务已取消：{0}", training.TrainingTask),
                                                Title = "人身风险预控取消",
                                                UserId = item.UserId,
                                                Category = MessageCategory.Message,
                                                MessageKey = "人身风险预控取消",
                                                CreateTime = DateTime.Now
                                            });
                                        }
                                    }
                                    else
                                    {
                                        var trainingusers = ctx.HumanDangerTrainingUsers.Where(x => x.TrainingId == training.TrainingId).ToList();
                                        foreach (var item in trainingusers)
                                        {
                                            var bid = item.TrainingUserId.ToString();
                                            var todos = ctx.Messages.Where(x => x.MessageKey == "人身风险预控" && x.BusinessId == bid).ToList();
                                            todos.ForEach(x => x.IsFinished = true);

                                            ctx.Messages.Add(new Message()
                                            {
                                                MessageId = Guid.NewGuid(),
                                                BusinessId = bid,
                                                Content = string.Format("您有条人身风险预控任务已取消：{0}", training.TrainingTask),
                                                Title = "人身风险预控取消",
                                                UserId = item.UserId,
                                                Category = MessageCategory.Message,
                                                MessageKey = "人身风险预控取消",
                                                CreateTime = DateTime.Now
                                            });
                                        }
                                        ctx.HumanDangerTrainings.Remove(training);
                                    }
                                }
                            }
                            else
                            {
                                if (danger != null)
                                {
                                    db.Delete(danger);

                                    var measures = (from q in db.IQueryable<MeasuresEntity>()
                                                    where q.DangerId == danger.Id
                                                    select q).ToList();
                                    db.Delete(measures);

                                    var jobusers = (from q in db.IQueryable<JobUserEntity>()
                                                    where q.MeetingJobId == entity.Relation.MeetingJobId
                                                    select q).ToList();

                                    var todos = ctx.Messages.Where(x => x.MessageKey == trainingtype2 && x.BusinessId == danger.Id).ToList();

                                    foreach (var item in jobusers)
                                    {
                                        ctx.Messages.Add(new Message()
                                        {
                                            MessageId = Guid.NewGuid(),
                                            BusinessId = danger.Id,
                                            Content = string.Format("您有项{1}任务已取消：{0}", danger.JobName, trainingtype2),
                                            Title = trainingtype2 + "取消",
                                            UserId = item.UserId,
                                            Category = MessageCategory.Message,
                                            MessageKey = trainingtype2 + "取消",
                                            CreateTime = DateTime.Now
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                ctx.SaveChanges();
                var persons = (from q1 in db.IQueryable<JobUserEntity>()
                               join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                               where q2.MeetingJobId == model.Relation.MeetingJobId
                               select q1).ToList();
                foreach (var item in model.Relation.JobUsers)
                {
                    var person = persons.Find(x => x.UserId == item.UserId);
                    if (person != null)
                    {
                        person.JobType = item.JobType;
                        person.TaskHour = item.TaskHour;
                        db.Update(person);
                    }
                }

                var delpersons = persons.Where(x => !model.Relation.JobUsers.Select(y => y.UserId).Contains(x.UserId)).ToList();
                db.Delete<JobUserEntity>(delpersons);

                var newpersons = model.Relation.JobUsers.Where(x => !persons.Select(y => y.UserId).Contains(x.UserId)).ToList();
                db.Insert(newpersons);

                var dangerous = (from q1 in db.IQueryable<JobDangerousEntity>()
                                 join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into t2
                                 where q1.JobId == model.JobId
                                 select new { q1, q2 = t2 }).ToList();
                foreach (var item in model.DangerousList)
                {
                    var dan = dangerous.Find(x => x.q1.JobDangerousId == item.JobDangerousId);
                    if (dan == null)
                    {
                        db.Insert(item);
                        db.Insert(item.MeasureList);
                    }
                    else
                    {
                        dan.q1.Content = item.Content;
                        dan.q1.DangerousId = item.DangerousId;
                        db.Update(dan.q1);

                        foreach (var item1 in item.MeasureList)
                        {
                            var mea = dan.q2.FirstOrDefault(x => x.JobMeasureId == item1.JobMeasureId);
                            if (mea == null) db.Insert(item1);
                            else
                            {
                                mea.Content = item1.Content;
                                mea.MeasureId = item1.MeasureId;
                                db.Update(mea);
                            }
                        }

                        var deletemeasures = dan.q2.Where(x => !item.MeasureList.Any(y => y.JobMeasureId == x.JobMeasureId)).ToList();
                        db.Delete(deletemeasures);
                    }
                }
                var deletedangerous = dangerous.Where(x => !model.DangerousList.Any(y => y.JobDangerousId == x.q1.JobDangerousId)).ToList();
                db.Delete(deletedangerous.Select(x => x.q1).ToList());
                db.Delete(deletedangerous.SelectMany(x => x.q2).ToList());

                var relation = entity.Relation;
                entity.Relation = null;
                db.Update(entity);
                db.Update(relation);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void UpdateJob3(string userKey, MeetingJobEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<MeetingJobEntity>(model.JobId);
                entity.Job = model.Job;
                entity.StartTime = model.StartTime;
                entity.IsFinished = model.IsFinished;
                entity.Score = model.Score;
                entity.Files = null;
                db.Update(entity);

                //IList<JobUserEntity> list = model.Persons.Where(x => x.UserId == userKey).ToList();
                //if (list.Count > 0)
                //{
                //    var jobUserEntity = db.FindEntity<JobUserEntity>(list[0].JobUserId);
                //    jobUserEntity.Score = Convert.ToInt32(model.Score);
                //    db.Update(jobUserEntity);
                //}

                //var query = from q in db.IQueryable<JobUserEntity>()
                //            where q.JobId == entity.JobId && q.UserId==entity.UserId
                //            select q;
                //List<JobUserEntity> list= query.ToList();
                //if (list.Count > 0)
                //{
                //    var jobUserEntity = db.FindEntity<JobUserEntity>();
                //}
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }


        public void DeleteJob(MeetingAndJobEntity arg)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                where q.MeetingJobId == arg.MeetingJobId
                                select q).FirstOrDefault();

                var job = (from q in db.IQueryable<MeetingJobEntity>()
                           where q.JobId == relation.JobId
                           select q).FirstOrDefault();

                var otherrelations = (from q in db.IQueryable<MeetingAndJobEntity>()
                                      where q.StartMeetingId == relation.StartMeetingId && q.MeetingJobId != relation.MeetingJobId
                                      select q).Count();

                var jobusers = (from q in db.IQueryable<JobUserEntity>()
                                where q.MeetingJobId == relation.MeetingJobId
                                select q).ToList();

                var files = (from q in db.IQueryable<FileInfoEntity>()
                             where q.RecId == relation.MeetingJobId
                             select q).ToList();

                db.Delete(relation);
                if (otherrelations == 0) db.Delete(job);
                db.Delete(jobusers);
                db.Delete(files);

                //var job = db.FindEntity<MeetingJobEntity>(jobid);
                //var persons = (from q1 in db.IQueryable<JobUserEntity>()
                //               join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                //               where q2.JobId == jobid
                //               select q1).ToList();
                //var files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == jobid).ToList();
                //db.Delete(persons);
                //db.Delete(files);
                //db.Delete(job);

                db.Commit();

                if (job.JobType == "旁站监督")
                    new SkSystem().DelTask(job.RecId);
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public List<MeetingSigninEntity> Signin(List<MeetingSigninEntity> signins)
        {
            var data = new List<MeetingSigninEntity>();

            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var meetingid = signins.First().MeetingId;
                var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                               where q.MeetingId == meetingid
                               select q).FirstOrDefault();
                meeting.ActuallyJoin = signins.Count(x => x.IsSigned);
                db.Update(meeting);
                UserWorkStateService bll = new UserWorkStateService();//个人状态service
                foreach (var item in signins)
                {
                    var signin = db.FindEntity<MeetingSigninEntity>(item.SigninId);
                    if (signin == null)
                    {
                        signin = new MeetingSigninEntity()
                        {
                            SigninId = Guid.NewGuid().ToString(),
                            UserId = item.UserId,
                            PersonName = item.PersonName,
                            MeetingId = item.MeetingId,
                            IsSigned = item.IsSigned,
                            Reason = item.Reason,
                            ReasonRemark = item.ReasonRemark,
                            CreateDate = DateTime.Now,
                            MentalCondition = "正常",
                            ClosingCondition = "正常"
                        };
                        db.Insert(signin);
                    }
                    else
                    {
                        signin.IsSigned = item.IsSigned;
                        signin.Reason = item.Reason;
                        signin.ReasonRemark = item.ReasonRemark;

                        db.Update(signin);
                    }
                    #region  同步个人状态
                    var getState = bll.selectState(item.UserId);
                    if (getState == null)
                    {
                        getState = new UserWorkStateEntity();
                        getState.userId = item.UserId;
                    }
                    getState.State = string.IsNullOrEmpty(item.Reason) ? "" : item.Reason;
                    bll.addState(getState);
                    #endregion
                    data.Add(signin);
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

            return data;
        }

        public List<MeetingSigninEntity> SyncState(List<MeetingSigninEntity> signins)
        {
            var data = new List<MeetingSigninEntity>();

            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                foreach (var item in signins)
                {
                    var signin = db.FindEntity<MeetingSigninEntity>(item.SigninId);
                    if (signin == null)
                    {
                        signin = new MeetingSigninEntity()
                        {
                            SigninId = Guid.NewGuid().ToString(),
                            UserId = item.UserId,
                            PersonName = item.PersonName,
                            MeetingId = item.MeetingId,
                            MentalCondition = item.MentalCondition,
                            ClosingCondition = item.ClosingCondition,
                            CreateDate = DateTime.Now,
                            IsSigned = true,
                        };
                    }
                    else
                    {
                        signin.MentalCondition = item.MentalCondition;
                        signin.ClosingCondition = item.ClosingCondition;

                        item.MeetingId = signin.MeetingId;
                        db.Update(signin);
                    }

                    data.Add(signin);
                }

                if (signins.Count > 0)
                {
                    var meeting = db.FindEntity<WorkmeetingEntity>(signins.First().MeetingId);
                    if (signins.Count(x => x.ClosingCondition != "正常" || x.MentalCondition != "正常") > 0)
                        meeting.PersonState = "异常";
                    else
                        meeting.PersonState = "正常";

                    meeting.Signins = null;
                    meeting.Jobs = null;
                    meeting.Files = null;

                    db.Update(meeting);
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

            return data;
        }

        public void Score(MeetingJobEntity job)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<MeetingJobEntity>(job.JobId);
                entity.Score = "ok";
                db.Update(entity);

                var jobuserquery = from q in db.IQueryable<JobUserEntity>()
                                   join q1 in db.IQueryable<MeetingAndJobEntity>() on q.MeetingJobId equals q1.MeetingJobId
                                   where q1.JobId == job.JobId
                                   select q;

                var jobusers = jobuserquery.ToList();
                foreach (var item in job.Relation.JobUsers)
                {
                    var jobuser = jobusers.Find(x => x.JobUserId == item.JobUserId);
                    if (jobuser != null)
                    {
                        jobuser.Score = item.Score;
                        db.Update(jobuser);
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

        public void PostVideo(List<FileInfoEntity> files)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(files);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void EditStartMeeting(WorkmeetingEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var meetingquery = from q in db.IQueryable<WorkmeetingEntity>()
                                   where q.MeetingId == model.MeetingId
                                   select q;

                var entity = meetingquery.FirstOrDefault();
                if (entity == null) return;

                //更新主表
                entity.ActuallyJoin = model.ActuallyJoin;
                entity.Remark = model.Remark;
                entity.IsOver = model.IsOver;
                entity.MeetingPerson = model.MeetingPerson;
                entity.PersonState = model.PersonState;

                //结束班前会
                if (model.IsOver)
                    entity.MeetingEndTime = model.MeetingEndTime;

                db.Update(entity);

                var jobs = (from q1 in db.IQueryable<MeetingAndJobEntity>()
                            join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                            join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into t1
                            where q1.StartMeetingId == model.MeetingId
                            select new { q1, q2, jobusers = t1 }).ToList();
                foreach (var item in model.Jobs)
                {
                    var job = jobs.Find(x => x.q2.JobId == item.JobId);
                    if (job == null)
                    {
                        db.Insert(item);
                        db.Insert(item.Relation);
                        db.Insert(item.Relation.JobUsers);
                        db.Insert(item.DangerousList);
                        db.Insert(item.DangerousList.SelectMany(x => x.MeasureList).ToList());
                    }
                    else
                    {
                        job.q2.StartTime = item.StartTime;
                        job.q2.EndTime = item.EndTime;
                        job.q2.Job = item.Job;
                        job.q2.Dangerous = item.Dangerous;
                        job.q2.Measure = item.Measure;
                        job.q2.NeedTrain = item.NeedTrain;
                        job.q2.TaskType = item.TaskType;
                        job.q2.RiskLevel = item.RiskLevel;
                        db.Update(job.q2);

                        job.q1.JobUserId = item.Relation.JobUserId;
                        job.q1.JobUser = item.Relation.JobUser;
                        db.Update(job.q1);

                        foreach (var item1 in item.Relation.JobUsers)
                        {
                            var jobuser = job.jobusers.FirstOrDefault(x => x.UserId == item1.UserId);
                            if (jobuser == null) db.Insert(item1);
                            else
                            {
                                jobuser.UserName = item1.UserName;
                                jobuser.JobType = item1.JobType;
                                db.Update(jobuser);
                            }
                        }
                        var deleteusers = job.jobusers.Where(x => !item.Relation.JobUsers.Any(y => y.UserId == x.UserId)).ToList();
                        db.Delete(deleteusers);

                        var dangerous = (from q1 in db.IQueryable<JobDangerousEntity>()
                                         join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into t1
                                         where q1.JobId == job.q1.JobId
                                         select new { q1, q2 = t1 }).ToList();
                        foreach (var item1 in item.DangerousList)
                        {
                            var dan = dangerous.Find(x => x.q1.JobDangerousId == item1.JobDangerousId);
                            if (dan == null)
                            {
                                db.Insert(item1);
                                db.Insert(item1.MeasureList);
                            }
                            else
                            {
                                dan.q1.Content = item1.Content;
                                db.Update(dan.q1);

                                foreach (var item2 in item1.MeasureList)
                                {
                                    var mea = dan.q2.FirstOrDefault(x => x.JobMeasureId == item2.JobMeasureId);
                                    if (mea == null) db.Insert(mea);
                                    else
                                    {
                                        mea.Content = item2.Content;
                                        db.Update(mea);
                                    }
                                }
                                var deletemeasures = dan.q2.Where(x => !item1.MeasureList.Any(y => y.JobMeasureId == x.JobMeasureId)).ToList();
                                db.Delete(deletemeasures);
                            }
                        }
                        var deletedangerous = dangerous.Where(x => !item.DangerousList.Any(y => y.JobDangerousId == x.q1.JobDangerousId)).ToList();
                        db.Delete(deletedangerous.Select(x => x.q1).ToList());
                        db.Delete(deletedangerous.SelectMany(x => x.q2).ToList());
                    }
                }

                var list_jobid = model.Jobs.Select(y => y.JobId).ToList();
                var deletejobs = jobs.Where(x => !list_jobid.Contains(x.q1.JobId)).ToList();
                foreach (var item in deletejobs)
                {
                    db.Delete(item.q1);
                    var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                    where q.StartMeetingId == model.MeetingId && q.JobId == item.q1.JobId
                                    select q).ToList();

                    var jobusers = (from q1 in db.IQueryable<JobUserEntity>()
                                    join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                    where q2.StartMeetingId == model.MeetingId && q2.JobId == item.q1.JobId
                                    select q1).ToList();
                    var dangerous = (from q1 in db.IQueryable<JobDangerousEntity>()
                                     join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into1
                                     where q1.JobId == item.q1.JobId
                                     select new { q1, q2 = into1 }).ToList();

                    db.Delete(relation);
                    db.Delete(jobusers);
                    db.Delete(dangerous.Select(x => x.q1).ToList());
                    db.Delete(dangerous.SelectMany(x => x.q2).ToList());

                    if (item.q2.JobType == "旁站监督")
                        new SkSystem().DelTask(item.q2.RecId);
                }

                var unsigndata = new List<UnSignRecordEntity>();
                UserWorkStateService bll = new UserWorkStateService();//个人状态service
                var signins = (from q in db.IQueryable<MeetingSigninEntity>()
                               where q.MeetingId == model.MeetingId
                               select q).ToList();
                foreach (var item in model.Signins)
                {
                    var signin = signins.Find(x => x.SigninId == item.SigninId);
                    if (signin != null)
                    {
                        signin.IsSigned = item.IsSigned;
                        signin.Reason = item.Reason;
                        signin.MentalCondition = item.MentalCondition;
                        signin.ClosingCondition = item.ClosingCondition;
                        db.Update(signin);
                    }
                    if (!signin.IsSigned)
                    {
                        unsigndata.Add(new UnSignRecordEntity() { UnSignRecordId = Guid.NewGuid().ToString(), UserId = signin.UserId, UserName = signin.PersonName, UnSignDate = entity.MeetingStartTime.Date, StartTime = new DateTime(entity.MeetingStartTime.Year, entity.MeetingStartTime.Month, entity.MeetingStartTime.Day, 8, 30, 0), EndTime = new DateTime(entity.MeetingStartTime.Year, entity.MeetingStartTime.Month, entity.MeetingStartTime.Day, 12, 0, 0), Hours = 3.5f, Reason = signin.Reason });
                    }
                    #region  同步个人状态
                    var getState = bll.selectState(item.UserId);
                    if (getState == null)
                    {
                        getState = new UserWorkStateEntity();
                        getState.userId = item.UserId;
                    }
                    getState.State = string.IsNullOrEmpty(item.Reason) ? "" : item.Reason;
                    bll.addState(getState);
                    #endregion
                }


                var DayTime = model.MeetingStartTime.Date;
                var DayEndTime = DayTime.AddDays(1).AddMilliseconds(-1);
                var IsOne = from q in db.IQueryable<WorkmeetingEntity>()
                            where q.MeetingStartTime >= DayTime && q.MeetingStartTime <= DayEndTime && q.MeetingType == "班前会" && q.GroupId == model.GroupId
                            select q;

                if (model.IsOver && IsOne.Count() == 1)
                    db.Insert(unsigndata);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public void finishwork(WorkmeetingEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            var wpservice = new WorkPlanService();
            try
            {
                foreach (var item in model.Jobs)
                {
                    var job = (from q in db.IQueryable<MeetingJobEntity>()
                               where q.JobId == item.JobId
                               select q).FirstOrDefault();

                    if (job != null)
                    {
                        job.IsFinished = item.IsFinished;
                        db.Update(job);

                        var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                        where q.MeetingJobId == item.Relation.MeetingJobId
                                        select q).FirstOrDefault();
                        relation.IsFinished = item.IsFinished;
                        db.Update(relation);

                        //20190409_xjl  更新任务计划中子任务的完成状态
                        if (relation.IsFinished == "finish")
                        {
                            var wpcontent = wpservice.GetWorkPlanContentEntity(relation.WorkPlanContentId);
                            if (wpcontent != null)
                            {
                                //更新子任务
                                wpcontent.IsFinished = "已完成";
                                wpservice.SaveWorkPlanContent(wpcontent.ID, wpcontent);
                                //更新父任务
                                var parentwork = wpservice.GetWorkPlanContentEntity(wpcontent.ParentId);
                                if (parentwork != null)
                                {
                                    var childlist = wpservice.GetContentList("").Where(x => x.ParentId == parentwork.ID && x.IsFinished == "未完成");
                                    if (childlist.Count() == 0) //所有子任务均已完成
                                    {
                                        parentwork.IsFinished = "已完成";
                                        wpservice.SaveWorkPlanContent(parentwork.ID, parentwork);
                                        //更新任务计划
                                        var plan = wpservice.GetWorkPlanEntity(parentwork.PlanId);
                                        if (plan != null)
                                        {
                                            var parentworks = wpservice.GetContentList(plan.ID).Where(x => x.IsFinished == "未完成");
                                            if (parentworks.Count() == 0)
                                            {
                                                plan.IsFinished = "已完成";
                                                wpservice.SaveWorkPlan(plan.ID, plan);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public void EditEndMeeting(WorkmeetingEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var meetingquery = from q in db.IQueryable<WorkmeetingEntity>()
                                   where q.MeetingId == model.MeetingId
                                   select q;

                var entity = meetingquery.FirstOrDefault();
                if (entity == null)
                {
                    db.Commit();
                    return;
                }

                var signinquery = from q in db.IQueryable<MeetingSigninEntity>()
                                  where q.MeetingId == model.MeetingId
                                  select q;

                var signins = signinquery.ToList();
                var unsigndata = new List<UnSignRecordEntity>();
                foreach (var item in model.Signins)
                {
                    var signin = signins.Find(x => x.SigninId == item.SigninId);
                    if (signin != null)
                    {
                        signin.IsSigned = item.IsSigned;
                        signin.Reason = item.Reason;
                    }
                    if (!signin.IsSigned)
                    {
                        unsigndata.Add(new UnSignRecordEntity() { UnSignRecordId = Guid.NewGuid().ToString(), UserId = signin.UserId, UserName = signin.PersonName, UnSignDate = entity.MeetingStartTime.Date, StartTime = new DateTime(entity.MeetingStartTime.Year, entity.MeetingStartTime.Month, entity.MeetingStartTime.Day, 13, 30, 0), EndTime = new DateTime(entity.MeetingStartTime.Year, entity.MeetingStartTime.Month, entity.MeetingStartTime.Day, 18, 0, 0), Hours = 4.5f, Reason = signin.Reason });
                    }
                }
                var DayTime = model.MeetingStartTime.Date;
                var DayEndTime = DayTime.AddDays(1).AddMilliseconds(-1);
                var IsOne = from q in db.IQueryable<WorkmeetingEntity>()
                            where q.MeetingStartTime >= DayTime && q.MeetingStartTime <= DayEndTime && q.MeetingType == "班前会" && q.GroupId == model.GroupId
                            select q;

                if (model.IsOver && IsOne.Count() == 1)
                    db.Insert(unsigndata);

                db.Update(signins);

                var wpservice = new WorkPlanService();
                foreach (var item in model.Jobs)
                {
                    var job = (from q in db.IQueryable<MeetingJobEntity>()
                               where q.JobId == item.JobId
                               select q).FirstOrDefault();

                    if (job != null)
                    {
                        job.IsFinished = item.IsFinished;
                        db.Update(job);

                        var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                        where q.MeetingJobId == item.Relation.MeetingJobId
                                        select q).FirstOrDefault();
                        relation.IsFinished = item.IsFinished;
                        db.Update(relation);

                        //20190409_xjl  更新任务计划中子任务的完成状态
                        if (relation.IsFinished == "finish")
                        {
                            var wpcontent = wpservice.GetWorkPlanContentEntity(relation.WorkPlanContentId);
                            if (wpcontent != null)
                            {
                                //更新子任务
                                wpcontent.IsFinished = "已完成";
                                wpservice.SaveWorkPlanContent(wpcontent.ID, wpcontent);
                                //更新父任务
                                var parentwork = wpservice.GetWorkPlanContentEntity(wpcontent.ParentId);
                                if (parentwork != null)
                                {
                                    var childlist = wpservice.GetContentList("").Where(x => x.ParentId == parentwork.ID && x.IsFinished == "未完成");
                                    if (childlist.Count() == 0) //所有子任务均已完成
                                    {
                                        parentwork.IsFinished = "已完成";
                                        wpservice.SaveWorkPlanContent(parentwork.ID, parentwork);
                                        //更新任务计划
                                        var plan = wpservice.GetWorkPlanEntity(parentwork.PlanId);
                                        if (plan != null)
                                        {
                                            var parentworks = wpservice.GetContentList(plan.ID).Where(x => x.IsFinished == "未完成");
                                            if (parentworks.Count() == 0)
                                            {
                                                plan.IsFinished = "已完成";
                                                wpservice.SaveWorkPlan(plan.ID, plan);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                entity.ShouldJoin = signins.Count;
                entity.ActuallyJoin = signins.Count(x => x.IsSigned);
                entity.Remark = model.Remark;
                if (model.IsOver)
                    entity.MeetingEndTime = model.MeetingEndTime;

                if (model.IsOver)
                {
                    entity.IsOver = model.IsOver;

                    var dangerquery = from q3 in db.IQueryable<MeetingAndJobEntity>()
                                      join q2 in db.IQueryable<DangerEntity>() on q3.MeetingJobId equals q2.JobId
                                      where q3.EndMeetingId == model.MeetingId
                                      select q2;
                    var dangers = dangerquery.ToList();
                    dangers.ForEach(x => x.Status = x.Status == 2 ? x.Status : 3);
                    db.Update(dangers);
                }

                db.Update(entity);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }

        }

        public WorkmeetingEntity GetWorkMeeting(string deptid, DateTime date)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<WorkmeetingEntity>()
                        where q.GroupId == deptid
                        orderby q.MeetingStartTime descending
                        select q;

            var meeting = query.FirstOrDefault();

            return meeting;
        }

        public WorkmeetingEntity HomeMeetingDetail(string startmeetingid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var meeting = new WorkmeetingEntity();
            meeting.OtherMeetingId = startmeetingid;
            meeting.MeetingType = "班后会";
            var jobquery = from q in db.IQueryable<MeetingJobEntity>()
                           join q2 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q2.JobId
                           where q2.StartMeetingId == startmeetingid || q2.EndMeetingId == startmeetingid
                           select q;
            meeting.Jobs = jobquery.ToList();
            foreach (var item in meeting.Jobs)
            {
                var jobusers = from q in db.IQueryable<JobUserEntity>()
                               join q1 in db.IQueryable<MeetingAndJobEntity>() on q.MeetingJobId equals q1.MeetingJobId
                               where q1.JobId == item.JobId
                               select q;

                //item.Persons = jobusers.ToList();
                item.Files = (from q in db.IQueryable<FileInfoEntity>()
                              where q.RecId == item.JobId
                              select q).ToList();
            }

            var signinquery = from q in db.IQueryable<MeetingSigninEntity>()
                              where q.MeetingId == startmeetingid
                              select q;

            meeting.Signins = signinquery.ToList();
            meeting.ShouldJoin = meeting.Signins.Count;
            meeting.ActuallyJoin = meeting.Signins.Count(x => x.IsSigned);
            return meeting;
        }

        public WorkmeetingEntity BuildWorkMeeting(string startmeetingid, string deptid, DateTime date, string trainingtype)
        {
            var dayofmonth = ',' + date.Day.ToString() + ',';


            IRepository db = new RepositoryFactory().BaseRepository();

            var confirmquery = from q in db.IQueryable<WorkmeetingEntity>()
                               where q.GroupId == deptid
                               orderby q.MeetingStartTime descending
                               select q;

            var confirm = confirmquery.FirstOrDefault();
            if (confirm != null && !confirm.IsOver)
            {
                throw new Exception("meeting is not over");
            }

            var meeting = new WorkmeetingEntity()
            {
                GroupId = deptid
            };
            var dept = (from q in db.IQueryable<DepartmentEntity>()
                        where q.DepartmentId == deptid
                        select q).FirstOrDefault();
            if (dept != null) meeting.GroupName = dept.FullName;

            if (string.IsNullOrEmpty(startmeetingid))
            {
                meeting.MeetingType = "班前会";

                var lastquery = from q in db.IQueryable<WorkmeetingEntity>()
                                where q.GroupId == deptid && q.MeetingType == "班后会"
                                orderby q.MeetingStartTime descending
                                select q;

                var last = lastquery.FirstOrDefault();

                //签到
                var userquery = from q in db.IQueryable<UserEntity>()
                                where q.DepartmentId == deptid
                                select q;

                var persons = userquery.ToList();

                meeting.Signins = persons.Select(x => new MeetingSigninEntity() { UserId = x.UserId, PersonName = x.RealName, ClosingCondition = "正常", IsSigned = true, MentalCondition = "正常" }).ToList();
                meeting.ShouldJoin = meeting.Signins.Count;
                meeting.ActuallyJoin = meeting.Signins.Count(x => x.IsSigned);
                meeting.PersonState = "正常";
                meeting.Jobs = new List<MeetingJobEntity>();

                //取消任务结转
                //if (last != null && last.MeetingType == "班后会")
                //{
                //未完成结转的任务
                //var jobquery = from q in db.IQueryable<MeetingJobEntity>()
                //               where q.EndMeetingId == last.MeetingId && q.IsFinished == "undo"
                //               select q;
                //var jobs = jobquery.ToList();
                //jobs.ForEach(x =>
                //{
                //    x.Persons = db.IQueryable<JobUserEntity>().Where(y => y.JobId == x.JobId).ToList();
                //    x.PlanId = x.JobId;
                //    meeting.Jobs.Add(x);
                //});
                //}

                //旁站监督
                var std = new DateTime(date.Year, date.Month, date.Day);
                var etd = std.AddDays(1).AddSeconds(-1);
                var meetingjobs = from q in db.IQueryable<MeetingAndJobEntity>()
                                  select q.JobId;
                var otherjobs = (from q1 in db.IQueryable<MeetingJobEntity>()
                                 where q1.GroupId == deptid && q1.JobType == "旁站监督" && !meetingjobs.Any(y => y == q1.JobId) && q1.StartTime >= std && q1.EndTime <= etd
                                 select q1).ToList();

                otherjobs.ForEach(x =>
                {
                    x.Relation = new MeetingAndJobEntity()
                    {
                        MeetingJobId = Guid.NewGuid().ToString(),
                        JobId = x.JobId,
                        IsFinished = "undo",
                        StartMeetingId = meeting.MeetingId
                    };
                    meeting.Jobs.Add(x);
                });


                //任务库任务
                var templates = GetWorkDateJob(deptid, date);// templatequery.ToList();
                foreach (var item in templates)
                {
                    var users = new List<UserEntity>();
                    if (!string.IsNullOrEmpty(item.JobPerson))
                    {
                        var jobpersons = item.JobPerson.Split(',');
                        foreach (var name in jobpersons)
                        {
                            var user = persons.Find(x => x.RealName == name);
                            if (user != null) users.Add(user);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.otherperson))
                    {
                        var jobpersons = item.otherperson.Split(',');
                        foreach (var name in jobpersons)
                        {
                            var user = persons.Find(x => x.RealName == name);
                            if (user != null) users.Add(user);
                        }
                    }
                    var job = new MeetingJobEntity()
                    {
                        Job = item.JobContent,
                        StartTime = item.JobStartTime.HasValue ? item.JobStartTime.Value : new DateTime(date.Year, date.Month, date.Day, 8, 30, 0),
                        EndTime = item.JobEndTime.HasValue ? item.JobEndTime.Value > date ? item.JobEndTime.Value :
                        new DateTime(date.Year, date.Month, date.Day, item.JobEndTime.Value.Hour, item.JobEndTime.Value.Minute, item.JobEndTime.Value.Second)
                        : new DateTime(date.Year, date.Month, date.Day, 17, 30, 0),
                        Dangerous = item.Dangerous,
                        Measure = item.Measure,
                        NeedTrain = item.EnableTraining,
                        TemplateId = item.JobId,
                        JobType = "班前班后会",
                        Relation = new MeetingAndJobEntity() { JobUserId = string.Join(",", users.Select(x => x.UserId)), JobUser = string.Join(",", users.Select(x => x.RealName)), JobUsers = users.Select(x => new JobUserEntity() { CreateDate = date, JobType = "isdoperson", JobUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.RealName }).ToList() },
                    };
                    job.DangerousList = item.DangerousList.Select(x => new JobDangerousEntity()
                    {
                        Content = x.Content,
                        CreateTime = DateTime.Now,
                        DangerousId = x.DangerousId,
                        JobDangerousId = Guid.NewGuid().ToString(),
                        JobId = job.JobId,
                        MeasureList = x.MeasureList.Select(y => new JobMeasureEntity()
                        {
                            Content = y.Content,
                            CreateTime = DateTime.Now,
                            JobMeasureId = Guid.NewGuid().ToString(),
                            MeasureId = y.MeasureId
                        }).ToList()
                    }).ToList();
                    foreach (var item1 in job.DangerousList)
                    {
                        foreach (var item2 in item1.MeasureList)
                        {
                            item2.JobDangerousId = item1.JobDangerousId;
                        }
                    }
                    meeting.Jobs.Add(job);

                    if (meeting.Jobs.Last().Relation.JobUsers.FirstOrDefault() != null) meeting.Jobs.Last().Relation.JobUsers.FirstOrDefault().JobType = "ischecker";
                }

                //跨天任务
                var jobquery = from q in db.IQueryable<MeetingJobEntity>()
                               where q.GroupId == deptid && q.StartTime <= date && q.EndTime >= date && q.IsFinished == "undo"
                               select q;
                var longjobs = jobquery.ToList().Where(x => x.EndTime.Date > x.StartTime.Date).ToList();
                var lastmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                   where q.GroupId == deptid
                                   where q.MeetingType == "班前会"
                                   orderby q.MeetingStartTime descending
                                   select q).FirstOrDefault();
                if (lastmeeting != null)
                {
                    foreach (var item in longjobs)
                    {
                        var jobusers = (from q1 in db.IQueryable<JobUserEntity>()
                                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                        where q2.StartMeetingId == lastmeeting.MeetingId && q2.JobId == item.JobId
                                        select q1).ToList();
                        item.Relation = new MeetingAndJobEntity()
                        {
                            MeetingJobId = Guid.NewGuid().ToString(),
                            StartMeetingId = meeting.MeetingId,
                            IsFinished = "undo",
                            JobId = item.JobId,
                            JobUserId = string.Join(",", jobusers.Select(x => x.UserId)),
                            JobUser = string.Join(",", jobusers.Select(x => x.UserName))
                        };
                        item.Relation.JobUsers = jobusers.Select(x => new JobUserEntity() { JobUserId = Guid.NewGuid().ToString(), JobType = x.JobType, MeetingJobId = item.Relation.MeetingJobId, UserId = x.UserId, UserName = x.UserName }).ToList();
                        meeting.Jobs.Add(item);
                    }
                }
                //跨天任务

                //20190409_xjl 查询任务计划中的子任务
                var startdate = DateTime.Now.Date;
                var enddate = DateTime.Now.Date.AddDays(1);
                var workplan = (from q in db.IQueryable<WorkPlanContentEntity>()
                                where q.BZID == deptid && q.IsFinished == "未完成" && ((q.StartDate >= startdate && q.StartDate < enddate) || (q.EndDate >= startdate && q.EndDate < enddate))
                                select q).ToList();
                //var meetajobs = (from q in db.IQueryable<MeetingAndJobEntity>()
                //                 where 1 == 1
                //                 select q.MeetingJobId).ToList();
                //workplan = workplan.Where(x => !meetajobs.Contains(x.ID)).ToList();
                foreach (var item in workplan)
                {
                    var job = new MeetingJobEntity()
                    {
                        JobId = Guid.NewGuid().ToString(),
                        Job = item.WorkContent,
                        StartTime = item.StartDate == null ? new DateTime(date.Year, date.Month, date.Day, 8, 30, 0) : item.StartDate.Value,
                        EndTime = item.EndDate == null ? new DateTime(date.Year, date.Month, date.Day, 17, 30, 0) : item.EndDate.Value,
                        Dangerous = "",
                        Measure = "",
                        IsFinished = "undo",
                        GroupId = deptid,
                        TemplateId = "",
                        NeedTrain = false
                    };
                    job.Relation = new MeetingAndJobEntity()
                    {
                        MeetingJobId = Guid.NewGuid().ToString(),
                        JobUserId = item.WorkPeopleId,
                        JobUser = item.WorkPeopleName,
                        IsFinished = "undo",
                        JobId = job.JobId,
                        WorkPlanContentId = item.ID
                    };
                    job.Relation.JobUsers = new List<JobUserEntity>();
                    var userarr = job.Relation.JobUserId.Split(',');
                    var namearr = job.Relation.JobUser.Split(',');
                    if (userarr != null && namearr != null)
                    {
                        for (int i = 0; i < userarr.Length; i++)
                        {
                            String jobtype = "isdoperson";
                            if (i == 0) jobtype = "ischecker";
                            job.Relation.JobUsers.Add(new JobUserEntity { JobUserId = Guid.NewGuid().ToString(), UserId = userarr[i], UserName = namearr[i], JobType = jobtype, CreateDate = DateTime.Now, MeetingJobId = job.JobId });
                        }
                    }
                    meeting.Jobs.Add(job);
                }
            }
            else
            {
                var startmeeting = db.FindEntity<WorkmeetingEntity>(startmeetingid);
                meeting.OtherMeetingId = startmeetingid;
                meeting.MeetingType = "班后会";
                meeting.ShouldStartTime = startmeeting.ShouldStartTime;
                meeting.ShouldEndTime = startmeeting.ShouldEndTime;

                var list = (from q1 in db.IQueryable<MeetingAndJobEntity>()
                            join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                            join q3 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q3.JobId into t3
                            from q3 in t3.DefaultIfEmpty()
                            join q4 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q4.MeetingJobId into t4
                            join q5 in (from q1 in db.IQueryable<JobDangerousEntity>()
                                        join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into t2
                                        select new { q1, q2 = t2 }) on q1.JobId equals q5.q1.JobId into t5
                            join q6 in db.IQueryable<FileInfoEntity>() on q1.MeetingJobId equals q6.RecId into t6
                            join q7 in (from q1 in db.IQueryable<HumanDangerTrainingBaseEntity>()
                                        join q2 in db.IQueryable<HumanDangerTrainingUserEntity>() on q1.TrainingId equals q2.TrainingId into t2
                                        select new { q1, q2 = t2 }) on q1.MeetingJobId equals q7.q1.MeetingJobId into t7
                            from q7 in t7.DefaultIfEmpty()
                            where q1.StartMeetingId == startmeetingid
                            select new { q1, q2, q3, q4 = t4, q5 = t5, q6 = t6, q7 }).ToList();

                list.ForEach(x =>
                {
                    x.q2.Relation = x.q1;
                    if (trainingtype == "人身风险预控")
                        x.q2.TrainingDone = x.q7 == null ? false : x.q7.q2.All(y => y.IsDone == true && y.IsMarked == true);
                    else
                        x.q2.TrainingDone = x.q3 == null ? false : x.q3.Status == 2;
                    x.q1.JobUsers = x.q4.ToList();
                    foreach (var item in x.q5)
                    {
                        item.q1.MeasureList = item.q2.ToList();
                    }
                    x.q2.DangerousList = x.q5.Select(y => y.q1).ToList();
                    x.q2.Files = x.q6.ToList();
                    if (trainingtype == "人身风险预控")
                    {
                        if (x.q7 != null)
                        {
                            x.q2.Training = new DangerEntity()
                            {
                                Id = x.q7.q2.FirstOrDefault().TrainingUserId.ToString()
                            };
                        }
                    }
                    else
                    {
                        x.q2.Training = x.q3;
                    }
                });
                meeting.Jobs = list.Select(x => x.q2).ToList();

                var signinquery = from q in db.IQueryable<MeetingSigninEntity>()
                                  where q.MeetingId == startmeetingid
                                  select q;

                meeting.Signins = signinquery.ToList();
                meeting.ShouldJoin = meeting.Signins.Count;
                meeting.ActuallyJoin = meeting.Signins.Count(x => x.IsSigned);
            }

            return meeting;
        }
        #region 周期筛选
        private IWorkOrderService OrderService = new WorkOrderService();
        private List<JobTemplateEntity> GetWorkDateJob(string deptid, DateTime date)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var templatequery = from q1 in db.IQueryable<JobTemplateEntity>()
                                join q2 in (
                                    from q1 in db.IQueryable<JobDangerousEntity>()
                                    join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                    select new { q1, q2 = into2 }
                                    ) on q1.JobId equals q2.q1.JobId into into2
                                where q1.DangerType == "job" && q1.DeptId == deptid
                                orderby q1.CreateDate
                                select new { q1, q2 = into2 };

            //任务库任务
            //获取最后一次班前会
            //var endQuery = from q in db.IQueryable<WorkmeetingEntity>()
            //               where q.GroupId == deptid && q.MeetingType == "班前会"
            //               orderby q.MeetingStartTime descending
            //               select q;
            //var betweenDay = 1;
            ////无记录
            //if (endQuery.Count() != 0)
            //{
            //    var end = endQuery.FirstOrDefault();
            //    betweenDay = Math.Abs(Util.Time.DiffDays(date, end.MeetingStartTime));
            //}
            var templates = new List<JobTemplateEntity>();
            //间隔几天 将中间未进行的任务承接给当前时间
            //if (betweenDay != 1 && betweenDay != 0)
            //{
            //    for (int i = 0; i < betweenDay; i++)
            //    {
            //        var oneday = GetDateJob(templatequery.ToList(), date.AddDays(-i));
            //        foreach (var item in oneday)
            //        {
            //            var ck = templates.FirstOrDefault(x => x.JobId == item.JobId);
            //            if (ck == null)
            //            {
            //                templates.Add(item);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //根据班次移除
            var data = templatequery.ToList();
            foreach (var item in data)
            {
                foreach (var item1 in item.q2)
                {
                    item1.q1.MeasureList = item1.q2.ToList();
                }
                item.q1.DangerousList = item.q2.Select(x => x.q1).ToList();
            }
            var jobs = data.Select(x => x.q1).ToList();
            templates = removeWork(jobs, ref date);
            templates = GetDateJob(templates, date);// templatequery.ToList();

            // }
            return templates;
        }
        /// <summary>
        /// 移除周期
        /// </summary>
        /// <param name="list"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private List<JobTemplateEntity> removeWork(List<JobTemplateEntity> list, ref DateTime date)
        {
            if (list.Count == 0 || list == null) return list;
            var nextTime = date;
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;
            #region  获取班组班次
            var Sort = new List<WorkTimeSortEntity>();

            if (list.Count() > 0)
            {
                var GetList = list.ToList();
                Sort = OrderService.GetWorkOrderList(date, date, list.ToList()[0].DeptId).ToList();
            }
            var nowSort = Sort.FirstOrDefault(x => x.year == year && x.month == month);

            if (nowSort == null)
            {
                list = list.Where(x => string.IsNullOrEmpty(x.worksetid)).ToList();
                return list;
            }

            #endregion
            #region  筛选 班次
            var SortCk = new List<JobTemplateEntity>();
            var strid = nowSort.timedataid.Split(',');
            var strData = nowSort.timedata.Split(',');
            foreach (var item in list)
            {
                SortCk.Add(item);
            }
            #region 根据排版数据进行筛选
            var DayTime = string.Empty;
            var daySum = date.Day - 1;
            //开始时间是否为休息
            while (true)
            {
                //获取到时间点
                if (strid[daySum] != "0")
                {
                    DayTime = strData[daySum];
                    break;
                }
                daySum++;
                date = date.AddDays(1);
                if (strid.Length == daySum)
                {
                    year = date.Year;
                    month = date.Month;
                    Sort = OrderService.GetWorkOrderList(date, date, list.ToList()[0].DeptId).ToList();
                    nowSort = Sort.FirstOrDefault(x => x.year == year && x.month == month);
                    strid = nowSort.timedataid.Split(',');
                    strData = nowSort.timedata.Split(',');
                    daySum = 0;
                }
            }

            //根据时间判读是否需要递进
            var nowTime = DateTime.Now;
            var dateTimeSpan = DayTime.Split('(')[0];
            var startSpan = dateTimeSpan.Split('-')[0];
            var endSpan = dateTimeSpan.Split('-')[1];
            var ckStart = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + startSpan);
            var workTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + endSpan);
            //跨天
            if (ckStart >= workTime)
            {
                workTime = workTime.AddDays(1);
            }
            if (nowTime > workTime && nextTime == date)
            {
                //查找下一个时间点
                //开始时间是否为休息
                while (true)
                {
                    daySum++;
                    date = date.AddDays(1);
                    if (strid.Length == daySum)
                    {
                        year = date.Year;
                        month = date.Month;
                        Sort = OrderService.GetWorkOrderList(date, date, list.ToList()[0].DeptId).ToList();
                        nowSort = Sort.FirstOrDefault(x => x.year == year && x.month == month);
                        strid = nowSort.timedataid.Split(',');
                        strData = nowSort.timedata.Split(',');
                        daySum = 0;
                    }
                    //获取到时间点
                    if (strid[daySum] != "0")
                    {
                        DayTime = strData[daySum];
                        dateTimeSpan = DayTime.Split('(')[0];
                        startSpan = dateTimeSpan.Split('-')[0];
                        endSpan = dateTimeSpan.Split('-')[1];
                        break;
                    }


                }
            }
            #endregion
            foreach (var item in SortCk)
            {
                if (nowSort == null)
                {
                    list.Remove(item);
                }
                else
                {

                    //对比排班计算是否排除
                    if (item.worksetid != null)
                    {
                        if (strid[date.Day - 1] != "0")
                        {
                            var banci = strData[date.Day - 1].Split('(')[1].Replace(")", "");
                            if (!item.worksetname.Contains(banci))
                            {
                                list.Remove(item);

                            }

                        }
                        else
                        {
                            if (strid[date.Day - 1] == "0")
                            {
                                list.Remove(item);
                            }
                            else
                            {

                                var updateOne = list.FirstOrDefault(x => x.JobId == item.JobId);

                                var startHour = Convert.ToInt32(startSpan.Split(':')[0]);
                                var endHour = Convert.ToInt32(endSpan.Split(':')[0]);
                                if (startHour <= endHour)
                                {
                                    var nowDate = date.ToString("yyyy-MM-dd");
                                    updateOne.JobStartTime = Convert.ToDateTime(nowDate + " " + startSpan);
                                    updateOne.JobEndTime = Convert.ToDateTime(nowDate + " " + endSpan);
                                }
                                else
                                {
                                    var nowDate = date.ToString("yyyy-MM-dd");
                                    var nextDate = date.AddDays(1).ToString("yyyy-MM-dd");
                                    updateOne.JobStartTime = Convert.ToDateTime(nowDate + " " + startSpan);
                                    updateOne.JobEndTime = Convert.ToDateTime(nextDate + " " + endSpan);
                                }
                            }

                        }
                    }
                    else
                    {
                        var updateOne = list.FirstOrDefault(x => x.JobId == item.JobId);
                        if (updateOne.JobStartTime.HasValue && updateOne.JobEndTime.HasValue)
                        {


                            var startStr = updateOne.JobStartTime.Value.ToString("HH:mm");
                            var endStr = updateOne.JobEndTime.Value.ToString("HH:mm");
                            var startHour = Convert.ToInt32(updateOne.JobStartTime.Value.ToString("HH"));
                            var endHour = Convert.ToInt32(updateOne.JobEndTime.Value.ToString("HH"));
                            if (startHour <= endHour)
                            {
                                var nowDate = date.ToString("yyyy-MM-dd");
                                updateOne.JobStartTime = Convert.ToDateTime(nowDate + " " + startSpan);
                                updateOne.JobEndTime = Convert.ToDateTime(nowDate + " " + endSpan);
                            }
                            else
                            {
                                var nowDate = date.ToString("yyyy-MM-dd");
                                var nextDate = date.AddDays(1).ToString("yyyy-MM-dd");
                                updateOne.JobStartTime = Convert.ToDateTime(nowDate + " " + startSpan);
                                updateOne.JobEndTime = Convert.ToDateTime(nextDate + " " + endSpan);
                            }
                        }
                    }

                    if (strid[date.Day - 1] == "0")
                    {
                        list.Remove(item);
                    }
                }
            }
            #endregion
            return list;
        }
        // 根据规则过滤任务数据
        private List<JobTemplateEntity> GetDateJob(List<JobTemplateEntity> list, DateTime date)
        {

            List<JobTemplateEntity> jobs = new List<JobTemplateEntity>();


            var month = date.Month.ToString();//月
            var day = date.Day.ToString();//天

            #region  月份转化为大写
            switch (month)
            {
                case "1":
                    month = "一月";
                    break;
                case "2":
                    month = "二月";
                    break;
                case "3":
                    month = "三月";
                    break;
                case "4":
                    month = "四月";
                    break;
                case "5":
                    month = "五月";
                    break;
                case "6":
                    month = "六月";
                    break;
                case "7":
                    month = "七月";
                    break;
                case "8":
                    month = "八月";
                    break;
                case "9":
                    month = "九月";
                    break;
                case "10":
                    month = "十月";
                    break;
                case "11":
                    month = "十一月";
                    break;
                case "12":
                    month = "十二月";
                    break;
            }
            #endregion
            #region  计算天为本月的第几周
            var newdate = new DateTime(date.Year, date.Month, 1);
            int num = Util.Time.GetWeekNumberOfDay(newdate);//一号为星期几
            int totalday = Util.Time.GetDaysOfMonth(date.Year, date.Month);//本月有多少天
                                                                           //容器存放周数据
            var weeknum = 1;
            var page = 1;
            var weekstr = string.Empty;//星期几
            var weeknumstr = string.Empty;//第几周
            while (true)
            {
                #region 转化
                switch (num)
                {
                    case 1:
                        weekstr = "星期一";
                        break;
                    case 2:
                        weekstr = "星期二";
                        break;
                    case 3:
                        weekstr = "星期三";
                        break;
                    case 4:
                        weekstr = "星期四";
                        break;
                    case 5:
                        weekstr = "星期五";
                        break;
                    case 6:
                        weekstr = "星期六";
                        break;
                    default:
                        weekstr = "星期天";
                        break;
                }
                switch (weeknum)
                {
                    case 1:
                        weeknumstr = "第一周";
                        break;
                    case 2:
                        weeknumstr = "第二周";
                        break;
                    case 3:
                        weeknumstr = "第三周";
                        break;
                    case 4:
                        weeknumstr = "第四周";
                        break;
                    case 5:
                        weeknumstr = "第五周";
                        break;
                    case 6:
                        weeknumstr = "第六周";
                        break;
                    default:
                        weeknumstr = "第一周";
                        break;
                }
                if (page == date.Day)
                {
                    break;
                }
                #endregion
                //如果是星期7则加一个星期
                if (num == 7)
                {
                    weeknum++;
                    num = 1; page++;
                }
                else { num++; page++; }

            }
            #endregion
            //存放星期顺序容器
            List<string> weeksort = new List<string> {
                "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天"
            };
            list = list.OrderByDescending(x => x.CreateDate).ToList();
            #region  数据筛选
            foreach (var item in list)
            {
                bool ck = false;
                //每天单独计算
                if (item.Cycle == "每天")
                {
                    if (item.isweek)
                    {
                        if (weekstr == "星期六" || weekstr == "星期天")
                        {
                            continue;
                        }
                    }
                    var senddate = new DateTime(date.Year, date.Month, date.Day);
                    var tosenddate = new DateTime(date.Year, date.Month, date.Day);
                    GetStartEndTime(ref senddate, ref tosenddate, item.DeptId);
                    item.JobStartTime = senddate;
                    item.JobEndTime = tosenddate;
                    jobs.Add(item);
                    continue;
                }
                //  不勾选日期  勾选最后一天  跳过双休  是否截止 特殊情况:每月才存在
                if (string.IsNullOrEmpty(item.CycleDate))
                {
                    //可选择是否双休 最后一天
                    if (item.isweek)
                    {
                        if (weekstr == "星期六" || weekstr == "星期天")
                        {
                            ck = false;
                            continue;
                        }
                    }
                    //判断是否最后一天   是否截止日期 截止只有一个日期 勾选最后一天后可以不选日期
                    if (item.islastday)
                    {

                        var daySum = Util.Time.GetDaysOfMonth(date);
                        if (item.isend)
                        {

                            var senddate = new DateTime(date.Year, date.Month, date.Day);
                            var start = new DateTime(date.Year, date.Month, 1);
                            var tosenddate = new DateTime(date.Year, date.Month, totalday).AddDays(1).AddMinutes(-1);
                            var workCk = ckWorkMeeting(start, tosenddate, item.DeptId);
                            //存在班会
                            if (workCk)
                            {
                                GetStartEndTime(ref senddate, ref tosenddate, item.DeptId);
                                item.JobStartTime = senddate;
                                item.JobEndTime = tosenddate;
                                jobs.Add(item);
                            }
                            continue;
                            //if (daySum >= date.Day)
                            //{
                            //    jobs.Add(item);
                            //    continue;
                            //}
                        }
                        else
                        if (date.Day == daySum)
                        {
                            jobs.Add(item);
                            continue;
                        }
                    }
                    continue;
                }
                //获取规则数组
                var useTimeStr = item.CycleDate.Split(';');
                #region
                //规则 年  多选月  多选第几周、几号 多选星期几 班次  是否截至 是否跳过最后一天
                //获取节点  在基础节点判断是否可以推送
                for (int i = 0; i < useTimeStr.Length; i++)
                {
                    var str = useTimeStr[i].ToString();
                    //多选时  按规则顺序 if

                    //每年 每日期  勾选最后一天  特殊情况   每年 二月,八月;
                    if (string.IsNullOrEmpty(str))
                    {
                        //可选择是否双休 最后一天
                        if (item.isweek)
                        {
                            if (weekstr == "星期六" || weekstr == "星期天")
                            {
                                ck = false;
                                break;
                            }
                        }
                        //判断是否最后一天   是否截止日期 截止只有一个日期 勾选最后一天后可以不选日期
                        if (item.islastday)
                        {

                            var daySum = Util.Time.GetDaysOfMonth(date);
                            if (item.isend)
                            {

                                var senddate = new DateTime(date.Year, date.Month, date.Day);
                                var start = new DateTime(date.Year, date.Month, 1);
                                var tosenddate = new DateTime(date.Year, date.Month, totalday).AddDays(1).AddMinutes(-1);
                                var workCk = ckWorkMeeting(start, tosenddate, item.DeptId);
                                //存在班会
                                if (workCk)
                                {
                                    GetStartEndTime(ref senddate, ref tosenddate, item.DeptId);
                                    item.JobStartTime = senddate;
                                    item.JobEndTime = tosenddate;
                                    ck = true;
                                    break;
                                }
                                else
                                {
                                    ck = false;
                                    break;
                                }

                            }
                            else

                            //if (daySum >= date.Day)
                            //{
                            //    ck = true;
                            //    break;
                            //}
                            if (date.Day == daySum)
                            {
                                ck = true;
                                break;
                            }

                        }
                    }

                    //哪几个月
                    if (str.Contains("月"))
                    {
                        var ckstr = str.Split(',');
                        for (int j = 0; j < ckstr.Length; j++)
                        {
                            ck = ckstr[j] == month ? true : false;
                            if (ck)
                            {
                                break;
                            }
                        }
                        if (!ck) break;

                    }
                    //第几周
                    else
                        if (str.Contains("周"))
                    {
                        ck = str.Contains(weeknumstr) ? true : false;
                        if (!ck) break;

                    }
                    //哪几个星期
                    else
                            if (str.Contains("星期"))
                    {
                        //是否截止日期 截止只有一个日期
                        if (item.isend)
                        {
                            //var sort = weeksort.IndexOf(str);//配置
                            //var nowsort = weeksort.IndexOf(weekstr);
                            //if (nowsort <= sort)
                            //{
                            //    ck = true;
                            //    break;
                            //}
                            var nowsort = weeksort.IndexOf(weekstr);
                            var sort = weeksort.IndexOf(str);//配置
                            var senddate = new DateTime(date.Year, date.Month, date.Day);
                            var start = senddate.AddDays(-nowsort);
                            var tosenddate = start.AddDays(sort + 1).AddMinutes(-1);
                            if (start > tosenddate)
                            {
                                ck = false;
                                break;
                            }
                            var workCk = ckWorkMeeting(start, tosenddate, item.DeptId);
                            //存在班会
                            if (workCk)
                            {
                                GetStartEndTime(ref senddate, ref tosenddate, item.DeptId);
                                item.JobStartTime = senddate;
                                item.JobEndTime = tosenddate;
                                ck = true;
                                break;

                            }
                            else
                            {
                                ck = false;
                                break;
                            }




                        }
                        ck = str.Contains(weekstr) ? true : false;

                    }
                    //哪几号
                    else
                    {

                        //可选择是否双休 最后一天
                        if (item.isweek)
                        {
                            if (weekstr == "星期六" || weekstr == "星期天")
                            {
                                ck = false;
                                break;
                            }
                        }


                        //是否截止日期 截止只有一个日期
                        if (item.isend)
                        {

                            var senddate = new DateTime(date.Year, date.Month, date.Day); ;
                            var dyaNum = Convert.ToInt32(str);
                            if (totalday < dyaNum)
                            {
                                ck = false;
                                break;
                            }
                            var start = new DateTime(date.Year, date.Month, 1);

                            var tosenddate = new DateTime(date.Year, date.Month, dyaNum).AddDays(1).AddMinutes(-1);
                            if (senddate > tosenddate)
                            {
                                ck = false;
                                break;
                            }
                            var workCk = ckWorkMeeting(start, tosenddate, item.DeptId);
                            //存在班会
                            if (workCk)
                            {
                                GetStartEndTime(ref senddate, ref tosenddate, item.DeptId);
                                item.JobStartTime = senddate;
                                item.JobEndTime = tosenddate;
                                ck = true;
                                break;
                            }
                            else
                            {
                                ck = false;
                                break;
                            }

                            //var setday = Convert.ToInt32(str);
                            //if (setday >= date.Day)
                            //{
                            //    ck = true;
                            //    break;
                            //}
                        }
                        //判断是否最后一天
                        if (item.islastday)
                        {
                            var daySum = Util.Time.GetDaysOfMonth(date);
                            if (date.Day == daySum)
                            {
                                ck = true;
                                break;
                            }
                        }

                        var ckstr = str.Split(',');
                        for (int j = 0; j < ckstr.Length; j++)
                        {
                            ck = ckstr[j] == day ? true : false;
                            if (ck)
                            {
                                break;
                            }
                        }


                    }
                    #endregion
                }
                if (ck)
                {
                    jobs.Add(item);
                }
            }
            #endregion

            return jobs;
        }
        /// <summary>
        /// 特殊情况 周期中存在“截止”设定  获取区间是否开班会
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="deptid"></param>
        private bool ckWorkMeeting(DateTime start, DateTime end, string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<WorkmeetingEntity>()
                        where q.GroupId == deptid && q.MeetingStartTime >= start && q.MeetingStartTime <= end
                        orderby q.MeetingStartTime descending
                        select q;
            var entity = query.FirstOrDefault();
            if (entity == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        // 特殊情况 周期中存在“截止”设定 根据班次设置开始结束时间
        private void GetStartEndTime(ref DateTime start, ref DateTime end, string deptid)
        {
            var startStr = start.ToString("yyyy-MM-dd");
            var endStr = end.ToString("yyyy-MM-dd");
            var startMonth = start.Month;
            var startday = start.Day;
            var endmonth = end.Month;
            var endday = end.Day;
            //获取班次
            var Sort = OrderService.GetWorkOrderList(start, end, deptid).ToList();
            if (Sort.Count > 0)
            {
                //是否存在当前周期数据
                var StartSort = Sort.FirstOrDefault(x => x.month == startMonth);
                if (StartSort != null)
                {
                    var strData = StartSort.timedata.Split(',');
                    if (strData[startday - 1] != "无" && strData[startday - 1] != "休息")
                    {
                        var timeStr = strData[startday - 1].Split('(')[0];
                        var startTimeStr = timeStr.Split('-')[0];
                        start = Convert.ToDateTime(startStr + " " + startTimeStr);

                    }
                    else
                    {
                        start = Convert.ToDateTime(startStr + " 00:01");
                    }
                }
                //是否存在当前周期数据
                var endSort = Sort.FirstOrDefault(x => x.month == endmonth);
                if (endSort != null)
                {
                    var strData = endSort.timedata.Split(',');
                    if (strData[endday - 1] != "无" && strData[endday - 1] != "休息")
                    {
                        var timeStr = strData[endday - 1].Split('(')[0];
                        var endTimeStr = timeStr.Split('-')[1];
                        end = Convert.ToDateTime(endStr + " " + endTimeStr);

                    }
                    else
                    {
                        end = Convert.ToDateTime(endStr + " 23:59");
                    }
                }


            }
            else
            {
                start = Convert.ToDateTime(startStr + " 00:01");
                end = Convert.ToDateTime(endStr + " 23:59");
            }
        }

        #endregion
        public WorkmeetingEntity GetLastMeeting(string deptId)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<WorkmeetingEntity>()
                        where q.GroupId == deptId
                        orderby q.MeetingStartTime descending
                        select q;

            return query.FirstOrDefault();
        }

        public void OverMeeting(string meetingid, DateTime date, string trainingtype, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                               where q.MeetingId == meetingid
                               select q).FirstOrDefault();

                var dept = db.FindEntity<DepartmentEntity>(meeting.GroupId);

                if (meeting.IsOver)
                {
                    db.Commit();
                    return;
                }

                meeting.IsOver = true;
                meeting.MeetingEndTime = date;

                meeting.Files = null;
                meeting.Jobs = null;
                meeting.Signins = null;

                var signs = (from q in db.IQueryable<MeetingSigninEntity>()
                             where q.MeetingId == meetingid
                             select q).ToList();
                if (signs.Count(x => x.Reason == "替班") > 0)
                {
                    meeting.MeetingPerson = signs.Find(x => x.Reason == "替班").ReasonRemark;
                }
                var unsigndata = new List<UnSignRecordEntity>();
                foreach (var signin in signs)
                {
                    if (!signin.IsSigned)
                    {
                        if (meeting.MeetingType == "班前会")
                        {
                            unsigndata.Add(new UnSignRecordEntity() { UnSignRecordId = Guid.NewGuid().ToString(), UserId = signin.UserId, UserName = signin.PersonName, UnSignDate = meeting.MeetingStartTime.Date, StartTime = new DateTime(meeting.MeetingStartTime.Year, meeting.MeetingStartTime.Month, meeting.MeetingStartTime.Day, 8, 30, 0), EndTime = new DateTime(meeting.MeetingStartTime.Year, meeting.MeetingStartTime.Month, meeting.MeetingStartTime.Day, 12, 0, 0), Hours = 3.5f, Reason = signin.Reason });

                        }
                        else
                        {
                            unsigndata.Add(new UnSignRecordEntity() { UnSignRecordId = Guid.NewGuid().ToString(), UserId = signin.UserId, UserName = signin.PersonName, UnSignDate = meeting.MeetingStartTime.Date, StartTime = new DateTime(meeting.MeetingStartTime.Year, meeting.MeetingStartTime.Month, meeting.MeetingStartTime.Day, 13, 30, 0), EndTime = new DateTime(meeting.MeetingStartTime.Year, meeting.MeetingStartTime.Month, meeting.MeetingStartTime.Day, 18, 0, 0), Hours = 4.5f, Reason = signin.Reason });
                        }

                    }
                }
                var DayTime = meeting.MeetingStartTime.Date;
                var DayEndTime = DayTime.AddDays(1).AddMilliseconds(-1);
                var IsOne = from q in db.IQueryable<WorkmeetingEntity>()
                            where q.MeetingStartTime >= DayTime && q.MeetingStartTime <= DayEndTime && q.MeetingType == "班前会"
                            && q.GroupId == meeting.GroupId
                            select q;

                if (meeting.IsOver && IsOne.Count() == 1)
                    db.Insert(unsigndata);

                db.Update(meeting);


                if (meeting.MeetingType == "班前会")
                {
                    var jobs = (from q1 in db.IQueryable<MeetingAndJobEntity>()
                                join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                                join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into into3
                                where q1.StartMeetingId == meeting.MeetingId
                                select new { q1, q2, q3 = into3 }).ToList();
                    var trainings = new List<HumanDangerTrainingBaseEntity>();
                    var dangers = new List<DangerEntity>();
                    foreach (var item in jobs)
                    {
                        if (trainingtype == "CARC")
                        {
                            //if (item.q2.NeedTrain)
                            //{
                            var card = new CarcEntity
                            {
                                Id = Guid.NewGuid().ToString(),
                                WorkName = item.q2.Job,
                                WorkArea = item.q2.JobAddr,
                                DataType = item.q2.NeedTrain ? "carc" : "card",
                                StartTime = item.q2.StartTime,
                                EndTime = item.q2.EndTime,
                                MeetId = item.q1.StartMeetingId,
                                TutelagePersonId = item.q3.FirstOrDefault(x => x.JobType == "ischecker")?.UserId,
                                TutelagePerson = item.q3.FirstOrDefault(x => x.JobType == "ischecker")?.UserName,
                                OperationPersonId = string.Join(",", item.q3.Where(x => x.JobType == "isdoperson").Select(x => x.UserId)),
                                OperationPerson = string.Join(",", item.q3.Where(x => x.JobType == "isdoperson").Select(x => x.UserName)),

                            };
                            new CarcOrCardService().SaveForm(new List<CarcEntity>() { card }, userid);
                            //}
                        }
                        else if (trainingtype == "人身风险预控")
                        {
                            if (item.q2.NeedTrain)
                            {

                                var cnt = (from q in db.IQueryable<HumanDangerTrainingBaseEntity>()
                                           where q.MeetingJobId == item.q1.MeetingJobId
                                           select 1).Count();
                                if (cnt > 0) continue;

                                var training = new HumanDangerTrainingBaseEntity() { TrainingId = Guid.NewGuid().ToString(), HumanDangerId = item.q2.TemplateId, TrainingTask = item.q2.Job, CreateTime = DateTime.Now, CreateUserId = userid, MeetingJobId = item.q1.MeetingJobId, DeptId = meeting.GroupId, DeptName = dept.FullName };
                                training.TrainingUsers = new List<HumanDangerTrainingUserEntity>();
                                foreach (var item1 in item.q3)
                                {
                                    training.TrainingUsers.Add(new HumanDangerTrainingUserEntity()
                                    {
                                        TrainingUserId = Guid.NewGuid().ToString(),
                                        UserId = item1.UserId,
                                        UserName = item1.UserName,
                                        TrainingPlace = item.q2.JobAddr,
                                        No = item.q2.TicketCode,
                                        TrainingRole = item1.JobType == "ischecker" ? 1 : 0,
                                        TrainingId = training.TrainingId,
                                        TicketId = item.q2.TicketId
                                    });
                                }
                                if (!string.IsNullOrEmpty(item.q2.TemplateId))
                                {
                                    var hm = (from q in db.IQueryable<HumanDangerEntity>()
                                              where q.HumanDangerId.ToString() == item.q2.TemplateId
                                              select q).FirstOrDefault();
                                    if (hm == null) training.HumanDangerId = null;
                                }
                                trainings.Add(training);
                            }
                        }
                        else
                        {
                            if (item.q2.NeedTrain)
                            {

                                var cnt = (from q in db.IQueryable<DangerEntity>()
                                           where q.JobId == item.q1.MeetingJobId
                                           select q).Count();
                                if (cnt > 0) continue;

                                var danger = new DangerEntity
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    GroupId = meeting.GroupId,
                                    GroupName = dept.FullName,
                                    JobId = item.q1.MeetingJobId,
                                    JobName = item.q2.Job,
                                    Persons = item.q1.JobUser,
                                    JobUser = item.q1.JobUser,
                                    JobTime = item.q2.StartTime,
                                    Sno = DateTime.Now.ToString("yyyyMMddHHmmss"),
                                    DeptCode = dept.EnCode,
                                    CreateDate = DateTime.Now,
                                    CreateUserId = userid,
                                    TicketId = item.q2.TicketCode
                                };
                                dangers.Add(danger);
                            }
                        }
                    }
                    if (trainings.Count > 0)
                    {
                        db.Insert(trainings);
                        db.Insert(trainings.SelectMany(x => x.TrainingUsers).ToList());
                    }
                    if (dangers.Count > 0)
                    {
                        db.Insert(dangers);
                    }
                }
                else
                {
                    var dangerquery = from q3 in db.IQueryable<MeetingAndJobEntity>()
                                      join q2 in db.IQueryable<DangerEntity>() on q3.MeetingJobId equals q2.JobId
                                      where q3.EndMeetingId == meetingid
                                      select q2;
                    var dangers = dangerquery.ToList();
                    dangers.ForEach(x => x.Status = x.Status == 2 ? x.Status : 3);
                    db.Update(dangers);

                    //更新部门任务状态
                    var list = (from q1 in db.IQueryable<MeetingAndJobEntity>()
                                join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                                join q3 in db.IQueryable<DepartmentTaskEntity>() on q2.TemplateId equals q3.TaskId
                                where q1.IsFinished == "finish" && q3.Status != "已取消"
                                select q3).ToList();
                    list.ForEach(x => x.Status = "已完成");
                    db.Update(list);
                }

                var edu = eduBaseInfos.FirstOrDefault(x => x.MeetingId == meetingid);
                if (edu != null)
                {
                    var signins = meetingSignins.AsNoTracking().Where(x => x.MeetingId == meetingid && x.IsSigned == true).ToList();

                    _context.Entry(edu).State = EntityState.Modified;
                    edu.AttendNumber = signins.Count;
                    edu.AttendPeople = string.Join(",", signins.Select(x => x.PersonName));
                    edu.AttendPeopleId = string.Join(",", signins.Select(x => x.UserId));
                    edu.Flow = "1";
                    _context.SaveChanges();
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public string StartEndMeeting(WorkmeetingEntity meeting)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var startmeeting = db.FindEntity<WorkmeetingEntity>(meeting.MeetingId);
                if (startmeeting == null) return null;

                var signinquery = from q in db.IQueryable<MeetingSigninEntity>()
                                  where q.MeetingId == meeting.MeetingId
                                  select q;

                var signgins = signinquery.ToList();
                signgins.ForEach(x =>
                {
                    x.SigninId = Guid.NewGuid().ToString();
                    x.CreateDate = meeting.MeetingStartTime;
                });

                var endmeeting = new WorkmeetingEntity()
                {
                    MeetingId = Guid.NewGuid().ToString(),
                    MeetingType = "班后会",
                    MeetingStartTime = meeting.MeetingStartTime,
                    ShouldJoin = signgins.Count,
                    ActuallyJoin = signgins.Count(x => x.IsSigned),
                    GroupId = startmeeting.GroupId,
                    GroupName = startmeeting.GroupName,
                    OtherMeetingId = meeting.MeetingId,
                    IsOver = false,
                    PersonState = startmeeting.PersonState,
                    MeetingPerson = meeting.MeetingPerson,
                    ShouldStartTime = startmeeting.ShouldStartTime,
                    ShouldEndTime = startmeeting.ShouldEndTime,
                    Jobs = null,
                    Signins = null,
                    Files = null
                };

                signgins.ForEach(x =>
                {
                    x.MeetingId = endmeeting.MeetingId;
                });


                startmeeting.OtherMeetingId = endmeeting.MeetingId;
                startmeeting.Files = null;
                startmeeting.Signins = null;
                startmeeting.Jobs = null;

                var relations = (from q in db.IQueryable<MeetingAndJobEntity>()
                                 where q.StartMeetingId == meeting.MeetingId
                                 select q).ToList();
                relations.ForEach(x => x.EndMeetingId = endmeeting.MeetingId);
                #region 设备巡回检查
                //foreach (var item in relations)
                //{
                //    var DeviceInspectionJob = db.IQueryable<DeviceInspectionJobEntity>(x => x.MeetId == meeting.MeetingId && x.JobId == item.JobId).FirstOrDefault();
                //    if (DeviceInspectionJob != null)
                //    {
                //        var DeviceInspectionItemJob = db.IQueryable<DeviceInspectionItemJobEntity>(x => x.DeviceId == DeviceInspectionJob.Id).ToList();
                //        DeviceInspectionJob.Id = Guid.NewGuid().ToString();
                //        DeviceInspectionJob.MeetId = endmeeting.MeetingId;
                //        foreach (var itemJob in DeviceInspectionItemJob)
                //        {
                //            itemJob.Id = Guid.NewGuid().ToString();
                //            itemJob.DeviceId = DeviceInspectionJob.Id;
                //        }
                //        db.Insert(DeviceInspectionJob);
                //        db.Insert(DeviceInspectionItemJob);

                //    }
                //}

                #endregion

                db.Insert(endmeeting);
                db.Insert(signgins);
                db.Update(relations);
                db.Update(startmeeting);

                db.Commit();

                return endmeeting.MeetingId;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public WorkmeetingEntity UpdateRemark(WorkmeetingEntity meeting)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var query = from q in db.IQueryable<WorkmeetingEntity>()
                            where q.MeetingId == meeting.MeetingId
                            select q;

                var data = query.FirstOrDefault();
                data.Remark = meeting.Remark;

                data.Files = null;
                data.Jobs = null;
                data.Signins = null;

                db.Update(data);
                db.Commit();

                return data;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        /// <summary>
        /// 获取缺勤  绩效管理v2.0
        /// </summary>
        /// <param name="user"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<AttendanceTypeEntity> GetMonthAttendance2(List<PeopleEntity> user, DateTime from, DateTime to)
        {
            var db = new RepositoryFactory().BaseRepository();
            List<AttendanceTypeEntity> data = new List<AttendanceTypeEntity>();
            var userId = string.Join(",", user.Select(x => x.ID));

            //缺勤类型
            var unsigntypequery = from q1 in db.IQueryable<DataItemEntity>()
                                  join q2 in db.IQueryable<DataItemDetailEntity>() on q1.ItemId equals q2.ItemId
                                  where q1.ItemName == "缺勤原因"
                                  select q2;
            //缺勤类型
            var unsigntypedata = unsigntypequery.ToList();

            //缺勤统计
            var unsignquery = from q in db.IQueryable<UnSignRecordEntity>()
                              where userId.Contains(q.UserId) && q.UnSignDate >= @from && q.UnSignDate <= to
                              group q by q.UserId into g
                              select g;
            //缺勤统计
            foreach (var item in unsignquery)
            {
                var ReasonList = item.GroupBy(x => x.Reason).Select(g => new { Reason = g.Key, g });

                var userData = unsigntypedata.GroupJoin(ReasonList, x => x.ItemName, y => y.Reason, (x, y) => y.DefaultIfEmpty().Select(a => new AttendanceTypeEntity() { Category = x.ItemName, userId = item.Key, Times = a == null ? 0 : a.g.Count(), Hours = a == null ? 0 : a.g.Sum(z => z.Hours) })).SelectMany(x => x);
                data.AddRange(userData);
            }
            return data;

        }

        public UserAttendanceEntity GetMonthAttendance2(string userid, DateTime from, DateTime to)
        {
            var db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(userid);
            var people = db.FindEntity<PeopleEntity>(x => x.ID == userid && x.FingerMark == "yes");
            var photo = string.Empty;
            if (people != null)
            {
                photo = people.Photo;
            }
            ////人员
            //var userquery = from q in db.IQueryable<UserEntity>()
            //                where q.DepartmentId == deptid
            //                select q;

            ////排序
            //var sortquery = from q1 in db.IQueryable<UserEntity>()
            //                join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID into into1
            //                from d1 in into1.DefaultIfEmpty()
            //                where q1.DepartmentId == deptid
            //                select new { q1.UserId, q1.RealName, Sort1 = d1 == null ? "99" : d1.Planer, Sort2 = q1.CreateDate };

            //出勤
            var signquery = from q1 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                                 where ms.MeetingType == "班前会" && ss.UserId == userid && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
                                 select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
                            join q2 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                                 where ms.MeetingType == "班前会" && se.UserId == userid && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime <= to
                                 select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            select new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned };

            //出勤统计
            var signdata = signquery.ToList().GroupBy(x => x.MeetingStartTime.Date).Select(x => new { x.Key, Data = x.ToList() });

            //缺勤类型
            var unsigntypequery = from q1 in db.IQueryable<DataItemEntity>()
                                  join q2 in db.IQueryable<DataItemDetailEntity>() on q1.ItemId equals q2.ItemId
                                  where q1.ItemName == "缺勤原因"
                                  select q2;
            //缺勤类型
            var unsigntypedata = unsigntypequery.ToList();

            //缺勤统计
            var unsignquery = from q in db.IQueryable<UnSignRecordEntity>()
                              where q.UserId == userid && q.UnSignDate >= @from && q.UnSignDate <= to
                              group q by q.Reason into g
                              select new { Reason = g.Key, g };

            //缺勤统计
            var unsigndata = unsigntypedata.GroupJoin(unsignquery.ToList(), x => x.ItemName, y => y.Reason, (x, y) => y.DefaultIfEmpty().Select(a => new AttendanceTypeEntity() { Category = x.ItemName, Times = a == null ? 0 : a.g.Count(), Hours = a == null ? 0 : a.g.Sum(z => z.Hours) })).SelectMany(x => x);


            #region   排班数据统计
            // 签到
            var unsignWorkType = from q in db.IQueryable<UnSignRecordEntity>()
                                 where q.UserId == userid && q.UnSignDate >= @from && q.UnSignDate <= to && q.Reason == "值班"
                                 select new { day = q.UnSignDate.Day };

            //  var unsignWorkdata = unsignWorkType.Where(x => x.ReasonRemark == "白班" || x.ReasonRemark == "夜班").Select(x => new AttendanceTypeEntity() { Category = x.ReasonRemark, Times = x.g.Count() });
            var workSortData = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.departmentid == user.DepartmentId && x.year == @from.Year && x.month == @from.Month);
            List<string> SortList = new List<string>();
            int i = 0;
            if (workSortData != null)
            {
                var SortData = workSortData.timedata.Split(',').ToList();
                foreach (var item in SortData)
                {


                    if (!item.Contains("休息") && !item.Contains("无"))
                    {
                        var ck = unsignWorkType.Where(x => x.day == i + 1);
                        if (ck.Count() > 0)
                        {
                            var oneStr = item.Split('(')[1].Replace(")", "");
                            SortList.Add(oneStr);
                        }

                    }
                    i++;
                }
            }
            var SortGroup = SortList.GroupBy(x => x).Select(g => new { g.Key, g });
            var unsignWorkdata = SortGroup.Select(x => new AttendanceTypeEntity() { Category = x.Key, Times = x.g.Count() });
            #endregion

            ////缺勤排序整合
            //var data1 = sortquery.ToList().GroupJoin(unsigndata, x => x.UserId, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.UserId, x.RealName, Data = a == null ? new List<AttendanceTypeEntity>() : a.Data })).SelectMany(z => z).ToList();
            ////出勤排序整合
            //var data2 = sortquery.ToList().GroupJoin(signdata, x => x.UserId, y => y.UserId, (x, y) => y.DefaultIfEmpty().Select(a => new { x.UserId, x.RealName, Data = a == null ? new List<AttendanceTypeEntity>() : new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = a.SignCount } } })).SelectMany(z => z).ToList();
            //整合
            var data = new List<AttendanceTypeEntity>() { new AttendanceTypeEntity() { Category = "出勤", Times = signdata.Count(x => x.Data.Count(y => y.IsSigned1 || y.IsSigned2) > 0) } }.Concat(unsignWorkdata).Concat(unsigndata);

            return new UserAttendanceEntity() { UserId = userid, UserName = user.RealName, Photo = photo, Data = data.ToList() };
        }



        public List<DayAttendanceEntity> GetDayAttendance2(string userid, DateTime from, DateTime to)
        {
            var datalist = new List<DateTime>();
            var date = from;
            while (date < to)
            {
                datalist.Add(date);
                date = date.AddDays(1);
            }
            var db = new RepositoryFactory().BaseRepository();

            var signdata = (from q1 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                                 where ms.MeetingType == "班前会" && ss.UserId == userid && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime < to
                                 select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
                            join q2 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                                 where ms.MeetingType == "班前会" && se.UserId == userid && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime < to
                                 select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            select new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned });

            var unsigndata = (from q in db.IQueryable<UnSignRecordEntity>()
                              where q.UserId == userid && q.UnSignDate >= @from && q.UnSignDate < to && q.Reason == "值班"
                              select new { q.Reason, q.ReasonRemark, q.UnSignDate }).ToList();

            var data = datalist.GroupJoin(signdata, x => x.Date, y => y.MeetingStartTime.Date,
                (x, y) => new { x, State = y.Count(n => n.IsSigned1 || n.IsSigned2) > 0 ? "出勤" : y.Count() == 0 ? string.Empty : "缺勤" }
                ).GroupJoin(unsigndata, x => x.x, y => y.UnSignDate.Date,
                (x, y) => new DayAttendanceEntity
                {
                    Date = x.x,
                    State = x.State,
                    DayType = y.Count() > 0 ? "值班" : null,
                    ReasonRemark = string.Empty
                }).ToList();
            #region 排班
            var user = db.FindEntity<UserEntity>(userid);
            var unsignWorkType = unsigndata.Select(x => new { day = x.UnSignDate.Day });
            var workSortData = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.departmentid == user.DepartmentId && x.year == @from.Year && x.month == @from.Month);
            List<string> SortList = new List<string>();
            int i = 0;
            if (workSortData != null)
            {
                var SortData = workSortData.timedata.Split(',').ToList();
                foreach (var item in data)
                {
                    var OneDay = item.Date.Day;
                    if (SortData[OneDay - 1] != "无")
                    {
                        var ck = unsignWorkType.Where(x => x.day == i + 1);
                        if (ck.Count() > 0)
                        {
                            if (SortData[OneDay - 1] == "休息")
                            {
                                item.ReasonRemark = "休息";
                            }
                            else
                            {
                                var oneStr = SortData[OneDay - 1].Split('(')[1].Replace(")", "");
                                item.ReasonRemark = oneStr;
                            }
                        }
                    }
                    i++;
                }
            }
            #endregion


            //var queryList = query.Where(x => x.UnSignDate < to && x.ReasonRemark != null).OrderBy(x => x.StartTime).Select(x => x.ReasonRemark).Distinct();

            //return new DayAttendanceEntity() { Date = date, State = signquery.Count(y => y.IsSigned1 || y.IsSigned2) > 0 ? "出勤" : signquery.Count() == 0 ? string.Empty : "缺勤", DayType = query.Count() > 0 ? "值班" : null, ReasonRemark = queryList.Count() > 0 ? string.Join(",", queryList) : string.Empty };
            return data;
        }
        /// <summary>
        /// 获取出勤  绩效管理v2.0
        /// </summary>
        /// <param name="user"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<DayAttendanceEntity> GetDayAttendance2(List<PeopleEntity> user, DateTime from, DateTime to, string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var datalist = new List<DateTime>();
            var userId = string.Join(",", user.Select(x => x.ID));
            //var meet = from ms in db.IQueryable<WorkmeetingEntity>()
            //           where ms.GroupId == deptid
            //           select ms;
            var date = from;
            while (date < to)
            {
                datalist.Add(date);
                date = date.AddDays(1);
            }

            var day = new List<DayAttendanceEntity>();
            var signdata = (from q1 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                                 where ms.MeetingType == "班前会" && userId.Contains(ss.UserId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime < to
                                 select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
                            join q2 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                                 where ms.MeetingType == "班前会" && userId.Contains(se.UserId) && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime < to
                                 select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            select new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned });

            foreach (var item in signdata.GroupBy(x => x.UserId))
            {
                var userid = item.Key;
                var userData = item.ToList();
                var data = datalist.GroupJoin(userData, x => x.Date, y => y.MeetingStartTime.Date,
               (x, y) => new DayAttendanceEntity { Date = x.Date, State = y.Count(n => n.IsSigned1 || n.IsSigned2) > 0 ? "出勤" : y.Count() == 0 ? string.Empty : "缺勤", userid = userid }
               );
                day.AddRange(data);
            }

            return day;
        }

        public DayAttendanceEntity GetDayAttendance4(string userid, DateTime date)
        {
            var datalist = new List<DateTime>();
            var from = date;
            var to = date.AddDays(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var signdata = (from q1 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join ss in db.IQueryable<MeetingSigninEntity>() on ms.MeetingId equals ss.MeetingId
                                 where ms.MeetingType == "班前会" && ss.UserId == userid && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime < to
                                 select new { ms.MeetingId, ms.MeetingStartTime, ss.UserId, ss.IsSigned })
                            join q2 in
                                (from ms in db.IQueryable<WorkmeetingEntity>()
                                 join se in db.IQueryable<MeetingSigninEntity>() on ms.OtherMeetingId equals se.MeetingId
                                 where ms.MeetingType == "班前会" && se.UserId == userid && ms.IsOver && ms.MeetingStartTime >= @from && ms.MeetingStartTime < to
                                 select new { ms.MeetingId, ms.MeetingStartTime, se.UserId, se.IsSigned }) on new { q1.MeetingId, q1.MeetingStartTime, q1.UserId } equals new { q2.MeetingId, q2.MeetingStartTime, q2.UserId }
                            select new { q1.MeetingId, q1.MeetingStartTime, q1.UserId, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned });

            var unsigndata = (from q in db.IQueryable<UnSignRecordEntity>()
                              where q.UserId == userid && q.UnSignDate >= @from && q.UnSignDate < to && q.Reason == "值班"
                              select new { q.Reason, q.ReasonRemark, q.UnSignDate }).ToList();

            var data = datalist.GroupJoin(signdata, x => x.Date, y => y.MeetingStartTime.Date, (x, y) => new { x, State = y.Count(n => n.IsSigned1 || n.IsSigned2) > 0 ? "出勤" : y.Count() == 0 ? string.Empty : "缺勤" }).GroupJoin(unsigndata, x => x.x, y => y.UnSignDate.Date, (x, y) => new DayAttendanceEntity { Date = x.x, State = x.State, DayType = y.Count() > 0 ? "值班" : null, ReasonRemark = y.Count() > 0 ? string.Join(",", y.Select(n => n.ReasonRemark)) : string.Empty }).ToList();

            //var queryList = query.Where(x => x.UnSignDate < to && x.ReasonRemark != null).OrderBy(x => x.StartTime).Select(x => x.ReasonRemark).Distinct();

            return new DayAttendanceEntity() { Date = date, State = signdata.Count(y => y.IsSigned1 || y.IsSigned2) > 0 ? "出勤" : signdata.Count() == 0 ? string.Empty : "缺勤", DayType = unsigndata.Count() > 0 ? "值班" : null, ReasonRemark = signdata.Count() > 0 ? string.Join(",", signdata) : string.Empty };
        }

        public List<UnSignRecordEntity> GetDutyPerson(string id)
        {
            var db = new RepositoryFactory().BaseRepository();

            var entity = (from q in db.IQueryable<WorkmeetingEntity>()
                          where q.MeetingId == id
                          select q).FirstOrDefault();

            if (entity == null) return null;

            var start = entity.MeetingStartTime.Date;
            var end = entity.MeetingStartTime.Date.AddDays(1).AddMinutes(-1);
            var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                        where q2.DepartmentId == entity.GroupId && q1.Reason == "值班" && q1.UnSignDate >= start && q1.UnSignDate <= end
                        select q1;

            return query.ToList();
        }

        public void PostDutyPerson(List<WorkmeetingEntity> list)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                foreach (var item in list)
                {
                    var start = item.MeetingStartTime.Date;
                    var end = item.MeetingStartTime.Date.AddDays(1).AddMinutes(-1);
                    var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                                join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                                where q2.DepartmentId == item.GroupId && q1.UnSignDate >= start && q1.UnSignDate <= end && q1.Reason == "值班"
                                select q1;

                    var data = query.ToList();
                    db.Delete(data);
                }
                db.Insert(list.SelectMany(x => x.DutyPerson).ToList());

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        /// <summary>
        /// 添加考勤数据
        /// </summary>
        /// <param name="list"></param>
        public void PostUnSignRecord(List<UnSignRecordEntity> list)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(list);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public void PostDelDutyPerson(DateTime date, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var start = date;
                var end = date.AddDays(1).AddMinutes(-1);
                var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                            join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                            where q1.UserId == userid && q1.UnSignDate >= start && q1.UnSignDate <= end
                            select q1;
                var data = query.ToList();
                db.Delete(data);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public List<UnSignRecordEntity> GetDutyPerson(string deptid, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository();

            var start = date.Date;
            var end = date.Date.AddDays(1).AddMinutes(-1);
            var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                        join q3 in db.IQueryable<PeopleEntity>() on q2.UserId equals q3.ID into into1
                        from t1 in into1.DefaultIfEmpty()
                        where q2.DepartmentId == deptid && q1.Reason == "值班" && q1.UnSignDate >= start && q1.UnSignDate <= end
                        orderby new { t1.Planer, q1.UserName }
                        select q1;

            return query.ToList();
        }

        public void PostAttendance(WorkmeetingEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var start = model.MeetingStartTime.Date;
                var end = model.MeetingStartTime.Date.AddDays(1).AddMinutes(-1);

                var ckStr = string.Join(",", model.DutyPerson.Select(x => x.Reason));

                if (string.IsNullOrEmpty(ckStr))
                {
                    var unsigndata = (from q in db.IQueryable<UnSignRecordEntity>()
                                      where q.UserId == model.UserId && q.UnSignDate >= start && q.UnSignDate <= end
                                      select q).ToList();
                    var signdata = (from q in db.IQueryable<MeetingSigninEntity>()
                                    where q.UserId == model.UserId && q.CreateDate >= start && q.CreateDate <= end
                                    select q).ToList();
                    var signPerson = model.DutyPerson.Where(x => x.Reason != "值班");
                    db.Update(signdata);
                    foreach (var item in signdata)
                    {
                        item.IsSigned = signPerson.Count() == 0 ? true : false;
                    }
                    db.Delete(unsigndata);
                }
                if (ckStr.Contains("值班") && !string.IsNullOrEmpty(ckStr))
                {
                    //值班
                    var unsigndata = (from q in db.IQueryable<UnSignRecordEntity>()
                                      where q.UserId == model.UserId && q.UnSignDate >= start && q.UnSignDate <= end
                                      && q.Reason == "值班"
                                      select q).ToList();
                    db.Delete(unsigndata);
                    var addlist = model.DutyPerson.Where(x => x.Reason == "值班").ToList();
                    db.Insert(addlist);
                    ckStr = ckStr.Replace("值班", "");
                }

                if (!ckStr.Contains("值班") && !string.IsNullOrEmpty(ckStr))
                {
                    //缺勤
                    var unsigndata = (from q in db.IQueryable<UnSignRecordEntity>()
                                      where q.Reason != "值班" && q.UnSignDate >= start && q.UnSignDate <= end
                                      select q).ToList();

                    db.Delete(unsigndata);

                    var signdata = (from q in db.IQueryable<MeetingSigninEntity>()
                                    where q.UserId == model.UserId && q.CreateDate >= start && q.CreateDate <= end
                                    select q).ToList();
                    var signPerson = model.DutyPerson.Where(x => x.Reason != "值班");
                    foreach (var item in signdata)
                    {
                        item.IsSigned = signPerson.Count() == 0 ? true : false;
                    }
                    db.Update(signdata);
                    var addlist = model.DutyPerson.Where(x => x.Reason != "值班").ToList();
                    db.Insert(addlist);
                }


                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public List<UnSignRecordEntity> GetDayAttendance3(string userid, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository();

            var start = date.Date;
            var end = date.Date.AddDays(1).AddMinutes(-1);
            var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                        where q1.UserId == userid && q1.Reason != "值班" && q1.UnSignDate >= start && q1.UnSignDate <= end
                        select q1;

            return query.ToList();
        }
        /// <summary>
        /// 获取今日值班人
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<UserInfoEntity> GetBeOnDutyStaffInfo()
        {
            var db = new RepositoryFactory().BaseRepository();
            var date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            var query = from q1 in db.IQueryable<UserInfoEntity>()
                        join q2 in db.IQueryable<UnSignRecordEntity>() on q1.UserId equals q2.UserId
                        where q2.UnSignDate == date && q2.Reason == "值班"
                        select new { q1.UserId, q1.RealName, q2.ReasonRemark, q1.DepartmentId, q1.DeptName, q1.Mobile };

            var data = query.ToList();
            return data.Select(x => new UserInfoEntity() { UserId = x.UserId, RealName = x.RealName, DepartmentId = x.DepartmentId, DeptName = x.DeptName, Mobile = x.Mobile, Description = x.ReasonRemark }).ToList();
        }
        /// <summary>
        /// 获取班组简介
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<PeopleEntity> GetMeetAbstractInfo(string[] bzId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<PeopleEntity>().Where(x => bzId.Contains(x.BZID) && x.FingerMark == "yes").ToList();
            return query;
        }

        public WorkmeetingEntity CreateStartMeeting(string deptid, DateTime date, DateTime? start, DateTime? end, string code)
        {
            //班前会主数据
            var meeting = new WorkmeetingEntity()
            {
                MeetingId = Guid.NewGuid().ToString(),
                MeetingType = "班前会",
                MeetingStartTime = date,
                MeetingEndTime = date,
                GroupId = deptid,
                IsOver = false,
                PersonState = "正常",
                IsStarted = false,
                ShouldStartTime = start,
                ShouldEndTime = end,
                MeetingCode = code
            };
            //班前会任务
            var jobs = this.FindMeetingJobs(deptid, date);
            var jobusers = jobs.SelectMany(x => x.Relation.JobUsers).ToList();
            var relations = jobs.Select(x => x.Relation).ToList();

            //签到
            var signins = this.CreateSignins(deptid, date);
            signins.ForEach(x =>
            {
                x.MeetingId = meeting.MeetingId;
                x.CreateDate = date;
            });
            meeting.ActuallyJoin = meeting.ShouldJoin = signins.Count;

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var dept = (from q in db.IQueryable<DepartmentEntity>()
                        where q.DepartmentId == deptid
                        select q).FirstOrDefault();
            if (dept != null) meeting.GroupName = dept.FullName;

            try
            {

                var meetingperson = (from q in db.IQueryable<PeopleEntity>()
                                     where q.BZID == deptid && q.Quarters == "班长"
                                     select q).FirstOrDefault();
                if (meetingperson != null) meeting.MeetingPerson = meetingperson.Name;

                //旁站监督
                var std = new DateTime(date.Year, date.Month, date.Day);
                var etd = std.AddDays(1).AddSeconds(-1);

                //已经存在的任务未归结的任务
                var existRelations = (from q1 in db.IQueryable<MeetingAndJobEntity>()
                                      join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                                      join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into t1
                                      where q2.GroupId == meeting.GroupId && q2.StartTime >= std && q2.EndTime <= etd && q1.StartMeetingId == null
                                      select new { q1, q2, t1 }).ToList();
                var updateRelations = existRelations.Select(x => x.q1).ToList();
                foreach (var item in updateRelations)
                {
                    item.StartMeetingId = meeting.MeetingId;
                }

                relations.ForEach(x => x.StartMeetingId = meeting.MeetingId);

                //跨天任务
                var jobquery = from q in db.IQueryable<MeetingJobEntity>()
                               where q.GroupId == deptid && q.StartTime <= date && q.EndTime >= date && q.IsFinished == "undo" && q.JobType != "旁站监督"
                               select q;
                var longjobs = jobquery.ToList().Where(x => x.EndTime.Date > x.StartTime.Date).ToList();
                var lastmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                                   where q.GroupId == deptid
                                   where q.MeetingType == "班前会"
                                   orderby q.MeetingStartTime descending
                                   select q).FirstOrDefault();
                if (lastmeeting != null)
                {
                    foreach (var item in longjobs)
                    {
                        var itemusers = (from q1 in db.IQueryable<JobUserEntity>()
                                         join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                         where q2.StartMeetingId == lastmeeting.MeetingId && q2.JobId == item.JobId
                                         select q1).ToList();
                        item.Relation = new MeetingAndJobEntity()
                        {
                            MeetingJobId = Guid.NewGuid().ToString(),
                            StartMeetingId = meeting.MeetingId,
                            IsFinished = "undo",
                            JobId = item.JobId,
                            JobUserId = string.Join(",", itemusers.Select(x => x.UserId)),
                            JobUser = string.Join(",", itemusers.Select(x => x.UserName))
                        };
                        item.Relation.JobUsers = itemusers.Select(x => new JobUserEntity() { JobUserId = Guid.NewGuid().ToString(), JobType = x.JobType, MeetingJobId = item.Relation.MeetingJobId, UserId = x.UserId, UserName = x.UserName }).ToList();
                        relations.Add(item.Relation);
                        jobusers.AddRange(item.Relation.JobUsers);
                    }
                }
                //跨天任务

                //
                var planids = (from q in db.IQueryable<MeetingAndJobEntity>()
                               select q.WorkPlanContentId).ToList();

                var startdate = DateTime.Now.Date;
                var enddate = DateTime.Now.Date.AddDays(1);
                var workplan = (from q in db.IQueryable<WorkPlanContentEntity>()
                                where q.BZID == deptid && q.IsFinished == "未完成" && ((q.StartDate >= startdate && q.StartDate < enddate) || (q.EndDate >= startdate && q.EndDate < enddate))
                                select q).ToList();
                //var meetajobs = (from q in db.IQueryable<MeetingAndJobEntity>()
                //                 where 1 == 1
                //                 select q.MeetingJobId).ToList();
                //workplan = workplan.Where(x => !meetajobs.Contains(x.ID)).ToList();
                workplan = workplan.Where(x => !planids.Contains(x.ID)).ToList();
                foreach (var item in workplan)
                {
                    var job = new MeetingJobEntity()
                    {
                        JobId = Guid.NewGuid().ToString(),
                        Job = item.WorkContent,
                        StartTime = item.StartDate == null ? new DateTime(date.Year, date.Month, date.Day, 8, 30, 0) : item.StartDate.Value,
                        EndTime = item.EndDate == null ? new DateTime(date.Year, date.Month, date.Day, 17, 30, 0) : item.EndDate.Value,
                        Dangerous = "",
                        Measure = "",
                        IsFinished = "undo",
                        GroupId = deptid,
                        TemplateId = "",
                        NeedTrain = false,
                        TrainingDone = false
                    };
                    job.Relation = new MeetingAndJobEntity()
                    {
                        MeetingJobId = Guid.NewGuid().ToString(),
                        JobUserId = item.WorkPeopleId,
                        JobUser = item.WorkPeopleName,
                        IsFinished = "undo",
                        JobId = job.JobId,
                        WorkPlanContentId = item.ID,
                        StartMeetingId = meeting.MeetingId
                    };
                    relations.Add(job.Relation);
                    job.Relation.JobUsers = new List<JobUserEntity>();
                    var userarr = job.Relation.JobUserId.Split(',');
                    var namearr = job.Relation.JobUser.Split(',');
                    if (userarr != null && namearr != null)
                    {
                        for (int i = 0; i < userarr.Length; i++)
                        {
                            String jobtype = "isdoperson";
                            if (i == 0) jobtype = "ischecker";
                            var jobuser = new JobUserEntity { JobUserId = Guid.NewGuid().ToString(), UserId = userarr[i], UserName = namearr[i], JobType = jobtype, CreateDate = DateTime.Now, MeetingJobId = job.Relation.MeetingJobId };
                            jobusers.Add(jobuser);
                            job.Relation.JobUsers.Add(jobuser);
                        }
                    }
                    jobs.Add(job);
                }
                //

                db.Update(updateRelations);

                foreach (var item in existRelations)
                {
                    item.q1.JobUsers = item.t1.ToList();
                    item.q2.Relation = item.q1;
                }
                var existJobs = existRelations.Select(x => x.q2).ToList();

                db.Insert(meeting);
                db.Insert(jobs);
                foreach (var item in jobs)
                {
                    if (item.DangerousList != null)
                    {
                        db.Insert(item.DangerousList);
                        db.Insert(item.DangerousList.SelectMany(x => x.MeasureList).ToList());
                    }
                }
                db.Insert(relations);
                db.Insert(jobusers);
                db.Insert(signins);
                db.Commit();

                jobs.AddRange(existJobs);
                jobs.AddRange(longjobs);

                meeting.Jobs = jobs;
                meeting.Signins = signins;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }


            return meeting;
        }

        private List<MeetingSigninEntity> CreateSignins(string deptid, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository();
            var persons = (from q in db.IQueryable<PeopleEntity>()
                           where q.BZID == deptid && q.FingerMark == "yes"
                           select q).ToList();

            var meetingSigngins = persons.Select(x => new MeetingSigninEntity() { SigninId = Guid.NewGuid().ToString(), UserId = x.ID, PersonName = x.Name, IsSigned = true, MentalCondition = "正常", ClosingCondition = "正常", CreateDate = date }).ToList();
            return meetingSigngins;
        }

        public List<MeetingJobEntity> FindMeetingJobs(string deptid, DateTime date)
        {
            var meetingjobs = new List<MeetingJobEntity>();
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            var persons = (from q1 in db.IQueryable<PeopleEntity>()
                           join q2 in db.IQueryable<UserEntity>() on q1.ID equals q2.UserId
                           where q1.BZID == deptid && q1.FingerMark == "yes"
                           select q1).ToList();

            //取消任务结转
            //结转的任务
            //var lastmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
            //                   where q.GroupId == deptid && q.MeetingType == "班后会"
            //                   orderby q.MeetingStartTime descending
            //                   select q).FirstOrDefault();
            //if (lastmeeting != null)
            //{
            //    var undoJobs = (from q1 in db.IQueryable<MeetingJobEntity>()
            //                    join q2 in db.IQueryable<JobUserEntity>() on q1.JobId equals q2.JobId into into1
            //                    where q1.EndMeetingId == lastmeeting.MeetingId && q1.IsFinished == "undo"
            //                    select new { q1.JobId, q1.Job, q1.Dangerous, q1.Measure, q1.NeedTrain, q1.StartTime, q1.EndTime, jobusers = into1.Select(x => new { x.UserId, x.UserName, x.JobType }) }).ToList();
            //    undoJobs.ForEach(x =>
            //    {
            //        meetingjobs.Add(new MeetingJobEntity()
            //        {
            //            JobId = Guid.NewGuid().ToString(),
            //            PlanId = x.JobId,
            //            Job = x.Job,
            //            StartTime = new DateTime(date.Year, date.Month, date.Day, x.StartTime.Hour, x.StartTime.Minute, 0),
            //            EndTime = new DateTime(date.Year, date.Month, date.Day, x.StartTime.Hour, x.StartTime.Minute, 0),
            //            UserId = string.Join(",", x.jobusers.Select(y => y.UserId)),
            //            JobUsers = string.Join(",", x.jobusers.Select(y => y.UserName)),
            //            Dangerous = x.Dangerous,
            //            Measure = x.Measure,
            //            IsFinished = "undo",
            //            GroupId = deptid,
            //            CreateDate = date,
            //            NeedTrain = x.NeedTrain,
            //            Persons = x.jobusers.Select(y => new JobUserEntity()
            //            {
            //                JobUserId = Guid.NewGuid().ToString(),
            //                UserId = y.UserId,
            //                UserName = y.UserName,
            //                JobType = y.JobType
            //            }).ToList()
            //        });
            //    });
            //}
            //结转的任务

            //任务库任务

            var templates = GetWorkDateJob(deptid, date);// templatequery.ToList();

            foreach (var item in templates)
            {
                var users = new List<PeopleEntity>();
                if (!string.IsNullOrEmpty(item.JobPerson))
                {
                    //users.AddRange(persons.Where(x => item.JobPerson.Contains(x.Name)).ToList());
                    var jobpersons = item.JobPerson.Split(',');
                    foreach (var name in jobpersons)
                    {
                        var user = persons.Find(x => x.Name == name);
                        if (user != null) users.Add(user);
                    }
                }

                if (!string.IsNullOrEmpty(item.otherperson))
                {
                    var jobpersons = item.otherperson.Split(',');
                    foreach (var name in jobpersons)
                    {
                        var user = persons.Find(x => x.Name == name);
                        if (user != null) users.Add(user);
                    }

                    //var more = persons.Where(x => item.otherperson.Split(',').Contains(x.Name)).ToList();
                    //users.AddRange(more);
                }
                var job = new MeetingJobEntity()
                {
                    JobId = Guid.NewGuid().ToString(),
                    Job = item.JobContent,
                    StartTime = item.JobStartTime.HasValue ? item.JobStartTime.Value : new DateTime(date.Year, date.Month, date.Day, 8, 30, 0),
                    EndTime = item.JobEndTime.HasValue ? item.JobEndTime.Value : new DateTime(date.Year, date.Month, date.Day, 17, 30, 0),
                    Dangerous = item.Dangerous,
                    Measure = item.Measure,
                    IsFinished = "undo",
                    GroupId = deptid,
                    TemplateId = item.JobId,
                    NeedTrain = item.EnableTraining,
                    TaskType = item.TaskType,
                    RiskLevel = item.RiskLevel
                };
                job.Relation = new MeetingAndJobEntity()
                {
                    MeetingJobId = Guid.NewGuid().ToString(),
                    JobUserId = string.Join(",", users.Select(x => x.ID)),
                    JobUser = string.Join(",", users.Select(x => x.Name)),
                    IsFinished = "undo",
                    JobId = job.JobId
                };
                job.Relation.JobUsers = users.Select(x => new JobUserEntity() { JobUserId = Guid.NewGuid().ToString(), JobType = "isdoperson", MeetingJobId = job.Relation.MeetingJobId, UserId = x.ID, UserName = x.Name }).ToList();
                if (job.Relation.JobUsers.FirstOrDefault() != null)
                {
                    job.Relation.JobUsers.FirstOrDefault().JobType = "ischecker";
                }

                job.DangerousList = item.DangerousList.Select(x => new JobDangerousEntity()
                {
                    Content = x.Content,
                    CreateTime = DateTime.Now,
                    DangerousId = x.DangerousId,
                    JobDangerousId = Guid.NewGuid().ToString(),
                    JobId = job.JobId,
                    MeasureList = x.MeasureList.Select(y => new JobMeasureEntity()
                    {
                        Content = y.Content,
                        CreateTime = DateTime.Now,
                        JobMeasureId = Guid.NewGuid().ToString(),
                        MeasureId = y.MeasureId
                    }).ToList()
                }).ToList();
                foreach (var item1 in job.DangerousList)
                {
                    foreach (var item2 in item1.MeasureList)
                    {
                        item2.JobDangerousId = item1.JobDangerousId;
                    }
                }

                meetingjobs.Add(job);
            }

            //部门任务
            var taskquery = from q in db.IQueryable<DepartmentTaskEntity>()
                            where q.DutyDepartmentId == deptid && q.IsPublish == false && q.StartDate <= date.Date && q.EndDate >= date.Date
                            select q;
            var tasks = taskquery.ToList();
            tasks.ForEach(x =>
            {
                var job = new MeetingJobEntity()
                {
                    JobId = Guid.NewGuid().ToString(),
                    Job = x.Content,
                    StartTime = new DateTime(date.Year, date.Month, date.Day, 8, 30, 0),
                    EndTime = new DateTime(date.Year, date.Month, date.Day, 17, 30, 0),
                    GroupId = deptid,
                    IsFinished = "undo",
                    TemplateId = x.TaskId,
                    TaskType = "日常工作",
                    RiskLevel = "低风险"
                };
                job.Relation = new MeetingAndJobEntity()
                {
                    MeetingJobId = Guid.NewGuid().ToString(),
                    JobUserId = x.DutyUserId,
                    JobUser = x.DutyUser,
                    IsFinished = "undo",
                    JobId = job.JobId
                };
                job.Relation.JobUsers = new List<JobUserEntity>();
                job.Relation.JobUsers.Add(new JobUserEntity()
                {
                    JobUserId = Guid.NewGuid().ToString(),
                    JobType = "ischecker",
                    MeetingJobId = job.Relation.MeetingJobId,
                    UserId = x.DutyUserId,
                    UserName = x.DutyUser
                });
                job.DangerousList = new List<JobDangerousEntity>();
                meetingjobs.Add(job);
            });
            //部门任务

            meetingjobs.RemoveAll(x => x.StartTime.Date != date.Date);

            var existjobs = this.FindMeetingJobs2(deptid, date);
            meetingjobs.RemoveAll(x => existjobs.Any(y => y.TemplateId == x.TemplateId));

            //已经存在的任务
            var ddd = (from q1 in db.IQueryable<MeetingJobEntity>()
                       join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                       where q1.GroupId == deptid && q1.StartTime > date && q2.StartMeetingId == null
                       select q1).ToList();
            meetingjobs.RemoveAll(x => ddd.Any(y => y.TemplateId == x.TemplateId));

            try
            {
                db.Insert(meetingjobs);
                db.Insert(meetingjobs.Select(x => x.Relation).ToList());
                db.Insert(meetingjobs.SelectMany(x => x.Relation.JobUsers).ToList());
                db.Insert(meetingjobs.SelectMany(x => x.DangerousList).ToList());
                db.Insert(meetingjobs.SelectMany(x => x.DangerousList.SelectMany(y => y.MeasureList)).ToList());
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

            meetingjobs.AddRange(existjobs);


            //任务库任务

            //20190409_xjl 查询任务计划中的子任务
            //var startdate = DateTime.Now.Date;
            //var enddate = DateTime.Now.Date.AddDays(1);
            //var workplan = (from q in db.IQueryable<WorkPlanContentEntity>()
            //                where q.BZID == deptid && q.IsFinished == "未完成" && ((q.StartDate >= startdate && q.StartDate < enddate) || (q.EndDate >= startdate && q.EndDate < enddate))
            //                select q).ToList();
            //foreach (var item in workplan) 
            //{
            //    var job = new MeetingJobEntity()
            //    {
            //        JobId = Guid.NewGuid().ToString(),
            //        Job = item.WorkContent,
            //        StartTime = item.StartDate == null ? new DateTime(date.Year, date.Month, date.Day, 8, 30, 0) : new DateTime(date.Year, date.Month, date.Day, item.StartDate.Value.Hour, item.StartDate.Value.Minute, 0),
            //        EndTime = item.EndDate == null ? new DateTime(date.Year, date.Month, date.Day, 17, 30, 0) : new DateTime(date.Year, date.Month, date.Day, item.EndDate.Value.Hour, item.EndDate.Value.Minute, 0),
            //        Dangerous = "",
            //        Measure = "",
            //        IsFinished = "undo",
            //        GroupId = deptid,
            //        TemplateId = "",
            //        NeedTrain = false
            //    };
            //    job.Relation = new MeetingAndJobEntity()
            //    {
            //        MeetingJobId = Guid.NewGuid().ToString(),
            //        JobUserId = item.WorkPeopleId,
            //        JobUser = item.WorkPeopleName,
            //        IsFinished = "undo",
            //        JobId = job.JobId,
            //        WortkPlanContentId = item.ID
            //    };
            //    job.Relation.JobUsers = new List<JobUserEntity>();
            //    meetingjobs.Add(job);
            //}
            return meetingjobs;
        }

        public WorkmeetingEntity StartMeeting(WorkmeetingEntity meeting, DateTime date, string meetingperson)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var settings = this.GetDefaultSigns(meeting.GroupId);
                var signins = this.CreateSignins(meeting.GroupId, meeting.MeetingStartTime.Date);
                foreach (var item in signins)
                {
                    var setting = settings.Find(x => x.UserId == item.UserId);
                    if (setting != null)
                    {
                        item.IsSigned = setting.IsSigned;
                        item.Reason = setting.Reason;
                        item.ReasonRemark = setting.ReasonRemark;
                        item.MeetingId = meeting.MeetingId;
                    }
                }

                meeting.ShouldJoin = signins.Count;
                meeting.ActuallyJoin = signins.Count(x => x.IsSigned == true);
                meeting.MeetingPerson = meetingperson;

                foreach (var item in meeting.Jobs)
                {
                    var relation = db.FindEntity<MeetingAndJobEntity>(item.Relation.MeetingJobId);
                    relation.StartMeetingId = meeting.MeetingId;
                    db.Update(relation);
                }
                if (meeting.Files == null) meeting.Files = new List<FileInfoEntity>();
                foreach (var item in meeting.Files)
                {
                    item.RecId = meeting.MeetingId;
                }

                db.Insert(meeting);
                db.Insert(signins);
                db.Insert(meeting.Files);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

            return meeting;
        }


        public string PostMonitorJob(MeetingJobEntity job)
        {
            var date = job.StartTime.Date;
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                var meetingquery = from q in db.IQueryable<WorkmeetingEntity>()
                                   where q.GroupId == job.GroupId
                                   orderby q.MeetingStartTime descending
                                   select q;

                var meeting = meetingquery.FirstOrDefault();
                if (meeting == null)
                {

                }
                else
                {
                    if (meeting.MeetingType == "班前会")
                    {
                        job.Relation = new MeetingAndJobEntity()
                        {
                            JobId = job.JobId,
                            MeetingJobId = Guid.NewGuid().ToString(),
                            IsFinished = "undo"
                        };

                        if (!meeting.IsStarted) job.Relation.StartMeetingId = meeting.MeetingId;
                    }
                    else
                    {
                        //if (meeting.IsOver == false)
                        //{

                        //    job.Relation = new MeetingAndJobEntity()
                        //    {
                        //        JobId = job.JobId,
                        //        MeetingJobId = Guid.NewGuid().ToString(),
                        //        IsFinished = "undo"
                        //    };
                        //}
                    }
                }

                db.Insert(job);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

            return job.JobId;
        }

        public void FinishMonitorJob(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                var job = (from q in db.IQueryable<MeetingJobEntity>()
                           where q.JobId == id
                           select q).FirstOrDefault();

                job.IsFinished = "undo";

                db.Update(job);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void NoticeMonitor(string meetingid, string userid, string state)
        {
            var db = new RepositoryFactory().BaseRepository();

            var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                           where q.MeetingId == meetingid
                           select q).FirstOrDefault();

            var startmeetingid = meetingid;
            if (meeting.MeetingType == "班后会")
                startmeetingid = meeting.OtherMeetingId;

            var monitorjob = (from q1 in db.IQueryable<MeetingJobEntity>()
                              join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                              where q2.StartMeetingId == startmeetingid && q1.JobType == "旁站监督"
                              select new { q1, q2 }).ToList();

            if (monitorjob.Count > 0)
            {
                SkSystem sync = new SkSystem();

                foreach (var item in monitorjob)
                {
                    item.q2.JobUsers = (from q in db.IQueryable<JobUserEntity>()
                                        where q.MeetingJobId == item.q2.MeetingJobId
                                        select q).ToList();
                    item.q1.Relation = item.q2;
                    sync.SyncTask(item.q1, userid, state);
                }
            }
        }

        public void UpdateMonitorJob(MeetingJobEntity model)
        {
            var db = new RepositoryFactory().BaseRepository();

            try
            {
                var job = (from q in db.IQueryable<MeetingJobEntity>()
                           where q.RecId == model.RecId
                           select q).FirstOrDefault();

                job.Result = job.Result;
                job.TimeLength = job.TimeLength;
                job.ImageList = job.ImageList;

                db.Update(job);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }


        public List<MeetingJobEntity> GetJobHistory(MeetingJobEntity meetingJobEntity)
        {
            var db = new RepositoryFactory().BaseRepository();

            var job = (from q in db.IQueryable<MeetingJobEntity>()
                       where q.JobId == meetingJobEntity.JobId
                       select q).ToList();

            var relations = (from q1 in db.IQueryable<MeetingAndJobEntity>()
                             join q2 in db.IQueryable<WorkmeetingEntity>() on q1.StartMeetingId equals q2.MeetingId
                             join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into into1
                             join q4 in db.IQueryable<FileInfoEntity>() on q1.MeetingJobId equals q4.RecId into into2
                             where q1.JobId == meetingJobEntity.JobId
                             orderby q2.MeetingStartTime
                             select new { q2.MeetingStartTime, into2, q1.JobId, q1.MeetingJobId, into1 }).ToList();

            var data = from q1 in job
                       join q2 in relations on q1.JobId equals q2.JobId
                       select new MeetingJobEntity()
                       {
                           JobId = q1.JobId,
                           Job = q1.Job,
                           StartTime = q2.MeetingStartTime,
                           Relation = new MeetingAndJobEntity() { JobUsers = q2.into1.ToList(), MeetingJobId = q2.MeetingJobId },
                           Files = q2.into2.ToList()
                       };

            return data.ToList();
        }


        public MeetingJobEntity GetJobDetail(string meetingjobid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                            where q.MeetingJobId == meetingjobid
                            select q).FirstOrDefault();

            var entity = (from q in db.IQueryable<MeetingJobEntity>()
                          where q.JobId == relation.JobId
                          select q).FirstOrDefault();

            entity.Relation = relation;
            entity.Relation.JobUsers = (from q in db.IQueryable<JobUserEntity>()
                                        where q.MeetingJobId == meetingjobid
                                        select q).ToList();

            entity.Files = (from q in db.IQueryable<FileInfoEntity>()
                            where q.RecId == meetingjobid
                            select q).ToList();

            return entity;
        }

        public void AddTempJobs(string meetingid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                               where q.MeetingId == meetingid
                               select q).FirstOrDefault();

                if (meeting.MeetingType != "班前会") return;

                var jobquery = from q1 in db.IQueryable<MeetingJobEntity>()
                               join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                               where string.IsNullOrEmpty(q1.TemplateId) && q2.StartMeetingId == meetingid && q1.JobType != "旁站监督"
                               select q1;
                var jobs = jobquery.ToList();
                var tempjobs = jobs.Select(x => new JobTemplateEntity() { JobId = Guid.NewGuid().ToString(), JobContent = x.Job, DeptId = x.GroupId, DangerType = "tmp" }).ToList();
                db.Insert(tempjobs);
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public WorkmeetingEntity GetEntity(string startMeetingId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var entity = (from q in db.IQueryable<WorkmeetingEntity>()
                          where q.MeetingId == startMeetingId
                          select q).FirstOrDefault();
            return entity;
        }

        public void ReBuildMeeting(string meetingId)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                               where q.MeetingId == meetingId
                               select q).FirstOrDefault();

                if (meeting == null || meeting.MeetingType == "班后会" || meeting.IsStarted) return;



                var templates = GetWorkDateJob(meeting.GroupId, DateTime.Now);
                var exijobs = (from q1 in db.IQueryable<MeetingJobEntity>()
                               join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                               where q2.StartMeetingId == meetingId
                               select q1).ToList();
                var users = (from q in db.IQueryable<PeopleEntity>()
                             where q.BZID == meeting.GroupId && q.FingerMark == "yes"
                             select q).ToList();
                var newjobs = templates.Where(x => exijobs.All(y => y.TemplateId != x.JobId)).ToList();
                var jobs = new List<MeetingJobEntity>();
                var meetingandjobs = new List<MeetingAndJobEntity>();
                foreach (var item in newjobs)
                {
                    var job = new MeetingJobEntity()
                    {
                        JobId = Guid.NewGuid().ToString(),
                        Job = item.JobContent,
                        StartTime = item.JobStartTime.HasValue ? item.JobStartTime.Value : new DateTime(meeting.MeetingStartTime.Year, meeting.MeetingStartTime.Month, meeting.MeetingStartTime.Day, 8, 30, 0),
                        EndTime = item.JobEndTime.HasValue ? item.JobEndTime.Value : new DateTime(meeting.MeetingStartTime.Year, meeting.MeetingStartTime.Month, meeting.MeetingStartTime.Day, 17, 30, 0),
                        Dangerous = item.Dangerous,
                        Measure = item.Measure,
                        IsFinished = "undo",
                        GroupId = meeting.GroupId,
                        TemplateId = item.JobId,
                        CreateDate = DateTime.Now,
                        NeedTrain = item.EnableTraining,
                    };
                    job.DangerousList = item.DangerousList.Select(x => new JobDangerousEntity()
                    {
                        Content = x.Content,
                        CreateTime = DateTime.Now,
                        DangerousId = x.DangerousId,
                        JobDangerousId = Guid.NewGuid().ToString(),
                        JobId = job.JobId,
                        MeasureList = x.MeasureList.Select(y => new JobMeasureEntity()
                        {
                            Content = y.Content,
                            CreateTime = DateTime.Now,
                            JobMeasureId = Guid.NewGuid().ToString(),
                            MeasureId = y.MeasureId
                        }).ToList()
                    }).ToList();
                    foreach (var item1 in job.DangerousList)
                    {
                        foreach (var item2 in item1.MeasureList)
                        {
                            item2.JobDangerousId = item1.JobDangerousId;
                        }
                    }
                    if (job.DangerousList != null)
                    {
                        db.Insert(job.DangerousList);
                        db.Insert(job.DangerousList.SelectMany(x => x.MeasureList).ToList());
                    }
                    jobs.Add(job);
                    var meetingandjob = new MeetingAndJobEntity()
                    {
                        MeetingJobId = Guid.NewGuid().ToString(),
                        StartMeetingId = meeting.MeetingId,
                        JobId = job.JobId,
                        IsFinished = "undo"
                    };
                    meetingandjobs.Add(meetingandjob);
                    meetingandjob.JobUsers = new List<JobUserEntity>();
                    if (!string.IsNullOrEmpty(item.JobPerson))
                    {

                        var usernames = item.JobPerson.Split(',');
                        var jobusers = users.Where(x => usernames.Any(y => y == x.Name)).ToList();
                        for (int i = 0; i < jobusers.Count; i++)
                        {
                            var jobuser = new JobUserEntity()
                            {
                                JobUserId = Guid.NewGuid().ToString(),
                                UserId = jobusers[i].ID,
                                UserName = jobusers[i].Name,
                                CreateDate = DateTime.Now,
                                MeetingJobId = meetingandjob.MeetingJobId,
                            };
                            if (i == 0) jobuser.JobType = "ischecker";
                            else jobuser.JobType = "isdoperson";
                            meetingandjob.JobUsers.Add(jobuser);
                        }
                        meetingandjob.JobUserId = string.Join(",", jobusers.Select(x => x.ID));
                        meetingandjob.JobUser = string.Join(",", jobusers.Select(x => x.Name));
                    }
                }

                // 20190409_xjl 查询任务计划中的子任务
                var date = DateTime.Now;
                var planids = (from q in db.IQueryable<MeetingAndJobEntity>()
                               select q.WorkPlanContentId).ToList();

                var startdate = DateTime.Now.Date;
                var enddate = DateTime.Now.Date.AddDays(1);
                var workplan = (from q in db.IQueryable<WorkPlanContentEntity>()
                                where q.BZID == meeting.GroupId && q.IsFinished == "未完成" && ((q.StartDate >= startdate && q.StartDate < enddate) || (q.EndDate >= startdate && q.EndDate < enddate))
                                select q).ToList();
                //var meetajobs = (from q in db.IQueryable<MeetingAndJobEntity>()
                //                 where 1 == 1
                //                 select q.MeetingJobId).ToList();
                //workplan = workplan.Where(x => !meetajobs.Contains(x.ID)).ToList();
                workplan = workplan.Where(x => !planids.Contains(x.ID)).ToList();
                foreach (var item in workplan)
                {
                    var job = new MeetingJobEntity()
                    {
                        JobId = Guid.NewGuid().ToString(),
                        Job = item.WorkContent,
                        StartTime = item.StartDate == null ? new DateTime(date.Year, date.Month, date.Day, 8, 30, 0) : item.StartDate.Value,
                        EndTime = item.EndDate == null ? new DateTime(date.Year, date.Month, date.Day, 17, 30, 0) : item.EndDate.Value,
                        Dangerous = "",
                        Measure = "",
                        IsFinished = "undo",
                        GroupId = meeting.GroupId,
                        TemplateId = "",
                        NeedTrain = false
                    };

                    job.Relation = new MeetingAndJobEntity()
                    {
                        MeetingJobId = Guid.NewGuid().ToString(),
                        JobUserId = item.WorkPeopleId,
                        JobUser = item.WorkPeopleName,
                        IsFinished = "undo",
                        JobId = job.JobId,
                        WorkPlanContentId = item.ID,
                        StartMeetingId = meeting.MeetingId
                    };

                    job.Relation.JobUsers = new List<JobUserEntity>();
                    var userarr = job.Relation.JobUserId.Split(',');
                    var namearr = job.Relation.JobUser.Split(',');
                    if (userarr != null && namearr != null)
                    {
                        for (int i = 0; i < userarr.Length; i++)
                        {
                            String jobtype = "isdoperson";
                            if (i == 0) jobtype = "ischecker";
                            var jobuser = new JobUserEntity { JobUserId = Guid.NewGuid().ToString(), UserId = userarr[i], UserName = namearr[i], JobType = jobtype, CreateDate = DateTime.Now, MeetingJobId = job.Relation.MeetingJobId };
                            //jobusers.Add(jobuser);
                            job.Relation.JobUsers.Add(jobuser);
                        }
                    }
                    meetingandjobs.Add(job.Relation);
                    //jobs.Add(job);
                    jobs.Add(job);
                }
                var alljobusers = meetingandjobs.SelectMany(x => x.JobUsers).ToList();

                db.Insert(meetingandjobs);
                db.Insert(jobs);
                db.Insert(alljobusers);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<KeyTimesEntity> GetUserMeetingTimes(string deptid, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in
                            (from q1 in db.IQueryable<MeetingSigninEntity>()
                             join q2 in db.IQueryable<WorkmeetingEntity>() on q1.MeetingId equals q2.MeetingId
                             where q2.MeetingType == "班前会" && q2.GroupId == deptid && q2.MeetingStartTime >= start && q2.MeetingEndTime < end
                             select new { q2.MeetingId, q1.UserId, q1.IsSigned, q2.MeetingStartTime })
                        join q2 in
                            (from q1 in db.IQueryable<MeetingSigninEntity>()
                             join q2 in db.IQueryable<WorkmeetingEntity>() on q1.MeetingId equals q2.OtherMeetingId
                             where q2.MeetingType == "班前会" && q2.GroupId == deptid && q2.MeetingStartTime >= start && q2.MeetingEndTime < end
                             select new { q2.MeetingId, q1.UserId, q1.IsSigned, q2.MeetingStartTime }) on new { q1.MeetingId, q1.UserId, q1.MeetingStartTime } equals new { q2.MeetingId, q2.UserId, q2.MeetingStartTime }
                        select new { q1.MeetingId, q1.UserId, q2.MeetingStartTime, IsSigned1 = q1.IsSigned, IsSigned2 = q2.IsSigned };
            var basedata = query.ToList();
            return basedata.GroupBy(x => x.UserId).Select(x => new KeyTimesEntity() { UserId = x.Key, Times = x.GroupBy(y => y.MeetingStartTime.Date, y => new { Times = y.IsSigned1 && y.IsSigned2 ? 1 : y.IsSigned1 || y.IsSigned2 ? 0.5 : 0 }).Sum(y => y.Any(z => z.Times == 1) ? 1 : y.All(z => z.Times == 0) ? 0 : 0.5) }).ToList();


            //.GroupBy(x => x.MeetingStartTime.Date, x => new { Times = x.IsSigned1 && x.IsSigned2 ? 1 : x.IsSigned1 || x.IsSigned2 ? 0.5 : 0 }).Sum(x => x.Any(y => y.Times == 1) ? 1 : x.All(y => y.Times == 0) ? 0 : 0.5);
        }

        public List<KeyTimesEntity> GetUserDutyTimes(string deptid, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                        where q2.DepartmentId == deptid && q1.Reason == "值班" && q1.UnSignDate >= start && q1.UnSignDate <= end
                        group new { q1.UserId, q1.UnSignRecordId } by q1.UserId into g
                        select new { g.Key, Times = g.Count() };

            return query.ToList().Select(x => new KeyTimesEntity() { UserId = x.Key, Times = x.Times }).ToList();
        }

        public List<KeyTimesEntity> GetUserJobs(string deptid, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                        where q1.GroupId == deptid && q2.IsFinished == "finish" && q1.EndTime >= start && q1.StartTime <= end && !string.IsNullOrEmpty(q2.StartMeetingId)
                        group new { q1.JobId, q3.UserId } by q3.UserId into g
                        select new { g.Key, Times = g.Count() };

            return query.ToList().Select(x => new KeyTimesEntity() { UserId = x.Key, Times = x.Times }).ToList();
        }
        public List<KeyTimesEntity> GetUserMonthTaskHour(string deptid, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                        where q1.GroupId == deptid && q2.IsFinished != "cancel" && q1.EndTime >= start && q1.StartTime <= end && !string.IsNullOrEmpty(q2.StartMeetingId)
                        group new { q3.UserId, q3.TaskHour } by q3.UserId into g
                        select new { g.Key, TaskHour = g.Sum(x => x.TaskHour ?? 0) };
            return query.ToList().Select(x => new KeyTimesEntity() { UserId = x.Key, Times = Convert.ToDouble(x.TaskHour) }).ToList();
        }

        public List<KeyTimesEntity> GetUserMonthScore(string deptid, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                        where q1.GroupId == deptid && q2.IsFinished != "cancel" && q1.EndTime >= start && q1.StartTime <= end && !string.IsNullOrEmpty(q2.StartMeetingId)
                        group new { q3.UserId, q3.Score } by q3.UserId into g
                        select new { g.Key, Score = g.Sum(x => x.Score ?? 0) };
            return query.ToList().Select(x => new KeyTimesEntity() { UserId = x.Key, Times = x.Score }).ToList();
        }

        public void UpdateScoreState(string meetingJobId, string userid, string score, string isFinished)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity1 = db.FindEntity<MeetingAndJobEntity>(meetingJobId);
                if (entity1 != null)
                {
                    entity1.IsFinished = isFinished;
                    db.Update(entity1);
                }

                var entity2 = (from q in db.IQueryable<JobUserEntity>()
                               where q.MeetingJobId == meetingJobId && q.UserId == userid
                               select q).FirstOrDefault();
                if (entity2 != null)
                {
                    entity2.Score = int.Parse(score);
                    db.Update(entity2);
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public List<UnSignRecordEntity> GetDutyData(string deptid, DateTime from, DateTime to)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UnSignRecordEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.UserId equals q2.UserId
                        join q3 in db.IQueryable<PeopleEntity>() on q2.UserId equals q3.ID into into1
                        from t1 in into1.DefaultIfEmpty()
                        where q2.DepartmentId == deptid && q1.Reason == "值班" && q1.UnSignDate >= @from && q1.UnSignDate < to
                        orderby new { t1.Planer, q1.UserName }
                        select q1;

            return query.ToList();
        }

        public int GetMeetingNumber(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = @from.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId
                        where q1.MeetingType == "班前会" && q1.GroupId == deptid && q1.IsOver && q2.IsOver && q1.MeetingStartTime >= @from && q1.MeetingStartTime <= to
                        select q1;

            return query.Count();
        }

        public int GetTaskNumber(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = @from.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        where q1.GroupId == deptid && q1.IsFinished == "finish" && q1.StartTime >= @from && q1.StartTime <= to
                        select q1;

            return query.Count();
        }

        public decimal GetTaskPct(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = @from.AddMonths(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        where q1.GroupId == deptid && q1.StartTime >= @from && q1.StartTime <= to && q1.IsFinished != "cancel"
                        select q1;
            var n1 = query.Count();
            var n2 = query.Count(x => x.IsFinished == "finish");
            if (n1 == 0) n1 = 1;

            return (decimal)n2 / n1;
        }

        public void UpdateState(string meetingjobid, string state, string trainingtype)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var relation = (from q in db.IQueryable<MeetingAndJobEntity>()
                                where q.MeetingJobId == meetingjobid
                                select q).FirstOrDefault();
                if (relation != null)
                {
                    if (trainingtype == "人身风险预控")
                    {
                        var setting = new DataItemDetailService().GetDetail("人身风险预控", "人身风险预控关联工作任务状态");
                        if (setting != null && setting.ItemValue == "是")
                        {
                            var training = (from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                                            join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                                            where q2.MeetingJobId == relation.MeetingJobId && (q1.IsDone == false || q1.IsMarked == false)
                                            select q1).Count();
                            if (training > 0) throw new Exception("人身风险预控未完成");
                        }
                    }
                    else
                    {
                        var setting = new DataItemDetailService().GetDetail("系统管理", "危险预知训练关联工作任务状态");
                        if (setting != null && setting.ItemValue == "是")
                        {
                            var training = (from q in db.IQueryable<DangerEntity>()
                                            where q.JobId == relation.MeetingJobId
                                            select q).FirstOrDefault();
                            if (training != null && training.Status != 2) throw new Exception("危险预知训练未完成");
                        }
                    }


                    relation.IsFinished = state;

                    var job = (from q in db.IQueryable<MeetingJobEntity>()
                               where q.JobId == relation.JobId
                               select q).FirstOrDefault();
                    if (job != null) job.IsFinished = state;

                    var relations = (from q in db.IQueryable<MeetingAndJobEntity>()
                                     where q.JobId == job.JobId
                                     select q).ToList();
                    foreach (var item in relations)
                    {
                        item.IsFinished = state;
                    }

                    db.Update(job);
                    db.Update(relations);

                    //部门任务
                    if (relation.IsFinished == "finish" && !string.IsNullOrEmpty(job.TemplateId))
                    {
                        var tasks = (from q in db.IQueryable<DepartmentTaskEntity>()
                                     where q.TaskId == job.TemplateId && q.Status != "已取消"
                                     select q).ToList();
                        tasks.ForEach(x => x.Status = "已完成");
                        db.Update(tasks);
                    }
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public bool ExistJob(JobTemplateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<JobTemplateEntity>()
                        where q.JobContent == entity.JobContent
                        select q;
            return query.Count() > 1;
        }

        public List<MeetingSigninEntity> GetDefaultSigns(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from user in db.IQueryable<PeopleEntity>()
                        join sign in db.IQueryable<UserSignSettingEntity>() on user.ID equals sign.UserId into t1
                        where user.BZID == deptid && user.FingerMark == "yes"
                        from sign in t1.DefaultIfEmpty()
                        orderby user.Planer, user.EntryDate
                        select new { user.ID, user.Name, IsSigned = sign == null ? true : sign.IsSigned, Reason = sign == null ? null : sign.Reason, ReasonRemark = sign == null ? null : sign.ReasonRemark };
            var data = query.ToList();
            return data.Select(x => new MeetingSigninEntity() { UserId = x.ID, PersonName = x.Name, IsSigned = x.IsSigned, Reason = x.Reason, ReasonRemark = x.ReasonRemark }).ToList();
        }

        public void SetDefaultSigns(List<MeetingSigninEntity> list)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var userids = list.Select(x => x.UserId);
                var query = from q in db.IQueryable<UserSignSettingEntity>()
                            where userids.Contains(q.UserId)
                            select q;

                var data = query.ToList();
                foreach (var item in list)
                {
                    var ss = data.Find(x => x.UserId == item.UserId);
                    if (ss == null) db.Insert(new UserSignSettingEntity() { UserId = item.UserId, IsSigned = item.IsSigned, Reason = item.Reason, ReasonRemark = item.ReasonRemark });
                    else
                    {
                        ss.IsSigned = item.IsSigned;
                        ss.Reason = string.IsNullOrEmpty(item.Reason) ? string.Empty : item.Reason;
                        ss.ReasonRemark = item.ReasonRemark;
                        db.Update(ss);
                    }
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        /// <summary>
        ///获取班会台账和评价
        /// </summary>
        /// <param name="pagination">分页公用类</param>
        /// <param name="queryJson">Depts部门id集合|startTime开始时间|endTime结束时间</param>
        /// <returns></returns>
        public List<MeetingBookEntity> GetMeetPage(Pagination pagination, string queryJson)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId into tb1
                        from s1 in tb1.DefaultIfEmpty()
                        join q3 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q3.DepartmentId into tb2
                        from s2 in tb2.DefaultIfEmpty()
                        join q4 in db.IQueryable<ActivityEvaluateEntity>() on q1.OtherMeetingId equals q4.Activityid into t3
                        from s3 in tb1.DefaultIfEmpty()
                        where q1.MeetingType == "班前会"
                        orderby q1.MeetingStartTime descending
                        select new MeetingBookEntity() { MeetingId = q1.MeetingId, TopStartTime = q1.MeetingStartTime, TopEndTime = q1.MeetingEndTime, AfterStartTime = s1.MeetingStartTime, AfterEndTime = s1.MeetingEndTime, DeptName = s2.FullName, DeptId = q1.GroupId, Evaluates = t3 };


            var queryParam = queryJson.ToJObject();
            // Depts部门id集合
            if (!queryParam["Depts"].IsEmpty())
            {
                string Depts = queryParam["Depts"].ToString();
                query = query.Where(p => Depts.Contains(p.DeptId));

            }
            //startTime开始时间
            if (!queryParam["startTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["startTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime startTime = time;
                query = query.Where(p => p.TopStartTime >= startTime);
            }
            //endTime结束时间
            if (!queryParam["endTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["endTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime endTime = time;
                query = query.Where(p => p.TopStartTime <= endTime);
            }

            int total = query.Count();
            pagination.records = total;
            var data = query.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            return data;
            // List<MeetingBookEntity> result = new List<MeetingBookEntity>();
            //data.ForEach(x => result.Add(new MeetingBookEntity
            //{
            //    MeetingId = x.MeetingId,
            //    TopStartTime = x.StartTime1,
            //    TopEndTime = x.EndTime1,
            //    AfterStartTime = x.StartTime2,
            //    AfterEndTime = x.EndTime2,
            //    DeptName = x.Team,
            //    DeptId = x.GroupId,
            //    Evaluates = x.q4.ToList()

            //}));
            // return result;
            //return data.Select(x => new MeetingBookEntity() { MeetingId = x.MeetingId, TopStartTime = x.StartTime1, TopEndTime = x.EndTime1, AfterStartTime = x.StartTime2, AfterEndTime = x.EndTime2, DeptName = x.Team, DeptId = x.GroupId, Evaluates = x.q4 }).ToList();


        }


        public List<MeetingEntity> GetMeetingList(string[] ary_deptid, string userid, bool? isEvaluate, DateTime? from, DateTime? to, int pagesize, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId
                        join q3 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q3.DepartmentId
                        join q4 in db.IQueryable<ActivityEvaluateEntity>() on q1.OtherMeetingId equals q4.Activityid into t4
                        where q1.MeetingType == "班前会"
                        select new { q1.MeetingId, StartTime1 = q1.MeetingStartTime, EndTime1 = q1.MeetingEndTime, StartTime2 = q2.MeetingStartTime, EndTime2 = q2.MeetingEndTime, Team = q3.FullName, q1.GroupId, q4 = t4 };
            if (ary_deptid != null) query = query.Where(x => ary_deptid.Contains(x.GroupId));
            if (from != null) query = query.Where(x => x.StartTime1 >= from);
            if (to != null) query = query.Where(x => x.StartTime1 <= to);
            if (isEvaluate.HasValue)
            {
                if (isEvaluate.Value)
                    query = query.Where(x => x.q4.Count(y => y.EvaluateId == userid) > 0);
                else
                    query = query.Where(x => x.q4.Count(y => y.EvaluateId == userid) == 0);
            }

            total = query.Count();
            var data = query.OrderByDescending(x => x.StartTime1).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            return data.Select(x => new MeetingEntity() { MeetingId = x.MeetingId, StartTime1 = x.StartTime1, EntTime1 = x.EndTime1, StartTime2 = x.StartTime2, EndTime2 = x.EndTime2, Team = x.Team, IsEvaluated = x.q4.Count(y => y.EvaluateId == userid) > 0 }).ToList();
        }

        public List<Meeting2Entity> GetData2(string[] deptid, int pagesize, int pageindex, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DepartmentEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.DepartmentId equals q2.GroupId
                        join q3 in db.IQueryable<WorkmeetingEntity>() on q2.MeetingId equals q3.OtherMeetingId
                        join q4 in db.IQueryable<FileInfoEntity>() on q2.MeetingId equals q4.RecId into into4
                        join q5 in db.IQueryable<FileInfoEntity>() on q2.OtherMeetingId equals q5.RecId into into5
                        //join q6 in (from q1 in db.IQueryable<MeetingAndJobEntity>()
                        //            join q2 in db.IQueryable<FileInfoEntity>() on q1.MeetingJobId equals q2.RecId into into2
                        //            select new { q1.MeetingJobId, q1.StartMeetingId, q2 = into2.Select(x => new { x.FileId, x.Description, x.FilePath }) }) on q2.MeetingId equals q6.StartMeetingId into into6
                        where deptid.Contains(q1.DepartmentId) && q2.MeetingType == "班前会" && q2.IsOver == true && q3.IsOver == true
                        select new { q2.MeetingId, q2.OtherMeetingId, q2.MeetingStartTime, q2.GroupId, q1.FullName, q4 = into4, q5 = into5 };

            //select new { q1.MeetingId, q1.GroupId, q1.GroupName, q1.MeetingStartTime, Pic1 = into3.Count(x => x.Description == "照片"), Job1 = into5.Count(), Job2 = into5.Count(x => x.HasPic > 0), Job3 = into5.Count(x => x.HasAudio > 0), Job4 = into5.Count(x => x.IsFinished != "finish"), Job5 = into5.Count(x => x.IsFinished == "finish") };

            total = query.Count();
            var data = query.OrderByDescending(x => x.MeetingStartTime).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
            var result = data.Select(x => new Meeting2Entity { MeetingId = x.MeetingId, OtherMeetingId = x.OtherMeetingId, DeptId = x.GroupId, DeptName = x.FullName, MeetingStartTime = x.MeetingStartTime, Pic1 = x.q4.Count(y => y.Description == "照片"), Pic2 = x.q5.Count(y => y.Description == "照片") }).ToList();

            foreach (var item in result)
            {
                var subquery = from q1 in db.IQueryable<MeetingAndJobEntity>()
                               join q2 in db.IQueryable<FileInfoEntity>() on q1.MeetingJobId equals q2.RecId into into2
                               where q1.StartMeetingId == item.MeetingId
                               select new { q1, q2 = into2 };
                var d = subquery.ToList();
                item.Job1 = d.Count();
                item.Job2 = d.Count(x => x.q2.Count(y => y.Description == "照片") > 0);
                item.Job3 = d.Count(x => x.q2.Count(y => y.Description == "音频") > 0);
                item.Job4 = d.Count(x => x.q1.IsFinished != "finish");
                item.Job5 = d.Count(x => x.q1.IsFinished == "finish");

                var video1 = (from q in db.IQueryable<FileInfoEntity>()
                              where q.RecId == item.MeetingId && q.Description == "视频"
                              select q).FirstOrDefault();
                if (video1 != null) item.Video1 = video1.FilePath;

                var video2 = (from q in db.IQueryable<FileInfoEntity>()
                              where q.RecId == item.OtherMeetingId && q.Description == "视频"
                              select q).FirstOrDefault();
                if (video2 != null) item.Video2 = video2.FilePath;
            }

            return result;
        }

        public WorkmeetingEntity HasMeeting(string departmentId, DateTime date)
        {
            var from = date.Date;
            var to = from.AddDays(1);
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId into into2
                        from q2 in into2.DefaultIfEmpty()
                        where q1.GroupId == departmentId && q1.MeetingType == "班前会" && q1.MeetingStartTime >= @from && q1.MeetingStartTime < to && q1.IsOver == true
                        orderby q1.MeetingStartTime ascending
                        select new { q1.MeetingId, OtherId = (q2 == null && q2.IsOver == true) ? null : q2.MeetingId };
            var data = query.FirstOrDefault();
            return new WorkmeetingEntity() { MeetingId = data == null ? null : data.MeetingId, OtherMeetingId = data == null ? null : data.OtherId };
        }

        public List<WorkmeetingEntity> GetList(string[] depts, string userid, DateTime? begin, DateTime? end, string appraise, bool isAsc, int rows, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.OtherMeetingId equals q2.MeetingId into into2
                        from q2 in into2.DefaultIfEmpty()
                        join q3 in db.IQueryable<ActivityEvaluateEntity>() on q1.OtherMeetingId equals q3.Activityid into into3
                        join q4 in db.IQueryable<FileInfoEntity>() on q1.MeetingId equals q4.RecId into into4
                        join q5 in db.IQueryable<FileInfoEntity>() on q1.OtherMeetingId equals q5.RecId into into5
                        join q6 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q6.DepartmentId into into6
                        from q6 in into6.DefaultIfEmpty()
                        where q1.MeetingType == "班前会" && depts.Contains(q1.GroupId)
                        select new { q1, q2, q3 = into3, q4 = into4, q5 = into5, q6 };

            if (begin != null) query = query.Where(x => x.q1.MeetingStartTime >= begin);
            if (end != null) query = query.Where(x => x.q1.MeetingEndTime <= end);
            if (appraise == "1") query = query.Where(x => x.q3.Count(y => y.EvaluateId == userid) > 0);
            else if (appraise == "2") query = query.Where(x => x.q3.Count(y => y.EvaluateId == userid) == 0);

            total = query.Count();

            if (isAsc) query = query.OrderBy(x => x.q1.MeetingStartTime);
            else query = query.OrderByDescending(x => x.q1.MeetingStartTime);

            var data = query.Skip(rows * (page - 1)).Take(rows).ToList();

            var list = new List<WorkmeetingEntity>();
            foreach (var item in data)
            {
                var dataitem = new WorkmeetingEntity()
                {
                    MeetingId = item.q1.MeetingId,
                    MeetingStartTime = item.q1.MeetingStartTime,
                    MeetingEndTime = item.q1.MeetingEndTime,
                    IsOver = item.q1.IsOver,
                    GroupId = item.q1.GroupId,
                    GroupName = item.q6.FullName,
                    Files = item.q4.ToList()
                };
                if (item.q2 != null)
                {
                    dataitem.ShouldStartTime = item.q2.MeetingStartTime;
                    dataitem.ShouldEndTime = item.q2.MeetingEndTime;
                    dataitem.IsOver2 = item.q2.IsOver;
                    dataitem.Files2 = item.q5.ToList();
                }
                list.Add(dataitem);
            }

            return list;
        }

        public List<MeetingJobEntity> GetMeetingJobs(string meetingId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into into3
                        join q4 in db.IQueryable<DangerEntity>() on q1.MeetingJobId equals q4.JobId into into4
                        from q4 in into4.DefaultIfEmpty()
                            //join q5 in ctx.Include("TrainingUsers") on q1.MeetingJobId equals q5.MeetingJobId
                            //join q5 in (
                            //     from q1 in db.IQueryable<HumanDangerTrainingBaseEntity>()
                            //     join q2 in db.IQueryable<HumanDangerTrainingUserEntity>() on q1.TrainingId equals q2.TrainingId into t2
                            //     select new { q1, q2 = t2 }
                            //     ) on q1.MeetingJobId equals q5.q1.MeetingJobId into into5
                            //from q5 in into5.DefaultIfEmpty()
                        join q6 in (from q1 in db.IQueryable<JobDangerousEntity>()
                                    join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                    select new { q1, q2 = into2 }) on q2.JobId equals q6.q1.JobId into into6
                        join q7 in db.IQueryable<FileInfoEntity>() on q1.MeetingJobId equals q7.RecId into into7
                        where q1.StartMeetingId == meetingId
                        select new { q1, q2, q3 = into3, q4, q6 = into6, q7 = into7 };
            var data = query.ToList();

            var ctx = db.IQueryable<HumanDangerTrainingBaseEntity>() as DbQuery<HumanDangerTrainingBaseEntity>;

            foreach (var item in data)
            {
                item.q2.Relation = item.q1;
                item.q2.Relation.JobUsers = item.q3.ToList();
                item.q2.Files = item.q7.ToList();

                item.q2.HumanDangerTraining = (from q in ctx.Include("TrainingUsers").AsNoTracking()
                                               where q.MeetingJobId == item.q1.MeetingJobId
                                               select q).FirstOrDefault();

                if (item.q4 != null)
                {
                    item.q2.Training = item.q4;
                    if (item.q4 != null)
                        item.q2.TrainingDone = item.q4.Status == 2;
                }
                else if (item.q2.HumanDangerTraining != null)
                {
                    item.q2.Training = new DangerEntity() { Id = item.q2.HumanDangerTraining.TrainingUsers.First().TrainingUserId.ToString() };
                    item.q2.TrainingDone = item.q2.HumanDangerTraining.TrainingUsers.All(x => x.IsDone == true && x.IsMarked == true);
                    item.q2.HumanDangerTraining.TrainingUsers.ForEach(x => x.Training = null);
                }

                if (item.q6 != null)
                {
                    foreach (var item1 in item.q6)
                    {
                        item1.q1.MeasureList = item1.q2.ToList();
                    }
                    item.q2.DangerousList = item.q6.Select(x => x.q1).ToList();
                }
            }
            return data.Select(x => x.q2).ToList();
        }

        public List<MeetingJobEntity> FindMeetingJobs2(string departmentId, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into into3
                        join q4 in (from q1 in db.IQueryable<JobDangerousEntity>()
                                    join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                    select new { q1, q2 = into2.ToList() }) on q2.JobId equals q4.q1.JobId into into4
                        where q2.GroupId == departmentId && (q2.EndTime >= date && q2.StartTime <= date) && string.IsNullOrEmpty(q1.StartMeetingId)
                        select new { q1, q2, q3 = into3, q4 = into4 };


            var data = query.ToList();
            foreach (var item in data)
            {
                item.q2.Relation = item.q1;
                item.q2.Relation.JobUsers = item.q3.ToList();
                foreach (var item1 in item.q4)
                {
                    item1.q1.MeasureList = item1.q2.ToList();
                }
                item.q2.DangerousList = item.q4.Select(x => x.q1).ToList();
                item.q2.Dangerous = string.Join("、", item.q2.DangerousList.Select(x => x.Content));
                item.q2.Measure = string.Join("、", item.q2.DangerousList.SelectMany(y => y.MeasureList).Select(x => x.Content));
            }

            return data.Select(x => x.q2).ToList();
        }

        public void AddTodayJobs(List<MeetingJobEntity> jobs)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                db.Insert(jobs);
                db.Insert(jobs.Select(x => x.Relation).ToList());
                db.Insert(jobs.SelectMany(x => x.Relation.JobUsers).ToList());
                db.Insert(jobs.SelectMany(x => x.DangerousList).ToList());
                db.Insert(jobs.SelectMany(x => x.DangerousList.SelectMany(y => y.MeasureList)).ToList());
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }

        public List<MeetingJobEntity> FindLongJobs(string departmentId, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<MeetingJobEntity>()
                        where q.GroupId == departmentId && q.StartTime <= date && q.EndTime >= date && q.IsFinished == "undo"
                        select q;

            var data = query.ToList().Where(x => x.EndTime.Date > x.StartTime.Date).ToList();
            var lastmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                               where q.GroupId == departmentId
                               where q.MeetingType == "班前会"
                               orderby q.MeetingStartTime descending
                               select q).FirstOrDefault();
            if (lastmeeting != null)
            {
                foreach (var item in data)
                {
                    var jobusers = (from q1 in db.IQueryable<JobUserEntity>()
                                    join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                    where q2.StartMeetingId == lastmeeting.MeetingId && q2.JobId == item.JobId
                                    select q1).ToList();
                    item.Relation = new MeetingAndJobEntity()
                    {
                        MeetingJobId = Guid.NewGuid().ToString(),
                        IsFinished = "undo",
                        JobId = item.JobId,
                        JobUserId = string.Join(",", jobusers.Select(x => x.UserId)),
                        JobUser = string.Join(",", jobusers.Select(x => x.UserName))
                    };
                    item.Relation.JobUsers = jobusers.Select(x => new JobUserEntity() { JobUserId = Guid.NewGuid().ToString(), JobType = x.JobType, MeetingJobId = item.Relation.MeetingJobId, UserId = x.UserId, UserName = x.UserName }).ToList();
                }
            }
            else
                data = new List<MeetingJobEntity>();

            return data;
        }

        public void AddLongJobs(List<MeetingJobEntity> jobs)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                db.Insert(jobs.Select(x => x.Relation).ToList());
                db.Insert(jobs.SelectMany(x => x.Relation.JobUsers).ToList());
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public bool IsEndMeetingOver(string deptId, DateTime now)
        {
            var db = new RepositoryFactory().BaseRepository();

            var endmeeting = (from q in db.IQueryable<WorkmeetingEntity>()
                              where q.GroupId == deptId && q.MeetingType == "班后会"
                              orderby q.MeetingStartTime descending
                              select q).FirstOrDefault();
            if (endmeeting == null) return true;
            return endmeeting.MeetingStartTime.Date == now.Date && endmeeting.IsOver;
        }

        public void UpdateJobs(List<MeetingJobEntity> jobs)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    var item = jobs[i];
                    if (string.IsNullOrEmpty(item.JobId))
                    {
                        item.JobId = Guid.NewGuid().ToString();
                        item.IsFinished = "undo";
                        item.CreateDate = DateTime.Now;
                        item.JobType = "班前班后会";
                        item.Relation.MeetingJobId = Guid.NewGuid().ToString();
                        item.Relation.JobId = item.JobId;
                        item.Relation.IsFinished = "undo";
                        if (item.Relation.JobUsers == null) item.Relation.JobUsers = new List<JobUserEntity>();
                        item.Relation.JobUserId = string.Join(",", item.Relation.JobUsers.Select(x => x.UserId));
                        item.Relation.JobUser = string.Join(",", item.Relation.JobUsers.Select(x => x.UserName));
                        item.Relation.JobUsers.ForEach(x =>
                        {
                            x.JobUserId = Guid.NewGuid().ToString();
                            x.MeetingJobId = item.Relation.MeetingJobId;
                            x.CreateDate = DateTime.Now;
                        });
                        if (item.DangerousList == null) item.DangerousList = new List<JobDangerousEntity>();
                        item.DangerousList.ForEach(x =>
                        {
                            x.JobDangerousId = Guid.NewGuid().ToString();
                            x.JobId = item.JobId;
                            x.CreateTime = DateTime.Now;
                            if (x.MeasureList == null) x.MeasureList = new List<JobMeasureEntity>();
                            x.MeasureList.ForEach(y =>
                            {
                                y.JobMeasureId = Guid.NewGuid().ToString();
                                y.CreateTime = DateTime.Now;
                                y.JobDangerousId = x.JobDangerousId;
                            });
                        });

                        db.Insert(item);
                        db.Insert(item.Relation);
                        db.Insert(item.Relation.JobUsers);
                        db.Insert(item.DangerousList);
                        db.Insert(item.DangerousList.SelectMany(x => x.MeasureList).ToList());
                    }
                    else
                    {
                        if (item.Status == "1")
                        {
                            var job = (from q1 in db.IQueryable<MeetingJobEntity>()
                                       join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId into into2
                                       join q3 in (from q1 in db.IQueryable<JobDangerousEntity>()
                                                   join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                                   select new { q1, q2 = into2 }
                                                   ) on q1.JobId equals q3.q1.JobId into into3
                                       where q1.JobId == item.JobId
                                       select new { q1, q2 = into2, q3 = into3 }).FirstOrDefault();
                            if (job != null)
                            {
                                if (job.q2.Count() == 1)
                                {
                                    db.Delete(job.q1);
                                    db.Delete(job.q2.ToList());
                                    db.Delete(job.q3.Select(x => x.q1).ToList());
                                    db.Delete(job.q3.SelectMany(x => x.q2).ToList());
                                }
                                else
                                {
                                    var relation = job.q2.FirstOrDefault(x => x.MeetingJobId == item.Relation.MeetingJobId);
                                    db.Delete(relation);
                                }
                                jobs.Remove(item);
                                i--;
                            }
                        }
                        else
                        {
                            var job = db.FindEntity<MeetingJobEntity>(item.JobId);
                            job.Job = item.Job;
                            job.StartTime = item.StartTime;
                            job.EndTime = item.EndTime;
                            job.NeedTrain = item.NeedTrain;
                            job.TaskType = item.TaskType;
                            job.RiskLevel = item.RiskLevel;
                            db.Update(job);

                            var relation = db.FindEntity<MeetingAndJobEntity>(item.Relation.MeetingJobId);
                            relation.JobUserId = item.Relation.JobUsers == null ? string.Empty : string.Join(",", item.Relation.JobUsers.Select(x => x.UserId));
                            relation.JobUser = item.Relation.JobUsers == null ? string.Empty : string.Join(",", item.Relation.JobUsers.Select(x => x.UserName));
                            db.Update(relation);

                            db.Delete<JobUserEntity>(x => x.MeetingJobId == item.Relation.MeetingJobId);
                            item.Relation.JobUsers.ForEach(x =>
                            {
                                x.JobUserId = Guid.NewGuid().ToString();
                                x.MeetingJobId = item.Relation.MeetingJobId;
                                x.CreateDate = DateTime.Now;
                            });
                            db.Insert(item.Relation.JobUsers);

                            var data = (from q1 in db.IQueryable<JobDangerousEntity>()
                                        join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                                        where q1.JobId == item.JobId
                                        select new { q1, q2 = into2 }).ToList();
                            var dangers = data.Select(x => x.q1).ToList();
                            var measures = data.SelectMany(x => x.q2).ToList();
                            if (dangers.Count > 0)
                                db.Delete(dangers);
                            if (measures.Count > 0)
                                db.Delete(measures);
                            if (item.DangerousList != null)
                            {
                                item.DangerousList.ForEach(x =>
                                {
                                    x.JobDangerousId = Guid.NewGuid().ToString();
                                    x.CreateTime = DateTime.Now;
                                    x.JobId = item.JobId;
                                    if (x.MeasureList != null)
                                    {
                                        x.MeasureList.ForEach(y =>
                                        {
                                            y.JobMeasureId = Guid.NewGuid().ToString();
                                            y.CreateTime = DateTime.Now;
                                            y.JobDangerousId = x.JobDangerousId;
                                        });
                                    }
                                });
                                db.Insert(item.DangerousList);
                                db.Insert(item.DangerousList.SelectMany(x => x.MeasureList).ToList());
                            }
                        }
                    }
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<string> Query(string key, string deptid, int pagesize, int pageindex, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            //任务库
            var query1 = from q in db.IQueryable<JobTemplateEntity>()
                         where q.DeptId == deptid && q.DangerType == "job" && q.JobContent.Contains(key)
                         select q.JobContent;

            //危险预知训练库
            var query2 = from q in db.IQueryable<JobTemplateEntity>()
                         where q.JobType == "danger" && q.JobContent.Contains(key)
                         select q.JobContent;

            //人身风险预控
            var query3 = from q in db.IQueryable<HumanDangerEntity>()
                         where q.DeptId.Contains(deptid) && q.Task.Contains(key)
                         select q.Task;

            //历史任务
            var query4 = from q in db.IQueryable<MeetingJobEntity>()
                         where q.GroupId == deptid && q.Job.Contains(key)
                         select q.Job;

            var query = from q in query1.Concat(query2).Concat(query3).Concat(query4)
                        select q;

            query = query.Distinct();
            total = query.Count();
            return query.OrderBy(x => x).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
        }



        public JobTemplateEntity Detail(string deptid, string data)
        {
            var result = default(JobTemplateEntity);
            var db = new RepositoryFactory().BaseRepository();

            var query1 = from q in db.IQueryable<JobTemplateEntity>()
                         where q.DeptId == deptid && q.DangerType == "job" && q.JobContent == data
                         orderby q.CreateDate descending
                         select q;
            result = query1.FirstOrDefault();
            if (result != null)
            {
                var subquery = from q1 in db.IQueryable<JobDangerousEntity>()
                               join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                               where q1.JobId == result.JobId
                               select new { q1, q2 = into2 };
                var list = subquery.ToList();
                foreach (var item in list)
                {
                    item.q1.MeasureList = item.q2.ToList();
                }
                result.DangerousList = list.Select(x => x.q1).ToList();
                return result;
            }

            var query2 = from q in db.IQueryable<JobTemplateEntity>()
                         where q.JobType == "danger" && q.JobContent == data
                         orderby q.CreateDate descending
                         select q;
            result = query2.FirstOrDefault();
            if (result != null) return result;

            var query3 = from q in db.IQueryable<HumanDangerEntity>()
                         where q.DeptId.Contains(deptid) && q.Task == data
                         select q;
            var r1 = query3.FirstOrDefault();
            if (r1 != null)
            {
                result = new JobTemplateEntity() { JobId = r1.HumanDangerId.ToString(), JobContent = r1.Task };
                return result;
            }

            var query4 = from q in db.IQueryable<MeetingJobEntity>()
                         where q.GroupId == deptid && q.Job == data
                         orderby q.CreateDate descending
                         select q;
            var r2 = query4.FirstOrDefault();
            if (r2 != null)
            {
                result = new JobTemplateEntity() { JobContent = r2.Job };
                var subquery = from q1 in db.IQueryable<JobDangerousEntity>()
                               join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                               where q1.JobId == r2.JobId
                               select new { q1, q2 = into2 };
                var list = subquery.ToList();
                foreach (var item in list)
                {
                    item.q1.MeasureList = item.q2.ToList();
                }
                result.DangerousList = list.Select(x => x.q1).ToList();
            }
            return result;
        }

        /// <summary>
        /// 获取任务的详情
        /// </summary>
        /// <param name="meetingJobId"></param>
        /// <returns></returns>
        public JobTemplateEntity GetJobTemplateByMeetingJobId(string meetingJobId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobTemplateEntity>() on q2.TemplateId equals q3.JobId
                        where q1.MeetingJobId == meetingJobId
                        select q3;

            var data = query.FirstOrDefault();
            return data;
        }
        /// <summary>
        /// 模糊查询任务
        /// </summary>
        /// <param name="key"></param>
        /// <param name="deptid"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList QueryNew(string key, string deptid, int pagesize, int pageindex, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            //任务库
            var query1 = from q in db.IQueryable<JobTemplateEntity>()
                         where q.DeptId == deptid && q.DangerType == "job" && q.JobContent.Contains(key)
                         select new { q.JobId, q.JobContent, q.RiskLevel, q.TaskType };

            //危险预知训练库
            var query2 = from q in db.IQueryable<JobTemplateEntity>()
                         where q.JobType == "danger" && q.JobContent.Contains(key)
                         select new { q.JobId, q.JobContent, q.RiskLevel, q.TaskType };

            //人身风险预控
            var query3 = from q in db.IQueryable<HumanDangerEntity>()
                         where q.DeptId.Contains(deptid) && q.Task.Contains(key)
                         select new { JobId = q.HumanDangerId.ToString(), JobContent = q.Task, RiskLevel = "", TaskType = "" };

            //历史任务
            var query4 = from q in db.IQueryable<MeetingJobEntity>()
                         join q2 in db.IQueryable<JobTemplateEntity>() on q.TemplateId equals q2.JobId
                         where q.GroupId == deptid && q.Job.Contains(key)
                         select new { JobId = q.JobId, JobContent = q.Job, q2.RiskLevel, q.TaskType };

            var query = from q in query1.Concat(query2).Concat(query3).Concat(query4)
                        select q;

            query = query.Distinct();
            total = query.Count();
            return query.OrderBy(x => x)
                        .Skip((pageindex - 1) * pagesize)
                        .Take(count: pagesize)
                        .ToList();
        }

        /// <summary>
        /// 今日工作各风险等级的统计
        /// </summary>
        /// <param name="searchDeptId">要搜索的部门的Id</param>
        /// <param name="now"></param>
        /// <returns></returns>
        public Dictionary<string, int> TodayWorkStatistics(string DeptStr, DateTime now)
        {
            //            string startTime = now.Date.ToString("yyyy-MM-dd");
            //            string endTime = now.AddDays(1).Date.ToString("yyyy-MM-dd");
            //            string sql = $@"SELECT  COUNT(*) as COUNT, RiskLevel  from  wg_meetingandjob A 
            //		left JOIN wg_meetingjob B  ON A.JobId=B.JobId WHERE B.GroupId IN (
            //select departmentid from base_department where ENCODE like '{searchDeptCode}%') AND  (A.startmeetingid IN (select  meetingid from wg_workmeeting where meetingstarttime>='{startTime}' and meetingendtime<'{endTime}'and meetingtype='班前会')
            //		or A.endmeetingid in (select  meetingid from wg_workmeeting where meetingstarttime>='{startTime}' and meetingendtime<'{endTime}'and meetingtype='班后会'))
            //		group by RiskLevel";

            //            Dictionary<string, int> dic = new Dictionary<string, int>();
            //            DataTable dt = BaseRepository().FindTable(sql);
            //            if (dt.Rows != null && dt.Rows.Count > 0)
            //            {
            //                var each = dt.Rows.GetEnumerator();
            //                while (each.MoveNext())
            //                {
            //                    DataRow dr = each.Current as DataRow;
            //                    if (dr["RiskLevel"] != null && !dr["RiskLevel"].ToString().IsEmpty())
            //                    {
            //                        dic.Add(dr["RiskLevel"].ToString(), Convert.ToInt32(dr["COUNT"]));
            //                    }
            //                }
            //            }
            List<string> riskList = new List<string>()
                        {
                            "重大风险",
                            "较大风险",
                            "一般风险",
                            "低风险"
                        };
            DateTime startTime = now.Date.AddDays(1);
            DateTime endTime = now.Date;
            var db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<MeetingJobEntity>()
                        join b in db.IQueryable<MeetingAndJobEntity>() on a.JobId equals b.JobId
                        join d in db.IQueryable<DepartmentEntity>() on a.GroupId equals d.DepartmentId
                        where DeptStr.Contains(d.DepartmentId) && a.StartTime < startTime && a.EndTime >= endTime
                        orderby a.CreateDate descending
                        group a by a.RiskLevel into g
                        select new { g.Key, Count = g.Count() };

            Dictionary<string, int> dic = new Dictionary<string, int>();
            var list = query.ToList();
            if (list != null && list.Count > 0)
            {
                list.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Key)) dic.Add(x.Key, x.Count);
                });
            }
            //去除已经有的风险
            riskList.RemoveAll(x => dic.Keys.ToList().Contains(x));
            //把没有的风险加进去，数量为0
            if (riskList.Count > 0)
            {
                riskList.ForEach(p =>
                {
                    dic.Add(p, 0);
                });
            }

            return dic;
        }


        /// <summary>
        /// 分页获取工作任务
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public object GetJobPagedList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<MeetingJobEntity>()
                        join b in db.IQueryable<MeetingAndJobEntity>() on a.JobId equals b.JobId
                        join c in db.IQueryable<JobUserEntity>() on b.MeetingJobId equals c.MeetingJobId into t
                        join d in db.IQueryable<DepartmentEntity>() on a.GroupId equals d.DepartmentId
                        orderby a.CreateDate descending
                        select new
                        {
                            a.Job,
                            a.TaskType,
                            d.FullName,
                            JobUsers = t.Select(x => x.UserName),
                            a.StartTime,
                            a.EndTime,
                            a.Dangerous,
                            a.Measure,
                            a.CreateDate,
                            a.RiskLevel,
                            d.EnCode
                        };

            var queryParam = queryJson.ToJObject();
            if (!queryParam["StartTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["StartTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime startTime = time.Date.AddDays(1);
                DateTime endTime = time.Date;
                query = query.Where(p => p.StartTime < startTime && p.EndTime >= endTime);
            }

            if (!queryParam["RiskLevel"].IsEmpty())
            {
                var riskLevel = queryParam["RiskLevel"].ToString();
                query = query.Where(p => p.RiskLevel == riskLevel);
            }

            if (!queryParam["KeyWord"].IsEmpty())
            {
                var keyword = queryParam["KeyWord"].ToString();
                query = query.Where(p => p.Job.Contains(keyword));
            }
            //用户的所在单位的Id，特殊部门/厂级部门查看全厂数据,部门级用户查看本部门数据.
            if (!queryParam["DeptCode"].IsEmpty())
            {
                var deptCode = queryParam["DeptCode"].ToString();
                query = query.Where(p => p.EnCode.StartsWith(deptCode));
            }
            int total = query.Count();
            pagination.records = total;
            var data = query.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            return data;
        }

        #region 工时管理

        /// <summary>
        /// 工时分页查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="keyWord"></param>
        /// <param name="deptCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList GetJobHourPagedList(string startTime, string endTime, string keyWord, string deptCode, int pageIndex, int pageSize, out int total)
        {
            total = 0;
            // 部门级用户查看本部门,厂级用户 / 特殊部门用户看全厂,界面如下图: 有评分的,可点击评分查看评分详情
            var db = new RepositoryFactory().BaseRepository();
            var grouplist = db.IQueryable<DepartmentEntity>().Where(p => p.EnCode.Contains(deptCode)).Select(x => x.DepartmentId).ToList();

            #region 2- 找出这些班组的班前班后会的jobId

            var meetingId = db.IQueryable<WorkmeetingEntity>(p => grouplist.Contains(p.GroupId)).Select(p => p.MeetingId).ToList();
            var othertMeetingId = db.IQueryable<WorkmeetingEntity>(p => grouplist.Contains(p.GroupId)).Select(p => p.OtherMeetingId).ToList();
            var allMeetingId = meetingId.Concat(othertMeetingId).Where(p => !string.IsNullOrWhiteSpace(p)).ToList(); ;
            var jobid = db.IQueryable<MeetingAndJobEntity>(p => allMeetingId.Contains(p.StartMeetingId) || allMeetingId.Contains(p.EndMeetingId)).Select(p => p.JobId);
            #endregion
            #region 3- 拼接查询条件
            var query = db.IQueryable<MeetingJobEntity>(p => jobid.Contains(p.JobId) && p.IsFinished != "cancel");

            if (!startTime.IsEmpty())
            {
                DateTime ks;
                if (DateTime.TryParse(startTime, out ks))
                {
                    query = query.Where(p => p.StartTime >= ks);
                }
            }
            if (!endTime.IsEmpty())
            {
                DateTime js;
                if (DateTime.TryParse(endTime, out js))
                {
                    js = js.AddDays(1).Date;
                    query = query.Where(p => p.StartTime <= js);
                }
            }
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                query = query.Where(p => p.Job.Contains(keyWord));
            }
            #endregion

            total = query.Count();
            var jobList = query.OrderByDescending(p => p.StartTime).Skip(pageSize * (pageIndex - 1)).Take(pageSize).Select(p => new
            {
                p.JobId,
                p.Job,
                p.IsFinished,
                p.StartTime,
                p.EndTime,
                p.EvaluateState,
                p.NeedTrain
            }).ToList();
            var data = from q1 in jobList
                       join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                       join q4 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q4.MeetingJobId into t
                       //group new { q1, q2, t } by q1 into g
                       select new
                       {
                           q1.JobId,
                           q1.Job,
                           q1.IsFinished,
                           q1.StartTime,
                           q1.EndTime,
                           q1.EvaluateState,
                           q1.NeedTrain,
                           JobUsers = t
                       };
            var dataList = data.ToList();
            return dataList;
        }
        /// <summary>
        /// 登记工时
        /// </summary>
        /// <param name="userEntities"></param>
        public void SaveTaskHour(List<JobUserEntity> userEntities)
        {
            if (userEntities != null && userEntities.Count > 0)
            {

                foreach (var item in userEntities)
                {
                    var db = new RepositoryFactory().BaseRepository();
                    var jobUser = db.IQueryable<JobUserEntity>(x => x.UserId == item.UserId && x.MeetingJobId == item.MeetingJobId).FirstOrDefault();
                    if (jobUser != null)
                    {
                        jobUser.TaskHour = item.TaskHour;
                        db.Update(jobUser);
                    }
                }
            }
        }

        public int Count(string[] depts, DateTime start, DateTime end)
        {
            var query = from q in _context.Set<WorkmeetingEntity>()
                        where depts.Contains(q.GroupId) && q.IsOver == true && q.MeetingStartTime >= start && q.MeetingStartTime < end
                        select q;

            return query.Count();
        }
        #endregion
        /// <summary>
        /// 获取工作任务
        /// </summary>
        /// <param name="pagination">分页公共类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id</param>
        /// <returns></returns>
        public List<MeetingJobEntity> GetMeetJobList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            IQueryable<MeetingJobEntity> query = from a in db.IQueryable<MeetingJobEntity>()
                                                 select a;
            var queryParam = queryJson.ToJObject();
            //startTime开始时间
            if (!queryParam["startTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["startTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime startTime = time;
                query = query.Where(p => p.StartTime >= startTime);
            }
            //endTime结束时间
            if (!queryParam["endTime"].IsEmpty())
            {
                DateTime time;
                if (!DateTime.TryParse(queryParam["endTime"].ToString(), out time))
                    time = DateTime.Now.Date;
                DateTime endTime = time;
                query = query.Where(p => p.EndTime <= endTime);
            }
            //deptId部门id
            if (!queryParam["deptId"].IsEmpty())
            {
                string deptId = queryParam["deptId"].ToString();
                query = query.Where(p => p.GroupId == deptId);

            }
            //userId用户id
            if (!queryParam["userId"].IsEmpty())
            {
                var userId = queryParam["userId"].ToString();
                query = query.Where(p => p.CreateUserId == userId);
            }
            int total = query.Count();
            pagination.records = total;
            var data = query.OrderByDescending(x => x.StartTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            return data;

        }

        public WorkmeetingEntity Get(string id)
        {
            var entity = workMeetings.AsNoTracking().FirstOrDefault(x => x.MeetingId == id);
            return entity;
        }
    }
}
