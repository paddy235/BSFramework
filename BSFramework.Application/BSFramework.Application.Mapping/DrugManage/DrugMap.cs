using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.Mapping.DrugManage
{
    public class DrugMap : EntityTypeConfiguration<DrugEntity>
    {
        public DrugMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DRUGMANAGE");
            //主键
            this.HasKey(t => t.Id);
            #endregion
        }
    }
}
