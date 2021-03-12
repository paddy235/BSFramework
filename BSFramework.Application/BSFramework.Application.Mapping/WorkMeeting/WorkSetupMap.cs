using BSFramework.Application.Entity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.Worksetup
{
    /// <summary>
    /// 班制
    /// </summary>
    public class WorkSetupMap : EntityTypeConfiguration<WorkSetTypeEntity>
    {
        public WorkSetupMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKSETUP");
            //主键
            this.HasKey(t => t.Id);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
