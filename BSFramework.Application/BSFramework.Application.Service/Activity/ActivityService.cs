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
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.EF;
using System.IO;
using System.Net.Http;
using System.ComponentModel.Design.Serialization;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;

namespace BSFramework.Service.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class ActivityService : RepositoryFactory<ActivityEntity>, ActivityIService
    {
        private System.Data.Entity.DbContext _context;

        public ActivityService()
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

            var query = from q1 in db.IQueryable<ActivityCategoryEntity>()
                        join q2 in db.IQueryable<ActivityEntity>() on q1.ActivityCategory equals q2.ActivityType into into1
                        where q1.DeptId == null || q1.DeptId.Contains(deptid)
                        select new { q1, into1 = into1.Where(x => x.GroupId == deptid && x.State != "Finish") };

            if (!string.IsNullOrEmpty(name)) query = from q in query
                                                     select new { q.q1, into1 = q.into1.Where(x => x.Subject.Contains(name)) };

            var query2 = from q in query
                         orderby q.q1.CreateTime
                         select new { q.q1.ActivityCategoryId, q.q1.ActivityCategory, q.q1.DeptId, Count = q.into1.Count() };

            //var query = this.BaseRepository().IQueryable().Where(x => x.GroupId == deptid && x.State != "Finish");
            //if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Subject.Contains(name));
            //return query.GroupBy(x => x.ActivityType).ToDictionary(x => x.Key, x => x.Count());

            var model = query2.ToList().Select(x => new ActivityCategoryEntity() { ActivityCategoryId = x.ActivityCategoryId, ActivityCategory = x.ActivityCategory, DeptId = x.DeptId, Total = x.Count }).ToList();
            //var user = OperatorProvider.Provider.Current();
            if (!string.IsNullOrEmpty(userid))
            {
                var safety = db.IQueryable<SafetydayReadEntity>().Where(x => x.Deptid == deptid && x.IsRead == 0 && x.Userid == userid);
                foreach (ActivityCategoryEntity ace in model.ToList())
                {
                    var state = db.FindEntity<ActivityEntity>(x => x.GroupId == deptid && x.ActivityType == ace.ActivityCategory && (x.State == "Study" || x.State == "Ready"));
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
                    if (ace.ActivityCategory == "安全日活动")
                    {
                        if (safety != null)
                        {
                            ace.Total = safety.Where(x => x.activitytype == "安全日活动").ToList().Count();
                        }
                        else
                        {
                            ace.Total = 0;
                        }
                    }
                    if (ace.ActivityCategory == "政治学习")
                    {
                        if (safety != null)
                        {
                            ace.Total = safety.Where(x => x.activitytype == "政治学习").ToList().Count();
                        }
                        else
                        {
                            ace.Total = 0;
                        }
                    }
                    if (ace.ActivityCategory == "上级精神宣贯")
                    {
                        if (safety != null)
                        {
                            ace.Total = safety.Where(x => x.activitytype == "上级精神宣贯").ToList().Count();
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
        public ActivityEntity GetEntity(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.FindEntity<ActivityEntity>(keyValue);
            if (model != null)
            {
                model.Files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == keyValue).ToList();
                model.ActivityPersons = db.IQueryable<ActivityPersonEntity>().OrderBy(x => x.Person).Where(x => x.ActivityId == keyValue).ToList();
            }
            return model;
        }

        public List<ActivityEvaluateEntity> GetEntityList()
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<ActivityEvaluateEntity>().ToList();
            return model;
        }
        public List<ActivitySupplyEntity> GetSupplyById()
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.IQueryable<ActivitySupplyEntity>().ToList();
            return model;
        }
        public ActivitySupplyEntity GetActivitySupplyEntity(string id)
        {
            var db = new RepositoryFactory().BaseRepository();

            var model = db.FindEntity<ActivitySupplyEntity>(id);
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

        public void SaveActivitySupply(string keyValue, ActivitySupplyEntity entity)
        {
            var db = new Repository<ActivitySupplyEntity>(DbFactory.Base());

            try
            {
                ActivitySupplyEntity entity1 = this.GetActivitySupplyEntity(keyValue);
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

        public void SaveActivityPerson(ActivityPersonEntity entity)
        {
            var db = new Repository<ActivityPersonEntity>(DbFactory.Base());
            db.Update(entity);

        }
        public int GetActivityList(string deptid, string from, string to)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<ActivityEntity>(x => x.GroupId == deptid && x.ActivityType == "安全日活动");
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

            //IRepository db = new RepositoryFactory().BaseRepository();

            //var sql = string.Format(" select count(*) from wg_activity where groupid = '{0}' and activitytype = '安全日活动' and starttime > '{1}' and starttime < '{2}'", deptid, from, to);
            ////            select a.departmentid,a.fullname,COUNT(b.activityid) as count 
            ////from base_department a 
            ////left join wg_activity b 
            ////on a.departmentid = b.groupid 
            ////and b.activitytype = '安全日活动' 
            ////and b.starttime > '2018-11-21' 
            ////where nature='班组' group by a.departmentid,a.fullname
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
        public List<ActivityEntity> GetList(int status, string bzDepart, DateTime? fromtime, DateTime? to)
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

            var queryOne = from a in db.IQueryable<ActivityEntity>()
                           join b in db.IQueryable<DepartmentEntity>() on a.GroupId equals b.DepartmentId into t1
                           from tb1 in t1.DefaultIfEmpty()
                               //join c in (from e in db.IQueryable<ActivityEvaluateEntity>()
                               //               //where e.EvaluateId == userid
                               //           group e by e.Activityid into t2
                               //           select new { Activityid = t2.Key, isEvaluate = t2.Count() }) on a.ActivityId equals c.Activityid
                               //           into t3
                               //from tb2 in t3.DefaultIfEmpty()
                               //join f in (from g in db.IQueryable<SubActivityEntity>()
                               //           group g by g.ActivityId into t4
                               //           select new { Activityid = t4.Key, dc = t4.Count() }
                               //           ) on a.ActivityId equals f.Activityid into t5
                               //from tb3 in t5.DefaultIfEmpty()
                           select new
                           {
                               activityid = a.ActivityId,
                               //PjCount = t3.Where(x => x.EvaluateId == userid).Count(),
                               activitytype = a.ActivityType,
                               subject = a.Subject,
                               planstarttime = a.PlanStartTime,
                               planendtime = a.PlanEndTime,
                               starttime = a.StartTime,
                               endtime = a.EndTime,
                               activityplace = a.ActivityPlace,
                               state = a.State,
                               fullname = tb1.FullName,
                               deptname = tb1.FullName,
                               groupid = a.GroupId,
                               encode = tb1.EnCode,
                               //number = t3.Count()

                           };

            //var queryOne = from a in db.IQueryable<ActivityEntity>()
            //               join b in db.IQueryable<DepartmentEntity>() on a.GroupId equals b.DepartmentId into t1
            //               from tb1 in t1.DefaultIfEmpty()
            //               join c in db.IQueryable<DepartmentEntity>() on tb1.ParentId equals c.DepartmentId into t2
            //               from tb2 in t2.DefaultIfEmpty()
            //               join d in db.IQueryable<ActivityEvaluateEntity>() on a.ActivityId equals d.Activityid into t3
            //               from tb3 in t3.DefaultIfEmpty()
            //               select new
            //               {
            //                   activityid = a.ActivityId,
            //                   PjCount = t3.Where(x => x.EvaluateId == userid).Count(),
            //                   activitytype = a.ActivityType,
            //                   subject = a.Subject,
            //                   planstarttime = a.PlanStartTime,
            //                   planendtime = a.PlanEndTime,
            //                   starttime = a.StartTime,
            //                   endtime = a.EndTime,
            //                   activityplace = a.ActivityPlace,
            //                   state = a.State,
            //                   fullname = tb1.FullName,
            //                   deptname = tb2.FullName,
            //                   groupid = a.GroupId,
            //                   encode = tb1.EnCode,
            //                   number = t3.Count()
            //               };

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
                var categories = new string[] { "上级精神宣贯", "安全日活动", "政治学习", "班务会", "民主管理会", "节能记录", "制度学习", "劳动保护监查" };
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
            //if (isEvaluate == "本人已评价")
            //{
            //    queryOne = queryOne.Where(x => x.PjCount > 0);
            //    //pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='{0}')>0 ", user.UserId);

            //}
            //else if (isEvaluate == "本人未评价")
            //{
            //    queryOne = queryOne.Where(x => x.PjCount == 0);
            //    // pagination.conditionJson += string.Format(" and (select count(1) from wg_activityevaluate g where c.id=g.activityid and g.evaluateid='{0}')=0 ", user.UserId);
            //}
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

            var queryData = from a in querytwo
                            join c in (from e in db.IQueryable<ActivityEvaluateEntity>()
                                           //where e.EvaluateId == userid
                                       group e by e.Activityid into t2
                                       select new
                                       {
                                           Activityid = t2.Key,
                                           isEvaluate = t2.Count()
                                           //,isMy=t2.Where
                                           //(g=>g.EvaluateId == userid).Count()
                                       }) on a.activityid equals c.Activityid into t3
                            from tb2 in t3.DefaultIfEmpty()
                            join f in (from g in db.IQueryable<ActivityEvaluateEntity>()
                                       where g.EvaluateId == userid
                                       group g by g.Activityid into t4
                                       select new { Activityid = t4.Key, isEvaluate = t4.Count() }) on a.activityid equals f.Activityid into t5
                            from tb3 in t5.DefaultIfEmpty()
                            select new
                            {
                                a.activityid,
                                PjCount = tb3 != null ? tb3.isEvaluate : 0,
                                a.activitytype,
                                a.subject,
                                a.planstarttime,
                                a.planendtime,
                                a.starttime,
                                a.endtime,
                                a.activityplace,
                                a.state,
                                a.fullname,
                                a.deptname,
                                a.groupid,
                                a.encode,
                                number = tb2 != null ? tb2.isEvaluate : 0
                            };



            var queryTalbe = DataHelper.ConvertToTable(queryData);
            return queryTalbe;

        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetActivityPageList(Pagination pagination, string queryJson, string select, string deptid, string category, string isEvaluate, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            #region  controller 查询


            var queryOne = from a in db.IQueryable<ActivityEntity>()
                           join b in db.IQueryable<DepartmentEntity>() on a.GroupId equals b.DepartmentId into t1
                           from tb1 in t1.DefaultIfEmpty()
                           join c in db.IQueryable<DepartmentEntity>() on tb1.ParentId equals c.DepartmentId into t2
                           from c in t2.DefaultIfEmpty()
                               //join c in (from e in db.IQueryable<ActivityEvaluateEntity>()
                               //               //where e.EvaluateId == userid
                               //           group e by e.Activityid into t2
                               //           select new { Activityid = t2.Key, isEvaluate = t2.Count() }) on a.ActivityId equals c.Activityid
                               //           into t3
                               //from tb2 in t3.DefaultIfEmpty()
                               //join f in (from g in db.IQueryable<SubActivityEntity>()
                               //           group g by g.ActivityId into t4
                               //           select new { Activityid = t4.Key, dc = t4.Count() }
                               //           ) on a.ActivityId equals f.Activityid into t5
                               //from tb3 in t5.DefaultIfEmpty()
                           select new
                           {
                               activityid = a.ActivityId,
                               starttime = a.StartTime,
                               endtime = a.EndTime,
                               activityplace = a.ActivityPlace,
                               state = a.State,
                               activitytype = a.ActivityType,
                               subject = a.Subject,
                               planstarttime = a.PlanStartTime,
                               planendtime = a.PlanEndTime,
                               fullname = tb1.FullName,
                               deptname = c.FullName,
                               ENCODE = tb1.EnCode,
                               //number = tb2!=null?0:1,
                               //hasSub = tb2 != null ? 0 : 1,
                           };





            if (!string.IsNullOrEmpty(select)) deptid = select;
            if (!string.IsNullOrEmpty(deptid))
            {
                var dept = new DepartmentService().GetEntity(deptid);
                if (dept == null) dept = new DepartmentService().GetRootDepartment();
                queryOne = queryOne.Where(x => x.ENCODE.StartsWith(dept.EnCode));

            }

            if (!string.IsNullOrEmpty(category))
            {
                var categories = new string[] { "上级精神宣贯", "安全日活动", "政治学习", "班务会", "民主管理会", "节能记录", "制度学习", "劳动保护监查" };
                if (categories.Contains(category))
                {
                    queryOne = queryOne.Where(x => x.activitytype == category);

                }
                else
                {

                    queryOne = queryOne.Where(x => categories.Contains(x.activitytype));
                }
            }
            //if (isEvaluate == "本人已评价")
            //{
            //    queryOne = queryOne.Where(x => x.number == 1);

            //}
            //else if (isEvaluate == "本人未评价")
            //{
            //    queryOne = queryOne.Where(x => x.number == 0);
            //}
            #endregion
            #region 视图查询

            var queryParam = queryJson.ToJObject();

            if (queryParam.SelectToken("activitytype") != null && queryParam.SelectToken("activitytype").Value<string>() != "全部")
            {
                var activitytype = queryParam.SelectToken("activitytype").Value<string>();
                queryOne = queryOne.Where(x => x.activitytype == activitytype);
            }
            if (queryParam.SelectToken("sdt") != null && !string.IsNullOrEmpty(queryParam.SelectToken("sdt").Value<string>()))
            {
                var sdt = Convert.ToDateTime(queryParam.SelectToken("sdt").Value<string>());
                queryOne = queryOne.Where(x => x.starttime >= sdt);
            }
            if (queryParam.SelectToken("edt") != null && !string.IsNullOrEmpty(queryParam.SelectToken("edt").Value<string>()))
            {
                var edt = Convert.ToDateTime(queryParam.SelectToken("edt").Value<string>() + " 23:59:59");
                queryOne = queryOne.Where(x => x.endtime <= edt);
            }

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString().Trim();
                queryOne = queryOne.Where(x => x.subject.Contains(keyword) || x.fullname.Contains(keyword) || x.activityplace.Contains(keyword));
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

            var queryData = from a in querytwo
                            join c in (from e in db.IQueryable<ActivityEvaluateEntity>()
                                           //where e.EvaluateId == userid
                                       group e by e.Activityid into t2
                                       select new { Activityid = t2.Key, isEvaluate = t2.Count() }) on a.activityid equals c.Activityid
                                       into t3
                            from tb2 in t3.DefaultIfEmpty()
                            join f in (from g in db.IQueryable<SubActivityEntity>()
                                       group g by g.ActivityId into t4
                                       select new { Activityid = t4.Key, dc = t4.Count() }
                                       ) on a.activityid equals f.Activityid into t5
                            from tb3 in t5.DefaultIfEmpty()
                            select new
                            {
                                a.activityid,
                                a.starttime,
                                a.endtime,
                                a.activityplace,
                                a.state,
                                a.activitytype,
                                a.subject,
                                a.planstarttime,
                                a.planendtime,
                                a.fullname,
                                a.deptname,
                                a.ENCODE,
                                number = tb2 != null ? tb2.isEvaluate : 0,
                                hasSub = tb3 != null ? 1 : 0,
                            };



            var queryTalbe = DataHelper.ConvertToTable(queryData);
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
            i = db.IQueryable<ActivityEntity>().Where(x => x.ActivityType == keyValue).ToList().Count;
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
        public void modfiyEntity(ActivityEntity entity)
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
        public void SaveForm(string keyValue, ActivityEntity entity)
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
        public void SaveFormSafetyday(string keyValue, ActivityEntity entity, List<SafetydayEntity> safetyday, string userId)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            //var user = OperatorProvider.Provider.Current();
            var user = db.FindEntity<PeopleEntity>(userId);
            var dept = db.FindEntity<DepartmentEntity>(user.BZID);
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
                            DeptCode = dept.EnCode,
                            SdId = "",
                            GroupId = user.BZID,
                            IsOrder = 0,
                            GroupName = dept.FullName
                        };
                        //order.Create();
                        order.Id = Guid.NewGuid().ToString();
                        order.CreateDate = DateTime.Now;
                        order.CreateUserId = userId;
                        order.CreateUserName = user.Name;
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
                    var resAct = new Repository<ActivityEntity>(DbFactory.Base());
                    if (entity.ActivityId == null)
                    {
                        entity.ActivityId = Guid.NewGuid().ToString();
                    }
                    //else
                    //{
                    //    entity.ActivityId = keyValue;
                    //}
                    entity.CreateDate = DateTime.Now;
                    resAct.Insert(entity);//批量插入班组活动

                    //resPerson.Delete(x => x.ActivityId == entity.ActivityId);
                    foreach (var item in entity.ActivityPersons)
                    {
                        item.ActivityId = entity.ActivityId;
                        if (string.IsNullOrEmpty(item.ActivityPersonId))
                        {
                            item.ActivityPersonId = Guid.NewGuid().ToString();
                            db.Insert(item);
                        }
                        else
                        {
                            var activityPerson = db.FindEntity<ActivityPersonEntity>(item.ActivityPersonId);
                            if (activity == null)
                                db.Insert(item);
                        }
                    }

                    if (entity.SubActivities != null)
                    {
                        var subject = entity.SubActivities.Find(x => x.ActivitySubject == "技术讲课");
                        if (subject != null)
                        {
                            var edu = new EduBaseInfoEntity
                            {
                                ID = Guid.NewGuid().ToString(),
                                CreateDate = DateTime.Now,
                                CreateUser = user.ID,
                                BZId = entity.GroupId,
                                ActivityDate = DateTime.Now,
                                ActivityLocation = entity.ActivityPlace,
                                Teacher = entity.ChairPerson,
                                RegisterPeople = entity.RecordPerson,
                                Remind = entity.ActivityLimited,
                                Theme = entity.Subject,
                                ActivityTime = entity.ActivityLimited,
                                Flow = "0",
                                EduType = "1",
                                BZName = entity.GroupName,
                                AttendPeople = string.Join(",", entity.ActivityPersons.Select(x => x.Person)),
                                AttendPeopleId = string.Join(",", entity.ActivityPersons.Select(x => x.PersonId)),
                                Category = "安全日活动"
                            };
                            db.Insert(edu);
                            subject.SubActivityId = edu.ID;
                        }

                        subject = entity.SubActivities.Find(x => x.ActivitySubject == "事故预想");
                        if (subject != null)
                        {
                            var edu = new EduBaseInfoEntity
                            {
                                ID = Guid.NewGuid().ToString(),
                                CreateDate = DateTime.Now,
                                CreateUser = user.ID,
                                BZId = entity.GroupId,
                                ActivityDate = DateTime.Now,
                                ActivityLocation = entity.ActivityPlace,
                                Teacher = entity.ChairPerson,
                                RegisterPeople = entity.RecordPerson,
                                Remind = entity.ActivityLimited,
                                Theme = entity.Subject,
                                ActivityTime = entity.ActivityLimited,
                                Flow = "0",
                                EduType = "3",
                                BZName = entity.GroupName,
                                AttendPeople = string.Join(",", entity.ActivityPersons.Select(x => x.Person)),
                                AttendPeopleId = string.Join(",", entity.ActivityPersons.Select(x => x.PersonId)),
                                Category = "安全日活动"
                            };
                            db.Insert(edu);
                            subject.SubActivityId = edu.ID;
                        }

                        subject = entity.SubActivities.Find(x => x.ActivitySubject == "反事故演习");
                        if (subject != null)
                        {
                            var edu = new EduBaseInfoEntity
                            {
                                ID = Guid.NewGuid().ToString(),
                                CreateDate = DateTime.Now,
                                CreateUser = user.ID,
                                BZId = entity.GroupId,
                                ActivityDate = DateTime.Now,
                                ActivityLocation = entity.ActivityPlace,
                                Teacher = entity.ChairPerson,
                                RegisterPeople = entity.RecordPerson,
                                Remind = entity.ActivityLimited,
                                Theme = entity.Subject,
                                ActivityTime = entity.ActivityLimited,
                                Flow = "0",
                                EduType = "4",
                                BZName = entity.GroupName,
                                AttendPeople = string.Join(",", entity.ActivityPersons.Select(x => x.Person)),
                                AttendPeopleId = string.Join(",", entity.ActivityPersons.Select(x => x.PersonId)),
                                Category = "安全日活动"
                            };
                            db.Insert(edu);
                            subject.SubActivityId = edu.ID;
                        }

                        subject = entity.SubActivities.Find(x => x.ActivitySubject == "工作总结安排");
                        if (subject != null)
                        {
                            var report = (from q in db.IQueryable<ReportEntity>()
                                          where q.ReportUserId == userId && q.ReportType == "0"
                                          orderby q.ReportTime descending
                                          select q).FirstOrDefault();
                            if (report != null) subject.SubActivityId = report.ReportId.ToString();
                        }

                        subject = entity.SubActivities.Find(x => x.ActivitySubject == "应急演练");
                        if (subject != null)
                        {
                            var obj = new
                            {
                                userid = userId,
                                data = new
                                {
                                    isconnectplan = "否",
                                    drilllevel = "班组",
                                    name = entity.Subject,
                                    drilltime = entity.StartTime,
                                    comperename = entity.ChairPerson,
                                    compere = entity.ChairPersonId,
                                    warntime = entity.AlertType,
                                    drillplace = entity.ActivityPlace,
                                    drillpeople = string.Join(",", entity.ActivityPersons.Select(x => x.PersonId)),
                                    drillpeoplename = string.Join(",", entity.ActivityPersons.Select(x => x.Person)),
                                    drillPeoplenumger = entity.ActivityPersons.Count(x => x.IsSigned == true)
                                }
                            };
                            var client = new HttpClient();
                            var dict = new Dictionary<string, string>();
                            dict.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(obj));

                            var response = client.PostAsync(Config.GetValue("ErchtmsApiUrl") + "EmergencyPlatform/SaveDrillRecordForRCHDStart", new FormUrlEncodedContent(dict)).Result;
                            var content = response.Content.ReadAsStringAsync().Result;
                            var jobject = Newtonsoft.Json.Linq.JObject.Parse(content);
                            subject.SubActivityId = jobject.Value<string>("data");
                        }

                        if (entity.SubActivities.Count > 0)
                        {
                            db.Insert(entity.SubActivities);
                        }
                    }

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
                        var activityPerson = db.FindEntity<ActivityPersonEntity>(item.ActivityPersonId);
                        if (activityPerson == null)
                            db.Insert(item);
                        else
                        {
                            activityPerson.IsSigned = item.IsSigned;
                            db.Update(activityPerson);
                        }
                    }

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
        public IEnumerable<ActivityEntity> GetList(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total)
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
        public IEnumerable<ActivityEntity> GetListApp(string userid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();
            var userList = db.IQueryable<ActivityPersonEntity>().Where(x => x.PersonId == userid).Select(x => x.ActivityId).ToList();
            var query = db.IQueryable<ActivityEntity>().Where(x => userList.Contains(x.ActivityId) && x.State == "Finish");
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
        public IEnumerable<ActivityEntity> GetAllList(DateTime from, string code)
        {
            var query = this.BaseRepository();
            var sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_activity a
left join wg_activityevaluate b on a.activityid = b.activityid
where a.starttime  > '{0}' and a.groupid in(select departmentid from base_department where encode like '{1}%' and nature = '班组')
group by a.activityid) a where total  =0", from, code);
            return query.FindList(sql);
        }
        public void Over(ActivityEntity model)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<ActivityEntity>(model.ActivityId);

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
        public ActivityEntity GetActivities(string category, string deptid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<ActivityEntity>()
                        where q.ActivityType == category && q.GroupId == deptid && q.State != "Finish"
                        orderby q.CreateDate
                        select q;

            var data = query.FirstOrDefault();

            if (data != null)
            {
                data.ActivityPersons = (from q in db.IQueryable<ActivityPersonEntity>()
                                        where q.ActivityId == data.ActivityId
                                        select q).ToList();

                data.Files = (from q in _context.Set<FileInfoEntity>().AsNoTracking()
                              where q.RecId == data.ActivityId
                              select q).OrderBy(x => x.CreateDate).ToList();

                data.SubActivities = (from q in db.IQueryable<SubActivityEntity>()
                                      where q.ActivityId == data.ActivityId
                                      orderby q.Seq
                                      select q).ToList();
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

        public void Start(ActivityEntity entity)
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

        public void Ready(ActivityEntity model)
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
                entity.OtherPersons = model.OtherPersons;
                entity.AlertType = model.AlertType;
                entity.AlertTime = model.AlertTime;
                entity.ActivityType = model.ActivityType;
                entity.State = model.State;
                entity.ActivityRecords = null;
                entity.ActivityPersons = null;

                var existspersons = db.IQueryable<ActivityPersonEntity>().Where(x => x.ActivityId == model.ActivityId).ToList();
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

        public void Edit(ActivityEntity model)
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
                entity.OtherPersons = model.OtherPersons;
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

                var existspersons = db.IQueryable<ActivityPersonEntity>().Where(x => x.ActivityId == model.ActivityId).ToList();
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

        public void Study(ActivityEntity model)
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
                entity.OtherPersons = model.OtherPersons;
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

        public ActivityEntity GetDetail(string id)
        {
            var activityset = _context.Set<ActivityEntity>();
            var personset = _context.Set<ActivityPersonEntity>();
            var supplyset = _context.Set<ActivitySupplyEntity>();
            var fileset = _context.Set<FileInfoEntity>();
            var subjectset = _context.Set<SubActivityEntity>();

            var entity = activityset.AsNoTracking().FirstOrDefault(x => x.ActivityId == id);
            if (entity != null)
            {
                entity.ActivityPersons = personset.AsNoTracking().Where(x => x.ActivityId == id).ToList();
                entity.Supplys = supplyset.AsNoTracking().Where(x => x.ActivityId == id).ToList();
                entity.Files = fileset.AsNoTracking().Where(x => x.RecId == id).ToList();
                entity.SubActivities = subjectset.AsNoTracking().Where(x => x.ActivityId == id).ToList();
            }

            return entity;
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
                                        join q2 in db.IQueryable<ActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
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
                                    join q2 in db.IQueryable<ActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
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
                var updateList = new List<ActivityEntity>();
                for (int i = 0; i < DeptId.Length; i++)
                {
                    var id = DeptId[i];
                    var activityquery = from q1 in db.IQueryable<ActivityCategoryEntity>()
                                        join q2 in db.IQueryable<ActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
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
                                    join q2 in db.IQueryable<ActivityEntity>() on q1.ActivityCategory equals q2.ActivityType
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
                var activityList = db.IQueryable<ActivityEntity>().Where(x => x.ActivityType == entity1.ActivityCategory && x.GroupId == entity.DeptId).ToList();
                entity1.ActivityCategory = entity.ActivityCategory;
                entity1.CreateTime = entity.CreateTime;
                entity1.CreateUserId = entity.CreateUserId;
                var addList = new List<ActivityEntity>();
                foreach (ActivityEntity ae in activityList)
                {
                    ae.ActivityType = entity.ActivityCategory;
                    ae.ActivityPersons = null;
                    ae.Files = null;
                }
                db.Update<ActivityEntity>(activityList);
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

        public List<ActivityEntity> GetActivities2(string userid, DateTime from, DateTime to, string category, string deptid, bool isall, int pageSize, int pageIndex, out int total, string notcategory = null)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var user = db.FindEntity<UserEntity>(userid);

            //if (isall)
            //{
            if (string.IsNullOrEmpty(deptid))
            {
                var query = from q1 in db.IQueryable<ActivityEntity>()
                            join q2 in db.IQueryable<SubActivityEntity>() on q1.ActivityId equals q2.ActivityId into into3
                            join q3 in db.IQueryable<FileInfoEntity>() on q1.ActivityId equals q3.RecId into into2
                            where q1.StartTime >= @from && q1.StartTime <= to && q1.State == "Finish"
                            orderby q1.StartTime descending
                            select new { q1.ActivityId, q1.Subject, q1.StartTime, q1.EndTime, q1.GroupId, q1.ActivityType, files = into2.Where(x => x.Description == "照片"), HasSub = into3.Count() > 0 };
                if (!string.IsNullOrEmpty(category))
                {
                    string[] categoryList = category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => categoryList.Contains(x.ActivityType));
                }
                if (!string.IsNullOrEmpty(notcategory))
                {
                    string[] notcategoryList = notcategory.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => !notcategoryList.Contains(x.ActivityType));
                }
                total = query.Count();
                var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                return data.Select(x => new ActivityEntity() { ActivityId = x.ActivityId, GroupId = x.GroupId, Subject = x.Subject, ActivityType = x.ActivityType, StartTime = x.StartTime, EndTime = x.EndTime, HasSub = x.HasSub, Files = x.files.ToList() }).ToList();
            }
            else
            {
                var query = from q1 in db.IQueryable<ActivityEntity>()
                            join q2 in db.IQueryable<SubActivityEntity>() on q1.ActivityId equals q2.ActivityId into into3
                            join q3 in db.IQueryable<FileInfoEntity>() on q1.ActivityId equals q3.RecId into into2
                            where q1.GroupId == deptid && q1.StartTime >= @from && q1.StartTime <= to && q1.State == "Finish"
                            orderby q1.StartTime descending
                            select new { q1.ActivityId, q1.Subject, q1.StartTime, q1.EndTime, q1.GroupId, HasSub = into3.Count() > 0, q1.ActivityType, files = into2.Where(x => x.Description == "照片") };
                if (!string.IsNullOrEmpty(category))
                {
                    string[] categoryList = category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => categoryList.Contains(x.ActivityType));
                }
                if (!string.IsNullOrEmpty(notcategory))
                {
                    string[] notcategoryList = notcategory.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(x => !notcategoryList.Contains(x.ActivityType));
                }
                total = query.Count();
                var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                return data.Select(x => new ActivityEntity() { ActivityId = x.ActivityId, GroupId = x.GroupId, Subject = x.Subject, ActivityType = x.ActivityType, StartTime = x.StartTime, EndTime = x.EndTime, HasSub = x.HasSub, Files = x.files.ToList() }).ToList();
            }
            //}
            //else
            //{
            //var query = from q1 in db.IQueryable<ActivityEntity>()
            //            join q2 in db.IQueryable<SubActivityEntity>() on q1.ActivityId equals q2.ActivityId into into3
            //            join q3 in db.IQueryable<FileInfoEntity>() on q1.ActivityId equals q3.RecId into into2
            //            where q1.GroupId == user.DepartmentId && q1.StartTime >= @from && q1.StartTime <= to && q1.State == "Finish"
            //            orderby q1.StartTime descending
            //            select new { q1.ActivityId, q1.Subject, q1.StartTime, q1.EndTime, q1.ActivityType, HasSub = into3.Count() > 0, files = into2.Where(x => x.Description == "照片") };
            //if (!string.IsNullOrEmpty(category))
            //{
            //    string[] categoryList = category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    query = query.Where(x => categoryList.Contains(x.ActivityType));
            //}
            //if (!string.IsNullOrEmpty(notcategory))
            //{
            //    string[] notcategoryList = notcategory.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    query = query.Where(x => !notcategoryList.Contains(x.ActivityType));
            //}
            //total = query.Count();
            //var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            //return data.Select(x => new ActivityEntity() { ActivityId = x.ActivityId, Subject = x.Subject, ActivityType = x.ActivityType, StartTime = x.StartTime, EndTime = x.EndTime, HasSub = x.HasSub, Files = x.files.ToList() }).ToList();
            //}

        }
        /// <summary>
        /// 获取班组活动
        /// </summary>
        ///<param name="pagination">分页公用类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id|State状态|Category分类|haveEvaluate是否查询评价|Depts部门id集合</param>
        /// <returns></returns>
        public List<ActivityEntity> GetAcJobCount(Pagination pagination, string queryJson)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<ActivityEntity>()
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
            // Depts部门id集合
            if (!queryParam["Depts"].IsEmpty())
            {
                string Depts = queryParam["Depts"].ToString();
                query = query.Where(p => Depts.Contains(p.GroupId));

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
            var data = query.OrderByDescending(x => x.StartTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();

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
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        public void SaveEvaluate(List<ActivityEvaluateEntity> entity)
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
                foreach (var item in entity)
                {
                    var toevaluate = (from q in db.IQueryable<ToEvaluateEntity>()
                                      where q.BusinessId == item.Activityid
                                      select q).FirstOrDefault();
                    if (toevaluate == null) continue;
                    else
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
                }
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        public List<ActivityEntity> GetActivityList(string deptid, string StrTime)
        {
            //var db = new RepositoryFactory().BaseRepository();
            //var query = (from q1 in db.IQueryable<ActivityEntity>()
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
            var query = (from q1 in db.IQueryable<ActivityEntity>().Where(x => x.State == "Finish")
                         join q2 in current on q1.GroupId equals q2.DepartmentId
                         select q1).ToList();
            var model = query.Where(x => x.EndTime.ToString("yyyy-MM") == DateTime.Parse(StrTime).ToString("yyyy-MM"));
            foreach (ActivityEntity entity in model)
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
                //var strSql = @"SELECT groupid,FULLNAME as GroupName,Endtime,
                //                SUM(case when activitytype= '安全日活动' then 1 else 0 end ) Safety,
                //                SUM(case when activitytype= '民主生活会' then 1 else 0 end ) Democratic,
                //                SUM(case when activitytype= '政治学习' then 1 else 0 end ) Politics,
                //                SUM(case when activitytype= '班务会' then 1 else 0 end ) team,
                //                SUM(case when activitytype!='安全日活动' and activitytype!='民主生活会'
                //                and activitytype!='政治学习' and activitytype!='班务会'  then 1 else 0 end ) Elseactivity
                //                from wg_activity a
                //                LEFT JOIN base_department b on b.DEPARTMENTID=a.groupid 
                //                WHERE endtime LIKE '%" + DateTime.Parse(date).ToString("yyyy-MM") + "%'GROUP BY groupid";
                //var query = (from q1 in db.FindList<StatisticsNumModel>(strSql.ToString())
                //             join q2 in current on q1.GroupId equals q2.DepartmentId
                //             select q1).ToList();

                var month = DateTime.Parse(date).Month;
                var year = DateTime.Parse(date).Year;
                var strSqlEntity = from item in (
                                   from a in db.IQueryable<ActivityEntity>()
                                   join b in db.IQueryable<DepartmentEntity>() on a.GroupId equals b.DepartmentId into t1
                                   from tb1 in t1.DefaultIfEmpty()
                                   where a.EndTime.Month == month && a.EndTime.Year == year
                                   select new { GroupId = a.GroupId, GroupName = tb1.FullName, a.ActivityType })
                                   group item by item.GroupId into tb2
                                   select new StatisticsNumModel()
                                   {
                                       GroupId = tb2.Key,
                                       GroupName = tb2.FirstOrDefault().GroupName,
                                       Safety = tb2.Where(x => x.ActivityType == "安全日活动").Count(),
                                       Democratic = tb2.Where(x => x.ActivityType == "民主生活会").Count(),
                                       Politics = tb2.Where(x => x.ActivityType == "政治学习").Count(),
                                       team = tb2.Where(x => x.ActivityType == "班务会").Count(),
                                       Elseactivity = tb2.Where(x => x.ActivityType != "班务会" && x.ActivityType != "政治学习"
                                       && x.ActivityType != "民主生活会"
                                       && x.ActivityType != "安全日活动").Count()
                                   };
                var query = (from q1 in strSqlEntity
                             join q2 in current on q1.GroupId equals q2.DepartmentId
                             select q1).ToList();



                //foreach (ActivityEntity entity in model)
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
        public void Update(string keyValue, ActivityEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataEntity = db.FindEntity<ActivityEntity>(keyValue);
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
        public void ManagerUpdate(string keyValue, ActivityEntity entity)
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

                        IList<ActivityPersonEntity> list = (from q in db.IQueryable<ActivityPersonEntity>() where q.ActivityId == keyValue select q).ToList();
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
            var query = from q in db.IQueryable<ActivityEntity>()
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
        /// <summary>
        /// 获取数据的所属模块
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public DataTable getIsModuleData(string strSql)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                var worktype = "班前班后会";
                var query = (from a in db.IQueryable<ActivityEntity>()
                             where a.ActivityId == strSql
                             select new
                             {
                                 edutype = a.ActivityType,
                                 state = a.State == "Finish" ? "1" : "0",
                                 createdate = a.CreateDate
                             })
                             .Concat(from a in db.IQueryable<EdActivityEntity>()
                                     where a.ActivityId == strSql
                                     select new
                                     {
                                         edutype = a.ActivityType,
                                         state = a.State == "Finish" ? "1" : "0",
                                         createdate = a.CreateDate
                                     })
                             .Concat(from a in db.IQueryable<EduBaseInfoEntity>()
                                     where a.ID == strSql
                                     select new
                                     {
                                         edutype = a.EduType,
                                         state = a.Flow == "1" ? "1" : "0",
                                         createdate = a.CreateDate
                                     })
                             .Concat(from a in db.IQueryable<WorkmeetingEntity>()
                                     where a.MeetingId == strSql
                                     select new
                                     {
                                         edutype = worktype,
                                         state = a.IsOver ? "1" : "0",
                                         createdate = a.MeetingStartTime
                                     });
                var queryTalbe = DataHelper.ConvertToTable(query);
                return queryTalbe;
                //return db.FindTable(strSql);

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

        #endregion


        #region 统计
        public DataTable GetGroupCount(string deptcode, DateTime f, DateTime t, string category)
        {
            string from = f.ToString("yyyy-MM-dd");
            string to = t.ToString("yyyy-MM-dd");

            //            string sql = string.Format(@" select a.activityid,a.starttime,a.endtime,b.ENCODE as deptcode,b.FULLNAME as deptname,a.groupid as deptid, 
            //(select count(activityid) from wg_activityperson where activityid=a.activityid) as sum FROM wg_activity a LEFT JOIN base_department b on a.groupid=b.DEPARTMENTID
            //where b.ENCODE like '{0}%' AND a.starttime>='{1}' and a.endtime<='{2}' and  a.state='Finish' and a.activitytype='{3}' ", deptcode, from, to, category);
            //            DataTable dt = this.BaseRepository().FindTable(sql);
            //            return dt;
            var db = new RepositoryFactory().BaseRepository();
            //var query = from a in db.IQueryable<ActivityEntity>()
            //            join b in db.IQueryable<DepartmentEntity>()
            //            on a.GroupId equals b.DepartmentId into t1
            //            from tb1 in t1.DefaultIfEmpty()
            //            join c in db.IQueryable<ActivityPersonEntity>()
            //            on a.ActivityId equals c.ActivityId into t2
            //            from tb2 in t2.DefaultIfEmpty()
            //            where tb1.EnCode.StartsWith(deptcode) && a.StartTime >= f
            //            && a.EndTime <= t && a.State == "Finish" && a.ActivityType == category
            //            select new
            //            {
            //                activityid = a.ActivityId,
            //                starttime = a.StartTime,
            //                endtime = a.EndTime,
            //                deptcode = tb1.EnCode,
            //                deptname = tb1.FullName,
            //                deptid = a.GroupId,
            //                sum = t2.Count(x => x.ActivityId == a.ActivityId)
            //            };

            var query =
                from a in db.IQueryable<ActivityEntity>()
                join b in db.IQueryable<DepartmentEntity>()
                on a.GroupId equals b.DepartmentId into t1
                from tb1 in t1.DefaultIfEmpty()
                join e in
                (
                from c in db.IQueryable<ActivityPersonEntity>()
                group c by c.ActivityId
                into t2
                select new
                {
                    Activityid = t2.Key,
                    sum = t2.Count()

                }
                ) on a.ActivityId equals e.Activityid into t3
                from tb3 in t3.DefaultIfEmpty()
                    //join c in db.IQueryable<ActivityPersonEntity>()
                    //on a.ActivityId equals c.ActivityId into t2
                    //from tb2 in t2.DefaultIfEmpty()
                where deptcode.Contains(a.GroupId) && a.StartTime >= f
                        && a.EndTime <= t && a.State == "Finish" && a.ActivityType == category
                select new
                {
                    activityid = a.ActivityId,
                    starttime = a.StartTime,
                    endtime = a.EndTime,
                    deptcode = tb1.EnCode,
                    deptname = tb1.FullName,
                    deptid = a.GroupId,
                    sum = tb3 == null ? 0 : tb3.sum
                };
            var data = DataHelper.ConvertToTable(query);
            return data;


        }

        public void EndActivity(ActivityEntity activity)
        {
            var activityset = _context.Set<ActivityEntity>();
            var entity = activityset.Find(activity.ActivityId);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.State = "Finish";
                entity.EndTime = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void EditActivity(ActivityEntity activity)
        {
            var activityset = _context.Set<ActivityEntity>();
            var entity = activityset.Find(activity.ActivityId);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.Leader = activity.Leader;
                entity.Remark = activity.Remark;
                _context.SaveChanges();
            }
        }

        public int Count(string[] depts, string category, DateTime start, DateTime end)
        {
            var query = from q in _context.Set<ActivityEntity>()
                        where depts.Contains(q.GroupId) && q.State == "Finish" && q.ActivityType == category && q.StartTime >= start && q.StartTime < end
                        select q;

            return query.Count();
        }


        /// <summary>
        /// 获取班组活动各类型的开展次数
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="to">结束时间</param>
        /// <returns></returns>
        public List<KeyValue> ActivityTypeCount(DateTime start, DateTime to, List<string> deptlist)
        {
            var query = from q in BaseRepository().IQueryable(p => p.StartTime >= start && p.StartTime < to && deptlist.Contains(p.GroupId))
                        group q.ActivityId by q.ActivityType into g
                        select new { g.Key, Count = g.Count() };
            var result = query.ToList();
            var data = result.Select(x => new KeyValue() { key = x.Key, value = x.Count.ToString() });
            return data.ToList();
        }
        #endregion

        #region web 活动页面配置
        /// <summary>
        /// 获取活动当前状态
        /// </summary>
        /// <param name="mianModule">选择的一级模块</param>
        /// <param name="userid"></param>
        /// <param name="deptid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ActivityCategoryEntity> GetMenuIndex(string mianModule, string userid, string deptid, Dictionary<string, string[]> type)
        {
            var db = new RepositoryFactory().BaseRepository();
            List<ActivityCategoryEntity> result = new List<ActivityCategoryEntity>();
            //循环获取类型
            foreach (var module in type)
            {
                ActivityCategoryEntity model = new ActivityCategoryEntity();

                //班组活动获取材料
                if (module.Key.Contains("activity"))
                {
                    model.ActivityCategory = module.Value[0];
                    model.ShowHtml = module.Key;
                    //班组活动展示进行
                    if (!string.IsNullOrEmpty(userid))
                    {
                        var safety = db.IQueryable<SafetydayReadEntity>().Where(x => x.Deptid == deptid && x.IsRead == 0 && x.Userid == userid);
                        var state = db.FindEntity<ActivityEntity>(x => x.GroupId == deptid && x.ActivityType == model.ActivityCategory && (x.State == "Study" || x.State == "Ready"));
                        if (state != null)
                        {

                            model.ActivityId = state.ActivityId;
                            if (state.State == "Ready")
                            {
                                if (!string.IsNullOrEmpty(state.Leader) && state.Leader.Length > 0)
                                {
                                    model.State = 3;
                                }
                                else
                                {
                                    model.State = 1;
                                }
                            }
                            else if (state.State == "Study")
                            {
                                model.State = 2;
                            }

                        }
                        model.Total = safety.Where(x => x.activitytype == model.ActivityCategory).ToList().Count();



                    }
                }
                //安全学习日 获取推送材料
                if (module.Key.Contains("EA"))
                {
                    model.ActivityCategory = module.Value[0];
                    model.ShowHtml = module.Key;
                    if (!string.IsNullOrEmpty(userid))
                    {
                        var safety = db.IQueryable<SafetydayReadEntity>().Where(x => x.Deptid == deptid && x.IsRead == 0 && x.Userid == userid);
                        var state = db.FindEntity<EdActivityEntity>(x => x.GroupId == deptid && x.ActivityType == model.ActivityCategory && (x.State == "Study" || x.State == "Ready"));
                        if (state != null)
                        {
                            model.ActivityId = state.ActivityId;
                            if (state.State == "Ready")
                            {
                                if (!string.IsNullOrEmpty(state.Leader) && state.Leader.Length > 0)
                                {
                                    model.State = 3;//已预约
                                }
                                else
                                {
                                    model.State = 1;//预约中
                                }
                            }
                            else if (state.State == "Study")
                            {
                                model.State = 2;//进行中
                            }

                        }

                        model.Total = safety.Where(x => x.activitytype == model.ActivityCategory).ToList().Count();
                    }
                }
                //教育培训
                if (module.Key.Contains("education"))
                {
                    model.ActivityCategory = module.Value[1];
                    model.ActivityCategoryId = module.Value[0];
                    model.ShowHtml = module.Key;
                    var state = db.FindEntity<EduBaseInfoEntity>(x => x.BZId == deptid && x.EduType == model.ActivityCategoryId && (x.Flow == "2" || x.Flow == "0"));
                    if (state != null)
                    {
                        model.ActivityId = state.ID;
                        if (state.Flow == "2")
                        {
                            model.State = 1;//预约中
                        }
                        else
                        {
                            model.State = 2;//进行中
                        }
                    }

                }
                result.Add(model);

            }
            //获取自定义分类
            if (mianModule == "activity")
            {
                //deptid为空是所有的
                var query = from q1 in db.IQueryable<ActivityCategoryEntity>()
                            where q1.DeptId == null || q1.DeptId.Contains(deptid)
                            orderby q1.CreateTime descending
                            select q1;
                foreach (var item in query.ToList())
                {
                    item.ShowHtml = "ACS";
                    var state = db.FindEntity<ActivityEntity>(x => x.GroupId == deptid && x.ActivityType == item.ActivityCategory && (x.State == "Study" || x.State == "Ready"));
                    if (state != null)
                    {
                        item.ActivityId = state.ActivityId;
                    }
                }

                result.AddRange(query);
            }


            return result;

        }

        public List<ActivityStatiticsEntity> Statistics(string[] depts, string[] categories, DateTime from, DateTime to)
        {
            var query = (from q in _context.Set<ActivityEntity>()
                         where depts.Contains(q.GroupId) && q.StartTime >= @from && q.EndTime < to && q.State == "Finish"
                         group q by q.ActivityType into g
                         select new { g.Key, Count = g.Count() }).Concat(
                from q in _context.Set<EduBaseInfoEntity>()
                where depts.Contains(q.BZId) && q.ActivityDate >= @from && q.ActivityDate < to && q.Flow == "1"
                group q by q.EduType into g
                select new { g.Key, Count = g.Count() }).Concat(
                from q in _context.Set<EdActivityEntity>()
                where depts.Contains(q.GroupId) && q.StartTime >= @from && q.EndTime < to && q.State == "Finish"
                group q by q.ActivityType into g
                select new { g.Key, Count = g.Count() });

            var data = query.ToList();
            var list = data.Select(x => new ActivityStatiticsEntity { Category = x.Key, Count = x.Count }).ToList();
            return list;
        }
        #endregion
    }
}
