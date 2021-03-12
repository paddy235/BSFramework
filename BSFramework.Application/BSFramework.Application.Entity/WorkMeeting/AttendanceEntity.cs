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
    /// Ãè Êö£º¿¼ÇÚÍ³¼Æ
    /// </summary>
    public class AttendanceEntity : BaseEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
        public decimal Chuqin { get; set; }
        public decimal Tiaoxiu { get; set; }
        public decimal Gongxiu { get; set; }
        public decimal Bingjia { get; set; }
        public decimal Shijia { get; set; }
        public decimal Chuchai { get; set; }
        public decimal Qita { get; set; }
    }

    public class AttendanceTypeEntity
    {
        public string Category { get; set; }
        public int Times { get; set; }
        public float Hours { get; set; }

        public string userId { get; set; }
    }

    public class UserAttendanceEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Sort1 { get; set; }
        public DateTime? Sort2 { get; set; }

        public string Photo { get; set; }
        public List<AttendanceTypeEntity> Data { get; set; }

    }

    public class DayAttendanceEntity
    {
        public DateTime Date { get; set; }
        public string State { get; set; }
        public string DayType { get; set; }
        public string ReasonRemark { get; set; }
        public string userid { get; set; }
    }

    public class JobScoreEntity
    {
        public int Month { get; set; }
        public int Score { get; set; }
    }

    public class UserScoreEntity
    {
        public string UserName { get; set; }
        public decimal Score { get; set; }
        public string userid { get; set; }
        public int total { get; set; }
    }
}