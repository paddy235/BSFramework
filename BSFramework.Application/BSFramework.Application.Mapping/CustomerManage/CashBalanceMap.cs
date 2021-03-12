using BSFramework.Application.Entity.CustomerManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CustomerManage
{
    /// <summary>
    /// �� �����ֽ����
    /// </summary>
    public class CashBalanceMap : EntityTypeConfiguration<CashBalanceEntity>
    {
        public CashBalanceMap()
        {
            #region ������
            //��
            this.ToTable("CLIENT_CASHBALANCE");
            //����
            this.HasKey(t => t.CashBalanceId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
