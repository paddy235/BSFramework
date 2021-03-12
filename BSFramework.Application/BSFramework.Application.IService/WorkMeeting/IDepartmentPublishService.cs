
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.IService.WorkMeeting
{
    /// <summary>
    /// 描 述：要务公开
    /// </summary>
    public interface IDepartmentPublishService
    {
        DepartmentPublishEntity Add(DepartmentPublishEntity model);
        DepartmentPublishEntity Edit(DepartmentPublishEntity model);
        void Delete(string data);
        List<DepartmentPublishEntity> List(string deptId);
    }
}
