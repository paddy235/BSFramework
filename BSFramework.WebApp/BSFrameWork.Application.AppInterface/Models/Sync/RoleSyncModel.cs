using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models.Sync
{
    /// <summary>
    /// 岗位
    /// </summary>
    public class RoleSyncModel
    {
        public string RoleId { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentId { get; set; }
        public string RoleIds { get; set; }
        public string RoleNames { get; set; }
    }
}