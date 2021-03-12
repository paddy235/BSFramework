using BSFramework.Application.Entity.SevenSManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BSFramework.Application.Mapping.SevenSManage
{
 public   class SevenSOfficeAuditMap : EntityTypeConfiguration<SevenSOfficeAuditEntity>
    {

        public SevenSOfficeAuditMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_SEVENSOFFICEAUDIT");
            //主键
            this.HasKey(t => t.auditid);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
