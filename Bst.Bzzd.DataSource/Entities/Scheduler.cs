using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("SYS_SCHEDULER")]
    public class Scheduler
    {
        [Key]
        [Column("SCHEDULERID")]
        public Guid SchedulerId { get; set; }
        [StringLength(50)]
        [Column("SCHEDULERNAME")]
        public string SchedulerName { get; set; }
        [StringLength(20)]
        [Column("STATUS")]
        public string Status { get; set; }
        [Column("NEXTRUNTIME")]
        public DateTime? NextRunTime { get; set; }
        [Column("LASTRUNTIME")]
        public DateTime LastRunTime { get; set; }
        [StringLength(500)]
        [Column("TRIGGER")]
        public string Trigger { get; set; }
    }
}
