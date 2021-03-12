using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PeopleManage;
using System.Collections;

namespace BSFramework.IService.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public interface WorkmeetingIService
    {
        IEnumerable<WorkmeetingEntity> GetAllList();
        WorkmeetingEntity PrepareWorkMeeting(string deptid, DateTime date);

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<WorkmeetingEntity> GetList(string[] depts, DateTime? from, DateTime? to, bool? isEvaluate, string userid, int page, int pagesize, out int total);
        DataTable getExport(string content);
        List<AttendanceEntity> GetAttendanceData(string deptid, int year, int month);
        List<UserAttendanceEntity> GetAttendanceData2(string deptid, DateTime from, DateTime to, bool[] isMenu);
        DataTable GetAttendanceData3(Pagination pagination, string name, string deptid, DateTime from, DateTime to, out decimal total, out int records, bool[] isMenu);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="query"></param>
        int GetIndex(string deptId, DateTime? from, DateTime? to, string name);
        /// <summary>
        /// 国家能源集团版本
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        int GetMeetCountries(string deptId, DateTime from, DateTime to);
        /// <summary>
        /// 获取实体
        /// </summary> 
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        WorkmeetingEntity GetDetail(string keyValue);

        List<MeetingJobEntity> GetJobList(string UserId);

        List<DangerTemplateEntity> getdangertemplate(string id);
        int UpdateJosState(string JobId, string isFinish);
        System.Collections.IList GetList(int page, int pageSize, out int total, int status, string startTime, string endTime, string bzDepart);
        int Count(string[] depts, DateTime start, DateTime end);
        bool ExistQrImage(WorkmeetingEntity model);
        void SaveQrImage(FileInfoEntity file);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        string GetAttendance(string id, DateTime date);
        AttendanceEntity GetMonthAttendance(string userid, int year, int month);
        WorkmeetingEntity GetLastMeeting(string deptId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingtype"></param>
        /// <returns></returns>
        WorkmeetingEntity GetStartMeeting(string deptid, string meetingtype, DateTime date);
        WorkmeetingEntity HasMeeting(string departmentId, DateTime date);

        ///// <summary>
        ///// 保存表单（新增、修改）
        ///// </summary>
        ///// <param name="keyValue">主键值</param>
        ///// <param name="entity">实体对象</param>
        ///// <returns></returns>
        //void SaveForm(string keyValue, WorkmeetingEntity entity);
        WorkmeetingEntity GetWorkMeeting(string deptid, DateTime date);
        WorkmeetingEntity CreateStartMeeting(string deptid, DateTime date, DateTime? start, DateTime? end, string code);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startmeetingid"></param>
        /// <returns></returns>
        WorkmeetingEntity GetEndMeeting(string startmeetingid);

        DayAttendanceEntity GetDayAttendance(string id, DateTime date);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        IList<MeetingJobEntity> GetGroupJobs(string groupid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        MeetingJobEntity AddNewJob(MeetingJobEntity entity);
        WorkmeetingEntity BuildWorkMeeting(string startmeetingid, string deptid, DateTime date, string trainingtype);

        /// <summary>
        /// 变更任务
        /// </summary>
        /// <param name="job"></param>
        void ChangeJob(MeetingJobEntity job);
        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="Id"></param>
        void CancelJob(string Id, string meetingjobid);

        dynamic GetData(int pagesize, int page, out int total, Dictionary<string, string> dict);
        dynamic GetDataNew(int pagesize, int page, out int total, Dictionary<string, string> dict, string[] depts);
        dynamic GetdeptId(Dictionary<string, string> dict);
        UserAttendanceEntity GetMonthAttendance2(string userid, DateTime from, DateTime to);
        /// <summary>
        /// 获取缺勤  绩效管理v2.0
        /// </summary>
        /// <param name="user"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        List<AttendanceTypeEntity> GetMonthAttendance2(List<PeopleEntity> user, DateTime from, DateTime to);

        /// <summary>
        /// 获取评论实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        List<ActivityEvaluateEntity> GetEntityEvaluate(string keyValue);
        List<DepartmentEntity> GetOverView(string deptid);
        void PostUserState(string id, DateTime date, DayAttendanceEntity model);
        List<DayAttendanceEntity> GetDayAttendance2(string userid, DateTime from, DateTime to);
        List<DayAttendanceEntity> GetDayAttendance2(List<PeopleEntity> user, DateTime from, DateTime to, string deptid);
        DayAttendanceEntity GetDayAttendance4(string userid, DateTime date);
        List<JobTemplateEntity> Find(string query, string deptid, int limit);
        WorkmeetingEntity AddStartMeeting(WorkmeetingEntity model);
        void AddEndMeeting(WorkmeetingEntity model);
        List<JobTemplateEntity> GetBaseData(string deptid, string empty, string jobplantype, int pagesize, int page, out int total);
        void PostDelDutyPerson(DateTime date, string userid);
        List<JobTemplateEntity> GetBaseDataNew(string content, int pagesize, int page, out int total);
        void AddJobTemplates(List<JobTemplateEntity> templates);
        void AddMeasure(JobTemplateEntity entity);
        List<JobTemplateEntity> GetDangerous(string deptId);
        List<JobTemplateEntity> GetMeasures(string deptId);
        void DeleteJobTemplate(string id);
        void DeleteJobTemplate(List<string> dept);
        void DeleteJobTemplateByDept(List<string> dept);

        void DeleteMeasure(string id);

        #endregion

        MeetingJobEntity GetJobDetail(string id, string meetingjobid, string trainingtype);

        MeetingJobEntity PostJob(MeetingJobEntity job);

        List<MeetingSigninEntity> GetJobUser(string deptid);

        void UpdateJob(MeetingJobEntity job);
        List<MeetingJobEntity> GetDeptJobs(string deptid, int pagesize, int pageindex, out int total);
        JobTemplateEntity GetJobTemplate(string id);
        string UpdateJobTemplate(JobTemplateEntity model);
        string UpdateJobTemplateGroup(JobTemplateEntity model, List<string> DeptList);
        void AddDangerous(JobTemplateEntity entity);

        /// <summary>
        /// 后台管理提交表单
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="mode"></param>
        void ManagerSaveForm(string keyvalue, WorkmeetingEntity mode);

        void AddDangerTemplates(List<DangerTemplateEntity> entity);

        List<JobUserEntity> GetJobUsers(string jobid, string meetingjobid);
        void PostScore(List<JobUserEntity> models);
        int GetMeetingNumber(string deptid);
        List<SinginUserEntity> GetSigninData(string deptid, int year, int month);
        int GetTaskNumber(string deptid);
        IEnumerable<MeetingJobEntity> GetJobs(string userid, string year, string month, string bzid, string isfinish);
        decimal GetTaskPct(string deptid);
        List<MeetingJobEntity> GetMyJobs(string uid, DateTime dateTime);
        WorkmeetingEntity HomeMeetingDetail(string startmeetingid);
        void finishwork(WorkmeetingEntity model);
        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="model"></param>
        void UpdateJob2(MeetingJobEntity model, string trainingtype, string trainingtype2);
        void UpdateJob3(string userKey, MeetingJobEntity model);
        void DeleteJob(MeetingAndJobEntity job);
        List<MeetingSigninEntity> Signin(List<MeetingSigninEntity> signins);
        List<MeetingSigninEntity> SyncState(List<MeetingSigninEntity> signins);
        void Score(MeetingJobEntity job);
        void PostVideo(List<FileInfoEntity> files);
        void EditStartMeeting(WorkmeetingEntity model);
        void EditEndMeeting(WorkmeetingEntity model);
        void OverMeeting(string meetingid, DateTime date, string trainingtype, string userid);
        string StartEndMeeting(WorkmeetingEntity meeting);
        WorkmeetingEntity UpdateRemark(WorkmeetingEntity data);
        List<UnSignRecordEntity> GetDutyPerson(string id);
        void PostDutyPerson(List<WorkmeetingEntity> model);
        List<UnSignRecordEntity> GetDutyPerson(string deptid, DateTime date);
        List<UserInfoEntity> GetBeOnDutyStaffInfo();
        void PostAttendance(WorkmeetingEntity data);
        List<UnSignRecordEntity> GetDayAttendance3(string userid, DateTime date);
        DataTable GetAttendanceUserData3(string UserId, string deptid, DateTime from, DateTime to, bool[] isMenu);
        DataTable GetAttendanceExportData3(string name, string deptid, DateTime from, DateTime to, bool[] isMenu);
        List<PeopleEntity> GetMeetAbstractInfo(string[] bzId);

        IEnumerable<JobTemplateEntity> GetPageList(string deptcode, Pagination pagination, string queryJson);

        WorkmeetingEntity StartMeeting(WorkmeetingEntity meeting, DateTime date, string meetingperson);
        string PostMonitorJob(MeetingJobEntity job);
        void FinishMonitorJob(string id);
        void NoticeMonitor(string id, string userid, string state);
        void UpdateMonitorJob(MeetingJobEntity model);

        DangerTemplateEntity getdangtemplateentity(string id);

        void deldangertemplateentity(string id);
        void DeltetMeeting(string id);

        void savedangertemplateentity(DangerTemplateEntity entity);

        List<MeetingJobEntity> GetJobHistory(MeetingJobEntity meetingJobEntity);
        MeetingJobEntity GetJobDetail(string jobId);
        void AddTempJobs(string meetingid);
        WorkmeetingEntity GetEntity(string startMeetingId);
        void ReBuildMeeting(string meetingId);

        /// <summary>
        /// 获取个人出勤天数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<KeyTimesEntity> GetUserMeetingTimes(string deptid, int year, int month);

        /// <summary>
        /// 获取值班次数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<KeyTimesEntity> GetUserDutyTimes(string deptid, int year, int month);

        /// <summary>
        /// 获取个人任务数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<KeyTimesEntity> GetUserJobs(string deptid, int year, int month);

        /// <summary>
        /// 获取个人任务总分
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<KeyTimesEntity> GetUserMonthScore(string deptid, int year, int month);
        /// <summary>
        /// 获取个人任务总分
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<KeyTimesEntity> GetUserMonthTaskHour(string deptid, int year, int month);
        /// <summary>
        /// 更新任务评分
        /// </summary>
        /// <param name="meetingJobId"></param>
        /// <param name="score"></param>
        /// <param name="isFinished"></param>
        void UpdateScoreState(string meetingJobId, string userid, string score, string isFinished);
        JobTemplateEntity GetJobTemplateByMeetingJobId(string meetingJobId);
        List<UnSignRecordEntity> GetDutyData(string deptId, DateTime from, DateTime to);
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="model"></param>
        void UpdateState(string meetingjobid, string state, string trainingtype);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool ExistJob(JobTemplateEntity entity);
        /// <summary>
        /// 获取默认出勤
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        List<MeetingSigninEntity> GetDefaultSigns(string deptid);
        /// <summary>
        /// 设置默认出勤
        /// </summary>
        /// <param name="data"></param>
        void SetDefaultSigns(List<MeetingSigninEntity> data);
        /// <summary>
        /// 获取班会台账列表
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<MeetingEntity> GetMeetingList(string[] ary_deptid, string userid, bool? isEvaluate, DateTime? from, DateTime? to, int pagesize, int page, out int total);
        /// <summary>
        ///获取班会台账和评价
        /// </summary>
        /// <param name="pagination">分页公用类</param>
        /// <param name="queryJson">Depts部门id集合|startTime开始时间|endTime结束时间</param>
        /// <returns></returns>
        List<MeetingBookEntity> GetMeetPage(Pagination pagination, string queryJson);

        List<Meeting2Entity> GetData2(string[] deptid, int pagesize, int pageindex, out int total);
        List<WorkmeetingEntity> GetList(string[] depts, string userId, DateTime? begin, DateTime? end, string appraise, bool v2, int rows, int page, out int total);
        List<MeetingJobEntity> FindMeetingJobs(string departmentId, DateTime today);
        List<MeetingJobEntity> GetMeetingJobs(string meetingId);
        List<MeetingJobEntity> FindMeetingJobs2(string departmentId, DateTime date);
        void AddTodayJobs(List<MeetingJobEntity> jobs);
        List<MeetingJobEntity> FindLongJobs(string departmentId, DateTime date);
        void AddLongJobs(List<MeetingJobEntity> jobs);
        void UpdateJobs(List<MeetingJobEntity> jobs);
        bool IsEndMeetingOver(string deptId, DateTime now);
        List<string> Query(string key, string deptid, int pagesize, int pageindex, out int total);
        JobTemplateEntity Detail(string deptid, string data);
        System.Collections.IList QueryNew(string key, string deptId, int pageSize, int pageIndex, out int total);
        Dictionary<string, int> TodayWorkStatistics(string searchCode, DateTime now);
        object GetJobPagedList(Pagination pagination, string queryJson);
        IList GetJobHourPagedList(string startTime, string endTime, string keyWord, string deptCode, int pageIndex, int pageSize, out int total);
        void SaveTaskHour(List<JobUserEntity> userEntities);

        /// <summary>
        /// 获取工作任务
        /// </summary>
        /// <param name="pagination">分页公共类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id</param>
        /// <returns></returns>
        List<MeetingJobEntity> GetMeetJobList(Pagination pagination, string queryJson);
        WorkmeetingEntity Get(string id);
    }
}
