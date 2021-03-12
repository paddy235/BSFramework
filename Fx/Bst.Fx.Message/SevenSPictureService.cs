using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class SevenSPictureService : BaseService
    {
        public SevenSPictureService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {

            return null;
        }

        public override string GetContent()
        {
            var data = this.BusinessData as SevenSPictureEntity;
            return data.evaluationUser;
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var data = (from q in db.IQueryable<SevenSPictureEntity>()
                        where q.Id == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var data = this.BusinessData as SevenSPictureEntity;
            return new string[] { data.deptid };
        }


    }
}
