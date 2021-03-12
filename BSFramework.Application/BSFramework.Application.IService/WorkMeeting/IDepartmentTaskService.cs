
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.IService.WorkMeeting
{
    public interface IDepartmentTaskService
    {
        List<DepartmentTaskEntity> List1(string deptid, DateTime? startdate, DateTime? enddate, string status, int pagesize, int page, out int total);
        void Edit(DepartmentTaskEntity model);
        DepartmentTaskEntity Detail(string id);
        void Cancel(string id, string user);
        List<DepartmentTaskEntity> List2(string userid, DateTime? startdate, DateTime? enddate, string status, int pagesize, int page, out int total);
        List<DepartmentTaskEntity> List(string deptid, string userid, int pagesize, int pageindex, out int total);
        void Complete(DepartmentTaskEntity model);
        void Publish(List<DepartmentTaskEntity> list);
    }
}
