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
    public class EducationEvaluateService : BaseService
    {
        public EducationEvaluateService(string messagekey, string businessId)
            : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            var entity = this.BusinessData as EduBaseInfoEntity;
            return entity.TeacherId;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as EduBaseInfoEntity;
            var content = entity.Theme;
            if (entity.EduType == "7") 
            {
                content = string.Format("{0}的考问讲解答案已提交，请您及时处理", entity.RegisterPeople);
            }
            return string.Format("{0}", content);
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
