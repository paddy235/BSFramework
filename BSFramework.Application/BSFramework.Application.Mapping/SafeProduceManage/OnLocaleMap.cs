using BSFramework.Application.Entity.SafeProduceManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SafeProduceManage
{
    public class OnLocaleMap : EntityTypeConfiguration<OnLocaleEntity>
    {
        public OnLocaleMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ONLOCALE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
