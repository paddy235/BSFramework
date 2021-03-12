using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PublicInfoManage
{
    /// <summary>
    /// 描 述：邮件分类
    /// </summary>
    public class EmailCategoryMap : EntityTypeConfiguration<EmailCategoryEntity>
    {
        public EmailCategoryMap()
        {
            #region 表、主键
            //表
            this.ToTable("EMAIL_CATEGORY");
            //主键
            this.HasKey(t => t.CategoryId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
