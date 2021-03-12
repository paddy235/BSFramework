using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.MessageManage
{
	/// <summary>
    /// 用户群组表
    /// </summary>
    [Table("IM_USERGROUP")]
    public class IMUserGroupEntity : BaseEntity
	{
		#region 获取/设置 字段值
      	/// <summary>
		/// 用户群组主键
        /// </summary>		
        [Column("USERGROUPID")]
        public string UserGroupId { get; set; }    
		/// <summary>
		/// 群组主键
        /// </summary>		
        [Column("GROUPID")]
        public string GroupId { get; set; }    
		/// <summary>
		/// 用户主键
        /// </summary>		
        [Column("USERID")]
        public string UserId { get; set; }    
		/// <summary>
		/// 创建时间
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
		   		#endregion
   		
   		#region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.UserGroupId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.GroupId = keyValue;
        }
        #endregion
	}
}