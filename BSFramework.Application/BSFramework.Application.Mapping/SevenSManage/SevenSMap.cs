using BSFramework.Application.Entity.SevenSManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SevenSManage
{
   public  class SevenSMap : EntityTypeConfiguration<SevenSEntity>
    {

        public SevenSMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_SEVENS");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            #endregion
        }

    }
}
