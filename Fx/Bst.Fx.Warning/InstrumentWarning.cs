using BSFramework.Application.Entity.DrugManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bst.Fx.Warning
{
    public class InstrumentWarning : WarningBase
    {
        public InstrumentWarning(string messagekey)
            : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var from = DateTime.Now.Date;
            var to = @from.AddDays(1).AddMinutes(-1);

            var query = from q in db.IQueryable<InstrumentEntity>()
                        where q.Validate >= @from && q.Validate <= to
                        select q;

            var data = query.ToList();
            return data.Select(x => x.ID).ToArray();
        }
    }
}
