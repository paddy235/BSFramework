using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("wg_edubook")]
    public class EduBookEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }


        /// <summary>
        /// 教育培训id
        /// </summary>
        [Column("EduId")]
        public String EduId { get; set; }
        /// <summary>
        /// 补课时间
        /// </summary>
        [Column("BookDate")]
        public DateTime? BookDate { get; set; }
        /// <summary>
        /// 补课人
        /// </summary>
        [Column("BookPeople")]
        public String BookPeople { get; set; }
        /// <summary>
        /// 补课人id
        /// </summary>
        [Column("BookPeopleId")]
        public String BookPeopleId { get; set; }
    }
}
