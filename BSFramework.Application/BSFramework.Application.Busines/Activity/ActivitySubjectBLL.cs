using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    public class ActivitySubjectBLL
    {
        private IActivitySubjectService _service;

        public ActivitySubjectBLL()
        {
            _service = new ActivitySubjectService();
        }

        public List<ActivitySubjectEntity> GetSubjects(int pagesize, int pageindex, out int total)
        {
            return _service.GetSubjects(pagesize, pageindex, out total);
        }

        public List<ActivitySubjectEntity> GetActiveSubjects()
        {
            return _service.GetActiveSubjects();
        }

        public void EditSubject(ActivitySubjectEntity activitySubjectEntity)
        {
            _service.EditSubject(activitySubjectEntity);
        }

        public ActivitySubjectEntity Get(string id)
        {
            return _service.Get(id);
        }

        public void DeleteSubject(string id)
        {
            _service.DeleteSubject(id);
        }
    }
}
