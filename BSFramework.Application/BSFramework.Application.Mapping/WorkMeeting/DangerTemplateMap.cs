using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class DangerTemplateMap : EntityTypeConfiguration<DangerTemplateEntity>
    {
        public DangerTemplateMap()
        {
            #region ������
            //��
            this.ToTable("WG_DANGERTEMPLATE");
            //����
            this.HasKey(t => t.DangerId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
