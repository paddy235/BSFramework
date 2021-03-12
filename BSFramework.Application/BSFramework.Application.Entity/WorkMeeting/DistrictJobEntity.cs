using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Application.Entity.WorkMeeting
{
    public class DistrictJobEntity : BaseEntity
    {
        [Column("DISTRICTJOBID")]
        public string DistrictJobId { get; set; }
        [Column("MEETINGJOBID")]
        public string MeetingJobId { get; set; }
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }
    }
}