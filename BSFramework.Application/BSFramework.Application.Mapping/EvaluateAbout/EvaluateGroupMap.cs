using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EvaluateGroupMap : EntityTypeConfiguration<EvaluateGroupEntity>
    {
        public EvaluateGroupMap()
        {
            #region ������
            //��
            this.ToTable("WG_EVALUATEGROUP");
            //����
            this.HasKey(t => t.EvaluateGroupId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
