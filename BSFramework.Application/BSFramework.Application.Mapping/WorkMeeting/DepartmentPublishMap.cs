using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class DepartmentPublishMap : EntityTypeConfiguration<DepartmentPublishEntity>
    {
        public DepartmentPublishMap()
        {
            #region ������
            //��
            this.ToTable("WG_DEPARTMENTPUBLISH");
            //����
            this.HasKey(t => t.PublishId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
