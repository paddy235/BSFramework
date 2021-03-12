

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
    /// Ãè Êö£ºÈÎÎñ
    /// </summary>
    public interface ITaskService
    {
        List<MeetingJobEntity> GetListByDeptId(string deptid, int pagesize, int page, out int total);
        void Delete(string jobid);
    }
}
