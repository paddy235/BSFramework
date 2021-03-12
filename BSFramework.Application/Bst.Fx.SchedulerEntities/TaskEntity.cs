using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.SchedulerEntities
{
    public class SchedulerTaskEntity
    {
        public string TaskName { get; set; }
        public string Status { get; set; }
        public DateTime? NextRunTime { get; set; }
        public DateTime LastRunTime { get; set; }
        public string LastResult { get; set; }
        public bool Enabled { get; set; }
        public DateTime ExecuteTime { get; set; }
        public string Category { get; set; }
        public int[] DayOfWeek { get; set; }
        public int[] DaysOfMonth { get; set; }
        public int[] MonthsOfYear { get; set; }
    }
}
