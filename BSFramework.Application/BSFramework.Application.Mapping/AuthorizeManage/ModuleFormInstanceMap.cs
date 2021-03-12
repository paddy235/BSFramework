using BSFramework.Application.Entity.AuthorizeManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.AuthorizeManage
{
    /// <summary>
    /// 描 述：系统表单
    /// </summary>
    public class ModuleFormInstanceMap : EntityTypeConfiguration<ModuleFormInstanceEntity>
    {
        public ModuleFormInstanceMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_MODULEFORMINSTANCE");
            //主键
            this.HasKey(t => t.FormInstanceId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
