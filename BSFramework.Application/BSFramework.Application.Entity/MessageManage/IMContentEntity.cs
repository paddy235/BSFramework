using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.MessageManage
{
	/// <summary>
    /// 即时消息表
    /// </summary>
     [Table("IM_CONTENT")]
    public class IMContentEntity : BaseEntity
	{
		#region 获取/设置 字段值
      	/// <summary>
		/// 消息主键
        /// </summary>		
         [Column("CONTENTID")]
         public string ContentId { get; set; }    
		/// <summary>
		/// 是否是群组消息
        /// </summary>		
         [Column("ISGROUP")]
         public int IsGroup { get; set; }    
		/// <summary>
		/// 发送者ID
        /// </summary>		
         [Column("SENDID")]
         public string SendId { get; set; }    
		/// <summary>
		/// 接收者ID
        /// </summary>		
         [Column("TOID")]
         public string ToId { get; set; }    
		/// <summary>
		/// 消息内容
        /// </summary>		
         [Column("MSGCONTENT")]
         public string MsgContent { get; set; }    
		/// <summary>
		/// 创建时间
        /// </summary>		
         [Column("CREATEDATE")]
         public DateTime CreateDate { get; set; }    
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
         /// 接收用户姓名
         /// </summary>		
         [Column("RECEIVERNAME")]
         public string ReceiverName { get; set; }    
		   		#endregion
   		
   		#region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ContentId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ContentId = keyValue;
        }
        #endregion
	}
}