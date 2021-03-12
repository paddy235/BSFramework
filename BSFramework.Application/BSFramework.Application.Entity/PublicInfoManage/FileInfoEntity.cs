using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件信息
    /// </summary>
    public class FileInfoEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 文件主键
        /// </summary>		
        [Column("FILEID")]
        public string FileId { get; set; }
        /// <summary>
        /// 文件夹主键
        /// </summary>		
        [Column("FOLDERID")]
        public string FolderId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>		
        [Column("FILENAME")]
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>	
        [Column("FILEPATH")]
        public string FilePath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>	
        [Column("FILESIZE")]
        public string FileSize { get; set; }
        /// <summary>
        /// 文件后缀
        /// </summary>		
        [Column("FILEEXTENSIONS")]
        public string FileExtensions { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>	
        [Column("FILETYPE")]
        public string FileType { get; set; }
        /// <summary>
        /// 共享
        /// </summary>	
        [Column("ISSHARE")]
        public int? IsShare { get; set; }
        /// <summary>
        /// 共享连接
        /// </summary>	
        [Column("SHARELINK")]
        public string ShareLink { get; set; }
        /// <summary>
        /// 共享提取码
        /// </summary>	
        [Column("SHARECODE")]
        public int? ShareCode { get; set; }
        /// <summary>
        /// 共享日期
        /// </summary>	
        [Column("SHARETIME")]
        public DateTime? ShareTime { get; set; }
        /// <summary>
        /// 下载次数
        /// </summary>	
        [Column("DOWNLOADCOUNT")]
        public int? DownloadCount { get; set; }
        /// <summary>
        /// 置顶
        /// </summary>		
        [Column("ISTOP")]
        public int? IsTop { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>	
        [Column("SORTCODE")]
        public decimal? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>		
        [Column("DELETEMARK")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>	
        [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>	
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>		
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>		
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>		
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 关联记录ID
        /// </summary>		
        [Column("RECID")]
        public string RecId { get; set; }
        [NotMapped]
        public int? State { get; set; }
        [NotMapped]
        public string duration { get; set; }
        [Column("OTHERURL")]
        public string OtherUrl { get; set; }



        /// <summary>
        /// 是否可预览
        /// </summary>
        [NotMapped]
        public bool? CanView { get; set; } = false;

        /// <summary>
        /// 预览URL
        /// </summary>
        [NotMapped]
        public string ViewUrl { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.FileId))
                this.FileId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
            this.EnabledMark = 1;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.FileId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(this.FileId)) return base.GetHashCode();
            return this.FileId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}