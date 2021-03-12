using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class DangerAnalysisModel
    {
        public string Danger { get; set; }
        public string JobId { get; set; }
        public List<string> Measures { get; set; }
    }
}