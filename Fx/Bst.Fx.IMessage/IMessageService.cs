using Bst.Fx.MessageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.IMessage
{
    public interface IMessageService
    {
        List<MessageEntity> GetTodo(string userid, out int total);
        List<MessageEntity> GetMessage(string userid, out int total);
        void SendMessage(string messagekey, string businessId);
        void FinishTodo(string messagekey, string businessId);
        MessageEntity GetMessageDetail(Guid messageid);
        int GetMessageTotal(string userId);
    }
}
