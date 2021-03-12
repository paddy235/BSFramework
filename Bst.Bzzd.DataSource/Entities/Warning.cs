using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_WARNING")]
    public class Warning
    {
        [Key]
        [Column("WARNINGID")]
        public Guid WarningId { get; set; }
        [StringLength(36)]
        [Column("BUSINESSID")]
        public string BusinessId { get; set; }
        [StringLength(30)]
        [Column("MESSAGEKEY")]
        public string MessageKey { get; set; }
        [Column("ISPUBLISHED")]
        public bool IsPublished { get; set; }
    }
}
