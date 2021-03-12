using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Cache.Factory;
using Bst.Fx.IMessage;
using Bst.Fx.Message;
using Bst.Fx.MessageData;
using Bst.ServiceContract.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace BSFramework.Application.Busines.SystemManage
{
    /// <summary>
    /// 描 述：消息
    /// </summary>
    public class MessageBLL
    {
        public MessageEntity GetMessageDetail(Guid messageid)
        {
            IMessageService service = new MessageService();
            return service.GetMessageDetail(messageid);
        }

        public List<MessageEntity> GetMessage(string userid, out int total)
        {
            IMessageService service = new MessageService();
            return service.GetMessage(userid, out total);
        }

        public List<MessageEntity> GetTodo(string userid, out int total)
        {
            IMessageService service = new MessageService();
            return service.GetTodo(userid, out total);
        }

        public void FinishTodo(string messagekey, string businessId)
        {
            IMessageService service = new MessageService();
            service.FinishTodo(messagekey, businessId);
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="messagekey">消息组</param>
        /// <param name="businessId">业务数据</param>
        public void SendMessage(string messagekey, string businessId)
        {
            //IMessageService service = new MessageService();
            //service.SendMessage(messagekey, businessId);
            using (var factory = new ChannelFactory<IMsgService>("message"))
            {
                var proxy = factory.CreateChannel();
                proxy.Send(messagekey, businessId);
            }
        }

        public int GetMessageTotal(string userId)
        {
            IMessageService service = new MessageService();
            return service.GetMessageTotal(userId);
        }
    }
}
