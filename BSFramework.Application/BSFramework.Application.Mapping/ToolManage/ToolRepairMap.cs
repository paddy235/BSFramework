﻿using BSFramework.Application.Entity.ToolManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.ToolManage
{
    public class ToolRepairMap : EntityTypeConfiguration<ToolRepairEntity>
    {
        public ToolRepairMap() 
        {
            this.ToTable("WG_TOOLREPAIR");
            //主键
            this.HasKey(t => t.ID);
        }
    }
}
