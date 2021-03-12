using BSFramework.Application.Entity.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using System.Data;
using BSFramework.Application.Entity.EducationManage;

namespace BSFramework.Application.IService.EducationManage
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public interface EdActivityIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        List<ActivityCategoryEntity> GetIndex(string userid, string deptid, string name);
        List<EdActivityEntity> GetList(int status, string bzDepart, DateTime? fromtime, DateTime? to);
        /// <summary>
        /// 获取类别/
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        ActivityCategoryEntity GetCategory(string keyvalue);
        /// <summary>
        /// 获取所有类别
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        List<ActivityCategoryEntity> GetCategoryList();
        List<EdActivitySupplyEntity> GetSupplyById();
        List<SupplyPeopleEntity> GetPeopleById();
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetIndexEntity(string keyValue);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        EdActivityEntity GetEntity(string keyValue);
        EdActivitySupplyEntity GetActivitySupplyEntity(string keyValue);
        int Count(string[] depts, string category, DateTime start, DateTime end);
        List<EdActivityEntity> GetActivityList(string deptid, string StrTime);

        List<ActivityEvaluateEntity> GetEntityList();
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        ActivityCategoryEntity GetCategoryEntity(string keyValue);
        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetIndex(string deptId, DateTime? from, DateTime? to, string name);
        ActivityCategoryEntity AddCategory(ActivityCategoryEntity model);

        /// <summary>
        /// 管理平台修改类型
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        void UpdateActivityCategoryType(ActivityCategoryEntity category);
        /// <summary>
        /// 管理平台删除类型
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        void DeleteCategoryType(string categoryid);
        /// <summary>
        /// 管理平台新增类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        void AddCategoryType(ActivityCategoryEntity model);
        bool IsEvaluate(string id, string userid);

        /// <summary>
        /// 获取班组活动列表
        /// </summary>
        /// <param name="page">请求页索引</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="total">记录总数</param>
        /// <param name="status">状态（0：未开展，1：已结束，2：所有）</param>
        /// <param name="subject">活动主题</param>
        /// <returns></returns>
        System.Collections.IList GetList(int page, int pagesize, out int total, int status, string subject, string bzDepart);

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        DataTable GetPageList(Pagination pagination, string queryJson);

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        DataTable GetPagesList(Pagination pagination, string queryJson, string type, string select, string deptid, string category, string isEvaluate, string userid);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        void SaveActivitySupply(string keyValue, EdActivitySupplyEntity entity);
        void SaveSupplyPeople(string keyValue, SupplyPeopleEntity entity);

        /// <summary>
        ///修改
        /// </summary>
        /// <param name="entity"></param>
        void modfiyEntity(EdActivityEntity entity);
        void Del(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, EdActivityEntity entity);

        void SaveFormSafetyday(string keyValue, EdActivityEntity entity, List<SafetydayEntity> safetyday, string userId);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveFormCategory(string keyValue, ActivityCategoryEntity entity);
        List<ActivityCategoryEntity> GetActivityCategories(string deptid);
        IEnumerable<EdActivityEntity> GetListBySql(string StrSql);

        DataTable GetDataTable(string StrSql);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IEnumerable<EdActivityEntity> GetList(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total);
        IEnumerable<EdActivityEntity> GetListApp(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityId"></param>
        void Over(EdActivityEntity model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        EdActivityEntity GetActivities(string category, string deptid);
        void Start(EdActivityEntity entity);
        void Ready(EdActivityEntity model);
        void Study(EdActivityEntity model);

        EdActivityEntity GetDetail(string id);
        void DeleteCategory(string categoryid);
        ActivityCategoryEntity UpdateActivityCategory(ActivityCategoryEntity category);
        string PostActivity(ActivityCategoryEntity activity);
        List<ActivityEvaluateEntity> GetActivityEvaluateEntity(List<string> list);
        #endregion

        List<EdActivityEntity> GetActivities2(string userid, DateTime from, DateTime to, string category, string deptid, bool isall);

        /// <summary>
        /// 获取安全学习日数量
        /// </summary>
        ///<param name="pagination">分页公用类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id|State状态|Category分类|haveEvaluate是否查询评价</param>
        /// <returns></returns>
        List<EdActivityEntity> GetEdAcJobCount(Pagination pagination, string queryJson);
        void Edit(EdActivityEntity activityEntity);

        void SaveEvaluate(string keyValue, ActivityEvaluateEntity entity);

        List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string keyValue, int pagesize, int page, out int total);
        List<StatisticsNumModel> GetActivityStatisticsEntity(string keyValue, int pagesize, int page, string date, out int total);

        void Update(string keyValue, EdActivityEntity entity);

        void ManagerUpdate(string keyValue, EdActivityEntity entity);
        int GetActivityList(string deptid, string from, string to);
        void SaveActivityPerson(EdActivityPersonEntity entity);
        string[] GetLastest(string deptid);
        List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string Activityid);

        DataTable GetEvaluateSetData(Pagination pagination, string queryJson);

        void SaveEvaluateSet(string keyvalue, EvaluateStepsEntity entity);
        void deleteEvaluateSet(string keyvalue);
        EvaluateStepsEntity getEvaluateSet(string keyvalue);
        List<EvaluateStepsEntity> getEvaluateSetBymodule(string module);
        //代办
        List<EvaluateTodoEntity> WorkToDo(string userId, string module);
        List<EvaluateTodoEntity> AcWorkToDo(string userId, string activityid);
        //生成代办
         void setToDo(string module, string activityid, string deptid);
        void NextTodo(string module, string activityid);
        int GetMonthCount(string deptId);
    }
}
