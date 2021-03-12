using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class TaskBLL
    {
        public List<MeetingJobEntity> GetListByDeptId(string deptid, int pagesize, int page, out int total)
        {
            ITaskService service = new TaskService();
            return service.GetListByDeptId(deptid, pagesize, page, out total);
        }

        public void Delete(string jobid)
        {
            ITaskService service = new TaskService();
            service.Delete(jobid);
        }
    }
}
