using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// 描 述：开票信息
    /// </summary>
    public class InvoiceMap : EntityTypeConfiguration<InvoiceEntity>
    {
        public InvoiceMap()
        {
            #region 表、主键
            //表
            this.ToTable("CLIENT_INVOICE");
            //主键
            this.HasKey(t => t.InvoiceId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
