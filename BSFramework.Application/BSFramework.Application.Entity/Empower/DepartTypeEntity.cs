using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.Empower
{
    [Table("wg_departtype")]
    public class DepartTypeEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Column("Name")]
        public String Name { get; set; }

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
        /// 父节点id
        /// </summary>
        [Column("ParentId")]
        public String ParentId { get; set; }

        /// <summary>
        /// 父节点名称
        /// </summary>
        [Column("ParentName")]
        public String ParentName { get; set; }


    }
}
