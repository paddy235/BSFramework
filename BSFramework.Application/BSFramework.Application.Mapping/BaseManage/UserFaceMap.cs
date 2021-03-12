using BSFramework.Application.Entity;
using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class UserFaceMap : EntityTypeConfiguration<UserFaceEntity>
    {
        public UserFaceMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_USERFACE");
            //主键
            this.HasKey(t => t.UserId);  
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
