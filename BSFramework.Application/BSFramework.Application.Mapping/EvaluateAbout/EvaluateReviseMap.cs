﻿using BSFramework.Application.Entity.EvaluateAbout;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.EvaluateAbout
{
 public   class EvaluateReviseMap : EntityTypeConfiguration<EvaluateReviseEntity>
    {
        public EvaluateReviseMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EVALUATEREVISE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
