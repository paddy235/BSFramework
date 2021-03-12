using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EvaluateDeptMap : EntityTypeConfiguration<EvaluateDeptEntity>
    {
        public EvaluateDeptMap()
        {
            #region ������
            //��
            this.ToTable("WG_EVALUATEDEPT");
            //����
            this.HasKey(t => t.EvaluateDeptId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
