using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 描 述：角色管理
    /// </summary>
    public class RoleMap : EntityTypeConfiguration<RoleEntity>
    {
        public RoleMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_ROLE");
            //主键
            this.HasKey(t => t.RoleId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
