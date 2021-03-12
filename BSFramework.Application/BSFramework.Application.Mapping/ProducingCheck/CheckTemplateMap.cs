using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Entity.ProducingCheck;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.ProducingCheck
{
    public class CheckTemplateMap : EntityTypeConfiguration<CheckTemplateEntity>
    {
        public CheckTemplateMap()
        {

            #region 表、主键
            //表
            this.ToTable("WG_CHECKTEMPLATE");
            //主键
            this.HasKey(t => t.TemplateId);
            #endregion

            #region 配置关系
            #endregion

        }
    }
}
