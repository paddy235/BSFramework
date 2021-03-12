using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 描 述：部门管理
    /// </summary>
    [Table("BASE_DEPARTMENT")]
    public class DepartmentEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 部门主键
        /// </summary>	
        [Column("DEPARTMENTID")]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 机构主键
        /// </summary>		
        [Column("ORGANIZEID")]
        public string OrganizeId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>		
        [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 部门代码
        /// </summary>		
        [Column("ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>		
        [Column("FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 部门简称
        /// </summary>		
        [Column("SHORTNAME")]
        public string ShortName { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>		
        [Column("NATURE")]
        public string Nature { get; set; }
        /// <summary>
        /// 负责人主键
        /// </summary>		
        [Column("MANAGERID")]
        public string ManagerId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>		
        [Column("MANAGER")]
        public string Manager { get; set; }
        /// <summary>
        /// 外线电话
        /// </summary>		
        [Column("OUTERPHONE")]
        public string OuterPhone { get; set; }
        /// <summary>
        /// 内线电话
        /// </summary>		
        [Column("INNERPHONE")]
        public string InnerPhone { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>		
        [Column("EMAIL")]
        public string Email { get; set; }
        /// <summary>
        ///专业类别
        /// </summary>
        [Column("SPECIALTYTYPE")]
        public string SpecialtyType { get; set; }
        /// <summary>
        /// 部门传真
        /// </summary>		
        [Column("FAX")]
        public string Fax { get; set; }
        /// <summary>
        /// 层
        /// </summary>		
        [Column("LAYER")]
        public int? Layer { get; set; }
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
        /// 是否特殊部门
        /// </summary>
        [Column("ISSPECIAL")]
        public bool IsSpecial { get; set; }
        [Column("TRAINUSER")]
        public string TrainUser { get; set; }
        [Column("STARTDATE")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 厂级部门
        /// </summary>
        [Column("ISORG")]
        public bool? IsOrg { get; set; }

        /// <summary>
        /// 职位人数
        /// </summary>
        /// 
        [NotMapped]
        public string NumberOfPeople { get; set; }

        /// <summary>
        /// 班组类型  01运行 02检修  03其他
        /// </summary>
        /// 
        [Column("TEAMTYPE")]
        public string TeamType { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.DepartmentId = string.IsNullOrEmpty(DepartmentId) ? Guid.NewGuid().ToString() : DepartmentId;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.DepartmentId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}