
using BSFramework.Application.Entity.CertificateManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.CertificateManage
{
 public   class UserCertificateMap : EntityTypeConfiguration<UserCertificateEntity>
    {

        public UserCertificateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_USERCERTIFICATE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
