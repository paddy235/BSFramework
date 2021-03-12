using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    public class HumanDangerTrainingBLL
    {
        public void Add(HumanDangerTrainingEntity training)
        {
            var deptservice = new DepartmentService();
            var dept = deptservice.GetEntity(training.DeptId);
            if (dept != null)
            {
                training.DeptId = dept.DepartmentId;
                training.DeptName = dept.FullName;
            }

            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.Add(training);
        }

        public List<HumanDangerTrainingEntity> GetList(string userid, string[] users, string[] duty, DateTime? from, DateTime? to, string status, string level, string evaluatestatus, int pageSize, int pageIndex, out int total)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetList(userid, users, duty, from, to, status, level, evaluatestatus, pageSize, pageIndex, out total);
        }

        public void EnsureStatus(MeetingJobEntity item, string userId)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.Ensure(item, userId);
        }

        /// <summary>
        /// 部门首页统计
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public object GetData2(string userid, string[] deptid, DateTime from, DateTime to)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetData2(userid, deptid, from, to);
        }

        public void Delete(string id)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.Delete(id);
        }

        public HumanDangerTrainingEntity GetDetail(string id)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetDetail(id);
        }

        public void SaveReasons(HumanDangerTrainingEntity model)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.SaveReasons(model);
        }

        public List<HumanDangerTrainingEntity> GetUserTodo(string userid, string[] users, int pageSize, int pageIndex, out int total)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetUserTodo(userid, users, pageSize, pageIndex, out total);
        }

        public void SaveMeasures(HumanDangerTrainingEntity model)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.SaveMeasures(model);
        }

        public void Finish(string id, DateTime date)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.Finish(id, date);
        }

        //public HumanDangerTrainingEntity GetDetailByJob(string jobid)
        //{
        //    IHumanDangerTrainingService service = new HumanDangerTrainingService();
        //    return service.GetDetailByJob(jobid);
        //}

        public List<HumanDangerTrainingEntity> GetListByDeptId(string deptid, DateTime from, DateTime to)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetListByDeptId(deptid, from, to);
        }

        public List<HumanDangerTrainingEntity> GetData(string[] deptid, DateTime from, DateTime to)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetData(deptid, from, to);
        }

        public void EditMeasure(HumanDangerTrainingEntity model, string userid, string username)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.EditMeasure(model, userid, username);
        }

        public List<HumanDangerTrainingEntity> GetTrainings(string userid, string[] depts, string key, DateTime? v1, DateTime? v2, string evaluatestatus, int rows, int page, string fzuser, string evaluatelevel, string status, out int total)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetTrainings(userid, depts, key, v1, v2, evaluatestatus, rows, page, fzuser, evaluatelevel, status, out total);
        }

        public List<HumanDangerTrainingEntity> GetContent(string id)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetContent(id);
        }

        public void Evaluate(string id)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.Evaluate(id);
        }

        public List<HumanDangerTrainingEntity> GetUndo(string[] depts, string key, DateTime? v2, DateTime? v3, int evaluate, int rows, int page, out int total)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetUndo(depts, key, v2, v3, rows, page, out total);
        }

        public void Evaluate(ActivityEvaluateEntity data)
        {
            IHumanDangerService ser = new HumanDangerService();
            ser.Evaluate(data);
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.Evaluate(data.Activityid);
        }

        public List<HumanDangerTrainingEntity> GetDataList(string userid, string[] users, DateTime? from, DateTime? to, string status, string level, string evaluatestatus, int pagesize, int page, out int total)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetDataList(userid, users, from, to, status, level, evaluatestatus, pagesize, page, out total);
        }

        public List<HumanDangerTrainingEntity> GetListByUserIdJobId(string jobid, string userid)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetListByUserIdJobId(jobid, userid);
        }

        public List<HumanDangerTrainingEntity> GetListByJobId(string jobid)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetListByJobId(jobid);
        }

        public List<HumanDangerTrainingEntity> GetTrainingsByTrainingUser(string[] trainingUserIds, DateTime? starttime, DateTime? endtime)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetTrainingsByTrainingUser(trainingUserIds, starttime, endtime);
        }

        public List<ItemEntity> GetUsers(string departmentId)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetUsers(departmentId);
        }

        public void DeleteTraining(string keyValue)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            service.DeleteTraining(keyValue);
        }

        public List<HumanDangerTrainingEntity> GetData3(string[] users, DateTime from, DateTime to)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetData3(users, from, to);
        }

        public List<HumanDangerTrainingEntity> GetToEvaluate(string deptid, string analyst, DateTime? begin, DateTime? end, int pagesize, int page, out int total)
        {
            IHumanDangerTrainingService service = new HumanDangerTrainingService();
            return service.GetToEvaluate(deptid, analyst, begin, end, pagesize, page, out total);
        }
    }
}
