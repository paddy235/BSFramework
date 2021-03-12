using BSFramework.Application.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.SystemManage.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DistrictPersonModel
    {
        public string DistrictPersonId { get; set; }
        public string DistrictId { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string DutyDepartmentId { get; set; }
        public string DutyDepartmentName { get; set; }
        public string DutyUserId { get; set; }
        public string DutyUser { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Phone { get; set; }
        public string Cycle { get; set; }
    }
}