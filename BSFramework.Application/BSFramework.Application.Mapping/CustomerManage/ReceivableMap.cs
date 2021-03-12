using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// 描 述：应收账款
    /// </summary>
    public class ReceivableMap : EntityTypeConfiguration<ReceivableEntity>
    {
        public ReceivableMap()
        {
            #region 表、主键
            //表
            this.ToTable("CLIENT_RECEIVABLE");
            //主键
            this.HasKey(t => t.ReceivableId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}