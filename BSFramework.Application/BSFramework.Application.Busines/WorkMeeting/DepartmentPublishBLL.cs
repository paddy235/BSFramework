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
    public class DepartmentPublishBLL
    {
        private IDepartmentPublishService service = new DepartmentPublishService();

        public DepartmentPublishEntity Add(DepartmentPublishEntity model)
        {
            return service.Add(model);
        }

        public DepartmentPublishEntity Edit(DepartmentPublishEntity model)
        {
            return service.Edit(model);
        }

        public void Delete(string data)
        {
            service.Delete(data);
        }

        public List<DepartmentPublishEntity> List(string deptId)
        {
            return service.List(deptId);
        }
    }
}
