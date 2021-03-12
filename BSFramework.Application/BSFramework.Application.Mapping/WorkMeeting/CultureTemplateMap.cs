using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class CultureTemplateMap : EntityTypeConfiguration<CultureTemplateEntity>
    {
        public CultureTemplateMap()
        {
            #region ������
            //��
            this.ToTable("WG_CULTURETEMPLATE");
            //����
            this.HasKey(t => t.CultureTemplateId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
    public class CultureTemplateItemMap : EntityTypeConfiguration<CultureTemplateItemEntity>
    {
        public CultureTemplateItemMap()
        {
            #region ������
            //��
            this.ToTable("WG_CULTURETEMPLATEITEM");
            //����
            this.HasKey(t => t.CultureTemplateItemId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
