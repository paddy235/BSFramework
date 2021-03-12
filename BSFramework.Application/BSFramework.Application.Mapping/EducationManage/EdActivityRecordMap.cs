
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EdActivityRecordMap : EntityTypeConfiguration<EdActivityRecordEntity>
    {
        public EdActivityRecordMap()
        {
            #region ������
            //��
            this.ToTable("WG_EDACTIVITYRECORD");
            //����
            this.HasKey(t => t.ActivityRecordId);
            #endregion

            #region ���ù�ϵ
            //this.HasRequired(t => t.Activity).WithMany(t => t.ActivityRecords).HasForeignKey(t => t.ActivityId).WillCascadeOnDelete(true);
            #endregion
        }
    }
}
