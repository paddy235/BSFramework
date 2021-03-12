using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("BASE_REPORTSETTING")]
    public class ReportSetting
    {
        [Key]
        [Column("SETTINGID")]
        public Guid SettingId { get; set; }
        [StringLength(200)]
        [Column("SETTINGNAME")]
        public string SettingName { get; set; }
        [Column("STARTTIME")]
        public DateTime StartTime { get; set; }
        [Column("ENDTIME")]
        public DateTime EndTime { get; set; }
        [Column("START")]
        public int Start { get; set; }
        [Column("END")]
        public int End { get; set; }
    }
}
