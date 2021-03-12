using BSFramework.Application.Entity.ToolManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
    public class ToolsWarning : WarningBase
    {
        public ToolsWarning(string messagekey) : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var from = DateTime.Now.Date;
            var to = @from.AddDays(1).AddMinutes(-1);

            var query = from q in db.IQueryable<ToolInfoEntity>()
                        where q.ValiDate >= @from && q.ValiDate <= to
                        select q;

            var data = query.ToList();
            return data.Select(x => x.ID).ToArray();
        }
    }
}
