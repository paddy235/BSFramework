using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.Mapping.DrugManage
{
    public class DrugGlassWareMap : EntityTypeConfiguration<DrugGlassWareEntity>
    {
        public DrugGlassWareMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DRUGGLASSWARE");
            //主键
            this.HasKey(t => t.GlassWareId);
            #endregion
        }
    }
}
