using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.WorkMeeting
{
    /// <summary>
    /// 早安中铝_班会记录
    /// </summary>
    [Table("WG_MEETINGRECORD")]
    public class MeetingRecordEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        [Description("")]
        public string Id { get; set; }

        /// <summary>
        /// 班会的Id
        /// </summary>
        [Column("MEETINGID")]
        [Description("班会的Id")]
        public string MeetingId { get; set; }

        
        /// <summary>
        /// 学习音频名称
        /// </summary>
        [Column("TITLE")]
        [Description("学习音频名称")]
        public string Title { get; set; }

        /// <summary>
        /// 分享人的UserId，英文逗号隔开
        /// </summary>
        [Column("SHAREUSERIDS")]
        [Description("分享人的UserId，英文逗号隔开")]
        public string ShareUserIds { get; set; }

        /// <summary>
        /// 分享人的名称 英文逗号隔开
        /// </summary>
        [Column("SHAREUSERNAMES")]
        [Description("分享人的名称 英文逗号隔开")]
        public string ShareUserNames { get; set; }

        /// <summary>
        /// 体会启示
        /// </summary>
        [Column("EXPERIENCE")]
        [Description("体会启示")]
        public string Experience { get; set; }

        /// <summary>
        /// 点评
        /// </summary>
        [Column("CRITIQUE")]
        [Description("点评")]
        public string Critique { get; set; }

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
        [Column("CREATEUSERDEPTID")]
        [Description("")]
        public string CreateUserDeptId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERDEPTNAME")]
        [Description("")]
        public string CreateUserDeptName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERDEPTCODE")]
        [Description("")]
        public string CreateUserDeptCode { get; set; }

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


        public override void Create()
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

        public  void Create(DepartmentEntity createUserDept)
        {
            Operator user = OperatorProvider.Provider.Current();
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
            if (createUserDept != null)
            {
                CreateUserDeptId = createUserDept.DepartmentId;
                CreateUserDeptName = createUserDept.FullName;
                CreateUserDeptCode = createUserDept.EnCode;
            }
        }
        public  void Modify()
        {
            Operator user = OperatorProvider.Provider.Current();
            CreateDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }
    }
}
