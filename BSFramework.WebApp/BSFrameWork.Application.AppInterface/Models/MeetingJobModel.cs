using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class MeetingJobModel
    {
        public string JobId { get; set; }
        public string Job { get; set; }
        public string StartMeetingId { get; set; }
        public string EndMeetingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Dangerous { get; set; }
        public string Measure { get; set; }
        public string IsFinished { get; set; }
        public string Remark { get; set; }
        public string GroupId { get; set; }
        public string Score { get; set; }
        public string TemplateId { get; set; }
        public bool NeedTrain { get; set; }
        public string Description { get; set; }
        public MeetingAndJobModel Relation { get; set; }
        public string JobUsers { get; set; }
    }

    public class MeetingAndJobModel
    {
        public string StartMeetingId { get; set; }
        public string MeetingJobId { get; set; }
        public List<JobUserModel> JobUsers { get; set; }
    }

    public class JobUserModel
    {
        public string JobUserId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string JobType { get; set; }
        public DateTime CreateDate { get; set; }
        public int? Score { get; set; }
    }
}