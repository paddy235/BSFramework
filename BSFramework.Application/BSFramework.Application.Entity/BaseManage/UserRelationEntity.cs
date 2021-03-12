using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 描 述：用户关系
    /// </summary>
    [Table("BASE_USERRELATION")]
    public class UserRelationEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 用户关系主键
        /// </summary>		
        [Column("USERRELATIONID")]
        public string UserRelationId { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>		
        [Column("USERID")]
        public string UserId { get; set; }
        /// <summary>
        /// 分类:1-部门2-角色3-岗位4-职位5-工作组
        /// </summary>		
        [Column("CATEGORY")]
        public int? Category { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>		
        [Column("OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 默认对象
        /// </summary>
        [Column("ISDEFAULT")]
        public int? IsDefault { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int SortCode { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.UserRelationId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.IsDefault = 0;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {

        }
        #endregion
    }
}