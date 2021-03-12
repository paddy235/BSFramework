using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class MeetingJobModel
    {
        public string JobId { get; set; }
        public List<JobUserModel> JobUsers { get; set; }
    }

    public class JobUserModel
    {
        public string JobUserId { get; set; }
        public int? Score { get; set; }
    }
}