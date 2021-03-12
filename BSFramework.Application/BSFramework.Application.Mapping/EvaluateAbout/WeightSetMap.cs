using BSFramework.Application.Entity.EvaluateAbout;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.EvaluateAbout
{
    /// <summary>
    /// 班组权重设置
    /// </summary>
    public class WeightSetMap : EntityTypeConfiguration<WeightSetEntity>
    {
        public WeightSetMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WEIGHTSET");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
