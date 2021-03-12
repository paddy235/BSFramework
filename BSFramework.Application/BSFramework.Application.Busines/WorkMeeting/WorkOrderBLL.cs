using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.OndutyManage;
using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class WorkOrderBLL
    {
        private IWorkOrderService service = new WorkOrderService();
        private DepartmentBLL deptbll = new DepartmentBLL();
        /// <summary>
        /// 保存或修改部门班次信息
        /// </summary>
        public void WorkSetSave(List<WorkOrderEntity> data, List<WorkGroupSetEntity> group, DateTime usetime)
        {
            try
            {
                //排班数据
                var list = GetTimeOrder(data, group, usetime, false);
                var old = OldTimeOrder(group, usetime);
                jobGroup(group);
                list.AddRange(old);
                service.WorkSetSave(data, group, list);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        /// <summary>
        /// 保存或修改部门班次信息
        /// </summary>
        public void nextyear(DateTime usetime)
        {
            try
            {
                var allorder = service.GetOrderAll();
                var allgroup = service.GetGroupAll();
                var byGroup = allgroup.GroupBy(x => x.bookmarks);
                foreach (var groupset in byGroup)
                {
                    var group = new List<WorkGroupSetEntity>();
                    var data = allorder.Where(x => x.bookmarks == groupset.Key).ToList();
                    foreach (var item in groupset)
                    {
                        group.Add(item);
                    }
                    //排班数据
                    var list = GetTimeOrder(data, group, usetime, true);
                    service.WorkSetSave(data, group, list);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 保存或修改单天部门班次信息
        /// </summary>
        public void WorkSetSaveOneDay(DateTime UpTime, string worksetting, string WorkOrderId)
        {
            try
            {
                //排班数据
                service.WorkSetSaveOneDay(UpTime, worksetting, WorkOrderId);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 计算排班
        /// </summary>
        /// <returns></returns>
        private List<WorkTimeSortEntity> GetTimeOrder(List<WorkOrderEntity> data, List<WorkGroupSetEntity> group, DateTime usetime, bool next)
        {
            //生成下一年数据  时间固定为下一年的一月一号 next=ture

            //计算得出所有数据
            List<WorkTimeSortEntity> allList = new List<WorkTimeSortEntity>();
            foreach (var item in group)
            {
                //获取部门所有生成的数据
                var sortList = service.GetWorkTimeSort(item.departmentid);

                var dept = deptbll.GetEntity(item.departmentid);
                if (dept == null)
                {
                    continue;
                }
                int year = usetime.Year;
                int month = usetime.Month;
                int day = usetime.Day;
                #region
                //连接月的循环
                var monthorder = -1;
                //生成下年数据
                if (next)
                {
                    var lastYear = year - 1;
                    var getMonthSort = sortList.FirstOrDefault(x => x.year == lastYear && x.month == 12);
                    if (getMonthSort == null)
                    {
                        continue;
                    }
                    monthorder = getMonthSort.setsort;
                }
                //循环月
                for (int i = 1; i <= 12; i++)
                {
                    //月实体
                    var setMonth = new WorkTimeSortEntity();
                    if (i >= month)
                    {
                        //获取当前时间数据
                        var nowTime = sortList.FirstOrDefault(x => x.year == year && x.month == i);
                        //不为空加入删除
                        if (nowTime != null)
                        {
                            nowTime.datatype = "del";
                            allList.Add(nowTime);
                        }
                        setMonth.fullname = item.fullname;
                        setMonth.departmentid = item.departmentid;
                        setMonth.worktimesortid = Guid.NewGuid().ToString();
                        setMonth.month = i;
                        setMonth.deptcode = dept.EnCode;
                        setMonth.year = year;
                        setMonth.datatype = "add";
                        //获取月的天数
                        var daySum = Time.GetDaysOfMonth(year, i);
                        StringBuilder timespan = new StringBuilder();
                        StringBuilder timespanId = new StringBuilder();
                        //规则循环
                        var start = -1;
                        for (int j = 1; j <= daySum; j++)
                        {

                            var week = Time.GetWeekNameOfDay(new DateTime(year, i, j));
                            if (j >= day || i > month)
                            {
                                //开始循环规则
                                if (monthorder > -1)
                                {
                                    if (monthorder == data.Count - 1)
                                    {
                                        start = 0;
                                    }
                                    else
                                    {
                                        start = monthorder + 1;
                                    }

                                    monthorder = -1;
                                }
                                else
                                if (start == -1)
                                {
                                    start = item.groupsort;
                                }
                                //判断是否常白班
                                var ck = data.Where(x => x.isweek).Count();
                                //根据序列获取规则数据
                                var set = new WorkOrderEntity();
                                if (ck == data.Count)
                                {
                                    start = 0;
                                    set = data.FirstOrDefault(x => x.WorkOrderId == item.workorderid);
                                }
                                else
                                {
                                    set = data.FirstOrDefault(x => x.OrderSort == start);
                                }
                                var spanId = set.settingid;
                                var span = set.WorkName == "休息" ? "休息" : set.StartTime.ToString("HH:mm") + "-" + set.EndTime.ToString("HH:mm") + "(" + set.WorkName + ")";
                                if (set.isweek && (week.Contains("六") || week.Contains("日")))
                                {
                                    span = "休息";
                                    spanId = "0";
                                }
                                //月结束时
                                if (j == daySum)
                                {
                                    timespanId.Append(spanId);
                                    timespan.Append(span);
                                    setMonth.timedata = timespan.ToString();
                                    setMonth.timedataid = timespanId.ToString();
                                    setMonth.setsort = start;//结尾时处于循环的那个点
                                    monthorder = start;
                                }
                                else
                                {
                                    timespanId.Append(spanId + ",");
                                    timespan.Append(span + ",");
                                }
                                //序列增加  到尾部 为0
                                start++;
                                if (start == data.Count)
                                {
                                    start = 0;
                                }
                            }
                            else
                            {

                                //不为空加入当前数据
                                if (nowTime != null)
                                {
                                    var str = nowTime.timedata.Split(',');
                                    var strId = nowTime.timedataid.Split(',');
                                    timespan.Append(str[j - 1] + ",");
                                    timespanId.Append(strId[j - 1] + ",");

                                }
                                else
                                {
                                    timespan.Append("无,");
                                    timespanId.Append("0,");
                                }

                            }
                        }
                        //加入list
                        allList.Add(setMonth);
                    }
                    //每年12月完成后下一年
                    if (i == 12)
                    {

                        year = year + 1;
                        month = 1;
                        day = 1;
                        //无下年数据
                        var ckyear = sortList.Where(x => x.year == year);
                        if (ckyear.Count() == 0)
                        {
                            continue;
                        }
                        else
                        {
                            i = 1;
                        }

                    }

                }
                #endregion
            }
            return allList;
        }
        /// <summary>
        /// 获取需要情况的任务库班组id
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private void jobGroup(List<WorkGroupSetEntity> group)
        {
            WorkmeetingBLL meet = new WorkmeetingBLL();
            var deptList = new List<string>();
            var newStr = string.Join(",", group.OrderBy(x => x.departmentid));
            foreach (var newGroup in group)
            {
                //获取之前绑定
                var oldGroup = GetWorkOrderGroup(newGroup.departmentid);
                var oldStr = string.Join(",", oldGroup.OrderBy(x => x.departmentid));
                //如果没有差别不需要清理任务库
                if (newStr == oldStr)
                {
                    break;
                }
                foreach (var old in oldGroup)
                {
                    if (!deptList.Contains(old.departmentid))
                    {
                        deptList.Add(old.departmentid);
                    }

                }
                if (!deptList.Contains(newGroup.departmentid))
                {
                    deptList.Add(newGroup.departmentid);
                }
            }
            //清理绑定班组数据库

            meet.DeleteJobTemplateByDept(deptList);


        }

        public WorkTimeSortEntity GetEntity(DateTime startDate, string keyWord)
        {
            return service.GetEntity(startDate, keyWord);
        }
        private List<WorkTimeSortEntity> OldTimeOrder(List<WorkGroupSetEntity> group, DateTime usetime)
        {

            //计算得出所有数据
            List<WorkTimeSortEntity> allList = new List<WorkTimeSortEntity>();
            int year = usetime.Year;
            int month = usetime.Month;
            int day = usetime.Day;
            foreach (var item in group)
            {

                var oldGroup = GetWorkOrderGroup(item.departmentid);
                var DpetSt = string.Join(",", group.Select(x => x.departmentid));
                foreach (var old in oldGroup)
                {
                    if (!DpetSt.Contains(old.departmentid))
                    {
                        //获取部门所有生成的数据
                        var sortList = service.GetWorkTimeSort(old.departmentid);
                        var clearData = sortList.Where(x => x.year >= year && x.month >= month);
                        foreach (var Sort in clearData)
                        {
                            if (Sort.year == year && Sort.month == month)
                            {
                                var str = Sort.timedata.Split(',');
                                var strId = Sort.timedataid.Split(',');
                                for (int i = day - 1; i < str.Length; i++)
                                {
                                    str[i] = "无";
                                    strId[i] = "0";
                                }
                                Sort.timedata = string.Join(",", str);
                                Sort.timedataid = string.Join(",", strId);
                                Sort.datatype = "update";
                            }
                            else
                            {
                                Sort.datatype = "del";
                            }
                            var ck = allList.FirstOrDefault(x => x.worktimesortid == Sort.worktimesortid);
                            if (ck == null)
                            {
                                allList.Add(Sort);
                            }

                        }
                    }
                }

            }
            return allList;
        }
        /// <summary>
        /// 批量删除排班
        /// </summary>
        /// <param name="keyValue"></param>
        public bool OrderRemoveForm(string keyValue)
        {
            try
            {

                service.OrderRemoveForm(keyValue);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 查询部门排班
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public IEnumerable<WorkTimeSortEntity> GetWorkOrderList(DateTime startTime, DateTime endTime, string departmentId)
        {
            return service.GetWorkOrderList(startTime, endTime, departmentId);
        }

        /// <summary>
        /// 查询班次班组
        /// </summary>
        /// <returns></returns>
        public string GetWorkOrderday(string WorkSortId)
        {
            return service.GetWorkOrderday(WorkSortId);
        }
        /// <summary>
        /// 查询同一批次班组
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public IEnumerable<WorkGroupSetEntity> GetWorkOrderGroup(string departmentId)
        {
            return service.GetWorkOrderGroup(departmentId);
        }
        /// <summary>
        /// 查询部门绑定的班次
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public void GetWorkSettingByDept(string departmentId, out string setupid, out string createuserid)
        {
            setupid = string.Empty;
            createuserid = string.Empty;
            service.GetWorkSettingByDept(departmentId, out setupid, out createuserid);
        }

        /// <summary>
        /// 获取绑定原班组班次
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        public Dictionary<string, string> getSetValue(string DeptId)
        {

            return service.getSetValue(DeptId);
        }
        /// <summary>
        /// 获取排班json
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="days">结束时间</param>
        /// <param name="departmentId">部门id</param>
        /// <returns></returns>
        public List<WorkTimeSortEntity> WorkOrderGet(DateTime startTime, DateTime endTime, string departmentId)
        {
            return service.GetWorkOrderList(startTime, endTime, departmentId).ToList();
        }




        #region 人脸考勤


        public string workAttendance(DateTime startTime, DateTime endTime, string departmentId, string userid)
        {
            FaceAttendanceBLL face = new FaceAttendanceBLL();
            var userTime = DateTime.Now;
            var Sort = service.GetWorkOrderList(startTime, endTime, departmentId).ToList();
            if (Sort.Count == 0)
            {
                return "无效";
            }
            string workTime = string.Empty;
            // 前天下班时间，前天下班有效时间，今天上班有效时间，今天上班时间，今天下班时间，今天下班有效时间
            var pd = workUseTime(startTime, endTime, Sort);
            //获取今天签到数据
            var facedata = face.GetList(startTime, endTime, userid, departmentId).OrderByDescending(x => x.ondutytime).ToList();
            var workState = string.Empty;//签到状态
            var NoNull = true;//是否第一次签到
            var Istime = true;//签到间隔时间
            if (facedata.Count > 0)
            {
                TimeSpan tsDiffer = userTime - facedata[0].ondutytime;
                var Minutes = tsDiffer.TotalMinutes;
                if (Minutes > 1)
                {
                    Istime = false;
                }

                //超过了上次签到的逻辑
                if (userTime > pd[1])
                {
                    NoNull = false;
                    //是否新签到数据
                    if (facedata[0].ondutytime <= pd[5] && facedata[0].ondutytime >= pd[2])
                    {
                        workState = facedata[0].ondutyshift;
                    }

                }
                else
                {
                    workState = facedata[0].ondutyshift;
                }

            }
            else
            {
                Istime = false;
                //超过了上次签到的逻辑
                if (userTime > pd[1])
                { NoNull = false; }

            }
            if (Istime)
            {
                workState = "无效";
                return workState;
            }
            if (NoNull)
            {
                // 上班打卡   迟到  早退  下班打卡
                //在前天下班打卡区间  跨天排班
                if (pd[0] <= userTime && userTime <= pd[1])
                {

                    if (workState == "早退")
                    {
                        face.RemoveForm(facedata[0].id);
                        workState = "下班打卡";
                    }
                    else if (workState == "下班打卡")
                    {
                        workState = "无效";
                    }
                    else
                    {
                        workState = "下班打卡";
                        face.SaveFormTime("", new FaceAttendanceTimeEntity() { id = Guid.NewGuid().ToString(), userid = userid, worktime = userTime });

                    }


                }
                else if (pd[0] > userTime)
                {

                    if (workState == "上班打卡")
                    {
                        workState = "迟到";
                    }
                    else
                         if (workState == "迟到")
                    {
                        workState = "早退";
                        face.SaveFormTime("", new FaceAttendanceTimeEntity() { id = Guid.NewGuid().ToString(), userid = userid, worktime = userTime });
                    }
                    else
                    if (workState == "早退")
                    {
                        workState = "早退";
                        face.RemoveForm(facedata[0].id);
                    }
                    else
                    {
                        workState = "迟到";
                    }

                }
                else if (pd[1] < userTime && userTime < pd[2])
                {
                    workState = "无效";
                }
            }
            else
            {
                //0无效打卡区间，1 上班有效区间，2 上班区间，3 下班打卡有效区间,4 下班无效区间
                int index = 0;
                for (int i = 2; i <= pd.Length - 1; i++)
                {
                    if (pd[i] <= userTime)
                    {
                        index++;
                    }
                }

                if (index > 0 && index < 4)
                {
                    if (index == 1)
                    {
                        //是否已经签到
                        if (workState == "上班打卡")
                        {
                            workState = "无效";
                        }
                        else
                        {
                            workState = "上班打卡";
                        }

                    }
                    if (index == 2)
                    {
                        if (workState == "上班打卡")
                        {
                            workState = "早退";
                        }
                        else
                        if (workState == "迟到")
                        {
                            workState = "早退";
                        }
                        else
                        if (workState == "早退")
                        {
                            workState = "早退";
                            face.RemoveForm(facedata[0].id);
                        }
                        else
                        {
                            workState = "迟到";
                        }

                    }
                    if (index == 3)
                    {

                        if (workState == "早退")
                        {
                            face.RemoveForm(facedata[0].id);
                            workState = "下班打卡";
                            face.SaveFormTime("", new FaceAttendanceTimeEntity() { id = Guid.NewGuid().ToString(), userid = userid, worktime = userTime });


                        }
                        else if (workState == "下班打卡" && (facedata[0].ondutytime >= pd[4] && facedata[0].ondutytime <= pd[5])) { workState = "无效"; }
                        else
                        {
                            workState = "下班打卡";
                            face.SaveFormTime("", new FaceAttendanceTimeEntity() { id = Guid.NewGuid().ToString(), userid = userid, worktime = userTime });

                        }
                    }
                }
                else
                {
                    workState = "无效";
                }

            }




            return workState;
        }
        /// <summary>
        /// 获取签到区间数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="Sort"></param>
        /// <returns></returns>
        private DateTime[] workUseTime(DateTime startTime, DateTime endTime, List<WorkTimeSortEntity> Sort)
        {
            WorkSettingBLL setBll = new WorkSettingBLL();
            // 前天下班时间，前天下班有效时间，今天上班有效时间，今天上班时间，今天下班时间，今天下班有效时间
            DateTime[] pb = new DateTime[6];
            //判断是否跨天数据
            if (startTime.Year == endTime.Year && startTime.Month == endTime.Month)
            {

                //获取数据
                var workSort = Sort.FirstOrDefault(x => x.year == startTime.Year && x.month == startTime.Month);
                var timeDataStr = workSort.timedata.Split(',');
                var timeDataid = workSort.timedataid.Split(',');
                var StartimeData = workDataTime(timeDataStr[startTime.Day - 1], startTime);
                var EndtimeData = workDataTime(timeDataStr[endTime.Day - 1], endTime);
                var StartSet = setBll.getEntity(timeDataid[startTime.Day - 1]);
                var EndSet = setBll.getEntity(timeDataid[endTime.Day - 1]);
                if (StartSet != null)
                {
                    //前天结束时间
                    pb[0] = StartimeData[1];
                    pb[1] = StartimeData[1].AddMinutes(StartSet.EndTimeSpan);

                }
                else
                {
                    pb[0] = StartimeData[1];
                    pb[1] = StartimeData[1];
                }
                if (EndSet != null)
                {
                    //当天上班
                    pb[2] = EndtimeData[0].AddMinutes(-EndSet.StartTimeSpan);
                    pb[3] = EndtimeData[0];
                    //当天下班
                    pb[4] = EndtimeData[1];
                    pb[5] = EndtimeData[1].AddMinutes(EndSet.EndTimeSpan);
                }
                else
                {
                    pb[2] = EndtimeData[0];
                    pb[3] = EndtimeData[0];
                    pb[4] = EndtimeData[1];
                    pb[5] = EndtimeData[1];
                }
            }
            else
            {
                //获取数据
                var StartWorkSort = Sort.FirstOrDefault(x => x.year == startTime.Year && x.month == startTime.Month);
                var StarttimeDataStr = StartWorkSort.timedata.Split(',');
                var StarttimeDataid = StartWorkSort.timedataid.Split(',');
                var StartimeData = workDataTime(StarttimeDataStr[startTime.Day], startTime);
                var StartSet = setBll.getEntity(StarttimeDataStr[startTime.Day]);

                var EndWorkSort = Sort.FirstOrDefault(x => x.year == endTime.Year && x.month == endTime.Month);
                var EndtimeDataStr = EndWorkSort.timedata.Split(',');
                var EndtimeDataid = EndWorkSort.timedataid.Split(',');
                var EndtimeData = workDataTime(EndtimeDataid[endTime.Day], endTime);
                var EndSet = setBll.getEntity(EndtimeDataid[endTime.Day]);

                if (StartSet != null)
                {
                    //前天结束时间
                    pb[0] = StartimeData[1];
                    pb[1] = StartimeData[1].AddMinutes(StartSet.EndTimeSpan);

                }
                else
                {
                    pb[0] = StartimeData[1];
                    pb[1] = StartimeData[1];
                }
                if (EndSet != null)
                {
                    //当天上班
                    pb[2] = EndtimeData[0].AddMinutes(-EndSet.StartTimeSpan);
                    pb[3] = EndtimeData[0];
                    //当天下班
                    pb[4] = EndtimeData[1];
                    pb[5] = EndtimeData[1].AddMinutes(EndSet.EndTimeSpan);
                }
                else
                {
                    pb[2] = EndtimeData[0];
                    pb[3] = EndtimeData[0];
                    pb[4] = EndtimeData[1];
                    pb[5] = EndtimeData[1];
                }

            }
            return pb;

        }
        /// <summary>
        /// 获取排班时间
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Time"></param>
        /// <returns></returns>
        private DateTime[] workDataTime(string data, DateTime Time)
        {
            if (data.Length < 5)
            {
                return new DateTime[2] { new DateTime(2001, 1, 1), new DateTime(2001, 1, 1) };
            }
            var getData = data.Split('(');
            var nowTime = getData[1];
            var dateTimeSpan = data.Split('(')[0];
            var startSpan = dateTimeSpan.Split('-')[0];
            var endSpan = dateTimeSpan.Split('-')[1];
            //Str[0] = nowTime.Replace(")", "");
            var startTime = DateTime.Now;
            var endTime = DateTime.Now;
            var startHour = Convert.ToInt32(startSpan.Split(':')[0]);
            var endHour = Convert.ToInt32(endSpan.Split(':')[0]);
            var startM = Convert.ToInt32(startSpan.Split(':')[1]);
            var endM = Convert.ToInt32(endSpan.Split(':')[1]);
            if ((startHour * 60 + startM) < (endHour * 60 + endM))
            {
                var nowDate = Time.ToString("yyyy-MM-dd");
                startTime = Convert.ToDateTime(nowDate + " " + startSpan);
                endTime = Convert.ToDateTime(nowDate + " " + endSpan);
            }
            else
            {
                var nowDate = Time.ToString("yyyy-MM-dd");
                var nextDate = Time.AddDays(1).ToString("yyyy-MM-dd");
                startTime = Convert.ToDateTime(nowDate + " " + startSpan);
                endTime = Convert.ToDateTime(nextDate + " " + endSpan);
            }

            return new DateTime[2] { startTime, endTime };
        }
        #endregion

        /// <summary>
        /// 查询班组一天数据
        /// </summary>
        /// <returns>长度为2  第一：上班时间段 8：00-15：00 第二：班次 白班 夜班</returns>
        public string[] GetWorkOrderList(DateTime Time, string deparMentId)
        {
            return service.GetWorkOrderList(Time, deparMentId);

        }

        /// <summary>
        /// 查询班组一天数据
        /// </summary>
        /// <returns>长度为2  第一：上班时间段 8：00-15：00 第二：班次 白班 夜班</returns>
        public string[] GetWorkOrderTotal(DateTime Time, string deparMentId)
        {
            return service.GetWorkOrderTotal(Time, deparMentId);

        }
        /// <summary>
        /// 是否可以进行班会
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="useTime"></param>
        /// <returns></returns>
        public bool WorkmeetingTime(string deptId, DateTime useTime)
        {
            var useStr = Convert.ToDateTime(useTime.ToString("yyyy-MM-dd"));

            return false;
        }

    }
}
