using BSFramework.Application.Entity.Activity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.Activity
{
    /// <summary>
    /// 安全日活动-改进行动
    /// </summary>
    public class ActivityActionMap : EntityTypeConfiguration<ActivityActionEntity>
    {
        public ActivityActionMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ACTIVITYACTION");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
