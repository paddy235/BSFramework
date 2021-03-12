using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WorkMeeting
{
    public class WorkMeetingNoteService : RepositoryFactory<WorkMeetingNoteEntity>, IWorkMeetingNoteService
    {
        public WorkMeetingNoteEntity GetEntity(string id)
        {
            return this.BaseRepository().FindEntity(id);
        }

        public WorkMeetingNoteEntity GetEntityByMeetingId(string meetingid)
        {
            return this.BaseRepository().IQueryable().Where(p => p.MeetingId.Equals(meetingid)).FirstOrDefault();
        }

        public void Insert(WorkMeetingNoteEntity noteEntity)
        {
            this.BaseRepository().Insert(noteEntity);
        }

        public void Update(WorkMeetingNoteEntity noteEntity)
        {
            this.BaseRepository().Update(noteEntity);
        }
    }
}
