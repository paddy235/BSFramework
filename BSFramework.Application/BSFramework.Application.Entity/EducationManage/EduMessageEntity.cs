using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("wg_edumessage")]
    public class EduMessageEntity
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
        /// 消息内容
        /// </summary>
        [Column("Content")]
        public String Content { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        [Column("InceptPeople")]
        public String InceptPeople { get; set; }
        /// <summary>
        /// 接收人id
        /// </summary>
        [Column("InceptPeopleId")]
        public String InceptPeopleId { get; set; }
    }
}
