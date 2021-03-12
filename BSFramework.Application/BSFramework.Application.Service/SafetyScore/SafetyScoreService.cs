using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Application.IService.SafetyScore;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.Log;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SafetyScore
{
    public class SafetyScoreService : RepositoryFactory<SafetyScoreEntity>, ISafetyScoreService
    {


        /// <summary>
        /// 根据内置的规则类型 ，添加积分数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dataType">根据内置的规则类型 ，添加积分数据   规则类型 
        /// 1 登记一般隐患（未整改） 
        /// 2 登记一般隐患（已整改）
        /// 3 登记重大隐患（未整改）    
        /// 4 登记违章
        /// 5 早安中铝
        /// 6 班前一题
        /// 7 班前一课
        /// </param>
        public void AddScore(string userId, int dataType)
        {
            WriteLog.AddLog($"Service AddScore 接收到参数userId:{userId}  dataType:{dataType}", "SafetyScore");
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                var user = (from q1 in db.IQueryable<UserEntity>()
                            join q2 in db.IQueryable<DepartmentEntity>()
                            on q1.DepartmentId equals q2.DepartmentId into t
                            from t1 in t.DefaultIfEmpty()
                            where q1.UserId == userId
                            select new { q1.UserId, q1.RealName, t1.DepartmentId, DepartmentCode = t1.EnCode, DeptName = t1.FullName, q1.Gender }).FirstOrDefault();

                if (user == null) throw new Exception("用户不存在");
                SafetyScoreEntity entity = new SafetyScoreEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    CreateUserId = user.UserId,
                    CreateUserName = user.RealName,
                    UserId = user.UserId,
                    UserName = user.RealName,
                    DeptId = user.DepartmentId,
                    DeptCode = user.DepartmentCode,
                    DeptName = user.DeptName,
                    Gender = user.Gender,
                    ScoreType = Enum.GetName(typeof(ScoreType), ScoreType.自动),
                    ModifyDate = DateTime.Now,
                    ModifyUserId = user.UserId,
                    ModifyUserName = user.RealName,
                };
                AccountRuleEntity rule;
                switch (dataType)
                {
                    case 1:
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "登记一般隐患（未整改）" && p.IsOpen == 1).FirstOrDefault();
                        break;
                    case 2:
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "登记一般隐患（已整改）" && p.IsOpen == 1).FirstOrDefault();
                        break;
                    case 3:
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "登记重大隐患（未整改）" && p.IsOpen == 1).FirstOrDefault();
                        break;
                    case 4:
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "登记违章" && p.IsOpen == 1).FirstOrDefault();
                        break;
                    case 5:
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "早安中铝" && p.IsOpen == 1).FirstOrDefault();
                        break;
                    case 6:
                        //每个班组每月不得超过60分
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "班前一题" && p.IsOpen == 1).FirstOrDefault();
                        if (rule == null) throw new Exception("内置规则不存在，或未启用");
                        var a = db.IQueryable<SafetyScoreEntity>(x => x.DeptId == user.DepartmentId && x.RuleId == rule.Id && x.CreateDate.Month == DateTime.Now.Month).Select(x => x.Score).ToList();
                        //查询用户所在的部门的总得分
                        var deptTotal = a.Sum(x => x);
                        if (deptTotal >= 60) return;
                        if ((deptTotal + rule.Score) > 60)
                        {
                            rule.Score = 60 - deptTotal;
                        }
                        break;
                    case 7:
                        rule = db.IQueryable<AccountRuleEntity>(p => p.Standard == "班前一课" && p.IsOpen == 1).FirstOrDefault();
                        if (rule == null) throw new Exception("内置规则不存在，或未启用");
                        var b = db.IQueryable<SafetyScoreEntity>(x => x.DeptId == user.DepartmentId && x.RuleId == rule.Id && x.CreateDate.Month == DateTime.Now.Month).Select(x => x.Score).ToList();
                        //查询用户所在的部门的总得分
                        var SubjectdeptTotal = b.Sum(x => x);
                        if (SubjectdeptTotal >= 60) return;
                        if ((SubjectdeptTotal + rule.Score) > 60)
                        {
                            rule.Score = 60 - SubjectdeptTotal;
                        }
                        break;
                    default:
                        throw new ArgumentException("规则类型不对");
                }
                if (rule == null) throw new Exception("内置规则不存在，或未启用");
                entity.Reasons = rule.Standard;
                entity.RuleId = rule.Id;
                entity.Score = rule.Score;
                db.Insert(entity);
                WriteLog.AddLog($"Service AddScore 添加安全积分成功:{JsonConvert.SerializeObject(entity)}", "SafetyScore");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"Service AddScore 错误:{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                throw;
            }

        }

        /// <summary>
        /// 查找单个实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public SafetyScoreEntity GetEntity(string keyValue)
        {
            return BaseRepository().FindEntity(keyValue);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public IEnumerable<SafetyScoreEntity> GetPagedList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyWord"].IsEmpty())
            {
                string keyword = queryParam["keyWord"].ToString();
                query = query.Where(p => p.UserName.Contains(keyword) || p.Reasons.Contains(keyword) || p.Area.Contains(keyword));
            }
            if (!queryParam["startDate"].IsEmpty())
            {
                DateTime startDate;
                if (DateTime.TryParse(queryParam["startDate"].ToString(), out startDate))
                    query = query.Where(p => p.CreateDate >= startDate);
            }
            if (!queryParam["endDate"].IsEmpty())
            {
                DateTime endDate;
                if (DateTime.TryParse(queryParam["endDate"].ToString(), out endDate))
                {
                    endDate = endDate.AddDays(1).Date;
                    query = query.Where(p => p.CreateDate < endDate);
                }
            }
            //if (!queryParam["deptCode"].IsEmpty())
            //{
            //    string deptcode = queryParam["deptCode"].ToString();
            //    query = query.Where(p => p.DeptCode.Contains(deptcode));
            //}
            if (!queryParam["deptId"].IsEmpty())
            {
                string deptId = queryParam["deptId"].ToString();
                var deptIds = (new DepartmentService().GetSubDepartments(new string[] { deptId })).Select(x => x.DepartmentId);

                query = query.Where(p => deptIds.Contains(p.DeptId));
            }
            if (!queryParam["userId"].IsEmpty())
            {
                string userId = queryParam["userId"].ToString();
                query = query.Where(p => p.UserId == userId);
            }
            int total = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query, p => p.CreateDate, out total, false);
            pagination.records = total;
            return data;
        }

        /// <summary>
        /// 用户各年份每月的积分信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<int, List<KeyValue>> GetUserScoreInfo(string userId)
        {

            var monthList = BaseRepository().IQueryable(p => p.UserId == userId).Select(x => new { x.Score, x.CreateDate, Month = x.CreateDate.Month, Year = x.CreateDate.Year }).ToList();
            var monthData = monthList.GroupBy(p => new { p.Month, p.Year }).Select(p => new { p.Key.Month, TotalScore = p.Sum(x => x.Score), p.Key.Year }).ToList();//各个月的数据
            var years = monthData.Select(p => p.Year).Distinct().OrderByDescending(x => x).ToList();//所有年份
            Dictionary<int, List<KeyValue>> dic = new Dictionary<int, List<KeyValue>>();
            if (years != null && years.Count > 0)
            {
                years.ForEach(year =>
                {
                    List<KeyValue> kv = new KeyValue().InitData();
                    kv.ForEach(m =>
                    {
                        var score = monthData.FirstOrDefault(p => p.Year == year && p.Month == (int)Enum.Parse(typeof(MothName), m.Key, true))?.TotalScore ?? 0;
                        m.Value = score;
                    });
                    dic.Add(year, kv);
                });
            }
            return dic;
        }

        /// <summary>
        /// 个人得分数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public object GetUserScorePagedList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();


            //查用户
            var userQuery = db.IQueryable<UserEntity>();
            if (!queryParam["deptCode"].IsEmpty())
            {
                string deptCode = queryParam["deptCode"].ToString();
                userQuery = userQuery.Where(p => p.DepartmentCode.Contains(deptCode));
            }
            if (!queryParam["deptId"].IsEmpty())
            {
                if (!queryParam["deptId"].IsEmpty())
                {
                    string deptId = queryParam["deptId"].ToString();
                    var deptIds = (new DepartmentService().GetSubDepartments(new string[] { deptId })).Select(x => x.DepartmentId);

                    userQuery = userQuery.Where(p => deptIds.Contains(p.DepartmentId));
                }
            }
            if (!queryParam["Gender"].IsEmpty())
            {
                int gender;
                if (int.TryParse(queryParam["Gender"].ToString(), out gender))
                {
                    userQuery = userQuery.Where(p => p.Gender == gender);
                }
            }
            if (!queryParam["keyWord"].IsEmpty())
            {
                string keyWord = queryParam["keyWord"].ToString();
                userQuery = userQuery.Where(p => p.RealName.Contains(keyWord) || p.DutyName.Contains(keyWord));
            }
            if (!queryParam["userId"].IsEmpty())
            {
                string userId = queryParam["userId"].ToString();
                userQuery = userQuery.Where(p => p.UserId == userId);
            }


            //查分数
            DateTime searchdate = DateTime.Now;
            if (!queryParam["serachDate"].IsEmpty())
            {
                string searchDateStr = queryParam["serachDate"].ToString();
                if (DateTime.TryParse(searchDateStr, out searchdate)) { }
            }
            #region 取用户数据
            var dataQuery = from q1 in userQuery
                            join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                            select new { q1.UserId, q1.Gender, q1.DutyName, q1.EnCode, UserName = q1.RealName, DeptName = q2.FullName };
            var total = dataQuery.Count();
            var userList = dataQuery.OrderBy(p => p.EnCode).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            #endregion
            var userIds = userList.Select(x => x.UserId).ToList();
            #region 月度积分
            DateTime monthStart = new DateTime(searchdate.Year, searchdate.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1);
            var monthList = db.IQueryable<SafetyScoreEntity>(t => userIds.Contains(t.UserId) && t.CreateDate >= monthStart && t.CreateDate < monthEnd)
                .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, MonthScore = p.Sum(x => x.Score) }).ToList();

            #endregion
            #region 季度积分
            DateTime quarterStart = monthStart;
            DateTime quarterEnd = monthEnd;
            switch (searchdate.Month)
            {
                case 1:
                case 2:
                case 3:
                    quarterStart = new DateTime(searchdate.Year, 1, 1);
                    quarterEnd = new DateTime(searchdate.Year, 4, 1);
                    break;
                case 4:
                case 5:
                case 6:
                    quarterStart = new DateTime(searchdate.Year, 4, 1);
                    quarterEnd = new DateTime(searchdate.Year, 7, 1);
                    break;
                case 7:
                case 8:
                case 9:
                    quarterStart = new DateTime(searchdate.Year, 7, 1);
                    quarterEnd = new DateTime(searchdate.Year, 10, 1);
                    break;
                case 10:
                case 11:
                case 12:
                    quarterStart = new DateTime(searchdate.Year, 10, 1);
                    quarterEnd = new DateTime(searchdate.Year + 1, 1, 1);
                    break;
            }
            var quarterList = db.IQueryable<SafetyScoreEntity>(t => userIds.Contains(t.UserId) && t.CreateDate >= quarterStart && t.CreateDate < quarterEnd)
                .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, QuarterScore = p.Sum(x => x.Score) }).ToList();
            #endregion
            #region 年度积分
            var yearScoreList = db.IQueryable<SafetyScoreEntity>(t => userIds.Contains(t.UserId) && t.CreateDate.Year == searchdate.Year)
                .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, YearScore = p.Sum(x => x.Score) }).ToList();
            #endregion

            pagination.records = total;

            var data = userList.Select(p =>
            {
                var monthScore = monthList.FirstOrDefault(x => x.UserId == p.UserId)?.MonthScore ?? 0;
                var quarterScore = quarterList.FirstOrDefault(x => x.UserId == p.UserId)?.QuarterScore ?? 0;
                var yearScore = yearScoreList.FirstOrDefault(x => x.UserId == p.UserId)?.YearScore ?? 0;
                return new
                {
                    p.UserId,
                    p.UserName,
                    p.DutyName,
                    Gender = p.Gender.HasValue ? (p.Gender.Value == 1 ? "男" : "女") : "",
                    p.DeptName,
                    p.EnCode,
                    MonthScore = monthScore,
                    QuarterScore = quarterScore,
                    YearScore = yearScore
                };
            });

            return data;
        }

        /// <summary>
        /// 个人得分数据统计 （查全厂全年的数据做统计）
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="searchdate">要查询的时间 yyyy-MM-dd 具体时间不重要，年月最重要</param>
        /// <param name="where">安全积分查询条件</param>
        /// <param name="userWhere">用户查询条件</param>
        /// <returns></returns>
        public object GetUserScorePagedList(Pagination pagination,DateTime searchdate,Expression<Func<UserEntity,bool>>userWhere)
        {
            #region 月度积分
            DateTime monthStart = new DateTime(searchdate.Year, searchdate.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1);
            #endregion
            #region 季度积分
            DateTime quarterStart = monthStart;
            DateTime quarterEnd = monthEnd;
            switch (searchdate.Month)
            {
                case 1:
                case 2:
                case 3:
                    quarterStart = new DateTime(searchdate.Year, 1, 1);
                    quarterEnd = new DateTime(searchdate.Year, 4, 1);
                    break;
                case 4:
                case 5:
                case 6:
                    quarterStart = new DateTime(searchdate.Year, 4, 1);
                    quarterEnd = new DateTime(searchdate.Year, 7, 1);
                    break;
                case 7:
                case 8:
                case 9:
                    quarterStart = new DateTime(searchdate.Year, 7, 1);
                    quarterEnd = new DateTime(searchdate.Year, 10, 1);
                    break;
                case 10:
                case 11:
                case 12:
                    quarterStart = new DateTime(searchdate.Year, 10, 1);
                    quarterEnd = new DateTime(searchdate.Year + 1, 1, 1);
                    break;
            }
            #endregion
            #region 年度积分
            DateTime yearStart = new DateTime(searchdate.Year, 1, 1);
            DateTime yearEnd = yearStart.AddYears(1);
            #endregion

            var db = new RepositoryFactory().BaseRepository();
            //var query1= from q1 in db.IQueryable<UserEntity>()
            //            join q2 in db.IQueryable<SafetyScoreEntity>() on q1.UserId equals q2.UserId into t
            //            from t1 in t.DefaultIfEmpty()
            //            group t1 by new { q1.UserId,q1.Gender,q1.DutyName,q1.EnCode,q1.RealName,q1.DepartmentName } into g
            //            select new {
            //                 g.Key,
            //                MonthScore=g.Where(x=>x.CreateDate>=monthStart && x.CreateDate<monthEnd).Sum(x=>x.Score),
            //                QuarterScore = g.Where(x => x.CreateDate >= quarterStart && x.CreateDate < quarterStart).Sum(x => x.Score),
            //                YearScore = g.Where(x => x.CreateDate >= yearStart && x.CreateDate < yearStart).Sum(x => x.Score),
            //                g.Key.UserId,
            //                g.Key.Gender,
            //                g.Key.DutyName,
            //                g.Key.EnCode,
            //                UserName = g.Key.RealName,
            //                DeptName = g.Key.DepartmentName
            //            };








            #region 废弃代码


            var userQuery = db.IQueryable<UserEntity>();
            if (userWhere != null) userQuery = userQuery.Where(userWhere);

            var dataQuery =from x in userQuery
                           join q2 in db.IQueryable<DepartmentEntity>() on x.DepartmentId equals q2.DepartmentId
                           select  new { x.UserId, x.Gender, x.DutyName, x.EnCode, x.RealName, DeptName =q2.FullName };
            var total = dataQuery.Count();
            var userList = dataQuery.ToList();
            //var userList = dataQuery.OrderBy(p => p.EnCode).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            var userIds = userList.Select(x => x.UserId).ToList();
            //查询全厂 所查询年的全部数据

            var ScoreYearData = db.IQueryable<SafetyScoreEntity>(x => x.CreateDate.Year == searchdate.Year && userIds.Contains(x.UserId)).Select(x => new { x.UserId, x.Score, x.CreateDate }).ToList();
            #region 月度积分
  
            var monthList = ScoreYearData.Where(t => t.CreateDate >= monthStart && t.CreateDate < monthEnd)
                .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, MonthScore = p.Sum(x => x.Score) }).ToList();

            #endregion
            #region 季度积分

            var quarterList = ScoreYearData.Where(t => t.CreateDate >= quarterStart && t.CreateDate < quarterEnd)
                .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, QuarterScore = p.Sum(x => x.Score) }).ToList();
            #endregion
            #region 年度积分
            var yearScoreList = ScoreYearData.Where(t => t.CreateDate.Year == searchdate.Year)
                .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, YearScore = p.Sum(x => x.Score) }).ToList();
            #endregion
            #endregion

            var data = userList.Select(p =>
            {
                var monthScore = monthList.FirstOrDefault(x => x.UserId == p.UserId)?.MonthScore ?? 0;
                var quarterScore = quarterList.FirstOrDefault(x => x.UserId == p.UserId)?.QuarterScore ?? 0;
                var yearScore = yearScoreList.FirstOrDefault(x => x.UserId == p.UserId)?.YearScore ?? 0;
                return new
                {
                    p.UserId,
                    UserName = p.RealName,
                    p.DutyName,
                    Gender = p.Gender.HasValue ? (p.Gender.Value == 1 ? "男" : "女") : "",
                    p.DeptName,
                    p.EnCode,
                    MonthScore = monthScore,
                    QuarterScore = quarterScore,
                    YearScore = yearScore
                };
            });
            pagination.records = total;
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                if (pagination.sidx == "MonthScore")
                {
                    if (pagination.sord=="asc")
                    {
                        data = data.OrderBy(x => x.MonthScore);
                    }
                    else
                    {
                        data = data.OrderByDescending(x => x.MonthScore);
                    }
                }
                if (pagination.sidx == "QuarterScore")
                {
                    if (pagination.sord == "asc")
                    {
                        data = data.OrderBy(x => x.QuarterScore);
                    }
                    else
                    {
                        data = data.OrderByDescending(x => x.QuarterScore);
                    }
                }
                if (pagination.sidx == "YearScore")
                {
                    if (pagination.sord == "asc")
                    {
                        data = data.OrderBy(x => x.YearScore);
                    }
                    else
                    {
                        data = data.OrderByDescending(x => x.YearScore);
                    }
                }
            }
            else
            {
                data = data.OrderByDescending(x => x.MonthScore);
            }
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList(); ;
        }

        /// <summary>
        /// 个人得分数据前三名
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public object GetUserScorefirstthree(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();


            //查用户
            var userQuery = db.IQueryable<UserEntity>();
            if (!queryParam["deptCode"].IsEmpty())
            {
                string deptCode = queryParam["deptCode"].ToString();
                userQuery = userQuery.Where(p => p.DepartmentCode.Contains(deptCode));
            }
            if (!queryParam["Gender"].IsEmpty())
            {
                int gender;
                if (int.TryParse(queryParam["Gender"].ToString(), out gender))
                {
                    userQuery = userQuery.Where(p => p.Gender == gender);
                }
            }
            if (!queryParam["keyWord"].IsEmpty())
            {
                string keyWord = queryParam["keyWord"].ToString();
                userQuery = userQuery.Where(p => p.RealName.Contains(keyWord) || p.DutyName.Contains(keyWord));
            }
            if (!queryParam["userId"].IsEmpty())
            {
                string userId = queryParam["userId"].ToString();
                userQuery = userQuery.Where(p => p.UserId == userId);
            }
            var searchdate = DateTime.Now;
            //月度  季度  年度
            var Type = string.Empty;
            if (!queryParam["Type"].IsEmpty())
            {
                Type = queryParam["Type"].ToString();
            }

            DateTime Start = DateTime.Now;
            DateTime End = DateTime.Now;
            #region 取用户数据
            var dataQuery = from q1 in userQuery
                            join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                            join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID
                            select new { q1.UserId, q1.Gender, q1.DutyName, q1.EnCode, UserName = q1.RealName, DeptName = q2.FullName,DeptId=q2.DepartmentId,q3.Photo };
            var total = dataQuery.Count();
            var userList = dataQuery.OrderBy(p => p.EnCode).ToList();
            #endregion
            var userIds = userList.Select(x => x.UserId).ToList();

            switch (Type)
            {
                case "1":
                    #region 季度积分

                    switch (searchdate.Month)
                    {
                        case 1:
                        case 2:
                        case 3:
                            Start = new DateTime(searchdate.Year, 1, 1);
                            End = new DateTime(searchdate.Year, 4, 1);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            Start = new DateTime(searchdate.Year, 4, 1);
                            End = new DateTime(searchdate.Year, 7, 1);
                            break;
                        case 7:
                        case 8:
                        case 9:
                            Start = new DateTime(searchdate.Year, 7, 1);
                            End = new DateTime(searchdate.Year, 10, 1);
                            break;
                        case 10:
                        case 11:
                        case 12:
                            Start = new DateTime(searchdate.Year, 10, 1);
                            End = new DateTime(searchdate.Year + 1, 1, 1);
                            break;
                    }
                    #endregion
                    break;

                case "2":
                    #region 年度积分
                    Start = new DateTime(searchdate.Year, 1, 1);
                    End = Start.AddYears(1).AddMilliseconds(-1);
                    #endregion
                    break;

                default:
                    #region 月度积分
                    Start = new DateTime(searchdate.Year, searchdate.Month, 1);
                    End = Start.AddMonths(1);

                    #endregion
                    break;
            }

            var ScoreList = db.IQueryable<SafetyScoreEntity>(t => userIds.Contains(t.UserId) && t.CreateDate >= Start && t.CreateDate < End)
        .GroupBy(x => x.UserId).Select(p => new { UserId = p.Key, Score = p.Sum(x => x.Score) }).ToList();
            pagination.records = 3;

            var data = userList.Select(p =>
            {
                var userScore = ScoreList.FirstOrDefault(x => x.UserId == p.UserId)?.Score ?? 0;
               
                return new
                {
                    p.UserId,
                    p.UserName,
                    p.DutyName,
                    Gender = p.Gender.HasValue ? (p.Gender.Value == 1 ? "男" : "女") : "",
                    p.DeptName,
                    p.EnCode,
                    p.DeptId,
                    p.Photo,
                    score = userScore
                };
            });
            data = data.OrderByDescending(x => x.score).Take(3);
            return data;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void Remove(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            db.Delete<SafetyScoreEntity>(keyValue);
            //删除附件
            var fileInfos = db.IQueryable<FileInfoEntity>().Where(p => p.RecId == keyValue).ToList();
            if (fileInfos != null && fileInfos.Count > 0)
            {
                Task.Run(() =>
                {
                    try
                    {
                        db.Delete(fileInfos);
                        fileInfos.ForEach(fileList =>
                        {
                            string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                            if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                            {
                                System.IO.File.Delete(url);
                            }
                        });
                    }
                    catch (Exception)
                    {

                    }
                });
            }

        }

        /// <summary>
        /// 编辑数据
        /// 当UserId为多个时，会生成多条数据。
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void SaveForm(string keyValue, SafetyScoreEntity entity)
        {
            if (entity == null) throw new Exception("传入的实体为空");
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            List<string> userIds = entity.UserId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var userInfoQuery = from q1 in db.IQueryable<UserEntity>()
                                join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                                where userIds.Contains(q1.UserId)
                                select new { q1.UserId, q1.RealName, q2.DepartmentId, q2.FullName, q2.EnCode, q1.Gender };
            var userInfo = userInfoQuery.ToList();
            if (userInfo == null || userInfo.Count == 0) throw new Exception("找不到对应的用户");
            List<SafetyScoreEntity> dataList = new List<SafetyScoreEntity>();
            try
            {
                if (string.IsNullOrWhiteSpace(entity.Id))
                {
                    //找到附件
                    var fileInfos = db.IQueryable<FileInfoEntity>().Where(p => p.RecId == keyValue).ToList(); ;
                    //新增
                    userInfo.ForEach(user =>
                    {
                        SafetyScoreEntity insert = DataHelper.InputToOutput<SafetyScoreEntity, SafetyScoreEntity>(entity);
                        insert.Create();
                        insert.UserId = user.UserId;
                        insert.UserName = user.RealName;
                        insert.Gender = user.Gender;
                        insert.DeptId = user.DepartmentId;
                        insert.DeptCode = user.EnCode;
                        insert.DeptName = user.FullName;
                        insert.ScoreType = Enum.GetName(typeof(ScoreType), 1);
                        dataList.Add(insert);

                        if (fileInfos != null && fileInfos.Count() > 0)
                        {
                            List<FileInfoEntity> newFiles = new List<FileInfoEntity>();
                            fileInfos.ForEach(file =>
                            {
                                FileInfoEntity insertFile = DataHelper.InputToOutput<FileInfoEntity, FileInfoEntity>(file);
                                insertFile.FileId = Guid.NewGuid().ToString();
                                insertFile.RecId = insert.Id;
                                newFiles.Add(insertFile);
                            });
                            //insert附件并删除之前的附件
                            db.Insert(newFiles);
                            db.Delete(fileInfos);
                        }
                    });
                    db.Insert(dataList);
                }
                else
                {

                    //修改  存在这样的数据：单个数据变多个用户的情况
                    //1、先找到之前的用户的数据
                    SafetyScoreEntity oldEntity = db.FindEntity<SafetyScoreEntity>(p => p.Id == keyValue);
                    if (oldEntity == null) throw new Exception("找不到要修改的数据");
                    //找到附件
                    var fileInfos = db.IQueryable<FileInfoEntity>().Where(p => p.RecId == keyValue).ToList(); ;
                    //新增
                    userInfo.ForEach(user =>
                    {
                        SafetyScoreEntity insert = DataHelper.InputToOutput<SafetyScoreEntity, SafetyScoreEntity>(entity);
                        insert.Create();
                        insert.UserId = user.UserId;
                        insert.UserName = user.RealName;
                        insert.Gender = user.Gender;
                        insert.DeptId = user.DepartmentId;
                        insert.DeptCode = user.EnCode;
                        insert.DeptName = user.FullName;
                        insert.ScoreType = Enum.GetName(typeof(ScoreType), 1);
                        dataList.Add(insert);

                        if (fileInfos != null && fileInfos.Count() > 0)
                        {
                            List<FileInfoEntity> newFiles = new List<FileInfoEntity>();
                            fileInfos.ForEach(file =>
                            {
                                FileInfoEntity insertFile = DataHelper.InputToOutput<FileInfoEntity, FileInfoEntity>(file);
                                insertFile.FileId = Guid.NewGuid().ToString();
                                insertFile.RecId = insert.Id;
                                newFiles.Add(insertFile);
                            });
                            //insert附件并删除之前的附件
                            db.Insert(newFiles);
                            db.Delete(fileInfos);
                        }
                    });
                    db.Insert(dataList);
                    //删除之前的数据，修改相当于重新新增 
                    db.Delete<SafetyScoreEntity>(keyValue);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }


        }

    }
}
