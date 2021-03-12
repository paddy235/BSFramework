using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：表单管理
    /// </summary>
    public class WFFrmMainMap : EntityTypeConfiguration<WFFrmMainEntity>
    {
        public WFFrmMainMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_FRMMAIN");
            //主键
            this.HasKey(t => t.FrmMainId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
