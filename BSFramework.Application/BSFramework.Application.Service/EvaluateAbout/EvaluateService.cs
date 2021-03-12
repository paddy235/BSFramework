using BSFramework.Entity.WorkMeeting;
using BSFramework.Data.Repository;
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
using BSFramework.IService.EvaluateAbout;
using BSFramework.Entity.EvaluateAbout;
using System.Data;
using BSFramework.Application.Entity.EvaluateAbout;
using Newtonsoft.Json;
using BSFramework.Data.EF;

namespace BSFramework.Service.EvaluateAbout
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EvaluateService : RepositoryFactory<WorkmeetingEntity>, IEvaluateService
    {
        private System.Data.Entity.DbContext _context;

        public EvaluateService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public void Add(EvaluateCategoryEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(model);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void AddItem(List<EvaluateCategoryItemEntity> models)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(models);
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void AddItem(EvaluateCategoryItemEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(model);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void DeleteCategory(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var cnt = db.IQueryable<EvaluateCategoryEntity>(x => x.ParentCategoryId == id).Count();
                if (cnt > 0) throw new Exception("请先删除子类别");
                cnt = db.IQueryable<EvaluateCategoryItemEntity>(x => x.CategoryId == id).Count();
                if (cnt > 0) throw new Exception("请先删除考评内容");
                db.Delete<EvaluateCategoryEntity>(id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void DeleteItem(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<EvaluateCategoryItemEntity>(id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void Edit(EvaluateCategoryEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<EvaluateCategoryEntity>(model.CategoryId);
                entity.Category = model.Category;
                entity.ParentCategoryId = model.ParentCategoryId;
                entity.SortCode = model.SortCode;
                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void EditEvaluateItem(EvaluateItemEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<EvaluateItemEntity>(model.EvaluateItemId);
                entity.ActualScore = model.ActualScore;
                entity.Reason = model.Reason;
                entity.EvaluateDept = model.EvaluateDept;
                entity.EvaluatePerson = model.EvaluatePerson;
                entity.EvaluateTime = model.EvaluateTime;
                entity.Pct = null;
                #region 计算权重
                var evaluatecategories = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                         select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.ParentCategoryId };
                var sss = evaluatecategories.ToList();//所有的分类

                //获取该条数据的分类Id
                var query = from a in db.IQueryable<EvaluateItemEntity>()
                            join c in db.IQueryable<EvaluateCategoryItemEntity>() on a.EvaluateContentId equals c.ItemId
                            where a.EvaluateItemId == entity.EvaluateItemId
                            select new { c.CategoryId };
                //根据categoryId判断权重
                var category = query.ToList().FirstOrDefault();
                var weidthList = db.IQueryable<WeightSetEntity>().Where(p => p.IsFiring == 1).ToList();
                WeightSetEntity weight = null;
                var rootId = string.Empty;
                if (category != null)
                {
                    if (weidthList.Any(x => x.Id == category.CategoryId))
                    {
                        weight = weidthList.Where(x => x.Id == category.CategoryId).FirstOrDefault();
                    }
                    else
                    {
                        var categoryObj = sss.Where(x => x.CategoryId == category.CategoryId).FirstOrDefault();
                        if (categoryObj != null)
                        {
                            rootId = categoryObj.ParentCategoryId;
                            if (weidthList.Any(x => x.Id == categoryObj.ParentCategoryId))
                            {
                                weight = weidthList.Where(x => x.Id == rootId).FirstOrDefault();
                            }
                            else
                            {
                                var sendsObj = sss.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                if (sendsObj != null)
                                {
                                    rootId = sendsObj.ParentCategoryId;
                                    if (weidthList.Any(x => x.Id == sendsObj.ParentCategoryId))
                                    {
                                        weight = weidthList.Where(x => x.Id == rootId).FirstOrDefault();
                                    }
                                    else
                                    {
                                        var rootObj = sss.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                        if (rootObj != null)
                                        {
                                            rootId = rootObj.ParentCategoryId;
                                            if (weidthList.Any(x => x.Id == rootObj.ParentCategoryId))
                                            {
                                                weight = weidthList.Where(x => x.Id == rootObj.ParentCategoryId).FirstOrDefault();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (weight != null && weight.Weight != null)
                    {
                        entity.WeightScore = Math.Round(Convert.ToDecimal(entity.ActualScore * weight.Weight), 2);
                    }
                    else
                    {
                        entity.WeightScore = entity.ActualScore;
                    }
                }
                else
                {
                    entity.WeightScore = entity.ActualScore;
                }
                #endregion

                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void EditItem(EvaluateCategoryItemEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<EvaluateCategoryItemEntity>(model.ItemId);
                entity.CategoryId = model.CategoryId;
                entity.ItemContent = model.ItemContent;
                entity.ItemStandard = model.ItemStandard;
                entity.Score = model.Score;
                entity.EvaluateDept = model.EvaluateDept;
                entity.UseDept = model.UseDept;
                entity.UseDeptId = model.UseDeptId;
                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public void EnsureEvaluate(string name, DateTime date)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var cnt = db.IQueryable<EvaluateEntity>().Count(x => x.EvaluateSeason == name);
            try
            {
                if (cnt == 0)
                {
                    var evaluation = new EvaluateEntity() { EvaluateId = Guid.NewGuid().ToString(), CreateTime = date, EvaluateSeason = name, PublishDate = date };
                    var groups = (from q1 in db.IQueryable<DepartmentEntity>()
                                  join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                                  where q1.Nature == "班组"
                                  orderby q2.EnCode
                                  select new { GroupId = q1.DepartmentId, GroupName = q1.FullName, DeptId = q2.DepartmentId, DeptName = q2.FullName }).ToList();
                    var evaluategroups = groups.Select(x => new EvaluateGroupEntity() { EvaluateGroupId = Guid.NewGuid().ToString(), EvaluateId = evaluation.EvaluateId, DeptId = x.DeptId, DeptName = x.DeptName, GroupId = x.GroupId, GroupName = x.GroupName, CreateTime = DateTime.Now }).ToList();
                    var categories = db.IQueryable<EvaluateCategoryItemEntity>().ToList();
                    var evaluateitems = new List<EvaluateItemEntity>();
                    evaluategroups.ForEach(x =>
                    {
                        evaluateitems.AddRange(categories.Where(y => string.IsNullOrEmpty(y.UseDept) || y.UseDept.Contains(x.DeptName)).Select(y => new EvaluateItemEntity() { EvaluateItemId = Guid.NewGuid().ToString(), EvaluateGroupId = x.EvaluateGroupId, EvaluateContentId = y.ItemId, EvaluateContent = y.ItemContent, CreateTime = DateTime.Now, ActualScore = y.Score, Score = y.Score }));
                    });

                    db.Insert(evaluation);
                    db.Insert(evaluategroups);
                    db.Insert(evaluateitems);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void Evaluate(string id, List<EvaluateItemEntity> entities)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var evaluations = (from q1 in db.IQueryable<EvaluateItemEntity>()
                                   join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                                   where q2.EvaluateId == id
                                   select q1).ToList();

                foreach (var item in evaluations)
                {
                    item.ActualScore = entities.Find(x => x.EvaluateItemId == item.EvaluateItemId).ActualScore;
                }

                db.Update(evaluations);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public IList<EvaluateCategoryEntity> GetAllCategories()
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<EvaluateCategoryEntity>().OrderBy(x => x.SortCode).ToList();
        }
        public IList<EvaluateDeptEntity> GetAllDeptsById(string eid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<EvaluateDeptEntity>().Where(x => x.EvaluateId == eid).ToList();
        }
        public IList<EvaluateCategoryItemEntity> GetAllCategoryItems(string deptname)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<EvaluateCategoryItemEntity>()
                        join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.CategoryId
                        select new { q1.ItemId, q1.CategoryId, q1.ItemContent, q1.ItemStandard, q1.EvaluateDept, q1.CreateTime, q1.Score, q2.Category, CreateTime1 = q2.CreateTime, q2.ParentCategoryId };
            if (!string.IsNullOrEmpty(deptname)) query = query.Where(x => x.EvaluateDept == deptname);

            query = query.OrderBy(x => x.Category).ThenBy(x => x.CreateTime1).ThenBy(x => x.CreateTime);

            return query.ToList().Select(x => new EvaluateCategoryItemEntity() { ItemId = x.ItemId, ItemContent = x.ItemContent, ItemStandard = x.ItemStandard, CategoryId = x.CategoryId, CreateTime = x.CreateTime, EvaluateDept = x.EvaluateDept, Score = x.Score, Category = new EvaluateCategoryEntity() { CategoryId = x.CategoryId, Category = x.Category, CreateTime = x.CreateTime1, ParentCategoryId = x.ParentCategoryId } }).ToList();
        }
        public IList<EvaluateCategoryItemEntity> GetAllCategoryItemsIndex(string deptname)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<EvaluateCategoryItemEntity>()
                        join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.CategoryId
                        select new { q1.ItemId, q1.CategoryId, q1.ItemContent, q1.ItemStandard, q1.EvaluateDept, q1.CreateTime, q1.Score, q2.Category, CreateTime1 = q2.CreateTime, q2.ParentCategoryId };
            if (!string.IsNullOrEmpty(deptname)) query = query.Where(x => deptname.Contains(x.EvaluateDept));

            query = query.OrderBy(x => x.Category).ThenBy(x => x.CreateTime1).ThenBy(x => x.CreateTime);

            return query.ToList().Select(x => new EvaluateCategoryItemEntity() { ItemId = x.ItemId, ItemContent = x.ItemContent, ItemStandard = x.ItemStandard, CategoryId = x.CategoryId, CreateTime = x.CreateTime, EvaluateDept = x.EvaluateDept, Score = x.Score, Category = new EvaluateCategoryEntity() { CategoryId = x.CategoryId, Category = x.Category, CreateTime = x.CreateTime1, ParentCategoryId = x.ParentCategoryId } }).ToList();
        }

        public EvaluateCalcEntity GetAvgScore(string evaluateid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var q1 = db.IQueryable<EvaluateCategoryEntity>().Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.ParentCategoryId, y => y.CategoryId, (x, y) => new { x, y }).Where(x => x.y.ParentCategoryId == null).Select(x => x.x);

            var query = db.IQueryable<EvaluateItemEntity>().Join(db.IQueryable<EvaluateGroupEntity>(), x => x.EvaluateGroupId, y => y.EvaluateGroupId, (x, y) => new { y.EvaluateId, y.GroupId, y.GroupName, x.EvaluateContentId, x.ActualScore, x.Score }).Where(x => x.EvaluateId == evaluateid).Join(db.IQueryable<EvaluateCategoryItemEntity>(), x => x.EvaluateContentId, y => y.ItemId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId }).Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId });

            var cnt = 0;
            while (query.Join(q1, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId }).Count() == 0 && cnt < 10)
            {
                query = query.Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.ParentCategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId });
                cnt++;
            }

            var qq = query.Join(q1, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId, y.Category, y.CreateTime }).GroupBy(x => new { x.Category, x.CreateTime }, (x, g) => new { x.Category, x.CreateTime, g = g.ToList() }).OrderBy(x => x.CreateTime).ToList();

            return new EvaluateCalcEntity() { Season = "平均值", Data = qq.Select(x => new { Category = x.Category, groups = x.g.GroupBy(y => y.GroupId, (y, g) => new { y, ActualScore = g.Sum(z => z.ActualScore), Score = g.Sum(z => z.Score) }).ToList() }).Select(x => new { Category = x.Category, g = x.groups.Select(y => new { y.y, y.ActualScore, y.Score, Pct = (decimal)y.ActualScore / (y.Score == 0 ? y.ActualScore == 0 ? 1 : y.ActualScore : y.Score) * 100 }) }).Select(x => new EvaluateItemCalcEntity() { Category = x.Category, ActualScore = x.g.Average(y => y.ActualScore), Score = x.g.Average(y => y.Score), Pct = x.g.Average(y => y.Pct) }).ToList() };
        }

        public IList<EvaluateCategoryEntity> GetBigCategories()
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                        join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                        where q2.ParentCategoryId == null
                        orderby q1.CreateTime
                        select q1;
            return query.ToList();
        }
        public DataTable GetGroups(string evaluateid, string itemcontentid, string type)
        {
            var db = new RepositoryFactory().BaseRepository();
            //本次考评中，该项考评内容适用的 分数不一致的班组数量
            string sql = string.Format(@"select *,score as(b.Score-b.actualscore) from wg_evaluategroup a where evaluategroupid in(
select evaluategroupid from wg_evaluateitem where evaluategroupid in (
select evaluategroupid from wg_evaluategroup 
where evaluateid='{0}')
and evaluatecontentid = '{1}'
and actualscore != score
) left join  wg_evaluateitem b on a.evaluategroupid=b.evaluategroupid ", evaluateid, itemcontentid);
            if (type == "1") //本次考评，该项考评内容适用的所有班组数量
            {
                sql = string.Format(@"select * from wg_evaluategroup where evaluategroupid in(
select evaluategroupid from wg_evaluateitem where evaluategroupid in (
select evaluategroupid from wg_evaluategroup 
where evaluateid='{0}')
and evaluatecontentid = '{1}'
)", evaluateid, itemcontentid);
            }
            DataTable dt = db.FindTable(sql);
            return dt;
        }
        public DataTable GetGroupsIndex(string evaluateid, string itemcontentid, string type)
        {
            var db = new RepositoryFactory().BaseRepository();
            //本次考评中，该项考评内容适用的 分数不一致的班组数量
            string sql = string.Format(@"select b.*,a.Score,a.actualscore,a.createtime modifytime,a.reason,c.categoryid from wg_evaluateitem  a
left join  wg_evaluategroup b on a.evaluategroupid=b.evaluategroupid  LEFT JOIN wg_EvaluateCategoryItem c on a.EvaluateContentId = c.ItemId  where a.evaluateitemid in(
select evaluateitemid from wg_evaluateitem where evaluategroupid in (
select evaluategroupid from wg_evaluategroup 
where evaluateid='{0}')
and actualscore != score
)  ", evaluateid);
            if (type == "1") //本次考评，该项考评内容适用的所有班组数量
            {
                sql = string.Format(@"select b.*,a.Score,a.actualscore,a.createtime modifytime,a.reason,c.categoryid from wg_evaluateitem  a
left join  wg_evaluategroup b on a.evaluategroupid=b.evaluategroupid   LEFT JOIN wg_EvaluateCategoryItem c on a.EvaluateContentId = c.ItemId  where a.evaluateitemid in(
select evaluateitemid from wg_evaluateitem where evaluategroupid in (
select evaluategroupid from wg_evaluategroup 
where evaluateid='{0}')
) ", evaluateid);
            }
            DataTable dt = db.FindTable(sql);
            return dt;
        }
        public IList<EvaluateGroupEntity> GetCalcScore(string id, string categoryid)
        {
            if (string.IsNullOrEmpty(categoryid)) categoryid = null;

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                 join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                                 where q2.ParentCategoryId == null
                                 select q1;

            if (!string.IsNullOrEmpty(categoryid)) categorygroups = categorygroups.Where(x => x.CategoryId == categoryid);

            var categories = from q1 in categorygroups
                             select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                          join q2 in categorygroups on q1.ParentCategoryId equals q2.CategoryId
                          select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            }

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                        join q4 in categories on q3.CategoryId equals q4.CategoryId
                        join q5 in db.IQueryable<DepartmentEntity>() on q2.DeptId equals q5.DepartmentId into into1
                        from t1 in into1.DefaultIfEmpty()
                        join q6 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q6.DepartmentId into into2
                        from t2 in into2.DefaultIfEmpty()
                        where q2.EvaluateId == id
                        select new { q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, q1.ActualScore, q1.Score, q2.DeptId, q2.DeptName, q1.WeightScore };// q1.WeightScore 权重分

            var metadata = query.ToList();
            var groups = metadata.GroupBy(x => new { x.GroupName, x.EnCode1, x.EnCode2, x.DeptId, x.DeptName }).Select(x => new { x.Key.GroupName, ActualScore = x.Select(y => y.ActualScore).DefaultIfEmpty(0).Sum(), Score = x.Select(y => y.Score).DefaultIfEmpty(0).Sum(), WeightScore = x.Select(p => p.WeightScore).DefaultIfEmpty().Sum(), DeptId = x.Key.DeptId, x.Key.DeptName }).ToList();
            //group new { q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, q1.ActualScore, q1.Score } by new { q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode } into g
            //orderby g.Key.EnCode1, g.Key.EnCode2
            //select new { g.Key.GroupName, ActualScore = ((int?)g.Select(x => x.ActualScore).DefaultIfEmpty(0).Sum()), Score = ((int?)g.Select(x => x.Score).DefaultIfEmpty(0).Sum()) };

            return groups.ToList().Select(x => new EvaluateGroupEntity() { DeptId = x.DeptId, DeptName = x.DeptName, GroupName = x.GroupName, ActualScore = x.WeightScore, Pct = (decimal)x.ActualScore / (x.Score == 0 ? x.ActualScore == 0 ? 1 : x.ActualScore : x.Score) * 100 }).OrderByDescending(x => x.Pct).ToList();//ActualScore得分替换成了WeightScore权重分

            //var query = from q1 in db.IQueryable<EvaluateGroupEntity>()
            //            join q2 in db.IQueryable<EvaluateItemEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
            //            join q3 in db.IQueryable<DepartmentEntity>() on q1.DeptId equals q3.DepartmentId into into1
            //            from t1 in into1.DefaultIfEmpty()
            //            join q4 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q4.DepartmentId into into2
            //            from t2 in into2.DefaultIfEmpty()
            //            where q1.EvaluateId == id
            //            group new { q1.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, q2.ActualScore, q2.Score } by new { q1.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode } into g
            //            select new { g.Key.GroupName, Pct = (decimal)g.Sum(x => x.ActualScore) / g.Sum(x => x.Score) * 100 };

            //var query = db.IQueryable<EvaluateGroupEntity>().Join(db.IQueryable<EvaluateItemEntity>(), x => x.EvaluateGroupId, y => y.EvaluateGroupId, (x, y) => new { x.EvaluateId, y.EvaluateContentId, x.GroupName, x.DeptId, x.GroupId, y.ActualScore, y.Score }).GroupJoin(db.IQueryable<DepartmentEntity>(), x => x.DeptId, y => y.DepartmentId, (x, y) => new { x, y }).SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x.EvaluateId, x.x.EvaluateContentId, x.x.GroupName, x.x.ActualScore, x.x.Score, x.x.GroupId, EnCode1 = y.EnCode }).GroupJoin(db.IQueryable<DepartmentEntity>(), x => x.GroupId, y => y.DepartmentId, (x, y) => new { x, y }).SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x.x.EvaluateId, x.x.EvaluateContentId, x.x.GroupName, x.x.ActualScore, x.x.Score, x.x.EnCode1, EnCode2 = y.EnCode }).Where(x => x.EvaluateId == id);

            //if (!string.IsNullOrEmpty(categoryid))
            //{
            //    var categories = db.FindList<EvaluateCategoryEntity>("select * from wg_evaluatecategory where find_in_set(categoryid, fn_recursive1(@id))", new DbParameter[] { DbParameters.CreateDbParameter("@id", categoryid) }).Select(x => x.CategoryId).ToList();
            //    var categoryitems = db.IQueryable<EvaluateCategoryItemEntity>().Where(x => categories.Contains(x.CategoryId));
            //    flag = categoryitems.Sum(x => x.Score) == 0;
            //    query = query.Where(x => categoryitems.Select(y => y.ItemId).Contains(x.EvaluateContentId));
            //}

            //return query.GroupBy(x => new
            //{
            //    x.GroupName,
            //    x.EnCode1,
            //    x.EnCode2
            //}).Select(x => new { x.Key.GroupName, x.Key.EnCode1, x.Key.EnCode2, ActualScore = x.Sum(y => y.ActualScore), Score = x.Sum(y => y.Score) }).ToList().Select(x => new EvaluateGroupEntity { GroupName = x.GroupName, ActualScore = x.ActualScore, Pct = flag ? (decimal)x.ActualScore : (decimal)x.ActualScore / x.Score * 100 }).OrderByDescending(x => x.Pct).ToList();
        }

        public IList<GroupIndex> GetCalcScoreIndex(string id, string categoryid, string deptcode)
        {
            if (string.IsNullOrEmpty(categoryid)) categoryid = null;

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                 join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                                 where q2.ParentCategoryId == null
                                 select q1;

            if (!string.IsNullOrEmpty(categoryid)) categorygroups = categorygroups.Where(x => x.CategoryId == categoryid);

            var categories = from q1 in categorygroups
                             select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId, TopCategoryId = q1.CategoryId, TopCategory = q1.Category };

            var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                          join q2 in categorygroups on q1.ParentCategoryId equals q2.CategoryId
                          select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId, TopCategoryId = q2.CategoryId, TopCategory = q2.Category };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId, TopCategoryId = q1.TopCategoryId, TopCategory = q1.TopCategory };
            }

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                        join q4 in categories on q3.CategoryId equals q4.CategoryId
                        join q5 in db.IQueryable<DepartmentEntity>() on q2.DeptId equals q5.DepartmentId
                        into into1
                        from t1 in into1.DefaultIfEmpty()
                        where t1.EnCode.StartsWith(deptcode)
                        join q6 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q6.DepartmentId into into2
                        from t2 in into2.DefaultIfEmpty()
                        where q2.EvaluateId == id
                        select new { q4.TopCategory, q4.TopCategoryId, q2.DeptId, q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, q1.ActualScore, q1.Score, q1.WeightScore };

            var metadata = query.ToList();
            var groups = metadata.GroupBy(x => new { x.TopCategory, x.DeptId, x.GroupName, x.EnCode1, x.EnCode2 }).Select(x => new { x.Key.GroupName, x.Key.TopCategory, ActualScore = x.Select(y => y.WeightScore).DefaultIfEmpty(0).Sum().Value, Score = x.Select(y => y.Score).DefaultIfEmpty(0).Sum(), DeptId = x.Key.DeptId }).ToList();

            return groups.ToList().Select(x => new GroupIndex() { Category = x.TopCategory, GroupName = x.GroupName, Score1 = x.ActualScore, Score = x.Score, DeptId = x.DeptId }).ToList();


        }
        public IList<EvaluateCalcEntity> GetCalcScore2(string year, string groupid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                 join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                                 where q2.ParentCategoryId == null
                                 select q1;

            var categories = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                             join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                             where q2.ParentCategoryId == null
                             select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                          join q2 in categories on q1.ParentCategoryId equals q2.CategoryId
                          select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            }

            var seasons = db.IQueryable<EvaluateEntity>().Where(x => x.EvaluateSeason.Contains(year)).ToList();
            var groupdata = categorygroups.OrderBy(x => x.CreateTime).ToList();
            var data = new List<EvaluateCalcEntity>();
            foreach (var item in seasons)
            {
                var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                            join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                            join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                            join q4 in categories on q3.CategoryId equals q4.CategoryId
                            where q2.EvaluateId == item.EvaluateId && q2.GroupId == groupid
                            select new { q4.ParentCategoryId, q1.ActualScore, q1.Score };

                var metadata = query.ToList();
                var dd = groupdata.GroupJoin(metadata, x => x.CategoryId, y => y.ParentCategoryId, (x, y) => new { x.Category, ActualScore = y.Select(z => z.ActualScore).DefaultIfEmpty(0).Sum(), Score = y.Select(z => z.Score).DefaultIfEmpty(0).Sum() }).ToList();

                data.Add(new EvaluateCalcEntity() { SeasonId = item.EvaluateId, Season = item.EvaluateSeason, Data = dd.Select(x => new EvaluateItemCalcEntity() { Category = x.Category, ActualScore = x.ActualScore, Score = x.Score, Pct = (decimal)x.ActualScore / (x.Score == 0 ? x.ActualScore == 0 ? 1 : x.ActualScore : x.Score) * 100 }).ToList() });
            }

            //var query = db.IQueryable<EvaluateItemEntity>().Join(db.IQueryable<EvaluateGroupEntity>(), x => x.EvaluateGroupId, y => y.EvaluateGroupId, (x, y) => new { y.EvaluateId, y.GroupId, x.EvaluateContentId, x.ActualScore, x.Score }).Join(db.IQueryable<EvaluateEntity>().Where(x => x.EvaluateSeason.Contains(year)), x => x.EvaluateId, y => y.EvaluateId, (x, y) => new { x.GroupId, x.EvaluateContentId, y.EvaluateSeason, x.ActualScore, x.Score }).Join(db.IQueryable<EvaluateCategoryItemEntity>(), x => x.EvaluateContentId, y => y.ItemId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.EvaluateSeason, x.ActualScore, x.Score, y.CategoryId }).Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.EvaluateSeason, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId });

            //var cnt = 0;
            //while (query.Join(q1, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.EvaluateSeason, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId }).Where(x => x.GroupId == groupid).Count() == 0 && db.IQueryable<DepartmentEntity>().Count(x => x.DepartmentId == groupid) > 0 && cnt < 10)
            //{
            //    query = query.Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.ParentCategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.EvaluateSeason, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId });
            //    cnt++;
            //}

            //var qq = query.Join(q1, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.EvaluateSeason, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId, y.Category, y.CreateTime }).Where(x => x.GroupId == groupid).GroupBy(x => x.EvaluateSeason, (x, g) => new { EvaluateSeason = x, g = g.OrderBy(y => y.CreateTime).ToList() }).ToList();

            //return qq.Select(x => new { EvaluateSeason = x.EvaluateSeason, Data = x.g.GroupBy(y => y.Category, (y, g) => new { Category = y, ActualScore = g.Sum(z => z.ActualScore), Score = g.Sum(z => z.Score) }).ToList() }).Select(x => new EvaluateCalcEntity { Season = x.EvaluateSeason, Data = x.Data.Select(y => new EvaluateItemCalcEntity { Category = y.Category, ActualScore = y.ActualScore, Score = y.Score, Pct = (decimal)y.ActualScore / (y.Score == 0 ? y.ActualScore == 0 ? 1 : y.ActualScore : y.Score) * 100 }).ToList() }).ToList();
            return data;
        }

        public EvaluateCalcEntity GetGroupScore(string evaluateId, string groupid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var dept = db.FindEntity<DepartmentEntity>(groupid);

            var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                 join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                                 where q2.ParentCategoryId == null
                                 select q1;

            var categories = from q1 in categorygroups
                             select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                          join q2 in categories on q1.ParentCategoryId equals q2.CategoryId
                          select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };
            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            }
            categories = categories.Concat(current);

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                        join q4 in categories on q3.CategoryId equals q4.CategoryId
                        //join q5 in categorygroups on q4.ParentCategoryId equals q5.CategoryId into into1
                        //from q6 in into1.DefaultIfEmpty()
                        where q2.EvaluateId == evaluateId && q2.GroupId == groupid
                        //orderby q6.CreateTime
                        //group new { q6.Category, q1.ActualScore, q1.Score } by q6.Category into g
                        //select new EvaluateItemCalcEntity() { Category = g.Key, ActualScore = g.Select(x => x.ActualScore).DefaultIfEmpty(0).Sum(), Score = g.Select(x => x.Score).DefaultIfEmpty(0).Sum() };
                        select new { q4.ParentCategoryId, q1.ActualScore, q1.Score };

            var d1 = categorygroups.OrderBy(x => x.CreateTime).ToList();
            var d2 = query.ToList();

            var data = d1.GroupJoin(d2, x => x.CategoryId, y => y.ParentCategoryId, (x, y) => new { x.Category, ActualScore = y.Select(z => z.ActualScore).DefaultIfEmpty(0).Sum(), Score = y.Select(z => z.Score).DefaultIfEmpty(0).Sum() });

            //var query2 = from q1 in categorygroups
            //             join q2 in query on q1.CategoryId equals q2.ParentCategoryId into into1
            //             from q3 in into1.DefaultIfEmpty()
            //             //orderby q1.CreateTime
            //             select new { q1.Category, ActualScore = q3.ActualScore ?? 0, Score = q3.Score ?? 0 };
            //group new { q1.Category, q3.ActualScore, q3.Score } by q1.Category into g
            //select new { Category = g.Key, ActualScore = g.Select(x => x.ActualScore).DefaultIfEmpty(0).Sum(), Score = g.Select(x => x.Score).DefaultIfEmpty(0).Sum() };
            //var sss = query2.ToList();
            //select new { q4.ParentCategoryId, q1.ActualScore, q1.Score };

            //var query2 = from q1 in categorygroups
            //             join q2 in query on q1.ParentCategoryId equals q2.ParentCategoryId into into1
            //             from q3 in into1.DefaultIfEmpty()
            //             orderby q1.CreateTime
            //             group new { q1, q3 } by q1.Category into g
            //             select new EvaluateItemCalcEntity() { Category = g.Key, ActualScore = g.Select(x => x.q3.ActualScore).DefaultIfEmpty(0).Sum(), Score = g.Select(x => x.q3.Score).DefaultIfEmpty(0).Sum() };

            //select new EvaluateItemCalcEntity() { Category = g.Key, ActualScore = g.Sum(x => x.ActualScore), Score = g.Sum(x => x.Score), Pct = 0 };



            //var q1 = db.IQueryable<EvaluateCategoryEntity>().Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.ParentCategoryId, y => y.CategoryId, (x, y) => new { x, y }).Where(x => x.y.ParentCategoryId == null).Select(x => x.x);

            //var query = db.IQueryable<EvaluateItemEntity>().Join(db.IQueryable<EvaluateGroupEntity>(), x => x.EvaluateGroupId, y => y.EvaluateGroupId, (x, y) => new { y.EvaluateId, y.GroupId, y.GroupName, x.EvaluateContentId, x.ActualScore, x.Score }).Where(x => x.EvaluateId == evaluateId).Join(db.IQueryable<EvaluateCategoryItemEntity>(), x => x.EvaluateContentId, y => y.ItemId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId }).Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId });

            //while (query.Join(q1, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId }).Count() == 0 && db.IQueryable<DepartmentEntity>().Count(x => x.DepartmentId == groupid) > 0)
            //{
            //    query = query.Join(db.IQueryable<EvaluateCategoryEntity>(), x => x.ParentCategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId });
            //}

            //var q2 = q1.GroupJoin(query) query.Where(x => x.GroupId == groupid).Join(q1, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.GroupId, x.GroupName, x.EvaluateContentId, x.ActualScore, x.Score, y.CategoryId, y.ParentCategoryId, y.Category, y.CreateTime }).GroupBy(x => new { x.Category, x.CreateTime }, (x, g) => new { x = x, ActualScore = g.Sum(y => y.ActualScore), Score = g.Sum(y => y.Score) }).OrderBy(x => x.x.CreateTime).Select(x => new EvaluateItemCalcEntity() { Category = x.x.Category, ActualScore = x.ActualScore, Score = x.Score, Pct = (decimal)x.ActualScore / (x.Score == 0 ? x.ActualScore == 0 ? 1 : x.ActualScore : x.Score) * 100 });

            return new EvaluateCalcEntity { Season = dept.FullName, Data = data.Select(x => new EvaluateItemCalcEntity() { Category = x.Category, ActualScore = x.ActualScore, Score = x.Score, Pct = (decimal)x.ActualScore / (x.Score == 0 ? x.ActualScore == 0 ? 1 : x.ActualScore : x.Score) * 100 }).ToList() };
        }


        /// <summary>
        /// 考评扣分明细
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CategoryId"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public IList<EvaluateScoreItemEntity> GetCalcScoreItme(string id, string CategoryId, string groupid)
        {
            var db = new RepositoryFactory().BaseRepository();

            //var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
            //                     join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
            //                     where q2.ParentCategoryId == null 
            //                     select q1;
            //if (!string.IsNullOrEmpty(CategoryId)) categorygroups = categorygroups.Where(x => x.CategoryId == CategoryId);

            var categories = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                             join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                             where q2.ParentCategoryId == null
                             select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            if (!string.IsNullOrEmpty(CategoryId)) categories = categories.Where(x => x.CategoryId == CategoryId);

            var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                          join q2 in categories on q1.ParentCategoryId equals q2.CategoryId
                          select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            }

            var seasons = db.IQueryable<EvaluateEntity>().Where(x => x.EvaluateId == id).ToList();
            //var groupdata = categorygroups.OrderBy(x => x.CreateTime).ToList();
            var data = new List<EvaluateScoreItemEntity>();
            foreach (var item in seasons)
            {

                var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                            join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                            join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                            join q4 in categories on q3.CategoryId equals q4.CategoryId
                            where q2.EvaluateId == item.EvaluateId && q2.GroupId == groupid
                            select new EvaluateScoreItemEntity
                            {
                                Reason = q1.Reason,
                                ItemContent = q3.ItemContent,
                                ActualScore = q1.ActualScore,
                                Score = q1.Score,
                                CategoryItemId = q3.ItemId,
                                WeightScore = q1.WeightScore,
                                CategoryId = q3.CategoryId
                            };
                var metadata = query.ToList();
                data.AddRange(metadata);
            }
            //计算权重
            var weidthList = db.IQueryable<WeightSetEntity>().Where(p => p.IsFiring == 1).ToList();
            var categoryList = db.IQueryable<EvaluateCategoryEntity>();
            data.ForEach(item =>
            {
                item.ActualScore = item.WeightScore == null ? item.ActualScore : item.WeightScore.Value;
                //根据categoryId判断权重
                WeightSetEntity weight = null;
                var rootId = string.Empty;
                if (weidthList.Any(x => x.Id == item.CategoryId))
                {
                    weight = weidthList.Where(x => x.Id == item.CategoryId).FirstOrDefault();
                }
                else
                {
                    var categoryObj = categoryList.Where(x => x.CategoryId == item.CategoryId).FirstOrDefault();
                    if (categoryObj != null)
                    {
                        rootId = categoryObj.ParentCategoryId;
                        if (weidthList.Any(x => x.Id == categoryObj.ParentCategoryId))
                        {
                            weight = weidthList.Where(x => x.Id == categoryObj.ParentCategoryId).FirstOrDefault();
                        }
                        else
                        {
                            var sendsObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
                            if (sendsObj != null)
                            {
                                rootId = sendsObj.ParentCategoryId;
                                if (weidthList.Any(x => x.Id == sendsObj.ParentCategoryId))
                                {
                                    weight = weidthList.Where(x => x.Id == sendsObj.ParentCategoryId).FirstOrDefault();
                                }
                                else
                                {
                                    var rootObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                    if (rootObj != null)
                                    {
                                        rootId = rootObj.ParentCategoryId;
                                        if (rootObj != null && weidthList.Any(x => x.Id == rootObj.ParentCategoryId))
                                        {
                                            weight = weidthList.Where(x => x.Id == rootObj.ParentCategoryId).FirstOrDefault();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (weight != null && weight.Weight != null)
                {
                    decimal Score = item.Score.ToDecimal();
                    item.Score = Math.Round((Score * weight.Weight.Value), 2);
                }
                else
                {
                    //item.Score = Math.Round(Convert.ToDecimal(item.Score * weight.Weight), 2);
                }
            });
            return data;
        }

        public IList<EvaluateCategoryEntity> GetCategories()
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                        join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                        where q2.ParentCategoryId == null
                        orderby q1.CreateTime
                        select q1;
            return query.ToList();
        }

        public EvaluateCategoryEntity GetCategory(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<EvaluateCategoryEntity>(id);
        }

        public EvaluateCategoryItemEntity GetCategoryItem(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<EvaluateCategoryItemEntity>(id);
            entity.Category = db.FindEntity<EvaluateCategoryEntity>(entity.CategoryId);
            return entity;
        }

        public IList<EvaluateCategoryItemEntity> GetCategoryItems(string key, string categoryid, int pagesize, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(categoryid)) categoryid = null;

            var node = from q in db.IQueryable<EvaluateCategoryEntity>()
                       where q.ParentCategoryId == null
                       select q;

            if (categoryid != null)
            {
                node = from q in db.IQueryable<EvaluateCategoryEntity>()
                       where q.CategoryId == categoryid
                       select q;
            }

            var categories = node;

            var current = from q1 in node
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select q2;

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select q2;

            }

            var query = from q1 in db.IQueryable<EvaluateCategoryItemEntity>()
                        join q2 in categories on q1.CategoryId equals q2.CategoryId
                        select new { q1.ItemId, q1.ItemContent, q1.ItemStandard, q1.Score, q1.CreateTime, q1.EvaluateDept, q1.UseDept, q1.CategoryId, Category = q2 };

            if (!string.IsNullOrEmpty(key))
            {
                query = from q1 in db.IQueryable<EvaluateCategoryItemEntity>()
                        join q2 in categories on q1.CategoryId equals q2.CategoryId
                        where q1.ItemContent.Contains(key)
                        select new { q1.ItemId, q1.ItemContent, q1.ItemStandard, q1.Score, q1.CreateTime, q1.EvaluateDept, q1.UseDept, q1.CategoryId, Category = q2 };
            }
            total = query.Count();
            query = query.OrderBy(x => x.Category.Category).ThenBy(x => x.CreateTime).Skip(pagesize * (page - 1)).Take(pagesize);
            return query.ToList().Select(x => new EvaluateCategoryItemEntity() { ItemId = x.ItemId, ItemContent = x.ItemContent, ItemStandard = x.ItemStandard, CategoryId = x.CategoryId, CreateTime = x.CreateTime, EvaluateDept = x.EvaluateDept, UseDept = x.UseDept, Score = x.Score, Category = new EvaluateCategoryEntity() { CategoryId = x.Category.CategoryId, Category = x.Category.Category, CreateTime = x.Category.CreateTime, ParentCategoryId = x.Category.ParentCategoryId } }).ToList();
        }

        public EvaluateEntity GetEvaluate(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<EvaluateEntity>(id);
        }

        public EvaluateEntity GetEvaluateionDetail(string id, string deptname, string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = (from q in db.IQueryable<EvaluateEntity>()
                          where q.EvaluateId == id
                          select q).FirstOrDefault();

            //var query1 = from q1 in db.IQueryable<EvaluateItemEntity>()
            //             join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
            //             where q2.EvaluateId == id
            //             select new { q1.EvaluateItemId, q2.EvaluateGroupId, q1.EvaluateContentId };
            //var query2 = from q1 in query1
            //             join q2 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q2.ItemId
            //             select new { q1.EvaluateItemId, q1.EvaluateGroupId, q1.EvaluateContentId, q2.ItemContent };


            var query1 = from q1 in db.IQueryable<EvaluateGroupEntity>()
                         join q2 in db.IQueryable<EvaluateItemEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                         join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q2.EvaluateContentId equals q3.ItemId
                         where q1.EvaluateId == id
                         select new { q1.EvaluateGroupId, q1.DeptId, q1.DeptName, q1.GroupId, q1.GroupName, q2.EvaluateItemId, q2.EvaluateContentId, q2.EvaluateContent, q3.EvaluateDept, q2.ActualScore, q2.CreateTime };
            //需求变动，有班组的部门只打本班组的数据 ，厂级部门打所有部门的特殊数据
            var allDepts = db.IQueryable<DepartmentEntity>();
            var userDept = allDepts.FirstOrDefault(p => p.DepartmentId == deptId);
            if (userDept == null)
            {
                query1.Where(p => p.EvaluateGroupId == null);//用户为空 ，不返回数据
            }
            else
            {
                if (!db.IQueryable<EvaluateCategoryItemEntity>(p => p.EvaluateDept == userDept.FullName).Any())//如果不是考评适用部门，则只能打本部门底下的班组的分
                {
                    var childDeptIds = allDepts.Where(p => p.ParentId == deptId).Select(x => x.DepartmentId).ToList();
                    query1 = from q in query1
                             where childDeptIds.Contains(q.GroupId)
                             select q;
                }
            }
            //if (!string.IsNullOrEmpty(deptname))
            //{
            //    query1 = from q in query1
            //             where q.EvaluateDept == deptname
            //             select q;
            //}

            var query = from q1 in query1
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.DeptId equals q2.DepartmentId into into1
                        from d1 in into1.DefaultIfEmpty()
                        join q3 in db.IQueryable<DepartmentEntity>() on q1.GroupId equals q3.DepartmentId into into2
                        from d2 in into2.DefaultIfEmpty()
                        group new { q1.EvaluateGroupId, q1.DeptId, q1.DeptName, q1.GroupId, q1.GroupName, Seq1 = d1.EnCode, Seq2 = d2.EnCode, q1.EvaluateItemId, q1.EvaluateContentId, q1.EvaluateContent, q1.ActualScore, q1.CreateTime } by new { q1.EvaluateGroupId, q1.DeptId, q1.DeptName, q1.GroupId, q1.GroupName, d1.EnCode, EnCode2 = d2.EnCode } into g
                        select new { g.Key.EvaluateGroupId, g.Key.DeptId, g.Key.DeptName, g.Key.GroupId, g.Key.GroupName, g.Key.EnCode, g.Key.EnCode2, Items = g.OrderBy(y => y.CreateTime) };
            var groups = query.OrderBy(x => x.EnCode).ThenBy(x => x.EnCode2).ToList();

            entity.Groups = groups.Select(x => new EvaluateGroupEntity() { EvaluateGroupId = x.EvaluateGroupId, EvaluateId = entity.EvaluateId, DeptId = x.DeptId, DeptName = x.DeptName, GroupId = x.GroupId, GroupName = x.GroupName, Items = x.Items.Select(y => new EvaluateItemEntity() { EvaluateItemId = y.EvaluateItemId, EvaluateGroupId = y.EvaluateGroupId, EvaluateContentId = y.EvaluateContentId, EvaluateContent = y.EvaluateContent, ActualScore = y.ActualScore, CreateTime = y.CreateTime }).ToList() }).ToList();

            var itemIds = entity.Groups.SelectMany(p => p.Items).Select(x => x.EvaluateItemId).ToList();
            var marksQuery = db.IQueryable<EvaluateMarksRecordsEntity>(p => itemIds.Contains(p.EvaluateItemId)).ToList();
            entity.Groups.SelectMany(p => p.Items).ToList().ForEach(item =>
            {
                item.MarksRecord = marksQuery.Where(p => p.EvaluateItemId == item.EvaluateItemId).ToList();
            });

            return entity;
        }

        public EvaluateItemEntity GetEvaluateItem(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<EvaluateItemEntity>(id);
        }

        public IList<EvaluateItemEntity> GetItemsByItemId(string itemid)
        {
            var query = new Repository<EvaluateItemEntity>(DbFactory.Base()).IQueryable();
            query = query.Where(x => x.EvaluateContentId == itemid);
            return query.ToList();
        }
        /// <summary>
        /// web专用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userid"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<EvaluateEntity> GetEvaluationsFoWeb(string name, string userid, int pagesize, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<EvaluateEntity>()
                        select q;


            if (!string.IsNullOrEmpty(name))
            {
                query = from q in query
                        where q.EvaluateSeason.Contains(name)
                        select q;
            }

            total = query.Count();
            var data = query.OrderByDescending(x => x.EvaluateSeason).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            var deptList = db.IQueryable<DepartmentEntity>();
            if (!string.IsNullOrEmpty(userid))
            {
                var user = db.FindEntity<UserEntity>(userid);
                var dept = deptList.FirstOrDefault(p => p.DepartmentId == user.DepartmentId);

                foreach (var item in data)
                {
                    item.IsEvaluated = true;//现在默认新增之后就可发布
                    item.IsCalculated = true;
                    if (item.IsPublished == false)
                    {
                        if (item.EvaluateUserId == userid || user.DepartmentId == "System")
                        {
                            item.CanEdit = "1";
                            item.CanDel = "1";
                            item.CanPublish = "1";
                        }
                        //if (item.EvaluateUserId == userid)
                        //{
                        //    var query1 = from q in db.IQueryable<EvaluateDeptEntity>()
                        //                 where q.EvaluateId == item.EvaluateId && q.IsSubmitted == false
                        //                 select q;

                        //    if (query1.Count() == 0 && !item.IsCalculated) item.CanCalc = "1";
                        //    else
                        //        item.CanCalc = "0";
                        //}

                        if (dept != null)
                        {
                            var childDeptCount = deptList.Count(p => p.ParentId == dept.DepartmentId && item.DeptScope.Contains(p.DepartmentId));
                            if (dept.IsOrg == true || childDeptCount > 0)   //当前用户所在的部门为厂级部门，或者该部门下存在被考评的班组，则可以打分
                            {
                                item.CanScore = "1";
                            }
                            if (db.IQueryable<EvaluateCategoryItemEntity>(p => p.EvaluateDept == dept.FullName).Any())
                                item.CanScore = "1";
                            //var query1 = from q in db.IQueryable<EvaluateDeptEntity>()
                            //             where q.EvaluateId == item.EvaluateId && q.DeptId == dept.DepartmentId && q.IsSubmitted == false
                            //             select q;

                            //if (query1.Count() > 0)
                            //{
                            //    item.CanScore = "1";
                            //}
                            //else
                            //{
                            //    item.IsEvaluated = true;
                            //}
                        }
                    }
                }
            }

            return data;
        }


        /// <summary>
        /// 查找扣分了的与发布了的考评
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userid"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<EvaluateEntity> GetEvaluations(string name, string userid, int pagesize, int page, out int total)
        {
            #region 
            //var db = new RepositoryFactory().BaseRepository();

            //var sql = @" SELECT t1.* FROM wg_evaluate t1 LEFT JOIN (SELECT c.evaluategroupid ,b.evaluateid GROUPID,SUM(c.actualscore)  ,SUM(c.Score),SUM(c.actualscore) <SUM(c.Score) isKF FROM  wg_evaluateitem c 
            //		LEFT JOIN 	wg_evaluategroup  b on  b.evaluategroupid=c.evaluategroupid
            //		GROUP BY b.evaluateid
            //                                            HAVING SUM(c.actualscore) <SUM(c.Score)) t2 on t1.evaluateid=t2.GROUPID WHERE t2.isKF=1 or t1.IsPublished=1";

            //var dt = db.FindTable(sql);

            //List<EvaluateEntity> query2 = new List<EvaluateEntity>();
            //var enumetra = dt.Rows.GetEnumerator();
            //while (enumetra.MoveNext())
            //{
            //    var a = enumetra.Current as DataRow;
            //    query2.Add(new EvaluateEntity()
            //    {
            //        EvaluateId = a["EvaluateId"].ToString(),
            //        EvaluateSeason = a["EvaluateSeason"].ToString(),
            //        EvaluateCycle = a["EvaluateCycle"].ToString(),
            //        EvaluateStatus = a["EvaluateStatus"].ToString(),
            //        EvaluateUser = a["EvaluateUser"].ToString(),
            //        EvaluateUserId = a["EvaluateUserId"].ToString(),
            //        CreateTime = Convert.ToDateTime(a["CreateTime"]),
            //        IsEvaluated = Convert.ToBoolean(a["IsEvaluated"]),
            //        IsCalculated = Convert.ToBoolean(a["IsCalculated"]),
            //        IsPublished = Convert.ToBoolean(a["IsPublished"]),
            //        LimitTime = a["LimitTime"] == null ? DateTime.MinValue : Convert.ToDateTime(a["LimitTime"]),
            //        PublishDate = a["PublishDate"] == null ? DateTime.MinValue : Convert.ToDateTime(a["PublishDate"])
            //    });
            //}

            //IEnumerable<EvaluateEntity> query = query2;




            //if (!string.IsNullOrEmpty(name))
            //{
            //    query = from q in query
            //            where q.EvaluateSeason.Contains(name)
            //            select q;
            //}

            //total = query.Count();
            //var data = query.OrderByDescending(x => x.EvaluateSeason).Skip(pagesize * (page - 1)).Take(pagesize).ToList();

            //if (!string.IsNullOrEmpty(userid))
            //{
            //    var user = db.FindEntity<UserEntity>(userid);
            //    var dept = db.FindEntity<DepartmentEntity>(user.DepartmentId);

            //    foreach (var item in data)
            //    {
            //        if (item.EvaluateUserId == userid && item.IsPublished == false) item.CanEdit = "1";
            //        if (item.EvaluateUserId == userid)
            //        {
            //            var query1 = from q in db.IQueryable<EvaluateDeptEntity>()
            //                         where q.EvaluateId == item.EvaluateId && q.IsSubmitted == false
            //                         select q;

            //            if (query1.Count() == 0 && !item.IsCalculated) item.CanCalc = "1";
            //            else
            //                item.CanCalc = "0";
            //        }

            //        if (dept != null)
            //        {
            //            var query1 = from q in db.IQueryable<EvaluateDeptEntity>()
            //                         where q.EvaluateId == item.EvaluateId && q.DeptId == dept.DepartmentId && q.IsSubmitted == false
            //                         select q;

            //            if (query1.Count() > 0)
            //            {
            //                item.CanScore = "1";
            //            }
            //            else
            //            {
            //                item.IsEvaluated = true;
            //            }
            //        }
            //    }
            //}

            //return data;
            #endregion
            var db = new RepositoryFactory().BaseRepository();
            IEnumerable<EvaluateEntity> query = db.IQueryable<EvaluateEntity>();
            if (!string.IsNullOrEmpty(name))
            {
                query = from q in query
                        where q.EvaluateSeason.Contains(name)
                        select q;
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.EvaluateSeason).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            if (!string.IsNullOrEmpty(userid))
            {
                var user = db.FindEntity<UserEntity>(userid);
                var dept = db.FindEntity<DepartmentEntity>(user.DepartmentId);

                foreach (var item in data)
                {
                    if (item.EvaluateUserId == userid && item.IsPublished == false) item.CanEdit = "1";
                    if (item.EvaluateUserId == userid)
                    {
                        var query1 = from q in db.IQueryable<EvaluateDeptEntity>()
                                     where q.EvaluateId == item.EvaluateId && q.IsSubmitted == false
                                     select q;

                        if (query1.Count() == 0 && !item.IsCalculated) item.CanCalc = "1";
                        else
                            item.CanCalc = "0";
                    }

                    if (dept != null)
                    {
                        var query1 = from q in db.IQueryable<EvaluateDeptEntity>()
                                     where q.EvaluateId == item.EvaluateId && q.DeptId == dept.DepartmentId && q.IsSubmitted == false
                                     select q;

                        if (query1.Count() > 0)
                        {
                            item.CanScore = "1";
                        }
                        else
                        {
                            item.IsEvaluated = true;
                        }
                    }
                }
            }

            return data;
        }

        public void AddEvaluate(EvaluateEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var query = from q in db.IQueryable<EvaluateEntity>()
                            where q.EvaluateSeason == model.EvaluateSeason
                            select q;

                if (query.Count() > 0) throw new Exception("考评已存在！");

                var groups = (from q1 in db.IQueryable<DepartmentEntity>()
                              join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                              where q1.Nature == "班组" && model.DeptScope.Contains(q1.DepartmentId)//查找在考评范围内的班组
                              orderby q2.EnCode
                              select new { GroupId = q1.DepartmentId, GroupName = q1.FullName, DeptId = q2.DepartmentId, DeptName = q2.FullName }).ToList();

                var evaluategroups = groups.Select(x => new EvaluateGroupEntity() { EvaluateGroupId = Guid.NewGuid().ToString(), EvaluateId = model.EvaluateId, DeptId = x.DeptId, DeptName = x.DeptName, GroupId = x.GroupId, GroupName = x.GroupName, CreateTime = DateTime.Now }).ToList();
                var categories = db.IQueryable<EvaluateCategoryItemEntity>().ToList();
                var evaluateitems = new List<EvaluateItemEntity>();
                var evaluatedepts = new List<EvaluateDeptEntity>();//考评的班组
                var weidthList = db.IQueryable<WeightSetEntity>().Where(p => p.IsFiring == 1).ToList();
                evaluategroups.ForEach(x =>
                {
                    evaluateitems.AddRange(categories.Where(y => string.IsNullOrEmpty(y.UseDept) || y.UseDept.Contains(x.GroupName)).Select(y =>
                new EvaluateItemEntity()
                {
                    CategoryId = y.CategoryId,
                    EvaluateItemId = Guid.NewGuid().ToString(),
                    EvaluateGroupId = x.EvaluateGroupId,
                    EvaluateContentId = y.ItemId,
                    EvaluateContent = y.ItemContent,
                    CreateTime = DateTime.Now,
                    ActualScore = y.Score,
                    Score = y.Score,
                }));
                });

                var deptquery = from q1 in db.IQueryable<DepartmentEntity>()
                                join q2 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.FullName equals q2.EvaluateDept
                                select new { q1.DepartmentId, q1.FullName };

                foreach (var item in deptquery.Distinct())
                {
                    evaluatedepts.Add(new EvaluateDeptEntity()
                    {
                        EvaluateDeptId = Guid.NewGuid().ToString(),
                        EvaluateId = model.EvaluateId,
                        DeptId = item.DepartmentId,
                        DeptName = item.FullName,
                        IsSubmitted = false
                    });
                }


                var evaluatecategories = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                         select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.ParentCategoryId };
                var sss = evaluatecategories.ToList();
                foreach (var item in evaluateitems)
                {
                    //根据categoryId判断权重
                    WeightSetEntity weight = null;
                    var rootId = string.Empty;
                    if (weidthList.Any(x => x.Id == item.CategoryId))
                    {
                        weight = weidthList.Where(x => x.Id == item.CategoryId).FirstOrDefault();
                    }
                    else
                    {
                        var categoryObj = sss.Where(x => x.CategoryId == item.CategoryId).FirstOrDefault();
                        if (categoryObj != null)
                        {
                            rootId = categoryObj.ParentCategoryId;
                            if (weidthList.Any(x => x.Id == categoryObj.ParentCategoryId))
                            {
                                weight = weidthList.Where(x => x.Id == rootId).FirstOrDefault();
                            }
                            else
                            {
                                var sendsObj = sss.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                if (sendsObj != null)
                                {
                                    rootId = sendsObj.ParentCategoryId;
                                    if (weidthList.Any(x => x.Id == sendsObj.ParentCategoryId))
                                    {
                                        weight = weidthList.Where(x => x.Id == rootId).FirstOrDefault();
                                    }
                                    else
                                    {
                                        var rootObj = sss.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                        if (rootObj != null)
                                        {
                                            rootId = rootObj.ParentCategoryId;
                                            if (weidthList.Any(x => x.Id == rootObj.ParentCategoryId))
                                            {
                                                weight = weidthList.Where(x => x.Id == rootId).FirstOrDefault();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (weight != null && weight.Weight != null)
                    {
                        item.WeightScore = Math.Round(Convert.ToDecimal(item.ActualScore * weight.Weight), 2);
                        item.WeightId = weight.Id;
                    }
                    else
                    {
                        item.WeightScore = item.ActualScore;
                    }
                }
                //现需求 ，新增完之后即可发布
                model.IsCalculated = true;
                model.IsEvaluated = true;
                db.Insert(model);
                db.Insert(evaluategroups);
                db.Insert(evaluateitems);
                db.Insert(evaluatedepts);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void EditEvaluate(EvaluateEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = (from q in db.IQueryable<EvaluateEntity>()
                              where q.EvaluateId == model.EvaluateId
                              select q).FirstOrDefault();

                entity.EvaluateSeason = model.EvaluateSeason;
                entity.EvaluateUserId = model.EvaluateUserId;
                entity.EvaluateUser = model.EvaluateUser;
                entity.PublishDate = model.PublishDate;
                entity.LimitTime = model.LimitTime;
                entity.EvaluateCycle = model.EvaluateCycle;
                entity.DeptScope = model.DeptScope;
                entity.DeptScopeName = model.DeptScopeName;
                db.Update(entity);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void Submit(string evaluateid, string deptid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var evaluate = (from q in db.IQueryable<EvaluateEntity>()
                                where q.EvaluateId == evaluateid
                                select q).FirstOrDefault();

                var query = from q in db.IQueryable<EvaluateGroupEntity>()
                            where q.EvaluateId == evaluateid && q.DeptId == deptid
                            select q;

                var data = query.ToList();
                data.ForEach(x =>
                {
                    x.IsSubmitted = true;
                });

                db.Update(data);

                var countquery = from q in db.IQueryable<EvaluateGroupEntity>()
                                 where q.EvaluateId == evaluateid && q.IsSubmitted == false
                                 select q;

                //原 if (countquery.Count() == 0) evaluate.EvaluateStatus = "考评汇总";
                //改
                if (countquery.Count() > 0)
                {
                    var list = countquery.ToList();
                    list.ForEach(p =>
                    {
                        p.IsSubmitted = true;
                        db.Update(p);
                    });
                }

                evaluate.EvaluateStatus = "考评汇总";
                evaluate.IsCalculated = true;
                evaluate.IsEvaluated = true;
                var alldepts = (from q in db.IQueryable<EvaluateDeptEntity>()
                                where q.EvaluateId == evaluateid
                                select q).ToList();

                var evaluatedepts = alldepts.Where(x => x.DeptId == deptid).ToList();
                evaluatedepts.ForEach(x => x.IsSubmitted = true);

                db.Update(evaluatedepts);

                if (alldepts.Count(x => x.IsSubmitted == false) == 0) evaluate.IsEvaluated = true;

                db.Update(evaluate);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void Submit(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var evaluate = (from q in db.IQueryable<EvaluateEntity>()
                                where q.EvaluateId == id
                                select q).FirstOrDefault();

                evaluate.IsCalculated = true;
                //evaluate.IsPublished = true;

                db.Update(evaluate);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public void Publish(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var evaluate = (from q in db.IQueryable<EvaluateEntity>()
                                where q.EvaluateId == id
                                select q).FirstOrDefault();


                evaluate.IsPublished = true;

                db.Update(evaluate);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public object GetData1(string evaluateid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q3.DepartmentId
                        where q2.EvaluateId == evaluateid
                        select new { q1.EvaluateContent, q1.ActualScore, q1.Score, q1.Reason, q3.FullName };

            var data = query.OrderByDescending(x => x.ActualScore - x.Score).Take(5).ToList();
            return data;
        }

        public object GetData2(string evaluateid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q3.DepartmentId
                        where q2.EvaluateId == evaluateid
                        select new { q1.EvaluateContent, q1.ActualScore, q1.Score, q1.Reason, q3.FullName };

            var data = query.OrderBy(x => x.ActualScore - x.Score).Take(5).ToList();
            return data;
        }

        public List<EvaluateItemEntity> GetMainData(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var evaluate = (from q in db.IQueryable<EvaluateEntity>()
                            where q.IsPublished == true
                            orderby q.CreateTime descending
                            select q).FirstOrDefault();
            if (evaluate == null) return new List<EvaluateItemEntity>();

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        where q2.EvaluateId == evaluate.EvaluateId
                        select q1;

            var data = query.OrderBy(x => x.ActualScore - x.Score).Take(5).ToList();
            return data;
        }

        public List<EvaluateGroupEntity> GetEvaluationResult()
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<EvaluateEntity>()
                        orderby q.CreateTime descending
                        where q.IsPublished == true
                        select q;

            var evaluate = query.FirstOrDefault();

            if (evaluate == null) return null;
            var sss = this.GetCalcScore(evaluate.EvaluateId, null).ToList();
            return sss;
        }

        public EvaluateEntity GetLastEvaluate()
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<EvaluateEntity>()
                        orderby q.CreateTime descending
                        select q;
            return query.FirstOrDefault();
        }

        public List<EvaluateItemEntity> GetEvaluateContent(string[] evaluateids, string[] deptids)
        {
            var db = new RepositoryFactory().BaseRepository();

            var subquery = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                           join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                           where q2.ParentCategoryId == null
                           select new { ParentCategoryId = q1.CategoryId, q1.Category, q1.CreateTime, q1.CategoryId };

            var current = from q1 in subquery
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { ParentCategoryId = q1.ParentCategoryId, q1.Category, q1.CreateTime, q2.CategoryId };

            while (current.Count() > 0)
            {
                subquery = subquery.Concat(current);
                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { ParentCategoryId = q1.ParentCategoryId, q1.Category, q1.CreateTime, q2.CategoryId };
            }

            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                        join q4 in subquery on q3.CategoryId equals q4.CategoryId
                        where evaluateids.Contains(q2.EvaluateId) && deptids.Contains(q2.GroupId)
                        select new { q2.EvaluateId, q1.EvaluateItemId, q1.ActualScore, q1.Score, q4.ParentCategoryId, q2.GroupId, q4.CreateTime };
            var data = query.ToList().Select(x => new EvaluateItemEntity() { EvaluateItemId = x.EvaluateId, ActualScore = x.ActualScore, Score = x.Score, EvaluateContentId = x.ParentCategoryId, EvaluateGroupId = x.GroupId, CreateTime = x.CreateTime }).ToList();
            foreach (var item in evaluateids)
            {
                if (!data.Any(x => x.EvaluateItemId == item)) data.Add(new EvaluateItemEntity { EvaluateItemId = item });
            }
            return data;
        }

        public EvaluateEntity GetEvaluateBySeason(string season)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<EvaluateEntity>()
                        where q.EvaluateSeason == season
                        select q;
            return query.FirstOrDefault();
        }
        /// <summary>
        /// 判断考评打分没有打分的部门的数量
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int GetAllDeptCountById(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<EvaluateDeptEntity>().Count(x => x.EvaluateId == keyValue && x.IsSubmitted == true);

        }

        public void DelEvaluateById(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            db.Delete<EvaluateEntity>(keyValue);
        }
        /// <summary>
        /// 根据部门或者分组来查询考试排名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryid"></param>
        /// <param name="indexType"></param>
        /// <returns></returns>
        public object GetCalcScore(string id, string categoryid, int indexType)
        {
            if (string.IsNullOrEmpty(categoryid)) categoryid = null;

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                                 join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
                                 where q2.ParentCategoryId == null
                                 select q1;

            if (!string.IsNullOrEmpty(categoryid)) categorygroups = categorygroups.Where(x => x.CategoryId == categoryid);

            var categories = from q1 in categorygroups
                             select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
                          join q2 in categorygroups on q1.ParentCategoryId equals q2.CategoryId
                          select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
                          select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            }
            if (indexType == 0)
            {
                var query = from q1 in db.IQueryable<EvaluateItemEntity>()

                            join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                            join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                            join q4 in categories on q3.CategoryId equals q4.CategoryId
                            join q5 in db.IQueryable<DepartmentEntity>() on q2.DeptId equals q5.DepartmentId into into1
                            from t1 in into1.DefaultIfEmpty()
                            join q6 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q6.DepartmentId into into2
                            from t2 in into2.DefaultIfEmpty()
                            where q2.EvaluateId == id
                            select new { q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, q1.ActualScore, q1.Score, q2.DeptId, q2.DeptName };

                var metadata = query.ToList();
                var groups = metadata.GroupBy(x => new { x.GroupName, x.EnCode1, x.EnCode2, x.DeptId, x.DeptName }).Select(x => new { x.Key.GroupName, ActualScore = x.Select(y => y.ActualScore).DefaultIfEmpty(0).Sum(), Score = x.Select(y => y.Score).DefaultIfEmpty(0).Sum(), DeptId = x.Key.DeptId, x.Key.DeptName }).ToList();
                return groups.ToList().Select(x => new EvaluateGroupEntity() { DeptId = x.DeptId, DeptName = x.DeptName, GroupName = x.GroupName, ActualScore = x.ActualScore, Pct = (decimal)x.ActualScore / (x.Score == 0 ? x.ActualScore == 0 ? 1 : x.ActualScore : x.Score) * 100 }).OrderByDescending(x => x.Pct).ToList();
            }
            else
            {
                var EvaluateSetEntitys = db.IQueryable<EvaluateSetEntity>().ToList();

                var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                            join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                            join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                            join q4 in categories on q3.CategoryId equals q4.CategoryId
                            join q5 in db.IQueryable<DepartmentEntity>() on q2.DeptId equals q5.DepartmentId into into1
                            from t1 in into1.DefaultIfEmpty()
                            join q6 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q6.DepartmentId into into2
                            from t2 in into2.DefaultIfEmpty()
                            join q7 in db.IQueryable<EvaluateGroupTitleEntity>() on q2.GroupId equals q7.GroupId into into3
                            from t3 in into3.DefaultIfEmpty()
                            join q8 in db.IQueryable<DesignationEntity>() on t3.TitleId equals q8.Id into into4
                            from t4 in into4.DefaultIfEmpty()
                            where q2.EvaluateId == id
                            select new { q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, q1.ActualScore, q1.Score, q2.DeptId, q2.DeptName, q2.GroupId, t4.ClassName };

                var metadata = query.ToList();
                List<ScoreData> a = new List<ScoreData>();
                foreach (EvaluateSetEntity set in EvaluateSetEntitys)
                {
                    foreach (var data in metadata)
                    {
                        if (set.DeptId.Contains(data.GroupId))
                        {
                            a.Add(new ScoreData()
                            {
                                DeptId = data.DeptId,
                                DeptName = data.DeptName,
                                ActualScore = data.ActualScore,
                                EnCode1 = data.EnCode1,
                                EnCode2 = data.EnCode2,
                                GroupName = data.GroupName,
                                Score = data.Score,
                                TitleName = data.ClassName
                            });
                        }
                    }
                }

                var groups = a.GroupBy(x => new { x.GroupName, x.EnCode1, x.EnCode2, x.DeptId, x.DeptName, x.GroupById, x.GroupByName, x.TitleName }).Select(x => new { x.Key.GroupName, x.Key.GroupById, x.Key.GroupByName, ActualScore = x.Select(y => y.ActualScore).DefaultIfEmpty(0).Sum(), Score = x.Select(y => y.Score).DefaultIfEmpty(0).Sum(), DeptId = x.Key.DeptId, x.Key.DeptName, x.Key.TitleName }).ToList();


                return groups.ToList().Select(x => new EvaluateGroupEntity() { DeptId = x.DeptId, DeptName = x.DeptName, GroupName = x.GroupName, ActualScore = x.ActualScore, Pct = (decimal)x.ActualScore / (x.Score == 0 ? x.ActualScore == 0 ? 1 : x.ActualScore : x.Score) * 100, TitleName = x.TitleName }).OrderByDescending(x => x.Pct).ToList();
            }


        }

        public IList<Group> GetCalcScoreNew(string id, string categoryid)
        {
            if (string.IsNullOrEmpty(categoryid)) categoryid = null;

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = db.IQueryable<EvaluateCategoryEntity>();

            if (!string.IsNullOrEmpty(categoryid)) categorygroups = categorygroups.Where(x => x.CategoryId == categoryid);

            //var categories = from q1 in categorygroups
            //                 select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            //var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
            //              join q2 in categorygroups on q1.ParentCategoryId equals q2.CategoryId
            //              select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };

            //while (current.Count() > 0)
            //{
            //    categories = categories.Concat(current);

            //    current = from q1 in current
            //              join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
            //              select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            //}
            //var sss = categories.ToList();




            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId
                        join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                        join q4 in categorygroups on q3.CategoryId equals q4.CategoryId
                        join q5 in db.IQueryable<DepartmentEntity>() on q2.DeptId equals q5.DepartmentId into into1
                        from t1 in into1.DefaultIfEmpty()
                        join q6 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q6.DepartmentId into into2
                        from t2 in into2.DefaultIfEmpty()
                        join q7 in db.IQueryable<EvaluateGroupTitleEntity>() on new { a = q2.GroupId, b = q2.EvaluateId } equals new { a = q7.GroupId, b = q7.EvaluateId } into into3
                        from t3 in into3.DefaultIfEmpty()
                        join q8 in db.IQueryable<DesignationEntity>() on t3.TId equals q8.Id into into4
                        from t4 in into4.DefaultIfEmpty()
                        where q2.EvaluateId == id
                        select new ScoreData() { GroupName = q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, ActualScore = q1.ActualScore, Score = q1.Score, DeptId = q2.DeptId, DeptName = q2.DeptName, GroupId = q2.GroupId, ClassName = t4.ClassName, WeightScore = q1.WeightScore, CategoryId = q3.CategoryId, TitleId = t3.TitleId, Tid = t3.TId };

            var metadata = query.ToList();

            var weidthList = db.IQueryable<WeightSetEntity>().ToList();
            var categoryList = db.IQueryable<EvaluateCategoryEntity>();
            foreach (var item in metadata)
            {
                //根据categoryId判断权重
                WeightSetEntity weight = null;
                var rootId = string.Empty;
                if (weidthList.Any(x => x.Id == item.CategoryId))
                {
                    weight = weidthList.Where(x => x.Id == item.CategoryId).FirstOrDefault();
                }
                else
                {
                    var categoryObj = categoryList.Where(x => x.CategoryId == item.CategoryId).FirstOrDefault();
                    if (categoryObj != null)
                    {
                        rootId = categoryObj.ParentCategoryId;
                        if (weidthList.Any(x => x.Id == categoryObj.ParentCategoryId))
                        {
                            weight = weidthList.Where(x => x.Id == categoryObj.ParentCategoryId).FirstOrDefault();
                        }
                        else
                        {
                            var sendsObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
                            if (sendsObj != null)
                            {
                                rootId = sendsObj.ParentCategoryId;
                                if (weidthList.Any(x => x.Id == sendsObj.ParentCategoryId))
                                {
                                    weight = weidthList.Where(x => x.Id == sendsObj.ParentCategoryId).FirstOrDefault();
                                }
                                else
                                {
                                    var rootObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                    if (rootObj != null)
                                    {
                                        rootId = rootObj.ParentCategoryId;
                                        if (weidthList.Any(x => x.Id == rootObj.ParentCategoryId))
                                        {
                                            weight = weidthList.Where(x => x.Id == rootObj.ParentCategoryId).FirstOrDefault();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (weight != null && weight.Weight != null)
                {

                    item.Score = Math.Round(Convert.ToDecimal(item.Score * weight.Weight), 2);
                }
                else
                {
                    //item.Score = Math.Round(Convert.ToDecimal(item.Score * weight.Weight), 2);
                }
            }

            var groups = metadata.GroupBy(x => new { x.DeptId, x.GroupName, x.EnCode1, x.EnCode2, x.GroupId, x.ClassName, x.TitleId, x.Tid }).Select(x => new { x.Key.GroupName, ActualScore = x.Select(y => y.WeightScore == null ? y.ActualScore : y.WeightScore.Value).DefaultIfEmpty(0).Sum(), Score = x.Select(y => y.Score).DefaultIfEmpty(0).Sum(), DeptId = x.Key.DeptId, x.Key.GroupId, TitleName = x.Key.ClassName, x.Key.TitleId, x.Key.Tid }).ToList();

            var List = groups.ToList().Select(x => new Group() { GroupName = x.GroupName, Score1 = x.ActualScore, Score = x.Score, DeptId = x.DeptId, GroupBy = x.DeptId, GroupId = x.GroupId, TitleName = x.TitleName, TitleId = x.TitleId, Tid = x.Tid }).ToList();
            return List;


        }

        public IList<Group> GetCalcScoreNew(string id, string categoryid, int indexType)
        {
            if (string.IsNullOrEmpty(categoryid)) categoryid = null;

            var db = new RepositoryFactory().BaseRepository();


            var categorygroups = db.IQueryable<EvaluateCategoryEntity>();

            if (!string.IsNullOrEmpty(categoryid)) categorygroups = categorygroups.Where(x => x.CategoryId == categoryid);
            //var categorygroups = from q1 in db.IQueryable<EvaluateCategoryEntity>()
            //                     join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.ParentCategoryId equals q2.CategoryId
            //                     where q2.ParentCategoryId == null
            //                     select q1;

            //if (!string.IsNullOrEmpty(categoryid)) categorygroups = categorygroups.Where(x => x.CategoryId == categoryid);

            //var categories = from q1 in categorygroups
            //                 select new { CategoryId = q1.CategoryId, ParentCategoryId = q1.CategoryId };

            //var current = from q1 in db.IQueryable<EvaluateCategoryEntity>()
            //              join q2 in categorygroups on q1.ParentCategoryId equals q2.CategoryId
            //              select new { CategoryId = q1.CategoryId, ParentCategoryId = q2.CategoryId };

            //while (current.Count() > 0)
            //{
            //    categories = categories.Concat(current);

            //    current = from q1 in current
            //              join q2 in db.IQueryable<EvaluateCategoryEntity>() on q1.CategoryId equals q2.ParentCategoryId
            //              select new { CategoryId = q2.CategoryId, ParentCategoryId = q1.ParentCategoryId };
            //}
            //var sss = categories.ToList();
            var EvaluateSetEntitys = db.IQueryable<EvaluateSetEntity>().Where(x => x.IsFiring == 1).ToList();//查询启用的分组
            var query = from q1 in db.IQueryable<EvaluateItemEntity>()
                        join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId

                        join q3 in db.IQueryable<EvaluateCategoryItemEntity>() on q1.EvaluateContentId equals q3.ItemId
                        join q4 in categorygroups on q3.CategoryId equals q4.CategoryId
                        join q5 in db.IQueryable<DepartmentEntity>() on q2.DeptId equals q5.DepartmentId into into1
                        from t1 in into1.DefaultIfEmpty()
                        join q6 in db.IQueryable<DepartmentEntity>() on q2.GroupId equals q6.DepartmentId into into2
                        from t2 in into2.DefaultIfEmpty()
                        join q7 in db.IQueryable<EvaluateGroupTitleEntity>() on new { a = q2.GroupId, b = q2.EvaluateId } equals new { a = q7.GroupId, b = q7.EvaluateId } into into3
                        from t3 in into3.DefaultIfEmpty()
                        join q8 in db.IQueryable<DesignationEntity>() on t3.TId equals q8.Id into into4
                        from t4 in into4.DefaultIfEmpty()
                        where q2.EvaluateId == id
                        select new ScoreData() { GroupName = q2.GroupName, EnCode1 = t1.EnCode, EnCode2 = t2.EnCode, ActualScore = q1.ActualScore, Score = q1.Score, DeptId = q2.DeptId, DeptName = q2.DeptName, GroupId = q2.GroupId, ClassName = t4.ClassName, WeightScore = q1.WeightScore, CategoryId = q3.CategoryId, TitleId = t3.TitleId, Tid = t3.TId };

            var metadata = query.ToList();
            var weidthList = db.IQueryable<WeightSetEntity>().ToList();
            var categoryList = db.IQueryable<EvaluateCategoryEntity>();
            foreach (var item in metadata)
            {
                //根据categoryId判断权重
                WeightSetEntity weight = null;
                var rootId = string.Empty;
                if (weidthList.Any(x => x.Id == item.CategoryId))
                {
                    weight = weidthList.Where(x => x.Id == item.CategoryId).FirstOrDefault();
                }
                else
                {
                    var categoryObj = categoryList.Where(x => x.CategoryId == item.CategoryId).FirstOrDefault();
                    if (categoryObj != null)
                    {
                        rootId = categoryObj.ParentCategoryId;
                        if (weidthList.Any(x => x.Id == categoryObj.ParentCategoryId))
                        {
                            weight = weidthList.Where(x => x.Id == categoryObj.ParentCategoryId).FirstOrDefault();
                        }
                        else
                        {
                            var sendsObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
                            if (sendsObj != null)
                            {
                                rootId = sendsObj.ParentCategoryId;
                                if (weidthList.Any(x => x.Id == sendsObj.ParentCategoryId))
                                {
                                    weight = weidthList.Where(x => x.Id == sendsObj.ParentCategoryId).FirstOrDefault();
                                }
                                else
                                {
                                    var rootObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
                                    if (rootObj != null)
                                    {
                                        rootId = rootObj.ParentCategoryId;
                                        if (rootObj != null && weidthList.Any(x => x.Id == rootObj.ParentCategoryId))
                                        {
                                            weight = weidthList.Where(x => x.Id == rootObj.ParentCategoryId).FirstOrDefault();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (weight != null && weight.Weight != null)
                {
                    item.Score = Math.Round(Convert.ToDecimal(item.Score * weight.Weight), 2);
                }
                else
                {
                    //item.Score = Math.Round(Convert.ToDecimal(item.Score * weight.Weight), 2);
                }
            }
            foreach (EvaluateSetEntity set in EvaluateSetEntitys)
            {
                foreach (var data in metadata)
                {
                    if (set.DeptId.Contains(data.GroupId))
                    {
                        data.GroupById = set.Id;
                        //a.Add(new ScoreData()
                        //{
                        //    GroupId = data.GroupId,
                        //    DeptId = data.DeptId,
                        //    DeptName = data.DeptName,
                        //    ActualScore = data.ActualScore,
                        //    EnCode1 = data.EnCode1,
                        //    EnCode2 = data.EnCode2,
                        //    GroupName = data.GroupName,
                        //    Score = data.Score,
                        //    GroupById = set.Id,
                        //    GroupByName = set.ClassName,
                        //    TitleName = data.ClassName
                        //});
                    }
                }
            }
            var groups = metadata.GroupBy(x => new { x.GroupName, x.EnCode1, x.EnCode2, x.DeptId, x.DeptName, x.GroupId, x.ClassName, x.TitleId, x.Tid }).Select(x => new { x.Key.GroupName, x.Key.DeptId, x.Key.DeptName, ActualScore = x.Select(y => y.WeightScore).DefaultIfEmpty(0).Sum(), Score = x.Select(y => y.Score).DefaultIfEmpty(0).Sum(), x.Key.GroupId, x.Key.ClassName, x.Key.TitleId, x.Key.Tid }).ToList();

            var list = groups.ToList().Select(x => new Group() { GroupName = x.GroupName, Score1 = x.ActualScore.Value, Score = x.Score, GroupBy = x.DeptId, GroupId = x.GroupId, TitleName = x.ClassName, TitleId = x.TitleId, Tid = x.Tid }).ToList();
            return list;
        }
        /// <summary>
        /// 考评加减分记录
        /// </summary>
        /// <param name="evaluateItemId"></param>
        /// <returns></returns>
        public List<EvaluateMarksRecordsEntity> GetMarksRecords(string evaluateItemId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<EvaluateMarksRecordsEntity>(p => p.EvaluateItemId == evaluateItemId).OrderByDescending(x => x.CreateDate).ToList();
            return data;
        }
        /// <summary>
        /// 获取加减分详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EvaluateMarksRecordsEntity GetMarksRecordEntity(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.FindEntity<EvaluateMarksRecordsEntity>(id);
            return data;
        }
        /// <summary>
        /// 新增考评记录
        /// </summary>
        /// <param name="entity"></param>
        public void AddMarksRecord(EvaluateMarksRecordsEntity entity)
        {
            var tran = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                entity.Create();
                var dept = tran.FindEntity<DepartmentEntity>(entity.CreateUserDeptId);
                if (dept == null)
                    entity.IsOrg = 0;
                else
                    entity.IsOrg = dept.IsSpecial ? 1 : 0;
                tran.Insert(entity);
                //修改 实际得分
                var evaluateItem = tran.FindEntity<EvaluateItemEntity>(entity.EvaluateItemId);
                if (evaluateItem == null) throw new Exception("未找到打分项");
                evaluateItem.ActualScore += entity.Score;
                if (evaluateItem.ActualScore < 0) throw new Exception("最终评分不能小于0，请修改加/减分的分数");
                //计算权重分
                WeightSetEntity weightSet = tran.FindEntity<WeightSetEntity>(evaluateItem.WeightId);
                if (weightSet != null)
                {
                    evaluateItem.WeightScore = Math.Round(Convert.ToDecimal(evaluateItem.ActualScore * weightSet.Weight), 2);
                }
                tran.Update(evaluateItem);
                //如果当前数据时部门级的，并且数据是公司级人员修改的,添加一条修改记录
                if (entity.IsOrg == 1)
                {
                    //Operator user = OperatorProvider.Provider.Current();
                    EvaluateItemEntity evaluateItemEntity = null;
                    evaluateItemEntity = tran.FindEntity<EvaluateItemEntity>(entity.EvaluateItemId);
                    EvaluateCategoryItemEntity categoryItemEntity = null;
                    categoryItemEntity = tran.FindEntity<EvaluateCategoryItemEntity>(evaluateItemEntity.EvaluateContentId);
                    EvaluateCategoryEntity categoryEntity = null;
                    categoryEntity = tran.FindEntity<EvaluateCategoryEntity>(categoryItemEntity.CategoryId);
                    EvaluateGroupEntity evaluateGroup = null;
                    evaluateGroup = tran.FindEntity<EvaluateGroupEntity>(evaluateItemEntity.EvaluateGroupId);
                    EvaluateEntity evaluate = null;
                    evaluate = tran.FindEntity<EvaluateEntity>(evaluateGroup.EvaluateId);

                    EvaluateReviseEntity reviseEntity = new EvaluateReviseEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Category = categoryEntity?.Category,
                        CategoryId = categoryEntity?.CategoryId,
                        CreateDate = DateTime.Now,
                        CreateUser = entity.CreateUserName,
                        CreateUserId = entity.CreateUserId,
                        DepartmentId = evaluateGroup?.DeptId,
                        DepartmentName = evaluateGroup?.DeptName,
                        EvaluteContent = evaluateItemEntity?.EvaluateContent,
                        EvaluteContentId = evaluateItemEntity?.EvaluateContentId,
                        GroupId = evaluateGroup?.GroupId,
                        GroupName = evaluateGroup?.GroupName,
                        ReviseCause = entity.Cause,
                        ReviseScore = entity.Score,
                        ReviseUser = entity.CreateUserName,
                        ReviseUserId = entity.CreateUserId,
                        StandardScore = evaluateItemEntity.Score,
                        EvaluateId = evaluate.EvaluateId,
                        EvaluateSeason = evaluate.EvaluateSeason
                    };
                    tran.Insert(reviseEntity);
                }


                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 修改考评记录
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateMarksRecord(EvaluateMarksRecordsEntity entity)
        {
            var tran = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                //先找出旧的记录
                var oldRecord = tran.FindEntity<EvaluateMarksRecordsEntity>(entity.Id);
                //修改 实际得分
                var evaluateItem = tran.FindEntity<EvaluateItemEntity>(entity.EvaluateItemId);
                if (evaluateItem == null) throw new Exception("未找到打分项");
                evaluateItem.ActualScore = evaluateItem.ActualScore - oldRecord.Score + entity.Score;//减去旧的分数，加上新的评分，就得到最新的得分
                if (evaluateItem.ActualScore < 0) throw new Exception("最终评分不能小于0，请修改加/减分的分数");
                //计算权重分
                WeightSetEntity weightSet = tran.FindEntity<WeightSetEntity>(evaluateItem.WeightId);
                if (weightSet != null)
                {
                    evaluateItem.WeightScore = Math.Round(Convert.ToDecimal(evaluateItem.ActualScore * weightSet.Weight), 2);
                }

                tran.Update(evaluateItem);
                oldRecord.Score = entity.Score;
                oldRecord.Cause = entity.Cause;
                oldRecord.Modify(oldRecord.Id);
                tran.Update(oldRecord);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 删除考评记录
        /// </summary>
        /// <param name="id"></param>
        public void RemoveMarksRecord(string id)
        {
            var tran = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var record = tran.FindEntity<EvaluateMarksRecordsEntity>(id);
                if (record == null) throw new Exception("找不到要删除的数据");
                tran.Delete(record);
                //删除数据之后 ，最终评分得还原
                var evaluateItem = tran.FindEntity<EvaluateItemEntity>(record.EvaluateItemId);
                evaluateItem.ActualScore -= record.Score;
                if (evaluateItem.ActualScore < 0) evaluateItem.ActualScore = 0;//减到0以下，就归零就行
                //计算权重分
                WeightSetEntity weightSet = tran.FindEntity<WeightSetEntity>(evaluateItem.WeightId);
                if (weightSet != null)
                {
                    evaluateItem.WeightScore = Math.Round(Convert.ToDecimal(evaluateItem.ActualScore * weightSet.Weight), 2);
                }
                tran.Update(evaluateItem);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public List<EvaluateScoreDetail> EvaluateScoreDetail(string id, string CategoryId, string groupid)
        {

            var db = new RepositoryFactory().BaseRepository();
            var queryCategory = db.IQueryable<EvaluateCategoryItemEntity>();
            if (!string.IsNullOrWhiteSpace(CategoryId)) queryCategory.Where(p => p.CategoryId == CategoryId);
            var queryWhere = from q1 in db.IQueryable<EvaluateItemEntity>()
                             join q2 in db.IQueryable<EvaluateGroupEntity>() on q1.EvaluateGroupId equals q2.EvaluateGroupId into into2
                             from t2 in into2.DefaultIfEmpty()
                             join q3 in queryCategory on q1.EvaluateContentId equals q3.ItemId into into3
                             from t3 in into3.DefaultIfEmpty()
                             join q5 in db.IQueryable<EvaluateCategoryEntity>() on t3.CategoryId equals q5.CategoryId into into5
                             from t5 in into5.DefaultIfEmpty()
                             join q4 in db.IQueryable<EvaluateMarksRecordsEntity>() on q1.EvaluateItemId equals q4.EvaluateItemId into into4
                             from t4 in into4.DefaultIfEmpty()
                             where t2.EvaluateId == id && t2.GroupId == groupid && !string.IsNullOrEmpty(t4.Id)
                             select new EvaluateScoreDetail()
                             {
                                 ActualScore = q1.ActualScore,
                                 Score = q1.Score,
                                 WeightScore = q1.WeightScore,
                                 ItemStandard = t3.ItemStandard,
                                 MarksScore = t4.Score,
                                 ScoreType = t4.Score > 0 ? "加分" : "扣分",
                                 Records = t4,
                                 Category = t5.Category
                             };
            var data = queryWhere.ToList();
            return data;
        }
        /// <summary>
        ///平台首页- 获取最新的六条实时扣分信息
        /// </summary>
        /// <returns></returns>
        public List<EvaluateRecord> GetIndexGradeInfo()
        {
            var query = from q1 in _context.Set<EvaluateMarksRecordsEntity>()
                        join q2 in _context.Set<EvaluateItemEntity>() on q1.EvaluateItemId equals q2.EvaluateItemId
                        join q3 in _context.Set<WeightSetEntity>() on q2.WeightId equals q3.Id
                        join q4 in _context.Set<EvaluateGroupEntity>() on q2.EvaluateGroupId equals q4.EvaluateGroupId
                        orderby q1.CreateDate descending
                        select new { q1.Score, q1.Cause, q1.CreateDate, q3.Weight, q4.GroupName };
            var data = query.Take(6).Select(x => new EvaluateRecord { Score = x.Score, Cause = x.Cause, CreateDate = x.CreateDate, GroupName = x.GroupName, Weight = x.Weight }).ToList();

            return data;



            //      string sql = @"select t.Score,t.Cause,t.CREATEDATE,t.Weight,g.GROUPNAME from  (  select a.Score ,a.Cause ,a.CREATEDATE ,c.Weight,b.evaluategroupid  from wg_evaluatemarksrecords a left join wg_evaluateitem b 
            //on a.EvaluateItemId=b.EvaluateItemId
            //LEFT JOIN wg_WeightSet c on b.WeightId=c.ID order by a.CREATEDATE desc LIMIT 6)  as t  left join wg_evaluategroup g on t.evaluategroupid = g.evaluategroupid";
            //      DataTable dt = new RepositoryFactory().BaseRepository().FindTable(sql);
            //return dt;
        }
    }
    public class ScoreData
    {
        public string GroupId { get; internal set; }
        public string DeptId { get; internal set; }
        public string DeptName { get; internal set; }
        public decimal ActualScore { get; internal set; }
        public string EnCode1 { get; internal set; }
        public string EnCode2 { get; internal set; }
        public string GroupName { get; internal set; }
        public decimal Score { get; internal set; }
        public string GroupById { get; internal set; }
        public string GroupByName { get; internal set; }
        public string TitleName { get; set; }
        public decimal? WeightScore;
        /// <summary>
        /// 权重
        /// </summary>
        public decimal Weight { get; set; }
        public string ClassName { get; internal set; }
        public string CategoryId { get; internal set; }
        public string TitleId { get; set; }

        public string Tid { get; set; }
    }
}


