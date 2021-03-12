using BSFramework.Application.Entity.PeopleManage;
using System.Data.Entity.ModelConfiguration;


namespace BSFramework.Application.Mapping.PeopleManage
{
    public class DutyMap : EntityTypeConfiguration<DutyEntity>
    {
        public DutyMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_DUTY");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
