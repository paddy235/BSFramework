using BSFramework.Application.Entity.PerformanceManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PerformanceManage
{
   public class PerformanceMap : EntityTypeConfiguration<PerformanceEntity>
    {
        public PerformanceMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_PERFORMANCE");
            //主键
            this.HasKey(t => t.performanceid);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
