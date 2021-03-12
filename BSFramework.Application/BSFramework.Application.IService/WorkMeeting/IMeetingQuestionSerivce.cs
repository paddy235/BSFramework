using BSFramework.Application.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WorkMeeting
{
    public interface IMeetingQuestionSerivce
    {
        MeetingQuestionEntity GetEntityByMeetingId(string meetingId);
        void Update(MeetingQuestionEntity data);
        void Insert(MeetingQuestionEntity data);
        object GetStatistics(string startTime, string endTime, string deptCode);
    }
}
