using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class FaultModel
    {
        public string[] Units { get; set; }
        public string Specialty { get; set; }
        public string[] Categories { get; set; }
        public string Status { get; set; }
        public string[] Deptname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsBz { get; set; }
    }
}