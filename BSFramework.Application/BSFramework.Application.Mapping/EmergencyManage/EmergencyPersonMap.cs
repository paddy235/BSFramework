using BSFramework.Application.Entity.EmergencyManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSFramework.Application.Mapping.EmergencyManage
{
 public   class EmergencyPersonMap : EntityTypeConfiguration<EmergencyPersonEntity>
    {
        public EmergencyPersonMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EMERGENCYPERSON");
            //主键
            this.HasKey(t => t.EmergencyPersonId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
