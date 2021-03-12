using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class DeaprtmentTaskModel
    {
        public string Status { get; set; }
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
    }

}