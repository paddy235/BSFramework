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

namespace BSFramework.Busines.WorkMeeting
{
    public class ApproveRecordBLL
    {
        private IApproveRecordService _service;

        public ApproveRecordBLL()
        {
            _service = new ApproveRecordService();
        }

        public List<ApproveRecordEntity> List(string record)
        {
            return _service.List(record);
        }
    }
}