using BSFramework.Application.Entity.AuthorizeManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.AuthorizeManage
{
    /// <summary>
    /// 描 述：过滤IP
    /// </summary>
    public class FilterTimeMap : EntityTypeConfiguration<FilterTimeEntity>
    {
        public FilterTimeMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_FILTERTIME");
            //主键
            this.HasKey(t => t.FilterTimeId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
