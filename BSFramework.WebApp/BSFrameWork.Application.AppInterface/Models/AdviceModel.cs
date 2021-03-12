using BSFramework.Application.Entity.InnovationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class AdviceModel
    {
        public AdviceEntity adddata { get; set; }
        public AdviceEntity updatedata { get; set; }
        public string deldata { get; set; }
        public string DelKeys { get; set; }
        public AdviceAuditEntity audit { get; set; }
        public AdviceAuditEntity auditupdate { get; set; }
    }
}