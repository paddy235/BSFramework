using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class MeetingModel
    {
        public string DeptName { get; set; }
        public string Meeting1 { get; set; }
        public string Meeting2 { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string MeetingName { get; set; }
    }
}