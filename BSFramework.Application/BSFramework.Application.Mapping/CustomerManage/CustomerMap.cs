using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// �� �����ͻ���Ϣ
    /// </summary>
    public class CustomerMap : EntityTypeConfiguration<CustomerEntity>
    {
        public CustomerMap()
        {
            #region ��������
            //��
            this.ToTable("CLIENT_CUSTOMER");
            //����
            this.HasKey(t => t.CustomerId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}