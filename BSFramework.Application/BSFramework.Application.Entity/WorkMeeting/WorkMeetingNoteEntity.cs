using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkMeeting
{
    /// <summary>
    /// 当班记事
    /// 一个班前班后会只有一个当班记事
    /// </summary>
    [Table("wg_WorkMeetingNote")]
   public class WorkMeetingNoteEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 班前班后会Id
        /// </summary>
        [Column("MeetingId")]
        public string MeetingId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Column("Describes")]
        public string Describes { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK1")]
        public string BK1 { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK2")]
        public string BK2 { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK3")]
        public string BK3 { get; set; }

        [Column("CreateUserId")]
        public string CreateUserId { get; set; }

        [Column("CreateUser")]
        public string CreateUser { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("MODIFYDATE")]
        public DateTime MODIFYDATE { get; set; }

        [Column("MODIFYUserId")]
        public string MODIFYUserId { get; set; }

        [Column("MODIFYUser")]
        public string MODIFYUser { get; set; }
        [NotMapped]
        public List<FileClass> Files { get; set; }
    }
    /// <summary>
    /// 班组荣誉/风采剪影
    /// </summary>
    public class FileClass
    {
        public string fileid { get; set; }
        public string description { get; set; }
        public string createdate { get; set; }
        public string modifydate { get; set; }
        public string filepath { get; set; }
        public string filetype { get; set; }
        public int key { get; set; }
    }
}
