using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class WeekReportService : BaseService
    {
        public WeekReportService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var report = this.BusinessData as Report;
            return report.ReportUserId;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var report = this.BusinessData as Report;
            return string.Format("周工作总结 {0}-{1}", report.StartTime.ToString("M-dd"), report.EndTime.AddDays(-1).ToString("M-dd"));
        }

        public override object GetData(string businessId)
        {
            using (var ctx = new DataContext())
            {
                var data = ctx.Reports.Find(Guid.Parse(businessId));
                return data;
            }
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var report = this.BusinessData as Report;

            if (report == null) return null;

            var db = new RepositoryFactory().BaseRepository();
            var user = (from q in db.IQueryable<UserEntity>()
                        where q.UserId == report.ReportUserId
                        select q).FirstOrDefault();

            if (user == null) return null;
            return new string[] { user.DepartmentId };
        }
    }
}
