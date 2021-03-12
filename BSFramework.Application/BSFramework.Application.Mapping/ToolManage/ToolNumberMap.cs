using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ToolManage;

namespace BSFramework.Application.Mapping.ToolManage
{
    public class ToolNumberMap : EntityTypeConfiguration<ToolNumberEntity>
    {
        public ToolNumberMap() 
        {
            this.ToTable("WG_TOOLNUMBER");
            //主键
            this.HasKey(t => t.ID);
        }
    }
}
