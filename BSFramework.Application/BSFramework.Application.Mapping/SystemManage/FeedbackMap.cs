using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// 描 述：意见反馈
    /// </summary>
    public class FeedbackMap : EntityTypeConfiguration<FeedbackEntity>
    {
        public FeedbackMap()
        {
            #region 表、主键
            //表
            this.ToTable("BIS_OPINION");
            //主键
            this.HasKey(t => t.OpinionId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
