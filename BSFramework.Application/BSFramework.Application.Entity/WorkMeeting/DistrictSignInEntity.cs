using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Application.Entity.WorkMeeting
{
    public class DistrictSignInEntity
    {
        [Column("SIGNINID")]
        public string SigninId { get; set; }
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }
        [Column("DISTRICTNAME")]
        public string DistrictName { get; set; }
        [Column("SIGNINDATE")]
        public DateTime SigninDate { get; set; }
        [Column("USERID")]
        public string UserId { get; set; }
        [Column("DUTYDEPARTMENTID")]
        public string DutyDepartmentId { get; set; }
        [Column("DUTYDEPARTMENTNAME")]
        public string DutyDepartmentName { get; set; }
    }
}