using BSFramework.Application.Entity.PeopleManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.PeopleManage
{
    public class PeopleDutyTypeMap: EntityTypeConfiguration<PeopleDutyTypeEntity>
    {
        public PeopleDutyTypeMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_PEOPLEDUTYTYPE");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
