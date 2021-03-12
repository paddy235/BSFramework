using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EvaluateItemMap : EntityTypeConfiguration<EvaluateItemEntity>
    {
        public EvaluateItemMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EVALUATEITEM");
            //主键
            this.HasKey(t => t.EvaluateItemId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
