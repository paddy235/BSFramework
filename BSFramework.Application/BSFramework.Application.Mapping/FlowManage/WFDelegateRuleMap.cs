using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：工作流委托规则表
    /// </summary>
    public class WFDelegateRuleMap : EntityTypeConfiguration<WFDelegateRuleEntity>
    {
        public WFDelegateRuleMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_DELEGATERULE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
