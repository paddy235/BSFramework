using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;


namespace BSFramework.Application.Entity.EmergencyManage
{

    /// <summary>
    /// 
    /// </summary>
    [Table("wg_emergencycardtype")]
    public class EmergencyCardTypeEntity : BaseEntity
    {
        [Column("TypeId")]
        public string TypeId { get; set; }
        [Column("TypeName")]
        public string TypeName { get; set; }
        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }
        [Column("ParentCardId")]
        public string ParentCardId { get; set; }
        [Column("deptid")]
        public string deptid { get; set; }
        [Column("deptname")]
        public string deptname { get; set; }
        [Column("deptcode")]
        public string deptcode { get; set; }
        [NotMapped]
        public List<EmergencyEntity> childList { get; set; }

    }

}
