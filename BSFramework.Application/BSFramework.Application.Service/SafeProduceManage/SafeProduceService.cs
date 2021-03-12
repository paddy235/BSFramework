using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.SafeProduceManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.SafeProduceManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SafeProduceManage
{
    /// <summary>
    /// 安全文明生成检查
    /// </summary>
    public class SafeProduceService : RepositoryFactory<SafeProduceEntity>, ISafeProduceService
    {

        #region 获取数据



        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public SafeProduceEntity getSafeProduceDataById(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.FindEntity<SafeProduceEntity>(keyvalue);
            return data;
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SafeProduceEntity> GetPageSafeProduceList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<SafeProduceEntity>();
            var queryParam = queryJson.ToJObject();
            //部门和人员都存在时为或者

            if (!queryParam["DutyDeptId"].IsEmpty())
            {
                var DutyDeptId = queryParam["DutyDeptId"].ToString();
                query = query.Where(x => x.DutyDeptId == DutyDeptId);
            }

            if (!queryParam["userid"].IsEmpty())
            {
                var userId = queryParam["userid"].ToString();
                query = query.Where(x => x.CreateUserId == userId);
            }

            //处理结果
            if (!queryParam["state"].IsEmpty())
            {
                var State = queryParam["state"].ToString();
                query = query.Where(x => x.State == State);
            }

            //描述和措施和责任部门
            if (!queryParam["keyWord"].IsEmpty())
            {
                var KeyWord = queryParam["keyWord"].ToString();
                query = query.Where(x => x.Describe.Contains(KeyWord) || x.Measure.Contains(KeyWord) || x.DutyDeptName.Contains(KeyWord));
            }
            //区域id
            if (!queryParam["districtId"].IsEmpty())
            {
                var DistrictId = queryParam["districtId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            //区域id
            if (!queryParam["DistrictId"].IsEmpty())
            {
                var DistrictId = queryParam["DistrictId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            //创建时间
            if (!queryParam["CreateDate"].IsEmpty())
            {
                var Start = DateTime.Parse(queryParam["CreateDate"].ToString());
                var end = Start.AddDays(1);

                query = query.Where(x => x.CreateDate >= Start && x.CreateDate < end);
            }
            if (!queryParam["StartDate"].IsEmpty())
            {
                var Start = DateTime.Parse(queryParam["StartDate"].ToString());

                query = query.Where(x => x.CreateDate >= Start);
            }
            if (!queryParam["EndDate"].IsEmpty())
            {
                var end = DateTime.Parse(queryParam["EndDate"].ToString()).AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.CreateDate <= end);
            }
            if (!queryParam["TimeType"].IsEmpty())
            {
                var TimeType = queryParam["TimeType"].ToString();
                var NowTime = DateTime.Now;
                var UseNowTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day);
                DateTime Start, End;
                switch (TimeType)
                {
                    //今天
                    case "1":
                        Start = UseNowTime;
                        End = UseNowTime.AddDays(1).AddMilliseconds(-1);
                        query = query.Where(x => x.CreateDate >= Start && x.CreateDate <= End);
                        break;
                    //本周
                    case "2":
                        var dayNum = Convert.ToInt32(UseNowTime.DayOfWeek);
                        dayNum = dayNum == 0 ? 7 : dayNum;
                        Start = UseNowTime.AddDays(-(dayNum - 1));
                        End = UseNowTime.AddDays(8 - dayNum).AddMilliseconds(-1);
                        query = query.Where(x => x.CreateDate >= Start && x.CreateDate <= End);
                        break;
                    //本月
                    case "3":
                        var monSum = Time.GetDaysOfMonth(UseNowTime.Year, UseNowTime.Month);
                        Start = new DateTime(UseNowTime.Year, UseNowTime.Month, 1);
                        End = new DateTime(UseNowTime.Year, UseNowTime.Month, monSum).AddDays(1).AddMilliseconds(-1);
                        query = query.Where(x => x.CreateDate >= Start && x.CreateDate <= End);
                        break;
                    default:
                        break;
                }
            }
            if (!queryParam["DutyType"].IsEmpty())
            {
                var DutyType = queryParam["DutyType"].ToString();
                query = query.Where(x => x.DutyType == DutyType);
            }

            if (!queryParam["DistrictCode"].IsEmpty())
            {
                var DistrictCode = queryParam["DistrictCode"].ToString();
                query = query.Where(x => x.DistrictCode.StartsWith(DistrictCode));
            }
            if (!queryParam["DutyTypeId"].IsEmpty())
            {
                var DutyTypeId = queryParam["DutyTypeId"].ToString();
                query = query.Where(x => x.DutyTypeId == DutyTypeId);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(x => x.State).ThenByDescending(x => x.CreateDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return query.ToList();
        }

        /// <summary>
        /// 签到统计数据
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public object GetPageSafeProduceAndSigninList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var queryParam = queryJson.ToJObject();

            var query = from a in db.IQueryable<OnLocaleEntity>()
                        join b in
                        (from c in db.IQueryable<SafeProduceEntity>().Where(x => x.CreateDate.HasValue)
                         group c by new { c.DutyTypeId, c.CreateUserId } into d
                         select d)
                        on new { UserId = a.UserId, DutyTypeId = a.DutyTypeId } equals new { UserId = b.Key.CreateUserId, DutyTypeId = b.Key.DutyTypeId } into t1
                        from e in t1.DefaultIfEmpty()
                        where a.Module == "安全文明生产"
                        select new
                        {
                            a.District,
                            a.DistrictId,
                            a.DistrictCode,
                            a.DeptName,
                            a.UserName,
                            a.SigninDate,
                            a.DutyTypeId,
                            a.DutyType,
                            num = e.Count()
                        };
            if (!queryParam["DistrictId"].IsEmpty())
            {
                var DistrictId = queryParam["DistrictId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            if (!queryParam["DistrictCode"].IsEmpty())
            {
                var DistrictCode = queryParam["DistrictCode"].ToString();
                query = query.Where(x => x.DistrictCode.StartsWith(DistrictCode));
            }
            if (!queryParam["keyWord"].IsEmpty())
            {
                var KeyWork = queryParam["keyWord"].ToString();
                query = query.Where(x => x.UserName.Contains(KeyWork) || x.DeptName.Contains(KeyWork));
            }
            if (!queryParam["DutyType"].IsEmpty())
            {
                var DutyType = queryParam["DutyType"].ToString();
                query = query.Where(x => x.DutyType == DutyType);
            }
            if (!queryParam["DutyTypeId"].IsEmpty())
            {
                var DutyTypeId = queryParam["DutyTypeId"].ToString();
                query = query.Where(x => x.DutyTypeId == DutyTypeId);
            }
            if (!queryParam["StartDate"].IsEmpty())
            {
                var Start = DateTime.Parse(queryParam["StartDate"].ToString());

                query = query.Where(x => x.SigninDate >= Start);
            }
            if (!queryParam["EndDate"].IsEmpty())
            {
                var end = DateTime.Parse(queryParam["EndDate"].ToString()).AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.SigninDate <= end);
            }
            pagination.records = query.Count();
            var queryData = query.OrderByDescending(x => x.SigninDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            return queryData.ToList();
        }
        /// <summary>
        /// 查询考勤签到
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<OnLocaleEntity> GetPageOnLocaleList(Pagination pagination, string queryJson)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var query = from a in db.IQueryable<OnLocaleEntity>()
                        select a;

            if (!queryParam["DistrictId"].IsEmpty())
            {
                var DistrictId = queryParam["DistrictId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            if (!queryParam["DistrictCode"].IsEmpty())
            {
                var DistrictCode = queryParam["DistrictCode"].ToString();
                query = query.Where(x => x.DistrictCode.StartsWith(DistrictCode));
            }
            if (!queryParam["TimeType"].IsEmpty())
            {
                var TimeType = queryParam["TimeType"].ToString();
                var NowTime = DateTime.Now;
                var UseNowTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day);
                DateTime Start, End;
                switch (TimeType)
                {
                    //今天
                    case "1":
                        Start = UseNowTime;
                        End = UseNowTime.AddDays(1).AddMilliseconds(-1);
                        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                        break;
                    //本周
                    case "2":
                        var dayNum = Convert.ToInt32(UseNowTime.DayOfWeek);
                        dayNum = dayNum == 0 ? 7 : dayNum;
                        Start = UseNowTime.AddDays(-(dayNum - 1));
                        End = UseNowTime.AddDays(8 - dayNum).AddMilliseconds(-1);
                        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                        break;
                    //本月
                    case "3":
                        var monSum = Time.GetDaysOfMonth(UseNowTime.Year, UseNowTime.Month);
                        Start = new DateTime(UseNowTime.Year, UseNowTime.Month, 1);
                        End = new DateTime(UseNowTime.Year, UseNowTime.Month, monSum).AddDays(1).AddMilliseconds(-1);
                        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                        break;
                    default:
                        break;
                }
            }
            pagination.records = query.Count();
            var queryData = query.OrderByDescending(x => x.SigninDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            var result = from a in queryData
                         join b in db.IQueryable<PeopleEntity>()
                         on a.UserId equals b.ID
                         select new OnLocaleEntity()
                         {
                             Id = a.Id,
                             DistrictId = a.DistrictId,
                             District = a.District,
                             DistrictCode = a.DistrictCode,
                             UserId = a.UserId,
                             UserName = a.UserName,
                             SigninDate = a.SigninDate,
                             DeptId = a.DeptId,
                             DeptName = a.DeptName,
                             DeptCode = a.DeptCode,
                             Module = a.Module,
                             DutyType = a.DutyType,
                             DutyTypeId = a.DutyTypeId,
                             photo = b.Photo
                         };
            return result.ToList();
        }

        /// <summary>
        /// 查询考勤缺勤
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<OnLocaleEntity> GetPageNoOnLocaleList(Pagination pagination, string queryJson)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var query = from a in db.IQueryable<OnLocaleEntity>()
                        select a;

            if (!queryParam["DistrictId"].IsEmpty())
            {
                var DistrictId = queryParam["DistrictId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            if (!queryParam["DistrictCode"].IsEmpty())
            {
                var DistrictCode = queryParam["DistrictCode"].ToString();
                query = query.Where(x => x.DistrictCode.StartsWith(DistrictCode));
            }
            if (!queryParam["TimeType"].IsEmpty())
            {
                var TimeType = queryParam["TimeType"].ToString();
                var NowTime = DateTime.Now;
                var UseNowTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day);
                DateTime Start, End;
                //switch (TimeType)
                //{
                //    //今天
                //    case "1":
                //        Start = UseNowTime.AddDays(-1);
                //        End = UseNowTime.AddMilliseconds(-1);
                //        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                //        break;
                //    //本周
                //    case "2":
                //        var dayNum = Convert.ToInt32(UseNowTime.DayOfWeek);
                //        dayNum = dayNum == 0 ? 7 : dayNum;
                //        Start = UseNowTime.AddDays(dayNum - 1).AddDays(7);
                //        End = Start.AddMilliseconds(-1);
                //        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                //        break;
                //    //本月
                //    case "3":
                //        var last = UseNowTime.AddMonths(-1);
                //        var monSum = Time.GetDaysOfMonth(last.Year, last.Month);
                //        Start = new DateTime(last.Year, last.Month, 1);
                //        End = new DateTime(last.Year, last.Month, monSum).AddDays(1).AddMilliseconds(-1);
                //        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                //        break;
                //    default:
                //        break;
                //}
                switch (TimeType)
                {
                    //今天
                    case "1":
                        Start = UseNowTime;
                        End = UseNowTime.AddDays(1).AddMilliseconds(-1);
                        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                        break;
                    //本周
                    case "2":
                        var dayNum = Convert.ToInt32(UseNowTime.DayOfWeek);
                        dayNum = dayNum == 0 ? 7 : dayNum;
                        Start = UseNowTime.AddDays(-(dayNum - 1));
                        End = UseNowTime.AddDays(8 - dayNum).AddMilliseconds(-1);
                        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                        break;
                    //本月
                    case "3":
                        var monSum = Time.GetDaysOfMonth(UseNowTime.Year, UseNowTime.Month);
                        Start = new DateTime(UseNowTime.Year, UseNowTime.Month, 1);
                        End = new DateTime(UseNowTime.Year, UseNowTime.Month, monSum).AddDays(1).AddMilliseconds(-1);
                        query = query.Where(x => x.SigninDate >= Start && x.SigninDate <= End);
                        break;
                    default:
                        break;
                }
            }
            pagination.records = query.Count();
            var queryData = query.OrderByDescending(x => x.SigninDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            return queryData;
        }

        /// <summary>
        /// 查询考勤缺勤
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<OnLocaleEntity> GetPageNoOnLocaleAllList(string DistrictId)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<OnLocaleEntity>()
                        where a.DistrictId == DistrictId
                        select a;


            return query.ToList();
        }
        #endregion
        #region 数据操作

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateSafeProduce(SafeProduceEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {

                if (!string.IsNullOrEmpty(entity.Id))
                {
                    var dataEntity = db.FindEntity<SafeProduceEntity>(entity.Id);
                    if (dataEntity != null)
                    {
                        dataEntity.ModifyUserId = entity.ModifyUserId;
                        dataEntity.ModifyUserName = entity.ModifyUserName;
                        dataEntity.ModifyDate = entity.ModifyDate;
                        dataEntity.Measure = entity.Measure;
                        dataEntity.Situation = entity.Situation;
                        dataEntity.State = entity.State;
                        dataEntity.Describe = entity.Describe;
                        dataEntity.DutyDeptCode = entity.DutyDeptCode;
                        dataEntity.DutyDeptId = entity.DutyDeptId;
                        dataEntity.DutyDeptName = entity.DutyDeptName;
                        db.Update(dataEntity);

                    }
                    else
                    {
                        db.Insert(entity);
                    }
                }
                else
                {
                    entity.Id = Guid.NewGuid().ToString();
                    db.Insert(entity);
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
        /// 保存修改现场终端功能踩点记录
        /// </summary>
        /// <param name="entity"></param>
        public void operateOnLocale(OnLocaleEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var ck = db.IQueryable<OnLocaleEntity>(x => x.SigninDate.Year == entity.SigninDate.Year && x.SigninDate.Month == entity.SigninDate.Month && x.SigninDate.Day == entity.SigninDate.Day && x.UserId == entity.UserId && x.DutyTypeId == entity.DutyTypeId && x.DistrictId == entity.DistrictId);
                if (ck.Count() == 0)
                {
                    if (!string.IsNullOrEmpty(entity.Id))
                    {
                        var dataEntity = db.FindEntity<OnLocaleEntity>(entity.Id);
                        if (dataEntity != null)
                        {
                            db.Update(entity);

                        }
                        else
                        {
                            db.Insert(entity);
                        }
                    }
                    else
                    {
                        entity.Id = Guid.NewGuid().ToString();
                        db.Insert(entity);
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
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        public void removeSafeProduce(string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataEntity = db.FindEntity<SafeProduceEntity>(keyvalue);
                if (dataEntity != null)
                {
                    db.Delete(dataEntity);
                }
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        #endregion
    }
}
