using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：机构管理
    /// </summary>
    public class OrganizeMap : EntityTypeConfiguration<OrganizeEntity>
    {
        public OrganizeMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_ORGANIZE");
            //主键
            this.HasKey(t => t.OrganizeId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
