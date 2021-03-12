using BSFramework.Application.Entity.TaskManage;
using BSFramework.Application.IService.TaskManage;
using BSFramework.Application.Service.TaskManage;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.TaskManage
{
    /// <summary>
    /// 任务
    /// </summary>
    public class TaskBLL
    {
        ITaskService taskService = new TaskService();

        /// <summary>
        /// 今日工作列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="date"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<MeetingJobEntity> List(string userId, string districtId, DateTime date, int pageSize, int pageIndex, out int total)
        {
            return taskService.List(userId, districtId, date, pageSize, pageIndex, out total);
        }

        /// <summary>
        /// 选择今日工作
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="tasks"></param>
        public void UpdateDistrictTask(string districtId, string userId, string[] tasks)
        {
            taskService.UpdateDistrictTask(districtId, userId, tasks);
        }

        public void EnsureSignin(MeetingJobEntity meetingJob, string districtId, DateTime date)
        {
            taskService.EnsureSignin(meetingJob, districtId, date);
        }

        /// <summary>
        /// 现场终端
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="date"></param>
        public void SignIn(string userId, string districtId, string districtName, DateTime date)
        {
            taskService.SignIn(userId, districtId, districtName, date);
        }

        /// <summary>
        /// 统计未签到记录
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="date"></param>
        public void CalculateUnSignin(string cycle, DateTime date)
        {
            taskService.CalculateUnSignin(cycle, date);
        }

        /// <summary>
        /// 区域未签到记录
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<UnSigninEntity> GetUnSignin(string districtId, string categoryId, int pageSize, int pageIndex, out int total)
        {
            return taskService.GetUnSignin(districtId, categoryId, pageSize, pageIndex, out total);
        }

        /// <summary>
        /// 获取今日工作数
        /// </summary>
        /// <param name="deptId">班组</param>
        /// <param name="date">日期</param>
        /// <returns>工作数</returns>
        public int FetchTodayTotal(string deptId, DateTime date)
        {
            return taskService.FetchTodayTotal(deptId, date);
        }

        /// <summary>
        /// 获取今日工作数
        /// </summary>
        /// <param name="deptId">班组</param>
        /// <param name="date">日期</param>
        /// <param name="isFinished">是否完成</param>
        /// <returns>工作数</returns>
        public int FetchTodayTotal(string deptId, DateTime date, bool isFinished)
        {
            return taskService.FetchTodayTotal(deptId, date, isFinished);
        }

        public List<MeetingJobEntity> List(string deptId, int year, int month, string status, int pageSize, int pageIndex, out int total)
        {
            return taskService.List(deptId, year, month, status, pageSize, pageIndex, out total);
        }
    }
}
