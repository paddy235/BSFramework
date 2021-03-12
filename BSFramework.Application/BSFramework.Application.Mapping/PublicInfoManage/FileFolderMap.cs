using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件夹
    /// </summary>
    public class FileFolderMap : EntityTypeConfiguration<FileFolderEntity>
    {
        public FileFolderMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_FILEFOLDER");
            //主键
            this.HasKey(t => t.FolderId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
