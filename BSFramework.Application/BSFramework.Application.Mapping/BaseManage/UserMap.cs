using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class UserMap : EntityTypeConfiguration<UserEntity>
    {
        public UserMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_USER");
            //主键
            this.HasKey(t => t.UserId);  
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
