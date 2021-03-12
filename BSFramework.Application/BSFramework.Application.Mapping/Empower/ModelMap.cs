using BSFramework.Application.Entity.Empower;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Empower
{
    public class ModelMap : EntityTypeConfiguration<ModelEntity>
    {
        public ModelMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_MODEL");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
