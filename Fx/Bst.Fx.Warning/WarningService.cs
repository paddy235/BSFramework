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

namespace Bst.Fx.Warning
{
    public class WarningService : IWarningService
    {
        public void Execute()
        {
            using (var ctx = new DataContext())
            {
                var configs = ctx.WarningConfigs.ToList();
                foreach (var item in configs)
                {
                    if (!item.Enabled) continue;

                    var type = Type.GetType(item.Assembly);
                    var ins = Activator.CreateInstance(type, item.MessageKey) as WarningBase;

                    ins.SendWarning();
                }
            }
        }

        public void SendMessage()
        {
            using (var ctx = new DataContext())
            {
                var warnings = (from q in ctx.Warnings
                                where q.IsPublished == false
                                select q).ToList();

                //不使用消息对列
                IMessageService messageservice = new MessageService();
                //使用消息对列
                //using (var factory = new ChannelFactory<IMsgService>("message"))
                //{
                //    var proxy = factory.CreateChannel();

                foreach (var item in warnings)
                {
                    messageservice.SendMessage(item.MessageKey, item.BusinessId);
                    //proxy.Send(item.MessageKey, item.BusinessId);

                    item.IsPublished = true;
                }
                //}

                ctx.SaveChanges();
            }
        }

        public void SendWarning(string messagekey, string businessId)
        {
            using (var ctx = new DataContext())
            {
                ctx.Warnings.Add(new Bzzd.DataSource.Entities.Warning() { WarningId = Guid.NewGuid(), BusinessId = businessId, MessageKey = messagekey });
                ctx.SaveChanges();
            }
        }
    }
}
