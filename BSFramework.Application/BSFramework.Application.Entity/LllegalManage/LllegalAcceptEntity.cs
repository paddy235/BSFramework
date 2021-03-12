using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.PublicInfoManage;
namespace BSFramework.Application.Entity.LllegalManage
{
    [Table("wg_lllegalAccept")]
    public class LllegalAcceptEntity
    {
        [Column("Id")]
        public String Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("LllegalId")]
        public String LllegalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("AcceptPeople")]
        public String AcceptPeople { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("AcceptPeopleId")]
        public String AcceptPeopleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("AcceptResult")]
        public String AcceptResult { get; set; }


        [Column("AcceptMind")]
        public String AcceptMind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("AcceptTime")]
        public DateTime? AcceptTime { get; set; }

        [NotMapped]
        /// <summary>
        /// 图片路劲
        /// </summary>
        public IList fileList { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
