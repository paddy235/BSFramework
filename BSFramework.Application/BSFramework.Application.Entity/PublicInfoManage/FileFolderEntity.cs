using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件夹
    /// </summary>
    public class FileFolderEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 文件夹主键
        /// </summary>		
         [Column("FOLDERID")]
        public string FolderId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
         [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 文件夹类型
        /// </summary>		
         [Column("FOLDERTYPE")]
        public string FolderType { get; set; }
        /// <summary>
        /// 文件夹名称
        /// </summary>	
        [Column("FOLDERNAME")]
        public string FolderName { get; set; }
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
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.FolderId = Guid.NewGuid().ToString();
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
            this.FolderId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}