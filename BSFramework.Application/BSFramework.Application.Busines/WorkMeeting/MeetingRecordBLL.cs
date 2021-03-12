using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
   public class MeetingRecordBLL
    {
        private readonly IMeetingRecordService _service;
        public MeetingRecordBLL() 
        {
            _service = new MeetingRecordService();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="data"></param>
        public void Update(MeetingRecordEntity data)
        {
            _service.Update(data);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="data"></param>
        public void Insert(MeetingRecordEntity data)
        {
            _service.Insert(data);
        }

        /// <summary>
        /// 根据班会的Id，查询班会记录 
        /// （如果有多个默认查第一个）
        /// </summary>
        /// <param name="meetingId">班会的Id</param>
        /// <returns></returns>
        public MeetingRecordEntity GetEntityByMeetingId(string meetingId)
        {
            return _service.GetEntityByMeetingId(meetingId);
        }

        /// <summary>
        /// 各部门早安中铝完成情况
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="deptCode">查询部门的编码</param>
        /// <returns></returns>
        public object GetStatistics(string startTime, string endTime, string deptCode)
        {
            return _service.GetStatistics(startTime, endTime, deptCode);
        }
    }
}
