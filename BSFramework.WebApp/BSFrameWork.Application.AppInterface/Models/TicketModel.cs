using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class TicketModel
    {
        public string[] Units { get; set; }
        public string DutyPerson { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public bool IncludeCode { get; set; }
        public string KeyWord { get; set; }
    }
}