using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivityRecordMap : EntityTypeConfiguration<ActivityRecordEntity>
    {
        public ActivityRecordMap()
        {
            #region ������
            //��
            this.ToTable("WG_ACTIVITYRECORD");
            //����
            this.HasKey(t => t.ActivityRecordId);
            #endregion

            #region ���ù�ϵ
            //this.HasRequired(t => t.Activity).WithMany(t => t.ActivityRecords).HasForeignKey(t => t.ActivityId).WillCascadeOnDelete(true);
            #endregion
        }
    }
}
