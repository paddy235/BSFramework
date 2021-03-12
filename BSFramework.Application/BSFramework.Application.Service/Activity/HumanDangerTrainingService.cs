using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.Activity;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class HumanDangerTrainingService : IHumanDangerTrainingService
    {
        private System.Data.Entity.DbContext _context;

        public HumanDangerTrainingService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }



        public void Add(HumanDangerTrainingEntity model)
        {
            //IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            //var entity = new HumanDangerTrainingBaseEntity()
            //{
            //    TrainingId = model.TrainingId,
            //    TrainingTask = model.TrainingTask,
            //    CreateTime = model.CreateTime,
            //    HumanDangerId = model.HumanDangerId,
            //    CreateUserId = model.CreateUserId,
            //    DeptId = model.DeptId,
            //    DeptName = model.DeptName,
            //    MeetingJobId = model.MeetingJobId
            //};
            //entity.TrainingUsers = model.TrainingUsers.Select(x => new HumanDangerTrainingUserEntity() { TrainingUserId = x.TrainingUserId, UserId = x.UserId, UserName = x.UserName, TrainingRole = x.TrainingRole, No = model.No, TicketId = model.TicketId, TrainingPlace = model.TrainingPlace, Status = 0 }).ToList();

            //if (model.HumanDangerId != null)
            //{
            //    var hm = (from q in db.IQueryable<HumanDangerEntity>()
            //              where q.HumanDangerId == model.HumanDangerId
            //              select q);
            //    if (hm == null) entity.HumanDangerId = null;
            //}

            //try
            //{
            //    db.Insert(entity);
            //    db.Insert(entity.TrainingUsers);

            //    db.Commit();
            //}
            //catch (Exception)
            //{
            //    db.Rollback();
            //    throw;
            //}


            using (var ctx = new DataContext())
            {
                var entity = new HumanDangerTraining()
                {
                    TrainingId = model.TrainingId,
                    TrainingTask = model.TrainingTask,
                    CreateTime = model.CreateTime,
                    HumanDangerId = model.HumanDangerId,
                    CreateUserId = model.CreateUserId,
                    DeptId = model.DeptId,
                    DeptName = model.DeptName,
                    MeetingJobId = model.MeetingJobId,
                    TrainingUsers = model.TrainingUsers.Select(x => new HumanDangerTrainingUser() { TrainingUserId = x.TrainingUserId, UserId = x.UserId, UserName = x.UserName, TrainingRole = x.TrainingRole, No = model.No, TicketId = model.TicketId, TrainingPlace = model.TrainingPlace, Status = 0 }).ToList()
                };
                if (model.HumanDangerId != null)
                {
                    var hm = ctx.HumanDangers.Find(model.HumanDangerId);
                    if (hm == null) entity.HumanDangerId = null;
                }
                ctx.HumanDangerTrainings.Add(entity);

                ctx.SaveChanges();
            }
        }

        public void Delete(string id)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangerTrainingUsers.Find(id);
                if (ctx.HumanDangerTrainingUsers.Count(x => x.TrainingId == entity.TrainingId && x.IsDone == true && x.IsMarked == true) > 0)
                {
                    //var trainingitems = ctx.HumanDangerTrainingUsers.Include("Training").Where(x => x.TrainingId == entity.TrainingId).ToList();
                    var training = ctx.HumanDangerTrainings.Find(entity.TrainingId);
                    var trainingitems = ctx.HumanDangerTrainingUsers.Where(x => x.TrainingId == entity.TrainingId).ToList();
                    foreach (var item in trainingitems)
                    {
                        if (item.IsDone == true && item.IsMarked == true)
                            item.Status = 1;
                        else
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
                            ctx.HumanDangerTrainingUsers.Remove(item);
                        }
                    }
                }
                else
                {
                    var s = ctx.HumanDangerTrainings.Include("TrainingUsers").FirstOrDefault(x => x.TrainingId == entity.TrainingId);
                    foreach (var item in s.TrainingUsers)
                    {
                        var bid = item.TrainingUserId.ToString();
                        var todos = ctx.Messages.Where(x => x.MessageKey == "人身风险预控" && x.BusinessId == bid).ToList();
                        todos.ForEach(x => x.IsFinished = true);

                        ctx.Messages.Add(new Message()
                        {
                            MessageId = Guid.NewGuid(),
                            BusinessId = bid,
                            Content = string.Format("您有条人身风险预控任务已取消：{0}", s.TrainingTask),
                            Title = "人身风险预控取消",
                            UserId = item.UserId,
                            Category = MessageCategory.Message,
                            MessageKey = "人身风险预控取消",
                            CreateTime = DateTime.Now
                        });
                    }
                    ctx.HumanDangerTrainings.Remove(s);
                }

                ctx.SaveChanges();
            }
        }

        public HumanDangerTrainingEntity GetDetail(string id)
        {
            var result = default(HumanDangerTrainingEntity);

            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        join q3 in db.IQueryable<HumanDangerTrainingMeasureEntity>() on q1.TrainingUserId equals q3.TrainingUserId into into3
                        join q4 in db.IQueryable<HumanDangerTrainingTypeEntity>() on q1.TrainingUserId equals q4.TrainingUserId into into4
                        join q5 in db.IQueryable<ActivityEvaluateEntity>() on q1.TrainingUserId.ToString() equals q5.Activityid into into5
                        join q6 in db.IQueryable<HumanDangerTrainingUserEntity>() on q2.TrainingId equals q6.TrainingId into into6
                        where q1.TrainingUserId == id
                        select new { q1, q2, q3 = into3, q4 = into4, q5 = into5, q6 = into6 };
            var data = query.FirstOrDefault();
            result = new HumanDangerTrainingEntity()
            {
                TrainingId = data.q1.TrainingUserId,
                TrainingTask = data.q2.TrainingTask,
                TrainingPlace = data.q1.TrainingPlace,
                No = data.q1.No,
                TicketId = data.q1.TicketId,
                CreateTime = data.q2.CreateTime,
                CreateUserId = data.q2.CreateUserId,
                HumanDangerId = data.q2.HumanDangerId,
                IsMarked = data.q1.IsMarked,
                IsDone = data.q1.IsDone,
                DangerLevel = data.q1.DangerLevel,
                OtherMeasure = data.q1.OtherMeasure,
                UserId = data.q1.UserId,
                UserName = data.q1.UserName,
                TrainingTime = data.q1.TrainingTime,
                TrainingUsers = data.q6.OrderBy(x => x.TrainingRole).ThenBy(x => x.UserName).Select(x => new TrainingUserEntity() { TrainingUserId = x.TrainingUserId, UserId = x.UserId, UserName = x.UserName, TrainingRole = x.TrainingRole }).ToList(),
                Measures = data.q3.Select(x => new TrainingMeasureEntity() { TrainingMeasureId = x.TrainingMeasureId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, Standard = x.Standard, MeasureId = x.MeasureId, State = x.State, CategoryId = x.CategoryId, Category = x.Category }).ToList(),
                TaskTypes = data.q4.Select(x => new TaskTypeEntity() { TaskTypeId = x.TaskTypeId, TaskTypeName = x.TypeName, State = x.State }).ToList(),
                Evaluates = data.q5.ToList()
            };

            var categories = (from q in db.IQueryable<DangerCategoryEntity>()
                              select q).ToList();
            result.Measures = (from q1 in categories
                               join q2 in result.Measures on q1.CategoryId equals q2.CategoryId into t2
                               from q2 in t2.DefaultIfEmpty(new TrainingMeasureEntity { DangerReason = "无" })
                               orderby q1.Sort
                               select new TrainingMeasureEntity { TrainingMeasureId = q2.TrainingMeasureId, CategoryId = q1.CategoryId, Category = q1.CategoryName, DangerReason = q2.DangerReason, MeasureContent = q2.MeasureContent, MeasureId = q2.MeasureId, Standard = q2.Standard, State = q2.State }).ToList();




            //using (var ctx = new DataContext())
            //{
            //    var entity = ctx.HumanDangerTrainingUsers.Include("Training").Include("Training.TrainingUsers").Include("TrainingMeasures").Include("TrainingTypes").FirstOrDefault(x => x.TrainingUserId == id);
            //    result = new HumanDangerTrainingEntity()
            //    {
            //        TrainingId = entity.TrainingUserId,
            //        TrainingTask = entity.Training.TrainingTask,
            //        TrainingPlace = entity.TrainingPlace,
            //        No = entity.No,
            //        CreateTime = entity.Training.CreateTime,
            //        CreateUserId = entity.Training.CreateUserId,
            //        HumanDangerId = entity.Training.HumanDangerId,
            //        IsMarked = entity.IsMarked,
            //        IsDone = entity.IsDone,
            //        DangerLevel = entity.DangerLevel,
            //        OtherMeasure = entity.OtherMeasure,
            //        UserId = entity.UserId,
            //        UserName = entity.UserName,
            //        TrainingTime = entity.TrainingTime,
            //        TrainingUsers = entity.Training.TrainingUsers.OrderBy(x => x.TrainingRole).ThenBy(x => x.UserName).Select(x => new TrainingUserEntity() { TrainingUserId = x.TrainingUserId, UserId = x.UserId, UserName = x.UserName, TrainingRole = x.TrainingRole }).ToList(),
            //        Measures = entity.TrainingMeasures.Select(x => new TrainingMeasureEntity() { TrainingMeasureId = x.TrainingMeasureId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, Standard = x.Standard, MeasureId = x.MeasureId, State = x.State, CategoryId = x.CategoryId, Category = x.Category }).ToList(),
            //        TaskTypes = entity.TrainingTypes.Select(x => new TaskTypeEntity() { TaskTypeId = x.TaskTypeId, TaskTypeName = x.TypeName, State = x.State }).ToList()
            //    };

            //    var categories = ctx.DangerCategories.ToList();
            //result.Measures = (from q1 in categories
            //                   join q2 in result.Measures on q1.CategoryId equals q2.CategoryId into t2
            //                   from q2 in t2.DefaultIfEmpty(new TrainingMeasureEntity { DangerReason = "无" })
            //                   orderby q1.Sort
            //                   select new TrainingMeasureEntity { TrainingMeasureId = q2.TrainingMeasureId, CategoryId = q1.CategoryId, Category = q1.CategoryName, DangerReason = q2.DangerReason, MeasureContent = q2.MeasureContent, MeasureId = q2.MeasureId, Standard = q2.Standard, State = q2.State }).ToList();
            //}

            //if (result != null)
            //{
            //    var sid = id.ToString();
            //    IRepository db = new RepositoryFactory().BaseRepository();
            //    var evaluates = (from q in db.IQueryable<ActivityEvaluateEntity>()
            //                     where q.Activityid == sid
            //                     select q).ToList();
            //    result.Evaluates = evaluates;
            //}
            return result;
        }

        public List<HumanDangerTrainingEntity> GetList(string userid, string[] users, string[] duty, DateTime? from, DateTime? to, string status, string level, string evaluatestatus, int pageSize, int pageIndex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q2.MeetingJobId equals q3.MeetingJobId into t3
                        from q3 in t3.DefaultIfEmpty()
                        join q4 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q4.JobId into t4
                        from q4 in t4.DefaultIfEmpty()
                        join q5 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q5.MeetingId into t5
                        from q5 in t5.DefaultIfEmpty()
                        join q6 in db.IQueryable<ActivityEvaluateEntity>() on q1.TrainingUserId.ToString() equals q6.Activityid into t6
                        where (q1.IsDone == true && q1.IsMarked == true) || (!(q1.IsDone == true && q1.IsMarked == true) && q5 != null && q5.IsOver == true)
                        orderby q2.CreateTime descending
                        select new { q1, q2, q3, q4, q5, q6 = t6 };

            if (users == null || users.Length == 0) query = query.Where(x => x.q1.UserId == userid);
            else query = query.Where(x => users.Contains(x.q1.UserId));

            if (duty != null && duty.Length > 0) query = query.Where(x => duty.Contains(x.q1.UserId) && x.q1.TrainingRole == 1);

            if (from != null) query = query.Where(x => x.q2.CreateTime >= from);

            if (to != null) query = query.Where(x => x.q2.CreateTime <= to);

            switch (status)
            {
                case "已完成":
                    query = query.Where(x => x.q1.IsDone == true && x.q1.IsMarked == true);
                    break;
                case "逾期未开展":
                    query = query.Where(x => x.q5.IsOver == true && x.q1.IsDone == false);
                    break;
                case "工作任务已取消":
                    query = query.Where(x => x.q4.IsFinished == "cancel");
                    break;
                case "风险预控任务已取消":
                    query = query.Where(x => x.q1.Status == 1);
                    break;
                default:
                    break;
            }

            if (evaluatestatus == "已评")
            {
                switch (level)
                {
                    case "本人":
                        query = query.Where(x => x.q6.Count(y => y.EvaluateId == userid) > 0);
                        break;
                    case "班组":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) > 0);
                        break;
                    case "部门":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) > 0);
                        break;
                    case "厂级":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) > 0);
                        break;
                    default:
                        query = query.Where(x => x.q6.Count() > 0);
                        break;
                }
            }
            else if (evaluatestatus == "未评")
            {
                switch (level)
                {
                    case "本人":
                        query = query.Where(x => x.q6.Count(y => y.EvaluateId == userid) == 0);
                        break;
                    case "班组":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) == 0);
                        break;
                    case "部门":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) == 0);
                        break;
                    case "厂级":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) == 0);
                        break;
                    default:
                        query = query.Where(x => x.q6.Count() == 0);
                        break;
                }
            }

            total = query.Count();
            var result = new List<HumanDangerTrainingEntity>();

            var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            foreach (var item in data)
            {
                var t = new HumanDangerTrainingEntity() { TrainingId = item.q1.TrainingUserId, HumanDangerId = item.q2.HumanDangerId, TrainingTask = item.q2.TrainingTask, IsDone = item.q1.IsDone, IsMarked = item.q1.IsMarked, CreateTime = item.q2.CreateTime, CreateUserId = item.q2.CreateUserId, UserId = item.q1.UserId, UserName = item.q1.UserName, IsEvaluate = item.q6.Any(x => x.EvaluateId == userid), TrainingTime = item.q1.TrainingTime };
                //if (item.q1.IsDone == true && item.q1.IsMarked == true) t.Status = "已完成";
                //else if (item.q5.IsOver == true) t.Status = "逾期未开展";
                //else if (item.q4.IsFinished == "cancel") t.Status = "工作任务已取消";
                //else if (item.q1.Status == 0) t.Status = "风险预控任务已取消";

                if (item.q1.IsDone == true && item.q1.IsMarked == true)
                {
                    if (item.q4 != null && item.q4.IsFinished == "cancel") t.Status = "工作任务已取消";
                    else if (item.q1.Status == 1) t.Status = "风险预控任务已取消";
                    else t.Status = "已完成";
                }
                else
                {
                    if (item.q5 != null && item.q5.IsOver == true) t.Status = "逾期未开展";
                }

                result.Add(t);
            }

            return result;
        }

        public void SaveMeasures(HumanDangerTrainingEntity model)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangerTrainingUsers.Include("TrainingMeasures").FirstOrDefault(x => x.TrainingUserId == model.TrainingId);
                foreach (var item in entity.TrainingMeasures)
                {
                    var measure = model.Measures.Find(x => x.TrainingMeasureId == item.TrainingMeasureId);
                    if (measure != null) item.MeasureContent = measure.MeasureContent;
                }

                ctx.SaveChanges();
            }
        }

        public void SaveReasons(HumanDangerTrainingEntity model)
        {
            //IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            //var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
            //            join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
            //            join q3 in db.IQueryable<HumanDangerTrainingMeasureEntity>() on q1.TrainingUserId equals q3.TrainingUserId into into3
            //            join q4 in db.IQueryable<HumanDangerTrainingTypeEntity>() on q1.TrainingUserId equals q4.TrainingUserId into into4
            //            where q1.TrainingUserId == model.TrainingId
            //            select new { q1, q2, q3 = into3, q4 = into4 };
            //var data = query.FirstOrDefault();
            //data.q1.No = model.No;
            //data.q1.TicketId = model.TicketId;
            //data.q1.TrainingPlace = model.TrainingPlace;
            //data.q1.DangerLevel = model.DangerLevel;
            //try
            //{
            //    db.Update(data.q1);

            //    var newitems1 = model.Measures.Where(x => !data.q3.Any(y => y.TrainingMeasureId == x.TrainingMeasureId));
            //    db.Insert(newitems1.Select(x => new HumanDangerTrainingMeasureEntity() { TrainingMeasureId = x.TrainingMeasureId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId.Value, CategoryId = x.CategoryId, Category = x.Category }).ToList());
            //    var delitems1 = data.q3.Where(x => !model.Measures.Any(y => y.TrainingMeasureId == x.TrainingMeasureId)).ToList();
            //    db.Delete(delitems1);
            //    var newitems2 = model.TaskTypes.Where(x => !data.q4.Any(y => y.TaskTypeId == x.TaskTypeId));
            //    db.Insert(newitems2.Select(x => new HumanDangerTrainingTypeEntity() { TaskTypeId = x.TaskTypeId, TypeName = x.TaskTypeName }).ToList());
            //    var delitems2 = data.q4.Where(x => !model.TaskTypes.Any(y => y.TaskTypeId == x.TaskTypeId)).ToList();
            //    db.Delete(delitems2);

            //    db.Commit();
            //}
            //catch (Exception)
            //{
            //    db.Rollback();
            //    throw;
            //}


            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangerTrainingUsers.Include("Training").Include("TrainingMeasures").Include("TrainingTypes").FirstOrDefault(x => x.TrainingUserId == model.TrainingId);
                entity.No = model.No;
                entity.TicketId = model.TicketId;
                entity.TrainingPlace = model.TrainingPlace;
                entity.DangerLevel = model.DangerLevel;
                var temp = default(HumanDanger);
                if (entity.Training.HumanDangerId != null)
                    temp = ctx.HumanDangers.Include("Measures").FirstOrDefault(x => x.HumanDangerId == entity.Training.HumanDangerId);
                var newitems1 = model.Measures.Where(x => !entity.TrainingMeasures.Any(y => y.TrainingMeasureId == x.TrainingMeasureId));
                entity.TrainingMeasures.AddRange(newitems1.Select(x => new TrainingMeasure() { TrainingMeasureId = x.TrainingMeasureId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId, CategoryId = x.CategoryId, Category = x.Category }));
                var delitems1 = entity.TrainingMeasures.Where(x => !model.Measures.Any(y => y.TrainingMeasureId == x.TrainingMeasureId)).ToList();
                foreach (var item in delitems1)
                {
                    ctx.Entry<TrainingMeasure>(item).State = System.Data.Entity.EntityState.Deleted;
                }
                var newitems2 = model.TaskTypes.Where(x => !entity.TrainingTypes.Any(y => y.TaskTypeId == x.TaskTypeId));
                entity.TrainingTypes.AddRange(newitems2.Select(x => new TrainingType() { TaskTypeId = x.TaskTypeId, TypeName = x.TaskTypeName }));
                var delitems2 = entity.TrainingTypes.Where(x => !model.TaskTypes.Any(y => y.TaskTypeId == x.TaskTypeId)).ToList();
                foreach (var item in delitems2)
                {
                    ctx.Entry<TrainingType>(item).State = System.Data.Entity.EntityState.Deleted;
                }

                ctx.SaveChanges();
            }
        }

        public void Finish(string id, DateTime date)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangerTrainingUsers.Include("Training").Include("TrainingMeasures").Include("TrainingTypes").FirstOrDefault(x => x.TrainingUserId == id);
                if (entity.Training.HumanDangerId == null)
                {
                    entity.TrainingMeasures.ForEach(x => x.State = 3);
                    entity.TrainingTypes.ForEach(x => x.State = 3);
                }
                else
                {
                    var humandanger = ctx.HumanDangers.Include("Measures").FirstOrDefault(x => x.HumanDangerId == entity.Training.HumanDangerId);
                    if (humandanger == null)
                    {
                        entity.TrainingMeasures.ForEach(x => x.State = 0);
                        entity.TrainingTypes.ForEach(x => x.State = 0);
                    }
                    else
                    {
                        foreach (var item in entity.TrainingMeasures)
                        {
                            if (humandanger.Measures.Any(x => x.MeasureId == item.MeasureId))
                            {
                                item.State = 1;
                                var measure = humandanger.Measures.Find(x => x.MeasureId == item.MeasureId);
                                item.Standard = measure.MeasureContent;
                            }
                            //else
                            //{
                            //    if (!humandanger.Measures.Any(x => x.CategoryId == item.CategoryId)) item.State = 3;
                            //}
                        }
                        var newitems = humandanger.Measures.Where(x => !entity.TrainingMeasures.Any(y => y.MeasureId == x.MeasureId)).ToList();
                        foreach (var item in newitems)
                        {
                            entity.TrainingMeasures.Add(new TrainingMeasure() { TrainingMeasureId = Guid.NewGuid(), CategoryId = item.CategoryId, Category = item.Category, DangerReason = item.DangerReason, MeasureContent = item.MeasureContent, MeasureId = item.MeasureId, State = 2 });
                        }
                        if (!string.IsNullOrEmpty(humandanger.TaskType))
                        {
                            var types = humandanger.TaskType.Split(',');
                            foreach (var item in types)
                            {
                                var type = entity.TrainingTypes.Find(x => x.TypeName == item);
                                if (type == null) entity.TrainingTypes.Add(new TrainingType() { TaskTypeId = Guid.NewGuid(), TypeName = item, State = 2 });
                                else type.State = 1;
                            }
                        }
                    }
                }
                entity.IsDone = true;
                entity.TrainingTime = date;

                ctx.SaveChanges();
            }
        }

        //public HumanDangerTrainingEntity GetDetailByJob(string jobid)
        //{
        //    var result = default(HumanDangerTrainingEntity);
        //    using (var ctx = new DataContext())
        //    {
        //        var entity = ctx.HumanDangerTrainings.Include("TrainingUsers").Include("Measures").FirstOrDefault(x => x.MeetingJobId == jobid);
        //        if (entity != null)
        //        {
        //            result = new HumanDangerTrainingEntity()
        //            {
        //                TrainingId = entity.TrainingId,
        //                TrainingTask = entity.TraningTask,
        //                TrainingPlace = entity.TrainingPlace,
        //                No = entity.No,
        //                CreateTime = entity.CreateTime,
        //                CreateUserId = entity.CreateUserId,
        //                HumanDangerId = entity.HumanDangerId,
        //                IsMarked = entity.IsMarked,
        //                IsDone = entity.IsDone,
        //                TrainingUsers = entity.TrainingUsers.Select(x => new TrainingUserEntity() { TrainingUserId = x.TrainingUserId, UserId = x.UserId, UserName = x.UserName, TrainingRole = x.TrainingRole }).ToList(),
        //                Measures = entity.Measures.Select(x => new TrainingMeasureEntity() { TrainingMeasureId = x.TrainingMeasureId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId }).ToList()
        //            };
        //        }
        //    }
        //    return result;
        //}

        public List<HumanDangerTrainingEntity> GetListByDeptId(string deptid, DateTime from, DateTime to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var meeting = (from q in db.IQueryable<WorkmeetingEntity>()
                           where q.GroupId == deptid
                           orderby q.MeetingStartTime descending
                           select q).FirstOrDefault();
            var jobs = new List<MeetingAndJobEntity>();
            if (meeting.MeetingType == "班前会" && meeting.IsOver)
            {
                jobs = (from q in db.IQueryable<MeetingAndJobEntity>()
                        where q.StartMeetingId == meeting.MeetingId
                        select q).ToList();
            }
            else if (meeting.MeetingType == "班后会" && meeting.IsOver)
            {
                jobs = (from q in db.IQueryable<MeetingAndJobEntity>()
                        where q.StartMeetingId == meeting.OtherMeetingId
                        select q).ToList();
            }
            var jobids = jobs.Select(x => x.MeetingJobId).ToList();

            var userquery = from q in db.IQueryable<PeopleEntity>()
                            where q.BZID == deptid
                            select new { q.ID, q.Planer };
            var users = userquery.ToList();

            var result = new List<HumanDangerTrainingEntity>();
            using (var ctx = new DataContext())
            {
                var query = ctx.HumanDangerTrainingUsers.Include("Training").Where(x => x.Training.DeptId == deptid && x.Training.CreateTime >= @from && x.Training.CreateTime <= to);
                //if (meeting == null) query = query.Where(x => false);
                //else if (meeting.MeetingType == "班前会" && meeting.IsOver == false)
                //    query = query.Where(x => false);
                //else if (meeting.MeetingType == "班后会" && meeting.IsOver == true)
                //    query = query.Where(x => false);
                query = query.Where(x => (x.Training.MeetingJobId == null && (x.IsDone == false || x.IsMarked == false)) || jobids.Contains(x.Training.MeetingJobId) || (x.IsDone == true && x.IsMarked == false));

                var data = query.Select(x => new { x.TrainingUserId, x.Training.TrainingTask, x.IsDone, x.IsMarked, x.IsEvaluated, x.UserId, x.UserName, x.Training.CreateTime }).ToList();
                var sss = data.GroupJoin(users, x => x.UserId, y => y.ID, (x, y) => new { x.TrainingUserId, x.TrainingTask, x.IsDone, x.IsEvaluated, x.IsMarked, x.UserId, x.UserName, x.CreateTime, Planer = y == null ? "99" : y.First().Planer }).OrderByDescending(x => x.CreateTime).ThenBy(x => x.TrainingTask).ThenBy(x => x.IsDone).ThenByDescending(x => x.IsMarked).ToList();
                foreach (var item in sss)
                {
                    result.Add(new HumanDangerTrainingEntity() { TrainingId = item.TrainingUserId, TrainingTask = item.TrainingTask, IsDone = item.IsDone, IsMarked = item.IsMarked, IsEvaluate = item.IsEvaluated, UserName = item.UserName, UserId = item.UserId, CreateTime = item.CreateTime });
                }
            }
            return result;
        }

        public List<HumanDangerTrainingEntity> GetData(string[] deptid, DateTime from, DateTime to)
        {
            var result = new List<HumanDangerTrainingEntity>();

            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<PeopleEntity>()
                        join q2 in (from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                                    join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                                    join q3 in db.IQueryable<MeetingAndJobEntity>() on q2.MeetingJobId equals q3.MeetingJobId into t3
                                    from q3 in t3.DefaultIfEmpty()
                                    join q4 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q4.MeetingId into t4
                                    from q4 in t4.DefaultIfEmpty()
                                    select new { q1, q2, q3, q4 }) on q1.ID equals q2.q1.UserId
                        where q1.FingerMark == "yes" && ((q2.q3 == null && q2.q1.IsDone == true && q2.q1.IsMarked == true) || (q2.q3 != null && ((q2.q1.IsDone == true && q2.q1.IsMarked == true) || (!(q2.q1.IsDone == true && q2.q1.IsMarked == true) && q2.q4.IsOver == true)))) && deptid.Contains(q2.q2.DeptId) && q2.q2.CreateTime >= @from && q2.q2.CreateTime <= to
                        orderby q1.Planer
                        select new { q1, q2 };

            var data = query.ToList();
            foreach (var item in data)
            {
                result.Add(new HumanDangerTrainingEntity() { TrainingId = item.q2.q1.TrainingId, UserId = item.q2.q1.UserId, UserName = item.q2.q1.UserName, IsDone = item.q2.q1.IsDone, IsEvaluate = item.q2.q1.IsEvaluated, Seq = item.q2.q4 == null ? 0 : 1 });
            }


            //using (var ctx = new DataContext())
            //{
            //    var query = from q in ctx.HumanDangerTrainingUsers.Include("Training")
            //                where q.IsDone == true && q.IsMarked == true && q.Training.DeptId == deptid && q.Training.CreateTime >= @from && q.Training.CreateTime <= to
            //                select q;
            //    var data = query.ToList();
            //    foreach (var item in data)
            //    {
            //        result.Add(new HumanDangerTrainingEntity() { TrainingId = item.TrainingUserId, UserId = item.UserId, UserName = item.UserName, IsDone = item.IsDone, IsEvaluate = item.IsEvaluated });
            //    }
            //}
            return result;
        }

        public void EditMeasure(HumanDangerTrainingEntity model, string userid, string username)
        {
            //IRepository db = new RepositoryFactory().BaseRepository();

            //var entity = db.FindEntity<HumanDangerTrainingUserEntity>(model.TrainingId);
            //entity.No = model.No;
            //entity.TicketId = model.TicketId;
            //entity.IsMarked = true;

            //db.Update(entity);

            var db = DbFactory.Base();

            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangerTrainingUsers.Find(model.TrainingId);
                entity.OtherMeasure = model.OtherMeasure;
                entity.IsMarked = true;
                entity.No = model.No;
                entity.TicketId = model.TicketId;

                var training = ctx.HumanDangerTrainings.Find(entity.TrainingId);
                if (training.HumanDangerId == null)
                {
                    ctx.Entry(entity).Collection(x => x.TrainingTypes).Load();
                    ctx.Entry(entity).Collection(x => x.TrainingMeasures).Load();

                    var dept = (from q in db.IQueryable<DepartmentEntity>()
                                where q.DepartmentId == training.DeptId
                                select q).FirstOrDefault();

                    var state = 1;
                    if (dept.Nature == "部门") state = 2;
                    if (dept.IsSpecial == true) state = 3;

                    var deptlist = default(List<WorkGroupSetEntity>);
                    if (dept.TeamType == "01")
                    {
                        deptlist = new WorkOrderService().GetWorkOrderGroup(dept.DepartmentId).ToList();
                    }

                    var hm = ctx.HumanDangers.FirstOrDefault(x => x.Task == training.TrainingTask && x.State == state);
                    if (hm == null)
                    {
                        hm = new HumanDanger()
                        {
                            HumanDangerId = Guid.NewGuid().ToString(),
                            Task = training.TrainingTask,
                            TaskArea = entity.TrainingPlace,
                            TaskType = string.Join(",", entity.TrainingTypes.Select(x => x.TypeName)),
                            DangerLevel = entity.DangerLevel,
                            OperateTime = DateTime.Now,
                            OperateUserId = userid,
                            OperateUser = username,
                            State = state,
                            DeptId = (deptlist == null || deptlist.Count == 0) ? training.DeptId : string.Join(",", deptlist.Select(x => x.departmentid)),
                            DeptName = (deptlist == null || deptlist.Count == 0) ? training.DeptName : string.Join(",", deptlist.Select(x => x.fullname)),
                            Measures = new List<HumanDangerMeasure>()
                        };
                        foreach (var item in entity.TrainingMeasures)
                        {
                            var measure = ctx.DangerMeasures.Find(item.MeasureId);
                            hm.Measures.Add(new HumanDangerMeasure()
                            {
                                HumanDangerMeasureId = Guid.NewGuid().ToString(),
                                CategoryId = item.CategoryId,
                                Category = item.Category,
                                MeasureId = item.MeasureId,
                                DangerReason = item.DangerReason,
                                MeasureContent = (measure == null || string.IsNullOrEmpty(measure.MeasureContent)) ? item.MeasureContent : measure.MeasureContent
                            });
                        }
                        ctx.HumanDangers.Add(hm);
                    }
                    else
                    {
                        ctx.Entry(hm).Collection(x => x.Measures).Load();

                        hm.TaskArea = entity.TrainingPlace;
                        hm.TaskType = string.Join(",", hm.TaskType.Split(',').Concat(entity.TrainingTypes.Select(x => x.TypeName)).Distinct());
                        hm.DangerLevel = string.Join(",", hm.DangerLevel.Split(',').Concat(entity.DangerLevel.Split(',')).Distinct());
                        foreach (var item in entity.TrainingMeasures)
                        {
                            var measure = ctx.DangerMeasures.Find(item.MeasureId);
                            if (hm.Measures.Any(x => x.MeasureId == item.MeasureId))
                            {
                                if (string.IsNullOrEmpty(measure.MeasureContent))
                                {
                                    var hmMeasure = hm.Measures.Find(x => x.MeasureId == item.MeasureId);
                                    hmMeasure.MeasureContent = item.MeasureContent;
                                    //ctx.Entry(hmMeasure).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                            else
                            {
                                hm.Measures.Add(new HumanDangerMeasure()
                                {
                                    HumanDangerMeasureId = Guid.NewGuid().ToString(),
                                    CategoryId = item.CategoryId,
                                    Category = item.Category,
                                    MeasureId = item.MeasureId,
                                    DangerReason = item.DangerReason,
                                    MeasureContent = (measure == null || string.IsNullOrEmpty(measure.MeasureContent)) ? item.MeasureContent : measure.MeasureContent
                                });
                            }
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }

        public List<HumanDangerTrainingEntity> GetTrainings(string userid, string[] depts, string key, DateTime? from, DateTime? to, string evaluatestatus, int pageSize, int pageIndex, string fzuser, string evaluatelevel, string status, out int total)
        {
            var result = new List<HumanDangerTrainingEntity>();

            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        //join q3 in db.IQueryable<MeetingAndJobEntity>() on q2.MeetingJobId equals q3.MeetingJobId into t3
                        //from q3 in t3.DefaultIfEmpty()
                        //join q4 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q4.JobId into t4
                        //from q4 in t4.DefaultIfEmpty()
                        //join q5 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q5.MeetingId into t5
                        //from q5 in t5.DefaultIfEmpty()
                        join q6 in db.IQueryable<ActivityEvaluateEntity>() on q1.TrainingUserId equals q6.Activityid into t6
                        where q1.IsDone == true && q1.IsMarked == true
                        orderby q1.TrainingTime descending
                        select new { q1, q2, /*q3, q4, q5,*/ q6 = t6 };

            if (depts != null) query = query.Where(x => depts.Contains(x.q2.DeptId));
            if (!string.IsNullOrEmpty(key))
            {
                string[] users = key.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(x => users.Contains(x.q1.UserName));
            }
            if (!string.IsNullOrEmpty(fzuser))
            {
                string[] fzusers = fzuser.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(x => fzusers.Contains(x.q1.UserName) && x.q1.TrainingRole == 1);
            }
            if (from != null) query = query.Where(x => x.q2.CreateTime >= from);
            if (to != null) query = query.Where(x => x.q2.CreateTime <= to);

            switch (status)
            {
                case "已完成":
                    query = query.Where(x => x.q1.IsDone == true && x.q1.IsMarked == true);
                    break;
                case "逾期未开展":
                    query = from q1 in query
                            join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.q2.MeetingJobId equals q2.MeetingJobId
                            join q3 in db.IQueryable<WorkmeetingEntity>() on q2.EndMeetingId equals q3.MeetingId
                            where q3.IsOver == true && q1.q1.IsDone == false
                            select q1;
                    //query = query.Where(x => x.q5.IsOver == true && x.q1.IsDone == false);
                    break;
                case "工作任务已取消":
                    query = from q1 in query
                            join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.q2.MeetingJobId equals q2.MeetingJobId
                            join q3 in db.IQueryable<MeetingJobEntity>() on q2.JobId equals q3.JobId
                            where q3.IsFinished == "cancel"
                            select q1;
                    //query = query.Where(x => x.q4.IsFinished == "cancel");
                    break;
                case "风险预控任务已取消":
                    query = query.Where(x => x.q1.Status == 1);
                    break;
                default:
                    break;
            }

            if (evaluatestatus == "已评")
            {
                switch (evaluatelevel)
                {
                    case "本人":
                        query = query.Where(x => x.q6.Count(y => y.EvaluateId == userid) > 0);
                        break;
                    case "班组":
                        query = query.Where(x => x.q6.Count(y => y.Nature == evaluatestatus) > 0);
                        break;
                    case "部门":
                        query = query.Where(x => x.q6.Count(y => y.Nature == evaluatestatus) > 0);
                        break;
                    case "厂级":
                        query = query.Where(x => x.q6.Count(y => y.Nature == evaluatestatus) > 0);
                        break;
                    default:
                        query = query.Where(x => x.q6.Count() > 0);
                        break;
                }
            }
            total = query.Count();
            var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            foreach (var item in data)
            {
                var t = new HumanDangerTrainingEntity() { TrainingId = item.q1.TrainingUserId, HumanDangerId = item.q2.HumanDangerId, TrainingTask = item.q2.TrainingTask, IsDone = item.q1.IsDone, IsMarked = item.q1.IsMarked, CreateTime = item.q2.CreateTime, CreateUserId = item.q2.CreateUserId, UserId = item.q1.UserId, UserName = item.q1.UserName, DeptId = item.q2.DeptId, DeptName = item.q2.DeptName, TrainingTime = item.q1.TrainingTime, IsEvaluate = item.q6.Any(x => x.EvaluateId == userid), Evaluates = item.q6.ToList() };

                if (item.q1.IsDone == true && item.q1.IsMarked == true)
                {
                    var relation = db.IQueryable<MeetingAndJobEntity>().FirstOrDefault(x => x.MeetingJobId == item.q2.MeetingJobId);
                    if (relation != null)
                    {
                        var job = db.IQueryable<MeetingJobEntity>().FirstOrDefault(x => x.JobId == relation.JobId);
                        if (job != null)
                        {
                            if (job.IsFinished == "cancel") t.Status = "工作任务已取消";
                            else t.Status = "已完成";
                        }
                        else
                            t.Status = "已完成";
                    }
                    else
                    {
                        if (item.q1.Status == 1) t.Status = "风险预控任务已取消";
                        else t.Status = "已完成";
                    }
                    //if (item.q4 != null && item.q4.IsFinished == "cancel") t.Status = "工作任务已取消";
                    //else if (item.q1.Status == 1) t.Status = "风险预控任务已取消";
                    //else t.Status = "已完成";
                }
                else
                {
                    var relation = db.IQueryable<MeetingAndJobEntity>().FirstOrDefault(x => x.MeetingJobId == item.q2.MeetingJobId);
                    if (relation != null)
                    {
                        var meeting = db.IQueryable<WorkmeetingEntity>().FirstOrDefault(x => x.MeetingId == relation.EndMeetingId);
                        if (meeting != null && meeting.IsOver == true) t.Status = "逾期未开展";
                    }
                    //if (item.q5 != null && item.q5.IsOver == true) t.Status = "逾期未开展";
                }

                result.Add(t);
            }







            //using (var ctx = new DataContext())
            //{
            //    var query = ctx.HumanDangerTrainings.Include("TrainingUsers").Where(x => x.TrainingUsers.Any(y => y.IsDone == true && y.IsMarked == true));
            //    if (depts != null) query = query.Where(x => depts.Contains(x.DeptId));
            //    if (!string.IsNullOrEmpty(key))
            //    {
            //        string[] users = key.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //        //query = query.Where(x => x.TrainingUsers.Any(y => y.UserName.Contains(key)));
            //        query = query.Where(x => x.TrainingUsers.Any(y => users.Contains(y.UserName)));
            //    }
            //    if (!string.IsNullOrEmpty(fzuser))
            //    {
            //        string[] fzusers = fzuser.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //        //query = query.Where(x => x.TrainingUsers.Any(y => y.UserName.Contains(key)));
            //        query = query.Where(x => x.TrainingUsers.Any(y => fzusers.Contains(y.UserName)));
            //    }
            //    if (from != null) query = query.Where(x => x.CreateTime >= from);
            //    if (to != null) query = query.Where(x => x.CreateTime <= to);
            //    //if (v3 == 1) query = query.Where(x => x.TrainingUsers.All(y => y.IsEvaluated == false));
            //    //else if (v3 == 2) query = query.Where(x => x.TrainingUsers.Any(y => y.IsEvaluated == true));
            //    //else if (v3 == 3) query = query.Where(x => x.TrainingUsers.All(y => y.IsEvaluated == true));
            //    var userids = query.SelectMany(x => x.TrainingUsers).Select(p => p.TrainingUserId.ToString()).ToList();
            //    var db = new RepositoryFactory().BaseRepository();
            //    if (!string.IsNullOrWhiteSpace(evaluatelevel) && evaluatelevel != "全部" && v3 != 0)// v3=0即全部状态，即查所有的数据 ，就不做下面的筛选
            //    {
            //        var evaluateQuery = db.IQueryable<ActivityEvaluateEntity>(x => userids.Contains(x.Activityid));
            //        if (evaluatelevel == "本人")
            //        {
            //            string userId = OperatorProvider.Provider.Current().UserId;
            //            if (v3 == 3)//本人已评价
            //            {
            //                evaluateQuery = evaluateQuery.Where(x => x.EvaluateId == userId);
            //            }
            //            else if (v3 == 1)//本人未评价
            //            {
            //                evaluateQuery = evaluateQuery.Where(x => x.EvaluateId != userId);//本人未评价即 非本人评价的数据
            //            }
            //        }
            //        else
            //        {
            //            if (v3 == 3)//已评价
            //            {
            //                evaluateQuery = evaluateQuery.Where(x => x.Nature == evaluatelevel);
            //            }
            //            else if (v3 == 1)//未评价
            //            {
            //                evaluateQuery = evaluateQuery.Where(x => x.Nature != evaluatelevel);//部门未评价即 非本部门评价的数据
            //            }
            //        }
            //        List<string> trainingUserIds = evaluateQuery.Select(x => x.Activityid).Distinct().ToList();
            //        query = query.Where(x => x.TrainingUsers.Any(p => trainingUserIds.Contains(p.TrainingUserId.ToString())));
            //    }
            //    else if ((string.IsNullOrWhiteSpace(evaluatelevel) || evaluatelevel == "全部") && v3 != 0)
            //    {
            //        var evaluateQuery = db.IQueryable<ActivityEvaluateEntity>();
            //        if (v3 == 3)//已评价
            //        {
            //            evaluateQuery = evaluateQuery.Where(x => userids.Contains(x.Activityid));
            //            List<string> trainingUserIds = evaluateQuery.Select(x => x.Activityid).Distinct().ToList();
            //            query = query.Where(x => x.TrainingUsers.Any(p => trainingUserIds.Contains(p.TrainingUserId.ToString())));
            //        }
            //        else if (v3 == 1)//未评价
            //        {
            //            var ids = evaluateQuery.Select(x => x.Activityid).ToList();
            //            //所有的id不在评价表里的，即时未评价的
            //            query = query.Where(x => x.TrainingUsers.All(p => !ids.Contains(p.TrainingUserId.ToString())));
            //        }
            //    }

            //    total = query.Count();

            //    var data = query.OrderByDescending(x => x.CreateTime).Skip(rows * (page - 1)).Take(rows).ToList();
            //    data.ForEach(x =>
            //    {
            //        if (x.TrainingUsers != null && x.TrainingUsers.Count > 0)
            //        {
            //            x.TrainingUsers.ForEach(m =>
            //            {
            //                if (m.IsDone == true && m.IsMarked == true)
            //                {
            //                    result.Add(new HumanDangerTrainingEntity()
            //                    {
            //                        TrainingId = x.TrainingId,
            //                        TrainingTask = x.TrainingTask,
            //                        CreateTime = x.CreateTime,
            //                        CreateUserId = x.CreateUserId,
            //                        DeptId = x.DeptId,
            //                        DeptName = x.DeptName,
            //                        Evaluate = x.TrainingUsers.All(y => y.IsEvaluated == true) ? 3 : x.TrainingUsers.All(y => y.IsEvaluated == false) ? 1 : 2,
            //                        TrainingUsers = x.TrainingUsers.Where(y => y.IsDone == true && y.IsMarked == true).Select(y =>
            //                        new TrainingUserEntity()
            //                        {
            //                            TrainingUserId = y.TrainingUserId,
            //                            UserId = y.UserId,
            //                            UserName = y.UserName,
            //                            TrainingRole = y.TrainingRole
            //                        }).ToList(),
            //                        UserId = m.TrainingUserId.ToString(),
            //                        UserName = m.UserName
            //                    });
            //                }
            //            });
            //        }
            //    });
            //    var a = data.Select(x =>
            //       new HumanDangerTrainingEntity()
            //       {
            //           TrainingId = x.TrainingId,
            //           TrainingTask = x.TrainingTask,
            //           CreateTime = x.CreateTime,
            //           CreateUserId = x.CreateUserId,
            //           DeptId = x.DeptId,
            //           DeptName = x.DeptName,
            //           Evaluate = x.TrainingUsers.All(y => y.IsEvaluated == true) ? 3 : x.TrainingUsers.All(y => y.IsEvaluated == false) ? 1 : 2,
            //           TrainingUsers = x.TrainingUsers.Where(y => y.IsDone == true && y.IsMarked == true).Select(y =>
            //           new TrainingUserEntity()
            //           {
            //               TrainingUserId = y.TrainingUserId,
            //               UserId = y.UserId,
            //               UserName = y.UserName,
            //               TrainingRole = y.TrainingRole
            //           }).ToList()
            //       });
            //}
            return result;
        }

        public List<HumanDangerTrainingEntity> GetContent(string id)
        {
            var result = new List<HumanDangerTrainingEntity>();
            using (var ctx = new DataContext())
            {
                var data = ctx.HumanDangerTrainingUsers.Include("Training").Include("Training.TrainingUsers").Include("TrainingMeasures").Include("TrainingTypes").Where(x => x.TrainingUserId == id && x.IsDone == true && x.IsMarked == true).ToList();
                if (data == null) return result;

                result.AddRange(data.Select(x => new HumanDangerTrainingEntity()
                {
                    TrainingId = x.TrainingUserId, //hm 2019-06-25
                    CreateTime = x.Training.CreateTime,
                    CreateUserId = x.Training.CreateUserId,
                    DangerLevel = x.DangerLevel,
                    DeptId = x.Training.DeptId,
                    DeptName = x.Training.DeptName,
                    No = x.No,
                    OtherMeasure = x.OtherMeasure,
                    TrainingPlace = x.TrainingPlace,
                    TrainingTask = x.Training.TrainingTask,
                    IsDone = x.IsDone,
                    IsMarked = x.IsMarked,
                    UserName = x.UserName,
                    UserId = x.UserId,
                    HumanDangerId = x.Training.HumanDangerId,
                    Measures = x.TrainingMeasures.Select(y => new TrainingMeasureEntity() { TrainingMeasureId = y.TrainingMeasureId, CategoryId = y.CategoryId, Category = y.Category, DangerReason = y.DangerReason, MeasureContent = y.MeasureContent, Standard = y.Standard, MeasureId = y.MeasureId, State = y.State }).ToList(),
                    TaskTypes = x.TrainingTypes.Select(y => new TaskTypeEntity() { TaskTypeId = y.TaskTypeId, TaskTypeName = y.TypeName, State = y.State }).ToList(),
                    TrainingUsers = x.Training.TrainingUsers.OrderByDescending(y => y.TrainingRole).OrderBy(y => y.UserName).Select(y => new TrainingUserEntity() { UserId = y.UserId, UserName = y.UserName, TrainingRole = y.TrainingRole }).ToList(),
                    TrainingTime = x.TrainingTime
                }).ToList());
            }
            return result;
        }

        public void Evaluate(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var cnt = (from q in db.IQueryable<ActivityEvaluateEntity>()
                       where q.Activityid == id
                       select q).Count();

            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangerTrainingUsers.Find(id);
                entity.EvaluateTimes = cnt;
                entity.IsEvaluated = true;

                ctx.SaveChanges();
            }
        }

        public List<HumanDangerTrainingEntity> GetUndo(string[] depts, string key, DateTime? from, DateTime? to, int rows, int page, out int total)
        {
            var result = new List<HumanDangerTrainingEntity>();
            using (var ctx = new DataContext())
            {
                var query = ctx.HumanDangerTrainingUsers.Include("Training").Where(x => x.IsDone == false);
                if (depts != null) query = query.Where(x => depts.Contains(x.Training.DeptId));
                // if (!string.IsNullOrEmpty(key)) query = query.Where(x => x.UserName.Contains(key));
                if (!string.IsNullOrEmpty(key))
                {
                    string[] users = key.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //query = query.Where(x => x.TrainingUsers.Any(y => y.UserName.Contains(key)));
                    query = query.Where(x => users.Contains(x.UserName));
                }
                if (from != null) query = query.Where(x => x.Training.CreateTime >= from);
                if (to != null) query = query.Where(x => x.Training.CreateTime <= to);
                total = query.Count();

                var data = query.OrderByDescending(x => x.Training.CreateTime).Skip(rows * (page - 1)).Take(rows).ToList();
                result.AddRange(data.Select(x => new HumanDangerTrainingEntity() { TrainingUserId = x.TrainingUserId, TrainingId = x.TrainingId, TrainingTask = x.Training.TrainingTask, CreateTime = x.Training.CreateTime, CreateUserId = x.Training.CreateUserId, DeptId = x.Training.DeptId, DeptName = x.Training.DeptName, UserName = x.UserName }));
            }
            return result;
        }

        public List<HumanDangerTrainingEntity> GetDataList(string userid, string[] users, DateTime? from, DateTime? to, string status, string level, string evaluatestatus, int pagesize, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q2.MeetingJobId equals q3.MeetingJobId into t3
                        from q3 in t3.DefaultIfEmpty()
                        join q4 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q4.JobId into t4
                        from q4 in t4.DefaultIfEmpty()
                        join q5 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q5.MeetingId into t5
                        from q5 in t5.DefaultIfEmpty()
                        join q6 in db.IQueryable<ActivityEvaluateEntity>() on q1.TrainingUserId.ToString() equals q6.Activityid into t6
                        where (q1.IsDone == true && q1.IsMarked == true) || (!(q1.IsDone == true && q1.IsMarked == true) && q5 != null && q5.IsOver == true)
                        orderby q1.TrainingTime descending
                        select new { q1, q2, q3, q4, q5, q6 = t6 };

            if (users == null || users.Length == 0) query = query.Where(x => x.q1.UserId == userid);
            else query = query.Where(x => users.Contains(x.q1.UserId));

            if (from != null) query.Where(x => x.q2.CreateTime >= from);

            if (to != null) query.Where(x => x.q2.CreateTime <= to);

            switch (status)
            {
                case "已完成":
                    query = query.Where(x => x.q1.IsDone == true && x.q1.IsMarked == true);
                    break;
                case "逾期未开展":
                    query = query.Where(x => x.q5.IsOver == true && x.q1.IsDone == false);
                    break;
                case "工作任务已取消":
                    query = query.Where(x => x.q4.IsFinished == "cancel");
                    break;
                case "风险预控任务已取消":
                    query = query.Where(x => x.q1.Status == 1);
                    break;
                default:
                    break;
            }

            if (evaluatestatus == "已评")
            {
                switch (level)
                {
                    case "本人":
                        query = query.Where(x => x.q6.Count(y => y.EvaluateId == userid) > 0);
                        break;
                    case "班组":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) > 0);
                        break;
                    case "部门":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) > 0);
                        break;
                    case "厂级":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) > 0);
                        break;
                    default:
                        query = query.Where(x => x.q6.Count() > 0);
                        break;
                }
            }
            else if (evaluatestatus == "未评")
            {
                switch (level)
                {
                    case "本人":
                        query = query.Where(x => x.q6.Count(y => y.EvaluateId == userid) == 0);
                        break;
                    case "班组":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) == 0);
                        break;
                    case "部门":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) == 0);
                        break;
                    case "厂级":
                        query = query.Where(x => x.q6.Count(y => y.Nature == level) == 0);
                        break;
                    default:
                        query = query.Where(x => x.q6.Count() == 0);
                        break;
                }
            }

            total = query.Count();
            var result = new List<HumanDangerTrainingEntity>();

            var data = query.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            foreach (var item in data)
            {
                var t = new HumanDangerTrainingEntity() { TrainingId = item.q1.TrainingUserId, HumanDangerId = item.q2.HumanDangerId, TrainingTask = item.q2.TrainingTask, IsDone = item.q1.IsDone, IsMarked = item.q1.IsMarked, CreateTime = item.q2.CreateTime, CreateUserId = item.q2.CreateUserId, EvaluateTimes = item.q6.Count() };
                if (item.q1.IsDone == true && item.q1.IsMarked == true)
                {
                    if (item.q4 != null && item.q4.IsFinished == "cancel") t.Status = "工作任务已取消";
                    else if (item.q1.Status == 1) t.Status = "风险预控任务已取消";
                    else t.Status = "已完成";
                }
                else
                {
                    if (item.q5 != null && item.q5.IsOver == true) t.Status = "逾期未开展";
                }

                t.IsEvaluate = item.q6.Any(x => x.EvaluateId == userid);

                result.Add(t);
            }

            return result;
        }

        public List<HumanDangerTrainingEntity> GetListByUserIdJobId(string jobid, string userid)
        {
            var result = new List<HumanDangerTrainingEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.HumanDangerTrainingUsers.Include("Training")
                            where q.Training.MeetingJobId == jobid && q.UserId == userid
                            select q;
                var data = query.OrderByDescending(x => x.Training.CreateTime).OrderByDescending(x => x.IsEvaluated).ToList();
                foreach (var item in data)
                {
                    result.Add(new HumanDangerTrainingEntity() { TrainingId = item.TrainingUserId, TrainingTask = item.Training.TrainingTask, IsDone = item.IsDone, IsEvaluate = item.IsEvaluated, IsMarked = item.IsMarked, EvaluateTimes = item.EvaluateTimes, CreateTime = item.Training.CreateTime, UserId = item.UserId, UserName = item.UserName });
                }
            }
            return result;
        }

        public List<HumanDangerTrainingEntity> GetListByJobId(string jobid)
        {
            var result = new List<HumanDangerTrainingEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.HumanDangerTrainingUsers.Include("Training")
                            where q.Training.MeetingJobId == jobid
                            select q;
                var data = query.OrderByDescending(x => x.Training.CreateTime).OrderByDescending(x => x.IsEvaluated).Select(x => new { x.TrainingUserId, x.Training.TrainingTask, x.Training.CreateTime, x.IsDone, x.IsEvaluated, x.IsMarked, x.EvaluateTimes });
                foreach (var item in data)
                {
                    result.Add(new HumanDangerTrainingEntity() { TrainingId = item.TrainingUserId, TrainingTask = item.TrainingTask, IsDone = item.IsDone, IsEvaluate = item.IsEvaluated, IsMarked = item.IsMarked, EvaluateTimes = item.EvaluateTimes, CreateTime = item.CreateTime });
                }
            }
            return result;
        }

        public List<HumanDangerTrainingEntity> GetUserTodo(string userid, string[] users, int pageSize, int pageIndex, out int total)
        {
            var result = new List<HumanDangerTrainingEntity>();

            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q2.MeetingJobId equals q3.MeetingJobId into t3
                        from q3 in t3.DefaultIfEmpty()
                        join q4 in db.IQueryable<MeetingJobEntity>() on q3.JobId equals q4.JobId into t4
                        from q4 in t4.DefaultIfEmpty()
                        join q5 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q5.MeetingId into t5
                        from q5 in t5.DefaultIfEmpty()
                        let seq = userid == q1.UserId ? 0 : 1
                        where !(q1.IsDone == true && q1.IsMarked == true) && (q5 == null || q5.IsOver == false) && users.Contains(q1.UserId)
                        orderby seq, q1.IsDone, q1.IsMarked, q2.CreateTime descending
                        select new { q1, q2, q3, q4, q5 };
            total = query.Count();
            var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            foreach (var item in data)
            {
                result.Add(new HumanDangerTrainingEntity() { TrainingId = item.q1.TrainingUserId, HumanDangerId = item.q2.HumanDangerId, TrainingTask = item.q2.TrainingTask, IsDone = item.q1.IsDone, IsMarked = item.q1.IsMarked, CreateTime = item.q2.CreateTime, CreateUserId = item.q2.CreateUserId, UserId = item.q1.UserId, UserName = item.q1.UserName, DeptId = item.q2.DeptId, DeptName = item.q2.DeptName });
            }

            //using (var ctx = new DataContext())
            //{
            //    var query = from q in ctx.HumanDangerTrainingUsers.Include("Training")
            //                where userid.Contains(q.UserId) && (q.IsDone == false || q.IsMarked == false)
            //                select q;
            //    total = query.Count();
            //    var data = query.OrderByDescending(x => x.Training.CreateTime).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            //    foreach (var item in data)
            //    {
            //        result.Add(new HumanDangerTrainingEntity() { TrainingId = item.TrainingUserId, HumanDangerId = item.Training.HumanDangerId, TrainingTask = item.Training.TrainingTask, IsDone = item.IsDone, IsMarked = item.IsMarked, CreateTime = item.Training.CreateTime, CreateUserId = item.Training.CreateUserId, UserId = item.UserId, UserName = item.UserName, DeptId = item.Training.DeptId, DeptName = item.Training.DeptName });
            //    }
            //}
            return result;
        }

        public List<HumanDangerTrainingEntity> GetTrainingsByTrainingUser(string[] trainingUserIds, DateTime? starttime, DateTime? endtime)
        {
            var result = new List<HumanDangerTrainingEntity>();


            using (var ctx = new DataContext())
            {
                var query = ctx.HumanDangerTrainings.Include("TrainingUsers").Where(x => x.TrainingUsers.Any(y => y.IsDone == true && y.IsMarked == true));

                if (trainingUserIds != null && trainingUserIds.Length > 0)
                {
                    query = query.Where(x => x.TrainingUsers.Any(y => trainingUserIds.Contains(y.UserId)));
                }
                if (starttime != null) query = query.Where(x => x.CreateTime >= starttime);
                if (endtime != null) query = query.Where(x => x.CreateTime <= endtime);
                var data = query.OrderByDescending(x => x.CreateTime).ToList();

                data.ForEach(x =>
                {
                    if (x.TrainingUsers != null && x.TrainingUsers.Count > 0)
                    {
                        x.TrainingUsers.ForEach(m =>
                        {
                            if (m.IsDone == true && m.IsMarked == true)
                            {
                                var traEntity = new HumanDangerTrainingEntity()
                                {
                                    TrainingId = x.TrainingId,
                                    TrainingTask = x.TrainingTask,
                                    CreateTime = x.CreateTime,
                                    CreateUserId = x.CreateUserId,
                                    DeptId = x.DeptId,
                                    DeptName = x.DeptName,
                                    Evaluate = x.TrainingUsers.All(y => y.IsEvaluated == true) ? 3 : x.TrainingUsers.All(y => y.IsEvaluated == false) ? 1 : 2,
                                    TrainingUsers = x.TrainingUsers.Where(y => y.IsDone == true && y.IsMarked == true).Select(y =>
                                    new TrainingUserEntity()
                                    {
                                        TrainingUserId = y.TrainingUserId,
                                        UserId = y.UserId,
                                        UserName = y.UserName,
                                        TrainingRole = y.TrainingRole
                                    }).ToList(),
                                    UserId = m.TrainingUserId.ToString(),
                                    UserName = m.UserName,
                                    TrainingPlace = m.TrainingPlace,



                                };
                                var user = ctx.HumanDangerTrainingUsers.Include("TrainingMeasures").Include("TrainingTypes").FirstOrDefault(p => p.TrainingUserId == m.TrainingUserId);
                                if (user != null && user.TrainingMeasures != null)
                                {
                                    traEntity.Measures = user.TrainingMeasures.Select(measure => new TrainingMeasureEntity()
                                    {
                                        Category = measure.Category,
                                        CategoryId = measure.CategoryId,
                                        DangerReason = measure.DangerReason,
                                        MeasureContent = measure.MeasureContent,
                                        MeasureId = measure.MeasureId,
                                        Standard = measure.Standard,
                                        State = measure.State,
                                        TrainingMeasureId = measure.TrainingMeasureId,
                                    }).ToList();

                                    traEntity.TaskTypes = user.TrainingTypes.Select(a => new TaskTypeEntity()
                                    {
                                        State = a.State,
                                        TaskTypeId = a.TaskTypeId,
                                        TaskTypeName = a.TypeName
                                    }).ToList();
                                    traEntity.TrainingUserId = user.UserId;
                                };
                                result.Add(traEntity);
                            }
                        });
                    }
                });
            }
            return result;
        }

        public List<ItemEntity> GetUsers(string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<DepartmentEntity>()
                        where q.DepartmentId == deptid
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
            //            select new { ItemId = q.DepartmentId, ItemName = q.FullName, ParentItemId = q.ParentId, ItemType = "dept" }).Concat(
            //          from q in db.IQueryable<UserEntity>()
            //          where query.Any(x => x == q.DepartmentId)
            //          select new { ItemId = q.UserId, ItemName = q.RealName, ParentItemId = q.DepartmentId, ItemType = "user" }
            //          ).ToList();

            var data = new List<ItemEntity>();
            var one = (from q in db.IQueryable<DepartmentEntity>()
                       where query.Any(x => x == q.DepartmentId)
                       select new ItemEntity() { ItemId = q.DepartmentId, ItemName = q.FullName, ParentItemId = q.ParentId, ItemType = "dept" }).ToList();
            var two = (from q in db.IQueryable<UserEntity>()
                       where query.Any(x => x == q.DepartmentId)
                       select new ItemEntity() { ItemId = q.UserId, ItemName = q.RealName, ParentItemId = q.DepartmentId, ItemType = "user" }).ToList();

            data.AddRange(one);
            data.AddRange(two);

            var result = data.Select(x => new ItemEntity() { ItemId = x.ItemId, ItemName = x.ItemName, ItemType = x.ItemType, ParentItemId = x.ParentItemId }).ToList();
            return result;
        }
        /// <summary>
        /// 删除分析人
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteTraining(string keyValue)
        {

            using (var ctx = new DataContext())
            {
                HumanDangerTrainingUser trauser = ctx.HumanDangerTrainingUsers.Find(Guid.Parse(keyValue));
                var allUser = ctx.HumanDangerTrainingUsers.Where(p => p.TrainingId == trauser.TrainingId).ToList();
                if (allUser.Count() == 1 && allUser.Count(p => p.TrainingUserId == trauser.TrainingUserId) == 1)
                {
                    var training = ctx.HumanDangerTrainings.Find(trauser.TrainingId);
                    ctx.HumanDangerTrainings.Remove(training);
                    //db.Delete<HumanDangerTrainingEntity>(trauser.TrainingId);
                }
                ctx.HumanDangerTrainingUsers.Remove(trauser);
                ctx.SaveChanges();
            }
            //    IRepository db = new RepositoryFactory().BaseRepository();
            //HumanDangerTrainingUserEntity trauser  =  db.FindEntity<HumanDangerTrainingUserEntity>(Guid.Parse(keyValue));
            //var ctx = new DataContext();
            //    var allUser = ctx.HumanDangerTrainingUsers.Where(p=>p.TrainingId==trauser.TrainingId);
            //if (allUser.Count()==1 && allUser.Count(p=>p.TrainingUserId==trauser.TrainingUserId)==1)
            //{
            //    db.Delete<HumanDangerTrainingEntity>(trauser.TrainingId);
            //}
            //db.Delete<HumanDangerTrainingUserEntity>(trauser);
            //using (var ctx = new DataContext())
            //{
            //    HumanDangerTrainingUser trauser = ctx.HumanDangerTrainingUsers.Find(keyValue);

            //}
        }

        public object GetData2(string userid, string[] deptid, DateTime from, DateTime to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DepartmentEntity>()
                        join q2 in (from q1 in db.IQueryable<HumanDangerTrainingBaseEntity>()
                                    join q2 in db.IQueryable<HumanDangerTrainingUserEntity>() on q1.TrainingId equals q2.TrainingId
                                    where q1.CreateTime >= @from && q1.CreateTime < to
                                    select new { q2.TrainingUserId, q1.DeptId, q2.IsDone, q2.IsMarked, q1.CreateTime }) on q1.DepartmentId equals q2.DeptId into into2
                        let finished = into2.Count(x => x.IsDone == true && x.IsMarked == true)
                        let total = into2.Count()
                        where deptid.Contains(q1.DepartmentId)
                        select new { q1.DepartmentId, q1.FullName, q1.EnCode, Finished = finished, Total = total, Pct = (float)finished / (total == 0 ? 1 : total) };

            return query.ToList().OrderByDescending(x => x.Pct).ThenBy(x => x.EnCode).ToList();
        }

        public List<HumanDangerTrainingEntity> GetData3(string[] users, DateTime from, DateTime to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q2.MeetingJobId equals q3.MeetingJobId into t3
                        from q3 in t3.DefaultIfEmpty()
                        join q4 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q4.MeetingId into t4
                        from q4 in t4.DefaultIfEmpty()
                        where ((q3 == null && q1.IsDone == true && q1.IsMarked == true) || (q3 != null && ((q1.IsDone == true && q1.IsMarked == true) || (!(q1.IsDone == true && q1.IsMarked == true) && q4.IsOver == true)))) && users.Contains(q1.UserId) && q2.CreateTime >= @from && q2.CreateTime <= to
                        select new { q1, q2, q3, q4 };

            var data = query.ToList();

            return data.Select(x => new HumanDangerTrainingEntity() { TrainingId = x.q1.TrainingId, UserId = x.q1.UserId, UserName = x.q1.UserName, IsDone = x.q1.IsDone, IsEvaluate = x.q1.IsEvaluated, Seq = x.q4 == null ? 0 : 1 }).ToList();
        }

        public List<HumanDangerTrainingEntity> GetToEvaluate(string deptid, string analyst, DateTime? begin, DateTime? end, int pagesize, int page, out int total)
        {
            var result = new List<HumanDangerTrainingEntity>();

            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                        join q2 in db.IQueryable<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        join q3 in db.IQueryable<ToEvaluateEntity>() on q1.TrainingUserId.ToString() equals q3.BusinessId
                        where q3.EvaluateDeptId == deptid && q1.IsDone == true && q1.IsMarked == true && q3.IsDone == false
                        select new { q1, q2, q3 };

            if (!string.IsNullOrEmpty(analyst)) query = query.Where(x => x.q1.UserName == analyst);

            if (begin != null) query = query.Where(x => x.q1.TrainingTime >= begin);
            if (end != null) query = query.Where(x => x.q1.TrainingTime < end);


            total = query.Count();

            var data = query.OrderByDescending(x => x.q3.StartDate).ThenByDescending(x => x.q1.TrainingTime).ThenBy(x => x.q2.DeptId).ThenBy(x => x.q1.UserName).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            foreach (var item in data)
            {
                var t = new HumanDangerTrainingEntity() { TrainingId = item.q1.TrainingUserId, HumanDangerId = item.q2.HumanDangerId, TrainingTask = item.q2.TrainingTask, IsDone = item.q1.IsDone, IsMarked = item.q1.IsMarked, CreateTime = item.q2.CreateTime, CreateUserId = item.q2.CreateUserId, UserId = item.q1.UserId, UserName = item.q1.UserName, DeptId = item.q2.DeptId, DeptName = item.q2.DeptName, TrainingTime = item.q1.TrainingTime, IsEvaluate = false, StartDate = item.q3.StartDate, EndDate = item.q3.EndDate };
                //if (item.q1.IsDone == true && item.q1.IsMarked == true) t.Status = "已完成";
                //else if (item.q5.IsOver == true) t.Status = "逾期未开展";
                //else if (item.q4.IsFinished == "cancel") t.Status = "工作任务已取消";
                //else if (item.q1.Status == 0) t.Status = "风险预控任务已取消";

                //if (item.q1.IsDone == true && item.q1.IsMarked == true)
                //{
                //    if (item.q4 != null && item.q4.IsFinished == "cancel") t.Status = "工作任务已取消";
                //    else if (item.q1.Status == 1) t.Status = "风险预控任务已取消";
                //    else t.Status = "已完成";
                //}
                //else
                //{
                //    if (item.q5 != null && item.q5.IsOver == true) t.Status = "逾期未开展";
                //}

                result.Add(t);
            }

            return result;
        }

        public void Ensure(MeetingJobEntity item, string userId)
        {
            var query = from q1 in _context.Set<HumanDangerTrainingUserEntity>()
                        join q2 in _context.Set<HumanDangerTrainingBaseEntity>() on q1.TrainingId equals q2.TrainingId
                        where q2.MeetingJobId == item.Relation.MeetingJobId && q1.UserId == userId
                        select new { q1, q2 };
            var data = query.FirstOrDefault();
            if (data == null) item.TrainingDone = false;
            else
            {
                item.TrainingDone = data.q1.IsDone == true && data.q1.IsMarked == true;
                item.HumanDangerTraining = data.q2;
                item.HumanDangerTraining.TrainingUsers.ForEach(x => x.Training = null);
                item.Training = new DangerEntity { Id = data.q1.TrainingUserId };
            }
        }
    }
}
