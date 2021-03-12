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
            #region 表、主键
            //表
            this.ToTable("WG_FAULTJOB");
            //主键
            this.HasKey(t => t.RelationId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
