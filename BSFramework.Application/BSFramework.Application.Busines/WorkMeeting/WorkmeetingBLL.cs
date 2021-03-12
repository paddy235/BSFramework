using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using BSFramework.Application.Entity.BaseManage;
using System.Linq;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data;
using BSFramework.Application.Code;
using System.Text;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Util;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Cache.Factory;
using BSFramework.Util.Extension;
using BSFramework.Application.Busines.MisManage;
using BSFramework.Application.Entity.MisManage;
using System.Configuration;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using BSFramework.Application.Entity.WorkMeeting;
using System.Collections;

namespace BSFramework.Busines.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class WorkmeetingBLL
    {
        private WorkmeetingIService service = new WorkmeetingService();
        private WorkOrderBLL workorder = new WorkOrderBLL();
        private IDangerAnalysisService _dangerAnalysisService;

        public WorkmeetingBLL()
        {
            _dangerAnalysisService = new DangerAnalysisService();
        }


        public IEnumerable<WorkmeetingEntity> GetAllList()
        {
            return service.GetAllList();
        }
        public int GetIndex(string deptId, DateTime? from, DateTime? to, string query)
        {
            return service.GetIndex(deptId, from, to, query);
        }
        /// <summary>
        /// 国家能源集团版本
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public int GetMeetCountries(string deptId, DateTime from, DateTime to)
        {
            return service.GetMeetCountries(deptId, from, to);

        }
        public IEnumerable<JobTemplateEntity> GetPageList(string deptcode, Pagination pagination, string queryJson)
        {
            return service.GetPageList(deptcode, pagination, queryJson);
        }

        public DataTable GetAttendanceUserData3(string UserId, string deptid, DateTime from, DateTime to, bool[] isMenu)
        {
            return service.GetAttendanceUserData3(UserId, deptid, from, to, isMenu);
        }

        public DataTable GetAttendanceExportData3(string name, string deptid, DateTime from, DateTime to, bool[] isMenu)
        {
            return service.GetAttendanceExportData3(name, deptid, from, to, isMenu);
        }
        public DataTable GetAttendanceData3(Pagination pagination, string queryJson, out decimal total, out int records, bool[] isMenu)
        {
            var queryParam = queryJson.ToJObject();
            var deptid = string.Empty;
            var name = string.Empty;
            var nowTime = DateTime.Now;
            var start = new DateTime(nowTime.Year, nowTime.Month, 1);
            var end = start.AddMonths(1);
            //deptid
            if (!queryParam["deptid"].IsEmpty())
            {
                string pdeptid = queryParam["deptid"].ToString();
                var tree = new DepartmentBLL().GetSubDepartments(pdeptid, "");
                deptid = string.Join(",", tree.Select(x => x.DepartmentId));
            }
            //name deptname
            if (!queryParam["name"].IsEmpty())
            {
                name = queryParam["name"].ToString();
            }
            // start
            if (!queryParam["Start"].IsEmpty())
            {
                start = Convert.ToDateTime(queryParam["Start"].ToString());
            }
            //end
            if (!queryParam["End"].IsEmpty())
            {
                end = Convert.ToDateTime(queryParam["End"].ToString());
            }
            total = 0;
            records = 0;
            return service.GetAttendanceData3(pagination, name, deptid, start, end, out total, out records, isMenu);
        }
        public List<AttendanceEntity> GetAttendanceData(string deptid, int year, int month)
        {
            return service.GetAttendanceData(deptid, year, month);
        }

        #region 获取数据

        public DataTable getExport(string content)
        {
            return service.getExport(content);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<WorkmeetingEntity> GetList(string[] depts, DateTime? from, DateTime? to, bool? isEvaluate, string userid, int page, int pagesize, out int total)
        {
            return service.GetList(depts, from, to, isEvaluate, userid, page, pagesize, out total);
        }

        //public WorkmeetingEntity PrepareMeeting(string deptid, DateTime date)
        //{
        //    var meeting = service.PrepareWorkMeeting(deptid, date);
        //    return meeting;
        //}

        public bool ExistQrImage(WorkmeetingEntity model)
        {
            return service.ExistQrImage(model);
        }

        public WorkmeetingEntity HasMeeting(string departmentId, DateTime date)
        {
            return service.HasMeeting(departmentId, date);
        }

        public void SaveQrImage(FileInfoEntity file)
        {
            service.SaveQrImage(file);
        }

        public string GetAttendance(string id, DateTime date)
        {
            return service.GetAttendance(id, date);
        }

        public WorkmeetingEntity GetLastMeeting(string deptId)
        {
            return service.GetLastMeeting(deptId);
        }

        public bool EnsureWorkTime(string deptId, DateTime now)
        {
            return true;
        }

        public AttendanceEntity GetMonthAttendance(string userid, int year, int month)
        {
            return service.GetMonthAttendance(userid, year, month);
        }

        public WorkmeetingEntity CreateStartMeeting(string deptid, DateTime date, DateTime? start, DateTime? end, string code)
        {
            return service.CreateStartMeeting(deptid, date, start, end, code);
        }


        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public WorkmeetingEntity GetDetail(string keyValue)
        {
            return service.GetDetail(keyValue);
        }
        public void finishwork(WorkmeetingEntity model)
        {
            service.finishwork(model);
        }
        public List<UserAttendanceEntity> GetAttendanceData2(string deptid, DateTime from, DateTime to, bool[] isMenu)
        {
            return service.GetAttendanceData2(deptid, from, to, isMenu);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingtype"></param>
        /// <returns></returns>
        public WorkmeetingEntity GetWorkMeeting(string deptid, DateTime date)
        {
            return service.GetWorkMeeting(deptid, date);
        }

        public DayAttendanceEntity GetDayAttendance(string id, DateTime date)
        {
            return service.GetDayAttendance(id, date);
        }

        public WorkmeetingEntity GetEndMeeting(string startmeetingid)
        {
            return service.GetEndMeeting(startmeetingid);
        }

        public IList<MeetingJobEntity> GetGroupJobs(string groupid)
        {
            return service.GetGroupJobs(groupid) ?? new List<MeetingJobEntity>();
        }

        public WorkmeetingEntity BuildWorkMeeting(string startmeetingid, string deptid, DateTime date, string trainingtype)
        {
            return service.BuildWorkMeeting(startmeetingid, deptid, date, trainingtype);
        }

        public UserAttendanceEntity GetMonthAttendance2(string userid, DateTime from, DateTime to)
        {
            return service.GetMonthAttendance2(userid, from, to);
        }

        public List<MeetingJobEntity> GetJobList(string UserId)
        {
            return service.GetJobList(UserId);
        }
        public int UpdateJosState(string JobId, string isFinish)
        {
            return service.UpdateJosState(JobId, isFinish);
        }

        public List<DayAttendanceEntity> GetDayAttendance2(string userid, DateTime from, DateTime to)
        {
            return service.GetDayAttendance2(userid, from, to);
        }
        public DayAttendanceEntity GetDayAttendance4(string userid, DateTime date)
        {
            return service.GetDayAttendance4(userid, date);
        }

        public void PostUserState(string id, DateTime date, DayAttendanceEntity model)
        {
            service.PostUserState(id, date, model);
        }

        public System.Collections.IList GetList(int page, int pageSize, out int total, int status, string startTime, string endTime, string bzDepart)
        {
            return service.GetList(page, pageSize, out total, status, startTime, endTime, bzDepart);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteJobTemplate(string id)
        {
            service.DeleteJobTemplate(id);
        }
        public void DeleteJobTemplateByDept(List<string> deptList)
        {
            service.DeleteJobTemplateByDept(deptList);
        }
        public void DeleteJobTemplate(List<string> dept)
        {
            service.DeleteJobTemplate(dept);
        }

        public int GetMeetingNumber(string deptid)
        {
            return service.GetMeetingNumber(deptid);
        }

        public int GetTaskNumber(string deptid)
        {
            return service.GetTaskNumber(deptid);
        }

        public decimal GetTaskPct(string deptid)
        {
            return service.GetTaskPct(deptid);
        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void ManagerSaveForm(string keyValue, WorkmeetingEntity model)
        {
            try
            {
                service.ManagerSaveForm(keyValue, model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JobTemplateEntity GetJobTemplate(string id)
        {
            return service.GetJobTemplate(id);
        }
        public List<DangerTemplateEntity> getdangertemplate(string id)
        {
            return service.getdangertemplate(id);
        }
        public DangerTemplateEntity getdangtemplateentity(string id)
        {
            return service.getdangtemplateentity(id);
        }
        public void deldangertemplateentity(string id)
        {
            service.deldangertemplateentity(id);
        }

        /// <summary>
        /// 删除班前班后会
        /// </summary>
        /// <param name="id"></param>
        public void DeltetMeeting(string id)
        {
            service.DeltetMeeting(id);
        }


        public WorkmeetingEntity AddStartMeeting(WorkmeetingEntity model)
        {
            return service.AddStartMeeting(model);
        }

        public List<UnSignRecordEntity> GetDayAttendance3(string userid, DateTime date)
        {
            return service.GetDayAttendance3(userid, date);
        }
        public void PostDelDutyPerson(DateTime date, string userid)
        {
            service.PostDelDutyPerson(date, userid);
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="meetingJobId"></param>
        /// <returns></returns>
        public JobTemplateEntity GetJobTemplateByMeetingJobId(string meetingJobId)
        {
            return service.GetJobTemplateByMeetingJobId(meetingJobId);
        }

        public string UpdateJobTemplate(JobTemplateEntity model)
        {
            var order = new WorkOrderBLL();
            var group = order.GetWorkOrderGroup(model.DeptId);
            if (group.Count() > 1)
            {
                var deptList = group.Select(x => x.departmentid).ToList();
                var ck = true;
                DepartmentService deptService = new DepartmentService();
                foreach (var item in deptList)
                {
                    var deptck = deptService.GetEntity(item);
                    if (deptck.TeamType != "01")
                    {
                        ck = false;
                    }
                }
                if (ck)
                {
                    return service.UpdateJobTemplateGroup(model, deptList);
                }
            }
            return service.UpdateJobTemplate(model);
        }

        public void PostAttendance(WorkmeetingEntity data)
        {
            service.PostAttendance(data);
        }




        public void AddEndMeeting(WorkmeetingEntity model)
        {
            foreach (var item in model.Signins)
            {
                item.SigninId = Guid.NewGuid().ToString();
                item.MeetingId = model.MeetingId;
                item.CreateDate = DateTime.Now;
            }
            foreach (var item in model.Jobs)
            {
                item.Relation.EndMeetingId = model.MeetingId;
                item.Relation.IsFinished = item.IsFinished;
            }
            service.AddEndMeeting(model);
        }

        public List<SinginUserEntity> GetSigninData(string deptid, int year, int month)
        {
            return service.GetSigninData(deptid, year, month);
        }

        public void EditEndMeeting(WorkmeetingEntity model, string userid)
        {
            service.EditEndMeeting(model);

            if (model.IsOver)
            {
                this.NoticeMonitor(model.MeetingId, userid, "1");

                var analysis = _dangerAnalysisService.GetLast(model.GroupId);
                if (analysis == null)
                {
                    analysis = new DangerAnalysisEntity()
                    {
                        AnalysisId = Guid.NewGuid().ToString(),
                        DeptId = model.GroupId,
                        DeptName = model.GroupName
                    };
                    _dangerAnalysisService.Init(analysis);
                }
                analysis.MeetingId = model.OtherMeetingId;
                analysis.MeetingDate = model.MeetingStartTime;
                analysis.MeetingTime = string.Format("{0} ~ {1}", model.MeetingStartTime.ToString("HH:mm"), model.MeetingEndTime.ToString("HH:mm"));
                analysis.MeetingName = model.MeetingCode;
                _dangerAnalysisService.Edit(analysis);

            }
        }

        public void EditStartMeeting(WorkmeetingEntity model, string userid)
        {
            var dangerbll = new DangerBLL();
            var messagebll = new MessageBLL();

            service.EditStartMeeting(model);

            var trainingtype = Config.GetValue("TrainingType");

            if (model.Jobs != null)
            {
                foreach (var item in model.Jobs)
                {
                    if (model.MeetingType == "班前会" && model.IsOver && item.NeedTrain)
                    {
                        item.GroupId = model.GroupId;

                        if (trainingtype == "人身风险预控")
                        {
                            var training = new HumanDangerTrainingEntity() { TrainingId = Guid.NewGuid().ToString(), TrainingTask = item.Job, CreateTime = DateTime.Now, CreateUserId = userid, MeetingJobId = item.Relation.MeetingJobId, DeptId = model.GroupId, TrainingPlace = item.JobAddr };
                            if (!string.IsNullOrEmpty(item.TemplateId)) training.HumanDangerId = item.TemplateId;
                            training.TrainingUsers = item.Relation.JobUsers.Select(x => new TrainingUserEntity() { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, TrainingRole = x.JobType == "ischecker" ? 1 : 0 }).ToList();
                            new HumanDangerTrainingBLL().Add(training);
                            foreach (var item1 in training.TrainingUsers)
                            {
                                messagebll.SendMessage("人身风险预控", item1.TrainingUserId.ToString());
                            }
                        }
                        else
                        {
                            var danger = dangerbll.Save(item);
                            if (danger != null)
                                messagebll.SendMessage(Config.GetValue("CustomerModel"), danger.Id);
                        }
                    }
                }
            }

            if (model.IsOver)
            {
                this.NoticeMonitor(model.MeetingId, userid, "0");
                this.AddTempJobs(model.MeetingId);


            }
        }

        public void NoticeMonitor(string id, string userid, string state)
        {
            service.NoticeMonitor(id, userid, state);
        }

        public List<JobTemplateEntity> GetDangerous(string deptId)
        {
            return service.GetDangerous(deptId);
        }

        public MeetingJobEntity AddNewJob(MeetingJobEntity model)
        {

            return service.AddNewJob(model);
        }

        public void UpdateJob2(MeetingJobEntity model, string trainingtype, string traingtype2)
        {
            service.UpdateJob2(model, trainingtype, traingtype2);
        }

        public void UpdateScoreState(string meetingJobId, string userid, string score, string isFinished)
        {
            service.UpdateScoreState(meetingJobId, userid, score, isFinished);
        }

        public void UpdateJob3(string userKey, MeetingJobEntity model)
        {
            service.UpdateJob3(userKey, model);
        }


        public dynamic GetData(int pagesize, int page, out int total, Dictionary<string, string> dict)
        {
            return service.GetData(pagesize, page, out total, dict);
        }
        public dynamic GetDataNew(int pagesize, int page, out int total, Dictionary<string, string> dict, string[] depts)
        {
            return service.GetDataNew(pagesize, page, out total, dict, depts);
        }
        /// <summary>
        /// 获取评论实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public List<ActivityEvaluateEntity> GetEntityEvaluate(string keyValue)
        {
            return service.GetEntityEvaluate(keyValue);
        }
        public WorkmeetingEntity HomeMeetingDetail(string startmeetingid)
        {
            return service.HomeMeetingDetail(startmeetingid);
        }
        public dynamic GetdeptId(Dictionary<string, string> dict)
        {
            return service.GetdeptId(dict);

        }

        private List<TreeEntity> GetDeptTreeId(string Ids, int checkMode = 0, int mode = 0)
        {
            OrganizeBLL organizeBLL = new OrganizeBLL();
            if (Ids == "0")
            { //存在机构 否则为0
                var getIds = organizeBLL.GetList().FirstOrDefault(x => x.ParentId == "0");
                if (getIds != null)
                {
                    Ids = getIds.OrganizeId;
                }
            }
            Operator user = OperatorProvider.Provider.Current();
            List<OrganizeEntity> organizedata = new List<OrganizeEntity>();
            List<DepartmentEntity> departmentdata = new List<DepartmentEntity>();
            var departmentBLL = new DepartmentBLL();
            string roleNames = user.RoleName;
            if (user.IsSystem)
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                organizedata = organizedata1.ToList();
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }
                departmentdata = departmentdata1.ToList();
            }
            else
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }

                organizedata = organizedata1.Where(t => t.OrganizeId == user.OrganizeId).OrderByDescending(x => x.CreateDate).ToList();
                departmentdata = departmentdata1.OrderBy(x => x.SortCode).ToList();
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                #region 部门
                TreeEntity tree = new TreeEntity();
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                if (item.ParentId == "0")
                {
                    tree.parentId = item.OrganizeId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                if (item.Nature == "班组")
                {
                    tree.Attribute = "Code";
                    tree.AttributeValue = item.EnCode;
                }
                if (item.Nature == "班组" || item.Nature == "部门")
                {
                    tree.showcheck = checkMode == 0 ? false : true;
                }
                treeList.Add(tree);
                #endregion
            }
            var gettreeList = new List<TreeEntity>();
            getTree(treeList, Ids, gettreeList);

            if (gettreeList.Count > 0)
            {
                var parent = treeList.FirstOrDefault(x => x.id == Ids);
                if (parent == null)
                {
                    if (organizedata.Count == 1)
                    {
                        parent = treeList.FirstOrDefault(x => x.parentId == Ids);
                    }
                }
                if (parent != null)
                {
                    var one = gettreeList.FirstOrDefault(x => x.id == parent.id);
                    if (one == null)
                    {
                        parent.parentId = "0";
                        gettreeList.Add(parent);
                    }

                }

            }
            else
            {
                var one = treeList.FirstOrDefault(x => x.id == Ids);
                gettreeList.Add(one);
                var two = treeList.FirstOrDefault(x => x.id == one.parentId);
                if (two != null)
                {
                    two.parentId = "0";
                    gettreeList.Add(two);
                }


            }

            return gettreeList.ToList();
        }
        private void getTree(List<TreeEntity> my, string id, List<TreeEntity> get)
        {
            var go = my.Where(x => x.parentId == id).ToList();

            if (go.Count > 0)
            {
                get.AddRange(go);
            }
            foreach (var item in go)
            {
                getTree(my, item.id, get);
            }

        }
        /// <summary>
        /// 获取排班后未进行班会的信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="data1"></param>
        /// <param name="total"></param>
        public void workSetmeeting(List<TreeEntity> data, DateTime start, DateTime end, DataTable modelData, out DataTable data1, out int total)
        {
            var user = OperatorProvider.Provider.Current();
            WorkOrderBLL orderbll = new WorkOrderBLL();
            var daySum = Util.Time.DiffDays(start, end);
            var query = data;
            var returnData = modelData.Clone();
            total = 0;
            returnData.Columns.Add("workset", Type.GetType("System.String"));
            data1 = returnData.Clone();
            foreach (var item in query)
            {

                if (item.Attribute != "Code")
                {
                    continue;
                }
                //去除父节点
                if (item.parentId == "0")
                {
                    continue;
                }
                var id = item.id;
                #region 排班数据
                var order = workorder.WorkOrderGet(start, start, id);
                #endregion
                //按照查询时间开始检索排班数据  默认为常白班
                for (int i = 0; i <= daySum; i++)
                {
                    var myTime = start.AddDays(i);
                    if (Util.Time.GetWeekNameOfDay(myTime) == "星期日" || Util.Time.GetWeekNameOfDay(myTime) == "星期六")
                    {
                        continue;
                    }
                    //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示
                    var ckList = (from t in modelData.AsEnumerable()
                                  where t.Field<DateTime>("meetingstarttime").ToString("yyyy-MM-dd") == myTime.ToString("yyyy-MM-dd") &
                                  t.Field<string>("groupid").ToString() == id
                                  select t).ToList();

                    if (myTime.Month != start.Month)
                    {
                        order = workorder.WorkOrderGet(myTime, myTime, id);
                    }

                    if (order.Count == 1)
                    {
                        var MonthOrder = order[0].timedata.Split(',');
                        var dayNum = myTime.Day;
                        var DayOrder = MonthOrder[dayNum - 1];
                        if (!DayOrder.Contains("无") && !DayOrder.Contains("休息"))
                        {
                            var workSet = DayOrder.Split('(')[1].Replace(")", "");
                            var getData = DayOrder.Split('(');
                            var nowTime = getData[1];
                            var dateTimeSpan = DayOrder.Split('(')[0];
                            var startSpan = dateTimeSpan.Split('-')[0];
                            var endSpan = dateTimeSpan.Split('-')[1];
                            var startTime = string.Empty;
                            var endTime = string.Empty;
                            var startHour = Convert.ToInt32(startSpan.Split(':')[0]);
                            var endHour = Convert.ToInt32(endSpan.Split(':')[0]);
                            var startMinute = Convert.ToInt32(startSpan.Split(':')[1]);
                            var endMinute = Convert.ToInt32(endSpan.Split(':')[1]);
                            if (ckList.Count() > 0)
                            {
                                var ckTable = ckList.CopyToDataTable();
                                foreach (DataRow ckRow in ckTable.Rows)
                                {
                                    //此时肯定有班前会  但不确定班后会是否存在
                                    if (!string.IsNullOrEmpty(ckRow["beforeisover"].ToString()))
                                    {
                                        if (ckRow["beforeisover"].ToString() == "未开始")
                                        {
                                            returnData.ImportRow(ckRow);
                                            total++;
                                        }

                                    }

                                }
                                foreach (DataRow getRow in returnData.Rows)
                                {
                                    getRow["workset"] = workSet;
                                    data1.ImportRow(getRow);
                                }
                                returnData.Clear();

                            }
                            else
                            {
                                //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示
                                var BaseTable = returnData.Clone();
                                DataRow rows = returnData.NewRow();
                                if (query.Count > 1)
                                {
                                    rows["team"] = query.FirstOrDefault(x => x.id == item.parentId).text;

                                }
                                else
                                {
                                    rows["team"] = query.FirstOrDefault(x => x.id == item.id).text;

                                }

                                rows["department"] = item.text;
                                rows["meetingstarttime"] = myTime;
                                rows["afterisover"] = "未进行";
                                rows["beforeisover"] = "未进行";
                                rows["workset"] = workSet;
                                returnData.Rows.Add(rows);
                                foreach (DataRow getRow in returnData.Rows)
                                {
                                    data1.ImportRow(getRow);
                                }
                                returnData.Clear();
                                total++;
                            }
                            //跨天
                            if (startHour > endHour || (startHour == endHour && startMinute > endMinute))
                            {
                                var nextDay = myTime.AddDays(1);
                                //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示
                                var ckNextList = (from t in modelData.AsEnumerable()
                                                  where t.Field<DateTime>("meetingstarttime").ToString("yyyy-MM-dd") == nextDay.ToString("yyyy-MM-dd") &
                                                  t.Field<string>("groupid").ToString() == id
                                                  select t).ToList();
                                var dayNextNum = nextDay.Day;
                                var DayNextOrder = MonthOrder[dayNextNum - 1];
                                if (ckList.Count() > 0)
                                {
                                    var ckTable = ckList.CopyToDataTable();
                                    foreach (DataRow ckRow in ckTable.Rows)
                                    {
                                        //此时肯定有班前会  但不确定班后会是否存在
                                        if (!string.IsNullOrEmpty(ckRow["beforeisover"].ToString()))
                                        {
                                            if (ckRow["beforeisover"].ToString() == "未开始")
                                            {
                                                returnData.ImportRow(ckRow);
                                                total++;
                                            }

                                        }

                                    }
                                    foreach (DataRow getRow in returnData.Rows)
                                    {
                                        getRow["workset"] = workSet;
                                        data1.ImportRow(getRow);
                                    }
                                    returnData.Clear();

                                }
                                else
                                {
                                    //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示
                                    var BaseTable = returnData.Clone();
                                    DataRow rows = returnData.NewRow();
                                    if (query.Count > 1)
                                    {
                                        rows["team"] = query.FirstOrDefault(x => x.id == item.parentId).text;

                                    }
                                    else
                                    {
                                        rows["team"] = query.FirstOrDefault(x => x.id == item.id).text;

                                    }

                                    rows["department"] = item.text;
                                    rows["meetingstarttime"] = nextDay;
                                    rows["afterisover"] = "未进行";
                                    rows["beforeisover"] = "未进行";
                                    rows["workset"] = workSet;
                                    returnData.Rows.Add(rows);
                                    foreach (DataRow getRow in returnData.Rows)
                                    {
                                        data1.ImportRow(getRow);
                                    }
                                    returnData.Clear();
                                    total++;
                                }
                            }
                        }

                    }


                }
                continue;




            }

        }


        /// <summary>
        /// 获取排班后未进行班会的信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="data1"></param>
        /// <param name="total"></param>
        public DataTable GetIsMeet(List<DepartmentEntity> dept, DateTime start, DateTime end, List<WorkmeetingEntity> meet)
        {
            var user = OperatorProvider.Provider.Current();
            WorkOrderBLL orderbll = new WorkOrderBLL();
            var daySum = Util.Time.DiffDays(start, end);
            DepartmentBLL deptBll = new DepartmentBLL();
            DataTable IsMeet = new DataTable();
            IsMeet.Columns.Add("aftermeetingid", typeof(string));
            IsMeet.Columns.Add("team", typeof(string));
            IsMeet.Columns.Add("department", typeof(string));
            IsMeet.Columns.Add("workset", typeof(string));
            IsMeet.Columns.Add("afterisover", typeof(string));
            IsMeet.Columns.Add("beforeisover", typeof(string));
            IsMeet.Columns.Add("meetingstarttime", typeof(string));
            foreach (var item in dept)
            {
                //去除父节点
                if (item.ParentId == "0")
                {
                    continue;
                }
                var id = item.DepartmentId;
                #region 排班数据
                var order = workorder.WorkOrderGet(start, start, id);
                #endregion
                //按照查询时间开始检索排班数据  默认为常白班
                for (int i = 0; i <= daySum; i++)
                {
                    var myTime = start.AddDays(i);
                    //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示


                    if (myTime.Month != start.Month)
                    {
                        order = workorder.WorkOrderGet(myTime, myTime, id);
                    }

                    if (order.Count == 1)
                    {
                        var MonthOrder = order[0].timedata.Split(',');
                        var dayNum = myTime.Day;
                        var DayOrder = MonthOrder[dayNum - 1];
                        if (!DayOrder.Contains("无") && !DayOrder.Contains("休息"))
                        {
                            var workSet = DayOrder.Split('(')[1].Replace(")", "");
                            var getData = DayOrder.Split('(');
                            var nowTime = getData[1];
                            var dateTimeSpan = DayOrder.Split('(')[0];
                            var startSpan = dateTimeSpan.Split('-')[0];
                            var endSpan = dateTimeSpan.Split('-')[1];
                            var startTime = string.Empty;
                            var endTime = string.Empty;
                            var startHour = Convert.ToInt32(startSpan.Split(':')[0]);
                            var endHour = Convert.ToInt32(endSpan.Split(':')[0]);
                            var startMinute = Convert.ToInt32(startSpan.Split(':')[1]);
                            var endMinute = Convert.ToInt32(endSpan.Split(':')[1]);
                            var ck = meet.Where(x => x.MeetingStartTime.ToString("yyyy-MM-dd") == myTime.ToString("yyyy-MM-dd") && x.GroupId == id && x.MeetingType == "班前会").OrderBy(x => x.MeetingStartTime).FirstOrDefault();

                            //(from t in meet
                            //          where t.MeetingStartTime.ToString("yyyy-MM-dd") == myTime.ToString("yyyy-MM-dd") &
                            //          t.GroupId == id
                            //          select t).ToList();
                            if (ck != null)
                            {
                                DataRow row = IsMeet.NewRow();
                                var pdept = deptBll.GetEntity(item.ParentId);
                                row["aftermeetingid"] = ck.MeetingId;
                                row["team"] = pdept.FullName;
                                row["department"] = ck.GroupName;
                                row["workset"] = workSet;
                                row["afterisover"] = "已进行";
                                if (string.IsNullOrEmpty(ck.OtherMeetingId))
                                {
                                    row["beforeisover"] = "未进行";
                                }
                                else
                                {
                                    row["beforeisover"] = "已进行";
                                }

                                row["meetingstarttime"] = ck.MeetingStartTime.ToString("yyyy-MM-dd");
                                IsMeet.Rows.Add(row);
                            }
                            else
                            {
                                //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示
                                DataRow row = IsMeet.NewRow();
                                var pdept = deptBll.GetEntity(item.ParentId);
                                row["aftermeetingid"] = "";
                                row["team"] = pdept.FullName;
                                row["department"] = item.FullName;
                                row["workset"] = workSet;
                                row["afterisover"] = "未进行";
                                row["beforeisover"] = "未进行";
                                row["meetingstarttime"] = myTime.ToString("yyyy-MM-dd");
                                IsMeet.Rows.Add(row);
                            }
                            ////跨天
                            //if (startHour > endHour || (startHour == endHour && startMinute > endMinute))
                            //{
                            //    var nextDay = myTime.AddDays(1);
                            //    //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示
                            //    var ckNextList = (from t in meet
                            //                      where t.MeetingStartTime.ToString("yyyy-MM-dd") == myTime.ToString("yyyy-MM-dd") &
                            //                      t.GroupId == id
                            //                      select t).ToList();
                            //    var dayNextNum = nextDay.Day;
                            //    var DayNextOrder = MonthOrder[dayNextNum - 1];
                            //    if (ckList != null)
                            //    {


                            //    }
                            //    else
                            //    {
                            //        //存在排班数据  不存在班会数据 则 未进行班会  获取一条基础数据用于展示

                            //    }
                            //}
                        }

                    }


                }
                continue;




            }
            return IsMeet;
        }

        /// <summary>
        /// 今日工作各风险等级的统计
        /// </summary>
        /// <param name="searchDeptId">要搜索的部门的Id</param>
        /// <param name="now"></param>
        /// <returns></returns>
        public Dictionary<string, int> TodayWorkStatistics(string DeptStr, DateTime now)
        {
            return service.TodayWorkStatistics(DeptStr, now);
        }

        public void AddLongJobs(List<MeetingJobEntity> jobs)
        {
            service.AddLongJobs(jobs);
        }

        public List<MeetingJobEntity> FindLongJobs(string departmentId, DateTime date)
        {
            return service.FindLongJobs(departmentId, date);
        }

        public void AddTodayJobs(List<MeetingJobEntity> jobs)
        {
            service.AddTodayJobs(jobs);
        }

        /// <summary>
        /// 临时任务
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<MeetingJobEntity> FindMeetingJobs2(string departmentId, DateTime date)
        {
            return service.FindMeetingJobs2(departmentId, date);
        }

        public List<MeetingJobEntity> GetMeetingJobs(string meetingId)
        {
            return service.GetMeetingJobs(meetingId);
        }

        public List<MeetingJobEntity> FindMeetingJobs(string departmentId, DateTime today)
        {
            return service.FindMeetingJobs(departmentId, today);
        }

        public bool ExistJob(JobTemplateEntity entity)
        {
            return service.ExistJob(entity);
        }

        public List<UnSignRecordEntity> GetDutyData(string deptId, DateTime from, DateTime to)
        {
            return service.GetDutyData(deptId, from, to);
        }

        public WorkmeetingEntity GetEntity(string startMeetingId)
        {
            return service.GetEntity(startMeetingId);
        }

        public MeetingJobEntity GetJobDetail(string jobId)
        {
            return service.GetJobDetail(jobId);
        }

        public List<DepartmentEntity> GetOverView(string deptid)
        {
            return service.GetOverView(deptid);
        }

        public List<JobTemplateEntity> GetMeasures(string deptId)
        {
            return service.GetMeasures(deptId);
        }

        public List<JobTemplateEntity> Find(string query, string deptid, int limit)
        {
            return service.Find(query, deptid, limit);
        }

        public List<JobTemplateEntity> GetBaseData(string deptid, string empty, string jobplantype, int pagesize, int page, out int total)
        {
            return service.GetBaseData(deptid, empty, jobplantype, pagesize, page, out total);
        }
        public List<JobTemplateEntity> GetBaseDataNew(string content, int pagesize, int page, out int total)
        {
            return service.GetBaseDataNew(content, pagesize, page, out total);
        }

        public void ReBuildMeeting(string meetingId)
        {
            service.ReBuildMeeting(meetingId);
        }

        public void AddJobTemplates(List<JobTemplateEntity> templates)
        {
            foreach (var item in templates)
            {
                // item.JobId = Guid.NewGuid().ToString();
                item.DangerType = "job";
            }
            service.AddJobTemplates(templates);
        }

        public void AddDangerTemplates(List<DangerTemplateEntity> templates)
        {
            service.AddDangerTemplates(templates);
        }

        public void AddMeasure(JobTemplateEntity entity)
        {
            service.AddMeasure(entity);
        }

        public void DeleteMeasure(string id)
        {
            service.DeleteMeasure(id);
        }
        #endregion

        public MeetingJobEntity GetJobDetail(string id, string meetingjobid, string trainingtype)
        {
            return service.GetJobDetail(id, meetingjobid, trainingtype);
        }

        public List<UnSignRecordEntity> GetDutyPerson(string deptid, DateTime date)
        {
            return service.GetDutyPerson(deptid, date);
        }

        public MeetingJobEntity PostJob(MeetingJobEntity job)
        {
            return service.PostJob(job);
        }

        public List<MeetingSigninEntity> GetJobUser(string deptid)
        {
            return service.GetJobUser(deptid);
        }

        public void UpdateJob(MeetingJobEntity job)
        {
            service.UpdateJob(job);
        }
        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="id"></param>
        public void CancelJob(string id, string meetingjobid)
        {

            service.CancelJob(id, meetingjobid);

        }
        /// <summary>
        /// 变更任务
        /// </summary>
        /// <param name="job"></param>
        public void ChangeJob(MeetingJobEntity job)
        {

            StringBuilder remark = new StringBuilder();

            ////获取之前的任务
            var oldJob = service.GetJobDetail(job.JobId, job.Relation.MeetingJobId, null);


            //if (!string.IsNullOrEmpty(job.Remark))
            //    remark.Append("\r\n");
            //remark.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            //if (oldJob.Job.TrimEnd() != job.Job.TrimEnd())
            //    remark.AppendFormat(" 工作：{0}  变更为：{1}  ", oldJob.Job, job.Job);

            //BuildPersons(job, oldJob, remark);

            //if (oldJob.Measure != job.Measure)
            //    remark.AppendFormat(" 防范措施：{0} 变更为：{1} ", oldJob.Measure, job.Measure);

            //if (oldJob.Dangerous != job.Dangerous)
            //    remark.AppendFormat(" 危险因素：{0} 变更为：{1} ", oldJob.Dangerous, job.Dangerous);

            //if ((oldJob.StartTime.ToString("HH:mm") != job.StartTime.ToString("HH:mm")) || (oldJob.EndTime.ToString("HH:mm") != job.EndTime.ToString("HH:mm")))
            //    remark.AppendFormat(" 计划时间：{0} 变更为：{1} ", oldJob.StartTime.ToString("HH:mm") + " - " + oldJob.EndTime.ToString("HH:mm"), job.StartTime.ToString("HH:mm") + " - " + job.EndTime.ToString("HH:mm"));




            if (oldJob.Job.TrimEnd() != job.Job.TrimEnd())
                remark.AppendFormat(" 工作：{0}  变更为：{1}  ", oldJob.Job, job.Job);

            BuildPersons(job, oldJob, remark);

            if (oldJob.Measure != job.Measure)
                remark.AppendFormat(" 防范措施：{0} 变更为：{1} ", oldJob.Measure, job.Measure);

            if (oldJob.Dangerous != job.Dangerous)
                remark.AppendFormat(" 危险因素：{0} 变更为：{1} ", oldJob.Dangerous, job.Dangerous);

            if ((oldJob.StartTime.ToString("yyyy/M/d H:mm") != job.StartTime.ToString("yyyy/M/d H:mm")) || (oldJob.EndTime.ToString("yyyy/M/d H:mm") != job.EndTime.ToString("yyyy/M/d H:mm")))
            {
                oldJob.StartTime = job.StartTime;
                oldJob.EndTime = job.EndTime;
                remark.AppendFormat(" 计划时间：{0} 变更为：{1} ", oldJob.StartTime.ToString("yyyy/M/d H:mm") + " - " + oldJob.EndTime.ToString("yyyy/M/d H:mm"), job.StartTime.ToString("yyyy/M/d H:mm") + " - " + job.EndTime.ToString("yyyy/M/d H:mm"));
            }

            if (!string.IsNullOrEmpty(job.Remark))
            {
                remark.Insert(0, "\r\n");
                remark.Insert(2, string.Format("{0} ", DateTime.Now.ToString("yyyy/M/d H:mm")));
            }
            else
            {
                remark.Insert(0, string.Format("{0} ", DateTime.Now.ToString("yyyy/M/d H:mm")));
            }



            oldJob.Remark += remark.ToString();
            oldJob.Job = job.Job;
            oldJob.TaskType = job.TaskType;
            oldJob.RiskLevel = job.RiskLevel;
            oldJob.Relation.JobUsers = job.Relation.JobUsers;
            oldJob.Relation.JobUserId = string.Join(",", job.Relation.JobUsers.OrderBy(x => x.JobType).Select(x => x.UserId));
            oldJob.Relation.JobUser = string.Join(",", job.Relation.JobUsers.OrderBy(x => x.JobType).Select(x => x.UserName));
            oldJob.Dangerous = job.Dangerous;
            oldJob.Measure = job.Measure;
            oldJob.Description = job.Description;
            oldJob.DangerousList = job.DangerousList;

            service.ChangeJob(oldJob);
        }

        public List<string> Query(string key, string deptid, int pagesize, int pageindex, out int total)
        {
            return service.Query(key, deptid, pagesize, pageindex, out total);
        }

        public System.Collections.IList QueryNew(string key, string deptId, int pageSize, int pageIndex, out int total)
        {
            return service.QueryNew(key, deptId, pageSize, pageIndex, out total);
        }

        /// <summary>
        /// 整理工作任务变更时，对人员的更变，包括新增加的，和删除的
        /// </summary>
        private void BuildPersons(MeetingJobEntity job, MeetingJobEntity oldJob, StringBuilder remark)
        {

            //job.NewPersons = new List<JobUserEntity>();
            //job.DeletePersons = new List<JobUserEntity>();

            //if (job.Relation != null && job.Relation.JobUsers != null)
            //{
            //    foreach (var item in job.Relation.JobUsers)
            //    {
            //        item.MeetingJobId = job.Relation.MeetingJobId;

            //        //没有jobuserId 表示新增
            //        if (string.IsNullOrEmpty(item.JobUserId))
            //        {
            //            item.JobUserId = Guid.NewGuid().ToString();
            //            job.NewPersons.Add(item);
            //        }
            //    }
            //}

            //对比工作负责人是否变更
            var checker = job.Relation.JobUsers.Where(s => s.JobType == "ischecker").FirstOrDefault();
            var oldChecker = oldJob.Relation.JobUsers.Where(s => s.JobType == "ischecker").FirstOrDefault();

            if (checker != null && oldChecker != null)
            {
                if (checker.UserId != oldChecker.UserId)
                {
                    remark.AppendFormat(" 工作负责人：{0} 变更为：{1} ", oldChecker.UserName, checker.UserName);
                }
            }

            //对比工作组成员是否变更 
            var doPerson = string.Join(",", job.Relation.JobUsers.Where(s => s.JobType == "isdoperson").OrderBy(o => o.UserName).Select(s => s.UserName).ToArray());
            var oldDoPerson = string.Join(",", oldJob.Relation.JobUsers.Where(s => s.JobType == "isdoperson").OrderBy(o => o.UserName).Select(s => s.UserName).ToArray());

            if (doPerson != oldDoPerson)
            {
                remark.AppendFormat(" 工作组成员：{0} 变更为：{1} ", oldDoPerson, doPerson);
            }
        }

        public List<MeetingJobEntity> GetDeptJobs(string deptid, int pagesize, int pageindex, out int total)
        {
            return service.GetDeptJobs(deptid, pagesize, pageindex, out total);
        }

        public void AddDangerous(JobTemplateEntity entity)
        {
            service.AddDangerous(entity);
        }

        public List<JobUserEntity> GetJobUsers(string jobid, string meetingjobid)
        {
            return service.GetJobUsers(jobid, meetingjobid);
        }

        public void PostScore(List<JobUserEntity> models)
        {
            service.PostScore(models);
        }

        public IEnumerable<MeetingJobEntity> GetJobs(string userid, string year, string month, string bzid, string isfinish)
        {
            return service.GetJobs(userid, year, month, bzid, isfinish);
        }

        public List<MeetingJobEntity> GetMyJobs(string userid, DateTime date)
        {
            return service.GetMyJobs(userid, date);
        }

        public void DeleteJob(MeetingAndJobEntity job)
        {
            service.DeleteJob(job);
        }

        public List<MeetingSigninEntity> Signin(List<MeetingSigninEntity> signins)
        {
            return service.Signin(signins);
        }

        public List<MeetingSigninEntity> SyncState(List<MeetingSigninEntity> signins)
        {
            return service.SyncState(signins);
        }

        public void Score(MeetingJobEntity job)
        {
            service.Score(job);
        }

        public void PostVideo(List<FileInfoEntity> files)
        {
            service.PostVideo(files);
        }

        public void OverMeeting(string meetingid, string userid, DateTime date)
        {
            var trainingtype = Config.GetValue("TrainingType");
            service.OverMeeting(meetingid, date, trainingtype, userid);

            var meeting = service.GetDetail(meetingid);
            if (meeting.MeetingType == "班前会")
            {
                var dangerbll = new DangerBLL();
                var messagebll = new MessageBLL();


                foreach (var item in meeting.Jobs)
                {
                    item.CreateDate = DateTime.Now;
                    item.CreateUserId = userid;

                    if (string.IsNullOrEmpty(item.JobId))
                    {
                        item.JobId = Guid.NewGuid().ToString();
                    }

                    if (item.NeedTrain)
                    {
                        item.GroupId = meeting.GroupId;

                        if (trainingtype == "人身风险预控")
                        {
                            //var training = new HumanDangerTrainingEntity() { TrainingId = Guid.NewGuid(), TrainingTask = item.Job, CreateTime = DateTime.Now, CreateUserId = userid, MeetingJobId = item.Relation.MeetingJobId, DeptId = meeting.GroupId, TrainingPlace = item.JobAddr, No = item.TicketCode };
                            //if (!string.IsNullOrEmpty(item.TemplateId)) training.HumanDangerId = Guid.Parse(item.TemplateId);
                            //training.TrainingUsers = item.Relation.JobUsers.Select(x => new TrainingUserEntity() { TrainingUserId = Guid.NewGuid(), UserId = x.UserId, UserName = x.UserName, TrainingRole = x.JobType == "ischecker" ? 1 : 0 }).ToList();
                            //new HumanDangerTrainingBLL().Add(training);
                            //foreach (var item1 in training.TrainingUsers)
                            //{
                            //    messagebll.SendMessage("人身风险预控", item1.TrainingUserId.ToString());
                            //}
                        }
                        else
                        {
                            //var danger = dangerbll.Save(item);
                            //if (danger != null)
                            //    messagebll.SendMessage(Config.GetValue("CustomerModel"), danger.Id);
                        }

                    }
                }

                this.AddTempJobs(meetingid);


            }
            else
            {
                var startmeeting = service.GetEntity(meeting.OtherMeetingId);

                var analysis = _dangerAnalysisService.GetLast(meeting.GroupId);
                if (analysis == null)
                {
                    analysis = new DangerAnalysisEntity()
                    {
                        AnalysisId = Guid.NewGuid().ToString(),
                        DeptId = meeting.GroupId,
                        DeptName = meeting.GroupName
                    };
                    _dangerAnalysisService.Init(analysis);
                }
                analysis.MeetingId = meeting.OtherMeetingId;
                analysis.MeetingDate = meeting.MeetingStartTime;
                analysis.MeetingTime = string.Format("{0} ~ {1}", startmeeting.MeetingStartTime.ToString("HH:mm"), meeting.MeetingEndTime.ToString("HH:mm"));
                analysis.MeetingName = meeting.MeetingCode;
                _dangerAnalysisService.Edit(analysis);
            }

            this.NoticeMonitor(meetingid, userid, meeting.MeetingType == "班前会" ? "0" : "1");

        }

        private void AddTempJobs(string meetingid)
        {
            service.AddTempJobs(meetingid);
        }

        public string StartEndMeeting(WorkmeetingEntity meeting)
        {
            return service.StartEndMeeting(meeting);
        }

        public WorkmeetingEntity UpdateRemark(WorkmeetingEntity data)
        {
            return service.UpdateRemark(data);
        }

        public List<UnSignRecordEntity> GetDutyPerson(string id)
        {
            return service.GetDutyPerson(id);
        }

        public void PostDutyPerson(List<WorkmeetingEntity> list)
        {
            service.PostDutyPerson(list);
        }
        public List<UserInfoEntity> GetBeOnDutyStaffInfo()
        {
            return service.GetBeOnDutyStaffInfo();
        }
        public List<PeopleEntity> GetMeetAbstractInfo(string[] bzId)
        {
            return service.GetMeetAbstractInfo(bzId);
        }

        public WorkmeetingEntity StartMeeting(WorkmeetingEntity meeting, DateTime date, string meetingperson)
        {
            return service.StartMeeting(meeting, date, meetingperson);
        }

        public void PostMonitorJob(MeetingJobEntity job)
        {
            service.PostMonitorJob(job);
        }

        public void UpdateMonitorJob(MeetingJobEntity model)
        {
            service.UpdateMonitorJob(model);
        }

        public void savedangertemplateentity(DangerTemplateEntity entity)
        {
            service.savedangertemplateentity(entity);
        }

        public List<MeetingJobEntity> GetJobHistory(MeetingJobEntity meetingJobEntity)
        {
            return service.GetJobHistory(meetingJobEntity);
        }

        public void UpdateState(string meetingjobid, string state, string trainingtype)
        {
            service.UpdateState(meetingjobid, state, trainingtype);
        }

        public List<MeetingSigninEntity> GetDefaultSigns(string deptid)
        {
            return service.GetDefaultSigns(deptid);
        }

        public void SetDefaultSigns(List<MeetingSigninEntity> data)
        {
            service.SetDefaultSigns(data);
        }

        public List<MeetingEntity> GetMeetingList(string[] ary_deptid, string userid, bool? isEvaluate, DateTime? from, DateTime? to, int pagesize, int page, out int total)
        {
            return service.GetMeetingList(ary_deptid, userid, isEvaluate, from, to, pagesize, page, out total);
        }

        public List<Meeting2Entity> GetData2(string[] deptid, int pagesize, int pageindex, out int total)
        {
            return service.GetData2(deptid, pagesize, pageindex, out total);
        }

        public List<WorkmeetingEntity> GetList(string[] depts, string userId, DateTime? begin, DateTime? end, string appraise, bool v2, int rows, int page, out int total)
        {
            return service.GetList(depts, userId, begin, end, appraise, v2, rows, page, out total);
        }

        public void UpdateJobs(List<MeetingJobEntity> jobs)
        {
            service.UpdateJobs(jobs);
        }

        public bool IsEndMeetingOver(string deptId, DateTime now)
        {
            return service.IsEndMeetingOver(deptId, now);
        }

        public JobTemplateEntity Detail(string deptid, string data)
        {
            return service.Detail(deptid, data);
        }

        public List<HumanDangerTrainingEntity> HumanDangerMeeting(WorkmeetingEntity meeting, UserEntity user)
        {

            #region 生成人生风险预控
            //生成人身风险预控
            var HumanDangerList = new List<HumanDangerTrainingEntity>();
            if (meeting.MeetingType == "班前会")
            {
                var dept = new DepartmentBLL();
                var userDept = dept.GetEntity(user.DepartmentId);
                if (userDept.TeamType == "02")
                {
                    //所有工作票id
                    var TicketIdList = new List<string>();
                    foreach (var item in meeting.Jobs)
                    {
                        if (string.IsNullOrEmpty(item.TicketId))
                        {
                            continue;
                        }
                        var TicketIds = item.TicketId.Split('|');
                        foreach (var TicketId in TicketIds)
                        {
                            if (!string.IsNullOrEmpty(TicketId))
                            {
                                TicketIdList.Add(TicketId);
                            }
                        }
                    }
                    //获取班组所有工作票
                    int TicketTotal = 0;
                    var TicketBLL = new TicketBLL();
                    var userList = new List<UserEntity>();
                    string ticketStr = ConfigurationManager.AppSettings["Ticket"].ToString();
                    List<TicketEntity> TicketList;
                    switch (ticketStr)
                    {
                        case "红雁池":
                            TicketList = TicketBLL.GetList(userDept.FullName, null, null, null, "开工", false, null, 5000, 1, out TicketTotal, ticketStr);
                            break;
                        default:
                            TicketList = TicketBLL.GetList(userDept.FullName, null, null, null, "已许可", false, null, 5000, 1, out TicketTotal, ticketStr);
                            break;
                    }

                    //获取未选择的工作票
                    var toDoTicket = TicketList.Where(x => !TicketIdList.Contains(x.TicketId)).ToList();
                    if (toDoTicket.Count > 0)
                    {
                        var userBLL = new UserBLL();
                        userList = userBLL.GetList().Where(x => x.DepartmentId == user.DepartmentId).ToList();
                    }
                    HumanDangerTrainingBLL HumanDangerbll = new HumanDangerTrainingBLL();
                    foreach (var item in toDoTicket)
                    {
                        var HumanDangerEntity = new HumanDangerTrainingEntity();
                        HumanDangerEntity.DeptId = user.DepartmentId;
                        HumanDangerEntity.DeptName = user.DepartmentName;
                        HumanDangerEntity.No = item.TicketCode;
                        HumanDangerEntity.TrainingTask = item.Content;
                        HumanDangerEntity.TrainingPlace = item.Place;
                        HumanDangerEntity.TrainingId = Guid.NewGuid().ToString();
                        HumanDangerEntity.CreateTime = DateTime.Now;
                        HumanDangerEntity.CreateUserId = user.UserId;
                        HumanDangerEntity.TicketId = item.TicketId;
                        var TrainingUserList = new List<TrainingUserEntity>();
                        var HumanDangerUser = userList.FirstOrDefault(x => x.RealName == item.DutyPerson);
                        if (!string.IsNullOrEmpty(item.DutyPerson2))
                        {
                            HumanDangerUser = userList.FirstOrDefault(x => x.RealName == item.DutyPerson2);
                        }
                        if (HumanDangerUser != null)
                        {
                            var TrainingUser = new TrainingUserEntity();
                            TrainingUser.TrainingRole = 1;
                            TrainingUser.UserId = HumanDangerUser.UserId;
                            TrainingUser.UserName = HumanDangerUser.RealName;
                            TrainingUserList.Add(TrainingUser);
                        }
                        if (!string.IsNullOrEmpty(item.WorkMate))
                        {
                            var todoUsers = item.WorkMate.Split(' ');
                            if (item.WorkMate.Contains(","))
                            {
                                todoUsers = item.WorkMate.Split(',');
                            }
                            if (item.WorkMate.Contains("、"))
                            {
                                todoUsers = item.WorkMate.Split('、');
                            }
                            foreach (var todoUser in todoUsers)
                            {
                                if (!string.IsNullOrEmpty(todoUser))
                                {
                                    var todoUserEntity = userList.FirstOrDefault(x => x.RealName == todoUser);
                                    if (todoUserEntity != null)
                                    {
                                        var TrainingUser = new TrainingUserEntity();
                                        TrainingUser.TrainingRole = 0;
                                        TrainingUser.UserId = todoUserEntity.UserId;
                                        TrainingUser.UserName = todoUserEntity.RealName;
                                        TrainingUserList.Add(TrainingUser);
                                    }
                                }
                            }
                        }

                        HumanDangerEntity.TrainingUsers = TrainingUserList;
                        HumanDangerEntity.TrainingUsers.ForEach(x => x.TrainingUserId = Guid.NewGuid().ToString());
                        HumanDangerbll.Add(HumanDangerEntity);
                        HumanDangerList.Add(HumanDangerEntity);
                    }
                }
            }
            return HumanDangerList;
            #endregion
        }

        /// <summary>
        /// 分页获取工作任务
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public object GetJobPagedList(Pagination pagination, string queryJson)
        {
            return service.GetJobPagedList(pagination, queryJson);
        }

        #region 工时管理
        public IList GetJobHourPagedList(string startTime, string endTime, string keyWord, string deptCode, int pageIndex, int pageSize, out int total)
        {
            return service.GetJobHourPagedList(startTime, endTime, keyWord, deptCode, pageIndex, pageSize, out total);
        }

        public void SaveTaskHour(List<JobUserEntity> userentity)
        {
            service.SaveTaskHour(userentity);
        }

        public WorkmeetingEntity Get(string id)
        {
            return service.Get(id);
        }
        #endregion
    }
}
