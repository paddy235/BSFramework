using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkMeeting
{
   public  class DangerHiddenEntity
    {
    }
    #region 调用隐患数量统计


    public class GetHtLevelChartJsonEntity
    {
        public string name { get; set; }
        public float value { get; set; }

    }
    public class GetHtLevelChartSumJsonEntity
    {
        public string name { get; set; }
        public int value { get; set; }

    }
    public class GetHtLevelChartReturnEntity
    {

        public List<GetHtLevelChartSumJsonEntity> sumJson { get; set; }
        public string[] data { get; set; }
        public List<GetHtLevelChartJsonEntity> arr { get; set; }
    }
    #endregion
    #region 调用隐患数量趋势
    public class GetHtNumChartJsonEntity
    {
        public string name { get; set; }
        public int[] data { get; set; }

    }
    #endregion
    #region 调用隐患数量统计
    public class GetHtNumReadjustChartEntity
    {
        public List<TimeData> tdata { get; set; }
        public List<SumData> sdata { get; set; }

    }
    public class SumData
    {
        public string name { get; set; }
        public int[] data { get; set; }

    }
    public class TimeData
    {
        public string month { get; set; }

        public float yvalue { get; set; }
        public float wvalue { get; set; }
    }
    #endregion
    #region 调用隐患数量统计
    public class GetHtNumChangeChartData
    {
        public string name { get; set; }
        public float[] data { get; set; }

    }
    #endregion
}
