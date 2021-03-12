using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;
namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：工作流委托记录表
    /// </summary>
    public class WFDelegateRecordMap : EntityTypeConfiguration<WFDelegateRecordEntity>
    {
        public WFDelegateRecordMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_DELEGATERECORD");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
