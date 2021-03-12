﻿using BSFramework.Application.Entity.YearWorkPlan;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.YearWorkPlanManage
{
 public   class YearWorkPlanMap : EntityTypeConfiguration<YearWorkPlanEntity>
    {
        public YearWorkPlanMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_YEARWORKPLAN");
            //主键
            this.HasKey(t => t.id);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
