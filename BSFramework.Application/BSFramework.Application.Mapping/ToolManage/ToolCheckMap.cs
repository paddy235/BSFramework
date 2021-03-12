using BSFramework.Application.Entity.ToolManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.ToolManage
{
    public class ToolCheckMap : EntityTypeConfiguration<ToolCheckEntity>
    {
        public ToolCheckMap() 
        {
            this.ToTable("WG_TOOLCHECK");
            //主键
            this.HasKey(t => t.ID);
        }
    }
}
