using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
    public class KwjjWarning : WarningBase
    {
        public KwjjWarning(string messagekey)
            : base(messagekey)
        {
        }

        public override string[] FindWaringData()
        {
            var db = new RepositoryFactory().BaseRepository();

            var from = DateTime.Now.Date;
            var to = @from.AddDays(1).AddMinutes(-1);

            var query = from q in db.IQueryable<EduBaseInfoEntity>()
                        where q.ActivityEndDate >= @from && q.ActivityEndDate <= to && (q.EduType == "7" ) && q.AnswerFlow == "0"
                        select q;
            var data = query.ToList();
            return data.Select(x => x.ID).ToArray();
        }
    }
}
