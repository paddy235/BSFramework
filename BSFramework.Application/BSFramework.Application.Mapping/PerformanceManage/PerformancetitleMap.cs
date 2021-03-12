using BSFramework.Application.Entity.PerformanceManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PerformanceManage
{
   public class PerformancetitleMap : EntityTypeConfiguration<PerformancetitleEntity>
    {
        public PerformancetitleMap() {

            #region 表、主键
            //表
            this.ToTable("WG_PERFORMANCETITLE");
            //主键
            this.HasKey(t => t.titleid);
            #endregion

            #region 配置关系
            #endregion

        }


    }
}
