using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板内容表
    /// </summary>
    public class WFSchemeContentMap: EntityTypeConfiguration<WFSchemeContentEntity>
    {
        public WFSchemeContentMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_SCHEMECONTENT");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
