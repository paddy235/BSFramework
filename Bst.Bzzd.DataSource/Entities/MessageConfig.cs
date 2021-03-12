using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("BASE_MESSAGECONFIG")]
    public class MessageConfig
    {
        [Key]
        [Column("CONFIGID")]
        public Guid ConfigId { get; set; }
        [Index(IsUnique = true)]
        [StringLength(30)]
        [Column("CONFIGKEY")]
        public string ConfigKey { get; set; }
        [Column("ENABLED")]
        public bool Enabled { get; set; }
        [StringLength(200)]
        [Column("TITLE")]
        public string Title { get; set; }
        [StringLength(500)]
        [Column("TEMPLATE")]
        public string Template { get; set; }
        [Column("CATEGORY")]
        public MessageCategory Category { get; set; }
        [StringLength(50)]
        [Column("RECIEVETYPE")]
        public string RecieveType { get; set; }
        [StringLength(200)]
        [Column("ASSEMBLY")]
        public string Assembly { get; set; }
    }

    public enum MessageCategory
    {
        Message,
        Todo,
        Warning
    }
}
