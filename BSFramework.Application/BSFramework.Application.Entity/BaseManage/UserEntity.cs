using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.ComponentModel;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class UserEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 用户主键
        /// </summary>		
        [Column("USERID")]
        public string UserId { get; set; }
        /// <summary>
        /// 工号
        /// </summary>	
        [Column("ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 账号
        /// </summary>	
        [Column("ACCOUNT")]
        public string Account { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>	
        [Column("PASSWORD")]
        public string Password { get; set; }
        /// <summary>
        /// 密码秘钥
        /// </summary>		
        [Column("SECRETKEY")]
        public string Secretkey { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>		
        [Column("REALNAME")]
        public string RealName { get; set; }
        /// <summary>
        /// 呢称
        /// </summary>		
        [Column("NICKNAME")]
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>		
        [Column("HEADICON")]
        public string HeadIcon { get; set; }
        /// <summary>
        /// 快速查询
        /// </summary>		
        [Column("QUICKQUERY")]
        public string QuickQuery { get; set; }
        /// <summary>
        ///专业类别
        /// </summary>
        [Column("SPECIALTYTYPE")]
        public string SpecialtyType { get; set; }
        /// <summary>
        /// 简拼
        /// </summary>		
        [Column("SIMPLESPELLING")]
        public string SimpleSpelling { get; set; }
        /// <summary>
        /// 性别
        /// </summary>		
        [Column("GENDER")]
        public int? Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>		
        [Column("BIRTHDAY")]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 手机
        /// </summary>		
        [Column("MOBILE")]
        public string Mobile { get; set; }
        /// <summary>
        /// 座机电话
        /// </summary>		
        [Column("TELEPHONE")]
        public string Telephone { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>		
        [Column("EMAIL")]
        public string Email { get; set; }
        /// <summary>
        /// QQ号
        /// </summary>		
        [Column("OICQ")]
        public string OICQ { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>		
        [Column("WECHAT")]
        public string WeChat { get; set; }
        /// <summary>
        /// MSN
        /// </summary>		
        [Column("MSN")]
        public string MSN { get; set; }
        /// <summary>
        /// 主管主键
        /// </summary>		
        [Column("MANAGERID")]
        public string ManagerId { get; set; }
        /// <summary>
        /// 主管
        /// </summary>	
        [Column("MANAGER")]
        public string Manager { get; set; }
        /// <summary>
        /// 机构主键
        /// </summary>		
        [Column("ORGANIZEID")]
        public string OrganizeId { get; set; }
        /// <summary>
        /// 机构编码
        /// </summary>		
        [Column("ORGANIZECODE")]
        public string OrganizeCode { get; set; }
        /// <summary>
        /// 部门主键
        /// </summary>		
        [Column("DEPARTMENTID")]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 角色主键
        /// </summary>		
        [Column("ROLEID")]
        public string RoleId { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>		
        [Column("DUTYID")]
        public string DutyId { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>		
        [Column("DUTYNAME")]
        public string DutyName { get; set; }
        /// <summary>
        /// 职位主键
        /// </summary>		
        [Column("POSTID")]
        public string PostId { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>		
        [Column("POSTNAME")]
        public string PostName { get; set; }
        /// <summary>
        /// 工作组主键
        /// </summary>		
        [Column("WORKGROUPID")]
        public string WorkGroupId { get; set; }
        /// <summary>
        /// 安全级别
        /// </summary>		
        [Column("SECURITYLEVEL")]
        public int? SecurityLevel { get; set; }
        /// <summary>
        /// 在线状态
        /// </summary>		
        [Column("USERONLINE")]
        [DefaultValue(0)]
        public int? UserOnLine { get; set; }
        /// <summary>
        /// 单点登录标识
        /// </summary>		
        [Column("OPENID")]
        public int? OpenId { get; set; }
        /// <summary>
        /// 密码提示问题
        /// </summary>		
        [Column("QUESTION")]
        public string Question { get; set; }
        /// <summary>
        /// 密码提示答案
        /// </summary>		
        [Column("ANSWERQUESTION")]
        public string AnswerQuestion { get; set; }
        /// <summary>
        /// 允许多用户同时登录
        /// </summary>		
        [Column("CHECKONLINE")]
        public int? CheckOnLine { get; set; }
        /// <summary>
        /// 允许登录时间开始
        /// </summary>		
        [Column("ALLOWSTARTTIME")]
        public DateTime? AllowStartTime { get; set; }
        /// <summary>
        /// 允许登录时间结束
        /// </summary>		
        [Column("ALLOWENDTIME")]
        public DateTime? AllowEndTime { get; set; }
        /// <summary>
        /// 暂停用户开始日期
        /// </summary>		
        [Column("LOCKSTARTDATE")]
        public DateTime? LockStartDate { get; set; }
        /// <summary>
        /// 暂停用户结束日期
        /// </summary>		
        [Column("LOCKENDDATE")]
        public DateTime? LockEndDate { get; set; }
        /// <summary>
        /// 第一次访问时间
        /// </summary>		
        [Column("FIRSTVISIT")]
        public DateTime? FirstVisit { get; set; }
        /// <summary>
        /// 上一次访问时间
        /// </summary>		
        [Column("PREVIOUSVISIT")]
        public DateTime? PreviousVisit { get; set; }
        /// <summary>
        /// 最后访问时间
        /// </summary>		
        [Column("LASTVISIT")]
        public DateTime? LastVisit { get; set; }
        /// <summary>
        /// 登录次数
        /// </summary>		
        [Column("LOGONCOUNT")]
        public int? LogOnCount { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
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
        /// <summary>
        /// 所属部门编码
        /// </summary>		
        [Column("DEPARTMENTCODE")]
        public string DepartmentCode { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }

        [Column("ROLENAME")]
        public string RoleName { get; set; }

        /// <summary>
        /// 指纹特征ID
        /// </summary>
        [Column("FINGER")]
        public string FINGER { get; set; }

        [Column("IDENTIFYID")]
        public string IDENTIFYID { get; set; }

        [Column("EnterTime")]
        public DateTime EnterTime { get; set; }

        /// <summary>
        /// 是否外包 1是 其他不是
        /// </summary>
        [Column("ISEPIBOLY")]
        public string IsEpiboly { get; set; }

        /// <summary>
        /// 是否特种人员
        /// </summary>
        [NotMapped]
        public string IsSpecial { get; set; }
        /// <summary>
        /// 是否特种设备操作人员
        /// </summary>
        [NotMapped]
        public string IsSpecialEqu { get; set; }
        /// <summary>
        /// 职务编号sk
        /// </summary>
        [NotMapped]
        public string PostCode { get; set; }
        /// <summary>
        /// 工号bz
        /// </summary>
        [NotMapped]
        public string LabourNo { get; set; }
        /// <summary>
        /// 民族bz
        /// </summary>
        [NotMapped]
        public string Folk { get; set; }
        /// <summary>
        /// 工种工龄bz
        /// </summary>
        [NotMapped]
        public string CurrentWorkAge { get; set; }
        /// <summary>
        /// 原始学历bz
        /// </summary>
        [NotMapped]
        public string OldDegree { get; set; }
        /// <summary>
        /// 后期学历bz
        /// </summary>
        [NotMapped]
        public string NewDegree { get; set; }


        /// <summary>
        /// 职务bz
        /// </summary>
        [NotMapped]
        public string Quarters { get; set; }
        /// <summary>
        /// 职务编号bz
        /// </summary>
        [NotMapped]
        public string Planer { get; set; }
        /// <summary>
        /// 政治面貌bz
        /// </summary>
        [NotMapped]
        public string Visage { get; set; }
        /// <summary>
        /// 年龄 ==
        /// </summary>
        [NotMapped]
        public string Age { get; set; }
        /// <summary>
        /// 籍贯 ==
        /// </summary>
        [NotMapped]
        public string Native { get; set; }
        /// <summary>
        /// 职称bz
        /// </summary>
        [NotMapped]
        public string JobName { get; set; }
        /// <summary>
        /// 原始学历sk
        /// </summary>
        [NotMapped]
        public string Degrees { get; set; }
        /// <summary>
        /// 后期学历sk
        /// </summary>
        [NotMapped]
        public string LateDegrees { get; set; }
        /// <summary>
        /// 技术等级sk
        /// </summary>
        [NotMapped]
        public string TechnicalGrade { get; set; }
        /// <summary>
        /// 职称sk
        /// </summary>
        [NotMapped]
        public string JobTitle { get; set; }
        /// <summary>
        /// 工种sk
        /// </summary>
        [NotMapped]
        public string Craft { get; set; }
        /// <summary>
        /// 工种工龄sk
        /// </summary>
        [NotMapped]
        public string CraftAge { get; set; }
        /// <summary>
        /// 健康状况 ==
        /// </summary>
        [NotMapped]
        public string HealthStatus { get; set; }
        /// <summary>
        /// 民族sk
        /// </summary>
        [NotMapped]
        public string Nation { get; set; }
        /// <summary>
        /// 政治面貌sk
        /// </summary>
        [NotMapped]
        public string Political { get; set; }
        /// <summary>
        /// 头像bz
        /// </summary>
        [NotMapped]
        public string Photo { get; set; }

        /// <summary>
        /// 签名bz
        /// </summary>
        [NotMapped]
        public string Signature { get; set; }

        /// <summary>
        /// 签名bz
        /// </summary>
        [NotMapped]
        public string SignImg { get; set; }

        /// <summary>
        /// 人员类型
        /// </summary>
        [NotMapped]
        public string UserType { get; set; }
        /// <summary>
        /// 是否三种人
        /// </summary>
        [NotMapped]
        public string IsFourPerson { get; set; }
        /// <summary>
        /// 三种人类型
        /// </summary>
        [NotMapped]
        public string FourPersonType { get; set; }
        [NotMapped]
        public string WorkKind { get { return Craft; } set { Craft = value; } }
        [NotMapped]
        public string TecLevel { get { return TechnicalGrade; } set { TechnicalGrade = value; } }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.UserId = string.IsNullOrEmpty(UserId) ? Guid.NewGuid().ToString() : UserId;
            this.CreateDate = DateTime.Now;
            //this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            //this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
            this.EnabledMark = 1;

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.UserId = keyValue;
            this.ModifyDate = DateTime.Now;
            //this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            //this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }

}