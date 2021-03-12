using BSFramework.Application.Entity.MessageManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.MessageManage
{
    /// <summary>
    /// 描 述：即时通信即时消息表
    /// </summary>
    public class IMContentMap : EntityTypeConfiguration<IMContentEntity>
    {
        public IMContentMap()
        {
            #region 表、主键
            //表
            this.ToTable("IM_CONTENT");
            //主键
            this.HasKey(t => t.ContentId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
