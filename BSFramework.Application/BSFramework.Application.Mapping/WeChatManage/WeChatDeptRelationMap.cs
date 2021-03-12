using BSFramework.Application.Entity.WeChatManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.WeChatManage
{
    /// <summary>
    /// 描 述：企业号部门
    /// </summary>
    public class WeChatDeptRelationMap : EntityTypeConfiguration<WeChatDeptRelationEntity>
    {
        public WeChatDeptRelationMap()
        {
            #region 表、主键
            //表
            this.ToTable("WECHAT_DEPTRELATION");
            //主键
            this.HasKey(t => t.DeptId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
