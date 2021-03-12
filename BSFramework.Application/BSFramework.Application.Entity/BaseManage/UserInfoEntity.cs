using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.ComponentModel;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 描 述：用户基本信息(视图)
    /// </summary>
    [Table("V_USERINFO")]
    public class UserInfoEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 用户主键
        /// </summary>	
        [Key]
        [Column("USERID")]
        public string UserId { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>	
        [Column("ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 登录账户
        /// </summary>	
        [Column("ACCOUNT")]
        public string Account { get; set; }
        /// <summary>
        /// 登陆密码
        /// </summary>
        [Column("PASSWORD")]
        public string Password { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        [Column("SECRETKEY")]
        public string Secretkey { get; set; }
        /// <summary>
        /// 真实姓名
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
        /// 电话
        /// </summary>		
        [Column("TELEPHONE")]
        public string Telephone { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>		
        [Column("EMAIL")]
        public string Email { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>		
        [Column("ROLENAME")]
        public string RoleName { get; set; }
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
        /// 主管
        /// </summary>	
        [Column("MANAGER")]
        public string Manager { get; set; }
        /// <summary>
        ///
        /// </summary>		
        [Column("DUTYNAME")]
        public string DutyName { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>		
        [Column("POSTNAME")]
        public string PostName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>		
        [Column("ORGANIZENAME")]
        public string OrganizeName { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>		
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 所属机构ID
        /// </summary>		
        [Column("ORGANIZEID")]
        public string OrganizeId { get; set; }
        /// <summary>
        /// 所属机构Code
        /// </summary>		
        [Column("ORGANIZECODE")]
        public string OrganizeCode { get; set; }
        /// <summary>
        ///职务ID
        /// </summary>		
        [Column("DUTYID")]
        public string DutyId { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>		
        [Column("ROLEID")]
        public string RoleId { get; set; }
        /// <summary>
        /// 岗位ID
        /// </summary>		
        [Column("POSTID")]
        public string PostId { get; set; }
        /// <summary>
        /// 主管ID
        /// </summary>		
        [Column("MANAGERID")]
        public string ManagerId { get; set; }
        /// <summary>
        /// 所属部门ID
        /// </summary>		
        [Column("DEPARTMENTID")]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 所属部门编码
        /// </summary>		
        [Column("DEPARTMENTCODE")]
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>		
        [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }

        /// <summary>
        /// 发包部门
        /// </summary>		
        [Column("SENDDEPTID")]
        public string SendDeptID { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Column("IDENTIFYID")]
        public string IdentifyID { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
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