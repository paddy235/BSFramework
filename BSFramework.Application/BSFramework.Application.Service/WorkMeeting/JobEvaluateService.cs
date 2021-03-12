using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WorkMeeting
{
    public class JobEvaluateService : RepositoryFactory<JobEvaluateEntity>, IJobEvaluateService
    {
        /// <summary>
        /// 判断工作任务是否有图片 
        /// </summary>
        /// <param name="jobId">工作任务Id</param>
        /// <returns></returns>
        public bool CheckPhoto(string jobId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<FileInfoEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>()
                        on q1.RecId equals q2.MeetingJobId
                        where q2.JobId == jobId
                        select q1;
            var value = query.Count();
            return value > 0;
        }

        public int CheckPhotoCount(string jobId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<FileInfoEntity>()
                        join q2 in db.IQueryable<MeetingAndJobEntity>()
                        on q1.RecId equals q2.MeetingJobId
                        where q2.JobId == jobId
                        select q1;
            var value = query.Count();
            return value;
        }

        public List<JobEvaluateEntity> GetAllByJobId(string jobId)
        {
            return BaseRepository().IQueryable(p => p.JobId == jobId).ToList();
        }

        public List<PeopleEntity> GetData1(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);

            IRepository db = new RepositoryFactory().BaseRepository();


            var query = from q5 in db.IQueryable<PeopleEntity>()
                        join q4 in db.IQueryable<JobUserEntity>() on q5.ID equals q4.UserId
                        join q2 in db.IQueryable<MeetingAndJobEntity>() on q4.MeetingJobId equals q2.MeetingJobId
                        join q3 in db.IQueryable<MeetingJobEntity>() on q2.JobId equals q3.JobId
                        join q6 in db.IQueryable<JobEvaluateEntity>() on q2.JobId equals q6.JobId into t
                        from c1 in t.DefaultIfEmpty()
                        where q5.BZID == deptid && q3.EndTime >= @from && q3.EndTime <= to && q3.IsFinished != "cancel"
                        group new { q5.ID, q5.Name, q2.MeetingJobId, q2.IsFinished, TotalScore = c1 == null ? 0 : c1.TotalScore, q5.Planer, q4.TaskHour } by new { q5.ID, q5.Name, q5.Planer, q5.Photo, } into g
                        //select new { g.Key.ID, g.Key.Name, g.Key.Planer, s1 = g.Count(x => x.IsFinished == "finish"), s2 = g.Sum(x => x.TotalScore), s3 = (decimal)g.Count(x => x.IsFinished == "finish") / (g.Count() == 0 ? 1 : g.Count()), TotalHour = g.Sum(x => x.TaskHour), g.Key.Photo };
                        select new { g.Key.ID, g.Key.Name, g.Key.Planer, s1 = g.Count(x => x.IsFinished == "finish"), s2 = g.Sum(x => x.TotalScore), num1 = g.Count(x => x.IsFinished == "finish"), num2 = (g.Count() == 0 ? 1 : g.Count()), TotalHour = g.Sum(x => x.TaskHour), g.Key.Photo };

            var data = query.OrderBy(x => x.Planer).ThenBy(x => x.Name).ToList();
            var list = data.Select(x => new PeopleEntity() { ID = x.ID, Name = x.Name, Jobs = x.s1.ToString(), Percent = ((decimal)x.num1 / (decimal)x.num2).ToString("p"), Scores = x.s2.ToString(), Age = x.TotalHour.HasValue ? x.TotalHour.ToString() : "0", Photo = x.Photo }).ToList();
            //var scoreQuery = from q2 in db.IQueryable<MeetingAndJobEntity>()
            //                 join q3 in db.IQueryable<MeetingJobEntity>() on q2.JobId equals q3.JobId
            //                 join q4 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q4.MeetingJobId
            //                 join q5 in db.IQueryable<PeopleEntity>() on q4.UserId equals q5.ID
            //                 join q6 in db.IQueryable<JobEvaluateEntity>() on q2.JobId equals q6.JobId 
            //                 where q5.BZID == deptid && q3.EndTime >= @from && q3.EndTime <= to && q3.IsFinished !="cancel"
            //                 group new { q5.ID,  q6.TotalScore } by new { q5.ID } into g
            //                 select new { g.Key.ID, TotalScore = g.Sum(x => x.TotalScore) };

            //var scoreList = scoreQuery.ToList();
            //list.ForEach(x => {
            //    var entity = scoreList.FirstOrDefault(p => p.ID == x.ID);
            //    if (entity != null)
            //        x.Scores = entity.TotalScore.ToString();
            //    else
            //        x.Scores = "0";
            //});
            var userIds = list.Select(p => p.ID).ToList();
            var peoepleList = db.IQueryable<PeopleEntity>().Where(x => x.BZID == deptid && !userIds.Contains(x.ID)).ToList();
            peoepleList.ForEach(p =>
            {
                p.Jobs = "0";
                p.Percent = 0.ToString("p"); p.Scores = 0.ToString(); p.Age = "0";
            });
            list.AddRange(peoepleList);

            return list;
        }

        /// <summary> 
        /// 获取任务的详情
        /// </summary>
        /// <param name="jobId">任务的ID</param>
        /// <returns></returns>
        public object GetJobDetail(string jobId)
        {
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            var db = new RepositoryFactory().BaseRepository();
            MeetingJobEntity entity = db.FindEntity<MeetingJobEntity>(jobId);
            var dangermeasureQuery = from q1 in db.IQueryable<JobDangerousEntity>()
                                     join q2 in db.IQueryable<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into1
                                     where q1.JobId == jobId
                                     select new
                                     {
                                         q1.Content,
                                         MeasureList = into1.Select(p => new { p.Content })
                                     };
            var dangermeasureList = dangermeasureQuery.ToList();
            if (entity == null)
                return null;
            var query = from q2 in db.IQueryable<MeetingAndJobEntity>()
                        join q3 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q3.MeetingJobId into into1

                        join q4 in db.IQueryable<DangerEntity>() on q2.MeetingJobId equals q4.JobId into into2
                        from q4 in into2.DefaultIfEmpty()
                        join q5 in db.IQueryable<FileInfoEntity>() on q2.MeetingJobId equals q5.RecId into into3
                        join q7 in db.IQueryable<WorkmeetingEntity>() on q2.StartMeetingId equals q7.MeetingId
                        where q2.JobId == entity.JobId
                        orderby q7.MeetingStartTime ascending
                        select new
                        {
                            q2.MeetingJobId,
                            UserList = into1.Select(p => new { p.UserName, p.JobType }).ToList(),
                            TrainingDone = q4 == null ? false : q4.Status == 2,
                            SoundFile = into3.Where(x => x.Description == "音频").Select(p => new { p.FileId, p.FileName, FilePath = p.FilePath != null ? p.FilePath.Replace("~/", url) : null }).ToList(),
                            PhotoFile = into3.Where(x => x.Description == "照片").Select(p => new { p.FileId, p.FileName, FilePath = p.FilePath != null ? p.FilePath.Replace("~/", url) : null }).ToList(),
                            JobDate = q7.MeetingStartTime
                        };
            var itemInfo = query.ToList();
            var data = new
            {
                MeetingJob =
                new
                {
                    entity.JobId,
                    entity.Job,
                    entity.StartTime,
                    entity.EndTime,
                    entity.TicketCode,
                    entity.Dangerous,
                    entity.Measure,
                    entity.Description,
                    DangerousList = dangermeasureList
                },
                ItemData = itemInfo
            };
            return data;
        }

        /// <summary>
        /// 获取任务的评分信息
        /// </summary>
        /// <param name="jobId">任务的Id</param>
        /// <returns></returns>
        public JobEvaluateEntity GetJobEvaluat(string jobId)
        {
            var jobEvaluate = this.BaseRepository().IQueryable(p => p.JobId == jobId).FirstOrDefault();
            return jobEvaluate;
        }

        public IList GetJobPageList(int pageIndex, int pageSize, string userId, string evaluateState, bool isMy, string startTime, string endTime, string deptId, string keyWord, out int total)
        {
            total = 0;
            // 部门级用户查看本部门,厂级用户 / 特殊部门用户看全厂,界面如下图: 有评分的,可点击评分查看评分详情
            var db = new RepositoryFactory().BaseRepository();
            var user = db.FindEntity<UserEntity>(userId);
            var userDept = db.FindEntity<DepartmentEntity>(user.DepartmentId);
            IQueryable<string> grouplist;//所有班组的集合

            #region 1-找出 当前用户所能看的班组
            //判断是否是特殊部门
            if (userDept.IsSpecial)
            {
                var dept = userDept;
                //特殊部门查全厂
                while (dept != null && dept.Nature != "厂级")
                {
                    dept = (from q in db.IQueryable<DepartmentEntity>()
                            where q.DepartmentId == dept.ParentId
                            select q).FirstOrDefault();
                }
                //查询所有的班组
                grouplist = db.IQueryable<DepartmentEntity>(p => p.EnCode.StartsWith(dept.EnCode) && p.Nature == "班组").Select(x => x.DepartmentId);
            }
            else
            {
                //不是特殊部门查询本部门底下的所有的班组
                grouplist = db.IQueryable<DepartmentEntity>(p => p.EnCode.StartsWith(userDept.EnCode) && p.Nature == "班组").Select(x => x.DepartmentId);
            }
            #endregion
            #region 2- 找出这些班组的班前班后会的jobId
            if (!string.IsNullOrWhiteSpace(deptId)) grouplist = grouplist.Where(p => p == deptId);
            var meetingId = db.IQueryable<WorkmeetingEntity>(p => grouplist.Contains(p.GroupId)).Select(p => p.MeetingId).ToList();
            var othertMeetingId = db.IQueryable<WorkmeetingEntity>(p => grouplist.Contains(p.GroupId)).Select(p => p.OtherMeetingId).ToList();
            var allMeetingId = meetingId.Concat(othertMeetingId).Where(p => !string.IsNullOrWhiteSpace(p)).ToList(); ;
            var jobid = db.IQueryable<MeetingAndJobEntity>(p => allMeetingId.Contains(p.StartMeetingId) || allMeetingId.Contains(p.EndMeetingId)).Select(p => p.JobId);
            #endregion
            #region 3- 拼接查询条件
            var query = db.IQueryable<MeetingJobEntity>(p => jobid.Contains(p.JobId) && p.IsFinished != "cancel");
            if (!string.IsNullOrWhiteSpace(evaluateState))
            {
                if (evaluateState == "已评分")
                    query = query.Where(p => p.EvaluateState == 1);
                else
                    query = query.Where(p => p.EvaluateState == 0);
            }
            if (isMy)
                query = query.Where(p => p.UserId.Contains(userId));
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime start;
                if (DateTime.TryParse(startTime, out start))
                {
                    query = query.Where(p => p.StartTime >= start);
                }
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime end;
                if (DateTime.TryParse(endTime, out end))
                {
                    end = end.AddDays(1).Date;
                    query = query.Where(p => p.StartTime <= end);
                }
            }
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                query = query.Where(p => p.Job.Contains(keyWord));
            }


            #endregion

            total = query.Count();
            var jobList = query.OrderByDescending(p => p.StartTime).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            var data = from q1 in jobList
                       join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q2.JobId
                       join q3 in db.IQueryable<DangerEntity>() on q2.MeetingJobId equals q3.JobId into into1
                       from t1 in into1.DefaultIfEmpty()
                       select new
                       {
                           q1.JobId,
                           q1.Job,
                           q1.IsFinished,
                           q1.StartTime,
                           q1.EndTime,
                           q1.EvaluateState,
                           q1.NeedTrain,
                           TrainingDone = t1 == null ? false : t1.Status == 2,

                       };
            //  UserList = into2.Select(x => new { x.UserName, x.JobType }),
            //   CanEvaluate = q1.EvaluateState==1 ? false : (into2.Any(p=>p.UserId==userId && p.JobType== "ischecker") || (user.RoleName=="班组长" || user.RoleName=="班长") ? true :false),

            var dataList = data.Distinct().ToList();
            //单独去查询一次人员信息
            var jobids = dataList.Select(x => x.JobId).ToList();
            var jobUserQuery = from q1 in db.IQueryable<MeetingAndJobEntity>()
                               join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                               where jobids.Contains(q1.JobId)
                               select new { q1.MeetingJobId, q1.JobId, q2 };
            var jobUsers = jobUserQuery.ToList();
            //组装分数与人员
            var evaluateQuery = from q1 in dataList
                                join q2 in db.IQueryable<JobEvaluateEntity>() on q1.JobId equals q2.JobId
                                select new
                                {
                                    q2.TotalScore,
                                    q2.CreateUserId,
                                    q2.JobId
                                };
            var evaluateList = evaluateQuery.ToList();
            //jobEvaluateUserIds = into3.Select(p => new { UserId = p.CreateUserId })
            //    Total = into3 == null ? 0 : into3.Sum(p => p.TotalScore),
            var deptUser = db.IQueryable<UserEntity>(p => p.DepartmentId == userDept.DepartmentId);
            var finaData = dataList.Select(x =>
            {
                var thisEvaluateList = evaluateList.Where(p => p.JobId == x.JobId);
                bool canevaluate = false;
                var evaluateUserIds = thisEvaluateList.Select(m => m.CreateUserId).ToList();
                var evaluateUsers = deptUser.Where(p => evaluateUserIds.Contains(p.UserId));
                //if (!evaluateUsers.Any(p => p.RoleName.Contains("班组长") || p.RoleName.Contains("班长")))//没有班组长评过
                //{
                //    canevaluate = x.EvaluateState == 1 ? false : (thisEvaluateList.Select(m => m.CreateUserId).Any(p => p == userId)) ? false : (jobUsers.Where(p => p.JobId == x.JobId).Any(p => p.q2.UserId == userId && p.q2.JobType == "ischecker") || (user.RoleName.Contains("班组长") || user.RoleName.Contains("班长")) ? true : false);
                //}

                if (x.EvaluateState != 1)//判断是否评分
                {
                    //未评分
                    if (!thisEvaluateList.Select(m => m.CreateUserId).Any(p => p == userId))//本人是否已评
                    {
                        //本人未评分
                        if (user.RoleName.Contains("班组长") || user.RoleName.Contains("班长"))//判断本人是否是班组长
                        {
                            //是班组长
                            if (!evaluateUsers.Any(p => p.RoleName.Contains("班组长") || p.RoleName.Contains("班长")))//其他班组长是否已经评论过
                            {
                                canevaluate = true;//其他班组长未评分,代表本人可评分
                            }
                        }
                        else
                        {
                            //不是班组长
                            if (jobUsers.Where(p => p.JobId == x.JobId).Any(p => p.q2.UserId == userId && p.q2.JobType == "ischecker"))//判断 本人是否是责任人
                            {
                                canevaluate = true;//本人是责任人而且未评分。则可评分  
                            }
                        }
                    }

                }


                return new
                {
                    x.JobId,
                    x.Job,
                    x.IsFinished,
                    x.StartTime,
                    x.EndTime,
                    x.EvaluateState,
                    x.NeedTrain,
                    x.TrainingDone,
                    Total = thisEvaluateList.Sum(m => m.TotalScore),
                    AnyEvaluate = evaluateUsers.Count() > 0,
                    MeetingUsers = jobUsers.Where(p => p.JobId == x.JobId).Select(m => new { m.MeetingJobId, UserList = new { m.q2.UserName, m.q2.JobType } }),
                    CanEvaluate = canevaluate
                };
            }).ToList();
            return finaData;
        }

        public List<UserScoreEntity> GetPersonScore(string deptId, int year, int month)
        {
            var db = new RepositoryFactory().BaseRepository();
            IQueryable<string> grouplist;//所有班组的集合

            #region 2- 找出这些班组的班前班后会的jobId
            var meetingId = db.IQueryable<WorkmeetingEntity>(p => deptId == p.GroupId).Select(p => new { p.MeetingId, p.OtherMeetingId }).ToList();
            var allMeetingId = meetingId.Select(p => p.MeetingId).ToList();
            allMeetingId.AddRange(meetingId.Select(p => p.OtherMeetingId));
            var meetingAndJobQuery = db.IQueryable<MeetingAndJobEntity>(p => allMeetingId.Contains(p.StartMeetingId) || allMeetingId.Contains(p.EndMeetingId));
            #endregion
            DateTime startTime = new DateTime(year, month, 1);
            DateTime endTime = startTime.AddMonths(1);
            var meetingJobQuery = db.IQueryable<MeetingJobEntity>(p => p.EndTime >= startTime && p.EndTime < endTime && p.IsFinished != "cancel");

            var linq = from q1 in meetingAndJobQuery
                       join q2 in meetingJobQuery on q1.JobId equals q2.JobId
                       join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId into t1
                       from c1 in t1.DefaultIfEmpty()
                       join q4 in db.IQueryable<JobEvaluateEntity>() on q1.JobId equals q4.JobId
                       select new { c1.UserId, c1.UserName, q4.TotalScore };


            var linqList = linq.ToList();
            var data = linqList.GroupBy(p => new { p.UserId, p.UserName }).Select(p => new UserScoreEntity() { UserName = p.Key.UserName, Score = p.Sum(x => x.TotalScore), userid = p.Key.UserId, total = p.Count() }).ToList();
            return data;
        }


        /// <summary>
        /// 获取个人当前年 的任务得分与平均得分
        /// </summary>
        /// <param name="userKey"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<KeyValue> GetPersonScore(string userKey, string year)
        {
            //            var list = new KeyValue().InitData(1);
            //            string sql = $@"SELECT  CASE MONTH(D.ENDTIME)
            //                                        WHEN 1 THEN '1月'
            //                                        WHEN 2 THEN '2月'
            //                                        WHEN 3 THEN '3月'
            //                                        WHEN 4 THEN '4月'
            //                                        WHEN 5 THEN '5月'
            //                                        WHEN 6 THEN '6月'
            //                                        WHEN 7 THEN '7月'
            //                                        WHEN 8 THEN '8月'
            //                                        WHEN 9 THEN '9月'
            //                                        WHEN 10 THEN '10月'
            //                                        WHEN 11 THEN '11月'
            //                                        WHEN 12 THEN '12月'
            //                                        END AS MONTH,SUM(IFNULL(C.TOTALSCORE,0)) AS TOTALSCORE,AVG(IFNULL(C.TOTALSCORE,0)) AS AVGSCORE, MONTH(D.ENDTIME) AS SORT
            //                                        FROM  WG_MEETINGANDJOB A 
            //	                                    LEFT JOIN WG_MEETINGJOB D ON A.JOBID=D.JOBID
            //                                        LEFT JOIN WG_JOBUSER B ON A.MEETINGJOBID=B.MEETINGJOBID
            //                                        LEFT JOIN WG_JOBEVALUATE C ON A.JOBID = C.JOBID
            //                                        WHERE B
            //.USERID='{userKey}' AND YEAR(D.ENDTIME)='{year}'  and D.ISFINISHED='cancel'
            //                                        GROUP BY `MONTH`
            //                                        ORDER BY `SORT` ASC";
            //            DataTable dt = BaseRepository().FindTable(sql);
            //            if (dt.Rows != null && dt.Rows.Count > 0)
            //            {
            //                var enumertors = dt.Rows.GetEnumerator();
            //                while (enumertors.MoveNext())
            //                {
            //                    DataRow dr = enumertors.Current as DataRow;
            //                    var keyStr = dr["MONTH"]?.ToString();
            //                    var kv = list.FirstOrDefault(x => x.Key == keyStr);
            //                    if (kv != null)
            //                    {
            //                        kv.Value = dr["TOTALSCORE"] == null || dr["TOTALSCORE"] is DBNull ? 0 : Convert.ToDecimal(dr["TOTALSCORE"]);
            //                        kv.Num1 = dr["AVGSCORE"] == null || dr["AVGSCORE"] is DBNull ? 0 : Convert.ToDecimal(dr["AVGSCORE"]);
            //                    }
            //                }
            //            }
            //            #region 个人工时统计
            //            int month = 0;//月份列表 时间默认是从一月份开始的
            //            list.ForEach(p => {
            //                month++;
            //                DateTime startTime = new DateTime(int.Parse(year), month, 1);
            //                DateTime endTime = startTime.AddMonths(1);
            //                var hours = new RepositoryFactory().BaseRepository().IQueryable<JobUserEntity>(x => x.UserId == userKey && x.CreateDate >= startTime && x.CreateDate < endTime).Sum(x => x.TaskHour);
            //                if (!hours.HasValue) hours = 0;
            //                p.Num2 = hours.Value;
            //            });
            //            #endregion
            //            return list;


            var db = new RepositoryFactory().BaseRepository();
            //var meetingJobQuery = db.IQueryable<MeetingJobEntity>(p => p.EndTime >= startTime && p.EndTime < endTime && p.IsFinished != "cancel");
            var searchYear = int.Parse(year);
            var linq = from q2 in db.IQueryable<MeetingAndJobEntity>()
                       join q3 in db.IQueryable<MeetingJobEntity>() on q2.JobId equals q3.JobId
                       join q4 in db.IQueryable<JobUserEntity>() on q2.MeetingJobId equals q4.MeetingJobId
                       join q6 in db.IQueryable<JobEvaluateEntity>() on q2.JobId equals q6.JobId into t
                       from c1 in t.DefaultIfEmpty()
                       where q4.UserId == userKey && q3.IsFinished != "cancel" && q3.CreateDate.Year == searchYear
                       select new { q4.UserId, q4.UserName, TotalScore = c1 == null ? 0 : c1.TotalScore, q4.TaskHour, q3.CreateDate };


            var linqList = linq.ToList();
            var query = linqList.GroupBy(x => new { Month = x.CreateDate.Month, x.UserId, x.UserName }).Select(x => new
            {
                x.Key.UserId,
                x.Key.UserName,
                TotalScore = x.Sum(p => p.TotalScore),
                AvgScore = x.Average(p => p.TotalScore),
                TaskHour = x.Sum(p => p.TaskHour),
                Month = x.Key.Month
            }).ToList();

            var kvList = new KeyValue().InitData(1);
            kvList.ForEach(x =>
            {
                var data = query.FirstOrDefault(p => p.Month == Convert.ToInt32(x.Key.Substring(0, 1)));
                x.Value = data?.TotalScore ?? 0;
                x.Num1 = data == null ? 0 : Convert.ToDecimal(data.AvgScore);
                x.Num2 = data?.TaskHour ?? 0;
            });
            return kvList;
        }


        /// <summary>
        /// 返回任务总分与成员平均分
        /// TotalScore  总分
        /// AvgScore 平均分
        /// </summary>
        /// <param name="deptId">部门的Id</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public Hashtable GetTotalScore(string deptId, int year, int month)
        {
            var db = new RepositoryFactory().BaseRepository();
            Hashtable ht = new Hashtable();
            DateTime startTime = new DateTime(year, month, 1);
            DateTime endTime = startTime.AddMonths(1);

            var meetingIds = db.IQueryable<WorkmeetingEntity>(p => p.GroupId == deptId).Select(p => new { p.MeetingId, p.OtherMeetingId });
            var where1 = meetingIds.Select(x => x.MeetingId);
            var where2 = meetingIds.Select(x => x.OtherMeetingId);

            var query = from q1 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<MeetingJobEntity>() on q1.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<JobEvaluateEntity>() on q1.JobId equals q4.JobId into t
                        from t1 in t.DefaultIfEmpty()
                        where q2.EndTime >= startTime && q2.EndTime < endTime && (where1.Contains(q1.StartMeetingId) || where1.Contains(q1.EndMeetingId) || where2.Contains
                        (q1.StartMeetingId) || where2.Contains(q1.EndMeetingId))
                        select new { q3.JobUserId, q1.JobId, TotalScore = t1 != null ? t1.TotalScore : 0 } into s
                        group s.JobUserId by new { s.JobId, s.TotalScore } into g
                        select new
                        {
                            count = g.Count(),
                            g.Key.TotalScore

                        };

            var Score = query.Sum(x => x.count * x.TotalScore);
            var avgScore = (decimal)Score / query.Sum(x => x.count);
            avgScore = Math.Round(avgScore, 2);
            ht.Add("TotalScore", Score);
            ht.Add("AvgScore", avgScore);
            //string wher1 = $"SELECT MEETINGID FROM WG_WORKMEETING WHERE GROUPID='{deptId}'";
            //string wher2 = $"SELECT OTHERMEETINGID FROM WG_WORKMEETING WHERE GROUPID='{deptId}'";
            //string sql = $@"SELECT SUM(T.TOTALSCORE*T.USERCOUNT) AS SCORE, ROUND(SUM(T.TOTALSCORE*T.USERCOUNT)/SUM(USERCOUNT),2)  AS AVGSCORE FROM ( SELECT C.TOTALSCORE AS TOTALSCORE,COUNT(B.JOBUSERID) AS USERCOUNT  FROM WG_MEETINGANDJOB A 
            //                         LEFT JOIN WG_MEETINGJOB D ON A.JOBID=D.JOBID
            //                            LEFT JOIN WG_JOBUSER B ON A.MEETINGJOBID=B.MEETINGJOBID
            //                            LEFT JOIN WG_JOBEVALUATE C ON A.JOBID = C.JOBID
            //                            WHERE D.ENDTIME>='{startTime.ToString("yyyy-MM-dd")}' AND D.ENDTIME<'{endTime.ToString("yyyy-MM-dd")}' AND
            //                            (A.STARTMEETINGID IN (	 {wher1}) 
            //                            OR A.ENDMEETINGID IN (	{wher1})
            //                            or 	A.STARTMEETINGID IN ({wher2}) 
            //                            OR A.ENDMEETINGID IN ({wher2}))
            //                            GROUP BY A.JOBID,C.TotalScore	) T		";
            //DataTable dt = db.FindTable(sql);
            //if (dt.Rows != null && dt.Rows.Count > 0)
            //{
            //    int totalScore = dt.Rows[0]["SCORE"] == null || dt.Rows[0]["SCORE"] is DBNull ? 0 : Convert.ToInt32(dt.Rows[0]["SCORE"]);
            //    decimal avg = dt.Rows[0]["AVGSCORE"] == null || dt.Rows[0]["AVGSCORE"] is DBNull ? 0 : Convert.ToDecimal(dt.Rows[0]["AVGSCORE"]);
            //    ht.Add("TotalScore", totalScore);
            //    ht.Add("AvgScore", avg);
            //}
            //else
            //{
            //    ht.Add("TotalScore", 0);
            //    ht.Add("AvgScore", 0);
            //}
            return ht;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(JobEvaluateEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var meetingjob = db.FindEntity<MeetingJobEntity>(entity.JobId);
                var count = db.FindList<JobEvaluateEntity>(p => p.JobId == entity.JobId).Count();
                var jobUserQuery = from q1 in db.IQueryable<MeetingAndJobEntity>()
                                   join q2 in db.IQueryable<JobUserEntity>() on q1.MeetingJobId equals q2.MeetingJobId
                                   where q1.JobId == entity.JobId
                                   select new { q2.UserId, q2.JobType };
                var jobuser = jobUserQuery.ToList();
                var user = db.FindEntity<UserEntity>(entity.CreateUserId);
                bool evaluetOver = jobuser.Any(p => p.UserId == user.UserId && p.JobType == "ischecker") && (user.RoleName.Contains("班组长") || user.RoleName.Contains("班长"));//如果既是任务负责人又是班组长，则不能再评了
                if (count > 0 || evaluetOver)
                {
                    meetingjob.EvaluateState = 1;
                    db.Update(meetingjob);
                }
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 判断用户是否是工作负责人
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="jobId">工作任务的ID</param>
        /// <returns></returns>
        public bool IsChecker(string userId, string jobId)
        {
            var meetingJobs = new RepositoryFactory().BaseRepository().IQueryable<MeetingAndJobEntity>(p => p.JobId == jobId).Select(x => x.MeetingJobId).ToList();
            var IsChecker = new RepositoryFactory().BaseRepository().IQueryable<JobUserEntity>()
                .Any(p => p.UserId == userId && meetingJobs.Contains(p.MeetingJobId) && p.JobType == "ischecker");
            return IsChecker;
        }

        public List<MeetingJobEntity> PersonJobsObject(string userKey, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddMinutes(-1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q4 in db.IQueryable<MeetingAndJobEntity>()
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        join q5 in db.IQueryable<JobEvaluateEntity>() on q4.JobId equals q5.JobId
                        where q2.EndTime >= @from && q2.EndTime <= to && q3.UserId == userKey && q4.IsFinished != "cancel"
                        select new { q2.JobId, q2.Job, /*q2.JobUsers,*/ q2.Measure, q2.Remark, q4.StartMeetingId, q2.StartTime, /*q2.UserId, */q2.CreateDate, q2.Dangerous, q2.Description, q4.EndMeetingId, q2.EndTime, q4.IsFinished, Score = q5.TotalScore, q4 };
            var data = query.OrderByDescending(x => x.CreateDate).ToList().Select(x => new MeetingJobEntity() { JobId = x.JobId, Job = x.Job, Measure = x.Measure, Remark = x.Remark, StartTime = x.StartTime, /*UserId = x.UserId, JobUsers = x.JobUsers, */CreateDate = x.CreateDate, Dangerous = x.Dangerous, Description = x.Description, EndTime = x.EndTime, IsFinished = x.IsFinished, JobUsers = x.q4.JobUser, Score = x.Score.ToString(), Relation = new MeetingAndJobEntity() { MeetingJobId = x.q4.MeetingJobId } }).ToList();

            //return query.OrderByDescending(x=>x.CreateDate).ToList();
            return data;
        }

        /// <summary>
        /// 月度数据统计  平均完成任务数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetAvgTaskCount(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);

            IRepository db = new RepositoryFactory().BaseRepository();

            var usercount = db.IQueryable<UserEntity>().Count(x => x.DepartmentId == deptid);

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q4 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q4.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q4.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q4.MeetingJobId equals q3.MeetingJobId
                        where q2.EndTime > @from && q2.EndTime < to && q1.GroupId == deptid && q2.IsFinished == "finish"
                        select q3;

            return Math.Round((decimal)query.Count() / usercount, 1);
        }

        /// <summary>
        /// 月度数据统计  完成任务总数
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetFinishTaskCount(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q2.EndTime > @from && q2.EndTime < to && q1.GroupId == deptid && q2.IsFinished == "finish"
                        select q2;

            return query.Count();
        }

        /// <summary>
        /// 月度数据统计 未完成任务数量
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetUnfinishTaskCount(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q2.EndTime > @from && q2.EndTime < to && q1.GroupId == deptid && q2.IsFinished == "undo"
                        select q2;

            return query.Count();
        }


        /// <summary>
        /// 月度数据统计 班组任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetScore4(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q3.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q2.JobId
                        where q2.EndTime > @from && q2.EndTime < to && q1.GroupId == deptid
                        select q2;
            if (query.Count() > 0)
            {
                return (decimal)query.Count(x => x.IsFinished == "finish") / (query.Count() == 0 ? 1 : query.Count());
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 月度数据统计  人员任务完成率
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<UserScoreEntity> GetScore2(string deptid, int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = new DateTime(year, month, 1).AddMonths(1);
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<WorkmeetingEntity>()
                        join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
                        join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
                        join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId
                        join q4 in db.IQueryable<UserEntity>() on q3.UserId equals q4.UserId
                        where q2.EndTime > @from && q2.EndTime < to && q4.DepartmentId == deptid
                        select new { q2.IsFinished, q3.UserId };

            var query2 = from q1 in db.IQueryable<UserEntity>()
                         join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID// into into2
                         join q2 in query on q1.UserId equals q2.UserId into into1
                         //from d1 in into2.DefaultIfEmpty()
                         where q1.DepartmentId == deptid
                         //select new { q1.RealName, Sort = /*d1 == null ? "99" :*/ q3.Planer, Score = (decimal)into1.Count(x => x.IsFinished == "finish") / (into1.Count() == 0 ? 1 : into1.Count()) };
                         select new { q1.RealName, Sort = /*d1 == null ? "99" :*/ q3.Planer, num1 = into1.Count(x => x.IsFinished == "finish"), num2 = into1.Count() == 0 ? 1 : into1.Count() };

            //var querys = from q1 in db.IQueryable<UserEntity>()
            //             from q1 in db.IQueryable<WorkmeetingEntity>()
            //             join q5 in db.IQueryable<MeetingAndJobEntity>() on q1.MeetingId equals q5.EndMeetingId
            //             join q2 in db.IQueryable<MeetingJobEntity>() on q5.JobId equals q2.JobId
            //             join q3 in db.IQueryable<JobUserEntity>() on q5.MeetingJobId equals q3.MeetingJobId

            return query2.OrderBy(x => x.Sort).ThenBy(x => x.RealName).ToList().Select(x => new UserScoreEntity() { UserName = x.RealName, Score = Math.Round((decimal)x.num1 / (decimal)x.num2, 4) * 100 }).ToList();
        }
    }
}
