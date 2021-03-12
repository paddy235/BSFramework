using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("wg_toolnumber")]
    public class ToolNumberEntity : BaseEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("ToolId")]
        public String ToolId { get; set; }
        [Column("Number")]
        public String Number { get; set; }
        [Column("IsBreak")]
        public bool IsBreak { get; set; }
    }
}
