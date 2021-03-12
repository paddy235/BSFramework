using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ToolManage;

namespace BSFramework.Application.Mapping.ToolManage
{
    public class ToolInfoMap : EntityTypeConfiguration<ToolInfoEntity>
    {
        public ToolInfoMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_TOOLINFO");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
