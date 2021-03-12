
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EdActivityRecordMap : EntityTypeConfiguration<EdActivityRecordEntity>
    {
        public EdActivityRecordMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EDACTIVITYRECORD");
            //主键
            this.HasKey(t => t.ActivityRecordId);
            #endregion

            #region 配置关系
            //this.HasRequired(t => t.Activity).WithMany(t => t.ActivityRecords).HasForeignKey(t => t.ActivityId).WillCascadeOnDelete(true);
            #endregion
        }
    }
}
