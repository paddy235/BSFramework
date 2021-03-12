using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{
    /// <summary>
    /// 
    /// </summary>
    [Table("wg_sevenstype")]
    public class SevenSTypeEntity : BaseEntity
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
        public List<SevenSEntity> childList { get; set; }

    }
}
