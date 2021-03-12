using BSFramework.Application.Entity.OndutyManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{


    public class OndutyMeetimg
    {
        public string keyvalue { get; set; }
        public string meettype { get; set; }
        public string moduletype { get; set; }
    }
    public class IndutyMeetimg
    {
        public List<string> faces { get; set; }
        public string keyvalue { get; set; }
        public string meettype { get; set; }
        public string moduletype { get; set; }
    }

    public class OndutyMeetModel
    {
        public OndutyMeetEntity entity { get; set; }
        public string DelKeys { get; set; }
    }

    public class OnOffModel
    {
        public string keyvalue { get; set; }
        public string onoff { get; set; }
    }
}