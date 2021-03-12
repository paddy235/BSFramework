using BSFramework.Application.Entity.SetManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.SetManage
{
    public class RiskFactorSetMap : EntityTypeConfiguration<RiskFactorSetEntity>
    {
        public RiskFactorSetMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_RISKFACTORSET");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            this.Ignore(t => t.measures);
            this.Ignore(t => t.measureids);
            #endregion
        }
    }
}
