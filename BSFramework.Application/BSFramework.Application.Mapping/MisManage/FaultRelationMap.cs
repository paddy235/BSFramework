using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.MisManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.MisManage
{
    /// <summary>
    /// 
    /// </summary>
    public class FaultRelationMap : EntityTypeConfiguration<FaultRelationEntity>
    {
        public FaultRelationMap()
        {
            #region ������
            //��
            this.ToTable("WG_FAULTJOB");
            //����
            this.HasKey(t => t.RelationId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
