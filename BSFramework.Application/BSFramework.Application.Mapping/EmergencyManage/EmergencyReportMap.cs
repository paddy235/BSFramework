using BSFramework.Application.Entity.EmergencyManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.EmergencyManage
{
    public class EmergencyReportMap : EntityTypeConfiguration<EmergencyReportEntity>
    {
        public EmergencyReportMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EMERGENCYREPORT");
            //主键
            this.HasKey(t => t.EmergencyReportId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
