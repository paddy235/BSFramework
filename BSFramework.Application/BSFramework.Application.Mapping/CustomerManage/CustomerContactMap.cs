using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// 描 述：客户联系人
    /// </summary>
    public class CustomerContactMap : EntityTypeConfiguration<CustomerContactEntity>
    {
        public CustomerContactMap()
        {
            #region 表、主键
            //表
            this.ToTable("CLIENT_CUSTOMERCONTACT");
            //主键
            this.HasKey(t => t.CustomerContactId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}