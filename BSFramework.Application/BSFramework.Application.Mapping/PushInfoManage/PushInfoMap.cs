using BSFramework.Application.Entity.PushInfoManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.PushInfoManage
{
 public   class PushInfoMap : EntityTypeConfiguration<PushInfoEntity>
    {
        public PushInfoMap() {
            #region 表、主键
            //表
            this.ToTable("WG_PUSHINFO");
            //主键
            this.HasKey(t => t.pushid);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
