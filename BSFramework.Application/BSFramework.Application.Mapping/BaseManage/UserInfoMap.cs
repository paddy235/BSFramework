using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class UserInfoMap : EntityTypeConfiguration<UserInfoEntity>
    {
        public UserInfoMap() 
        {
            #region 表、主键
            //表
            this.ToTable("V_USERINFO");
            //主键
            this.HasKey(t => t.UserId);  
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
