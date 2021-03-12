using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models.QueryModels
{
    public class TaskQueryModel
    {
        public string UserId { get; set; }
        public string DistrictId { get; set; }
    }

    public class DistrictTaskModel
    {
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string[] Tasks { get; set; }
    }
}