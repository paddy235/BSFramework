using BSFramework.Application.Entity.DeviceInspection;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.DeviceInspection
{
   public class DeviceInspectionItemJobMap : EntityTypeConfiguration<DeviceInspectionItemJobEntity>
    {
        public DeviceInspectionItemJobMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DEVICEINSPECTIONITEMJOB");
            //主键
            this.HasKey(t => t.Id);
            #endregion
        }
    }
}
