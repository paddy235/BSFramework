using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// 描 述：数据字典明细
    /// </summary>
    public class DataItemDetailMap : EntityTypeConfiguration<DataItemDetailEntity>
    {
        public DataItemDetailMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_DATAITEMDETAIL");
            //主键
            this.HasKey(t => t.ItemDetailId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
