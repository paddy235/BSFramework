using BSFramework.Application.Entity.PushInfoManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.PushInfoManage
{
    public class PushPersonMap : EntityTypeConfiguration<PushPersonEntity>
    {
        public PushPersonMap() {
            #region 表、主键
            //表
            this.ToTable("WG_PUSHPERSON");
            //主键
            this.HasKey(t => t.id);
            #endregion

            #region 配置关系
            #endregion
        }

    }
}
