using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_MESSAGE")]
    public class Message
    {
        [Key]
        [Column("MESSAGEID")]
        public Guid MessageId { get; set; }
        [StringLength(500)]
        [Column("TITLE")]
        public string Title { get; set; }
        [StringLength(2000)]
        [Column("CONTENT")]
        public string Content { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("USERID")]
        public string UserId { get; set; }
        [Index(IsClustered = true)]
        [StringLength(36)]
        [Column("BUSINESSID")]
        public string BusinessId { get; set; }
        [Column("ISFINISHED")]
        public bool IsFinished { get; set; }
        [Column("HASREADED")]
        public bool HasReaded { get; set; }
        [Column("CATEGORY")]
        public MessageCategory Category { get; set; }
        [Index(IsClustered = true)]
        [StringLength(30)]
        [Column("MESSAGEKEY")]
        public string MessageKey { get; set; }
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
    }
}
