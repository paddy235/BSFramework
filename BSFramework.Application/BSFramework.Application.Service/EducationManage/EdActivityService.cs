using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Data;
using BSFramework.Data;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using System.Text;
using BSFramework.Application.Service.PublicInfoManage;
using Bst.ServiceContract.MessageQueue;
using System.ServiceModel;
using Newtonsoft.Json.Linq;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.EF;

namespace BSFramework.Application.Service.EducationManage
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EdActivityService : RepositoryFactory<EdActivityEntity>, EdActivityIService
    {

        private System.Data.Entity.DbContext _context;
        public EdActivityService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }



        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="deptid">查询参数</param>
        /// <param name="name">查询参数</param>
        /// <returns>返回列表</returns>
        public List<ActivityCategoryEntity> GetIndex(string userid, string deptid, string name)
        {
            var db = new RepositoryFactory().BaseRepository();

            //var query = from q1 in db.IQueryable<ActivityCategoryEntity>()
            //            join q2 in db.IQueryable<EdActivityEntity>() on q1.ActivityCategory equals q2.ActivityType into into1
            //            where (q1.DeptId == null || q1.DeptId.Contains(deptid))&&q1.CreateUser=="education"
            //            select new { q1, into1 = into1.Where(x => x.GroupId == deptid && x.State != "Finish") };

            //if (!string.IsNullOrEmpty(name)) query = from q in query
            //                                         select new { q.q1, into1 = q.into1.Where(x => x.Subject.Contains(name)) };

            //var query2 = from q in query
            //             orderby q.q1.CreateTime
            //             select new { q.q1.ActivityCategoryId, q.q1.ActivityCategory, q.q1.DeptId, Count = q.into1.Count() };

            //var query = this.BaseRepository().IQueryable().Where(x => x.GroupId == deptid && x.State != "Finish");
            //if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Subject.Contains(name));
            //return query.GroupBy(x => x.ActivityType).ToDictionary(x => x.Key, x => x.Count());

            //var model = query2.ToList().Select(x => new ActivityCategoryEntity() { ActivityCategoryId = x.ActivityCategoryId, ActivityCategory = x.ActivityCategory, DeptId = x.DeptId, Total = x.Count }).ToList();
            var model = new List<ActivityCategoryEntity>() { new ActivityCategoryEntity() { ActivityCategoryId = Guid.NewGuid().ToString(), ActivityCategory = "安全学习日", DeptId = deptid, Total = 0 } };
            //var user = OperatorProvider.Provider.Current();
            if (userid != null)
            {
                var safety = db.IQueryable<SafetydayReadEntity>().Where(x => x.Deptid == deptid && x.IsRead == 0 && x.Userid == userid);
                foreach (ActivityCategoryEntity ace in model.ToList())
                {
                    var state = db.FindEntity<EdActivityEntity>(x => x.GroupId == deptid && x.ActivityType == ace.ActivityCategory && (x.State == "Study" || x.State == "Ready"));
                    if (state != null)
                    {
                        if (state.State == "Ready")
                        {
                            if (!string.IsNullOrEmpty(state.Leader) && state.Leader.Length > 0)
                            {
                                ace.State = 3;
                            }
                            else
                            {
                                ace.State = 1;
                            }
                        }
                        else if (state.State == "Study")
                        {
                            ace.State = 2;
                        }

                    }
                    if (ace.ActivityCategory == "安全学习日")
                    {
                        if (safety != null)
                        {
                            ace.Total = safety.Where(x => x.activitytype == "安全学习日").ToList().Count();
                        }
                        else
                        {
                            ace.Total = 0;
                        }
                    }

                }
            }
            return model;
        }
        /// <summary>
        /// 获取类别/
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActivityCategoryEntity GetCategory(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<ActivityCategoryEntity>(keyvalue);
        }

        /// <summary>
        /// 获取所有类别
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public List<ActivityCategoryEntity> GetCategoryList()
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<ActivityCategoryEntity>()
                        where !string.IsNullOrEmpty(q1.deptname)
                        select q1;
            return query.ToList(); ;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public EdActivityEntity GetEntity(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.FindEntity<EdActivityEntity>(keyValue);
            if (model != null)
            {
                model.Files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == keyValue).ToList();
                model.ActivityPersons = db.IQueryable<EdActivityPersonEntity>().OrderBy(x => x.Person).Where(x => x.ActivityId == keyValue).ToList();
            }
            return model;
        }

        public List<ActivityEvaluateEntity> GetEntityList()
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<ActivityEvaluateEntity>().ToList();
            return model;
        }
        public List<EdActivitySupplyEntity> GetSupplyById()
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<EdActivitySupplyEntity>().ToList();
            return model;
        }
        public EdActivitySupplyEntity GetActivitySupplyEntity(string id)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.FindEntity<EdActivitySupplyEntity>(id);
            return model;
        }
        public SupplyPeopleEntity GetSupplyPeopleEntity(string id)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.FindEntity<SupplyPeopleEntity>(id);
            return model;
        }
        public List<SupplyPeopleEntity> GetPeopleById()
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<SupplyPeopleEntity>().ToList();
            return model;
        }

        public void SaveActivitySupply(string keyValue, EdActivitySupplyEntity entity)
        {
            var db = new Repository<EdActivitySupplyEntity>(DbFactory.Base());

            try
            {
                EdActivitySupplyEntity entity1 = this.GetActivitySupplyEntity(keyValue);
                if (entity1 == null)
                {
                    db.Insert(entity);
                }
                else
                {

                    entity1.EndDate = entity.EndDate;
                    entity1.IsOver = entity.IsOver;
                    entity1.UserName = entity.UserName;
                    db.Update(entity1);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SaveSupplyPeople(string keyValue, SupplyPeopleEntity entity)
        {
            var db = new Repository<SupplyPeopleEntity>(DbFactory.Base());

            try
            {
                SupplyPeopleEntity entity1 = this.GetSupplyPeopleEntity(keyValue);
                if (entity1 == null)
                {
                    db.Insert(entity);
                }
                else
                {


                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SaveActivityPerson(EdActivityPersonEntity entity)
        {
            var db = new Repository<EdActivityPersonEntity>(DbFactory.Base());
            db.Update(entity);

        }
        public int GetActivityList(string deptid, string from, string to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<EdActivityEntity>(x => x.GroupId == deptid && x.ActivityType == "安全学习日");
            if (!string.IsNullOrWhiteSpace(from))
            {
                DateTime start;
                if (DateTime.TryParse(from, out start))
                {
                    query = query.Where(x => x.StartTime > start);
                }
            }
            if (!string.IsNullOrWhiteSpace(to))
            {
                DateTime end;
                if (DateTime.TryParse(to, out end))
                {
                    query = query.Where(x => x.EndTime < end);
                }
            }
            return query.Count();
            //var sql = string.Format(" select count(*) from wg_activity where groupid = '{0}' and activitytype = '安全学习日' and starttime > '{1}' and starttime < '{2}'", deptid, from, to);
            //            select a.departmentid,a.fullname,COUNT(b.activityid) as count 
            //from base_department a 
            //left join wg_activity b 
            //on a.departmentid = b.groupid 
            //and b.activitytype = '安全日活动' 
            //and b.starttime > '2018-11-21' 
            //where nature='班组' group by a.departmentid,a.fullname
            //var data = db.FindTable(sql);
            //return Convert.ToInt32(data.Rows[0][0]);
        }
        /// <summary>
        /// 获取班组活动列表
        /// </summary>
        /// <param name="page">请求页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="total">记录总数</param>
        /// <param name="status">状态（0：未开展，1：已结束，2：所有）</param>
        /// <param name="subject">活动主题</param>
        /// <returns></returns>
        public System.Collections.IList GetList(int page, int pageSize, out int total, int status, string subject, string bzDepart)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(subject)) query = query.Where(x => x.Subject.Contains(subject));
            if (!string.IsNullOrEmpty(bzDepart)) query = query.Where(x => x.GroupId == bzDepart);
            if (status == 0)
            {
                query = query.Where(x => x.State != "Finish");
            }
            if (status == 1)
            {
                query = query.Where(x => x.State == "Finish");
            }
            total = query.Count();
            return query.OrderByDescending(x => x.StartTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList().Select(x => new
            {
                x.ActivityId,
                ActivityTime = x.StartTime.ToString("yyyy-MM-dd HH:mm"),
                x.ActivityLimited,
                x.Subject
            }).ToList();
        }
        public List<EdActivityEntity> GetList(int status, string bzDepart, DateTime? fromtime, DateTime? to)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(bzDepart)) query = query.Where(x => bzDepart.Contains(x.GroupId));
            if (fromtime != null) query = query.Where(x => x.StartTime >= fromtime);
            if (to != null) query = query.Where(x => x.StartTime <= to);
            if (status == 0)
            {
                query = query.Where(x => x.State != "Finish");
            }
            if (status == 1)
            {
                query = query.Where(x => x.State == "Finish");
            }
            return query.ToList();
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

            if (queryParam.SelectToken("activitytype") != null && queryParam.SelectToken("activitytype").Value<string>() != "全部")
            {
                pagination.conditionJson += string.Format(" and activitytype ='{0}'", queryParam.SelectToken("activitytype").Value<string>());
            }
            if (queryParam.SelectToken("sdt") != null && !string.IsNullOrEmpty(queryParam.SelectToken("sdt").Value<string>()))
            {
                pagination.conditionJson += string.Format(" and starttime >='{0}'", queryParam.SelectToken("sdt").Value<string>());
            }
            if (queryParam.SelectToken("edt") != null && !string.IsNullOrEmpty(queryParam.SelectToken("edt").Value<string>()))
            {
                pagination.conditionJson += string.Format(" and endtime <='{0} 23:59:59'", queryParam.SelectToken("edt").Value<string>());
            }

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString().Trim();
                pagination.conditionJson += string.Format(" and (subject like '%{0}%' or a.fullname like '%{0}%' or activityplace like '%{0}%')", keyword);
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
            //if (!queryParam["deptid"].IsEmpty())
            //{
            //    var db = new RepositoryFactory().BaseRepository();
            //    string deptId = queryParam["deptid"].ToString();
            //    var deptIdEntity = db.FindEntity<DepartmentEntity>(deptId);
            //    if (deptId == "0" || db.IQueryable<DepartmentEntity>().Where(x => x.DepartmentId == deptId).Count() == 0 || deptIdEntity.Nature == "厂级")
            //    {
            //        //deptId = db.IQueryable<DepartmentEntity>().Where(x => x.ParentId == "0" && x.Nature == "厂级").FirstOrDefault().DepartmentId;
            //    }
            //    else
            //    {
            //        List<DepartmentEntity> depts = GetSubDepartments(deptId, "").ToList();
            //        string strDeptId = string.Empty;
            //        foreach (DepartmentEntity de in depts)
            //        {
            //            strDeptId += "'" + de.DepartmentId + "',";
            //        }
            //        strDeptId = strDeptId.TrimEnd(',');
            //        pagination.conditionJson += string.Format(" and groupid in ({0})", strDeptId);
            //    }
            //}
            //else
            //{
            //    var db = new RepositoryFactory().BaseRepository();
            //    string deptId = OperatorProvider.Provider.Current().DeptId;
            //    if (deptId == "0")
            //    {
            //        //deptId = db.IQueryable<DepartmentEntity>().Where(x => x.ParentId == "0" && x.Nature == "厂级").FirstOrDefault().DepartmentId;
            //    }
            //    else
            //    {
            //        List<DepartmentEntity> depts = GetSubDepartments(deptId, "").ToList();
            //        string strDeptId = string.Empty;
            //        foreach (DepartmentEntity de in depts)
            //        {
            //            strDeptId += "'" + de.DepartmentId + "',";
            //        }
            //        strDeptId = strDeptId.TrimEnd(',');
            //        pagination.conditionJson += string.Format(" and groupid in ({0})", strDeptId.ToString().TrimEnd(','));
            //    }
            //}
            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            return dt;
        }



        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPagesList(Pagination pagination, string queryJson, string type, string select, string deptid, string category, string isEvaluate, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            #region  controller 查询
            var queryOne = from a in db.IQueryable<EdActivityEntity>()
                           join b in db.IQueryable<DepartmentEntity>() on a.GroupId equals b.DepartmentId into t1
                           from tb1 in t1.DefaultIfEmpty()
                           join c in db.IQueryable<DepartmentEntity>() on tb1.ParentId equals c.DepartmentId into t2
                           from tb2 in t2.DefaultIfEmpty()
                           join d in (from e in db.IQueryable<ActivityEvaluateEntity>()
                                      join f in db.IQueryable<EdActivityEntity>() on e.Activityid equals f.ActivityId into t3
                                      from tb3 in t3
                                      group tb3 by tb3.ActivityId into t4
                                      select new { number = t4.Count(), id = t4.Key }
                           ) on a.ActivityId equals d.id into t5
                           from tb5 in t5.DefaultIfEmpty()
                           select new
                           {
                               activityid = a.ActivityId,
                               PjCount = (from g in db.IQueryable<ActivityEvaluateEntity>()
                                          where g.Activityid == tb5.id && g.EvaluateId == userid
                                          select g).Count(),
                               activitytype = a.ActivityType,
                               subject = a.Subject,
                               planstarttime = a.PlanStartTime,
                               planendtime = a.PlanEndTime,
                               starttime = a.StartTime,
                               endtime = a.EndTime,
                               activityplace = a.ActivityPlace,
                               state = a.State,
                               fullname = tb1.FullName,
                               deptname = tb2.FullName,
                               groupid = a.GroupId,
                               encode = tb1.EnCode,
                               number = tb5.number
                           };

            if (type == "4")
            {
                int month = 1;
                if (DateTime.Now.Month < 4) month = 1;
                else if (DateTime.Now.Month < 7) month = 4;
                else if (DateTime.Now.Month < 10) month = 7;
                else if (DateTime.Now.Month <= 12) month = 10;
                var sdt = new DateTime(DateTime.Now.Year, month, 1);  //当前季度开始日期
                var edt = DateTime.Now;
                queryOne = queryOne.Where(x => x.starttime >= sdt && x.starttime < edt);
                //pagination.conditionJson += string.Format(" and starttime >= '{0}' and starttime < '{1}'", sdt, edt);

            }
            if (!string.IsNullOrEmpty(select)) deptid = select;
            if (!string.IsNullOrEmpty(deptid))
            {
                var dept = new DepartmentService().GetEntity(deptid);
                if (dept == null) dept = new DepartmentService().GetRootDepartment();
                var depts = new DepartmentService().GetSubDepartments(deptid, null);
                var deptStr = string.Join("','", depts.Select(x => x.DepartmentId));
                queryOne = queryOne.Where(x => deptStr.Contains(x.groupid));

                //pagination.conditionJson += string.Format(" and groupid in ('{0}')", string.Join("','", depts.Select(x => x.DepartmentId)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                var categories = new string[] { "上级精神宣贯", "安全学习日", "政治学习", "班务会", "民主管理会", "节能记录", "制度学习", "劳动保护监查" };
                if (categories.Contains(category))
                {
                    queryOne = queryOne.Where(x => x.activitytype == category);
                    //pagination.conditionJson += " and activitytype = '" + category + "'";

                }
                else
                {

                    queryOne = queryOne.Where(x => categories.Contains(x.activitytype));
                    //pagination.conditionJson += " and activitytype not in ('" + string.Join("','", categories) + "')";
                }
            }
            if (isEvaluate == "本人已评价")
            {
                queryOne = queryOne.Where(x => x.PjCount > 0);
                //pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='{0}')>0 ", user.UserId);

            }
            else if (isEvaluate == "本人未评价")
            {
                queryOne = queryOne.Where(x => x.PjCount == 0);
                // pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='{0}')=0 ", user.UserId);
            }
            #endregion
            #region 视图查询

            var queryParam = queryJson.ToJObject();

            if (queryParam.SelectToken("activitytype") != null && queryParam.SelectToken("activitytype").Value<string>() != "全部")
            {
                var activitytype = queryParam.SelectToken("activitytype").Value<string>();
                queryOne = queryOne.Where(x => x.activitytype == activitytype);
                //pagination.conditionJson += string.Format(" and activitytype ='{0}'", queryParam.SelectToken("activitytype").Value<string>());
            }
            if (queryParam.SelectToken("sdt") != null && !string.IsNullOrEmpty(queryParam.SelectToken("sdt").Value<string>()))
            {
                var sdt = Convert.ToDateTime(queryParam.SelectToken("sdt").Value<string>());
                queryOne = queryOne.Where(x => x.starttime >= sdt);
                //pagination.conditionJson += string.Format(" and starttime >='{0}'", queryParam.SelectToken("sdt").Value<string>());
            }
            if (queryParam.SelectToken("edt") != null && !string.IsNullOrEmpty(queryParam.SelectToken("edt").Value<string>()))
            {
                var edt = Convert.ToDateTime(queryParam.SelectToken("edt").Value<string>() + " 23:59:59");
                queryOne = queryOne.Where(x => x.endtime <= edt);
                //   pagination.conditionJson += string.Format(" and endtime <='{0} 23:59:59'", queryParam.SelectToken("edt").Value<string>());
            }

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString().Trim();
                queryOne = queryOne.Where(x => x.subject.Contains(keyword) || x.fullname.Contains(keyword) || x.activityplace.Contains(keyword));
                //pagination.conditionJson += string.Format(" and (subject like '%{0}%' or a.fullname like '%{0}%' or activityplace like '%{0}%')", keyword);
            }
            //使用页面没有传入此参数condition
            ////查询条件
            //if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            //{
            //    string condition = queryParam["condition"].ToString();
            //    string keyord = queryParam["keyword"].ToString();
            //    switch (condition)
            //    {
            //        case "Account":            //账户

            //           // pagination.conditionJson += string.Format(" and ACCOUNT  like '%{0}%'", keyord);
            //            break;
            //        case "RealName":          //姓名
            //            pagination.conditionJson += string.Format(" and REALNAME  like '%{0}%'", keyord);
            //            break;
            //        case "Mobile":          //手机
            //            pagination.conditionJson += string.Format(" and MOBILE like '%{0}%'", keyord);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            #endregion
            pagination.records = queryOne.Count();
            var querytwo = queryOne.OrderByDescending(x => x.starttime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var queryTalbe = DataHelper.ConvertToTable(querytwo);
            return queryTalbe;

        }
        public List<DepartmentEntity> GetSubDepartments(string id, string category)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == id
                          select q;

            var subquery = from q in db.IQueryable<DepartmentEntity>()
                           where q.ParentId == id
                           select q;

            var list = default(List<string>);

            if (!string.IsNullOrEmpty(category))
            {
                list = category.Split(',').ToList();
                subquery = subquery.Where(x => list.Contains(x.Nature));
            }

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q in db.IQueryable<DepartmentEntity>()
                           join q1 in subquery on q.ParentId equals q1.DepartmentId
                           select q;

                if (list != null && list.Count > 0) subquery = subquery.Where(x => list.Contains(x.Nature));
            }

            return current.ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ActivityCategoryEntity GetCategoryEntity(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var model = db.FindEntity<ActivityCategoryEntity>(keyValue);
            return model;
        }
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int GetIndexEntity(string keyValue)
        {
            int i = 0;
            var db = new RepositoryFactory().BaseRepository();
            i = db.IQueryable<EdActivityEntity>().Where(x => x.ActivityType == keyValue).ToList().Count;
            return i;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        ///修改
        /// </summary>
        /// <param name="entity"></param>
        public void modfiyEntity(EdActivityEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EdActivityEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (string.IsNullOrEmpty(keyValue))
                {
                    entity.ActivityRecords = null;
                    foreach (var item in entity.ActivityPersons)
                    {
                        item.ActivityId = entity.ActivityId;
                    }
                    this.BaseRepository().Insert(entity);

                    var resFile = new Repository<FileInfoEntity>(DbFactory.Base());
                    List<SafetydayMaterialEntity> SafetydayList = new List<SafetydayMaterialEntity>();
                    var file = resFile.IQueryable().Where(x => x.RecId == entity.ActivityId);
                    foreach (var f in file)
                    {
                        #region  已学习材料
                        if (!string.IsNullOrEmpty(f.FolderId))
                        {
                            var Safetyday = db.FindEntity<SafetydayMaterialEntity>(x => x.fileid == f.FolderId && x.deptid == entity.GroupId);
                            if (Safetyday != null)
                            {
                                Safetyday.isread = true;
                                SafetydayList.Add(Safetyday);

                            }
                        }
                        #endregion
                    }
                    db.Update(SafetydayList);
                    db.Insert(entity.Files.ToList());
                    db.Insert(entity.ActivityPersons.ToList());
                }
                else
                {
                    var entity1 = this.GetEntity(keyValue);
                    entity1.StartTime = entity.StartTime;
                    entity1.EndTime = entity.EndTime;
                    entity1.PlanStartTime = entity.PlanStartTime;
                    entity1.PlanEndTime = entity.PlanEndTime;
                    entity1.ActivityLimited = entity.ActivityLimited;
                    entity1.ActivityPlace = entity.ActivityPlace;
                    entity1.Subject = entity.Subject;
                    entity1.ChairPerson = entity.ChairPerson;
                    entity1.RecordPerson = entity.RecordPerson;
                    entity1.Leader = entity.Leader;
                    entity1.AlertType = entity.AlertType;
                    entity1.AlertTime = entity.AlertTime;
                    entity1.ActivityType = entity.ActivityType;
                    entity1.ActivityRecords = null;
                    entity1.ActivityPersons = null;
                    entity1.Supplys = null;
                    entity1.State = "Ready";

                    foreach (var item in entity.ActivityPersons)
                    {
                        item.ActivityId = entity.ActivityId;
                    }
                    db.Insert(entity.ActivityPersons.ToList());

                    if (entity1.Files.Count(x => x.Description == "二维码") == 0)
                    {
                        var images = entity.Files.Where(x => x.Description == "二维码").ToList();
                        db.Insert(images);
                    }
                    entity1.Files = null;
                    db.Update(entity1);
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public void SaveFormSafetyday(string keyValue, EdActivityEntity entity, List<SafetydayEntity> safetyday, string userId)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            var user = db.FindEntity<UserInfoEntity>(userId);
            var dept = db.FindEntity<DepartmentEntity>(user.DepartmentId);
            var activity = this.GetEntity(keyValue);
            try
            {
                if (string.IsNullOrEmpty(keyValue) || activity == null)
                {
                    if (entity.ActivityType == "安全日活动")
                    {
                        var resGroup = new Repository<OrderinfoEntity>(DbFactory.Base());
                        OrderinfoEntity order = new OrderinfoEntity
                        {
                            DeptCode = "",
                            SdId = "",
                            GroupId = user.DepartmentId,
                            IsOrder = 0,
                            GroupName = dept.FullName
                        };
                        //order.Create();
                        order.Id = Guid.NewGuid().ToString();
                        order.CreateDate = DateTime.Now;
                        order.CreateUserId = userId;
                        order.CreateUserName = user.RealName;
                        resGroup.Insert(order);
                    }
                    //var fileInfo = db.IQueryable<FileInfoEntity>(x => x.RecId == entity.ActivityId);
                    var fileInfo = new List<FileInfoEntity>();
                    if (entity.Files != null)
                    {
                        var resFile = new Repository<FileInfoEntity>(DbFactory.Base());
                        List<SafetydayMaterialEntity> SafetydayList = new List<SafetydayMaterialEntity>();
                        foreach (FileInfoEntity f in entity.Files)
                        {

                            if (f.State == 0 || f.State == 1 || f.State == null)
                            {
                                if (db.FindEntity<FileInfoEntity>(f.FileId) == null)
                                {
                                    fileInfo.Add(f);
                                }
                            }
                            #region  已学习材料
                            if (!string.IsNullOrEmpty(f.FolderId))
                            {
                                var Safetyday = db.FindEntity<SafetydayMaterialEntity>(x => x.fileid == f.FolderId && x.deptid == entity.GroupId);
                                if (Safetyday != null)
                                {
                                    Safetyday.isread = true;
                                    SafetydayList.Add(Safetyday);

                                }
                            }
                            #endregion
                        }
                        db.Update(SafetydayList);
                        resFile.Insert(fileInfo);
                    }
                    var resAct = new Repository<EdActivityEntity>(DbFactory.Base());
                    if (entity.ActivityId == null)
                    {
                        entity.ActivityId = Guid.NewGuid().ToString();
                    }
                    //else
                    //{
                    //    entity.ActivityId = keyValue;
                    //}
                    resAct.Insert(entity);//批量插入班组活动
                    var resPerson = new Repository<EdActivityPersonEntity>(DbFactory.Base());
                    resPerson.Delete(x => x.ActivityId == entity.ActivityId);
                    foreach (var item in entity.ActivityPersons)
                    {
                        item.ActivityId = entity.ActivityId;
                        if (string.IsNullOrEmpty(item.ActivityPersonId))
                        {
                            item.ActivityPersonId = Guid.NewGuid().ToString();
                        }
                    }
                    resPerson.Insert(entity.ActivityPersons.ToList());

                    //foreach (SafetydayEntity se in safetyday)
                    //{
                    //    StringBuilder sb = new StringBuilder();
                    //    if (!string.IsNullOrEmpty(se.ActIds) && se.ActIds != "")
                    //    {
                    //        sb.Append(se.ActIds + "," + entity.ActivityId + ",");
                    //    }
                    //    else
                    //    {
                    //        sb.Append(entity.ActivityId + ",");
                    //    }

                    //    se.ActIds = sb.ToString().TrimEnd(',');
                    //    var resSafetyday = new Repository<SafetydayEntity>(DbFactory.Base());
                    //    resSafetyday.Update(se);
                    //}
                }
                else
                {
                    var entity1 = this.GetEntity(keyValue);
                    entity1.StartTime = entity.StartTime;
                    entity1.EndTime = entity.EndTime;
                    entity1.PlanStartTime = entity.PlanStartTime;
                    entity1.PlanEndTime = entity.PlanEndTime;
                    entity1.ActivityLimited = entity.ActivityLimited;
                    entity1.ActivityPlace = entity.ActivityPlace;
                    entity1.Subject = entity.Subject;
                    entity1.ChairPerson = entity.ChairPerson;
                    entity1.RecordPerson = entity.RecordPerson;
                    entity1.Leader = entity.Leader;
                    entity1.AlertType = entity.AlertType;
                    entity1.AlertTime = entity.AlertTime;
                    entity1.ActivityType = entity.ActivityType;
                    entity1.ActivityRecords = null;
                    entity1.ActivityPersons = null;
                    entity1.State = "Ready";

                    foreach (var item in entity.ActivityPersons)
                    {
                        item.ActivityId = entity.ActivityId;
                    }
                    db.Insert(entity.ActivityPersons.ToList());

                    if (entity1.Files.Count(x => x.Description == "二维码") == 0)
                    {
                        var images = entity.Files.Where(x => x.Description == "二维码").ToList();
                        db.Insert(images);
                    }
                    entity1.Files = null;
                    db.Update(entity1);
                }

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public IEnumerable<EdActivityEntity> GetListBySql(string StrSql)
        {
            var query = this.BaseRepository();
            var sql = string.Format(@"select * from  wg_edactivity   where 1=1 {0}", StrSql);
            return query.FindList(sql);
        }


        public DataTable GetDataTable(string StrSql)
        {
            var query = this.BaseRepository();
            var sql = string.Format(@"{0}", StrSql);
            return query.FindTable(sql);
        }
        public IEnumerable<EdActivityEntity> GetList(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total)
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.GroupId == deptid && x.State == "Finish");
            if (!string.IsNullOrEmpty(category)) query = query.Where(x => x.ActivityType == category);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Subject.Contains(name));
            if (from != null) query = query.Where(x => x.PlanStartTime >= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.PlanEndTime <= to);
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.PlanStartTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            var db = new Repository<FileInfoEntity>(DbFactory.Base());
            data.ForEach(x => x.Files = db.IQueryable().Where(y => y.RecId == x.ActivityId && y.Description == "照片").OrderBy(y => y.CreateDate).Take(1).ToList());
            return data;
        }
        public IEnumerable<EdActivityEntity> GetListApp(string userid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();
            var userList = db.IQueryable<EdActivityPersonEntity>().Where(x => x.PersonId == userid).Select(x => x.ActivityId).ToList();
            var query = db.IQueryable<EdActivityEntity>().Where(x => userList.Contains(x.ActivityId) && x.State == "Finish");
            if (!string.IsNullOrEmpty(category)) query = query.Where(x => x.ActivityType == category);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Subject.Contains(name));
            if (from != null) query = query.Where(x => x.PlanStartTime >= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.PlanEndTime <= to);
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.PlanStartTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            if (pagesize > 0 && page > 0)
            {
                data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            }
            return data;
        }
        public IEnumerable<EdActivityEntity> GetAllList(DateTime from, string code)
        {
            var query = this.BaseRepository();
            var sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_activity a
left join wg_activityevaluate b on a.activityid = b.activityid
where a.starttime  > '{0}' and a.groupid in(select departmentid from base_department where encode like '{1}%' and nature = '班组')
group by a.activityid) a where total  =0", from, code);
            return query.FindList(sql);
        }
        public void Over(EdActivityEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<EdActivityEntity>(model.ActivityId);

                if (model.Files != null)
                {
                    foreach (var item in model.Files)
                    {
                        if (item.State == 2)
                            db.Delete<FileInfoEntity>(item.FileId);
                    }
                }
                entity.Files = null;
                entity.ActivityPersons = null;
                entity.ActivityRecords = null;
                entity.Remark = model.Remark;
                entity.EndTime = model.EndTime;
                entity.Leader = model.Leader;
                entity.State = model.State;

                db.Update(entity);

                List<string> pdf = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" };
                var fileBll = new FileInfoService();
                var files = fileBll.GetFilesByRecIdNew(entity.ActivityId).Where(x => x.Description != "二维码" && pdf.Contains(x.FileExtensions));
                using (var factory = new ChannelFactory<IQueueService>("upload"))
                {
                    var channel = factory.CreateChannel();
                    foreach (FileInfoEntity f in files)
                    {
                        channel.Upload(f.FileId);
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
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public EdActivityEntity GetActivities(string category, string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<EdActivityEntity>()
                        where q.ActivityType == category && q.GroupId == deptid && q.State != "Finish"
                        orderby q.CreateDate
                        select q;

            var data = query.FirstOrDefault();

            if (data != null)
            {
                data.ActivityPersons = (from q in db.IQueryable<EdActivityPersonEntity>()
                                        where q.ActivityId == data.ActivityId
                                        select q).ToList();

                data.Files = (from q in db.IQueryable<FileInfoEntity>()
                              where q.RecId == data.ActivityId
                              select q).OrderBy(x => x.CreateDate).ToList();
            }

            return data;
        }

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIndex(string deptId, DateTime? from, DateTime? to, string name)
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.GroupId == deptId);
            if (from != null) query = query.Where(x => x.PlanStartTime >= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.PlanEndTime <= to);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Subject.Contains(name));
            }
            return query.Count();
        }

        public void Start(EdActivityEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var persons = entity.ActivityPersons.ToList();
                persons.ForEach(x => x.ActivityId = entity.ActivityId);
                var files = entity.Files.ToList();
                entity.ActivityRecords = null;
                entity.ActivityPersons = null;
                entity.Files = null;
                db.Insert(entity);
                db.Insert(files);
                db.Insert(persons);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public void Ready(EdActivityEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = this.GetEntity(model.ActivityId);
                entity.StartTime = model.StartTime;
                entity.EndTime = model.EndTime;
                entity.PlanStartTime = model.PlanStartTime;
                entity.PlanEndTime = model.PlanEndTime;
                entity.ActivityLimited = model.ActivityLimited;
                entity.ActivityPlace = model.ActivityPlace;
                entity.Subject = model.Subject;
                entity.ChairPerson = model.ChairPerson;
                entity.RecordPerson = model.RecordPerson;
                entity.Leader = model.Leader;
                entity.AlertType = model.AlertType;
                entity.AlertTime = model.AlertTime;
                entity.ActivityType = model.ActivityType;
                entity.State = model.State;
                entity.ActivityRecords = null;
                entity.ActivityPersons = null;

                var existspersons = db.IQueryable<EdActivityPersonEntity>().Where(x => x.ActivityId == model.ActivityId).ToList();
                foreach (var item in model.ActivityPersons)
                {
                    var current = existspersons.Find(x => x.ActivityPersonId == item.ActivityPersonId);
                    if (current == null)
                    {
                        item.ActivityId = entity.ActivityId;
                        db.Insert(item);
                    }
                    else
                    {
                        current.IsSigned = item.IsSigned;
                        db.Update(current);
                    }
                    item.ActivityId = entity.ActivityId;
                }

                if (entity.Files.Count(x => x.Description == "二维码") == 0)
                {
                    var images = model.Files.Where(x => x.Description == "二维码").ToList();
                    db.Insert(images);
                }
                if (model.Files != null)
                {
                    foreach (var item in model.Files)
                    {
                        switch (item.State)
                        {
                            case 1:
                                db.Insert(item);
                                break;
                            case 2:
                                db.Delete<FileInfoEntity>(item.FileId);
                                break;
                            default:
                                break;
                        }
                    }
                }

                entity.Files = null;
                db.Update(entity);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public void Edit(EdActivityEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = this.GetEntity(model.ActivityId);
                entity.StartTime = model.StartTime;
                entity.EndTime = model.EndTime;
                entity.PlanStartTime = model.PlanStartTime;
                entity.PlanEndTime = model.PlanEndTime;
                entity.ActivityLimited = model.ActivityLimited;
                entity.ActivityPlace = model.ActivityPlace;
                entity.Subject = model.Subject;
                entity.ChairPerson = model.ChairPerson;
                entity.RecordPerson = model.RecordPerson;
                entity.Leader = model.Leader;
                entity.AlertType = model.AlertType;
                entity.AlertTime = model.AlertTime;
                entity.ActivityType = model.ActivityType;
                entity.State = model.State;
                entity.Remark = model.Remark;
                entity.ActivityRecords = null;
                entity.ActivityPersons = null;

                var existspersons = db.IQueryable<EdActivityPersonEntity>().Where(x => x.ActivityId == model.ActivityId).ToList();
                foreach (var item in model.ActivityPersons)
                {
                    var current = existspersons.Find(x => x.ActivityPersonId == item.ActivityPersonId);
                    if (current == null)
                    {
                        item.ActivityId = entity.ActivityId;
                        db.Insert(item);
                    }
                    else
                    {
                        current.IsSigned = item.IsSigned;
                        db.Update(current);
                    }
                    item.ActivityId = entity.ActivityId;
                }

                if (entity.Files.Count(x => x.Description == "二维码") == 0)
                {
                    var images = model.Files.Where(x => x.Description == "二维码").ToList();
                    db.Insert(images);
                }
                if (model.Files != null)
                {
                    foreach (var item in model.Files)
                    {
                        switch (item.State)
                        {
                            case 1:
                                db.Insert(item);
                                break;
                            case 2:
                                db.Delete<FileInfoEntity>(item.FileId);
                                break;
                            default:
                                break;
                        }
                    }
                }

                entity.Files = null;
                db.Update(entity);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public void Study(EdActivityEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = this.GetEntity(model.ActivityId);
                entity.StartTime = model.StartTime;
                entity.EndTime = model.EndTime;
                entity.PlanStartTime = model.PlanStartTime;
                entity.PlanEndTime = model.PlanEndTime;
                entity.ActivityLimited = model.ActivityLimited;
                entity.ActivityPlace = model.ActivityPlace;
                entity.Subject = model.Subject;
                entity.ChairPerson = model.ChairPerson;
                entity.RecordPerson = model.RecordPerson;
                entity.Leader = model.Leader;
                entity.AlertType = model.AlertType;
                entity.AlertTime = model.AlertTime;
                entity.ActivityType = model.ActivityType;
                entity.State = model.State;
                entity.ActivityRecords = null;
                entity.ActivityPersons = null;
                entity.Files = null;

                db.Update(entity);

                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public ActivityCategoryEntity AddCategory(ActivityCategoryEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var query = from q in db.IQueryable<ActivityCategoryEntity>()
                            where (q.DeptId == null || q.DeptId == model.DeptId) && q.ActivityCategory == model.ActivityCategory
                            select q;

                if (query.Count() > 0)
                    throw new Exception("已存在此活动类型");

                db.Insert(model);
                db.Commit();

                return model;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        /// <summary>
        /// 管理平台新增类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void AddCategoryType(ActivityCategoryEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var deptList = model.DeptId.Split(',');
                var del = new List<ActivityCategoryEntity>();
                foreach (var item in deptList)
                {
                    //获取原数据结构数据
                    var query = (from q in db.IQueryable<ActivityCategoryEntity>()
                                 where (q.DeptId == null || q.DeptId.Contains(model.DeptId)) && q.ActivityCategory == model.ActivityCategory
                                 select q).ToList();
                    if (query.Count() > 0) del.AddRange(query);
                }
                db.Delete(del);
                db.Insert(model);
                db.Commit();


            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public EdActivityEntity GetDetail(string id)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<EdActivityEntity>()
                        where q.ActivityId == id
                        select q;

            var data = query.FirstOrDefault();

            data.ActivityPersons = (from q in db.IQueryable<EdActivityPersonEntity>()
                                    where q.ActivityId == id
                                    select q).ToList();
            data.Supplys = (from q in db.IQueryable<EdActivitySupplyEntity>()
                            where q.ActivityId == id
                            select q).ToList();
            data.Files = (from q in db.IQueryable<FileInfoEntity>()
                          where q.RecId == id
                          select q).ToList();

            return data;
        }
        /// <summary>
        /// 管理平台删除类型
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public void DeleteCategoryType(string categoryid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var model = db.FindEntity<ActivityCategoryEntity>(categoryid);
                var newDeptId = string.Empty;
                var newdeptname = string.Empty;
                var DeptId = model.DeptId.Split(',');
                var deptname = model.deptname.Split(',');
                for (int i = 0; i < DeptId.Length; i++)
                {
                    var id = DeptId[i];
                    var activityquery = from q1 in db.IQueryable<ActivityCategoryEntity>()
                                        join q2 in db.IQueryable<EdActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
                                        where q1.ActivityCategoryId == categoryid && q2.GroupId == id
                                        select q2;
                    if (activityquery.Count() > 0)
                    {
                        if (i == DeptId.Length - 1)
                        {
                            newDeptId += DeptId[i];
                            newdeptname += deptname[i];
                        }
                        else
                        {
                            newDeptId += DeptId[i] + ",";
                            newdeptname += deptname[i] + ",";
                        }

                    }
                }
                if (string.IsNullOrEmpty(newDeptId))
                {
                    db.Delete<ActivityCategoryEntity>(categoryid);
                }
                else
                {
                    model.DeptId = newDeptId;
                    model.deptname = newdeptname;
                    db.Update(model);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        public void DeleteCategory(string categoryid)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var activityquery = from q1 in db.IQueryable<ActivityCategoryEntity>()
                                    join q2 in db.IQueryable<EdActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
                                    where q1.ActivityCategoryId == categoryid
                                    select q2;

                if (activityquery.Count() > 0) throw new Exception("该活动类型已存在活动！");

                var model = db.FindEntity<ActivityCategoryEntity>(categoryid);
                if (!string.IsNullOrEmpty(model.deptname))
                {
                    throw new Exception("该活动类型为管理平台添加，请在管理平台操作！");
                }
                db.Delete<ActivityCategoryEntity>(categoryid);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        /// <summary>
        /// 管理平台修改类型
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public void UpdateActivityCategoryType(ActivityCategoryEntity category)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var DeptId = category.DeptId.Split(',');
                var updateList = new List<EdActivityEntity>();
                for (int i = 0; i < DeptId.Length; i++)
                {
                    var id = DeptId[i];
                    var activityquery = from q1 in db.IQueryable<ActivityCategoryEntity>()
                                        join q2 in db.IQueryable<EdActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
                                        where q1.ActivityCategoryId == category.ActivityCategoryId && q2.GroupId == id
                                        select q2;
                    if (activityquery.Count() > 0)
                    {
                        var activities = activityquery.ToList();
                        activities.ForEach(x =>
                        {
                            x.ActivityType = category.ActivityCategory;
                            x.ActivityPersons = null;
                            x.Files = null;
                        });
                        updateList.AddRange(activities);
                    }

                }
                db.Update(category);
                db.Update(updateList);
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
        public ActivityCategoryEntity UpdateActivityCategory(ActivityCategoryEntity category)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var activityquery = from q1 in db.IQueryable<ActivityCategoryEntity>()
                                    join q2 in db.IQueryable<EdActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
                                    where q1.ActivityCategoryId == category.ActivityCategoryId
                                    select q2;

                var activities = activityquery.ToList();
                activities.ForEach(x =>
                {
                    x.ActivityType = category.ActivityCategory;
                    x.ActivityPersons = null;
                    x.Files = null;
                });

                var entity = db.FindEntity<ActivityCategoryEntity>(category.ActivityCategoryId);
                if (!string.IsNullOrEmpty(entity.deptname))
                {
                    throw new Exception("该活动类型为管理平台添加，请在管理平台操作！");
                }
                entity.ActivityCategory = category.ActivityCategory;

                db.Update(entity);
                db.Update(activities);

                db.Commit();

                return category;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        public string PostActivity(ActivityCategoryEntity activity)
        {
            return string.Empty;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void Del(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            db.Delete<ActivityCategoryEntity>(keyValue);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue"></param>
        public void SaveFormCategory(string keyValue, ActivityCategoryEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                //activitycategory,createtime,createuserid
                var entity1 = db.FindEntity<ActivityCategoryEntity>(keyValue);
                if (!string.IsNullOrEmpty(entity1.deptname))
                {
                    throw new Exception("该活动类型为管理平台添加，请在管理平台操作！");
                }
                var activityList = db.IQueryable<EdActivityEntity>().Where(x => x.ActivityType == entity1.ActivityCategory && x.GroupId == entity.DeptId).ToList();
                entity1.ActivityCategory = entity.ActivityCategory;
                entity1.CreateTime = entity.CreateTime;
                entity1.CreateUserId = entity.CreateUserId;
                var addList = new List<EdActivityEntity>();
                foreach (EdActivityEntity ae in activityList)
                {
                    ae.ActivityType = entity.ActivityCategory;
                    ae.ActivityPersons = null;
                    ae.Files = null;
                }
                db.Update<EdActivityEntity>(activityList);
                db.Update<ActivityCategoryEntity>(entity1);
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        #endregion

        public List<EdActivityEntity> GetActivities2(string userid, DateTime from, DateTime to, string category, string deptid, bool isall)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(userid);

            if (isall)
            {
                if (string.IsNullOrEmpty(deptid))
                {
                    var query = from q1 in db.IQueryable<EdActivityEntity>()
                                join q3 in db.IQueryable<FileInfoEntity>() on q1.ActivityId equals q3.RecId into into2
                                where q1.StartTime >= @from && q1.StartTime <= to && q1.State == "Finish"
                                orderby q1.StartTime descending
                                select new { q1.ActivityId, q1.Subject, q1.StartTime, q1.EndTime, q1.GroupId, q1.ActivityType, files = into2.Where(x => x.Description == "照片") };
                    if (!string.IsNullOrEmpty(category))
                    {
                        query = query.Where(x => x.ActivityType == category);
                    }
                    var data = query.ToList();
                    return data.Select(x => new EdActivityEntity() { ActivityId = x.ActivityId, GroupId = x.GroupId, Subject = x.Subject, ActivityType = x.ActivityType, StartTime = x.StartTime, EndTime = x.EndTime, Files = x.files.ToList() }).ToList();
                }
                else
                {
                    var query = from q1 in db.IQueryable<EdActivityEntity>()
                                join q3 in db.IQueryable<FileInfoEntity>() on q1.ActivityId equals q3.RecId into into2
                                where q1.GroupId == deptid && q1.StartTime >= @from && q1.StartTime <= to && q1.State == "Finish"
                                orderby q1.StartTime descending
                                select new { q1.ActivityId, q1.Subject, q1.StartTime, q1.EndTime, q1.GroupId, q1.ActivityType, files = into2.Where(x => x.Description == "照片") };
                    if (!string.IsNullOrEmpty(category))
                    {
                        query = query.Where(x => x.ActivityType == category);
                    }
                    var data = query.ToList();
                    return data.Select(x => new EdActivityEntity() { ActivityId = x.ActivityId, GroupId = x.GroupId, Subject = x.Subject, ActivityType = x.ActivityType, StartTime = x.StartTime, EndTime = x.EndTime, Files = x.files.ToList() }).ToList();
                }
            }
            else
            {
                var query = from q1 in db.IQueryable<EdActivityEntity>()
                            join q3 in db.IQueryable<FileInfoEntity>() on q1.ActivityId equals q3.RecId into into2
                            where q1.GroupId == user.DepartmentId && q1.StartTime >= @from && q1.StartTime <= to && q1.State == "Finish"
                            orderby q1.StartTime descending
                            select new { q1.ActivityId, q1.Subject, q1.StartTime, q1.EndTime, q1.ActivityType, files = into2.Where(x => x.Description == "照片") };
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(x => x.ActivityType == category);
                }
                var data = query.ToList();
                return data.Select(x => new EdActivityEntity() { ActivityId = x.ActivityId, Subject = x.Subject, ActivityType = x.ActivityType, StartTime = x.StartTime, EndTime = x.EndTime, Files = x.files.ToList() }).ToList();
            }

        }
        /// <summary>
        /// 获取安全学习日数量
        /// </summary>
        ///<param name="pagination">分页公用类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id|State状态|Category分类|haveEvaluate是否查询评价</param>
        /// <returns></returns>
        public List<EdActivityEntity> GetEdAcJobCount(Pagination pagination, string queryJson)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<EdActivityEntity>()
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
                query = query.Where(p => p.StartTime <= endTime);
            }
            //deptId部门id
            if (!queryParam["deptId"].IsEmpty())
            {
                string deptId = queryParam["deptId"].ToString();
                query = query.Where(p => p.GroupId == deptId);

            }
            // Category分类
            if (!queryParam["Category"].IsEmpty())
            {
                string Category = queryParam["Category"].ToString();
                query = query.Where(p => p.ActivityType == Category);

            }

            ////userId用户id
            //if (!queryParam["userId"].IsEmpty())
            //{
            //    var userId = queryParam["userId"].ToString();
            //    query = query.Where(p => p. == user.DepartmentId);
            //}

            //State状态
            if (!queryParam["State"].IsEmpty())
            {
                var State = queryParam["State"].ToString();
                query = query.Where(p => p.State == State);
            }
            int total = query.Count();
            pagination.records = total;
            var data = query.OrderByDescending(x=>x.StartTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            //haveEvaluate是否查询评价
            if (!queryParam["haveEvaluate"].IsEmpty())
            {
                var haveEvaluate = queryParam["haveEvaluate"].ToString();
                if (haveEvaluate == "1")
                {
                    data.ForEach(x => x.Evaluates = db.IQueryable<ActivityEvaluateEntity>(p => p.Activityid == x.ActivityId).ToList());

                }
            }
            return data;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveEvaluate(string keyValue, ActivityEvaluateEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            //var deptId = BSFramework.Application.Code.OperatorProvider.Provider.Current().DeptId;
            //if (deptId == "0")
            //{
            //    entity.Nature = entity.EvaluateUser;
            //}
            try
            {
                // var dept = db.FindEntity<DepartmentEntity>(deptId);
                //if (dept != null)
                //{
                //    entity.Nature = dept.Nature;
                // }
                db.Insert<ActivityEvaluateEntity>(entity);
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public List<EdActivityEntity> GetActivityList(string deptid, string StrTime)
        {
            //var db = new RepositoryFactory().BaseRepository();
            //var query = (from q1 in db.IQueryable<EdActivityEntity>()
            //        join q2 in categories on q1.GroupId equals q2.DepartmentId
            //        select q1).ToList();
            //return query;
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == deptid
                          select q;

            var subquery = from q in db.IQueryable<DepartmentEntity>()
                           where q.ParentId == deptid
                           select q;

            var list = default(List<string>);

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q in db.IQueryable<DepartmentEntity>()
                           join q1 in subquery on q.ParentId equals q1.DepartmentId
                           select q;

                if (list != null && list.Count > 0) subquery = subquery.Where(x => list.Contains(x.Nature));
            }
            var query = (from q1 in db.IQueryable<EdActivityEntity>().Where(x => x.State == "Finish")
                         join q2 in current on q1.GroupId equals q2.DepartmentId
                         select q1).ToList();
            var model = query.Where(x => x.EndTime.ToString("yyyy-MM") == DateTime.Parse(StrTime).ToString("yyyy-MM"));
            foreach (EdActivityEntity entity in model)
            {
                entity.GroupName = db.FindEntity<DepartmentEntity>(x => x.DepartmentId == entity.GroupId).FullName;
            }
            return model.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string keyValue, int pagesize, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<ActivityEvaluateEntity>(x => x.Activityid == keyValue).ToList();
            total = model.Count();
            return model.OrderByDescending(x => x.CREATEDATE).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="date"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<StatisticsNumModel> GetActivityStatisticsEntity(string keyValue, int pagesize, int page, string date, out int total)
        {
            if (keyValue != "")
            {
                var db = new RepositoryFactory().BaseRepository();

                var current = from q in db.IQueryable<DepartmentEntity>()
                              where q.DepartmentId == keyValue
                              select q;

                var subquery = from q in db.IQueryable<DepartmentEntity>()
                               where q.ParentId == keyValue
                               select q;

                var list = default(List<string>);

                while (subquery.Count() > 0)
                {
                    current = current.Concat(subquery);
                    subquery = from q in db.IQueryable<DepartmentEntity>()
                               join q1 in subquery on q.ParentId equals q1.DepartmentId
                               select q;

                    if (list != null && list.Count > 0) subquery = subquery.Where(x => list.Contains(x.Nature));
                }
                var strSql = @"SELECT groupid,FULLNAME as GroupName,Endtime,
                                SUM(case when activitytype= '安全日活动' then 1 else 0 end ) Safety,
                                SUM(case when activitytype= '民主生活会' then 1 else 0 end ) Democratic,
                                SUM(case when activitytype= '政治学习' then 1 else 0 end ) Politics,
                                SUM(case when activitytype= '班务会' then 1 else 0 end ) team,
                                SUM(case when activitytype!='安全日活动' and activitytype!='民主生活会'
                                and activitytype!='政治学习' and activitytype!='班务会'  then 1 else 0 end ) Elseactivity
                                from wg_activity a
                                LEFT JOIN base_department b on b.DEPARTMENTID=a.groupid 
                                WHERE endtime LIKE '%" + DateTime.Parse(date).ToString("yyyy-MM") + "%'GROUP BY groupid";

                var query = (from q1 in db.FindList<StatisticsNumModel>(strSql.ToString())
                             join q2 in current on q1.GroupId equals q2.DepartmentId
                             select q1).ToList();
                //foreach (EdActivityEntity entity in model)
                //{
                //    entity.GroupName = db.FindEntity<DepartmentEntity>(x => x.DepartmentId == entity.GroupId).FullName;
                //}

                total = query.Count();
                return query.OrderByDescending(x => x.Endtime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            }
            else
            {
                var query = new List<StatisticsNumModel>();
                total = query.Count();
                return query;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        public void Update(string keyValue, EdActivityEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataEntity = db.FindEntity<EdActivityEntity>(keyValue);
                entity.Modify(keyValue);
                dataEntity.Remark = entity.Remark;
                dataEntity.Leader = entity.Leader;
                dataEntity.ActivityPersons = null;
                dataEntity.Files = null;

                db.Update(dataEntity);
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        /// <summary>
        /// 后台管理安全日活动
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        public void ManagerUpdate(string keyValue, EdActivityEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity1 = this.GetEntity(keyValue);
                if (entity1 != null)
                {
                    entity1.StartTime = entity.StartTime;
                    entity1.EndTime = entity.EndTime;
                    //entity1.PlanStartTime = entity.PlanStartTime;
                    //entity1.PlanEndTime = entity.PlanEndTime;
                    entity1.ActivityLimited = entity.ActivityLimited;
                    entity1.ActivityPlace = entity.ActivityPlace;
                    entity1.Subject = entity.Subject;
                    entity1.ChairPerson = entity.ChairPerson;
                    entity1.RecordPerson = entity.RecordPerson;
                    entity1.Leader = entity.Leader;
                    entity1.AlertType = entity.AlertType;
                    entity1.AlertTime = entity.AlertTime;
                    entity1.ActivityType = entity.ActivityType;
                    entity1.ActivityRecords = null;
                    entity1.ActivityPersons = null;
                    entity1.Supplys = null;
                    //entity1.State = entity
                    entity1.Remark = entity.Remark;
                    entity1.Leader = entity.Leader;

                    if (entity1.Files.Count(x => x.Description == "二维码") == 0)
                    {
                        var images = entity.Files.Where(x => x.Description == "二维码").ToList();
                        db.Insert(images);
                    }
                    entity1.Files = null;

                    #region 修改参与人员状态
                    if (entity.PersonId != null)
                    {
                        //var Ulist = entity.PersonId.Split(',');
                        //var res = string.Empty;
                        //foreach (var item in Ulist)
                        //{
                        //    if (string.IsNullOrEmpty(item)) continue;
                        //    res += "'" + item + "'" + ","; ;
                        //}
                        //string sql = string.Format(" select activitypersonid from wg_activityperson d  where d.activityid='{0}' and d.personid not in ({1})", keyValue, res.TrimEnd(','));
                        //DataTable dt = db.FindTable(sql);
                        //foreach (DataRow item in dt.Rows)
                        //{
                        //    sql = string.Format("update wg_activityperson  set issigned=0 where activitypersonid='{0}'", item[0].ToString());
                        //    db.ExecuteBySql(sql);
                        //}

                        IList<EdActivityPersonEntity> list = (from q in db.IQueryable<EdActivityPersonEntity>() where q.ActivityId == keyValue select q).ToList();
                        foreach (var item in list)
                        {
                            if (!entity.PersonId.Split(',').Contains(item.PersonId))
                            {
                                item.IsSigned = false;
                            }
                            else
                            {
                                item.IsSigned = true;
                            }
                            db.Update(item);
                        }
                    }

                    #endregion

                    db.Update(entity1);
                }
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public List<ActivityCategoryEntity> GetActivityCategories(string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<ActivityCategoryEntity>()
                            //where q.DeptId == deptid
                        select q;

            return query.ToList();
        }

        public string[] GetLastest(string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var categories = new string[] { "政治学习", "班务会", "民主生活会" };
            var query = from q in db.IQueryable<EdActivityEntity>()
                        where q.GroupId == deptid && q.State == "Finish" && categories.Contains(q.ActivityType)
                        orderby q.StartTime descending
                        select q.Subject;

            return query.Take(5).ToArray();
        }

        public bool IsEvaluate(string id, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<ActivityEvaluateEntity>()
                        where q.Activityid == id && q.EvaluateId == userid
                        select q;
            return query.Count() > 0;
        }

        public List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string Activityid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<ActivityEvaluateEntity>(x => x.Activityid == Activityid).OrderByDescending(x => x.CREATEDATE).ToList();

            return model;
        }

        public List<ActivityEvaluateEntity> GetActivityEvaluateEntity(List<string> list)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<ActivityEvaluateEntity>(x => list.Contains(x.Activityid)).ToList();

            return model;
        }

        #region 评价设置
        public DataTable GetEvaluateSetData(Pagination pagination, string queryJson)
        {

            DatabaseType dataType = DbHelper.DbType;

            return this.BaseRepository().FindTableByProcPager(pagination, dataType);
        }


        public void SaveEvaluateSet(string keyvalue, EvaluateStepsEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                if (!string.IsNullOrEmpty(keyvalue))
                {

                    entity.Modify(keyvalue);
                    var ModelEntiyt = db.FindEntity<EvaluateStepsEntity>(keyvalue);
                    db.Delete(ModelEntiyt);
                    var Model = new EvaluateStepsEntity();
                    Model.Id = Guid.NewGuid().ToString();
                    Model.deptid = entity.deptid;
                    Model.deptname = entity.deptname;
                    Model.module = entity.module;
                    Model.modulename = entity.modulename;

                    Model.isdept = entity.isdept;
                    Model.isgroup = entity.isgroup;
                    Model.userrole = entity.userrole;
                    Model.userroleid = entity.userroleid;
                    Model.userjobs = entity.userjobs;
                    Model.sort = entity.sort == null ? null : entity.sort;
                    Model.isprofessional = entity.isprofessional;
                    Model.evaluatesort = entity.evaluatesort;
                    Model.modifydate = entity.modifydate;
                    Model.modifyuserid = entity.modifyuserid;
                    Model.modifyusername = entity.modifyusername;
                    Model.createdate = ModelEntiyt.createdate;
                    Model.createuserid = ModelEntiyt.createuserid;
                    Model.createusername = ModelEntiyt.createusername;
                    db.Insert(Model);

                }
                else
                {
                    entity.Create();
                    db.Insert(entity);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }

        public void deleteEvaluateSet(string keyvalue)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                db.Delete<EvaluateStepsEntity>(keyvalue);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public EvaluateStepsEntity getEvaluateSet(string keyvalue)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.FindEntity<EvaluateStepsEntity>(keyvalue);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<EvaluateStepsEntity> getEvaluateSetBymodule(string module)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.FindList<EvaluateStepsEntity>(x => x.module.Contains(module)).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //代办
        public List<EvaluateTodoEntity> WorkToDo(string userId, string module)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = (from q in db.IQueryable<EvaluateTodoEntity>()
                         where q.userid == userId
                         select q).ToList();

            if (!string.IsNullOrEmpty(module))
            {
                query = query.Where(x => x.module.Contains(module)).ToList();
            }
            return query;

        }

        public List<EvaluateTodoEntity> AcWorkToDo(string userId, string activityid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = (from q in db.IQueryable<EvaluateTodoEntity>()
                         where q.userid == userId && q.activityid == activityid
                         select q).ToList();



            return query;
        }
        //生成代办
        public void setToDo(string module, string activityid, string deptid)
        {
            try
            {
                IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
                var moduleTodoList = db.IQueryable<EvaluateStepsEntity>().Where(x => x.module.Contains(module)).OrderBy(x => x.evaluatesort);
                if (moduleTodoList.Count() > 0)
                {
                    IRepository dbQuery = new RepositoryFactory().BaseRepository();

                    foreach (var moduleTodo in moduleTodoList)
                    {
                        var workDeptid = string.Empty;
                        var userlist = new List<UserEntity>();
                        //获取操作部门
                        if (!string.IsNullOrEmpty(moduleTodo.deptid))
                        {
                            workDeptid = moduleTodo.deptid;
                        }
                        else if (moduleTodo.isdept)
                        {
                            var dept = dbQuery.FindEntity<DepartmentEntity>(deptid);
                            var parent = dbQuery.FindEntity<DepartmentEntity>(dept.ParentId);
                            workDeptid = parent.DepartmentId;
                        }
                        else
                        {
                            workDeptid = deptid;
                        }
                        userlist = dbQuery.IQueryable<UserEntity>().Where(x => x.DepartmentId == workDeptid).ToList();

                        if (!string.IsNullOrEmpty(moduleTodo.userrole))
                        {
                            userlist = userlist.Where(x => x.RoleName.Contains(moduleTodo.userrole)).ToList();
                        }

                        if (moduleTodo.isprofessional)
                        {
                            var dept = dbQuery.FindEntity<DepartmentEntity>(workDeptid);
                            if (!string.IsNullOrEmpty(dept.SpecialtyType))
                            {
                                userlist = userlist.Where(x => !string.IsNullOrEmpty(x.SpecialtyType)).ToList();
                                userlist = userlist.Where(x => x.SpecialtyType.Contains(dept.SpecialtyType)).ToList();
                            }

                        }
                        var newList = new List<UserEntity>();

                        if (!string.IsNullOrEmpty(moduleTodo.userjobs))
                        {
                            userlist = userlist.Where(x => !string.IsNullOrEmpty(x.DutyName)).ToList();
                            var jobs = moduleTodo.userjobs.Split(',');
                            foreach (var item in jobs)
                            {
                                var one = userlist.Where(x => x.DutyName.Contains(item)).ToList();
                                newList.AddRange(one);
                            }
                        }
                        var todoList = new List<EvaluateTodoEntity>();
                        foreach (var item in newList)
                        {
                            var GoToDo = new EvaluateTodoEntity();
                            GoToDo.Id = Guid.NewGuid().ToString();
                            GoToDo.userid = item.UserId;
                            GoToDo.module = module;
                            GoToDo.stepsid = moduleTodo.Id;
                            GoToDo.activityid = activityid;
                            GoToDo.activitydeptid = deptid;
                            todoList.Add(GoToDo);
                        }
                        db.Insert(todoList);
                        if (todoList.Count > 0)
                        {
                            break;
                        }
                    }
                }
                db.Commit();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void NextTodo(string module, string activityid)
        {
            try
            {
                IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
                var workTodo = db.IQueryable<EvaluateTodoEntity>().Where(x => x.activityid == activityid).ToList();
                if (workTodo.Count() > 0)
                {
                    IRepository dbQuery = new RepositoryFactory().BaseRepository();
                    db.Delete(workTodo);
                    var stpesId = workTodo.FirstOrDefault().stepsid;
                    var deptid = workTodo.FirstOrDefault().activitydeptid;
                    var moduleTodoList = dbQuery.IQueryable<EvaluateStepsEntity>().Where(x => x.module.Contains(module)).OrderBy(x => x.evaluatesort);

                    if (moduleTodoList.Count() > 0)
                    {
                        var List = moduleTodoList.ToList();
                        var now = List.FirstOrDefault(x => x.Id == stpesId);
                        var nowNum = List.IndexOf(now);
                        List.RemoveRange(0, nowNum + 1);
                        foreach (var moduleTodo in List)
                        {
                            var workDeptid = string.Empty;
                            var userlist = new List<UserEntity>();
                            //获取操作部门
                            if (!string.IsNullOrEmpty(moduleTodo.deptid))
                            {
                                workDeptid = moduleTodo.deptid;
                            }
                            else if (moduleTodo.isdept)
                            {
                                var dept = dbQuery.FindEntity<DepartmentEntity>(deptid);
                                var parent = dbQuery.FindEntity<DepartmentEntity>(dept.ParentId);
                                workDeptid = parent.DepartmentId;
                            }
                            else
                            {
                                workDeptid = deptid;
                            }
                            userlist = dbQuery.IQueryable<UserEntity>().Where(x => x.DepartmentId == workDeptid).ToList();

                            if (!string.IsNullOrEmpty(moduleTodo.userrole))
                            {
                                userlist = userlist.Where(x => x.RoleName.Contains(moduleTodo.userrole)).ToList();
                            }

                            if (moduleTodo.isprofessional)
                            {
                                var dept = dbQuery.FindEntity<DepartmentEntity>(workDeptid);
                                if (!string.IsNullOrEmpty(dept.SpecialtyType))
                                {
                                    userlist = userlist.Where(x => !string.IsNullOrEmpty(x.SpecialtyType)).ToList();
                                    userlist = userlist.Where(x => x.SpecialtyType.Contains(dept.SpecialtyType)).ToList();
                                }

                            }
                            var newList = new List<UserEntity>();

                            if (!string.IsNullOrEmpty(moduleTodo.userjobs))
                            {
                                userlist = userlist.Where(x => !string.IsNullOrEmpty(x.DutyName)).ToList();
                                var jobs = moduleTodo.userjobs.Split(',');
                                foreach (var item in jobs)
                                {
                                    var one = userlist.Where(x => x.DutyName.Contains(item)).ToList();
                                    newList.AddRange(one);
                                }
                            }
                            var todoList = new List<EvaluateTodoEntity>();
                            foreach (var item in newList)
                            {
                                var GoToDo = new EvaluateTodoEntity();
                                GoToDo.Id = Guid.NewGuid().ToString();
                                GoToDo.userid = item.UserId;
                                GoToDo.module = module;
                                GoToDo.stepsid = moduleTodo.Id;
                                GoToDo.activityid = activityid;
                                GoToDo.activitydeptid = deptid;
                                todoList.Add(GoToDo);
                            }
                            db.Insert(todoList);
                            if (todoList.Count > 0)
                            {
                                break;
                            }
                        }
                    }

                    db.Commit();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// 获取某班组当前月安全学习日活动的次数
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public int GetMonthCount(string deptId)
        {
            DateTime now = DateTime.Now;
            DateTime startTime = new DateTime(now.Year, now.Month, 1);
            DateTime endTime = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)).AddDays(1);
            int count = this.BaseRepository().IQueryable(p => p.GroupId == deptId && p.StartTime >= startTime && p.StartTime < endTime).Count();
            return count;
        }

        public int Count(string[] depts, string category, DateTime start, DateTime end)
        {
            var query = from q in _context.Set<EdActivityEntity>()
                        where depts.Contains(q.GroupId) && q.State == "Finish" && q.ActivityType == category && q.StartTime >= start && q.StartTime < end
                        select q;

            return query.Count();
        }



        #endregion
    }
}
