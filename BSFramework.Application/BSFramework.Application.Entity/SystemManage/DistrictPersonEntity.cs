using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 区域责任人
    /// </summary>
    public class DistrictPersonEntity
    {
        [Column("DISTRICTPERSONID")]
        public string DistrictPersonId { get; set; }
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }
        [Column("DISTRICTCODE")]
        public string DistrictCode { get; set; }
        [Column("DISTRICTNAME")]
        public string DistrictName { get; set; }
        [Column("DUTYDEPARTMENTID")]
        public string DutyDepartmentId { get; set; }
        [Column("DUTYDEPARTMENTNAME")]
        public string DutyDepartmentName { get; set; }
        [Column("DUTYUSERID")]
        public string DutyUserId { get; set; }
        [Column("DUTYUSER")]
        public string DutyUser { get; set; }
        [Column("COMPANYID")]
        public string CompanyId { get; set; }
        [Column("COMPANYNAME")]
        public string CompanyName { get; set; }
        [Column("CATEGORYID")]
        public string CategoryId { get; set; }
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        [Column("PHONE")]
        public string Phone { get; set; }
        [Column("CYCLE")]
        public string Cycle { get; set; }
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        [Column("UPDATEDATE")]
        public DateTime? UpdateDate { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("UPDATEUSERID")]
        public string UpdateUserId { get; set; }
    }
}
