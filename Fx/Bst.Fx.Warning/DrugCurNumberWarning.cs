using BSFramework.Application.Entity.DrugManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
    public class DrugCurNumberWarning : WarningBase
    {
        public DrugCurNumberWarning(string messagekey)
            : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var to = DateTime.Now.Date;
            //var to = @from.AddDays(1).AddMinutes(-1);
            
            var query = from q in db.IQueryable<DrugEntity>()
                        select q;

            var data = query.ToList();
            var ndata = new List<DrugEntity>();
            foreach (DrugEntity d in data) 
            {
                if (string.IsNullOrEmpty(d.StockWarn)) continue;
                if (d.DrugNum <= int.Parse(d.StockWarn)) ndata.Add(d);
            }
            return ndata.Select(x => x.Id).ToArray();
        }
    }
}
