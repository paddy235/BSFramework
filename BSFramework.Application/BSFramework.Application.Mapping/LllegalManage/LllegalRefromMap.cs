﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.LllegalManage;

namespace BSFramework.Application.Mapping.LllegalManage
{
    public class LllegalRefromMap : EntityTypeConfiguration<LllegalRefromEntity>
    {

        public LllegalRefromMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_LLLEGALREFROM");
            //主键
            this.HasKey(t => t.Id);
            #endregion
        }
    }
}
