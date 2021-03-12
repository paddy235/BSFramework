using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using BSFramework.Application.Entity.Activity;
using System.Data;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Busines.BaseManage;
using System.Linq;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;

namespace BSFramework.Busines.WorkMeeting
{
    /// <summary>
    /// ÆÀ¼Û
    /// </summary>
    public class ActivityEvaluateBLL
    {

        private readonly IActivityEvaluateService activityEvaluateService;

        public ActivityEvaluateBLL()
        {
            activityEvaluateService = new ActivityEvaluateService();
        }

        public string GetEvaluateStatus(string id, string userid)
        {
            return activityEvaluateService.GetEvaluateStatus(id, userid);
        }
    }
}