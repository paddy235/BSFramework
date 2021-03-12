using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.WorkMeeting;

namespace BSFramework.Application.IService.WorkMeeting
{
    public interface IWorkMeetingNoteService
    {
        WorkMeetingNoteEntity GetEntity(string id);
        void Insert(WorkMeetingNoteEntity noteEntity);
        void Update(WorkMeetingNoteEntity noteEntity);
        WorkMeetingNoteEntity GetEntityByMeetingId(string meetingid);
    }
}
