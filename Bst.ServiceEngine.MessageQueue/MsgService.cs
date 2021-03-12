using Bst.Fx.IMessage;
using Bst.Fx.Message;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Bst.ServiceEngine.MessageQueue
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MsgService : IMsgService
    {
        public MsgService()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Bst.Bzzd.DataSource.DataContext, Bst.Bzzd.DataSource.Migrations.Configuration>());
            NLog.LogManager.GetCurrentClassLogger().Info("MsgService ctor");
        }

        public void Send(string messagekey, string businessId)
        {
            IMessageService msg = new MessageService();
            msg.SendMessage(messagekey, businessId);
        }
    }
}
