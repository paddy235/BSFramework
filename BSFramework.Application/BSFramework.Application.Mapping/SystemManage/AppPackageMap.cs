using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public class AppPackageMap : EntityTypeConfiguration<AppPackageEntity>
    {
        public AppPackageMap()
        {
            #region 表、主键
            //表
            this.ToTable("BIS_PACKAGE");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
