using BSFramework.Application.Entity.PerformanceManage;
using System.Data.Entity.ModelConfiguration;
namespace BSFramework.Application.Mapping.PerformanceManage
{
    public class PerformancesetupMap : EntityTypeConfiguration<PerformancesetupEntity>
    {

        public PerformancesetupMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_PERFORMANCESETUP");
            //主键
            this.HasKey(t => t.performancetypeid);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
