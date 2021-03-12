using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EvaluateItemMap : EntityTypeConfiguration<EvaluateItemEntity>
    {
        public EvaluateItemMap()
        {
            #region ������
            //��
            this.ToTable("WG_EVALUATEITEM");
            //����
            this.HasKey(t => t.EvaluateItemId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
