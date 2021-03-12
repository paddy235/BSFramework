using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：邮件内容
    /// </summary>
    public class EmailContentEntity : BaseEntity
    {
        #region 实体成员
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
        /// 父级主键
        /// </summary>	
        [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 邮件主题
        /// </summary>	
        [Column("THEME")]
        public string Theme { get; set; }
        /// <summary>
        /// 邮件主题色彩
        /// </summary>		
        [Column("THEMECOLOR")]
        public string ThemeColor { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>	
        [Column("EMAILCONTENT")]
        public string EmailContent { get; set; }
        /// <summary>
        /// 邮件类型（1-内部2-外部）
        /// </summary>		
        [Column("EMAILTYPE")]
        public int? EmailType { get; set; }
        /// <summary>
        /// 发件人Id
        /// </summary>	
        [Column("SENDERID")]
        public string SenderId { get; set; }
        /// <summary>
        /// 发件人
        /// </summary>	
        [Column("SENDERNAME")]
        public string SenderName { get; set; }
        /// <summary>
        /// 发件日期
        /// </summary>		
         [Column("SENDERTIME")]
        public DateTime? SenderTime { get; set; }
        /// <summary>
        /// 设置红旗
        /// </summary>	
          [Column("ISHIGHLIGHT")]
        public int? IsHighlight { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>		
         [Column("SENDPRIORITY")]
        public string SendPriority { get; set; }
        /// <summary>
        /// 短信提醒
        /// </summary>	
         [Column("ISSMSREMINDER")]
        public int? IsSmsReminder { get; set; }
        /// <summary>
        /// 已读回执
        /// </summary>	
         [Column("ISRECEIPT")]
        public int? IsReceipt { get; set; }
        /// <summary>
        /// 发送状态（1-已发送0-草稿）
        /// </summary>	
         [Column("SENDSTATE")]
        public int? SendState { get; set; }
        /// <summary>
        /// 收件人html
        /// </summary>	
          [Column("ADDRESSSHTML")]
        public string AddresssHtml { get; set; }
        /// <summary>
        /// 抄送人html
        /// </summary>	
         [Column("COPYSENDHTML")]
        public string CopysendHtml { get; set; }
        /// <summary>
        /// 密送人html
        /// </summary>	
        [Column("BCCSENDHTML")]
        public string BccsendHtml { get; set; }
        /// <summary>
        /// 删除标记（0-正常1-删除2-彻底删除）
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

            this.ContentId = string.IsNullOrEmpty(this.ContentId) ? Guid.NewGuid().ToString() : this.ContentId;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.SenderTime = DateTime.Now;
            this.SenderId = OperatorProvider.Provider.Current().UserId;
            this.SenderName = OperatorProvider.Provider.Current().UserName + "（" + OperatorProvider.Provider.Current().Account + "）";
            this.DeleteMark = 0;
            this.EnabledMark = 1;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ContentId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}