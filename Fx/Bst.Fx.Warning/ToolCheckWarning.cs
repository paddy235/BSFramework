using BSFramework.Application.Entity.ToolManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
    public class ToolCheckWarning : WarningBase
    {
        public ToolCheckWarning(string messagekey)
            : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var to = DateTime.Now.Date;
            //var to = @from.AddDays(1).AddMinutes(-1);
            
            var query = from q in db.IQueryable<ToolInfoEntity>()
                        select q;

            var data = query.ToList();
            var ndata = new List<ToolInfoEntity>();
            foreach (ToolInfoEntity t in data) 
            {
                if (string.IsNullOrEmpty(t.Remind)||t.ValiDate==null) continue;
                to.AddDays(0 - int.Parse(t.Remind.Substring(2, t.Remind.Length - 3)));
                if (t.ValiDate >= to) ndata.Add(t);
            }
            return ndata.Select(x => x.ID).ToArray();
        }
    }
}
