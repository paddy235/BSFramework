using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    public class EdActivitySupplyMap : EntityTypeConfiguration<EdActivitySupplyEntity>
    {
        public EdActivitySupplyMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EDACTIVITYSUPPLY");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
