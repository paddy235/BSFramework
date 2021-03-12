using BSFramework.Application.Entity.SafetyScore;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SafetyScore
{
   public  class SafetyScoreMap : EntityTypeConfiguration<SafetyScoreEntity>
    {
        public SafetyScoreMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_SAFETYSCORE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
