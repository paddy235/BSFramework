using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class DeptCycleTaskModel
    {
        public string content { get; set; }
        public string dutyuserid { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
        public string workstate { get; set; }

        public string app { get; set; }
    }
}