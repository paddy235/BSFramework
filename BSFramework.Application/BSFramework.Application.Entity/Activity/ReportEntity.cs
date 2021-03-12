using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    [Table("WG_REPORT")]
    public class ReportEntity
    {
        [Column("REPORTID")]
        public Guid ReportId { get; set; }
        [Column("REPORTUSERID")]
        public string ReportUserId { get; set; }
        [Column("REPORTUSER")]
        public string ReportUser { get; set; }
        [NotMapped]
        public string ReportDeptId { get; set; }
        [NotMapped]
        public string ReprotDeptName { get; set; }
        [Column("REPORTTIME")]
        public DateTime ReportTime { get; set; }
        [Column("REPORTCONTENT")]
        public string ReportContent { get; set; }
        [Column("REPORTTYPE")]
        public string ReportType { get; set; }
        [Column("TASKS")]
        public string Tasks { get; set; }
        [Column("PLAN")]
        public string Plan { get; set; }
        [Column("UNDO")]
        public string Undo { get; set; }
        [Column("CANTDO")]
        public string Cantdo { get; set; }
        [Column("STARTTIME")]
        public DateTime? StartTime { get; set; }
        [Column("ENDTIME")]
        public DateTime? EndTime { get; set; }
        [NotMapped]
        public List<TaskEntity> TaskList { get; set; }
        [NotMapped]
        public List<NoticeEntity> Notices { get; set; }
        [NotMapped]
        public List<CommentEntity> Comments { get; set; }
        [NotMapped]
        public string ToUserId { get; set; }
        [NotMapped]
        public string ShareId { get; set; }
        [NotMapped]
        public bool? UnRead { get; set; }
        [NotMapped]
        public int CommentsTotal { get; set; }
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public int ReadTotal { get; set; }
        [NotMapped]
        public string FilePath { get; set; }
        [Column("ISSUBMIT")]
        public bool IsSubmit { get; set; }
    }

    [Table("WG_REPORTTASK")]
    public class TaskEntity
    {
        [Column("REPORTTASKID")]
        public string ReportTaskId { get; set; }
        [Column("TASKCONTENT")]
        public string TaskContent { get; set; }
        [NotMapped]
        public string TaskPrior { get; set; }
        [Column("TASKPERSON")]
        public string TaskPerson { get; set; }
        [Column("TASKPERSONID")]
        public string TaskPersonId { get; set; }
        [Column("STARTTIME")]
        public DateTime? StartTime { get; set; }
        [Column("ENDTIME")]
        public DateTime? EndTime { get; set; }
        [Column("PHOTO")]
        public string Photo { get; set; }
        [Column("REPORTID")]
        public Guid ReportId { get; set; }
    }

    public class CommentEntity
    {
        public Guid CommentId { get; set; }
        public Guid ReportId { get; set; }
        public string Content { get; set; }
        public string CommentUserId { get; set; }
        public string CommentUser { get; set; }
    }

    public class NoticeEntity
    {
        public Guid NoticeId { get; set; }
        public Guid ReportId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }
    }

    public class ReportSettingEntity
    {
        public Guid SettingId { get; set; }
        public string SettingName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
