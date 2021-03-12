using BSFramework.Application.Entity.Activity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.Activity
{
    /// <summary>
    /// 描 述：班组台
    /// </summary>
    public class OrderinfoMap : EntityTypeConfiguration<OrderinfoEntity>
    {
        public OrderinfoMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ORDERINFO");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
