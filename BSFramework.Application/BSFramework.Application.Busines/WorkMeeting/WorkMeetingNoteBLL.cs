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
   public class WorkMeetingNoteBLL
    {
        private IWorkMeetingNoteService  service = new WorkMeetingNoteService();

        public WorkMeetingNoteEntity GetEntity(string id)
        {
            return service.GetEntity(id);
        }
        public void Insert(WorkMeetingNoteEntity noteEntity)
        {
             service.Insert(noteEntity);
        }

        public void Update(WorkMeetingNoteEntity noteEntity)
        {
            service.Update(noteEntity);
        }

        public WorkMeetingNoteEntity GetEntityByMeetingId(string meetingid)
        {
            return service.GetEntityByMeetingId(meetingid);
        }
    }
}
