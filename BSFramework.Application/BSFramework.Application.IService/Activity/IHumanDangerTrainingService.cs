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
using BSFramework.Application.Entity.BaseManage;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 描 述：人身风险预控
    /// </summary>
    public interface IHumanDangerTrainingService
    {
        void Add(HumanDangerTrainingEntity training);
        List<HumanDangerTrainingEntity> GetList(string userid, string[] users, string[] duty, DateTime? from, DateTime? to, string status, string level, string evaluatestatus, int pageSize, int pageIndex, out int total);
        void Delete(string id);
        HumanDangerTrainingEntity GetDetail(string id);
        void SaveReasons(HumanDangerTrainingEntity model);
        void SaveMeasures(HumanDangerTrainingEntity model);
        void Finish(string id, DateTime date);
        //HumanDangerTrainingEntity GetDetailByJob(string jobid);
        List<HumanDangerTrainingEntity> GetListByDeptId(string deptid, DateTime from, DateTime to);
        List<HumanDangerTrainingEntity> GetData(string[] deptid, DateTime from, DateTime to);
        void EditMeasure(HumanDangerTrainingEntity model, string userid, string username);
        List<HumanDangerTrainingEntity> GetTrainings(string userid, string[] depts, string key, DateTime? v1, DateTime? v2, string evaluatestatus, int rows, int page, string fzuser, string evaluatelevel, string status, out int total);
        void Ensure(MeetingJobEntity item, string userId);
        List<HumanDangerTrainingEntity> GetContent(string id);
        void Evaluate(string id);
        List<HumanDangerTrainingEntity> GetUndo(string[] depts, string key, DateTime? v1, DateTime? v2, int rows, int page, out int total);
        object GetData2(string userid, string[] deptid, DateTime from, DateTime to);
        List<HumanDangerTrainingEntity> GetDataList(string userid, string[] users, DateTime? from, DateTime? to, string status, string level, string evaluatestatus, int pagesize, int page, out int total);
        List<HumanDangerTrainingEntity> GetListByUserIdJobId(string userid, string jobid);
        List<HumanDangerTrainingEntity> GetListByJobId(string jobid);

        /// <summary>
        /// 待办
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<HumanDangerTrainingEntity> GetUserTodo(string userid, string[] users, int pageSize, int pageIndex, out int total);
        List<HumanDangerTrainingEntity> GetTrainingsByTrainingUser(string[] trainingUserIds, DateTime? starttime, DateTime? endtime);
        List<ItemEntity> GetUsers(string departmentId);
        void DeleteTraining(string keyValue);

        List<HumanDangerTrainingEntity> GetData3(string[] users, DateTime from, DateTime to);
        List<HumanDangerTrainingEntity> GetToEvaluate(string deptid, string analyst, DateTime? begin, DateTime? end, int pagesize, int page, out int total);
    }
}
