using BSFramework.Application.Entity.CarcOrCardManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.CarcOrCardManage
{
   public class CarcOrCardMap : EntityTypeConfiguration<CCardEntity>
    {
        public CarcOrCardMap()
        {
            #region 表、主键
            //表
            this.ToTable("wg_ccarc");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
