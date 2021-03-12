using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.SystemManage.Models
{
    public class DistrictModel
    {
        public string DistrictID { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string ChargeDept { get; set; }
        public string DisreictChargePerson { get; set; }
        public string BelongCompany { get; set; }
        public string ParentID { get; set; }
        public bool Expanded { get; set; }
        public bool IsLeaf { get; set; }
        public int Level { get; set; }
        public bool hasChildren { get; set; }
        public int lft { get; set; }
        public int rgt { get; set; }
    }
}