using BSFramework.Application.Entity.MessageManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.MessageManage
{
    /// <summary>
    /// 描 述：即时通信未读消息表
    /// </summary>
    public class IMReadMap : EntityTypeConfiguration<IMReadEntity>
    {
        public IMReadMap()
        {
            #region 表、主键
            //表
            this.ToTable("IM_READ");
            //主键
            this.HasKey(t => t.ReadId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
