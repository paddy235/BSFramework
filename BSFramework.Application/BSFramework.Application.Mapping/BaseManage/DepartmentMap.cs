using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentMap : EntityTypeConfiguration<DepartmentEntity>
    {
        public DepartmentMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_DEPARTMENT");
            //主键
            this.HasKey(t => t.DepartmentId);
            #endregion

            #region 配置关系
            this.Ignore(x => x.NumberOfPeople);
            #endregion
        }
    }
}
