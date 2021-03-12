using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class SevenSaduitService : BaseService
    {
        public SevenSaduitService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var data = this.BusinessData as SevenSOfficeEntity;
            if (data != null)
            {
                if (data.aduitstate == "待审核")
                {

                    var db = new RepositoryFactory().BaseRepository();
                    var audit = (from q in db.IQueryable<SevenSOfficeAuditEntity>()
                                 where q.officeid == data.id
                                 orderby q.sort descending
                                 select q).FirstOrDefault();
                    if (audit != null)
                    {
                        return audit.userid;
                    }
                }
            }
            return data.createuserid;
        }

        public override string GetContent()
        {
            var data = this.BusinessData as SevenSOfficeEntity;
            if (data != null)
            {
                if (data.aduitstate == "待审核")
                {

                    var db = new RepositoryFactory().BaseRepository();
                    var audit = (from q in db.IQueryable<SevenSOfficeAuditEntity>()
                                 where q.officeid == data.id
                                 orderby q.sort descending
                                 select q).FirstOrDefault();
                    if (audit != null)
                    {
                        return audit.username;
                    }
                }
            }
            return data.createusername;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var data = (from q in db.IQueryable<SevenSOfficeEntity>()
                        where q.id == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var data = this.BusinessData as SevenSOfficeEntity;
            return new string[] { data.deptid };
        }
    }
}
