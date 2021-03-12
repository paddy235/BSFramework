
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EdActivityPersonMap : EntityTypeConfiguration<EdActivityPersonEntity>
    {
        public EdActivityPersonMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EDACTIVITYPERSON");
            //主键
            this.HasKey(t => t.ActivityPersonId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
