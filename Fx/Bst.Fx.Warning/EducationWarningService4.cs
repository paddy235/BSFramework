using Bst.Fx.IWarning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bst.Fx.WarningData;
using Bst.Bzzd.DataSource;
using Bst.Fx.IMessage;
using Bst.Fx.Message;
using Bst.ServiceContract.MessageQueue;
using System.ServiceModel;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EducationManage;

namespace Bst.Fx.Warning
{
    public class EducationWarningService4 : IEducationWarningService
    {
        private string key = "4";
        public void Execute(string category)
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var db = new RepositoryFactory().BaseRepository();

            var query1 = from q in db.IQueryable<EduBaseInfoEntity>()
                         where q.EduType == key && q.ActivityDate > @from
                         select q;


            var query2 = from q in db.IQueryable<DepartmentEntity>()
                         where q.Nature == "班组" && !query1.Any(x => x.BZId == q.DepartmentId)
                         select q.DepartmentId;

            using (var factory = new ChannelFactory<IMsgService>("message"))
            {
                foreach (var item in query2)
                {
                    var proxy = factory.CreateChannel();
                    proxy.Send(category, item);
                }
            }
        }
    }
}
