using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkMeeting
{
    class SafetyHazardEntity
    {
    }

    public class SafetyHazardChart
    {
        public List<SafetyHazardChartRows> rows { get; set; }
    }
    public class SafetyHazardChartRows
    {
        public string month { get; set; }

        public int rc { get; set; }
        public int zx { get; set; }
        public int jj { get; set; }
        public int jjr { get; set; }
        public int sum { get; set; }
        public int zh { get; set; }
    }
}
