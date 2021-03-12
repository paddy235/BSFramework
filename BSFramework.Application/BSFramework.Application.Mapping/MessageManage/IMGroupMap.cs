using BSFramework.Application.Entity.MessageManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.MessageManage
{
    /// <summary>
    /// 描 述：即时通信用户群组表
    /// </summary>
    public class IMGroupMap: EntityTypeConfiguration<IMGroupEntity>
    {
        public IMGroupMap()
        {
            #region 表、主键
            //表
            this.ToTable("IM_GROUP");
            //主键
            this.HasKey(t => t.GroupId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
