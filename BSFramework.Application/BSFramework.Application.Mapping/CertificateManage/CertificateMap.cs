
using BSFramework.Application.Entity.CertificateManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CertificateManage
{
  public   class CertificateMap : EntityTypeConfiguration<CertificateEntity>
    {
        public CertificateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_CERTIFICATE");
            //主键
            this.HasKey(t => t.CertificateId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
