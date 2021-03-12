using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PublicInfoManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public class PackageMap : EntityTypeConfiguration<PackageEntity>
    {
        public PackageMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_PACKAGE");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
