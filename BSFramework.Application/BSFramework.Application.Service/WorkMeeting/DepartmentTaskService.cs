using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Common;
using BSFramework.Data;
using System.Text;
using System.Configuration;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.Activity;
using System.Data.Entity;

namespace BSFramework.Service.WorkMeeting
{
    public class DepartmentTaskService : IDepartmentTaskService
    {
        public void Cancel(string id, string user)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var query = from q in db.IQueryable<DepartmentTaskEntity>()
                            where q.TaskId == id
                            select q;

                var subquery = from q1 in db.IQueryable<DepartmentTaskEntity>()
                               join q2 in query on q1.ParentTaskId equals q2.TaskId
                               select q1;

                while (subquery.Count() > 0)
                {
                    query = query.Concat(subquery);

                    subquery = from q1 in db.IQueryable<DepartmentTaskEntity>()
                               join q2 in subquery on q1.ParentTaskId equals q2.TaskId
                               select q1;
                }

                var data = query.ToList();
                foreach (var item in data)
                {
                    item.Status = "已取消";
                    item.UpdateRecords += string.Format("{0}更新于{1}，工作任务已取消{2}", user, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), System.Environment.NewLine);

                }
                db.Update(data);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void Complete(DepartmentTaskEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var entity = db.FindEntity<DepartmentTaskEntity>(model.TaskId);
            if (entity == null) return;

            entity.Status = model.Status;
            db.Update(entity);
        }

        public DepartmentTaskEntity Detail(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DepartmentTaskEntity>()
                        join q2 in db.IQueryable<DepartmentTaskEntity>() on q1.TaskId equals q2.ParentTaskId into into2
                        where q1.TaskId == id
                        select new { q1, q2 = into2 };

            var data = query.FirstOrDefault();
            data.q1.SubTasks = data.q2.ToList();

            var query2 = from q1 in db.IQueryable<FileInfoEntity>()
                         join q2 in db.IQueryable<MeetingAndJobEntity>() on q1.RecId equals q2.MeetingJobId
                         join q3 in db.IQueryable<MeetingJobEntity>() on q2.JobId equals q3.JobId
                         where q3.TemplateId == id
                         select q1;
            data.q1.Files = query2.ToList();
            return data.q1;
        }

        public void Edit(DepartmentTaskEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var query = from q1 in db.IQueryable<DepartmentTaskEntity>()
                            join q2 in db.IQueryable<DepartmentTaskEntity>() on q1.TaskId equals q2.ParentTaskId into into2
                            where q1.TaskId == model.TaskId
                            select new { q1, q2 = into2 };
                var entity = query.FirstOrDefault();

                if (entity == null)
                {
                    db.Insert(model);
                    db.Insert(model.SubTasks);
                }
                else
                {
                    BuildRecords(model, entity.q1, model.ModifyUser);
                    entity.q1.Content = model.Content;
                    entity.q1.StartDate = model.StartDate;
                    entity.q1.EndDate = model.EndDate;
                    entity.q1.DutyDepartmentId = model.DutyDepartmentId;
                    entity.q1.DutyDepartment = model.DutyDepartment;
                    entity.q1.DutyUserId = model.DutyUserId;
                    entity.q1.DutyUser = model.DutyUser;
                    entity.q1.TodoUserId = model.TodoUserId;
                    entity.q1.TodoUser = model.TodoUser;
                    entity.q1.Remark = model.Remark;
                    entity.q1.Status = model.Status;
                    db.Update(entity.q1);

                    foreach (var item in entity.q2)
                    {
                        var m = model.SubTasks.Find(x => x.TaskId == item.TaskId);
                        BuildRecords(m, item, model.ModifyUser);
                        item.Content = m.Content;
                        item.StartDate = m.StartDate;
                        item.EndDate = m.EndDate;
                        item.DutyDepartmentId = m.DutyDepartmentId;
                        item.DutyDepartment = m.DutyDepartment;
                        item.DutyUserId = m.DutyUserId;
                        item.DutyUser = m.DutyUser;
                        item.Remark = m.Remark;
                        item.Status = m.Status;
                        db.Update(item);

                        var taskquery = from q in db.IQueryable<DepartmentTaskEntity>()
                                        where q.ParentTaskId == item.TaskId
                                        select q;

                        var subquery = from q1 in db.IQueryable<DepartmentTaskEntity>()
                                       join q2 in query on q1.ParentTaskId equals item.TaskId
                                       select q1;

                        while (subquery.Count() > 0)
                        {
                            taskquery = taskquery.Concat(subquery);

                            subquery = from q1 in db.IQueryable<DepartmentTaskEntity>()
                                       join q2 in subquery on q1.ParentTaskId equals q2.TaskId
                                       select q1;
                        }

                        var data = taskquery.ToList();
                        foreach (var item1 in data)
                        {
                            item1.Status = "已取消";
                            item1.UpdateRecords += string.Format("{0}更新于{1}，工作任务已取消{2}", model.ModifyUser, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), System.Environment.NewLine);
                        }
                        db.Update(data);
                    }

                    var newitems = model.SubTasks.Where(x => !entity.q2.Any(y => y.TaskId == x.TaskId)).ToList();
                    db.Insert(newitems);

                    newitems.ForEach(x => x.State = 1);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<DepartmentTaskEntity> List(string deptid, string userid, int pagesize, int pageindex, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(userid))
            {
                var qurey = from q1 in db.IQueryable<DepartmentTaskEntity>()
                            join q2 in db.IQueryable<DepartmentTaskEntity>() on q1.ParentTaskId equals q2.TaskId into into2
                            from q2 in into2.DefaultIfEmpty()
                            join q3 in db.IQueryable<DepartmentTaskEntity>() on q1.TaskId equals q3.ParentTaskId into into3
                            where q1.DutyDepartmentId == deptid && (q2 == null || (q2 != null && q2.DutyDepartmentId != deptid))
                            select new { q1, q2, q3 = into3 };
                total = qurey.Count();
                var data = qurey.OrderByDescending(x => x.q1.EndDate).ThenByDescending(x => x.q1.StartDate).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
                foreach (var item in data)
                {
                    item.q1.SubTaskTotal = item.q3.Count();
                    item.q1.SubTasks = item.q3.ToList();
                }
                return data.Select(x => x.q1).ToList();
            }
            else
            {
                var qurey = from q1 in db.IQueryable<DepartmentTaskEntity>()
                            join q2 in db.IQueryable<DepartmentTaskEntity>() on q1.ParentTaskId equals q2.TaskId into into2
                            from q2 in into2.DefaultIfEmpty()
                            where q1.DutyUserId == userid || q1.TodoUserId == userid
                            select new { q1, q2 };
                total = qurey.Count();
                var data = qurey.OrderBy(x => x.q1.CreateDeptId).OrderByDescending(x => x.q1.EndDate).ThenByDescending(x => x.q1.StartDate).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
                foreach (var item in data)
                {
                    item.q1.ParentDutyUserId = item.q2 == null ? null : item.q2.DutyUserId;
                    item.q1.ParentCreateUserId = item.q2 == null ? null : item.q2.CreateUserId;
                    item.q1.ParentDutyDepartmentId = item.q2 == null ? null : item.q2.DutyDepartmentId;
                }
                return data.Select(x => x.q1).ToList();
            }
        }

        public List<DepartmentTaskEntity> List1(string deptid, DateTime? startdate, DateTime? enddate, string status, int pagesize, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var qurey = from q1 in db.IQueryable<DepartmentTaskEntity>()
                        join q2 in db.IQueryable<DepartmentTaskEntity>() on q1.ParentTaskId equals q2.TaskId into into2
                        from q2 in into2.DefaultIfEmpty()
                        join q3 in db.IQueryable<DepartmentTaskEntity>() on q1.TaskId equals q3.ParentTaskId into into3
                        where q1.DutyDepartmentId == deptid && (q2 == null || (q2 != null && q2.DutyDepartmentId != deptid))
                        select new { q1, total = into3.Count() };
            if (startdate != null) qurey = qurey.Where(x => x.q1.CreateTime >= startdate);
            if (enddate != null)
            {
                enddate = enddate.Value.AddDays(1);
                qurey = qurey.Where(x => x.q1.CreateTime < enddate);
            }
            if (!string.IsNullOrEmpty(status))
            {
                var date = DateTime.Today;
                switch (status)
                {
                    case "未开始":
                        qurey = qurey.Where(x => x.q1.Status == status && x.q1.StartDate > date);
                        break;
                    case "进行中":
                        qurey = qurey.Where(x => x.q1.Status == "未开始" && x.q1.StartDate <= date && x.q1.EndDate >= date);
                        break;
                    case "未完成":
                        qurey = qurey.Where(x => x.q1.Status == "未开始" && x.q1.EndDate < date);
                        break;
                    case "已完成":
                        qurey = qurey.Where(x => x.q1.Status == status);
                        break;
                    case "已取消":
                        qurey = qurey.Where(x => x.q1.Status == status);
                        break;
                    default:
                        break;
                }
            }

            total = qurey.Count();
            var data = qurey.OrderByDescending(x => x.q1.EndDate).ThenByDescending(x => x.q1.StartDate).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            foreach (var item in data)
            {
                item.q1.SubTaskTotal = item.total;
            }
            return data.Select(x => x.q1).ToList();
        }

        public List<DepartmentTaskEntity> List2(string userid, DateTime? startdate, DateTime? enddate, string status, int pagesize, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var qurey = from q1 in db.IQueryable<DepartmentTaskEntity>()
                        join q2 in db.IQueryable<DepartmentTaskEntity>() on q1.ParentTaskId equals q2.TaskId into into2
                        from q2 in into2.DefaultIfEmpty()
                        where q1.DutyUserId == userid || q1.TodoUserId == userid
                        select new { q1, q2 };
            if (startdate != null) qurey = qurey.Where(x => x.q1.CreateTime >= startdate);
            if (enddate != null)
            {
                enddate = enddate.Value.AddDays(1);
                qurey = qurey.Where(x => x.q1.CreateTime < enddate);
            }
            if (!string.IsNullOrEmpty(status))
            {
                var date = DateTime.Today;
                switch (status)
                {
                    case "未开始":
                        qurey = qurey.Where(x => x.q1.Status == status && x.q1.StartDate > date);
                        break;
                    case "进行中":
                        qurey = qurey.Where(x => x.q1.Status == "未开始" && x.q1.StartDate <= date && x.q1.EndDate >= date);
                        break;
                    case "未完成":
                        qurey = qurey.Where(x => x.q1.Status == "未开始" && x.q1.EndDate < date);
                        break;
                    case "已完成":
                        qurey = qurey.Where(x => x.q1.Status == status);
                        break;
                    case "已取消":
                        qurey = qurey.Where(x => x.q1.Status == status);
                        break;
                    default:
                        break;
                }
            }

            total = qurey.Count();
            var data = qurey.OrderBy(x => x.q1.CreateDeptId).OrderByDescending(x => x.q1.EndDate).ThenByDescending(x => x.q1.StartDate).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            foreach (var item in data)
            {
                item.q1.ParentDutyUserId = item.q2 == null ? null : item.q2.DutyUserId;
                item.q1.ParentCreateUserId = item.q2 == null ? null : item.q2.CreateUserId;
                item.q1.ParentDutyDepartmentId = item.q2 == null ? null : item.q2.DutyDepartmentId;
            }
            return data.Select(x => x.q1).ToList();
        }

        public void Publish(List<DepartmentTaskEntity> list)
        {
            var db = DbFactory.Base();
            var context = (db as BSFramework.Data.EF.Database).dbcontext;
            var dbset = db.IQueryable<DepartmentTaskEntity>() as DbSet<DepartmentTaskEntity>;

            var ids = list.Select(x => x.TaskId).ToArray();

            var entities = (from q in dbset
                            where ids.Contains(q.TaskId)
                            select q).ToList();

            foreach (var item in entities)
            {
                item.IsPublish = true;
                context.Entry(item).State = EntityState.Modified;
            }

            (db as BSFramework.Data.EF.Database).dbcontext.SaveChanges();
        }

        private void BuildRecords(DepartmentTaskEntity newEntity, DepartmentTaskEntity oldEntity, string user)
        {
            if (oldEntity.UpdateRecords == null) oldEntity.UpdateRecords = string.Empty;
            if (newEntity.Content != oldEntity.Content) oldEntity.UpdateRecords += string.Format("{0}更新于{1}，任务名称从{2}变更为{3}。{4}", user, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), oldEntity.Content, newEntity.Content, System.Environment.NewLine);
            if (newEntity.StartDate != oldEntity.StartDate || newEntity.EndDate != oldEntity.EndDate) oldEntity.UpdateRecords += string.Format("{0}更新于{1}，计划时间从{2}变更为{3}。{4}", user, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), oldEntity.StartDate.Value.ToString("yyyy-MM-dd") + "至" + oldEntity.EndDate.Value.ToString("yyyy-MM-dd"), newEntity.StartDate.Value.ToString("yyyy-MM-dd") + "至" + newEntity.EndDate.Value.ToString("yyyy-MM-dd"), System.Environment.NewLine);
            if (newEntity.DutyUser != oldEntity.DutyUser) oldEntity.UpdateRecords += string.Format("{0}更新于{1}，责任人从{2}变更为{3}。{4}", user, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), oldEntity.DutyUser, newEntity.DutyUser, System.Environment.NewLine);
            if (newEntity.Status == "已取消") oldEntity.UpdateRecords += string.Format("{0}更新于{1}，工作任务已取消{2}", user, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), System.Environment.NewLine);
        }
    }
}
