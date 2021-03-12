using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ReportMap : EntityTypeConfiguration<ReportEntity>
    {
        public ReportMap()
        {
            #region ������
            //��
            this.ToTable("WG_REPORT");
            //����
            this.HasKey(t => t.ReportId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }

    }


    public class ReportTaskMap : EntityTypeConfiguration<TaskEntity>
    {
        public ReportTaskMap()
        {
            #region ������
            //��
            this.ToTable("WG_REPORTTASK");
            //����
            this.HasKey(t => t.ReportTaskId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
