using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class ActivitySourceService : BaseService
    {
        public ActivitySourceService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            return string.Empty;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var activitysource = this.BusinessData as SafetydayEntity;
            return activitysource.ActivityType;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<SafetydayEntity>()
                        where q.Id == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var activitysource = this.BusinessData as SafetydayEntity;
            if (activitysource.DeptId == null) return null;
            return activitysource.DeptId.Split(',');
        }
    }
}
