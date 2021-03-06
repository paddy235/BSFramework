using BSFramework.Application.Entity.DeviceInspection;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.DeviceInspection
{
    public class InspectionRecordMap : EntityTypeConfiguration<InspectionRecordEntity>
    {
        public InspectionRecordMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DEVICEINSPECTIONRECORD");
            //主键
            this.HasKey(t => t.Id);
            #endregion
        }
    }
}