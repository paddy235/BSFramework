using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BSFramework.Application.Entity.PeopleManage
{

    [Table("wg_peopledutytype")]
    public class PeopleDutyTypeEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Name")]
        public String Name { get; set; }

        [Column("BZID")]
        public String BZID { get; set; }

        [Column("BZName")]
        public String BZName { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; }


    }
}
