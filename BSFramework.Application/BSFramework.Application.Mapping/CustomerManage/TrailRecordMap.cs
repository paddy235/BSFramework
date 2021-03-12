using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// 描 述：商机跟进记录
    /// </summary>
    public class TrailRecordMap : EntityTypeConfiguration<TrailRecordEntity>
    {
        public TrailRecordMap()
        {
            #region 表、主键
            //表
            this.ToTable("CLIENT_TRAILRECORD");
            //主键
            this.HasKey(t => t.TrailId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}