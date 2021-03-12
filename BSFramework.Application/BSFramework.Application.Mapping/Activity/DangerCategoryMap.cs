using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Activity
{
    /// <summary>
    /// 人身风险预控库
    /// </summary>
    public class DangerCategoryMap : EntityTypeConfiguration<DangerCategoryEntity>
    {
        public DangerCategoryMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DANGERCATEGORY");
            //主键
            this.HasKey(t => t.CategoryId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
