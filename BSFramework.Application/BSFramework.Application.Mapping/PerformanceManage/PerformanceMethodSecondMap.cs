using BSFramework.Application.Entity.PerformanceManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PerformanceManage
{
   public class PerformanceMethodSecondMap : EntityTypeConfiguration<PerformanceMethodSecondEntity>
    {
        public PerformanceMethodSecondMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_PERFORMANCEMETHOD_SECOND");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
