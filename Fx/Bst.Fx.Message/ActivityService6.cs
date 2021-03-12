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
    public class ActivityService6 : BaseService
    {
        public ActivityService6(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var deptid = this.BusinessData as string;
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<UserEntity>()
                        where q.DepartmentId == deptid
                        select q;
            var data = query.ToList();
            return string.Join(",", data.Select(x => x.UserId));
        }

        public override string GetContent()
        {
            return string.Empty;
        }

        public override object GetData(string businessId)
        {
            return businessId;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var deptid = this.BusinessData as string;

            return new string[] { deptid };
        }
    }
}
