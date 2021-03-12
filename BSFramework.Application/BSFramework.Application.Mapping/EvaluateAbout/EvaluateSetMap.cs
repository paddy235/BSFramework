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
    /// 班组分类管理映射
    /// </summary>
    public class EvaluateSetMap : EntityTypeConfiguration<EvaluateSetEntity>
    {
        public EvaluateSetMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EVALUATESET");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
