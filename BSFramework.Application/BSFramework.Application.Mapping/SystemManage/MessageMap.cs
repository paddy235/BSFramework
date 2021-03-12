using BSFramework.Application.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SystemManage
{
    public class MessageMap : EntityTypeConfiguration<MessageEntity>
    {
        public MessageMap()
        {
            this.ToTable("WG_MESSAGE");
            this.HasKey(t => t.MessageId);

        }
    }
}
