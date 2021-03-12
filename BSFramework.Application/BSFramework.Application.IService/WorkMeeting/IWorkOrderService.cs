using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.IService.WorkMeeting
{
    public interface IWorkOrderService
    {
        /// <summary>
        /// 保存或修改部门班次信息
        /// </summary>
        void WorkSetSave(List<WorkOrderEntity> data, List<WorkGroupSetEntity> group, List<WorkTimeSortEntity> timeList);
        /// <summary>
        /// 保存或修改单天部门班次信息
        /// </summary>
        void WorkSetSaveOneDay(DateTime UpTime, string worksetting, string WorkOrderId);
        /// <summary>
        /// 查询部门排班
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        IEnumerable<WorkTimeSortEntity> GetWorkOrderList(DateTime startTime, DateTime endTime, string departmentId);


        /// <summary>
        /// 查询班组一天数据
        /// </summary>
        /// <returns>长度为2  第一：上班时间段 8：00-15：00 第二：班次 白班 夜班</returns>
        string[] GetWorkOrderList(DateTime Time, string deparMentId);
        /// <summary>
        /// 查询班组一天数据
        /// </summary>
        /// <returns>长度为2  第一：上班时间段 8：00-15：00 第二：班次 白班 夜班</returns>
        string[] GetWorkOrderTotal(DateTime Time, string deparMentId);
        /// <summary>
        /// 查询同一批次班组
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        IEnumerable<WorkGroupSetEntity> GetWorkOrderGroup(string departmentId);

        /// <summary>
        /// 批量删除排班
        /// </summary>
        /// <param name="keyValue"></param>
        void OrderRemoveForm(string keyValue);
        /// <summary>
        /// 查询班次班组
        /// </summary>
        /// <param name="WorkOrderId"></param>
        /// <returns></returns>
        string GetWorkOrderday(string WorkSortId);
        /// <summary>
        /// 查询部门排班
        /// <returns></returns>
        IEnumerable<WorkTimeSortEntity> GetWorkTimeSort(string deptid);
        /// <summary>
        /// 获取所有设置
        /// </summary>
        /// <returns></returns>
         IEnumerable<WorkOrderEntity> GetOrderAll();
        /// <summary>
        /// 获取所有设置
        /// </summary>
        /// <returns></returns>
         IEnumerable<WorkGroupSetEntity> GetGroupAll();

        /// <summary>
        /// 获取绑定原班组班次
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        Dictionary<string, string> getSetValue(string DeptId);
        /// <summary>
        /// 查询部门绑定的班次
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        void GetWorkSettingByDept(string departmentId, out string setupid, out string createuserid);
        WorkTimeSortEntity GetEntity(DateTime startDate, string keyWord);
    }
}
