using BSFramework.Application.Entity.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    public interface IActivitySubjectService
    {
        List<ActivitySubjectEntity> GetSubjects(int pagesize, int pageindex, out int total);
        void EditSubject(ActivitySubjectEntity activitySubjectEntity);
        ActivitySubjectEntity Get(string id);
        void DeleteSubject(string id);
        List<ActivitySubjectEntity> GetActiveSubjects();
    }
}
