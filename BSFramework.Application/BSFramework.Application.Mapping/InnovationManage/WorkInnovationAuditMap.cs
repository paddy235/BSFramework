using BSFramework.Application.Entity.InnovationManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSFramework.Application.Mapping.InnovationManage
{
  public  class WorkInnovationAuditMap : EntityTypeConfiguration<WorkInnovationAuditEntity>
    {
        public WorkInnovationAuditMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKINNOVATIONAUDIT");
            //主键
            this.HasKey(t => t.auditid);
            #endregion
        }
    }
}
