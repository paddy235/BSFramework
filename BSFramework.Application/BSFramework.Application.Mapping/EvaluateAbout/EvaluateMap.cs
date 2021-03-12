using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EvaluateMap : EntityTypeConfiguration<EvaluateEntity>
    {
        public EvaluateMap()
        {
            #region ������
            //��
            this.ToTable("WG_EVALUATE");
            //����
            this.HasKey(t => t.EvaluateId);
            #endregion

            #region ���ù�ϵ
            this.Ignore(x => x.CanScore);
            this.Ignore(x => x.CanEdit);
            #endregion
        }
    }
}
