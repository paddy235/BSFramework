using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// 描 述：订单管理
    /// </summary>
    public class OrderMap : EntityTypeConfiguration<OrderEntity>
    {
        public OrderMap()
        {
            #region 表、主键
            //表
            this.ToTable("CLIENT_ORDER");
            //主键
            this.HasKey(t => t.OrderId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}