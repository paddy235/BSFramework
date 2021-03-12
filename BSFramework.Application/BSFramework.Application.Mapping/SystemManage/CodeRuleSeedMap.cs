using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// 描 述：编号规则种子
    /// </summary>
    public class CodeRuleSeedMap : EntityTypeConfiguration<CodeRuleSeedEntity>
    {
        public CodeRuleSeedMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_CODERULESEED");
            //主键
            this.HasKey(t => t.RuleSeedId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
