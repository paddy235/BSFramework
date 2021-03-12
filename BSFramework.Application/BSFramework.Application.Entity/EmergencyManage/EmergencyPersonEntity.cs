using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSFramework.Application.Entity.EmergencyManage
{
    [Table("wg_emergencyperson")]
    public class EmergencyPersonEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("emergencypersonid")]
        public string EmergencyPersonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("emergencyreportid")]
        public string EmergencyReportId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("emergencyid")]
        public string EmergencyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("PersonId")]
        public string PersonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Person")]
        public string Person { get; set; }

        [NotMapped]
        public bool IsSigned { get; set; }
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.EmergencyPersonId))
            {
                this.EmergencyPersonId = Guid.NewGuid().ToString();
            }
        }
    }
}
