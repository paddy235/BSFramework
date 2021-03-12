using BSFramework.Application.Entity.BaseManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.BaseManage
{
 public   class AndroidmenuMap : EntityTypeConfiguration<AndroidmenuEntity>
    {

        public AndroidmenuMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ANDROIDMENU");
            //主键
            this.HasKey(t => t.MenuId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
