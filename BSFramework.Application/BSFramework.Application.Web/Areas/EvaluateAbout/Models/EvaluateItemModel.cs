using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Models
{
    public class EvaluateItemModel
    {
        public string EvaluateItemId { get; set; }
        public string EvaluateContentId { get; set; }
        public string EvaluateContent { get; set; }
        public string EvaluateStandard { get; set; }
        public string EvaluateDept { get; set; }
        public string EvaluatePerson { get; set; }
        public DateTime? EvaluateTime { get; set; }
        public decimal? Score { get; set; }
        public decimal? ActualScore { get; set; }
        public string Reason { get; set; }
    }
}