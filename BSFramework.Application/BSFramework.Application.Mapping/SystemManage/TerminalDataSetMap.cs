using BSFramework.Application.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SystemManage
{
   public class TerminalDataSetMap : EntityTypeConfiguration<TerminalDataSetEntity>
    {
        public TerminalDataSetMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_TERMINALDATASET");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
