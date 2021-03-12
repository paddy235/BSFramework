using BSFramework.Application.Entity.AuthorizeManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.AuthorizeManage
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：系统功能
    /// </summary>
    public class ModuleMap : EntityTypeConfiguration<ModuleEntity>
    {
        public ModuleMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_MODULE");
            //主键
            this.HasKey(t => t.ModuleId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
