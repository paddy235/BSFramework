using BSFramework.Application.Entity.TaskManage;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.TaskManage
{
    /// <summary>
    /// 任务相关
    /// </summary>
    public interface ITaskService
    {
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
        List<MeetingJobEntity> List(string userId, string districtId, DateTime date, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 选择今日工作
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="tasks"></param>
        void UpdateDistrictTask(string districtId, string userId, string[] tasks);

        /// <summary>
        /// 现场终端签到
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="districtId"></param>
        /// <param name="date"></param>
        void SignIn(string userId, string districtId, string districtName, DateTime date);

        /// <summary>
        /// 统计未签到记录
        /// </summary>
        /// <param name="cycle"></param>
        void CalculateUnSignin(string cycle, DateTime date);

        /// <summary>
        /// 签到记录
        /// </summary>
        /// <param name="meetingJob"></param>
        void EnsureSignin(MeetingJobEntity meetingJob, string districtId, DateTime date);

        /// <summary>
        /// 未签到记录
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        List<UnSigninEntity> GetUnSignin(string districtId, string categoryId, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 获取今日工作数
        /// </summary>
        /// <param name="deptId">班组</param>
        /// <param name="date">日期</param>
        /// <returns>完成数</returns>
        int FetchTodayTotal(string deptId, DateTime date);

        /// <summary>
        /// 获取今日工作数
        /// </summary>
        /// <param name="deptId">班组</param>
        /// <param name="date">日期</param>
        /// <param name="isFinished">是否完成</param>
        /// <returns>完成数</returns>
        int FetchTodayTotal(string deptId, DateTime date, bool isFinished);
        List<MeetingJobEntity> List(string deptId, int year, int month, string status, int pageSize, int pageIndex, out int total);
    }
}
