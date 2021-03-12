using BSFramework.Application.Entity.TaskManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.TaskManage
{
    public class UnSigninMap : EntityTypeConfiguration<UnSigninEntity>
    {
        public UnSigninMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_UNSIGNIN");
            //主键
            this.HasKey(t => t.UnSigninId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
