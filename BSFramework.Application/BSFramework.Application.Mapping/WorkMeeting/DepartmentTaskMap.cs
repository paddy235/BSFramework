using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class DepartmentTaskMap : EntityTypeConfiguration<DepartmentTaskEntity>
    {
        public DepartmentTaskMap()
        {
            #region ������
            //��
            this.ToTable("WG_DEPARTMENTTASK");
            //����
            this.HasKey(t => t.TaskId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }

}
