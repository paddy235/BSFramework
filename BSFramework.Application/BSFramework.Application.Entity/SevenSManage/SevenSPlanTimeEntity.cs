using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{
    [Table("wg_sevensplantime")]
    public class SevenSPlanTimeEntity : BaseEntity
    {
        [Column("Id")]
        public String Id { get; set; }

        [Column("PlanTime")]
        public String PlanTime { get; set; }
    }
}
