using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.Empower
{
    [Table("wg_empower")]
    public class EmpowerEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        /// <summary>
        /// 单位ID
        /// </summary>
        [Column("DepartId")]
        public String DepartId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Column("DepartName")]
        public String DepartName { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        [Column("ModelId")]
        public String ModelId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Column("ModelName")]
        public String ModelName { get; set; }
    }
}
