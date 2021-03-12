using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.WorkMeeting
{
    /// <summary>
    /// 班前一题
    /// </summary>
    [Table("WG_MEETINGQUESTION")]
    public class MeetingQuestionEntity :BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 班会的Id
        /// </summary>
        [Column("MEETINGID")]
        [Description("班会的Id")]
        public string MeetingId { get; set; }

        /// <summary>
        /// 试题的内容
        /// </summary>
        [Column("QUESTION")]
        [Description("试题的内容")]
        public string Question { get; set; }

        /// <summary>
        /// 参考答案
        /// </summary>
        [Column("ANSWER")]
        [Description("参考答案")]
        public string Answer { get; set; }

        /// <summary>
        /// 答题人的Id
        /// </summary>
        [Column("ANSWERUSERID")]
        [Description("答题人的Id")]
        public string AnswerUserId { get; set; }

        /// <summary>
        /// 答题人的名称
        /// </summary>
        [Column("ANSWERUSERNAME")]
        [Description("答题人的名称")]
        public string AnswerUserName { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [Column("SCORE")]
        [Description("评分")]
        public double? Score { get; set; }

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

        public void Create(DepartmentEntity createUserDept)
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
        public void Modify()
        {
            Operator user = OperatorProvider.Provider.Current();
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }

    }
}
