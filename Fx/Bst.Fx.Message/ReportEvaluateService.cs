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
    public class ReportEvaluateService : BaseService
    {
        public ReportEvaluateService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var report = this.BusinessData as Report;
            return string.Join(",", report.Notices.Where(x => x.NoticeType == 1).Select(x => x.UserId));
        }

        public override string GetContent()
        {
            return string.Empty;
        }

        public override object GetData(string businessId)
        {
            var id = Guid.Parse(businessId);
            using (var ctx = new DataContext())
            {
                var report = ctx.Reports.Include("Notices").FirstOrDefault(x => x.ReportId == id);
                return report;
            }
        }

        public override string[] GetDeptId()
        {
            return null;
        }
    }
}
