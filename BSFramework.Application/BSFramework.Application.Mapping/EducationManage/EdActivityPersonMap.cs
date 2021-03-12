
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EdActivityPersonMap : EntityTypeConfiguration<EdActivityPersonEntity>
    {
        public EdActivityPersonMap()
        {
            #region ������
            //��
            this.ToTable("WG_EDACTIVITYPERSON");
            //����
            this.HasKey(t => t.ActivityPersonId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
