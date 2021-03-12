using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class CultureTemplateMap : EntityTypeConfiguration<CultureTemplateEntity>
    {
        public CultureTemplateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_CULTURETEMPLATE");
            //主键
            this.HasKey(t => t.CultureTemplateId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }
    }
    public class CultureTemplateItemMap : EntityTypeConfiguration<CultureTemplateItemEntity>
    {
        public CultureTemplateItemMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_CULTURETEMPLATEITEM");
            //主键
            this.HasKey(t => t.CultureTemplateItemId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
