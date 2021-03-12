using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WorkMeeting
{
    public interface IMeetingRecordService
    {
        void Insert(MeetingRecordEntity data);
        void Update(MeetingRecordEntity data);
        MeetingRecordEntity GetEntityByMeetingId(string meetingId);
        object GetStatistics(string startTime, string endTime, string deptCode);
    }
}
