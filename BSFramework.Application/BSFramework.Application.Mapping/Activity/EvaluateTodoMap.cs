using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;
namespace BSFramework.Application.Mapping.Activity
{
    public class EvaluateTodoMap : EntityTypeConfiguration<EvaluateTodoEntity>
    {
        public EvaluateTodoMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EVALUATETODO");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }

    }
}
