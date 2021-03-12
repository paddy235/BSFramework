using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.WorkPlanManage;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;

namespace BSFramework.Application.Service.WorkPlanManage
{
    public class WorkPlanService : IWorkPlanService
    {
        public IEnumerable<WorkPlanEntity> GetPlanList()
        {
            var db = new RepositoryFactory<WorkPlanEntity>().BaseRepository();
            var query = db.IQueryable();
            return query.ToList();
        }

        public IEnumerable<WorkPlanContentEntity> GetContentList(string planid)
        {
            var db = new RepositoryFactory<WorkPlanContentEntity>().BaseRepository();
            var query = db.IQueryable();
            if (!string.IsNullOrEmpty(planid)) query = query.Where(x => x.PlanId == planid);
            return query.ToList();
        }

        public void RemoveWorkPlan(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<WorkPlanEntity>(keyValue);
            var ctx = new DataContext();

            var roles = new string[] { "班长", "副班长", "技术员" };
            var query = from q in db.IQueryable<UserEntity>()
                        where q.UserId == null
                        select q.UserId;
            foreach (var item in roles)
            {
                query = query.Concat(from q1 in db.IQueryable<UserEntity>()
                                     join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                                     where q1.DepartmentId == entity.UseDeptId && ("," + q2.Quarters + ",").Contains("," + item + ",")
                                     select q1.UserId);
            }

            var users = query.ToList();
            foreach (var item in users)
            {
                ctx.Messages.Add(new Message()
                {
                    MessageId = Guid.NewGuid(),
                    BusinessId = keyValue,
                    Content = string.Format("{0}{1}已取消，请您及时处理。", entity.PlanType, entity.StartDate.ToString("yyyy-M-d") + " - " + entity.EndDate.ToString("y-M-d")),
                    Title = "工作计划取消",
                    Category = MessageCategory.Message,
                    MessageKey = "工作计划取消",
                    CreateTime = DateTime.Now,
                    UserId = item
                });
            }
            ctx.SaveChanges();

            db.Delete<WorkPlanEntity>(keyValue);
        }

        public void RemoveWorkPlanContent(string keyValue)
        {
            var db = new RepositoryFactory<WorkPlanContentEntity>().BaseRepository();
            db.Delete(keyValue);
        }

        public void SaveWorkPlan(string keyValue, WorkPlanEntity en)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<WorkPlanEntity>(keyValue);

            if (entity == null)
            {
                db.Insert(en);
            }
            else
            {
                if (en.DeleteRemark == true)
                {
                    var ctx = new DataContext();

                    var roles = new string[] { "班长", "副班长", "技术员" };
                    var query = from q in db.IQueryable<UserEntity>()
                                where q.UserId == null
                                select q.UserId;
                    foreach (var item in roles)
                    {
                        query = query.Concat(from q1 in db.IQueryable<UserEntity>()
                                             join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                                             where q1.DepartmentId == entity.UseDeptId && ("," + q2.Quarters + ",").Contains("," + item + ",")
                                             select q1.UserId);
                    }

                    var users = query.ToList();
                    foreach (var item in users)
                    {
                        ctx.Messages.Add(new Message()
                        {
                            MessageId = Guid.NewGuid(),
                            BusinessId = keyValue,
                            Content = string.Format("{0}{1}已取消，请您及时处理。", entity.PlanType, entity.StartDate.ToString("yyyy-M-d") + " - " + entity.EndDate.ToString("y-M-d")),
                            Title = "工作计划取消",
                            Category = MessageCategory.Message,
                            MessageKey = "工作计划取消",
                            CreateTime = DateTime.Now,
                            UserId = item
                        });
                    }
                    ctx.SaveChanges();

                }

                entity.CreateDate = en.CreateDate;
                entity.CreateUser = en.CreateUser;
                entity.CreateUserId = en.CreateUserId;
                entity.DeleteRemark = en.DeleteRemark;
                entity.EndDate = en.EndDate;
                entity.IsFinished = en.IsFinished;
                entity.PlanType = en.PlanType;
                entity.Remark = en.Remark;
                entity.StartDate = en.StartDate;
                entity.UseDeptCode = en.UseDeptCode;
                entity.UseDeptId = en.UseDeptId;
                entity.UseDeptName = en.UseDeptName;
                db.Update(entity);
            }
        }

        public void SaveWorkPlanContent(string keyValue, WorkPlanContentEntity entity)
        {
            var db = new RepositoryFactory<WorkPlanContentEntity>().BaseRepository();
            var entity1 = this.GetWorkPlanContentEntity(keyValue);
            if (entity1 == null)
            {
                db.Insert(entity);
            }
            else
            {
                entity.Start = null;
                entity.End = null;
                entity.ChildrenContent = null;
                entity.PlanType = null;
                db.Update(entity);
            }
        }

        public WorkPlanEntity GetWorkPlanEntity(string keyValue)
        {
            var db = new RepositoryFactory<WorkPlanEntity>().BaseRepository();
            return db.FindEntity(keyValue);
        }

        public WorkPlanContentEntity GetWorkPlanContentEntity(string keyValue)
        {
            var db = new RepositoryFactory<WorkPlanContentEntity>().BaseRepository();
            return db.FindEntity(keyValue);
        }
    }
}
