using BSFramework.Application.Entity.WorkPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class HumanModel
    {
        public string UserId { get; set; }
        public string UsreName { get; set; }
        public int Total1 { get; set; }
        public int Total2 { get; set; }
        public int Total3 { get; set; }
        public DateTime Date { get; set; }
    }
}