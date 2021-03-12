using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SystemManage
{
    public class MessageEntity
    {
        public string MessageId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string BusinessId { get; set; }
        public bool IsFinished { get; set; }
        public bool HasReaded { get; set; }
        public int Category { get; set; }
        public string MessageKey { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
