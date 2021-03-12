using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    [Table("WG_ACTIVITYSUBJECT")]
    public class ActivitySubjectEntity
    {
        [Column("ACTIVITYSUBJECTID")]
        public string ActivitySubjectId { get; set; }
        [Column("SEQ")]
        public int? Seq { get; set; }
        [Column("ACTIVITYSUBJECT")]
        public string ActivitySubject { get; set; }
        [Column("STATUS")]
        public string Status { get; set; }
        [Column("SUBJECTTYPE")]
        public string SubjectType { get; set; }
    }
}
