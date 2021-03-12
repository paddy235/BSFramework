using System;
using BSFramework.Application.Entity.PerformanceManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PerformanceManage
{
    public class PerformanceupSecondMap : EntityTypeConfiguration<PerformanceupSecondEntity>
    {
        public PerformanceupSecondMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_PERFORMANCEUP_SECOND");
            //主键
            this.HasKey(t => t.id);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
