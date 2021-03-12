using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 描 述：安全预知训练
    /// </summary>
    public interface DangerIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<DangerEntity> GetList(string queryJson);
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
        DataTable GetPagesList(Pagination pagination, string queryJson);

        /// <summary>
        /// 获取危险预知台账和评价
        /// </summary>
        ///<param name="pagination">分页公用类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|haveEvaluate是否查询评价|Depts部门id集合</param>
        /// <returns></returns>
        List<DangerEntity> GetDangerCount(Pagination pagination, string queryJson);
        IList<DangerEntity> GetTrainingsApp(string deptid, DateTime fromtime, DateTime to);
        IList<DangerEntity> GetTrainingsDo(string deptid, DateTime fromtime, DateTime to);
        List<PeopleEntity> GetGroupTraining(int year, int month, string groupid);
        /// <summary>
        /// 获取需要预知训练的工作任务
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        DataTable GetJobList(string groupId);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        DangerEntity GetEntity(string keyValue);
        IList<DangerEntity> GetMyTrainings(string userId, int pageSize, int pageIndex, out int total);
        IList<DangerEntity> GetTrainings(string userId, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 根据工作任务Id选择措施落实人员
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        DataTable GetJobUserList(string jobId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, DangerEntity entity);

        DangerEntity Save(MeetingJobEntity entity);
        #endregion
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="keyValue">实体主键</param>
        /// <param name="entity">危险预知训练实体</param>
        /// <param name="measures">防控措施</param>
        void Update(string keyValue, DangerEntity entity, List<MeasuresEntity> measures);
        DangerEntity GetTrainingDetail(string id);
        MeasuresEntity EditItem(MeasuresEntity entity);
        MeasuresEntity AddItem(MeasuresEntity entity);
        int Count(string[] depts, DateTime start, DateTime end);
        void UploadTraining(DangerEntity entity);
        dynamic GetTrainingUsers(string id, string userid, int pageSize, int pageIndex, out int total);
        void PostTraining(DangerEntity entity);
        void FinishTraining(DangerEntity entity);
        IList<DangerTemplateEntity> GetDangerous(string query, int pageSize, int pageIndex, out int total);
        IList<DangerTemplateEntity> GetMeasures(string query, int pageSize, int pageIndex, out int total);
        void BeginTraining(DangerEntity entity);

        DangerEntity FinishTraining2(DangerEntity training);

        List<DangerEntity> FindTrainings(string key, int limit);

        DangerEntity AddTraining(DangerEntity training);

        void TrainingScore(DangerEntity dangerEntity);

        List<DangerEntity> GetTrainingData(string userid, DateTime from, DateTime to, int pageSize, int pageIndex, out int total);

        void DeleteItem(string id);

        string UpdateTraingItems(string dangerid, string jobid);
        List<DangerEntity> GetRecords(string deptid, string name, DateTime? from, DateTime? to, int pagesize, int pageindex, out int total);
        List<DangerEntity> GetRecordsApp(string deptid, string name, DateTime? from, DateTime? to, int pagesize, int pageindex, out int total);

        MeasuresEntity UpdateTrainingPerson(string itemid, string userid, string users);
        MeasuresEntity UpdateTrainingState(string itemid, bool isover);

        DataTable GetDangerPageList(string userid, Pagination pagination, string queryJson);
        DataTable GetDangerJsonNew(Pagination pagination, string queryJson, string name, string from, string to, string userid);

        DataTable GetDangerJson(Pagination pagination, string queryJson, string type, string[] depts, string userid);
        string GetCount(string deptid, DateTime f, DateTime t);
        /// <summary>
        /// 获取危险预知训练
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        DangerEntity GetTraining(string id, string userid);
        /// <summary>
        /// 获取危险预知训练列表
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<DangerEntity> GetUserTrainings(string userid, string id);
        List<MeasuresEntity> UpdateTraingingItem(DangerEntity data, string userid);
        List<MeasuresEntity> SubmitTraingingItem(DangerEntity data, string userid);
        DangerEntity UpdateTraingingItem2(DangerEntity danger, string userid);
        DangerEntity SubmitTraingingItem2(DangerEntity danger, string userid);
        /// <summary>
        /// 作业人训练时保存
        /// </summary>
        /// <returns></returns>
        void DoTraining(DangerEntity entity, string userid);

        /// <summary>
        /// 负责人训练时保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void DoTraining2(DangerEntity entity, string userid);

        /// <summary>
        /// 作业人训练时提交
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void SubmitDoTraining(DangerEntity entity, string userid);

        /// <summary>
        /// 负责人训练时提交
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void SubmitDoTraining2(DangerEntity entity, string userid);
        int GetTrainingTimes(string deptid);

        List<DangerEntity> GetHistory(string[] deptid, string key, DateTime? from, DateTime? to, bool? isEvaluate, string userid, int pageSize, int pageIndex, out int total);
        void Delete(string id);


        List<DangerEntity> GetTimeCount(string deptid, DateTime f, DateTime t);
    }
}
