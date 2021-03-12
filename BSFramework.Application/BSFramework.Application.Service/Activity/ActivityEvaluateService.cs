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
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.EF;
using System.IO;
using System.Net.Http;
using System.ComponentModel.Design.Serialization;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.IService.Activity;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class ActivityEvaluateService : RepositoryFactory<ActivityEvaluateEntity>, IActivityEvaluateService
    {
        private System.Data.Entity.DbContext _context;

        public ActivityEvaluateService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public string GetEvaluateStatus(string id, string userid)
        {
            var count = _context.Set<ActivityEvaluateEntity>().Where(x => x.Activityid == id && x.CREATEUSERID == userid).Count();
            if (count > 0) return "本人已评价";
            else return "本人未评价";
        }
    }
}