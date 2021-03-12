using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// Ä¬ÈÏ³öÇÚ
    /// </summary>
    public class UserSignSettingEntity
    {
        public string UserId { get; set; }
        public bool IsSigned { get; set; }
        public string Reason { get; set; }
        public string ReasonRemark { get; set; }
    }
}