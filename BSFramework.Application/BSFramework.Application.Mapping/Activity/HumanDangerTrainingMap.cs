using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 风险预控
    /// </summary>
    public class HumanDangerTrainingBaseMap : EntityTypeConfiguration<HumanDangerTrainingBaseEntity>
    {
        public HumanDangerTrainingBaseMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_HUMANDANGERTRAINING");
            //主键
            this.HasKey(t => t.TrainingId);
            #endregion

            #region 配置关系
            this.HasMany(x => x.TrainingUsers).WithRequired(x => x.Training).HasForeignKey(x => x.TrainingId).WillCascadeOnDelete(true);
            #endregion
        }
    }
    public class HumanDangerTrainingUserMap : EntityTypeConfiguration<HumanDangerTrainingUserEntity>
    {
        public HumanDangerTrainingUserMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_TRAININGUSER");
            //主键
            this.HasKey(t => t.TrainingUserId);
            #endregion

            #region 配置关系
            this.HasRequired(x => x.Training).WithMany(x => x.TrainingUsers).HasForeignKey(x => x.TrainingId);
            this.HasMany(x => x.TrainingMeasures).WithRequired(x => x.Training).HasForeignKey(x => x.TrainingUserId).WillCascadeOnDelete(true);
            this.HasMany(x => x.TrainingTypes).WithRequired(x => x.Training).HasForeignKey(x => x.TrainingUserId).WillCascadeOnDelete(true);
            #endregion
        }
    }

    public class HumanDangerTrainingMeasureMap : EntityTypeConfiguration<HumanDangerTrainingMeasureEntity>
    {
        public HumanDangerTrainingMeasureMap()
        {
            this.ToTable("WG_TRAININGMEASURE");
            this.HasKey(t => t.TrainingMeasureId);

            this.HasRequired(x => x.Training).WithMany(x => x.TrainingMeasures).HasForeignKey(x => x.TrainingUserId);
        }
    }

    public class HumanDangerTrainingTypeMap : EntityTypeConfiguration<HumanDangerTrainingTypeEntity>
    {
        public HumanDangerTrainingTypeMap()
        {
            this.ToTable("WG_TRAININGTYPE");
            this.HasKey(t => t.TaskTypeId);

            this.HasRequired(x => x.Training).WithMany(x => x.TrainingTypes).HasForeignKey(x => x.TrainingUserId);
        }
    }
}
