
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EdActivityMap : EntityTypeConfiguration<EdActivityEntity>
    {
        public EdActivityMap()
        {
            #region ������
            //��
            this.ToTable("WG_EDACTIVITY");
            //����
            this.HasKey(t => t.ActivityId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
