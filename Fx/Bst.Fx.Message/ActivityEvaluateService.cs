using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;

namespace Bst.Fx.Message
{
    public class ActivityEvaluateService : BaseService
    {
        public ActivityEvaluateService(string messagekey, string businessId)
            : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {

            return string.Empty;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as ActivityEvaluateEntity;
            var db = new RepositoryFactory().BaseRepository();
            var eduentity = (from q in db.IQueryable<EduBaseInfoEntity>()
                             where q.ID == entity.Activityid
                             select q).FirstOrDefault();
            string type = "";
            switch (eduentity.EduType)
            {
                case "1":
                    type = "技术讲课";
                    break;
                case "2":
                    type = "技术问答（集中式）";
                    break;
                case "3":
                    type = "事故预想（集中式）";
                    break;
                case "4":
                    type = "反事故演习";
                    break;
                case "5":
                    type = "技术问答（分散式）";
                    break;
                case "6":
                    type = "事故预想（分散式）";
                    break;
                case "7":
                    type = "考问讲解";
                    break;
                case "8":
                    type = "考问讲解（集中式）";
                    break;
                default:
                    break;
            }
            return string.Format("{4}{5}台账有一条新的评价记录：{0} {1} {2}分 {3}", entity.DeptName, entity.EvaluateUser, entity.Score, entity.EvaluateContent, eduentity.ActivityDate.Value.ToString("yyyy-MM-dd"), type);
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<ActivityEvaluateEntity>()
                        where q.ActivityEvaluateId == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as ActivityEvaluateEntity;
            if (entity == null) return null;
            var db = new RepositoryFactory().BaseRepository();
            var eduentity = (from q in db.IQueryable<EduBaseInfoEntity>()
                             where q.ID == entity.Activityid
                             select q).FirstOrDefault();
            return new string[] { eduentity.BZId };
        }
    }
}
