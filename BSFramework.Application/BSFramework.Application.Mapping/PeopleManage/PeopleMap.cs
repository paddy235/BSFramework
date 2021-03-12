using BSFramework.Application.Entity.PeopleManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PeopleManage
{
    public class PeopleMap : EntityTypeConfiguration<PeopleEntity>
    {
        public PeopleMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_PEOPLE");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
