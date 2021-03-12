using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BSFramework.Application.IService.WorkMeeting
{
    /// <summary>
    /// 班前一课
    /// </summary>
    public interface IMeetSubjectService
    {

        #region MyRegion
        /// <summary>
        /// 活动id获取数据
        /// </summary>
        /// <returns></returns>
        MeetSubjectEntity getDataByMeetID(string MeetId);

        /// <summary>
        /// 主键获取数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        MeetSubjectEntity GetEntity(string keyValue);

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void delEntity(string keyValue);
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
        void SaveForm(string keyValue, MeetSubjectEntity entity);


        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
         void Save(MeetSubjectEntity entity);
        
        #endregion
    }
}
