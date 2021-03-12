using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;
using System;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EvaluateCategoryMap : EntityTypeConfiguration<EvaluateCategoryEntity>
    {
        public EvaluateCategoryMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EVALUATECATEGORY");
            //主键
            this.HasKey(t => t.CategoryId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }

    }
}
