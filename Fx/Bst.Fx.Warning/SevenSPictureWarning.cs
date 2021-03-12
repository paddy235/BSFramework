using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
 public   class SevenSPictureWarning : WarningBase
    {
        public SevenSPictureWarning(string messagekey) : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var from = DateTime.Now.Date;
            var to = @from.AddDays(1);

            var query = from q in db.IQueryable<SevenSPictureEntity>()
                        where q.planeEndDate == to&&q.state=="未提交"
                        select q;

            var data = query.ToList();
            return data.Select(x => x.Id).ToArray();
        }
    }
}
