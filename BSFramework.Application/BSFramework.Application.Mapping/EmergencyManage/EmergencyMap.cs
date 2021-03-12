using BSFramework.Application.Entity.EmergencyManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.EmergencyManage
{
    public class EmergencyMap : EntityTypeConfiguration<EmergencyEntity>
    {
        public EmergencyMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_EMERGENCY");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
