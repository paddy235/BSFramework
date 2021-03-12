using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.ProducingCheck.Models
{
    public class TemplateModel
    {
        public string TemplateId { get; set; }
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string DutyDepartmentId { get; set; }
        public string DutyDepartmentName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProblemContent { get; set; }
        public string ProblemMeasure { get; set; }
        public string OperateUser { get; set; }
        public DateTime OperateDate { get; set; }
    }
}