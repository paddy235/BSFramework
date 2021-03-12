using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Bst.Bzzd.DataSource.Entities
{
    [Table("BASE_WARNINGCONFIG")]
    public class WarningConfig
    {
        [Key]
        [Column("CONFIGID")]
        public Guid ConfigId { get; set; }
        [StringLength(200)]
        [Column("ASSEMBLY")]
        public string Assembly { get; set; }
        [StringLength(30)]
        [Column("MESSAGEKEY")]
        public string MessageKey { get; set; }
        [Column("ENABLED")]
        public bool Enabled { get; set; }
    }
}
