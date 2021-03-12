
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 安全日活动-改进行动
    /// </summary>
    [Table("WG_ACTIVITYACTION")]
    public class ActivityActionEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("ID")]
        [Description("")]
        public string Id { get; set; }

        /// <summary>
        /// 安全日活动的主键关联ID
        /// </summary>
        [Column("ACTIVITYID")]
        [Description("安全日活动的主键关联ID")]
        public string ActivityId { get; set; }

        /// <summary>
        /// 行动项
        /// </summary>
        [Column("CONTENT")]
        [Description("行动项")]
        [MaxLength(2048)]
        public string Content { get; set; }

        /// <summary>
        /// 责任人Id  英文逗号隔开
        /// </summary>
        [Column("USERIDS")]
        [Description("责任人Id  英文逗号隔开")]
        [MaxLength(2048)]
        public string UserIds { get; set; }

        /// <summary>
        /// 责任人名称
        /// </summary>
        [Column("USERNAMES")]
        [Description("责任人名称")]
        [MaxLength(2048)]
        public string UserNames { get; set; }

        /// <summary>
        /// 落实情况  已完成  未完成
        /// </summary>
        [Column("STATUS")]
        [Description("落实情况  已完成  进行中")]
        public string Status { get; set; }

        /// <summary>
        /// 完成时间 
        /// </summary>
        [Column("FINISHDATE")]
        [Description("完成时间 ")]
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// 创建人Id

        /// </summary>
        [Column("CREATEUSERID")]
        [Description("创建人Id")]
                    public string CreateUserId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [Column("CREATEUSERNAME")]
        [Description("创建人名称")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CREATEDATE")]
        [Description("创建时间")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改人Id
        /// </summary>
        [Column("MODIFYUSERID")]
        [Description("修改人Id")]
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 修改人名称
        /// </summary>
        [Column("MODIFYUSERNAME")]
        [Description("修改人名称")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("MODIFYDATE")]
        [Description("修改时间")]
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 所属部门编码
        /// </summary>
        [Column("DEPTCODE")]
        [Description("所属部门编码")]
        public string DeptCode { get; set; }

        /// <summary>
        /// 所属部门Id
        /// </summary>
        [Column("DEPTID")]
        [Description("所属部门Id")]
        public string DeptId { get; set; }

        /// <summary>
        /// 所属部门名称
        /// </summary>
        [Column("DEPTNAME")]
        [Description("所属部门名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        [NotMapped]
        public IEnumerable<FileInfoEntity> FileList { get; set; }

        /// <summary>
        /// 是否是上次落实
        /// </summary>
        [NotMapped]
        public bool? IsLast { get; set; }

        #region 方法
        public new void Create()
        {
            Operator user = OperatorProvider.Provider.Current();
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
            DeptCode = user.DeptCode;
            DeptId = user.DeptId;
            DeptName = user.DeptName;
        }

        public new void Modify()
        {
            Operator user = OperatorProvider.Provider.Current();
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }
        #endregion
    }
}
