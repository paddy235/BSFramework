using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    /// <summary>
    /// 活动人员
    /// </summary>
    [Table("wg_edactivityperson")]
    public class EdActivityPersonEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityPersonId")]
        public string ActivityPersonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityId")]
        public string ActivityId { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        [Column("IsSigned")]
        public bool IsSigned { get; set; }
    }
}
