using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class JobTemplateMap : EntityTypeConfiguration<JobTemplateEntity>
    {
        public JobTemplateMap()
        {
            #region ������
            //��
            this.ToTable("WG_JOBTEMPLATE");
            //����
            this.HasKey(t => t.JobId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
