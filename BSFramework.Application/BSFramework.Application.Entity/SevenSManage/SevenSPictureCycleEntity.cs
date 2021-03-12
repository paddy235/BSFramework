using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{
    [Table("wg_sevenspicturecycle")]
    public class SevenSPictureCycleEntity
    {
        [Column("Id")]
        public String Id { get; set; }
        [Column("cycle")]
        public String cycle { get; set; }
        [Column("iswork")]
        public bool iswork { get; set; }
        [Column("sort")]
        public int sort { get; set; }
        [Column("starttime")]
        public string starttime { get; set; }
        [Column("regulation")]
        public string regulation { get; set; }
        

    }
}
