using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Entity.ProducingCheck;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.ProducingCheck
{
    public class CheckCategoryMap : EntityTypeConfiguration<CheckCategoryEntity>
    {
        public CheckCategoryMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_CHECKCATEGORY");
            //主键
            this.HasKey(t => t.CategoryId);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
