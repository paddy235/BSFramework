using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PeopleManage
{
    [Table("wg_dutydanger")]
    public class DutyDangerEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        [Column("RoleId")]
        public String RoleId { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        [Column("RoleName")]
        public String RoleName { get; set; }
        [Column("Danger")]
        public String Danger { get; set; }
        [Column("Measure")]
        public String Measure { get; set; }
        [Column("ReviseUserId")]
        public String ReviseUserId { get; set; }
        [Column("ReviseUserName")]
        public String ReviseUserName { get; set; }
        [Column("ReviseDate")]
        public DateTime? ReviseDate { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        [Column("CreateUserName")]
        public String CreateUserName { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("DutyContent")]
        public String DutyContent { get; set; }
        [Column("DangerReviseUserId")]
        public String DangerReviseUserId { get; set; }
        [Column("DangerReviseUserName")]
        public String DangerReviseUserName { get; set; }
        [Column("DangerReviseDate")]
        public DateTime? DangerReviseDate { get; set; }
    }
}
