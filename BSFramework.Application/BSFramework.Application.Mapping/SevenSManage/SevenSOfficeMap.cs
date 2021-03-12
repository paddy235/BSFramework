using BSFramework.Application.Entity.SevenSManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SevenSManage
{
 public   class SevenSOfficeMap : EntityTypeConfiguration<SevenSOfficeEntity>
    {

        public SevenSOfficeMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_SEVENSOFFICE");
            //主键
            this.HasKey(t => t.id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
