using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class SaftyCheckDataRecordModel
    {
        public int records { get; set; }
        public int page { get; set; }
        public List<SaftyCheckDataRecordrows> rows { get; set; }
    }

    public class SaftyCheckDataRecordrows
    {
        public string Id { get; set; }
        public string CheckBeginTime { get; set; }
        public string CheckEndTime { get; set; }    
        public string CheckDataRecordName { get; set; }
        public string CheckMan { get; set; }
        public string SolveCount { get; set; }
        public string Count { get; set; }
        public string CheckDataType { get; set; }



    }
}