using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Data;
using BSFramework.Data;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using System.Text;
using BSFramework.Application.Service.PublicInfoManage;
using Bst.ServiceContract.MessageQueue;
using System.ServiceModel;
using Newtonsoft.Json.Linq;
using BSFramework.Application.Entity.SystemManage;

namespace BSFramework.Service.WorkMeeting
{
    public class ApproveRecordService : IApproveRecordService
    {
        public List<ApproveRecordEntity> List(string record)
        {
            var db = DbFactory.Base();

            var query = from q in db.IQueryable<ApproveRecordEntity>()
                        where q.RecordId == record
                        orderby q.ApproveTime ascending
                        select q;

            return query.ToList();
        }
    }
}