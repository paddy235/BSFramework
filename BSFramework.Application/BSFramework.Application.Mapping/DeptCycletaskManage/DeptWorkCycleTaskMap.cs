using BSFramework.Application.Entity.DeptCycleTaskManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.DeptCycletaskManage
{
   public class DeptWorkCycleTaskMap : EntityTypeConfiguration<DeptWorkCycleTaskEntity>
    {

        public DeptWorkCycleTaskMap() {

            #region 表、主键
            //表
            this.ToTable("WG_DEPTWORKCYCLETASK");
            //主键
            this.HasKey(t => t.id);
            #endregion
        }
    }
}
