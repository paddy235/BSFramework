using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
    /// <summary>
    /// 班前一课
    /// </summary>
    public class MeetSubjectBLL
    {
        private IMeetSubjectService service;
        public MeetSubjectBLL() {
            service=new MeetSubjectService();
        }

        #region MyRegion
        /// <summary>
        /// 活动id获取数据
        /// </summary>
        /// <returns></returns>
        public MeetSubjectEntity getDataByMeetID(string MeetId)
        {
            return service.getDataByMeetID(MeetId);
        }
        /// <summary>
        /// 主键获取数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public MeetSubjectEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void delEntity(string keyValue) {
            service.delEntity(keyValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            service.RemoveForm(keyValue);
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, MeetSubjectEntity entity)
        {
            service.SaveForm(keyValue,entity);
        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void Save(MeetSubjectEntity entity)
        {
            service.Save(entity);
        }
        #endregion
    }
}
