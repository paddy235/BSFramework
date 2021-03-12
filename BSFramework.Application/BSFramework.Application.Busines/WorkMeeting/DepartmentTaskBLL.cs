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
    public class DepartmentTaskBLL
    {
        public List<DepartmentTaskEntity> List1(string deptid, DateTime? startdate, DateTime? enddate, string status, int pagesize, int page, out int total)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            return service.List1(deptid, startdate, enddate, status, pagesize, page, out total);
        }
        public List<DepartmentTaskEntity> List2(string userid, DateTime? startdate, DateTime? enddate, string status, int pagesize, int page, out int total)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            return service.List2(userid, startdate, enddate, status, pagesize, page, out total);
        }

        public void Edit(DepartmentTaskEntity model)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            service.Edit(model);
        }

        public DepartmentTaskEntity Detail(string id)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            return service.Detail(id);
        }

        public void Cancel(string id, string user)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            service.Cancel(id, user);
        }

        public List<DepartmentTaskEntity> List(string deptid, string userid, int pagesize, int pageindex, out int total)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            return service.List(deptid, userid, pagesize, pageindex, out total);
        }

        public void Complete(DepartmentTaskEntity model)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            service.Complete(model);
        }

        public void Publish(List<DepartmentTaskEntity> list)
        {
            IDepartmentTaskService service = new DepartmentTaskService();
            service.Publish(list);
        }
    }
}
