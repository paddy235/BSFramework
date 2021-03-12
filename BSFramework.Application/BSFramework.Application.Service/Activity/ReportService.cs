using BSFramework.Application.IService.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;
using Bst.Bzzd.DataSource;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.Entity.EducationManage;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Data.EF;
using BSFramework.Util.Extension;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 工作总结实现
    /// </summary>
    public class ReportService : IReportService
    {
        private System.Data.Entity.DbContext _context;

        public ReportService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }



        private ReportEntity BuildReport(PeopleEntity user, DateTime start, DateTime end, string reporttype)
        {
            var db = new RepositoryFactory().BaseRepository();

            var reportcontent = string.Empty;
            var tasks = string.Empty;
            var undo = string.Empty;
            var groupjobs = default(List<TaskEntity>);

            if (reporttype == "月工作总结")
            {
                //月总结

                if (user.Quarters == "班长" || user.Quarters == "副班长" || user.Quarters == "技术员")
                {
                    //工作
                    var part1 = string.Empty;
                    var jobs = (from q in db.IQueryable<MeetingJobEntity>()
                                join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.JobId
                                where q.GroupId == user.BZID && q.IsFinished != "cancel" && q.EndTime >= start && q.StartTime <= end && !string.IsNullOrEmpty(q1.StartMeetingId)
                                select q).ToList();

                    if (jobs.Count > 0) part1 = string.Format("本月安排工作任务{0}项，完成{1}项,{2}进行中,{3}未完成。", jobs.Count, jobs.Count(x => x.IsFinished == "finish"), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime > end), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime <= end));

                    //危险预知训练
                    var part2 = string.Empty;
                    var dangers = (from q in db.IQueryable<DangerEntity>()
                                   where q.GroupId == user.BZID && q.Status == 2 && q.JobTime >= start && q.JobTime <= end
                                   select q).ToList();
                    if (dangers.Count > 0) part2 = string.Format("本月进行危险预知训练{0}", dangers.Count);

                    //班组活动
                    var part3 = string.Empty;
                    var activities = (from q in db.IQueryable<ActivityEntity>()
                                      where q.GroupId == user.BZID && q.State == "Finish" && q.StartTime >= start && q.StartTime <= end
                                      select q).ToList();
                    if (activities.Count > 0) part3 = string.Format("本月进行了{0}", string.Join("，", activities.GroupBy(x => x.ActivityType).Select(x => string.Format("{0}次{1}", x.Count(), x.Key))));

                    //技术问答和事故预想
                    var part4 = string.Empty;
                    var acts = (from q in db.IQueryable<EduBaseInfoEntity>()
                                where q.BZId == user.BZID && q.Flow == "1" && q.StartDate >= start && q.StartDate <= end
                                select q).ToList();
                    if (acts.Count > 0) part4 = string.Format("本月进行了{0}次技术问答，{1}次事故预想", acts.Count(x => x.EduType == "2"), acts.Count(x => x.EduType == "3"));

                    //考勤
                    var part5 = string.Empty;
                    var meetings = (from q in db.IQueryable<WorkmeetingEntity>()
                                    where q.GroupId == user.BZID && q.MeetingStartTime >= start && q.MeetingEndTime <= end
                                    orderby q.MeetingStartTime
                                    select q).ToList();
                    if (meetings.Count > 0)
                    {
                        var timestamp = meetings.Last().MeetingStartTime.Date - meetings.First().MeetingStartTime.Date;
                        var days = Math.Ceiling(timestamp.TotalDays);
                        var matesquery = from q in db.IQueryable<PeopleEntity>()
                                         where q.BZID == user.BZID
                                         select q.ID;
                        var matescount = matesquery.Count();
                        var unsign = (from q in db.IQueryable<UnSignRecordEntity>()
                                      where matesquery.Contains(q.UserId) && q.StartTime >= start && q.EndTime <= end
                                      select q).ToList();
                        part5 = string.Format("本月出勤率{0}", ((matescount * days - unsign.Count) / ((matescount * days) == 0 ? 1 : (matescount * days))).ToString("p"));
                    }

                    var alljobs = (from q1 in db.IQueryable<MeetingJobEntity>()
                                   join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                   join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                                   where q1.GroupId == user.BZID && q1.IsFinished != "cancel" && q1.StartTime >= start && q1.StartTime <= end && !string.IsNullOrEmpty(q2.StartMeetingId)
                                   group new { q1.Job, q1.StartTime, q1.EndTime, q3.UserName } by new { q1.Job, q1.StartTime, q1.EndTime } into g
                                   select g).ToList();
                    groupjobs = alljobs.Select(x => new TaskEntity() { TaskContent = x.Key.Job, TaskPerson = string.Join(",", x.Select(y => y.UserName)), TaskPrior = string.Format("{0} ~ {1}", x.Key.StartTime.ToString("yyyy-M-d H:mm"), x.Key.EndTime.ToString("yyyy-M-d H:mm")) }).ToList();
                    reportcontent = string.Join(Environment.NewLine, new string[] { part1, part2, part3, part4, part5 }.Where(x => !string.IsNullOrEmpty(x)));
                    tasks = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "finish").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                    undo = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "undo").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                }
                else
                {
                    //工作
                    var part1 = string.Empty;
                    var jobs = (from q in db.IQueryable<MeetingJobEntity>()
                                join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.JobId
                                join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                where q2.UserId == user.ID && q.IsFinished != "cancel" && q.EndTime >= start && q.StartTime <= end && !string.IsNullOrEmpty(q1.StartMeetingId)
                                select q).ToList();
                    if (jobs.Count > 0) part1 = string.Format("本月安排工作任务{0}项，完成{1}项,{2}项进行中,{3}项未完成。", jobs.Count, jobs.Count(x => x.IsFinished == "finish"), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime > end), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime <= end));

                    //技术问答和事故预想
                    var part2 = string.Empty;
                    var total = (from q1 in db.IQueryable<MeetingJobEntity>()
                                 join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                 join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                                 where q1.IsFinished != "cancel" && q1.EndTime >= start && q1.StartTime <= end && q1.GroupId == user.BZID
                                 select q3.Score).ToList();
                    var usercount = db.IQueryable<UserEntity>().Count(x => x.DepartmentId == user.BZID);
                    var avg = (decimal)total.Sum(x => x ?? 0) / (usercount == 0 ? 1 : usercount);
                    var score = (from q1 in db.IQueryable<MeetingJobEntity>()
                                 join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                                 join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId
                                 join q4 in db.IQueryable<WorkmeetingEntity>() on q2.EndMeetingId equals q4.MeetingId
                                 where q4.MeetingType == "班后会" && q1.IsFinished != "cancel" && q1.EndTime >= start && q1.StartTime <= end && q3.UserId == user.ID
                                 select q3.Score).Sum();
                    part2 = string.Format("本月班组平均绩效评分{0}分，本人得分{1}分", avg.ToString("f1"), score ?? 0);
                    reportcontent = string.Join(Environment.NewLine, new string[] { part1, part2 }.Where(x => !string.IsNullOrEmpty(x)));
                    tasks = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "finish").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                    undo = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "undo").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                }
            }
            else
            {
                if (user.Quarters == "班长" || user.Quarters == "副班长" || user.Quarters == "技术员")
                {
                    //工作
                    var part1 = string.Empty;
                    var jobs = (from q in db.IQueryable<MeetingJobEntity>()
                                join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.JobId
                                where q.GroupId == user.BZID && q.IsFinished != "cancel" && q.EndTime >= start && q.StartTime <= end && !string.IsNullOrEmpty(q1.StartMeetingId)
                                select q).ToList();
                    if (jobs.Count > 0) part1 = string.Format("本周安排工作任务{0}项，完成{1}项,{2}进行中,{3}未完成。", jobs.Count, jobs.Count(x => x.IsFinished == "finish"), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime > end), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime <= end));

                    //危险预知训练
                    var part2 = string.Empty;
                    var dangers = (from q in db.IQueryable<DangerEntity>()
                                   where q.GroupId == user.BZID && q.Status == 2 && q.JobTime >= start && q.JobTime <= end
                                   select q).ToList();
                    if (dangers.Count > 0) part2 = string.Format("本周进行危险预知训练{0}", dangers.Count);

                    //班组活动
                    var part3 = string.Empty;
                    var activities = (from q in db.IQueryable<ActivityEntity>()
                                      where q.GroupId == user.BZID && q.State == "Finish" && q.StartTime >= start && q.StartTime <= end
                                      select q).ToList();
                    if (activities.Count > 0) part3 = string.Format("本周进行了{0}", string.Join("，", activities.GroupBy(x => x.ActivityType).Select(x => string.Format("{0}次{1}", x.Count(), x.Key))));

                    //技术问答和事故预想
                    var part4 = string.Empty;
                    var acts = (from q in db.IQueryable<EduBaseInfoEntity>()
                                where q.BZId == user.BZID && q.Flow == "1" && q.StartDate >= start && q.StartDate <= end
                                select q).ToList();
                    if (acts.Count > 0) part4 = string.Format("本周进行了{0}次技术问答，{1}次事故预想", acts.Count(x => x.EduType == "2"), acts.Count(x => x.EduType == "3"));

                    //考勤
                    var part5 = string.Empty;
                    var meetings = (from q in db.IQueryable<WorkmeetingEntity>()
                                    where q.GroupId == user.BZID && q.MeetingStartTime >= start && q.MeetingEndTime <= end
                                    orderby q.MeetingStartTime
                                    select q).ToList();
                    if (meetings.Count > 0)
                    {
                        var timestamp = meetings.Last().MeetingStartTime.Date - meetings.First().MeetingStartTime.Date;
                        var days = Math.Ceiling(timestamp.TotalDays);
                        var matesquery = from q in db.IQueryable<PeopleEntity>()
                                         where q.BZID == user.BZID
                                         select q.ID;
                        var matescount = matesquery.Count();
                        var unsign = (from q in db.IQueryable<UnSignRecordEntity>()
                                      where matesquery.Contains(q.UserId) && q.StartTime >= start && q.EndTime <= end
                                      select q).ToList();
                        part5 = string.Format("本周出勤率{0}", ((matescount * days - unsign.Count) / ((matescount * days) == 0 ? 1 : (matescount * days))).ToString("p"));
                    }
                    reportcontent = string.Join(Environment.NewLine, new string[] { part1, part2, part3, part4, part5
}.Where(x => !string.IsNullOrEmpty(x)));
                    var group = jobs.Where(x => x.IsFinished == "finish").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).ToList();
                    var sb = new StringBuilder();
                    for (int i = 0; i < group.Count; i++)
                    {
                        sb.AppendFormat("{0}、{1}{2}{3}", (i + 1), group[i].Key, group[i].Count() > 1 ? group.Count() + "次" : string.Empty, Environment.NewLine);
                    }
                    tasks = sb.ToString();
                    group = jobs.Where(x => x.IsFinished == "undo").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).ToList();
                    sb = new StringBuilder();
                    for (int i = 0; i < group.Count; i++)
                    {
                        sb.AppendFormat("{0}、{1}{2}{3}", (i + 1), group[i].Key, group[i].Count() > 1 ? group.Count() + "次" : string.Empty, Environment.NewLine);
                    }
                    undo = sb.ToString();
                    //tasks = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "finish").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                    //undo = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "undo").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                }
                else
                {
                    //工作
                    var part1 = string.Empty;
                    var jobs = (from q in db.IQueryable<MeetingJobEntity>()
                                join q1 in db.IQueryable<MeetingAndJobEntity>() on q.JobId equals q1.JobId
                                join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                where q2.UserId == user.ID && q.IsFinished != "cancel" && q.EndTime >= start && q.StartTime <= end && !string.IsNullOrEmpty(q1.StartMeetingId)
                                select q).ToList();
                    if (jobs.Count > 0) part1 = string.Format("本周安排工作任务{0}项，完成{1}项,{2}项进行中,{3}项未完成。", jobs.Count, jobs.Count(x => x.IsFinished == "finish"), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime > end), jobs.Count(x => (x.IsFinished == "undo" || string.IsNullOrEmpty(x.IsFinished)) && x.EndTime <= end));

                    //技术问答和事故预想
                    var part2 = string.Empty;
                    var acts = (from q in db.IQueryable<EduBaseInfoEntity>()
                                where q.BZId == user.BZID && q.Flow == "1" && q.StartDate >= start && q.StartDate <= end
                                select q).ToList();
                    if (acts.Count > 0) part2 = string.Format("本周进行了{0}次培训", acts.Count);
                    reportcontent = string.Join(Environment.NewLine, new string[] { part1, part2 }.Where(x => !string.IsNullOrEmpty(x)));
                    var group = jobs.Where(x => x.IsFinished == "finish").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).ToList();
                    var sb = new StringBuilder();
                    for (int i = 0; i < group.Count; i++)
                    {
                        sb.AppendFormat("{0}、{1}{2}{3}", (i + 1), group[i].Key, group[i].Count() > 1 ? group.Count() + "次" : string.Empty, Environment.NewLine);
                    }
                    tasks = sb.ToString();
                    group = jobs.Where(x => x.IsFinished == "undo").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).ToList();
                    sb = new StringBuilder();
                    for (int i = 0; i < group.Count; i++)
                    {
                        sb.AppendFormat("{0}、{1}{2}{3}", (i + 1), group[i].Key, group[i].Count() > 1 ? group.Count() + "次" : string.Empty, Environment.NewLine);
                    }
                    undo = sb.ToString();
                    //tasks = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "finish").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                    //undo = string.Join(Environment.NewLine, jobs.Where(x => x.IsFinished == "undo").OrderBy(x => x.StartTime).Select(x => x.Job).GroupBy(x => x).Select(x => string.Format("{0}{1}", x.Key, x.Count() > 1 ? x.Count() + "次" : string.Empty)));
                }
            }

            var result = new ReportEntity { ReportId = Guid.NewGuid(), ReportContent = reportcontent, ReportTime = DateTime.Now, ReportType = reporttype, ReportUser = user.Name, StartTime = start, EndTime = end, Tasks = tasks, Undo = undo, TaskList = groupjobs };

            return result;
        }

        public List<ReportEntity> BuildAllReport(DateTime start, DateTime end, string reporttype)
        {
            var db = new RepositoryFactory().BaseRepository();
            var users = (from q in db.IQueryable<PeopleEntity>()
                         select q).ToList();

            var result = new List<ReportEntity>();
            var data = new List<Report>();
            foreach (var item in users)
            {
                var report = this.BuildReport(item, start, end, reporttype);
                result.Add(report);
                data.Add(new Report()
                {
                    ReportId = report.ReportId,
                    ReportUserId = item.ID.Trim(),
                    ReportUser = item.Name,
                    ReportDeptId = item.BZID,
                    ReportDeptName = item.BZName,
                    ReportTime = DateTime.Now,
                    ReportType = this.ToTypeInt(report.ReportType),
                    ReportContent = report.ReportContent,
                    Tasks = report.Tasks,
                    Plan = report.Plan,
                    StartTime = report.StartTime.Value,
                    EndTime = report.EndTime.Value,
                    Undo = report.Undo,
                    Notices = new List<ReportNotice>()
                    {
                        new ReportNotice() {IsRead = false, NoticeId = Guid.NewGuid(), UserId = item.ID.Trim(), UserName = item.Name }
                    }
                });
            }

            using (var ctx = new DataContext())
            {
                ctx.Reports.AddRange(data);
                ctx.SaveChanges();
            }

            return result;
        }

        public ReportEntity GetDetail(string id)
        {
            var reportset = _context.Set<ReportEntity>();
            var taskset = _context.Set<TaskEntity>();
            var entity = reportset.Find(Guid.Parse(id));
            if (entity != null)
            {
                entity.TaskList = taskset.Where(x => x.ReportId == entity.ReportId).ToList();
                //entity.ReportType = this.ToTypeString(entity.ReportType);
            }
            return entity;
            //using (var ctx = new DataContext())
            //{
            //    var data = ctx.Reports.Find(Guid.Parse(id));
            //    if (data == null) return null;

            //    return new ReportEntity() { ReportId = data.ReportId, ReportContent = data.ReportContent, ReportTime = data.ReportTime, ReportType = this.ToTypeString(data.ReportType), ReportUser = data.ReportUser, Undo = data.Undo, StartTime = data.StartTime, EndTime = data.EndTime, Plan = data.Plan, Tasks = data.Tasks };
            //}
        }

        public List<ReportEntity> GetReports(string type, DateTime? start, DateTime? end, string key, int pagesize, int pageindex, out int total)
        {
            return this.GetReports(null, type, null, start, end, key, pagesize, pageindex, out total, null, null);
        }

        public List<ReportEntity> GetReports(string category, string type, string reportuserid, DateTime? start, DateTime? end, string key, int pagesize, int pageindex, out int total, string userId, bool? unread)
        {
            using (var ctx = new DataContext())
            {
                var query = ctx.Reports.Include("Comments").AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    switch (category)
                    {
                        case "全部":
                            query = query.Where(x => x.ReportUserId == userId || x.Notices.Any(y => y.UserId == userId));
                            break;
                        case "未读":
                            query = query.Where(x => x.Notices.Any(y => y.UserId == userId && y.IsRead == false));
                            break;
                        case "我的":
                            query = query.Where(x => x.ReportUserId == userId);
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(type) && type != "全部")
                {
                    var reporttype = ToTypeInt(type);
                    query = query.Where(x => x.ReportType == reporttype);
                }
                if (!string.IsNullOrEmpty(key))
                    query = query.Where(x => x.ReportContent.Contains(key) || x.ReportUser.Contains(key));

                if (start != null)
                    query = query.Where(x => x.StartTime >= start);

                if (end != null)
                {
                    end = end.Value.AddDays(1);
                    query = query.Where(x => x.EndTime < end);
                }

                if (reportuserid != null)
                    query = query.Where(x => x.ReportUserId == reportuserid);

                if (unread != null)
                    query = query.Where(x => x.Notices.Any(y => y.UserId == userId && y.IsRead == false));

                var data = query.OrderByDescending(x => x.ReportTime).Skip(pagesize * (pageindex - 1)).Take(pagesize).Select(x => new { x.ReportId, x.ReportContent, x.ReportTime, x.ReportUser, x.ReportType, x.Tasks, x.Plan, x.Undo, x.Cantdo, x.StartTime, x.EndTime, x.IsSubmit, ReadTotal = x.Notices.Count(y => y.IsRead), x.Comments, CommentsTotal = x.Comments.Count }).ToList();
                total = query.Count();

                return data.Select(x => new ReportEntity() { ReportId = x.ReportId, ReportContent = x.ReportContent, ReportTime = x.ReportTime, ReportUser = x.ReportUser, ReportType = this.ToTypeString(x.ReportType), Tasks = x.Tasks, Plan = x.Plan, Undo = x.Undo, Cantdo = x.Cantdo, StartTime = x.StartTime, EndTime = x.EndTime, ReadTotal = x.ReadTotal, CommentsTotal = x.CommentsTotal, IsSubmit = x.IsSubmit, Comments = x.Comments.Select(y => new CommentEntity() { CommentId = y.CommentId, Content = y.Content, CommentUserId = y.CommentUserId, CommentUser = y.CommentUser }).ToList() }).ToList();
            }
        }

        private Bst.Bzzd.DataSource.Entities.ReportType ToTypeInt(string type)
        {
            switch (type)
            {
                case "周工作总结":
                    return Bst.Bzzd.DataSource.Entities.ReportType.Weekly;
                case "月工作总结":
                    return Bst.Bzzd.DataSource.Entities.ReportType.Monthly;
                default:
                    return 0;
            }
        }

        private string ToTypeString(ReportType type)
        {
            switch (type)
            {
                case ReportType.Weekly:
                    return "周工作总结";
                case ReportType.Monthly:
                    return "月工作总结";
                default:
                    return "周工作总结";
            }
        }

        public ReportSettingEntity GetSetting(string reporttype)
        {
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.ReportSettings
                            where q.SettingName == reporttype
                            select q;

                var data = query.FirstOrDefault();
                if (data == null) return null;

                return new ReportSettingEntity() { SettingId = data.SettingId, SettingName = data.SettingName, StartTime = data.StartTime, EndTime = data.EndTime, Start = data.Start, End = data.End };
            }
        }

        public ReportEntity Submit(ReportEntity data, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var users = default(List<UserEntity>);
            if (!string.IsNullOrEmpty(data.ToUserId))
            {
                var userids = data.ToUserId.Split(',');
                users = (from q in db.IQueryable<UserEntity>()
                         where userids.Contains(q.UserId)
                         select q).ToList();
            }

            using (var ctx = new DataContext())
            {
                var entity = ctx.Reports.Include("Notices").FirstOrDefault(x => x.ReportId == data.ReportId);
                entity.Plan = data.Plan;
                entity.ReportContent = data.ReportContent;
                entity.Tasks = data.Tasks;
                entity.Undo = data.Undo;
                entity.Cantdo = data.Cantdo;
                entity.IsSubmit = true;

                if (users != null)
                {
                    users.RemoveAll(x => entity.Notices.Any(y => y.UserId == x.UserId));
                    foreach (var item in users)
                    {
                        entity.Notices.Add(new ReportNotice()
                        {
                            NoticeId = Guid.NewGuid(),
                            IsRead = false,
                            UserId = item.UserId,
                            UserName = item.RealName,
                            NoticeType = 1
                        });
                    }
                }

                ctx.SaveChanges();

                return new ReportEntity() { ReportId = entity.ReportId, ReportType = this.ToTypeString(entity.ReportType) };
            }

        }

        public ReportEntity GetReport(string id, string userid)
        {
            var reportid = Guid.Parse(id);
            var result = default(ReportEntity);

            using (var ctx = new DataContext())
            {
                var entity = ctx.Reports.Include("Notices").Include("Comments").FirstOrDefault(x => x.ReportId == reportid);
                var notice = entity.Notices.Find(x => x.UserId == userid);
                if (notice.IsRead == false)
                {
                    notice.IsRead = true;
                    ctx.SaveChanges();
                }

                result = new ReportEntity() { ReportId = entity.ReportId, ReportContent = entity.ReportContent, StartTime = entity.StartTime, EndTime = entity.EndTime, Plan = entity.Plan, ReportTime = entity.ReportTime, ReportType = this.ToTypeString(entity.ReportType), ReportUserId = entity.ReportUserId, ReportUser = entity.ReportUser, Tasks = entity.Tasks, Undo = entity.Undo, Cantdo = entity.Cantdo, IsSubmit = entity.IsSubmit, Comments = entity.Comments.OrderByDescending(x => x.CommentTime).Select(y => new CommentEntity() { CommentId = y.CommentId, Content = y.Content, CommentUserId = y.CommentUserId, CommentUser = y.CommentUser }).ToList(), Notices = entity.Notices.Select(x => new NoticeEntity() { NoticeId = x.NoticeId, ReportId = x.ReportId, UserId = x.UserId, UserName = x.UserName, IsRead = x.IsRead }).ToList() };
            }

            if (result != null)
            {
                IRepository db = new RepositoryFactory().BaseRepository();
                var file = (from q in db.IQueryable<FileInfoEntity>()
                            where q.RecId == id
                            select q).FirstOrDefault();

                if (file != null)
                    result.FilePath = file.FilePath;
            }

            return result;
        }

        public string[] Share(string id, string[] users)
        {
            var reportid = Guid.Parse(id);
            var db = new RepositoryFactory().BaseRepository();
            var userdata = (from q in db.IQueryable<UserEntity>()
                            where users.Contains(q.UserId)
                            select q).ToList();

            using (var ctx = new DataContext())
            {
                var entity = ctx.Reports.Include("Notices").FirstOrDefault(x => x.ReportId == reportid);
                userdata.RemoveAll(x => entity.Notices.Any(y => y.UserId == x.UserId));

                foreach (var item in userdata)
                {
                    entity.Notices.Add(new ReportNotice()
                    {
                        NoticeId = Guid.NewGuid(),
                        IsRead = false,
                        UserId = item.UserId,
                        UserName = item.RealName,
                        NoticeType = 2
                    });
                }


                ctx.SaveChanges();
            }

            return userdata.Select(x => x.UserId).ToArray();
        }

        public List<ReportEntity> GetReportsByUser(string userId, int pagesize, int pageindex, out int total)
        {
            using (var ctx = new DataContext())
            {
                var query = ctx.Reports.AsQueryable();
                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(x => x.ReportUserId == userId);
                }
                var data = query.OrderByDescending(x => x.ReportTime).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
                total = query.Count();

                return data.Select(x => new ReportEntity() { ReportId = x.ReportId, ReportContent = x.ReportContent, ReportTime = x.ReportTime, ReportUser = x.ReportUser, ReportType = this.ToTypeString(x.ReportType), Tasks = x.Tasks, Plan = x.Plan, Undo = x.Undo, StartTime = x.StartTime, EndTime = x.EndTime }).ToList();
            }
        }

        public void Comment(CommentEntity data)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.Reports.Include("Comments").FirstOrDefault(x => x.ReportId == data.ReportId);
                if (entity == null) return;

                entity.Comments.Add(new Bst.Bzzd.DataSource.Entities.Comment()
                {
                    CommentId = Guid.NewGuid(),
                    Content = data.Content,
                    CommentUserId = data.CommentUserId,
                    CommentUser = data.CommentUser,
                    CommentTime = DateTime.Now
                });

                ctx.SaveChanges();
            }
        }

        public void Setting(List<ReportSettingEntity> list)
        {
            using (var ctx = new DataContext())
            {
                foreach (var item in list)
                {
                    var entity = ctx.ReportSettings.FirstOrDefault(x => x.SettingName == item.SettingName);
                    if (entity != null)
                    {
                        entity.Start = item.Start;
                        entity.End = item.End;
                        entity.StartTime = item.StartTime;
                        entity.EndTime = item.EndTime;
                    }
                }
                ctx.SaveChanges();
            }
        }

        public List<ReportSettingEntity> GetSettings()
        {
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.ReportSettings
                            select q;

                var data = query.ToList();
                if (data == null) return null;

                return data.Select(x => new ReportSettingEntity() { SettingId = x.SettingId, SettingName = x.SettingName, StartTime = x.StartTime, EndTime = x.EndTime, Start = x.Start, End = x.End }).ToList();
            }
        }

        public List<ItemEntity> GetSubmitPerson(string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(deptid) || deptid == "0")
                deptid = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.ParentId == "0").DepartmentId;

            var frist = from q in db.IQueryable<DepartmentEntity>()
                        where q.DepartmentId == deptid
                        select q.DepartmentId;
            if (frist.Count() == 0)
            {
                return new List<ItemEntity>();
            }
            var query = from q in db.IQueryable<DepartmentEntity>()
                        where q.ParentId == deptid
                        select q.DepartmentId;

            var query1 = from q in db.IQueryable<DepartmentEntity>()
                         join q1 in query on q.ParentId equals q1
                         select q.DepartmentId;


            while (query1.Count() > 0)
            {
                query = query.Concat(query1);

                query1 = from q in query1
                         join q1 in db.IQueryable<DepartmentEntity>() on q equals q1.ParentId
                         select q1.DepartmentId;
            }

            //var data = (from q in db.IQueryable<DepartmentEntity>()
            //            where query.Any(x => x == q.DepartmentId)
            //            select new { ItemId = q.DepartmentId, ItemName = q.FullName, ParentItemId = q.ParentId, ItemType = "dept" }).
            //           Concat(
            //          from q in db.IQueryable<UserEntity>()
            //          where query.Any(x => x == q.DepartmentId)
            //          select new { ItemId = q.UserId, ItemName = q.RealName, ParentItemId = q.DepartmentId, ItemType = "user" }
            //          )
            //          .Concat(from q in db.IQueryable<DepartmentEntity>()
            //                  where frist.Any(x => x == q.DepartmentId)
            //                  select new { ItemId = q.DepartmentId, ItemName = q.FullName, ParentItemId = "0", ItemType = "dept" })
            //          .Concat(
            //          from q in db.IQueryable<UserEntity>()
            //          where frist.Any(x => x == q.DepartmentId)
            //          select new { ItemId = q.UserId, ItemName = q.RealName, ParentItemId = q.DepartmentId, ItemType = "user" }
            //          ).ToList();
            var data = new List<ItemEntity>();
            var one = (from q in db.IQueryable<DepartmentEntity>()
                       where query.Any(x => x == q.DepartmentId)
                       select new ItemEntity() { ItemId = q.DepartmentId, ItemName = q.FullName, ParentItemId = q.ParentId, ItemType = "dept" }).ToList();
            var two = (from q in db.IQueryable<UserEntity>()
                       where query.Any(x => x == q.DepartmentId)
                       select new ItemEntity() { ItemId = q.UserId, ItemName = q.RealName, ParentItemId = q.DepartmentId, ItemType = "user" }).ToList();
            var three = (from q in db.IQueryable<DepartmentEntity>()
                         where frist.Any(x => x == q.DepartmentId)
                         select new ItemEntity() { ItemId = q.DepartmentId, ItemName = q.FullName, ParentItemId = "0", ItemType = "dept" }).ToList();
            var four = (from q in db.IQueryable<UserEntity>()
                        where frist.Any(x => x == q.DepartmentId)
                        select new ItemEntity() { ItemId = q.UserId, ItemName = q.RealName, ParentItemId = q.DepartmentId, ItemType = "user" }).ToList();
            data.AddRange(one);
            data.AddRange(two);
            data.AddRange(three);
            data.AddRange(four);


            var result = data.Select(x => new ItemEntity() { ItemId = x.ItemId, ItemName = x.ItemName, ItemType = x.ItemType, ParentItemId = x.ParentItemId }).ToList();
            return result;
        }

        public List<ReportEntity> GetAllReport(string userid, int pagesize, int pageindex, out int total)
        {
            using (var ctx = new DataContext())
            {
                var query = ctx.Reports.Include("Comments").AsQueryable();

                query = query.Where(x => x.ReportUserId == userid || x.Notices.Any(y => y.UserId == userid));

                var data = query.OrderByDescending(x => x.ReportTime).Skip(pagesize * (pageindex - 1)).Take(pagesize).Select(x => new { x.ReportId, x.ReportContent, x.ReportTime, x.ReportUser, x.ReportType, x.Tasks, x.Plan, x.Undo, x.Cantdo, x.StartTime, x.EndTime, ReadTotal = x.Notices.Count(y => y.IsRead), x.Comments, CommentsTotal = x.Comments.Count }).ToList();
                total = query.Count();

                return data.Select(x => new ReportEntity() { ReportId = x.ReportId, ReportContent = x.ReportContent, ReportTime = x.ReportTime, ReportUser = x.ReportUser, ReportType = this.ToTypeString(x.ReportType), Tasks = x.Tasks, Plan = x.Plan, Undo = x.Undo, Cantdo = x.Cantdo, StartTime = x.StartTime, EndTime = x.EndTime, ReadTotal = x.ReadTotal, CommentsTotal = x.CommentsTotal, Comments = x.Comments.Select(y => new CommentEntity() { CommentId = y.CommentId, Content = y.Content, CommentUserId = y.CommentUserId, CommentUser = y.CommentUser }).ToList() }).ToList();
            }
        }

        public void EditReport(ReportEntity data)
        {
            var reportset = _context.Set<ReportEntity>();
            var taskset = _context.Set<TaskEntity>();
            var entity = reportset.Find(data.ReportId);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.Undo = data.Undo;
                entity.Plan = data.Plan;
                entity.ReportContent = data.ReportContent;

                var tasks = taskset.Where(x => x.ReportId == data.ReportId).ToList();
                var newtasks = data.TaskList.Where(x => !tasks.Any(y => y.ReportTaskId == x.ReportTaskId)).ToList();
                taskset.AddRange(newtasks);
                foreach (var item in tasks)
                {
                    var task = data.TaskList.Find(x => x.ReportTaskId == item.ReportTaskId);
                    if (task == null) _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    else
                    {
                        _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        item.StartTime = task.StartTime;
                        item.EndTime = task.EndTime;
                        item.Photo = task.Photo;
                        item.TaskContent = task.TaskContent;
                        item.TaskPerson = task.TaskPerson;
                        item.TaskPersonId = task.TaskPersonId;
                    }
                }

                _context.SaveChanges();
            }
        }
    }
}
