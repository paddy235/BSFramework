using BSFramework.Application.Entity.AuthorizeManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.AuthorizeManage
{
    /// <summary>
    /// 描 述：授权数据范围
    /// </summary>
    public class AuthorizeDataMap : EntityTypeConfiguration<AuthorizeDataEntity>
    {
        public AuthorizeDataMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_AUTHORIZEDATA");
            //主键
            this.HasKey(t => t.AuthorizeDataId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
