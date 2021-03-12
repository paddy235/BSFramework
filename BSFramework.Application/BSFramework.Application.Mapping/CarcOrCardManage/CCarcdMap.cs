using BSFramework.Application.Entity.CarcOrCardManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.CarcOrCardManage
{
   public class CCarcdMap : EntityTypeConfiguration<CCardEntity>
    {
        public CCarcdMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_CCARD");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
