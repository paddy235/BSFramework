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
using System.Data.Entity;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class DangerMeasureService : IDangerMeasureService
    {
        public void AddMeasures(List<DangerMeasureEntity> measuredata)
        {
            using (var ctx = new DataContext())
            {
                var entities = measuredata.Select(x => new DangerMeasure()
                {
                    MeasureId = x.MeasureId,
                    MeasureContent = x.MeasureContent,
                    DangerReason = x.DangerReason,
                    CategoryId = x.CategoryId,
                    OperateTime = x.OperateTime,
                    OperateUser = x.OperateUser,
                    OperateUserId = x.OperateUserId,
                });
                ctx.DangerMeasures.AddRange(entities);

                ctx.SaveChanges();
            }
        }

        public void Delete(string id)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.DangerCategories.Find(id);
                if (entity != null)
                {
                    if (ctx.DangerMeasures.Count(x => x.CategoryId == id) > 0) throw new Exception("该类别下存在风险预控措施！");
                    ctx.DangerCategories.Remove(entity);
                    ctx.SaveChanges();
                }
            }
        }

        public void DeleteMeasure(string id)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.DangerMeasures.Find(id);
                if (entity != null)
                {
                    ctx.DangerMeasures.Remove(entity);
                    ctx.SaveChanges();
                }
            }
        }

        public bool ExistDangerReason(string categoryId, string dangerReason)
        {
            using (var ctx = new DataContext())
            {
                return ctx.DangerMeasures.Count(x => x.CategoryId == categoryId && x.DangerReason == dangerReason) > 0;
            }
        }

        public List<DangerMeasureEntity> GetAllReasons()
        {
            var result = new List<DangerMeasureEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q1 in ctx.DangerMeasures
                            join q2 in ctx.DangerCategories on q1.CategoryId equals q2.CategoryId into into1
                            from t1 in into1.DefaultIfEmpty()
                            orderby t1.Sort
                            select new { q1.MeasureId, q1.MeasureContent, Category = t1.CategoryName, q1.DangerReason, q1.OperateTime, q1.OperateUser, q1.CategoryId};

                var data = query.ToList();
                foreach (var item in data)
                {
                    result.Add(new DangerMeasureEntity() { MeasureId = item.MeasureId, MeasureContent = item.MeasureContent, Category = item.Category, CategoryId = item.CategoryId, DangerReason = item.DangerReason, OperateTime = item.OperateTime, OperateUser = item.OperateUser });
                }
            }

            return result;
        }

        public List<DangerCategoryEntity> GetCategories(string categoryid)
        {
            var result = new List<DangerCategoryEntity>();
            using (var ctx = new DataContext())
            {
                var query = ctx.DangerCategories.AsQueryable();
                if (!string.IsNullOrEmpty(categoryid))
                {
                    query = query.Where(x => x.ParentCategoryId == categoryid);
                }

                var data = query.OrderBy(x => x.Sort).ToList();
                foreach (var item in data)
                {
                    result.Add(new DangerCategoryEntity() { CategoryId = item.CategoryId, CategoryName = item.CategoryName, ParentCategoryId = item.ParentCategoryId, Sort = item.Sort });
                }
            }

            return result;
        }

        public DangerCategoryEntity GetCategory(Guid guid)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.DangerCategories.Find(guid);
                return new DangerCategoryEntity() { CategoryId = entity.CategoryId, CategoryName = entity.CategoryName, ParentCategoryId = entity.ParentCategoryId, Sort = entity.Sort };
            }
        }

        public List<DangerMeasureEntity> GetDangerReasons(string categoryid)
        {
            var result = new List<DangerMeasureEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q1 in ctx.DangerMeasures
                            join q2 in ctx.DangerCategories on q1.CategoryId equals q2.CategoryId into into1
                            from t1 in into1.DefaultIfEmpty()
                            where q1.CategoryId == categoryid
                            select new { q1.MeasureId, q1.MeasureContent, Category = t1.CategoryName, q1.DangerReason, q1.OperateTime, q1.OperateUser, q1.CategoryId };

                var data = query.ToList();
                foreach (var item in data)
                {
                    result.Add(new DangerMeasureEntity() { MeasureId = item.MeasureId, MeasureContent = string.Empty, Category = item.Category, CategoryId = item.CategoryId, DangerReason = item.DangerReason, OperateTime = item.OperateTime, OperateUser = item.OperateUser });
                }
            }

            return result;
        }

        public List<DangerMeasureEntity> GetData(string categoryid, string key, int pagesize, int page, out int total, string sortfield, string direction)
        {
            var result = new List<DangerMeasureEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q1 in ctx.DangerMeasures
                            join q2 in ctx.DangerCategories on q1.CategoryId equals q2.CategoryId into into1
                            from t1 in into1.DefaultIfEmpty()
                            select new { q1.MeasureId, q1.MeasureContent, Category = t1.CategoryName, q1.DangerReason, q1.OperateTime, q1.OperateUser, q1.CategoryId };
                if (!string.IsNullOrEmpty(categoryid))
                {
                    query = query.Where(x => x.CategoryId == categoryid);
                }
                if (!string.IsNullOrEmpty(key))
                    query = query.Where(x => x.Category.Contains(key) || x.MeasureContent.Contains(key) || x.DangerReason.Contains(key));

                total = query.Count();
                if (!string.IsNullOrEmpty(sortfield))
                {
                    if (direction == "desc") query = query.OrderByDescending(x => x.Category);
                    else query = query.OrderBy(x => x.Category);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Category);
                }
                var data = query.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                foreach (var item in data)
                {
                    result.Add(new DangerMeasureEntity() { MeasureId = item.MeasureId, MeasureContent = item.MeasureContent, Category = item.Category, DangerReason = item.DangerReason, OperateTime = item.OperateTime, OperateUser = item.OperateUser });
                }
            }

            return result;
        }

        public DangerMeasureEntity GetMeasureDetail(string measureid)
        {
            var result = default(DangerMeasureEntity);
            using (var ctx = new DataContext())
            {
                var entity = ctx.DangerMeasures.Include("Category").FirstOrDefault(x => x.MeasureId == measureid);
                if (entity != null)
                {
                    result = new DangerMeasureEntity() { MeasureId = entity.MeasureId, MeasureContent = entity.MeasureContent, CategoryId = entity.Category.CategoryId, Category = entity.Category.CategoryName, DangerReason = entity.DangerReason, OperateTime = entity.OperateTime, OperateUser = entity.OperateUser };
                }
            }
            return result;
        }

        public List<DangerMeasureEntity> GetMeasures(string humandangerid, string measureid)
        {
            var result = new List<DangerMeasureEntity>();

            using (var ctx = new DataContext())
            {
                var humandanger = default(HumanDanger);
                if (humandangerid != null)
                    humandanger = ctx.HumanDangers.Include("Measures").FirstOrDefault(x => x.HumanDangerId == humandangerid);

                if (humandanger == null)
                {
                    var measure = ctx.DangerMeasures.Find(measureid);
                    if (measure == null || string.IsNullOrEmpty(measure.MeasureContent)) return result;
                    else
                    {
                        result.Add(new DangerMeasureEntity() { MeasureId = measure.MeasureId, MeasureContent = measure.MeasureContent });
                        var measurelist = ctx.DangerMeasures.Where(x => x.CategoryId == measure.CategoryId).Select(x => new DangerMeasureEntity { MeasureId = x.MeasureId, MeasureContent = x.MeasureContent }).ToList();
                        result.AddRange(this.TakeTwo(measurelist, measure.MeasureId));
                    }
                }
                else
                {
                    var measure = humandanger.Measures.Find(x => x.MeasureId == measureid);
                    if (measure == null || string.IsNullOrEmpty(measure.MeasureContent))
                    {
                        var measure2 = ctx.DangerMeasures.Find(measureid);
                        if (measure2 == null || string.IsNullOrEmpty(measure2.MeasureContent)) return result;
                        else
                        {
                            result.Add(new DangerMeasureEntity() { MeasureId = measure2.MeasureId, MeasureContent = measure2.MeasureContent });
                            var measurelist = ctx.DangerMeasures.Where(x => x.CategoryId == measure2.CategoryId).Select(x => new DangerMeasureEntity { MeasureId = x.MeasureId, MeasureContent = x.MeasureContent }).ToList();
                            result.AddRange(this.TakeTwo(measurelist, measure2.MeasureId));
                        }
                    }
                    else
                    {
                        result.Add(new DangerMeasureEntity() { MeasureId = measure.MeasureId, MeasureContent = measure.MeasureContent });
                        var measurelist = ctx.DangerMeasures.Where(x => x.CategoryId == measure.CategoryId).Select(x => new DangerMeasureEntity { MeasureId = x.MeasureId, MeasureContent = x.MeasureContent }).ToList();
                        result.AddRange(this.TakeTwo(measurelist, measure.MeasureId));
                    }
                }
            }

            result = this.Sort(result);
            return result;
        }

        private List<DangerMeasureEntity> TakeTwo(List<DangerMeasureEntity> measures, string mid)
        {
            measures = measures.Where(x => !string.IsNullOrEmpty(x.MeasureContent) && x.MeasureId != mid).ToList();
            var result = new List<DangerMeasureEntity>();
            var random = new Random();
            var max = 2;
            if (measures.Count < 2) max = measures.Count;
            for (int i = 0; i < max; i++)
            {
                var num = random.Next(measures.Count);
                if (result.Any(x => x.MeasureId == measures[num].MeasureId))
                {
                    i--;
                    continue;
                }
                result.Add(measures[num]);
                measures.RemoveAt(num);
            }
            return result;
        }

        private List<DangerMeasureEntity> Sort(List<DangerMeasureEntity> measures)
        {
            var result = new List<DangerMeasureEntity>();
            var random = new Random();
            foreach (var item in measures)
            {
                result.Insert(random.Next(result.Count), item);
            }
            return result;
        }

        public List<string> GetTaskAreas(string key, int pageSize, int pageIndex, out int total)
        {
            var result = default(List<string>);
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.HumanDangers
                            orderby q.Task
                            select q.TaskArea;

                if (!string.IsNullOrEmpty(key)) query = query.Where(x => x.Contains(key));

                total = query.Count();
                result = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            return result;
        }

        public void Save(DangerCategoryEntity model)
        {
            using (var ctx = new DataContext())
            {
                if (ctx.DangerCategories.Count(x => x.CategoryId != model.CategoryId && x.CategoryName == model.CategoryName) > 0) throw new Exception("该风险类别已存在！");
                if (ctx.DangerCategories.Count(x => x.Sort == model.Sort && x.CategoryId != model.CategoryId) > 0) throw new Exception("排序重复！");
                var entity = ctx.DangerCategories.Find(model.CategoryId);
                if (entity == null)
                {
                    model.CategoryId = Guid.NewGuid().ToString();
                    entity = new DangerCategory() { CategoryId = model.CategoryId, CategoryName = model.CategoryName, Sort = model.Sort };
                    ctx.DangerCategories.Add(entity);
                }
                else
                {
                    entity.CategoryName = model.CategoryName;
                    entity.Sort = model.Sort;
                    var dangermeasures = ctx.HumanDangers.SelectMany(x => x.Measures).Where(x => x.CategoryId == model.CategoryId);
                    foreach (var item in dangermeasures)
                    {
                        item.Category = model.CategoryName;
                    }
                }

                ctx.SaveChanges();
            }
        }

        public void SaveMeasure(DangerMeasureEntity model)
        {
            using (var ctx = new DataContext())
            {
                if (ctx.DangerMeasures.Count(x => x.MeasureId != model.MeasureId && x.CategoryId == model.CategoryId && x.DangerReason == model.DangerReason) > 0) throw new Exception("该风险因素已存在！");

                var entity = ctx.DangerMeasures.Find(model.MeasureId);
                if (entity == null)
                {
                    entity = new DangerMeasure() { MeasureId = Guid.NewGuid().ToString(), MeasureContent = model.MeasureContent, CategoryId = model.CategoryId, DangerReason = model.DangerReason, OperateTime = model.OperateTime, OperateUser = model.OperateUser, OperateUserId = model.OperateUserId };
                    ctx.DangerMeasures.Add(entity);
                }
                else
                {
                    entity.MeasureContent = model.MeasureContent;
                    entity.DangerReason = model.DangerReason;
                    entity.CategoryId = model.CategoryId;
                    entity.OperateTime = model.OperateTime;
                    entity.OperateUser = model.OperateUser;
                    entity.OperateUserId = model.OperateUserId;
                    ctx.Entry(entity).State = EntityState.Modified;
                }

                int a = ctx.SaveChanges();
            }
        }
    }
}
