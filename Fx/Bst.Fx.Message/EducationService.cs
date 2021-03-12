using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bst.Fx.Message
{
    public class EducationService : BaseService
    {
        public EducationService(string messagekey, string businessId)
            : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var entity = this.BusinessData as EduBaseInfoEntity;
            return entity.RegisterPeopleId;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as EduBaseInfoEntity;
            var content = entity.Theme;
            if (entity.EduType == "5" || entity.EduType == "6" || entity.EduType == "7") content = string.Format("您已领取到新的题目：{0}，请您及时处理", entity.Theme);
            return content;
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
