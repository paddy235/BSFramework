using BSFramework.Application.Entity.PerformanceManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PerformanceManage
{
   public class PerformanceSecondMap : EntityTypeConfiguration<PerformanceSecondEntity>
    {
        public PerformanceSecondMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_PERFORMANCE_SECOND");
            //主键
            this.HasKey(t => t.performanceid);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
