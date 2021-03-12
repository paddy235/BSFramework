using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public class ToolsService : BaseService
    {
        public ToolsService(string messagekey, string businessId) : base(messagekey, businessId)
        { }

        public override string GetBusinessUserId()
        {
            return string.Empty;
        }

        public override string GetContent()
        {
            if (this.BusinessData == null) return null;
            var tool = this.BusinessData as ToolInfoEntity;
            return string.Format("{0}({1})", tool.Name, tool.Spec);
        }

        public override object GetData(string businessId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = (from q in db.IQueryable<ToolInfoEntity>()
                        where q.ID == businessId
                        select q).FirstOrDefault();
            return data;
        }

        public override string[] GetDeptId()
        {
            if (this.BusinessData == null) return null;
            var tool = this.BusinessData as ToolInfoEntity;
            if (tool == null) return null;
            return new string[] { tool.BZID };
        }
    }
}
