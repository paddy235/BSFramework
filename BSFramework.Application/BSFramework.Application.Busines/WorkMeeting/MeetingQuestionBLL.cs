using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.WorkMeeting;
using System;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class MeetingQuestionBLL
    {
        private readonly IMeetingQuestionSerivce _service;
        public MeetingQuestionBLL()
        {
            _service = new MeetingQuestionSerivce();
        }
        /// <summary>
        /// 根据班会id获取班前一题
        /// </summary>
        /// <param name="meetingId">班会Id</param>
        /// <returns></returns>
        public MeetingQuestionEntity GetEntityByMeetingId(string meetingId)
        {
            return _service.GetEntityByMeetingId(meetingId);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="data"></param>
        public void Update(MeetingQuestionEntity data)
        {
            _service.Update(data);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data"></param>
        public void Insert(MeetingQuestionEntity data)
        {
            _service.Insert(data);
        }

        /// <summary>
        /// 班前一题 数据统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public object GetStatistics(string startTime, string endTime, string deptCode)
        {
            return _service.GetStatistics(startTime, endTime, deptCode);
        }
    }
}
