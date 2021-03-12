using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PeopleManage
{
     [Table("wg_peopleduty")]
    public class PeopleDutyEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        [Column("PeopleId")]
        public String PeopleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Name")]
        public String Name { get; set; }

        [Column("State")]
        public String State { get; set; }

        [Column("DeptId")]
        public String DeptId { get; set; }

        [Column("BZId")]
        public String BZId { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("SignDate")]
        public DateTime SignDate { get; set; }

        [Column("DutyMan")]
        public String DutyMan { get; set; }
        [Column("ParentDutyMan")]
        public String ParentDutyMan { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        [Column("Year")]
        public String Year { get; set; }

        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        /// <summary>
        /// 图片路劲
        /// </summary>
         [NotMapped]
         public IList fileList { get; set; }
         [NotMapped]
         public string qr { get; set; }

         [Column("TypeId")]
         public String TypeId { get; set; }

         [Column("TypeName")]
         public String TypeName { get; set; }
    }

}
