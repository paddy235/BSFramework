using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Activity
{
    /// <summary>
    /// �������Ԥ�ؿ�
    /// </summary>
    public class HumanDangerMap : EntityTypeConfiguration<HumanDangerEntity>
    {
        public HumanDangerMap()
        {
            #region ������
            //��
            this.ToTable("WG_HUMANDANGER");
            //����
            this.HasKey(t => t.HumanDangerId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
    public class HumanDangerMeasureMap : EntityTypeConfiguration<HumanDangerMeasureEntity>
    {
        public HumanDangerMeasureMap()
        {
            #region ������
            //��
            this.ToTable("WG_HUMANDANGERMEASURE");
            //����
            this.HasKey(t => t.HumanDangerMeasureId);
            #endregion

            #region ���ù�ϵ

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
