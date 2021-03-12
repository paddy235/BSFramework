using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件
    /// </summary>
    public class FileInfoMap : EntityTypeConfiguration<FileInfoEntity>
    {
        public FileInfoMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_FILEINFO");
            //主键
            this.HasKey(t => t.FileId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
