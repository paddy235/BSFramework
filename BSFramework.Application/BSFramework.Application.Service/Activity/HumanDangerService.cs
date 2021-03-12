using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Data;
using BSFramework.Data;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using System.Text;
using BSFramework.Application.Service.PublicInfoManage;
using Bst.ServiceContract.MessageQueue;
using System.ServiceModel;
using Newtonsoft.Json.Linq;
using BSFramework.Application.IService.Activity;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.HuamDanger;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Data.EF;
using System.Linq.Expressions;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class HumanDangerService : IHumanDangerService
    {
        public void Add(List<HumanDangerEntity> data)
        {
            using (var ctx = new DataContext())
            {
                var entities = data.Select(x => new HumanDanger()
                {
                    HumanDangerId = x.HumanDangerId,
                    Task = x.Task,
                    TaskArea = x.TaskArea,
                    DeptId = x.DeptId,
                    DeptName = x.DeptName,
                    TaskType = x.TaskType,
                    DangerLevel = x.DangerLevel,
                    OtherMeasure = x.OtherMeasure,
                    OperateTime = x.OperateTime,
                    OperateUserId = x.OperateUserId,
                    OperateUser = x.OperateUser,
                    Measures = x.Measures.Select(y => new HumanDangerMeasure() { HumanDangerMeasureId = y.HumanDangerMeasureId, Category = y.Category, CategoryId = y.CategoryId, DangerReason = y.DangerReason, MeasureContent = y.MeasureContent, MeasureId = y.MeasureId }).ToList()
                });
                ctx.HumanDangers.AddRange(entities);
                ctx.SaveChanges();
            }
        }

        public void Association()
        {
            using (var ctx = new DataContext())
            {
                string sqlStr = @"update wg_humandangermeasure a
                                        inner join wg_dangermeasure b on a.MeasureId = b.MeasureId
                                        set a.MeasureContent = b.MeasureContent ";
                var count = ctx.Database.ExecuteSqlCommand(sqlStr);
            }
        }

        public bool CheckUnique(string id, string task, string deptId)
        {
            using (var ctx = new DataContext())
            {
                if (!string.IsNullOrWhiteSpace(task) && !string.IsNullOrWhiteSpace(deptId))
                {
                    var deptIds = deptId.Split(',');
                    ////var query = ctx.HumanDangers.Where(p => p.Task.Equals(task));
                    //var expression = LinqExtensions.True<HumanDanger>();
                    //expression = expression.And(p => p.Task.Equals(task) && p.HumanDangerId != id);
                    //var expression2 = LinqExtensions.True<HumanDanger>();
                    //foreach (var item in deptIds)
                    //{
                    //    expression2 = expression2.Or(p => p.DeptId.Contains(item));
                    //}
                    //var where = expression.And(expression2);
                    //var result = ctx.HumanDangers.Count(where);
                    //return result > 0;
                    StringBuilder sqlStr = new StringBuilder();
                    sqlStr.AppendFormat("SELECT Count(*) as Count FROM wg_HumanDanger where task='{0}' and HumanDangerId<>'{1}'", task, id);
                    if (deptIds != null && deptIds.Length > 0)
                    {
                        sqlStr.AppendFormat(" AND (");
                        List<string> strList = new List<string>();

                        foreach (var item in deptIds)
                        {
                            strList.Add(string.Format("  DEPTID like '%{0}%'  ", item));
                        }
                        sqlStr.Append(string.Join("OR", strList));
                        sqlStr.AppendFormat(" )");
                    }

                    var count = ctx.Database.SqlQuery<int>(sqlStr.ToString()).ToList();
                    return count.FirstOrDefault() > 0;

                }
                return false;
            }
        }

        public bool CheckUnique(List<TaskDeptModel> dic)
        {
            using (var ctx = new DataContext())
            {
                if (dic != null && dic.Count > 0)
                {
                    StringBuilder sqlStr = new StringBuilder();
                    sqlStr.AppendFormat("SELECT Count(*) as Count FROM wg_HumanDanger where 1=1 ");
                    sqlStr.AppendFormat(" AND (");
                    List<string> strList = new List<string>();
                    foreach (var item in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(item.DeptId))
                        {

                            strList.Add(string.Format("  DEPTID like '%{0}%' and task='{1}'  ", item.DeptId, item.TaskName));
                        }
                        else
                        {
                            strList.Add(string.Format("  DEPTID is null and task='{0}'  ", item.TaskName));
                        }
                    }
                    sqlStr.Append(string.Join("OR", strList));
                    sqlStr.AppendFormat(" )");
                    var count = ctx.Database.SqlQuery<int>(sqlStr.ToString()).ToList();
                    return count.FirstOrDefault() > 0;
                }
                return false;
            }
        }

        public void Delete(string id)
        {
            using (var ctx = new DataContext())
            {
                var entity = new HumanDanger() { HumanDangerId = id };
                ctx.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

                ctx.SaveChanges();
            }
        }

        public void Evaluate(ActivityEvaluateEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert<ActivityEvaluateEntity>(entity);

                var toevaluate = (from q in db.IQueryable<ToEvaluateEntity>()
                                  where q.BusinessId == entity.Activityid
                                  select q).FirstOrDefault();
                if (toevaluate != null)
                {
                    toevaluate.IsDone = true;
                    db.Update(toevaluate);

                    var message = (from q in db.IQueryable<MessageEntity>()
                                   where q.BusinessId == toevaluate.ToEvaluateId
                                   select q).FirstOrDefault();
                    if (message != null)
                    {
                        message.IsFinished = true;
                        db.Update(message);
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

        public List<HumanDangerEntity> Find(string key, string deptid, int pageSize, int pageIndex, out int total)
        {
            var result = new List<HumanDangerEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.HumanDangers
                            let ss = q.DeptId.Contains(deptid) ? 0 : 1
                            where (string.IsNullOrEmpty(q.DeptId) || q.DeptId.Contains(deptid)) && (q.State == 0 || q.State > 1)
                            select new { q, ss };
                if (!string.IsNullOrEmpty(key)) query = query.Where(x => x.q.Task.Contains(key));
                total = query.Count();
                var data = query.OrderBy(x => x.ss).ThenBy(x => x.q.OperateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                foreach (var item in data)
                {
                    result.Add(new HumanDangerEntity()
                    {
                        HumanDangerId = item.q.HumanDangerId,
                        Task = item.q.Task,
                        TaskArea = item.q.TaskArea,
                        TaskType = item.q.TaskType,
                        DangerLevel = item.q.DangerLevel,
                        DeptName = item.q.DeptName,
                        OtherMeasure = item.q.OtherMeasure
                    });
                }
            }
            return result;
        }

        public List<HumanDangerEntity> GetData(string key, int pagesize, int page, string deptId, out int total)
        {

            var result = new List<HumanDangerEntity>();
            using (var ctx = new DataContext())
            {
                var query = ctx.HumanDangers.AsQueryable();

                query = query.Where(x => x.State == 0);

                if (!string.IsNullOrEmpty(key))
                    query = query.Where(x => x.Task.Contains(key) || x.TaskArea.Contains(key) || x.DeptName.Contains(key) || x.TaskType.Contains(key));
                if (!string.IsNullOrWhiteSpace(deptId))
                {
                    var db = DbFactory.Base();
                    var current = from q in db.IQueryable<DepartmentEntity>()
                                  where q.DepartmentId == deptId
                                  select q;

                    var subquery = from q in db.IQueryable<DepartmentEntity>()
                                   where q.ParentId == deptId
                                   select q;

                    while (subquery.Count() > 0)
                    {
                        current = current.Concat(subquery);
                        subquery = from q1 in subquery
                                   join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                                   select q2;
                    }

                    var deptids = current.Select(x => x.DepartmentId).ToList();
                    Expression<Func<HumanDanger, bool>> exp = x => false;
                    foreach (var item in deptids)
                    {
                        exp = exp.Or(x => x.DeptId.Contains(item));
                    }

                    query = query.Where(exp);

                    //查找选择的部门的所有下级 ，包括当前部门
                    //        var db = new RepositoryFactory().BaseRepository();
                    //var checkDept = db.IQueryable<DepartmentEntity>(p => p.DepartmentId == deptId).FirstOrDefault();
                    //if (checkDept == null)
                    //{
                    //    total = 0;
                    //    return result;
                    //}
                    //List<string> deptIds = db.IQueryable<DepartmentEntity>(x => x.EnCode.Contains(checkDept.EnCode)).Select(p => p.DepartmentId).ToList();//符合条件的部门的id
                    //System.Linq.Expressions.Expression<Func<HumanDanger, bool>> expressions = p => p.DeptId.Contains(deptId);
                    //if (deptIds != null && deptIds.Count > 0)
                    //{
                    //    foreach (var item in deptIds)
                    //    {
                    //        if (item != deptId)
                    //        {
                    //            expressions = expressions.Or(x => x.DeptId.Contains(item));
                    //        }
                    //    }
                    //}
                    //query = query.Where(expressions);
                }


                total = query.Count();
                var data = query.OrderByDescending(x => x.OperateTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                foreach (var item in data)
                {
                    var humandanger = new HumanDangerEntity()
                    {
                        HumanDangerId = item.HumanDangerId,
                        Task = item.Task,
                        TaskArea = item.TaskArea,
                        TaskType = item.TaskType,
                        DeptId = item.DeptId,
                        DeptName = item.DeptName,
                        DangerLevel = item.DangerLevel,
                        OperateTime = item.OperateTime,
                        OtherMeasure = item.OtherMeasure
                    };
                    result.Add(humandanger);
                }
            }
            return result;
        }

        public HumanDangerEntity GetDetail(string id)
        {
            var result = default(HumanDangerEntity);
            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangers.Include("Measures").FirstOrDefault(x => x.HumanDangerId == id);
                if (entity != null)
                {
                    result = new HumanDangerEntity()
                    {
                        HumanDangerId = entity.HumanDangerId,
                        Task = entity.Task,
                        TaskArea = entity.TaskArea,
                        DeptId = entity.DeptId,
                        DeptName = entity.DeptName,
                        TaskType = entity.TaskType,
                        DangerLevel = entity.DangerLevel,
                        OtherMeasure = entity.OtherMeasure,
                        OperateTime = entity.OperateTime,
                        OperateUserId = entity.OperateUserId,
                        OperateUser = entity.OperateUser,
                    };
                    result.Measures = entity.Measures.Select(x => new HumanDangerMeasureEntity() { HumanDangerMeasureId = x.HumanDangerMeasureId, Category = x.Category, CategoryId = x.CategoryId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId }).ToList();
                }
            }
            return result;
        }

        public void Save(HumanDangerEntity model)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangers.Include("Measures").FirstOrDefault(x => x.HumanDangerId == model.HumanDangerId);
                if (entity == null)
                {
                    entity = new HumanDanger()
                    {
                        HumanDangerId = model.HumanDangerId,
                        Task = model.Task,
                        TaskArea = model.TaskArea,
                        DeptId = model.DeptId,
                        DeptName = model.DeptName,
                        TaskType = model.TaskType,
                        DangerLevel = model.DangerLevel,
                        OtherMeasure = model.OtherMeasure,
                        OperateTime = model.OperateTime,
                        OperateUserId = model.OperateUserId,
                        OperateUser = model.OperateUser,
                        Measures = new List<HumanDangerMeasure>()
                    };
                    entity.Measures.AddRange(model.Measures.Select(x => new HumanDangerMeasure() { HumanDangerMeasureId = x.HumanDangerMeasureId, Category = x.Category, CategoryId = x.CategoryId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId }));
                    ctx.HumanDangers.Add(entity);
                }
                else
                {
                    entity.Task = model.Task;
                    entity.TaskArea = model.TaskArea;
                    entity.DeptId = model.DeptId;
                    entity.DeptName = model.DeptName;
                    entity.TaskType = model.TaskType;
                    entity.DangerLevel = model.DangerLevel;
                    entity.OtherMeasure = model.OtherMeasure;
                    entity.OperateTime = model.OperateTime;
                    entity.OperateUserId = model.OperateUserId;
                    entity.OperateUser = model.OperateUser;
                    for (int i = 0; i < entity.Measures.Count; i++)
                    {
                        var measure = model.Measures.Find(x => x.HumanDangerMeasureId == entity.Measures[i].HumanDangerMeasureId);
                        if (measure == null)
                        {
                            ctx.Entry(entity.Measures[i]).State = System.Data.Entity.EntityState.Deleted;
                            i--;
                        }
                        else
                        {
                            entity.Measures[i].CategoryId = measure.CategoryId;
                            entity.Measures[i].Category = measure.Category;
                            entity.Measures[i].MeasureId = measure.MeasureId;
                            entity.Measures[i].DangerReason = measure.DangerReason;
                            entity.Measures[i].MeasureContent = measure.MeasureContent;
                        }
                    }
                    var newitems = model.Measures.Where(x => !entity.Measures.Any(y => y.HumanDangerMeasureId == x.HumanDangerMeasureId));
                    entity.Measures.AddRange(newitems.Select(x => new HumanDangerMeasure() { HumanDangerMeasureId = x.HumanDangerMeasureId, Category = x.Category, CategoryId = x.CategoryId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId }));
                }
                ctx.SaveChanges();
            }
        }

        public List<HumanDangerEntity> GetTemplates(string deptid, string name, int status, int pagesize, int pageindex, out int total)
        {
            var db = DbFactory.Base();

            var query = from q in db.IQueryable<HumanDangerEntity>()
                        where q.State != 0
                        select q;

            if (status > 0) query = query.Where(x => x.State == status);

            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Task.Contains(name));

            if (!string.IsNullOrWhiteSpace(deptid))
            {
                var current = from q in db.IQueryable<DepartmentEntity>()
                              where q.DepartmentId == deptid
                              select q;

                var subquery = from q in db.IQueryable<DepartmentEntity>()
                               where q.ParentId == deptid
                               select q;

                while (subquery.Count() > 0)
                {
                    current = current.Concat(subquery);
                    subquery = from q1 in subquery
                               join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                               select q2;
                }

                var deptids = current.Select(x => x.DepartmentId).ToList();
                Expression<Func<HumanDangerEntity, bool>> exp = x => false;
                foreach (var item in deptids)
                {
                    exp = exp.Or(x => x.DeptId.Contains(item));
                }

                query = query.Where(exp);
            }
            total = query.Count();

            return query.OrderByDescending(x => x.OperateTime).Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
        }

        public void Approve(HumanDangerEntity model, ApproveRecordEntity approve)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.HumanDangers.Include("Measures").FirstOrDefault(x => x.HumanDangerId == model.HumanDangerId);
                entity.TaskArea = model.TaskArea;
                entity.TaskType = model.TaskType;
                entity.DangerLevel = model.DangerLevel;
                entity.State = model.State;
                for (int i = 0; i < entity.Measures.Count; i++)
                {
                    var measure = model.Measures.Find(x => x.HumanDangerMeasureId == entity.Measures[i].HumanDangerMeasureId);
                    if (measure == null)
                    {
                        ctx.Entry(entity.Measures[i]).State = System.Data.Entity.EntityState.Deleted;
                        i--;
                    }
                    else
                    {
                        entity.Measures[i].CategoryId = measure.CategoryId;
                        entity.Measures[i].Category = measure.Category;
                        entity.Measures[i].MeasureId = measure.MeasureId;
                        entity.Measures[i].DangerReason = measure.DangerReason;
                        entity.Measures[i].MeasureContent = measure.MeasureContent;
                    }
                }
                var newitems = model.Measures.Where(x => !entity.Measures.Any(y => y.HumanDangerMeasureId == x.HumanDangerMeasureId));
                entity.Measures.AddRange(newitems.Select(x => new HumanDangerMeasure() { HumanDangerMeasureId = x.HumanDangerMeasureId, Category = x.Category, CategoryId = x.CategoryId, DangerReason = x.DangerReason, MeasureContent = x.MeasureContent, MeasureId = x.MeasureId }));

                ctx.SaveChanges();
            }

            var db = DbFactory.Base() as Database;
            db.dbcontext.Set<ApproveRecordEntity>().Add(approve);
            db.dbcontext.SaveChanges();
        }
    }
}
