using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class CostSummaryModel
    {
        public string Category { get; set; }
        public decimal[] Data { get; set; }
    }

    public class CostArgModel
    {
        public int Year { get; set; }
        public string DeptId { get; set; }
    }

    public class CostRecordModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}