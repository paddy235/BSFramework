
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.IService.WorkMeeting
{
    /// <summary>
    /// 描 述：排班
    /// </summary>
    public interface IWorkSettingService
    {
        /// <summary>
        /// 初始化添加班次
        /// </summary>
        /// <param name="entityList"></param>
        void saveForm(List<WorkSettingEntity> entityList);

        /// <summary>
        /// 获取班次列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkSettingEntity> GetList(string depId);

        /// <summary>
        /// 获取班次
        /// </summary>
        /// <returns></returns>
        WorkSettingEntity getEntitybySetUp(string keyvalue,string deptid);
        /// <summary>
        /// 获取班次
        /// </summary>
        /// <returns></returns>
        WorkSettingEntity getEntity(string keyvalue);
        /// <summary>
        /// 删除一批量的班次
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
    }
}
