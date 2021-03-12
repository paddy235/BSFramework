using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    public class GetStatisticsChartJsonEntity
    {
        public string name { get; set; }
        public float value { get; set; }

    }
    public class GetStatisticsChartSumJsonEntity
    {
        public string name { get; set; }
        public int value { get; set; }

    }
    public class GetStatisticsChartReturnEntity
    {

        public List<GetStatisticsChartSumJsonEntity> sumJson { get; set; }
        public string[] data { get; set; }
        public List<GetStatisticsChartJsonEntity> arr { get; set; }
    }
}
