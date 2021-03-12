using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class EvaluateMap : EntityTypeConfiguration<EvaluateEntity>
    {
        public EvaluateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_EVALUATE");
            //主键
            this.HasKey(t => t.EvaluateId);
            #endregion

            #region 配置关系
            this.Ignore(x => x.CanScore);
            this.Ignore(x => x.CanEdit);
            #endregion
        }
    }
}
