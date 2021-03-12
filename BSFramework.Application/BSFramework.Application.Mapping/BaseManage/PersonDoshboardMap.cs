using BSFramework.Application.Entity;
using BSFramework.Application.Entity.BaseManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class PersonDoshboardMap : EntityTypeConfiguration<PersonDoshboardEntity>
    {
        public PersonDoshboardMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_PERSONDOSHBOARD");
            //主键
            this.HasKey(t => t.PersonDoshboardId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
