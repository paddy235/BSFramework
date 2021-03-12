using BSFramework.Application.Entity.InnovationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class WorkInnovationModel
    {
        public string deldata { get; set; }
        public string DelKeys { get; set; }
        public WorkInnovationEntity main { get; set; }
        public List<WorkInnovationAuditEntity> audit { get; set; }
    }
}