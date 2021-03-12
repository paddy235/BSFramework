using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;
namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板权限表
    /// </summary>
    public class WFSchemeInfoAuthorizeMap : EntityTypeConfiguration<WFSchemeInfoAuthorizeEntity>
    {
        public WFSchemeInfoAuthorizeMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_SCHEMEINFOAUTHORIZE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
