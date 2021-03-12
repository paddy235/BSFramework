using BSFramework.Application.Entity.ReportManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping
{
    /// <summary>
    /// 描 述：授权数据范围
    /// </summary>
    public class RptTempMap : EntityTypeConfiguration<RptTempEntity>
    {
        public RptTempMap()
        {
            #region 表、主键
            //表
            this.ToTable("RPT_TEMP");
            //主键
            this.HasKey(t => t.TempId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
