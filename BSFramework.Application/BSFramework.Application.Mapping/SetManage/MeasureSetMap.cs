using BSFramework.Application.Entity.SetManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SetManage
{
    public class MeasureSetMap : EntityTypeConfiguration<MeasureSetEntity>
    {
        public MeasureSetMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_MEASURESET");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
