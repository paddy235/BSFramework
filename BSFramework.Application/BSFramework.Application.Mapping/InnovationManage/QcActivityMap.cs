using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.InnovationManage;

namespace BSFramework.Application.Mapping.InnovationManage
{
    public class QcActivityMap : EntityTypeConfiguration<QcActivityEntity>
    {
        public QcActivityMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_QCACTIVITY");
            //主键
            this.HasKey(t => t.qcid);
            #endregion
        }
    }
}
