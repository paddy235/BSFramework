using BSFramework.Application.Entity.DrugManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
    public class GlassCurNumberWarning:WarningBase
    {
        public GlassCurNumberWarning(string messagekey)
            : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var to = DateTime.Now.Date;
            
            var query = from q in db.IQueryable<GlassEntity>()
                        select q;

            var data = query.ToList();
            var ndata = new List<GlassEntity>();
            foreach (GlassEntity d in data) 
            {
                if (string.IsNullOrEmpty(d.Warn)||string.IsNullOrEmpty(d.Amount)) continue;
                if (int.Parse(d.Amount) <= int.Parse(d.Warn)) ndata.Add(d);
            }
            return ndata.Select(x => x.ID).ToArray();
        }
    }
}
