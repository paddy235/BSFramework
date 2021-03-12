using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class SafetydayMaterialEntityMap : EntityTypeConfiguration<SafetydayMaterialEntity>
    {
        public SafetydayMaterialEntityMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_SAFETYDAYMATERIAL");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
