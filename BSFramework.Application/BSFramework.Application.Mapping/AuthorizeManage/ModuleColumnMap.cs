using BSFramework.Application.Entity.AuthorizeManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.AuthorizeManage
{
    /// <summary>
    /// 描 述：系统视图
    /// </summary>
    public class ModuleColumnMap : EntityTypeConfiguration<ModuleColumnEntity>
    {
        public ModuleColumnMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_MODULECOLUMN");
            //主键
            this.HasKey(t => t.ModuleColumnId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
