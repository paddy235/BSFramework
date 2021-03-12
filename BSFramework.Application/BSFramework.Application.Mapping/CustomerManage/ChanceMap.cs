using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// 描 述：商机信息
    /// </summary>
    public class ChanceMap : EntityTypeConfiguration<ChanceEntity>
    {
        public ChanceMap()
        {
            #region 表、主键
            //表
            this.ToTable("CLIENT_CHANCE");
            //主键
            this.HasKey(t => t.ChanceId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
