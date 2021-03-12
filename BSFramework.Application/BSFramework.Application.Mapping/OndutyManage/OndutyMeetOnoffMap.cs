﻿using BSFramework.Application.Entity.OndutyManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.OndutyManage
{
    public class OndutyMeetOnoffMap : EntityTypeConfiguration<OndutyMeetOnoffEntity>
    {
        public OndutyMeetOnoffMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ONDUTYMEETONOFF");
            //主键
            this.HasKey(t => t.id);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
