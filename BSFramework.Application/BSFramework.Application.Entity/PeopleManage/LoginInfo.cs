using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace BSFramework.Application.Entity.PeopleManage
{
     [Table("wg_logininfo")]
    public class LoginInfo : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        [Column("UserId")]
        public String UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Finger")]
        public String Finger { get; set; }
        [Column("Face")]
        public String Face { get; set; }

        [Column("IdentityNo")]
        public String IdentityNo { get; set; }

        [Column("LabourNo")]
        public String LabourNo { get; set; }

        [NotMapped]
        public String Tel { get; set; }
    }
}