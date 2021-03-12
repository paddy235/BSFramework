using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class UserModel
    {
        public string Factory { get; set; }
        public string Quarters { get; set; }
        public string TeamType { get; set; }

        public string userAccount { get; set; }
        public string userName { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string deptId { get; set; }
        public string deptCode { get; set; }
        public string deptName { get; set; }
        public string roleName { get; set; }
        public string postName { get; set; }
        public string faceUrl { get; set; }
        public string userNum { get; set; }
        public string breakRuleAdmin { get; set; }
        public string userId { get; set; }
    }

}