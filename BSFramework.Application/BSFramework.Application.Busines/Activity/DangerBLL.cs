using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    /// <summary>
    /// 描 述：安全预知训练
    /// </summary>
    public class DangerBLL
    {
        private DangerIService service = new DangerService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<DangerEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }

        public List<PeopleEntity> GetGroupTraining(int year, int month, string groupid)
        {
            return service.GetGroupTraining(year, month, groupid);
        }
        /// <summary>
        /// 获取需要预知训练的工作任务
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public DataTable GetJobList(string groupId)
        {
            return service.GetJobList(groupId);
        }

        public IList<DangerEntity> GetMyTrainings(string userId, int pageSize, int pageIndex, out int total)
        {
            return service.GetMyTrainings(userId, pageSize, pageIndex, out total);
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPagesList(Pagination pagination, string queryJson) {
            return service.GetPageList(pagination, queryJson);

        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DangerEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        /// <summary>
        /// 根据工作任务Id选择措施落实人员
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public DataTable GetJobUserList(string jobId)
        {
            return service.GetJobUserList(jobId);
        }

        public DangerEntity GetTrainingDetail(string id)
        {
            return service.GetTrainingDetail(id);
        }

        public IList<DangerEntity> GetTrainings(string deptid, int pageSize, int pageIndex, out int total)
        {
            return service.GetTrainings(deptid, pageSize, pageIndex, out total);
        }
        #endregion

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
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DangerEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DangerEntity> GetRecords(string deptid, string name, DateTime? from, DateTime? to, int pagesize, int pageindex, out int total)
        {
            return service.GetRecords(deptid, name, from, to, pagesize, pageindex, out total);
        }

        public void PostTraining(DangerEntity entity)
        {
            service.PostTraining(entity);
        }

        public MeasuresEntity EditItem(MeasuresEntity entity)
        {
            return service.EditItem(entity);
        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public DangerEntity Save(MeetingJobEntity entity)
        {
            try
            {
                return service.Save(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void FinishTraining(DangerEntity entity)
        {
            service.FinishTraining(entity);
        }

        public void BeginTraining(DangerEntity entity)
        {
            service.BeginTraining(entity);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="keyValue">实体主键</param>
        /// <param name="entity">危险预知训练实体</param>
        /// <param name="measures">防控措施</param>
        public void Update(string keyValue, DangerEntity entity, List<MeasuresEntity> measures)
        {
            service.Update(keyValue, entity, measures);
        }

        public void UploadTraining(DangerEntity entity)
        {
            service.UploadTraining(entity);
        }

        public dynamic GetTrainingUsers(string id, string userid, int pageSize, int pageIndex, out int total)
        {
            return service.GetTrainingUsers(id, userid, pageSize, pageIndex, out total);
        }

        public IList<DangerTemplateEntity> GetDangerous(string query, int pageSize, int pageIndex, out int total)
        {
            return service.GetDangerous(query, pageSize, pageIndex, out total);
        }

        public IList<DangerTemplateEntity> GetMeasures(string query, int pageSize, int pageIndex, out int total)
        {
            return service.GetMeasures(query, pageSize, pageIndex, out total);
        }

        public int GetTrainingTimes(string deptid)
        {
            return service.GetTrainingTimes(deptid);
        }

        public DangerEntity FinishTraining2(DangerEntity training)
        {
            return service.FinishTraining2(training);
        }

        public dynamic FindTrainings(string key, int limit)
        {
            return service.FindTrainings(key, limit);
        }

        public DangerEntity AddTraining(DangerEntity job)
        {
            //if (job.JobUsers != null)
            //{
            //    for (int i = 0; i < job.JobUsers.Count; i++)
            //    {
            //        if (i == 0)
            //            job.JobUsers[i].JobType = "ischecker";
            //        else job.JobUsers[i].JobType = "isdoperson";
            //    }
            //}
            return service.AddTraining(job);
        }

        public void TrainingScore(DangerEntity dangerEntity)
        {
            service.TrainingScore(dangerEntity);
        }

        public List<DangerEntity> GetTrainingData(string userid, DateTime from, DateTime to, int pageSize, int pageIndex, out int total)
        {
            return service.GetTrainingData(userid, from, to, pageSize, pageIndex, out total);
        }

        public void DeleteItem(string id)
        {
            service.DeleteItem(id);
        }

        public MeasuresEntity AddItem(MeasuresEntity entity)
        {
            return service.AddItem(entity);
        }

        public string UpdateTraingItems(string dangerid, string jobid)
        {
            return service.UpdateTraingItems(dangerid, jobid);
        }

        public MeasuresEntity UpdateTrainingPerson(string itemid, string userid, string users)
        {
            return service.UpdateTrainingPerson(itemid, userid, users);
        }

        public MeasuresEntity UpdateTrainingState(string itemid, bool isover)
        {
            return service.UpdateTrainingState(itemid, isover);
        }

        public DataTable GetDangerPageList(string userid, Pagination pagination, string queryJson)
        {
            return service.GetDangerPageList(userid, pagination, queryJson);
        }
        public DataTable GetDangerJsonNew(Pagination pagination, string queryJson, string name, string from, string to,string userid) {
            return service.GetDangerJsonNew(pagination, queryJson, name, from, to, userid);
        }

        public DataTable GetDangerJson(Pagination pagination, string queryJson, string type, string[] depts, string userid) {
            return service.GetDangerJson(pagination, queryJson, type, depts, userid);
        }       

        public string GetCount(string deptid, DateTime f, DateTime t)
        {
            return service.GetCount(deptid, f, t);
        }

        public DangerEntity GetTraining(string id, string userid)
        {
            return service.GetTraining(id, userid);
        }

        public List<DangerEntity> GetUserTrainings(string userid, string id)
        {
            return service.GetUserTrainings(userid, id);
        }

        public List<MeasuresEntity> UpdateTraingingItem(DangerEntity data, string userid)
        {
            return service.UpdateTraingingItem(data, userid);
        }

        public List<MeasuresEntity> SubmitTraingingItem(DangerEntity data, string userid)
        {
            return service.SubmitTraingingItem(data, userid);
        }
        public DangerEntity UpdateTraingingItem2(DangerEntity danger, string userid)
        {
            return service.UpdateTraingingItem2(danger, userid);
        }
        public DangerEntity SubmitTraingingItem2(DangerEntity danger, string userid)
        {
            return service.SubmitTraingingItem2(danger, userid);
        }

        /// <summary>
        /// 作业人训练时保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void DoTraining(DangerEntity entity, string userid)
        {
            service.DoTraining(entity, userid);
        }
        /// <summary>
        /// 负责人训练时保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void DoTraining2(DangerEntity entity, string userid)
        {
            service.DoTraining2(entity, userid);
        }

        /// <summary>
        /// 负责人训练时提交
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SubmitDoTraining2(DangerEntity entity, string userid)
        {
            service.SubmitDoTraining2(entity, userid);
        }

        /// <summary>
        /// 作业人训练时提交
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SubmitDoTraining(DangerEntity entity, string userid)
        {
            service.SubmitDoTraining(entity, userid);
        }

        /// <summary>
        /// 班组帮历史记录
        /// </summary>
        /// <param name="key"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<DangerEntity> GetHistory(string[] deptid, string key, DateTime? from, DateTime? to, bool? isEvaluate, string userid, int pageSize, int pageIndex, out int total)
        {
            return service.GetHistory(deptid, key, from, to, isEvaluate, userid, pageSize, pageIndex, out total);
        }

        public void Delete(string id)
        {
            service.Delete(id);

        }

        public List<DangerEntity> GetTimeCount(string deptid, DateTime f, DateTime t)
        {
            return service.GetTimeCount(deptid, f, t);
        }
    }
}
