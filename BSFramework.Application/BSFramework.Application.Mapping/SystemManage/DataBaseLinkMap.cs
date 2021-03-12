using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// 描 述：区域管理
    /// </summary>
    public class DataBaseLinkMap : EntityTypeConfiguration<DataBaseLinkEntity>
    {
        public DataBaseLinkMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_DATABASELINK");
            //主键
            this.HasKey(t => t.DatabaseLinkId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
