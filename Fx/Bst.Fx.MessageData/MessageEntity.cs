using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.MessageData
{
    public class MessageEntity
    {
        public Guid MessageId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string BusinessId { get; set; }
        public DateTime CreateTime { get; set; }
        public string MessageKey { get; set; }
    }
}
