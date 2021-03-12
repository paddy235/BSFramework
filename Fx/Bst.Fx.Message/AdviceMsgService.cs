using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class AdviceMsgService : BaseService
    {
        public AdviceMsgService(string messagekey, string businessId)
               : base(messagekey, businessId)
        { }
        public override string GetBusinessUserId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as AdviceEntity;

            if (entity.aduitresult == "待审核")
            {
                var adviceauditentity = new RepositoryFactory().BaseRepository().IQueryable<AdviceAuditEntity>(p => p.adviceid == entity.adviceid).FirstOrDefault();

                return adviceauditentity.userid;
            }
            else if (entity.aduitresult == "审核通过" || entity.aduitresult == "审核不通过" )
            {
                return entity.userid;//审核通过，通知填报人
            }
            else
            {
                return null;
            }
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as AdviceEntity;

            if (entity.aduitresult == "待审核")
            {
                return string.Format("{0}向您提交了一条建议，请您及时审批", entity.username);
            }
            else if (entity.aduitresult == "审核通过")
            {
                return string.Format("恭喜，您提交的建议已通过。{0}", entity.title);
            }
            else if (entity.aduitresult == "审核不通过")
            {
                return string.Format("抱歉，您提交的建议未通过。{0}", entity.title);
            }
            else
            {
                return null;
            }
            //if (verifytype)
            //{
            //    return string.Format("恭喜，您提交的建议已通过。{0}", entity.title);
            //}
            //else
            //{
            //    return string.Format("抱歉，您提交的建议未通过。{0}", entity.deptid);
            //}
        }


        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<AdviceEntity>()
                        where q.adviceid == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var entity = this.BusinessData as AdviceEntity;
            if (entity == null) return null;
            return new string[] { entity.deptid };
        }
    }
}
