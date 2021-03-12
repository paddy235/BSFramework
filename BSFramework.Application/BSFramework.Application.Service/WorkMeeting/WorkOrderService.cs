using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Service.WorkMeeting
{
    public class WorkOrderService : RepositoryFactory<WorkOrderEntity>, IWorkOrderService
    {
        private IWorkSettingService settingService = new WorkSettingService();

        private IDepartmentService service = new DepartmentService();
        /// <summary>
        /// 保存或修改部门班次信息
        /// </summary>
        public void WorkSetSave(List<WorkOrderEntity> data, List<WorkGroupSetEntity> group, List<WorkTimeSortEntity> timeList)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var addTime = new List<WorkTimeSortEntity>();
                #region 需要清理的数据
                var delGroup = new List<WorkGroupSetEntity>();
                var delOrder = new List<WorkOrderEntity>();
                //排班数据
                var delTime = new List<WorkTimeSortEntity>();
                var UpdateTime = new List<WorkTimeSortEntity>();
                foreach (var item in timeList)
                {
                    if (item.datatype == "add")
                    {
                        addTime.Add(item);
                    }
                    else if (item.datatype == "del")
                    {
                        delTime.Add(item);
                    }
                    else
                    {
                        UpdateTime.Add(item);
                    }
                    item.datatype = null;
                }
                //根据班组获取原有排班设置清理
                var bookmarks = string.Empty;
                foreach (var item in group)
                {
                    var getGroup = (from q in db.IQueryable<WorkGroupSetEntity>()
                                    where q.departmentid == item.departmentid
                                    select q).ToList();
                    //存在原有数据
                    if (getGroup.Count() == 1)
                    {
                        bookmarks = getGroup[0].bookmarks;
                        var allGroup = (from q in db.IQueryable<WorkGroupSetEntity>()
                                        where q.bookmarks == bookmarks
                                        select q).ToList();
                        foreach (var groups in allGroup)
                        {
                            if (delGroup.IndexOf(groups) < 0)
                            {
                                delGroup.Add(groups);
                            }
                        }
                    }
                }
                var allOrder = (from q in db.IQueryable<WorkOrderEntity>()
                                where q.bookmarks == bookmarks
                                select q).ToList();

                foreach (var order in allOrder)
                {
                    if (delOrder.IndexOf(order) < 0)
                    {
                        delOrder.Add(order);
                    }
                }

                if (delTime.Count > 0)
                {
                    db.Delete(delTime);
                }
                if (delGroup.Count > 0)
                {
                    db.Delete(delGroup);
                }
                if (delOrder.Count > 0)
                {
                    db.Delete(delOrder);
                }
                if (UpdateTime.Count > 0)
                {
                    db.Update(UpdateTime);
                }
                #endregion
                if (data.Count > 0)
                {
                    db.Insert(data);
                }
                if (group.Count > 0)
                {
                    db.Insert(group);
                }
                if (group.Count > 0)
                {
                    db.Insert(addTime);
                }
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 保存或修改单天部门班次信息
        /// </summary>
        public void WorkSetSaveOneDay(DateTime UpTime, string worksetting, string WorkOrderId)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                int day = UpTime.Day;
                if (string.IsNullOrEmpty(worksetting))
                {
                    var data = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.worktimesortid == WorkOrderId);
                    if (data != null)
                    {
                        var TimeStr = data.timedata.Split(',');
                        var TimeStrId = data.timedataid.Split(',');
                        var span = "休息";
                        var spanId = "0";
                        TimeStr[day - 1] = span;
                        TimeStrId[day - 1] = spanId;
                        data.timedata = string.Join(",", TimeStr);
                        data.timedataid = string.Join(",", TimeStrId);
                        db.Update(data);
                        db.Commit();
                    }
                }
                else
                {
                    var setting = db.IQueryable<WorkSettingEntity>().FirstOrDefault(x => x.WorkSettingId == worksetting);
                    if (setting != null)
                    {
                        var data = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.worktimesortid == WorkOrderId);
                        if (data != null)
                        {
                            var TimeStr = data.timedata.Split(',');
                            var TimeStrId = data.timedataid.Split(',');
                            var span = setting.Name == "休息" ? "休息" : setting.StartTime.ToString("HH:mm") + "-" + setting.EndTime.ToString("HH:mm") + "(" + setting.Name + ")";
                            var spanId = setting.WorkSettingId;
                            TimeStr[day - 1] = span;
                            TimeStrId[day - 1] = spanId;
                            data.timedata = string.Join(",", TimeStr);
                            data.timedataid = string.Join(",", TimeStrId);
                            db.Update(data);
                            db.Commit();
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// 查询班次班组
        /// </summary>
        /// <param name="WorkOrderId"></param>
        /// <returns></returns>
        public string GetWorkOrderday(string worktimesortid)
        {
            try
            {
                string setupId = string.Empty;
                var db = new RepositoryFactory().BaseRepository();
                var getList = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(q => q.worktimesortid == worktimesortid);
                // 获取设置数据
                if (getList==null)
                {
                    return "0";
                }
                var groupList = db.IQueryable<WorkGroupSetEntity>().FirstOrDefault(x=>x.departmentid==getList.departmentid);
                if (groupList == null)
                {
                    return "0";
                }
                var setlist = db.IQueryable<WorkOrderEntity>().FirstOrDefault(x => x.WorkOrderId == groupList.workorderid);
                if (setlist==null)
                {
                    return "0";
                }
                setupId = setlist.setupId;
                return setupId;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        /// <summary>
        /// 查询部门绑定的班次
        /// </summary>
        /// <returns></returns>
        public void GetWorkSettingByDept(string departmentId, out string setupid, out string createuserid)
        {
            try
            {
                setupid = string.Empty;
                createuserid = string.Empty;
                var db = new RepositoryFactory().BaseRepository();
                var group = db.IQueryable<WorkGroupSetEntity>().FirstOrDefault(x => x.departmentid == departmentId);
                if (group != null)
                {
                    var order = db.IQueryable<WorkOrderEntity>().FirstOrDefault(x => x.bookmarks == group.bookmarks);
                    if (order != null)
                    {
                        setupid = order.setupId;
                        createuserid = order.CreateUserId;
                    }

                }


            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 查询部门排班
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkTimeSortEntity> GetWorkOrderList(DateTime startTime, DateTime endTime, string deparMentId)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                var entity = service.GetSubDepartments(deparMentId, "班组");
                var deptStr = string.Join(",", entity.Select(x => x.DepartmentId));
                var startYear = startTime.Year;
                var endYear = endTime.Year;
                var startMonth = startTime.Month;
                var endMonth = endTime.Month;
                //获取设置数据
                //var setList = (from q in db.IQueryable<WorkGroupSetEntity>()
                //               join q3 in db.IQueryable<WorkOrderEntity>() on q.bookmarks equals q3.bookmarks
                //               where deptStr.Contains(q.departmentid)
                //               select q3).ToList();
                //获取区间数据

                if (endYear == startYear)
                {
                    var list = (from q in db.IQueryable<WorkTimeSortEntity>()
                                 where q.year == startYear && q.month >= startMonth&&q.month<=endMonth && deptStr.Contains(q.departmentid)
                                 select q).ToList();
                    return list;
                }
                var Start = (from q in db.IQueryable<WorkTimeSortEntity>()
                               where q.year == startYear && q.month >= startMonth && deptStr.Contains(q.departmentid)
                               select q).ToList();

                while (endYear > startYear)
                {
                    startYear++;
                    if ((endYear - startYear) > 0)
                    {
                        var End = (from q in db.IQueryable<WorkTimeSortEntity>()
                                   where q.year == startYear && deptStr.Contains(q.departmentid)
                                   select q).ToList();
                        Start.Concat(End);
                    }
                    else
                    {
                        var End = (from q in db.IQueryable<WorkTimeSortEntity>()
                                   where q.year == startYear && q.month <= endMonth && deptStr.Contains(q.departmentid)
                                   select q).ToList();
                        Start.Concat(End);
                    }
                }
                return Start;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 查询班组一天数据
        /// </summary>
        /// <returns>长度为2  第一：上班时间段 8：00-15：00 第二：班次 白班 夜班</returns>
        public string[] GetWorkOrderList(DateTime Time, string deparMentId)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                string[] Str = new string[2];
                var Month = Time.Month;
                var Year = Time.Year;
                var Day = Time.Day;
                var data = string.Empty;
                //获取区间数据
                var nowData = new WorkTimeSortEntity();
                while (true)
                {
                    nowData = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.month == Month && x.year == Year && x.departmentid == deparMentId);
                    if (nowData == null)
                    {
                        Str[0] = "无";
                        Str[1] = "无";
                        return Str;

                    }
                    else
                    {
                        var TimeData = nowData.timedata;
                        data = TimeData.Split(',')[Day - 1];
                        if (data == "无" || data == "休息")
                        {
                            Time = Time.AddDays(1);
                            Month = Time.Month;
                            Year = Time.Year;
                            Day = Time.Day;
                        }

                        else
                        {
                            var getDatack = data.Split('(');
                            var dateTimeSpanck = data.Split('(')[0];
                            var startSpanck = dateTimeSpanck.Split('-')[0];
                            var endSpanck = dateTimeSpanck.Split('-')[1];
                            var nowDate = Time.ToString("yyyy-MM-dd");
                            var endTimeck = Convert.ToDateTime(nowDate + " " + endSpanck);
                            var startTimeck = Convert.ToDateTime(nowDate + " " + startSpanck);
                            if (endTimeck <= startTimeck)
                            {
                                break;
                            }
                            var nowTimeck = DateTime.Now;
                            if (nowTimeck > endTimeck)
                            {
                                Time = Time.AddDays(1);
                                Month = Time.Month;
                                Year = Time.Year;
                                Day = Time.Day;
                            }
                            else
                                break;
                        }
                    }

                }
                var getData = data.Split('(');
                var nowTime = getData[1];
                var dateTimeSpan = data.Split('(')[0];
                var startSpan = dateTimeSpan.Split('-')[0];
                var endSpan = dateTimeSpan.Split('-')[1];
                Str[0] = nowTime.Replace(")", "");
                var startTime = string.Empty;
                var endTime = string.Empty;
                var startHour = Convert.ToInt32(startSpan.Split(':')[0]);
                var endHour = Convert.ToInt32(endSpan.Split(':')[0]);
                if (startHour <= endHour)
                {
                    var nowDate = Time.ToString("yyyy-MM-dd");
                    startTime = Convert.ToDateTime(nowDate + " " + startSpan).ToString("yyyy/MM/dd HH:mm");
                    endTime = Convert.ToDateTime(nowDate + " " + endSpan).ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    var nowDate = Time.ToString("yyyy-MM-dd");
                    var nextDate = Time.AddDays(1).ToString("yyyy-MM-dd");
                    startTime = Convert.ToDateTime(nowDate + " " + startSpan).ToString("yyyy/MM/dd HH:mm");
                    endTime = Convert.ToDateTime(nextDate + " " + endSpan).ToString("yyyy/MM/dd HH:mm");
                }
                Str[1] = startTime + "-" + endTime;
                return Str;
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// 查询班组一天数据
        /// </summary>
        /// <returns>长度为2  第一：上班时间段 8：00-15：00 第二：班次 白班 夜班</returns>
        public string[] GetWorkOrderTotal(DateTime Time, string deparMentId)
        {
            try
            {
                var db = new RepositoryFactory().BaseRepository();
                string[] Str = new string[2];
                var Month = Time.Month;
                var Year = Time.Year;
                var Day = Time.Day;
                var data = string.Empty;
                //获取区间数据
                var nowData = new WorkTimeSortEntity();
                while (true)
                {
                    nowData = db.IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.month == Month && x.year == Year && x.departmentid == deparMentId);
                    if (nowData == null)
                    {
                        Str[0] = "无";
                        Str[1] = "无";
                        return Str;

                    }
                    else
                    {
                        var TimeData = nowData.timedata;
                        data = TimeData.Split(',')[Day - 1];
                        if (data == "无" || data == "休息")
                        {

                            Str[0] = "无";
                            Str[1] = "无";
                            return Str;
                        }

                        else
                        {
                            var getDatack = data.Split('(');
                            var dateTimeSpanck = data.Split('(')[0];
                            var startSpanck = dateTimeSpanck.Split('-')[0];
                            var endSpanck = dateTimeSpanck.Split('-')[1];
                            var nowDate = Time.ToString("yyyy-MM-dd");
                            var endTimeck = Convert.ToDateTime(nowDate + " " + endSpanck);
                            var startTimeck = Convert.ToDateTime(nowDate + " " + startSpanck);
                            if (endTimeck <= startTimeck)
                            {
                                break;
                            }
                            var nowTimeck = DateTime.Now;
                            if (nowTimeck > endTimeck)
                            {
                                Time = Time.AddDays(1);
                                Month = Time.Month;
                                Year = Time.Year;
                                Day = Time.Day;
                            }
                            else
                                break;
                        }
                    }

                }
                var getData = data.Split('(');
                var nowTime = getData[1];
                var dateTimeSpan = data.Split('(')[0];
                var startSpan = dateTimeSpan.Split('-')[0];
                var endSpan = dateTimeSpan.Split('-')[1];
                Str[0] = nowTime.Replace(")", "");
                var startTime = string.Empty;
                var endTime = string.Empty;
                var startHour = Convert.ToInt32(startSpan.Split(':')[0]);
                var endHour = Convert.ToInt32(endSpan.Split(':')[0]);
                if (startHour <= endHour)
                {
                    var nowDate = Time.ToString("yyyy-MM-dd");
                    startTime = Convert.ToDateTime(nowDate + " " + startSpan).ToString("yyyy/MM/dd HH:mm");
                    endTime = Convert.ToDateTime(nowDate + " " + endSpan).ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    var nowDate = Time.ToString("yyyy-MM-dd");
                    var nextDate = Time.AddDays(1).ToString("yyyy-MM-dd");
                    startTime = Convert.ToDateTime(nowDate + " " + startSpan).ToString("yyyy/MM/dd HH:mm");
                    endTime = Convert.ToDateTime(nextDate + " " + endSpan).ToString("yyyy/MM/dd HH:mm");
                }
                Str[1] = startTime + "-" + endTime;
                return Str;
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// 查询部门排班
        /// <returns></returns>
        public IEnumerable<WorkTimeSortEntity> GetWorkTimeSort(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<WorkTimeSortEntity>().Where(x => x.departmentid == deptid);
        }
        /// <summary>
        /// 获取所有设置
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkGroupSetEntity> GetGroupAll()
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<WorkGroupSetEntity>();
        }

        /// <summary>
        /// 获取绑定原班组班次
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        public Dictionary<string, string> getSetValue(string DeptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var reslut = new Dictionary<string, string>();
            var groupList = db.IQueryable<WorkGroupSetEntity>().FirstOrDefault(x => x.departmentid == DeptId);
            if (groupList == null)
            {
                return reslut;
            }
            var bookmark = groupList.bookmarks;

            var setList = db.IQueryable<WorkOrderEntity>(x => x.bookmarks == bookmark);
            foreach (var item in setList)
            {


                //是否为空
                if (reslut.Count(x => x.Key == item.settingid) == 0)
                {
                    reslut.Add(item.settingid, item.WorkName);
                }

            }
            return reslut;

        }
        /// <summary>
        /// 获取所有设置
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkOrderEntity> GetOrderAll()
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<WorkOrderEntity>();
        }
        /// <summary>
        /// 批量删除排班
        /// </summary>
        /// <param name="keyValue"></param>
        public void OrderRemoveForm(string keyValue)
        {
            var one = this.BaseRepository().IQueryable().FirstOrDefault(row => row.WorkOrderId == keyValue);
            //if (one!=null)
            //{
            //    var other = this.BaseRepository().IQueryable().Where(row => row.DepartMentId == one.DepartMentId).GroupBy(row => row.bookmarks).ToList();
            //    foreach (var item in other)
            //    {
            //        var list = this.BaseRepository().IQueryable().Where(row => row.bookmarks == item.Key).ToList();
            //        foreach (var items in list)
            //        {
            //            this.BaseRepository().Delete(items.WorkOrderId);
            //        }
            //    }
            //}

        }
        /// <summary>
        /// 查询同一批次班组
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public IEnumerable<WorkGroupSetEntity> GetWorkOrderGroup(string departmentId)
        {

            var db = new RepositoryFactory().BaseRepository();
            var have = db.IQueryable<WorkGroupSetEntity>().FirstOrDefault(row => row.departmentid == departmentId);
            if (have != null)
            {
                List<WorkGroupSetEntity> allList = new List<WorkGroupSetEntity>();
                var all = db.IQueryable<WorkGroupSetEntity>().Where(row => row.bookmarks == have.bookmarks).ToList();
                foreach (var item in all)
                {
                    var check = allList.FirstOrDefault(row => row.fullname == item.fullname);
                    if (check == null)
                    {
                        allList.Add(item);
                    }

                }
                return allList;
            }
            return new List<WorkGroupSetEntity>();
        }

        /// <summary>
        /// 获取某班组整月的排班设置
        /// </summary>
        /// <param name="startDate">时间</param>
        /// <param name="keyWord">班组的ID</param>
        /// <returns></returns>
        public WorkTimeSortEntity GetEntity(DateTime startDate, string keyWord)
        {
            return new RepositoryFactory().BaseRepository().IQueryable<WorkTimeSortEntity>().FirstOrDefault(x => x.year == startDate.Year && x.month == startDate.Month && x.departmentid == keyWord);
        }
    }
}
