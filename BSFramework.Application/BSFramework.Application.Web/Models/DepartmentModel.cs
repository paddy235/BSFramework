using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Models
{
    /// <summary>
    /// 部门
    /// </summary>
    public class DepartmentModel
    {
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ParentDepartmentId { get; set; }
        public string ParentDeaprtmentName { get; set; }
        public string DepartmentCode { get; set; }

    }
}