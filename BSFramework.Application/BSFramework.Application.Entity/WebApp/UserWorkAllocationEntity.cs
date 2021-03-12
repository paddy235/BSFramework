using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WebApp
{
    [Table("wg_userworkallocation")]
    public class UserWorkAllocationEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Key]
        [Column("id")]
        public string Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>	
        [Column("userid")]
        public string userId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>	
        [Column("username")]
        public string username { get; set; }
        /// <summary>
        /// 前部门id
        /// </summary>	
        [Column("olddepartmentid")]
        public string olddepartmentid { get; set; }
        /// <summary>
        /// 前部门名称
        /// </summary>	
        [Column("olddepartment")]
        public string olddepartment { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("departmentid")]
        public string departmentid { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>	
        [Column("department")]
        public string department { get; set; }
        /// <summary>
        /// 移岗时间
        /// </summary>	
        [Column("allocationtime")]
        public string allocationtime { get; set; }
        /// <summary>
        /// 前职务
        /// </summary>	
        [Column("oldquarters")]
        public string oldquarters { get; set; }
        /// <summary>
        /// 前职务
        /// </summary>	
        [Column("oldquartersid")]
        public string oldquartersid { get; set; }
        /// <summary>
        /// 职务
        /// </summary>	
        [Column("quarters")]
        public string quarters { get; set; }
        /// <summary>
        /// 职务
        /// </summary>	
        [Column("quartersid")]
        public string quartersid { get; set; }
        /// <summary>
        /// 前岗位
        /// </summary>	
        [Column("oldRoleDutyName")]
        public string oldRoleDutyName { get; set; }
        /// <summary>
        /// 前岗位
        /// </summary>	
        [Column("oldRoleDutyId")]
        public string oldRoleDutyId { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>	
        [Column("RoleDutyName")]
        public string RoleDutyName { get; set; }
        /// <summary>
        /// 岗位id
        /// </summary>	
        [Column("RoleDutyId")]
        public string RoleDutyId { get; set; }
        /// <summary>
        /// 离厂时间
        /// </summary>	
        [Column("leavetime")]
        public string leavetime
        { get; set; }
        /// <summary>
        /// 离厂说明
        /// </summary>	
        [Column("leaveremark")]
        public string leaveremark { get; set; }
        /// <summary>
        /// 是否处理
        /// </summary>	
        [Column("iscomplete")]
        public bool iscomplete { get; set; }
        /// <summary>
        /// code
        /// </summary>	
        [NotMapped]
        public string Code { get; set; }

        #endregion
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }
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
