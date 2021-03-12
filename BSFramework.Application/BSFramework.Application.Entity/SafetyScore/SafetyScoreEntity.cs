
using BSFramework.Application.Code;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.SafetyScore
{
    /// <summary>
    /// 安全卫士_安全积分
    /// </summary>
    [Table("WG_SAFETYSCORE")]
    public class SafetyScoreEntity :  BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 适用积分标准的Id
        /// </summary>
        [Column("RULEID")]
        [Description("适用积分标准的Id")]
        public string RuleId { get; set; }
        
        /// <summary>
        /// 姓名
        /// </summary>
        [Column("USERNAME")]
        [Description("姓名")]
        public string UserName { get; set; }

        
        /// <summary>
        /// 用户Id
        /// </summary>
        [Column("USERID")]
        [Description("用户Id")]
        public string UserId { get; set; }

        /// <summary>
        /// 性别 0女 1男
        /// </summary>
        [Column("GENDER")]
        [Description("性别 0女 1男")]
        public int? Gender { get; set; }

        /// <summary>
        /// 分值
        /// </summary>
        [Column("SCORE")]
        [Description("分值")]
        public decimal Score { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        [Column("AREA")]
        [Description("区域")]
        public string Area { get; set; }

        /// <summary>
        /// 积分原因
        /// </summary>
        [Column("REASONS")]
        [Description("积分原因")]
        public string Reasons { get; set; }

        /// <summary>
        /// 计分类型   自动  手动
        /// </summary>
        [Column("SCORETYPE")]
        [Description("计分类型   自动  手动")]
        public string ScoreType { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [Column("DEPTID")]
        [Description("部门ID")]
        public string DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Column("DEPTNAME")]
        [Description("部门名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        [Column("DEPTCODE")]
        [Description("部门编码")]
        public string DeptCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERID")]
        [Description("")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERNAME")]
        [Description("")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        [Description("")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYUSERID")]
        [Description("")]
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYUSERNAME")]
        [Description("")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYDATE")]
        [Description("")]
        public DateTime ModifyDate { get; set; }


        public   void Create()
        {
            Operator user = OperatorProvider.Provider.Current();
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }

        public  void Modify()
        {
            Operator user = OperatorProvider.Provider.Current();
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }
    }

    public enum ScoreType
    {
        自动,
        手动
    }
}
