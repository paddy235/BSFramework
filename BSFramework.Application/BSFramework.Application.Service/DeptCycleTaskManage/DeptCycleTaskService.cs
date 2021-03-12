using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.DeptCycleTaskManage;
using BSFramework.Application.IService.DeptCycleTaskManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.DeptCycleTaskManage
{
    /// <summary>
    /// 部门任务库
    /// </summary>
    public class DeptCycleTaskService : RepositoryFactory<DeptCycleTaskEntity>, IDeptCycleTaskService
    {
        #region 查询数据
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<DeptCycleTaskEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var Expression = LinqExtensions.True<DeptCycleTaskEntity>();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId == "System")
            {
                user.DepartmentCode = "0";
            }
            Expression = Expression.And(x => x.deptcode.StartsWith(user.DepartmentCode));
            if (!queryParam["content"].IsEmpty())
            {
                var content = queryParam["content"].ToString();
                Expression = Expression.And(x => x.content.Contains(content));
            }
            if (!queryParam["dutyuserid"].IsEmpty())
            {
                var dutyuserid = queryParam["dutyuserid"].ToString();
                Expression = Expression.And(x => x.dutyuserid == dutyuserid);
            }
            var entity = db.FindList(Expression).OrderByDescending(x => x.modifytime);
            if (pagination.sidx == "cycleDataStr")
            {
                if (pagination.sord == "desc")
                {
                    entity = entity.OrderByDescending(x => x.cycle == "每年").ThenByDescending(x => x.cycle == "每月").ThenByDescending(x => x.cycle == "每周").ThenByDescending(x => x.cycle == "每天");
                }
                else
                {
                    entity = entity.OrderBy(x => x.cycle == "每年").ThenBy(x => x.cycle == "每月").ThenBy(x => x.cycle == "每周").ThenBy(x => x.cycle == "每天");
                    //entity = entity.OrderByDescending(x => x.cycle == "每年").ThenByDescending(x => x.cycle == "每月").ThenByDescending(x => x.cycle == "每周").ThenByDescending(x => x.cycle == "每天").Reverse();
                }
            }
            if (pagination.sidx == "dutyuser")
            {
                if (pagination.sord == "desc")
                {
                    entity = entity.OrderByDescending(x => x.dutyuser);
                }
                else
                {
                    entity = entity.OrderBy(x => x.dutyuser);
                }
            }
            pagination.records = entity.Count();
            var Resultentity = entity.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            return Resultentity;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public DeptCycleTaskEntity getEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<DeptCycleTaskEntity>(keyvalue);
            return entity;
        }

        #endregion

        #region 操作数据

        /// <summary>
        ///批量  新增 修改
        /// </summary>
        /// <param name="Listentity"></param>
        /// <param name="userid"></param>
        public void SaveForm(List<DeptCycleTaskEntity> Listentity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);
                List<DeptCycleTaskEntity> go = new List<DeptCycleTaskEntity>();
                var dayTime = DateTime.Now.Date;
                foreach (var entity in Listentity)

                {
                    if (entity.id.IsEmpty())
                    {
                        entity.id = Guid.NewGuid().ToString();
                        entity.deptcode = user.DepartmentCode;
                        entity.deptid = user.DepartmentId;
                        entity.deptname = user.DepartmentName;
                        entity.modifytime = DateTime.Now;
                        entity.modifyuser = user.RealName;
                        entity.modifyuserid = user.UserId;
                        entity.createtime = DateTime.Now;
                        entity.createuser = user.RealName;
                        entity.createuserid = user.UserId;
                        db.Insert(entity);
                        go.Add(entity);
                    }
                    else
                    {
                        var oldentity = db.FindEntity<DeptCycleTaskEntity>(entity.id);
                        if (oldentity != null)
                        {
                            oldentity.cycle = entity.cycle;
                            oldentity.cycledate = string.IsNullOrEmpty(entity.cycledate) ? string.Empty : entity.cycledate;
                            oldentity.content = entity.content;
                            oldentity.isend = entity.isend;
                            oldentity.islastday = entity.islastday;
                            oldentity.isweek = entity.isweek;
                            oldentity.modifytime = DateTime.Now;
                            oldentity.modifyuser = user.RealName;
                            oldentity.modifyuserid = user.UserId;
                            db.Update(oldentity);

                        }
                    }
                }
                var workCycle = GetDateJob(go, dayTime);
                foreach (var item in workCycle)
                {
                    var workTask = new DeptWorkCycleTaskEntity();
                    workTask.id = Guid.NewGuid().ToString();
                    workTask.content = item.content;
                    workTask.starttime = item.starttime.HasValue ? item.starttime.Value : dayTime;
                    workTask.endtime = item.endtime.HasValue ? item.endtime.Value : dayTime.AddDays(1).AddMinutes(-1);
                    workTask.cycle = item.cycle;
                    workTask.cycledate = item.cycledate;
                    workTask.isend = item.isend;
                    workTask.islastday = item.islastday;
                    workTask.isweek = item.isweek;
                    workTask.workstate = "进行中";
                    workTask.dutyuser = item.dutyuser;
                    workTask.dutyuserid = item.dutyuserid;
                    workTask.deptid = item.deptid;
                    workTask.deptcode = item.deptcode;
                    workTask.deptname = item.deptname;
                    workTask.cycletaskid = item.id;
                    workTask.modifytime = DateTime.Now;
                    workTask.modifyuser = user != null ? user.RealName : "";
                    workTask.modifyuserid = user != null ? user.UserId : null;
                    db.Insert(workTask);
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(DeptCycleTaskEntity entity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);
                List<DeptCycleTaskEntity> go = new List<DeptCycleTaskEntity>();
                var dayTime = DateTime.Now.Date;
                if (entity.id.IsEmpty())
                {
                    entity.id = Guid.NewGuid().ToString();
                    entity.deptcode = user.DepartmentCode;
                    entity.deptid = user.DepartmentId;
                    entity.deptname = user.DepartmentName;
                    entity.modifytime = DateTime.Now;
                    entity.modifyuser = user.RealName;
                    entity.modifyuserid = user.UserId;
                    entity.createtime = DateTime.Now;
                    entity.createuser = user.RealName;
                    entity.createuserid = user.UserId;
                    db.Insert(entity);
                    go.Add(entity);

                    var workCycle = GetDateJob(go, dayTime);
                    foreach (var item in workCycle)
                    {
                        var workTask = new DeptWorkCycleTaskEntity();
                        workTask.id = Guid.NewGuid().ToString();
                        workTask.content = item.content;
                        workTask.starttime = item.starttime.HasValue ? item.starttime.Value : dayTime;
                        workTask.endtime = item.endtime.HasValue ? item.endtime.Value : dayTime.AddDays(1).AddMinutes(-1);
                        workTask.cycle = item.cycle;
                        workTask.cycledate = item.cycledate;
                        workTask.isend = item.isend;
                        workTask.islastday = item.islastday;
                        workTask.isweek = item.isweek;
                        workTask.workstate = "进行中";
                        workTask.dutyuser = item.dutyuser;
                        workTask.dutyuserid = item.dutyuserid;
                        workTask.deptid = item.deptid;
                        workTask.deptcode = item.deptcode;
                        workTask.deptname = item.deptname;
                        workTask.cycletaskid = item.id;
                        workTask.modifytime = DateTime.Now;
                        workTask.modifyuser = user != null ? user.RealName : "";
                        workTask.modifyuserid = user != null ? user.UserId : null;
                        db.Insert(workTask);
                    }
                }
                else
                {
                    var oldentity = db.FindEntity<DeptCycleTaskEntity>(entity.id);
                    if (oldentity != null)
                    {
                        oldentity.cycle = entity.cycle;
                        oldentity.cycledate = string.IsNullOrEmpty(entity.cycledate) ? string.Empty : entity.cycledate;
                        oldentity.content = entity.content;
                        oldentity.isend = entity.isend;
                        oldentity.islastday = entity.islastday;
                        oldentity.dutyuser = entity.dutyuser;
                        oldentity.dutyuserid = entity.dutyuserid;
                        oldentity.isweek = entity.isweek;
                        oldentity.modifytime = DateTime.Now;
                        oldentity.modifyuser = user.RealName;
                        oldentity.modifyuserid = user.UserId;
                        db.Update(oldentity);

                    }
                    //go.Add(oldentity);
                    //var workCycle = GetDateJob(go, dayTime);
                    //foreach (var item in workCycle)
                    //{
                    //    var workTask = new DeptWorkCycleTaskEntity();
                    //    workTask.id = Guid.NewGuid().ToString();
                    //    workTask.content = item.content;
                    //    workTask.starttime = item.starttime.HasValue ? item.starttime.Value : dayTime;
                    //    workTask.endtime = item.endtime.HasValue ? item.endtime.Value : dayTime.AddDays(1).AddMinutes(-1);
                    //    workTask.cycle = item.cycle;
                    //    workTask.cycledate = item.cycledate;
                    //    workTask.isend = item.isend;
                    //    workTask.islastday = item.islastday;
                    //    workTask.isweek = item.isweek;
                    //    workTask.workstate = "进行中";
                    //    workTask.dutyuser = item.dutyuser;
                    //    workTask.dutyuserid = item.dutyuserid;
                    //    workTask.deptid = item.deptid;
                    //    workTask.deptcode = item.deptcode;
                    //    workTask.deptname = item.deptname;
                    //    workTask.cycletaskid = item.id;
                    //    workTask.modifytime = DateTime.Now;
                    //    workTask.modifyuser = user != null ? user.RealName : "";
                    //    workTask.modifyuserid = user != null ? user.UserId : null;
                    //    db.Insert(workTask);
                    //}
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<DeptCycleTaskEntity>(keyvalue);
                if (entity != null)
                {
                    db.Delete(entity);
                }
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        #endregion

        #region 部门定时任务推送
        /// <summary>
        /// 推送任务
        /// </summary>
        /// <param name="date"></param>
        /// <param name="deptid">可为空</param>
        /// <param name="userid">可为空</param>
        public void GetDpetTask(DateTime date, string deptid, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var WorkCycleTask = new List<DeptWorkCycleTaskEntity>();
                var dayTime = Convert.ToDateTime(date.ToDateString());
                var CycleTask = db.FindList<DeptCycleTaskEntity>(x => !string.IsNullOrEmpty(x.deptid));
                if (!string.IsNullOrEmpty(deptid))
                {
                    CycleTask = CycleTask.Where(x => x.deptid == deptid);
                }
                UserEntity user = new UserEntity();
                if (!string.IsNullOrEmpty(userid))
                {
                    user = db.FindEntity<UserEntity>(x => x.UserId == userid);
                }
                var workCycle = GetDateJob(CycleTask.ToList(), date);
                foreach (var item in workCycle)
                {
                    var workTask = new DeptWorkCycleTaskEntity();
                    workTask.id = Guid.NewGuid().ToString();
                    workTask.content = item.content;
                    workTask.starttime = item.starttime.HasValue ? item.starttime.Value : dayTime;
                    workTask.endtime = item.endtime.HasValue ? item.endtime.Value : dayTime.AddDays(1).AddMinutes(-1);
                    //if (item.isend)
                    //{
                    //    var ck = db.FindList<DeptWorkCycleTaskEntity>(x => x.isend && x.content == item.content && workTask.endtime == x.endtime);
                    //    if (ck.Count()>0)
                    //    {
                    //        continue;
                    //    }
                    //}
                    workTask.cycle = item.cycle;
                    workTask.cycledate = item.cycledate;
                    workTask.isend = item.isend;
                    workTask.islastday = item.islastday;
                    workTask.isweek = item.isweek;
                    workTask.workstate = "进行中";
                    workTask.dutyuser = item.dutyuser;
                    workTask.dutyuserid = item.dutyuserid;
                    workTask.deptid = item.deptid;
                    workTask.deptcode = item.deptcode;
                    workTask.deptname = item.deptname;
                    workTask.cycletaskid = item.id;
                    workTask.modifytime = DateTime.Now;
                    workTask.modifyuser = user != null ? user.RealName : "自定义服务";
                    workTask.modifyuserid = user != null ? user.UserId : null;
                    WorkCycleTask.Add(workTask);
                }

                #region 未完成任务
                var DeptWorkCycleTaskList = db.FindList<DeptWorkCycleTaskEntity>(x => x.endtime < dayTime).ToList();
                DeptWorkCycleTaskList.ForEach(x => x.workstate = "未完成");
                db.Update(DeptWorkCycleTaskList);
                #endregion

                db.Insert(WorkCycleTask);
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }


        private List<DeptCycleTaskEntity> GetDateJob(List<DeptCycleTaskEntity> list, DateTime date)
        {

            List<DeptCycleTaskEntity> jobs = new List<DeptCycleTaskEntity>();


            var month = date.Month.ToString();//月
            var day = date.Day.ToString();//天

            #region  月份转化为大写
            switch (month)
            {
                case "1":
                    month = "一月";
                    break;
                case "2":
                    month = "二月";
                    break;
                case "3":
                    month = "三月";
                    break;
                case "4":
                    month = "四月";
                    break;
                case "5":
                    month = "五月";
                    break;
                case "6":
                    month = "六月";
                    break;
                case "7":
                    month = "七月";
                    break;
                case "8":
                    month = "八月";
                    break;
                case "9":
                    month = "九月";
                    break;
                case "10":
                    month = "十月";
                    break;
                case "11":
                    month = "十一月";
                    break;
                case "12":
                    month = "十二月";
                    break;
            }
            #endregion
            #region  计算天为本月的第几周
            var newdate = new DateTime(date.Year, date.Month, 1);
            int num = Util.Time.GetWeekNumberOfDay(newdate);//一号为星期几
            int totalday = Util.Time.GetDaysOfMonth(date.Year, date.Month);//本月有多少天
                                                                           //容器存放周数据
            var weeknum = 1;
            var page = 1;
            var weekstr = string.Empty;//星期几
            var weeknumstr = string.Empty;//第几周
            while (true)
            {
                #region 转化
                switch (num)
                {
                    case 1:
                        weekstr = "星期一";
                        break;
                    case 2:
                        weekstr = "星期二";
                        break;
                    case 3:
                        weekstr = "星期三";
                        break;
                    case 4:
                        weekstr = "星期四";
                        break;
                    case 5:
                        weekstr = "星期五";
                        break;
                    case 6:
                        weekstr = "星期六";
                        break;
                    default:
                        weekstr = "星期天";
                        break;
                }
                switch (weeknum)
                {
                    case 1:
                        weeknumstr = "第一周";
                        break;
                    case 2:
                        weeknumstr = "第二周";
                        break;
                    case 3:
                        weeknumstr = "第三周";
                        break;
                    case 4:
                        weeknumstr = "第四周";
                        break;
                    case 5:
                        weeknumstr = "第五周";
                        break;
                    case 6:
                        weeknumstr = "第六周";
                        break;
                    default:
                        weeknumstr = "第一周";
                        break;
                }
                if (page == date.Day)
                {
                    break;
                }
                #endregion
                //如果是星期7则加一个星期
                if (num == 7)
                {
                    weeknum++;
                    num = 1; page++;
                }
                else { num++; page++; }

            }
            #endregion
            //存放星期顺序容器
            List<string> weeksort = new List<string> {
                "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天"
            };
            list = list.OrderByDescending(x => x.createtime).ToList();
            #region  数据筛选
            foreach (var item in list)
            {
                bool ck = false;
                //每天单独计算
                if (item.cycle == "每天")
                {
                    if (item.isweek)
                    {
                        if (weekstr == "星期六" || weekstr == "星期天")
                        {
                            continue;
                        }
                    }
                    jobs.Add(item);
                    continue;
                }
                //  不勾选日期  勾选最后一天  跳过双休  是否截止 特殊情况:每月才存在
                if (string.IsNullOrEmpty(item.cycledate))
                {
                    //可选择是否双休 最后一天
                    if (item.isweek)
                    {
                        if (weekstr == "星期六" || weekstr == "星期天")
                        {
                            ck = false;
                            continue;
                        }
                    }
                    //判断是否最后一天   是否截止日期 截止只有一个日期 勾选最后一天后可以不选日期
                    if (item.islastday)
                    {

                        var daySum = Util.Time.GetDaysOfMonth(date);
                        if (item.isend)
                        {

                            var senddate = new DateTime(date.Year, date.Month, date.Day);
                            var start = new DateTime(date.Year, date.Month, 1);
                            var tosenddate = new DateTime(date.Year, date.Month, totalday).AddDays(1).AddMinutes(-1);
                            var workCk = ckWorkMeeting(tosenddate, item.deptid, item.content);
                            //存在班会
                            if (workCk)
                            {

                                item.starttime = senddate;
                                item.endtime = tosenddate;
                                jobs.Add(item);
                            }
                            continue;
                            //if (daySum >= date.Day)
                            //{
                            //    jobs.Add(item);
                            //    continue;
                            //}
                        }
                        else
                        if (date.Day == daySum)
                        {
                            jobs.Add(item);
                            continue;
                        }
                    }
                    continue;
                }
                //获取规则数组
                var useTimeStr = item.cycledate.Split(';');
                #region
                //规则 年  多选月  多选第几周、几号 多选星期几 班次  是否截至 是否跳过最后一天
                //获取节点  在基础节点判断是否可以推送
                for (int i = 0; i < useTimeStr.Length; i++)
                {
                    var str = useTimeStr[i].ToString();
                    //多选时  按规则顺序 if

                    //每年 每日期  勾选最后一天  特殊情况   每年 二月,八月;
                    if (string.IsNullOrEmpty(str))
                    {
                        //可选择是否双休 最后一天
                        if (item.isweek)
                        {
                            if (weekstr == "星期六" || weekstr == "星期天")
                            {
                                ck = false;
                                break;
                            }
                        }
                        //判断是否最后一天   是否截止日期 截止只有一个日期 勾选最后一天后可以不选日期
                        if (item.islastday)
                        {

                            var daySum = Util.Time.GetDaysOfMonth(date);
                            if (item.isend)
                            {

                                var senddate = new DateTime(date.Year, date.Month, date.Day);
                                var start = new DateTime(date.Year, date.Month, 1);
                                var tosenddate = new DateTime(date.Year, date.Month, totalday).AddDays(1).AddMinutes(-1);
                                var workCk = ckWorkMeeting(tosenddate, item.deptid, item.content);
                                //存在班会
                                if (workCk)
                                {

                                    item.starttime = senddate;
                                    item.endtime = tosenddate;
                                    ck = true;
                                    continue;
                                }
                                else
                                {
                                    ck = false;
                                    break;
                                }

                            }
                            else

                            //if (daySum >= date.Day)
                            //{
                            //    ck = true;
                            //    break;
                            //}
                            if (date.Day == daySum)
                            {
                                ck = true;
                                break;
                            }

                        }
                    }

                    //哪几个月
                    if (str.Contains("月"))
                    {
                        var ckstr = str.Split(',');
                        for (int j = 0; j < ckstr.Length; j++)
                        {
                            ck = ckstr[j] == month ? true : false;
                            if (ck)
                            {
                                break;
                            }
                        }
                        if (!ck) break;

                    }
                    //第几周
                    else
                        if (str.Contains("周"))
                    {
                        ck = str.Contains(weeknumstr) ? true : false;
                        if (!ck) break;

                    }
                    //哪几个星期
                    else
                            if (str.Contains("星期"))
                    {
                        //是否截止日期 截止只有一个日期
                        if (item.isend)
                        {
                            //var sort = weeksort.IndexOf(str);//配置
                            //var nowsort = weeksort.IndexOf(weekstr);
                            //if (nowsort <= sort)
                            //{
                            //    ck = true;
                            //    break;
                            //}
                            var nowsort = weeksort.IndexOf(weekstr);
                            var sort = weeksort.IndexOf(str);//配置
                            var senddate = new DateTime(date.Year, date.Month, date.Day);
                            var start = senddate.AddDays(-nowsort);
                            var tosenddate = start.AddDays(sort + 1).AddMinutes(-1);
                            if (senddate > tosenddate)
                            {
                                ck = false;
                                break;
                            }
                            var workCk = ckWorkMeeting(tosenddate, item.deptid, item.content);
                            //存在班会
                            if (workCk)
                            {

                                item.starttime = senddate;
                                item.endtime = tosenddate;
                                ck = true;
                                continue;

                            }
                            else
                            {
                                ck = false;
                                break;
                            }




                        }
                        ck = str.Contains(weekstr) ? true : false;

                    }
                    //哪几号
                    else
                    {

                        //可选择是否双休 最后一天
                        if (item.isweek)
                        {
                            if (weekstr == "星期六" || weekstr == "星期天")
                            {
                                ck = false;
                                break;
                            }
                        }

                        //是否截止日期 截止只有一个日期
                        if (item.isend)
                        {

                            var senddate = new DateTime(date.Year, date.Month, date.Day); ;
                            var dyaNum = Convert.ToInt32(str);
                            if (totalday < dyaNum)
                            {
                                ck = false;
                                break;
                            }
                            var start = new DateTime(date.Year, date.Month, 1);
                            var tosenddate = new DateTime(date.Year, date.Month, dyaNum).AddDays(1).AddMinutes(-1);
                            if (senddate > tosenddate)
                            {
                                ck = false;
                                break;
                            }
                            var workCk = ckWorkMeeting(tosenddate, item.deptid, item.content);
                            //存在班会
                            if (workCk)
                            {

                                item.starttime = senddate;
                                item.endtime = tosenddate;
                                ck = true;
                                continue;
                            }
                            else
                            {
                                ck = false;
                                break;
                            }

                            //var setday = Convert.ToInt32(str);
                            //if (setday >= date.Day)
                            //{
                            //    ck = true;
                            //    break;
                            //}
                        }
                        //判断是否最后一天
                        if (item.islastday)
                        {
                            var daySum = Util.Time.GetDaysOfMonth(date);
                            if (date.Day == daySum)
                            {
                                ck = true;
                                break;
                            }
                        }

                        var ckstr = str.Split(',');
                        for (int j = 0; j < ckstr.Length; j++)
                        {
                            ck = ckstr[j] == day ? true : false;
                            if (ck)
                            {
                                break;
                            }
                        }


                    }
                    #endregion
                }
                if (ck)
                {
                    jobs.Add(item);
                }
            }
            #endregion

            return jobs;
        }

        /// <summary>
        /// 特殊情况 周期中存在“截止”设定  获取区间是否完成任务
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="deptid"></param>
        /// <param name="content"></param>
        private bool ckWorkMeeting(DateTime end, string deptid, string content)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            // var startend = start.AddDays(1).AddMinutes(-1);
            var query = from q in db.IQueryable<DeptWorkCycleTaskEntity>()
                        where q.deptid == deptid && q.endtime == end && q.content == content
                        orderby q.starttime descending
                        select q;
            var entity = query.FirstOrDefault();
            if (entity == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
    }
}
