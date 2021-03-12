using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：邮件收件人
    /// </summary>
    public class EmailAddresseeEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 邮箱收件人主键
        /// </summary>	
        [Column("ADDRESSEEID")]
        public string AddresseeId { get; set; }
        /// <summary>
        /// 邮件信息主键
        /// </summary>	
        [Column("CONTENTID")]
        public string ContentId { get; set; }
        /// <summary>
        /// 邮件分类主键
        /// </summary>		
         [Column("CATEGORYID")]
        public string CategoryId { get; set; }
        /// <summary>
        /// 收件人Id
        /// </summary>	
         [Column("RECIPIENTID")]
        public string RecipientId { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>	
         [Column("RECIPIENTNAME")]
        public string RecipientName { get; set; }
        /// <summary>
        /// 收件状态（0-收件1-抄送2-密送）
        /// </summary>	
         [Column("RECIPIENTSTATE")]
        public int? RecipientState { get; set; }
        /// <summary>
        /// 是否阅读
        /// </summary>	
       [Column("ISREAD")]
        public int? IsRead { get; set; }
        /// <summary>
        /// 阅读次数
        /// </summary>	
        [Column("READCOUNT")]
        public int? ReadCount { get; set; }
        /// <summary>
        /// 最后阅读日期
        /// </summary>	
        [Column("READDATE")]
        public DateTime? ReadDate { get; set; }
        /// <summary>
        /// 设置红旗
        /// </summary>	
         [Column("ISHIGHLIGHT")]
        public int? IsHighlight { get; set; }
        /// <summary>
        /// 设置待办
        /// </summary>	
         [Column("BACKLOG")]
        public int? Backlog { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>		
         [Column("DELETEMARK")]
        public int? DeleteMark { get; set; }
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
            this.AddresseeId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
            this.ReadCount = 0;
            this.IsRead = 0;
        }
        #endregion
    }
}