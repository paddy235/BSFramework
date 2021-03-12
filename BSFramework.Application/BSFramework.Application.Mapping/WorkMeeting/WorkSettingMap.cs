using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������
    /// </summary>
    public class WorkSettingMap : EntityTypeConfiguration<WorkSettingEntity>
    {
        public WorkSettingMap()
        {
            #region ������
            //��
            this.ToTable("WG_WORKSETTING");
            //����
            this.HasKey(t => t.WorkSettingId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
