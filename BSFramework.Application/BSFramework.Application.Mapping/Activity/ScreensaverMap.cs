using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.Activity
{
   public class ScreensaverMap : EntityTypeConfiguration<ScreensaverEntity>
    {
        public ScreensaverMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_SCREENSAVER");
            //主键
            this.HasKey(t => t.FileId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
