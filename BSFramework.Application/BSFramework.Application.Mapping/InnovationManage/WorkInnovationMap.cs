using BSFramework.Application.Entity.InnovationManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSFramework.Application.Mapping.InnovationManage
{
 public   class WorkInnovationMap : EntityTypeConfiguration<WorkInnovationEntity>
    {
        public WorkInnovationMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKINNOVATION");
            //主键
            this.HasKey(t => t.innovationid);
            #endregion
        }
    }
}
