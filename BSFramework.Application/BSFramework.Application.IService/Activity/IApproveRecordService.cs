using BSFramework.Application.Entity.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using System.Data;

namespace BSFramework.IService.WorkMeeting
{
    public interface IApproveRecordService
    {
        List<ApproveRecordEntity> List(string record);
    }
}