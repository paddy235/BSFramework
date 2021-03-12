using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板信息表
    /// </summary>
    public class WFSchemeInfoMap : EntityTypeConfiguration<WFSchemeInfoEntity>
    {
        public WFSchemeInfoMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_SCHEMEINFO");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
