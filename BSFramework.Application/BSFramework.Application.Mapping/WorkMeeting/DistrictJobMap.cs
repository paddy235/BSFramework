using BSFramework.Application.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.WorkMeeting
{
    public class DistrictJobMap : EntityTypeConfiguration<DistrictJobEntity>
    {
        public DistrictJobMap()
        {
            #region ������
            //��
            this.ToTable("WG_DISTRICTJOB");
            //����
            this.HasKey(t => t.DistrictJobId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
