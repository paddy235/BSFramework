using BSFramework.Application.Entity.EmergencyManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class EmergencyCardModel
    {
        public string CardId { get; set; }
        public string CardName { get; set; }
        public EmergencyCardModel ParentCard { get; set; }
    }

    public class GetTypeDetailReturn
    {
        public bool ispush { get; set; }
        public List<EmergencyEntity> entity { get; set; }
    }
}