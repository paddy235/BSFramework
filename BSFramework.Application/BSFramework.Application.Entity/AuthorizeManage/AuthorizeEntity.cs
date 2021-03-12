using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 描 述：授权功能
    /// </summary>
    [Table("BASE_AUTHORIZE")]
    public class AuthorizeEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 授权功能主键
        /// </summary>		
        [Column("AUTHORIZEID")]
        public string AuthorizeId { get; set; }
        /// <summary>
        /// 对象分类:1-部门 2-角色 3-岗位 4-职位 5-用户 6-用户组
        /// </summary>		
        [Column("CATEGORY")]
        public int? Category { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>		
        [Column("OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 项目类型:1-菜单2-按钮3-视图4表单
        /// </summary>		
        [Column("ITEMTYPE")]
        public int? ItemType { get; set; }
        /// <summary>
        /// 项目主键
        /// </summary>		
        [Column("ITEMID")]
        public string ItemId { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
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
            this.AuthorizeId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.AuthorizeId = keyValue;
        }
        #endregion
    }
}