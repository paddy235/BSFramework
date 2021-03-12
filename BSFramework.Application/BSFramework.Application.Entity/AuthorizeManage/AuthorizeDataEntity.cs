using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 描 述：授权数据范围
    /// </summary>
    [Table("BASE_AUTHORIZEDATA")]
    public class AuthorizeDataEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 授权数据主键
        /// </summary>		
        [Column("AUTHORIZEDATAID")]
        public string AuthorizeDataId { get; set; }
        /// <summary>
        /// 授权类型:1-仅限本人 2-本部门 3-本子部门 4-本机构 5-全部
        /// </summary>		
        [Column("AUTHORIZETYPE")]
        public int? AuthorizeType { get; set; }
        /// <summary>
        /// 对象分类:1-部门2-角色3-岗位4-职位 5-用户 6-用户组
        /// </summary>		
        [Column("CATEGORY")]
        public int Category { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>		
         [Column("OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 功能按钮Id
        /// </summary>		
        [Column("ITEMID")]
        public string ItemId { get; set; }
        /// <summary>
        /// 功能按钮Id
        /// </summary>		
        [Column("ITEMCODE")]
        public string ItemCode { get; set; }
        /// <summary>
        /// 功能按钮名称
        /// </summary>		
        [Column("ITEMNAME")]
        public string ItemName { get; set; }
        /// <summary>
        /// 模块Id
        /// </summary>		
         [Column("RESOURCEID")]
        public string ResourceId { get; set; }
        /// <summary>
        /// 只读
        /// </summary>		
         [Column("ISREAD")]
        public int IsRead { get; set; }
        /// <summary>
        /// 约束表达式
        /// </summary>		
         [Column("AUTHORIZECONSTRAINT")]
        public string AuthorizeConstraint { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>		
         [Column("SORTCODE")]
        public int? SortCode { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.AuthorizeDataId = Guid.NewGuid().ToString();
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
            this.AuthorizeDataId = keyValue;
        }
        #endregion
    }
}