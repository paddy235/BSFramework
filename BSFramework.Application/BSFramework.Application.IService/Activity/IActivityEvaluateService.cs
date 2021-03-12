using BSFramework.Application.Entity.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using System.Data;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// ÆÀ¼Û
    /// </summary>
    public interface IActivityEvaluateService
    {
        string GetEvaluateStatus(string id, string userid);
    }
}