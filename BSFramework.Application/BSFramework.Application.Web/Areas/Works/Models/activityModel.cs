using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{

    public class GetGroupCountList
    {
        public string activityid { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string dpeitid { get; set; }
        public string deptname { get; set; }
        public string deptcode { get; set; }
        public int sum { get; set; }

    }
    public class DangerGetGroupCountList
    {
        public string activityid { get; set; }
        public DateTime starttime { get; set; }
        public DateTime? endtime { get; set; }
        public string dpeitid { get; set; }
        public string deptname { get; set; }
        public string deptcode { get; set; }
        public int sum { get; set; }
        public string jobuser { get; set; }
        public string persons { get; set; }

    }

}