using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.Activity;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SystemManage
{
    public class IndexManageBLL
    {
        private IIndexManageService service = new IndexManageService();
        /// <summary>
        /// 班会
        /// </summary>
        private WorkmeetingIService _workService;
        /// <summary>
        /// 教育培训
        /// </summary>
        private IEduBaseInfoService _baseService;
        /// <summary>
        /// 教育培训 安全学习日
        /// </summary>
        private EdActivityIService _edaService;
        /// <summary>
        /// 班组活动
        /// </summary>
        private ActivityIService _aCService;
        /// <summary>
        /// 人身风险预控
        /// </summary>
        private IHumanDangerTrainingService _HDService;

        /// <summary>
        /// 危险预制训练
        /// </summary>
        private DangerIService _DService;

        /// <summary>
        /// 部门
        /// </summary>
        private IDepartmentService _DeptService;

        public IndexManageBLL()
        {
            _workService = new WorkmeetingService();
            _baseService = new EduBaseInfoService();
            _edaService = new EdActivityService();
            _aCService = new ActivityService();
            _HDService = new HumanDangerTrainingService();
            _DService = new DangerService();
            _DeptService = new DepartmentService();
        }

        /// <summary>
        /// 查询所有的首页指标标题
        /// </summary>
        /// <param name="deptId">单位id</param>
        /// <param name="indexType">类型 0-web平台端  1-安卓终端</param>
        /// <param name="keyword">查询关键字</param>
        /// <returns></returns>
        public List<IndexManageEntity> GetList(string deptId, int indexType, string keyword = null, int? templet = null)
        {
            return service.GetList(deptId, indexType, keyword, templet);
        }

        public void SaveForm(string keyValue, IndexManageEntity entity)
        {
            service.SaveForm(keyValue, entity);
        }

        public IndexManageEntity GetForm(string keyValue)
        {
            return service.GetForm(keyValue);
        }

        public void Remove(string keyValue)
        {
            service.Remove(keyValue);
        }
        #region 安卓终端  指标统计
        /// <summary>
        /// 获取任务统计
        /// </summary>
        /// <param name="deptId">部门Id</param>
        /// <returns></returns>
        public Dictionary<string, int> GetJobMonthCount(string deptId)
        {

            var time = DateTime.Now;
            var startTime = new DateTime(time.Year, time.Month, 1);
            var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"deptId\":\"" + deptId + "\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            var data = _workService.GetMeetJobList(pagination, queryJson);
            //业务分类有定期工作|巡回检查|日常工作|消缺任务|其他
            Dictionary<string, int> result = new Dictionary<string, int>();
            var dqrw = data.Count(x => x.TaskType == "定期工作");
            result.Add("定期工作", dqrw);
            var xhjc = data.Count(x => x.TaskType == "巡回检查");
            result.Add("巡回检查", xhjc);
            var rcgz = data.Count(x => x.TaskType == "日常工作");
            result.Add("日常工作", rcgz);
            var xqrw = data.Count(x => x.TaskType == "消缺任务");
            result.Add("消缺任务", xqrw);
            var qt = pagination.records - (dqrw + xhjc + rcgz + xqrw);
            result.Add("其他", qt);
            return result;
        }
        /// <summary>
        /// 教育培训统计
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetEdMonthCount(string deptId)
        {
            var time = DateTime.Now;
            var startTime = new DateTime(time.Year, time.Month, 1);
            var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"deptId\":\"" + deptId + "\",\"Flow\":\"1\",\"Category\":\"安全学习日\",\"State\":\"Finish\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            //业务分类有安全学习日|技术讲课|技术问答|事故预想|其他
            Dictionary<string, int> result = new Dictionary<string, int>();
            //获取安全学习日
            var acdata = _edaService.GetEdAcJobCount(pagination, queryJson);
            var aqxxr = pagination.records;
            result.Add("安全学习日", aqxxr);
            //获取教育培训
            var eddata = _baseService.GetEdJobList(pagination, queryJson);
            var jsjk = eddata.Count(x => x.EduType == "1");
            result.Add("技术讲课", jsjk);
            var jswd = eddata.Count(x => x.EduType == "2" || x.EduType == "5");
            result.Add("技术问答", jswd);
            var sgyx = eddata.Count(x => x.EduType == "3" || x.EduType == "6");
            result.Add("事故预想", jsjk);
            var qt = pagination.records - (jsjk + jswd + sgyx);
            result.Add("其他", qt);
            return result;
        }

        /// <summary>
        /// 班组活动统计
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetACMonthCount(string deptId)
        {
            var time = DateTime.Now;
            var startTime = new DateTime(time.Year, time.Month, 1);
            var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"deptId\":\"" + deptId + "\",\"State\":\"Finish\",\"haveEvaluate\":\"0\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            //业务分类有安全日活动|政治学习|民主管理会|班务会|其他
            Dictionary<string, int> result = new Dictionary<string, int>();
            var data = _aCService.GetAcJobCount(pagination, queryJson);
            var aqrhd = data.Count(x => x.ActivityType == "安全日活动");
            result.Add("安全日活动", aqrhd);
            var zzxx = data.Count(x => x.ActivityType == "政治学习");
            result.Add("政治学习", zzxx);
            var mzglh = data.Count(x => x.ActivityType == "民主管理会");
            result.Add("民主管理会", mzglh);
            var bwh = data.Count(x => x.ActivityType == "班务会");
            result.Add("班务会", bwh);
            var qt = pagination.records - (aqrhd + zzxx + mzglh + bwh);
            result.Add("其他", qt);
            return result;

        }

        /// <summary>
        /// 人身风险预控统计
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetHdMonthCount(string deptId)
        {


            var time = DateTime.Now;
            var startTime = new DateTime(time.Year, time.Month, 1);
            var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
            //var data = _HDService.GetListByDeptId(deptId, startTime, endTime);
            int Finishtotal = 0;
            int unFinishtotal = 0;
            var deptGroup = new string[] { deptId };
            _HDService.GetTrainings("", deptGroup, "", startTime, endTime, "", 50000, 1, null, null, null, out Finishtotal);
            _HDService.GetUndo(deptGroup, "", startTime, endTime, 50000, 1, out unFinishtotal);
            //业务分类有已完成|未完成
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("已完成", Finishtotal);
            result.Add("未完成", unFinishtotal);
            return result;
        }
        /// <summary>
        ///危险预制训练                           
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetDMonthCount(string deptId)
        {
            var time = DateTime.Now;
            var startTime = new DateTime(time.Year, time.Month, 1);
            var endTime = startTime.AddMonths(1).AddMilliseconds(-1);
            var data = _DService.GetTrainingsDo(deptId, startTime, endTime);
            //业务分类有已完成|未完成
            Dictionary<string, int> result = new Dictionary<string, int>();
            var ywc = data.Count(x => x.Status == 2);
            result.Add("已完成", ywc);

            var wwc = data.Count() - ywc;
            result.Add("未完成", wwc);
            return result;
        }



        #endregion


        #region 管理平台首页 


        #region 实时预警



        /// <summary>
        /// 获取连续一周未开班会|连续两周未开展班组活动|是否使用人身风险预控or14天未开|未评价台账
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="traingtype"></param>
        /// <param name="getData">bool [] 0连续一周未开班会|1连续两周未开展班组活动|2是否使用人身风险预控or14天未开|3未整改隐患未整改违章</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<string, int> RealTime(string deptId, string traingtype, bool[] getData, string userId)
        {
            //获取根节点
            var dept = _DeptService.GetAuthorizationDepartment(deptId);
            //获取所有班组信息 
            var depts = _DeptService.GetSubDepartments(dept.DepartmentId, "班组");
            var NowTime = DateTime.Now;
            var endTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day).AddDays(1).AddMilliseconds(-1);
            var startTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day).AddDays(-30).AddMilliseconds(-1);
            //存放所有数据
            var all = new Dictionary<string, int>();

            //2是否使用人身风险预控or14天未开
            if (getData[2])
            {
                if (traingtype == "人身风险预控")
                {
                    //今日未开展人身风险预控
                    var HDangerDayData = HDangerDay(depts, NowTime.Date, endTime);
                    foreach (var item in HDangerDayData)
                    {
                        all.Add(item.Key, item.Value);

                    }
                }
                else
                {
                    //14天未开  CustomerModel 危险预知训练 
                    var TheWeekNoDangerData = TheWeekNoDanger(depts, traingtype, startTime, endTime);
                    foreach (var item in TheWeekNoDangerData)
                    {
                        if (item.Key.Contains("评价"))
                        {
                            all["未评价台账"] = all["未评价台账"] + item.Value;
                        }
                        else
                        {
                            all.Add(item.Key, item.Value);
                        }


                    }
                }

            }
            //0连续一周未开班会
            if (getData[0])
            {
                //连续一周未开班会
                var TheWeekNoJobData = TheWeekNoJob(depts, startTime, endTime);
                foreach (var item in TheWeekNoJobData)
                {
                    if (item.Key.Contains("评价"))
                    {
                        all["未评价台账"] = all["未评价台账"] + item.Value;
                    }
                    else
                    {
                        all.Add(item.Key, item.Value);
                    }

                }
            }
            //1连续两周未开展班组活动
            if (getData[1])
            {
                //连续两周未开展班组活动
                var TheTwoWeekNoAcData = TheTwoWeekNoAc(depts, startTime, endTime);
                foreach (var item in TheTwoWeekNoAcData)
                {
                    if (item.Key.Contains("评价"))
                    {
                        all["未评价台账"] = all["未评价台账"] + item.Value;
                    }
                    else
                    {
                        all.Add(item.Key, item.Value);
                    }

                }
            }

            //未整改隐患未整改违章
            if (getData[3])
            {
                var TheHiddenFault = HiddenFault(userId);
                foreach (var item in TheHiddenFault)
                {
                    all.Add(item.Key, item.Value);
                }
            }
            //未评价台账 教育培训
            if (getData[4])
            {
                var EdTotal = GetEdTotal(depts);
                foreach (var item in EdTotal)
                {
                    all.Add(item.Key, item.Value);
                }
                //all.Add("未评价台账", 0);
            }

            return all;
        }


        /// <summary>
        /// 获取教育培训统计
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetEdTotal(List<DepartmentEntity> depts)
        {
            var time = DateTime.Now;

            //获取所有班组信息 
            var deptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            var queryJson = "{\"deptId\":\"" + deptStr + "\",\"Flow\":\"1\",\"State\":\"Finish\",\"haveEvaluate\":\"1\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 1000000
            };
            //获取教育培训
            var eddata = _baseService.GetEdJobList(pagination, queryJson);
            var result = new Dictionary<string, int>();
            var total = eddata.Where(x => x.Appraises.Count > 0).Count();
            result.Add("未评价台账", total);
            return result;
        }

        /// <summary>
        /// 连续一周未开班会 往前推30天中是否存在连续7天未开班会
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> TheWeekNoJob(List<DepartmentEntity> depts, DateTime startTime, DateTime endTime)
        {


            var deptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            //获取所有数据
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"Depts\":\"" + deptStr + "\",\"haveEvaluate\":\"0\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            var data = _workService.GetMeetPage(pagination, queryJson);
            var haveNum = 0;
            // var haveEvaluateNum = 0;
            foreach (var item in depts)
            {
                var deptData = data.Where(x => x.DeptId == item.DepartmentId).OrderByDescending(x => x.TopStartTime);
                if (deptData.Count() > 0)
                {
                    var current = deptData.FirstOrDefault();
                    ////是否评价
                    //if (current.Evaluates.Count() == 0)
                    //{
                    //    haveEvaluateNum++;
                    //}
                    int index = 1;
                    while (true)
                    {
                        var next = deptData.Skip(index).FirstOrDefault();
                        if (next == null)
                        {
                            if (index == 1)
                            {
                                //如果存在一条数据 但可能超出区间
                                if (Util.Time.DiffDays(startTime, current.TopStartTime.Value) >= 7)
                                {
                                    haveNum++;
                                }
                            }
                            break;
                        }
                        var nextNum = Util.Time.DiffDays(next.TopStartTime.Value, current.TopStartTime.Value);
                        if (nextNum >= 7)
                        {
                            haveNum++;
                        }
                        current = next;
                        index++;
                    }
                }
                else
                {
                    haveNum++;
                }

            }
            //业务分类有 连续一周未开班会
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("连续一周未开班会", haveNum);
            //result.Add("班会评价", haveEvaluateNum);
            return result;


        }

        /// <summary>
        /// 连续两周未开展班组活动
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> TheTwoWeekNoAc(List<DepartmentEntity> depts, DateTime startTime, DateTime endTime)
        {

            var deptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            //获取所有数据
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"Depts\":\"" + deptStr + "\",\"haveEvaluate\":\"0\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };

            var data = _aCService.GetAcJobCount(pagination, queryJson);
            var haveNum = 0;
            //var haveEvaluateNum = 0;
            foreach (var item in depts)
            {
                var deptData = data.Where(x => x.GroupId == item.DepartmentId).OrderByDescending(x => x.StartTime);
                if (deptData.Count() > 0)
                {
                    var current = deptData.FirstOrDefault();
                    ////是否评价
                    //if (current.Evaluates.Count() == 0)
                    //{
                    //    haveEvaluateNum++;
                    //}
                    int index = 1;
                    while (true)
                    {
                        var next = deptData.Skip(index).FirstOrDefault();
                        if (next == null)
                        {
                            if (index == 1)
                            {
                                //如果存在一条数据 但可能超出区间
                                if (Util.Time.DiffDays(startTime, current.StartTime) >= 14)
                                {
                                    haveNum++;
                                }
                            }
                            break;
                        }
                        var nextNum = Util.Time.DiffDays(next.StartTime, current.StartTime);
                        if (nextNum >= 14)
                        {
                            haveNum++;
                        }
                        current = next;
                        index++;
                    }
                }
                else
                {
                    haveNum++;
                }

            }
            //业务分类有 连续两周未开展班组活动
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("连续两周未开展班组活动", haveNum);
            //result.Add("班组活动评价", haveEvaluateNum);
            return result;
        }

        /// <summary>
        /// 今日未开展人身风险预控
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> HDangerDay(List<DepartmentEntity> depts, DateTime startTime, DateTime endTime)
        {
            var deptStrs = depts.Select(x => x.DepartmentId).ToArray();

            int total = 0;
            var data = _HDService.GetTrainings("", deptStrs, "", startTime, endTime, "", 50000, 1, null, null, null, out total);
            var num = 0;
            foreach (var item in depts)
            {
                var haveNum = data.Where(x => x.DeptId == item.DepartmentId).Count();
                if (haveNum == 0)
                {
                    num++;
                }
            }
            //业务分类有 今日未开展人身风险预控
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("今日未开展人身风险预控", num);
            return result;

        }

        /// <summary>
        /// 14天未开  CustomerModel 危险预知训练 
        /// </summary>
        /// <param name="depts"></param>
        /// <param name="CustomerModel">配置名称</param>
        /// <returns></returns>
        private Dictionary<string, int> TheWeekNoDanger(List<DepartmentEntity> depts, string CustomerModel, DateTime startTime, DateTime endTime)
        {
            var deptStrs = depts.Select(x => x.DepartmentId).ToArray();
            //获取所有数据
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"Depts\":\"" + deptStrs + "\",\"haveEvaluate\":\"0\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            var data = _DService.GetDangerCount(pagination, queryJson);
            var haveNum = 0;
            //var haveEvaluateNum = 0;
            foreach (var item in depts)
            {
                var deptData = data.Where(x => x.GroupId == item.DepartmentId).OrderByDescending(x => x.JobTime);
                if (deptData.Count() > 0)
                {
                    var current = deptData.FirstOrDefault();
                    ////是否评价
                    //if (current.Evaluateions.Count() == 0)
                    //{
                    //    haveEvaluateNum++;
                    //}
                    int index = 1;
                    while (true)
                    {
                        var next = deptData.Skip(index).FirstOrDefault();
                        if (next == null)
                        {
                            if (index == 1)
                            {
                                //如果存在一条数据 但可能超出区间
                                if (Util.Time.DiffDays(startTime, current.JobTime.Value) >= 14)
                                {
                                    haveNum++;
                                }
                            }
                            break;
                        }
                        var nextNum = Util.Time.DiffDays(next.JobTime.Value, current.JobTime.Value);
                        if (nextNum >= 14)
                        {
                            haveNum++;
                        }
                        current = next;
                        index++;
                    }
                }
                else
                {
                    haveNum++;
                }

            }
            //业务分类有 14天未开展+CustomerModel
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("14天未开展" + CustomerModel, haveNum);
            //result.Add(CustomerModel + "未评价", haveEvaluateNum);
            return result;
        }

        /// <summary>
        /// 获取未整改违章 未整改隐患
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Dictionary<string, int> HiddenFault(string userId)
        {

            var result = new Dictionary<string, int>();
            try
            {
                var josnObject = new
                {
                    userid = userId
                };
                string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Home", "getBZPlatformIndexInfo"), "json=" + JsonConvert.SerializeObject(josnObject));
                var ret = JsonConvert.DeserializeObject<ResponesModel>(res);
                List<KeyValue> skData = new List<KeyValue>();
                var yh = "0";
                var wz = "0";
                if (ret != null && ret.data != null)
                {
                    skData = JsonConvert.DeserializeObject<List<KeyValue>>(ret.data.ToString());
                    KeyValue k1 = skData.Where(x => x.key == "SK_WZGYH").FirstOrDefault();
                    KeyValue k2 = skData.Where(x => x.key == "SK_WZGWZ").FirstOrDefault();
                    yh = k1 == null ? "0" : k1.value;
                    wz = k2 == null ? "0" : k2.value;
                }
                result.Add("未整改违章", Convert.ToInt32(wz));
                result.Add("未整改隐患", Convert.ToInt32(yh));
            }
            catch (Exception ex)
            {

            }
            return result;

        }
        #endregion

        #region 今日班会


        public List<MeetingBookEntity> GetDeptsMeet(string deptId)
        {
            var time = DateTime.Now;
            var startTime = time.Date;
            //获取根节点
            var dept = _DeptService.GetAuthorizationDepartment(deptId);
            //获取所有班组信息 
            var depts = _DeptService.GetSubDepartments(dept.DepartmentId, "班组");
            var endTime = startTime.AddDays(1).AddMilliseconds(-1);
            var deptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            //获取所有数据
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"Depts\":\"" + deptStr + "\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            var data = _workService.GetMeetPage(pagination, queryJson);
            var ckdept = data.Select(x => x.DeptId);
            var result = new List<MeetingBookEntity>();
            foreach (var item in depts.OrderBy(x => x.EnCode))
            {
                if (!ckdept.Contains(item.DepartmentId))
                {
                    var One = new MeetingBookEntity()
                    {
                        DeptName = item.FullName,
                        DeptId=item.DepartmentId,
                        MeetingId = string.Empty

                    };
                    result.Add(One);
                }
                else
                {
                    var One = data.FirstOrDefault(x => x.DeptId == item.DepartmentId);
                    result.Add(One);
                }

            }
            return result;
        }

        #endregion

        #region 今日作业
        /// <summary>
        /// 获取今日作业
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public Dictionary<string, int> TodayWorkStatistics(string deptId)
        {

            //var dept = _DeptService.GetEntity(deptId);
            string searchCode = string.Empty;

            DateTime searchTime;
            //if (!string.IsNullOrWhiteSpace(startTime))
            //{
            //    if (DateTime.TryParse(startTime, out searchTime))
            //    {
            //        searchTime = DateTime.Now;
            //    }
            //}
            //else
            //{
            searchTime = DateTime.Now;
            //}
            var depts = _DeptService.GetSubDepartments(deptId, "");
            var DeptStr = string.Join(",", depts.Select(x => x.DepartmentId));


            Dictionary<string, int> dic = _workService.TodayWorkStatistics(DeptStr, searchTime.Date);
            return dic;
        }

        #endregion


        #region 教育培训统计
        /// <summary>
        /// 获取教育培训统计
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetEdCount(string deptId, DateTime startTime, DateTime endTime)
        {

            var time = DateTime.Now;
            //获取根节点
            var dept = _DeptService.GetAuthorizationDepartment(deptId);
            //获取所有班组信息 
            var depts = _DeptService.GetSubDepartments(dept.DepartmentId, "");
            var deptStr = string.Join(",", depts.Select(x => x.DepartmentId));
            var queryJson = "{\"startTime\":\"" + startTime + "\",\"endTime\":\"" + endTime + "\",\"deptId\":\"" + deptStr + "\",\"Flow\":\"1\",\"Category\":\"安全学习日\",\"State\":\"Finish\"}";
            Pagination pagination = new Pagination()
            {
                page = 1,
                rows = 100000
            };
            var result = new Dictionary<string, int>();
            //获取教育培训
            var eddata = _baseService.GetEdJobList(pagination, queryJson);
            var jsjk = eddata.Count(x => x.EduType == "1");
            result.Add("技术讲课", jsjk);
            var jswd = eddata.Count(x => x.EduType == "2" || x.EduType == "5");
            result.Add("技术问答", jswd);
            var sgyx = eddata.Count(x => x.EduType == "3" || x.EduType == "6");
            result.Add("事故预想", sgyx);
            var fsgyx = eddata.Count(x => x.EduType == "4");
            result.Add("反事故演习", fsgyx);
            var kwjj = eddata.Count(x => x.EduType == "7" || x.EduType == "8");
            result.Add("考问讲解", kwjj);

            return result;

        }

        #endregion
        #region 隐患趋势


        public Dictionary<string, Dictionary<int, int>> HiddenDanger(string userId, string year)
        {

            var result = new Dictionary<string, Dictionary<int, int>>();
            try
            {
                var josnObject = new
                {
                    userid = userId,
                    data = new { year = year }

                };
                string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Hidden", "GetHtChartForTeam"), "json=" + JsonConvert.SerializeObject(josnObject));
                var ret = JsonConvert.DeserializeObject<ResponesHiddenDanger>(res);

                if (ret != null && ret.data != null)
                {
                    var monthdata = ret.data;

                    var yb = new Dictionary<int, int>();
                    var zd = new Dictionary<int, int>();
                    foreach (var item in monthdata.monthdata)
                    {
                        yb.Add(item.name, item.ybnum);
                        zd.Add(item.name, item.zdnum);
                    }
                    result.Add("重大隐患", zd);
                    result.Add("一般隐患", yb);
                }

            }
            catch (Exception ex)
            {

            }
            return result;

        }

        public class ResponesHiddenDanger
        {
            public string code { get; set; }
            public string info { get; set; }
            public int? count { get; set; }
            public HiddenDangerData data { get; set; }
        }
        /// <summary>
        /// 隐患统计数据
        /// </summary>
        public class HiddenDangerData
        {
            public List<HiddenMonth> monthdata { get; set; }
            public List<HiddenRank> rankdata { get; set; }
        }

        public class HiddenMonth
        {
            public int name { get; set; }
            public int ybnum { get; set; }
            public int jyznum { get; set; }
            public int zdnum { get; set; }
        }

        public class HiddenRank
        {
            public string name { get; set; }

            public string num { get; set; }
            public string precent { get; set; }
            public string color { get; set; }
        }

        #endregion

        #region 人身风险预控
        /// <summary>
        /// 获取人身风险预控
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="depts"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetHDCount(string userId, List<DepartmentEntity> depts, DateTime startTime, DateTime endTime)
        {
            int Finishtotal = 0;
            int unFinishtotal = 0;

            var deptGroup = depts.Select(x => x.DepartmentId).ToArray();
            _HDService.GetTrainings(userId, deptGroup, "", startTime, endTime, "", 50000, 1, null, null, null, out Finishtotal);
            _HDService.GetUndo(deptGroup, "", startTime, endTime, 50000, 1, out unFinishtotal);
            var result = new Dictionary<string, int>();
            result.Add("已完成", Finishtotal);
            result.Add("未完成", unFinishtotal);
            return result;
        }


        #endregion



        #endregion

    }

}
