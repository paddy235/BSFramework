using BSFramework.Application.Entity.EducationManage;
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
    public class EducationAppraiseService : BaseService
    {
        public EducationAppraiseService(string messagekey, string businessId)
            : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            
            return string.Empty;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as EduBaseInfoEntity;
            return string.Format("{0}", entity.Theme);
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<EduBaseInfoEntity>()
                        where q.ID == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as EduBaseInfoEntity;
            if (entity == null) return null;
            return new string[] { entity.BZId };
        }
    }
}
