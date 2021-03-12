using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class WorkInnovationService : BaseService
    {
        public WorkInnovationService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var data = this.BusinessData as WorkInnovationEntity;
            var db = new RepositoryFactory().BaseRepository();
            var audit = (from q in db.IQueryable<WorkInnovationAuditEntity>()
                         where q.innovationid == data.innovationid
                         orderby q.sort ascending
                         select q).ToList();

            //审核通过或不通过时  返回申报人userid  
            if (audit[0].state == "审核不通过")
            {
                return data.reportuserid;
            }
            if (audit[1].state == "审核通过")
            {
                return data.reportuserid;
            }
            if (audit[1].state == "审核不通过")
            {
                return data.reportuserid;
            }
            //否则 返回一级审核人userid
            return audit[0].userid;
        }

        public override string GetContent()
        {
            var data = this.BusinessData as WorkInnovationEntity;
            var db = new RepositoryFactory().BaseRepository();
            var audit = (from q in db.IQueryable<WorkInnovationAuditEntity>()
                         where q.innovationid == data.innovationid
                         orderby q.sort ascending
                         select q).ToList();

            if (audit[0].state == "审核不通过")
            {
                return data.name;
            }
            if (audit[1].state == "审核通过")
            {
                return data.name;
            }
            if (audit[1].state == "审核不通过")
            {
                return data.name;
            }
            return data.reportuser;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var data = (from q in db.IQueryable<WorkInnovationEntity>()
                        where q.innovationid == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var data = this.BusinessData as WorkInnovationEntity;
            return new string[] { data.deptid };
        }

    }
}
