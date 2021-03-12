using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Activity
{
    /// <summary>
    /// 人身风险预控库
    /// </summary>
    public class HumanDangerMap : EntityTypeConfiguration<HumanDangerEntity>
    {
        public HumanDangerMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_HUMANDANGER");
            //主键
            this.HasKey(t => t.HumanDangerId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
    public class HumanDangerMeasureMap : EntityTypeConfiguration<HumanDangerMeasureEntity>
    {
        public HumanDangerMeasureMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_HUMANDANGERMEASURE");
            //主键
            this.HasKey(t => t.HumanDangerMeasureId);
            #endregion

            #region 配置关系

            #endregion
        }
    }

    public class ApproveRecordMap : EntityTypeConfiguration<ApproveRecordEntity>
    {
        public ApproveRecordMap()
        {
            this.ToTable("WG_APPROVERECORD");
            this.HasKey(x => x.ApproveRecordId);
        }
    }
}
