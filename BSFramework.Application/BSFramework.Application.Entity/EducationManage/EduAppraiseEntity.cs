using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("wg_eduappraise")]
    public class EduAppraiseEntity
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
        /// 评论内容
        /// </summary>
        [Column("AppraiseContent")]
        public String AppraiseContent { get; set; }
        /// <summary>
        /// 评论人
        /// </summary>
        [Column("AppraisePeople")]
        public String AppraisePeople { get; set; }
        /// <summary>
        /// 评论人id
        /// </summary>
        [Column("AppraisePeopleId")]
        public String AppraisePeopleId { get; set; }
        /// <summary>
        /// 评论日期
        /// </summary>
        [Column("AppraiseDate")]
        public DateTime? AppraiseDate { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        [Column("Grade")]
        public String Grade { get; set; }


    }
}
