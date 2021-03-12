using BSFramework.Application.Entity.KeyboxManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.KeyBoxManage
{
    /// <summary>
    /// 
    /// </summary>
  public  class KeyBoxMap : EntityTypeConfiguration<KeyBoxEntity>
    {
        public KeyBoxMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_KEYBOX");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
