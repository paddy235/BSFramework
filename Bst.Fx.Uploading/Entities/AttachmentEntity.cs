using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading.Entities
{
    [Table("base_fileinfo")]
    public class AttachmentEntity
    {
        [Key]
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        [Column("FILEEXTENSIONS")]
        public string Extention { get; set; }
        public string OtherUrl { get; set; }
    }
}
