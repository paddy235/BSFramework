using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_REPORT")]
    public class Report
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("REPORTID")]
        public Guid ReportId { get; set; }
        [StringLength(36)]
        [Column("REPORTUSERID")]
        public string ReportUserId { get; set; }
        [StringLength(50)]
        [Column("REPORTUSER")]
        public string ReportUser { get; set; }
        [StringLength(36)]
        [Column("REPORTDEPTID")]
        public string ReportDeptId { get; set; }
        [StringLength(50)]
        [Column("REPORTDEPTNAME")]
        public string ReportDeptName { get; set; }
        [Column("REPORTTIME")]
        public DateTime ReportTime { get; set; }
        [Column("REPORTTYPE")]
        public ReportType ReportType { get; set; }
        [StringLength(2000)]
        [Column("REPORTCONTENT")]
        public string ReportContent { get; set; }
        [StringLength(2000)]
        [Column("TASKS")]
        public string Tasks { get; set; }
        [StringLength(2000)]
        [Column("PLAN")]
        public string Plan { get; set; }
        [StringLength(2000)]
        [Column("CANTDO")]
        public string Cantdo { get; set; }
        [StringLength(2000)]
        [Column("UNDO")]
        public string Undo { get; set; }
        [Column("STARTTIME")]
        public DateTime StartTime { get; set; }
        [Column("ENDTIME")]
        public DateTime EndTime { get; set; }
        [InverseProperty("REPORT")]
        public List<ReportNotice> Notices { get; set; }
        [InverseProperty("REPORT")]
        public List<Comment> Comments { get; set; }
        [Column("ISSUBMIT")]
        public bool IsSubmit { get; set; }
    }

    [Table("WG_REPORTNOTICE")]
    public class ReportNotice
    {
        [Key]
        [Column("NOTICEID")]
        public Guid NoticeId { get; set; }
        [StringLength(36)]
        [Column("USERID")]
        public string UserId { get; set; }
        [StringLength(50)]
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("ISREAD")]
        public bool IsRead { get; set; }
        [Column("REPORTID")]
        public Guid ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report Report { get; set; }
        [Column("NOTICETYPE")]
        public int NoticeType { get; set; }
    }

    [Table("WG_REPORTCOMMENT")]
    public class Comment
    {
        [Key]
        [Column("COMMENTID")]
        public Guid CommentId { get; set; }
        [StringLength(500)]
        [Column("CONTENT")]
        public string Content { get; set; }
        [StringLength(36)]
        [Column("COMMENTUSERID")]
        public string CommentUserId { get; set; }
        [StringLength(30)]
        [Column("COMMENTUSER")]
        public string CommentUser { get; set; }
        [Column("COMMENTTIME")]
        public DateTime CommentTime { get; set; }
        [Column("REPORTID")]
        public Guid ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report Report { get; set; }
    }

    public enum ReportType
    {
        Weekly,
        Monthly
    }
}
