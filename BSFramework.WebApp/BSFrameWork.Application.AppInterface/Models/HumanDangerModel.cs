using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class HumanDangerModel
    {
        public string[] Users { get; set; }
        public string[] DutyUser { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Status { get; set; }
        public string Level { get; set; }
        public string EvaluateStatus { get; set; }
        public string UserId { get; set; }
    }
}