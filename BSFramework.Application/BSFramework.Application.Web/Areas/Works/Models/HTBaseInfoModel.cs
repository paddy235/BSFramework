using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class HTBaseInfoModel
    {
        public int records { get; set; }
        public int page { get; set; }
        public List<HTBaseInforows> rows { get; set; }
    }

    public class HTBaseInforows
    {
        public string Id { get; set; }
        public string  Workstream { get; set; }
        public string Hidcode { get; set; }
        public string Hidtypename { get; set; }
        public string Hidrankname { get; set; }
        public string Checktypename { get; set; }
        public string Checkdepartname { get; set; }
        public string Hiddescribe { get; set; }


    }
}