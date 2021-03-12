using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.ProducingCheck
{
    /// <summary>
    /// 问题类别
    /// </summary>
    public class CheckTemplateEntity
    {
        [Column("TEMPLATEID")]
        public string TemplateId { get; set; }
        [Column("CATEGORYID")]
        public string CategoryId { get; set; }
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        [Column("PROBLEMCONTENT")]
        public string ProblemContent { get; set; }
        [Column("PROBLEMMEASURE")]
        public string ProblemMeasure { get; set; }
        [Column("DUTYDEPARTMENTID")]
        public string DutyDepartmentId { get; set; }
        [Column("DUTYDEPARTMENTNAME")]
        public string DutyDepartmentName { get; set; }
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }
        [Column("DISTRICTNAME")]
        public string DistrictName { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }
    }
}
