using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("wg_warningmessage")]
    public class WarningMessage
    {
        [Key]
        public Guid WarningMessageId { get; set; }
        [StringLength(36)]
        public string BusinessId { get; set; }
        [StringLength(30)]
        public string MessageKey { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
