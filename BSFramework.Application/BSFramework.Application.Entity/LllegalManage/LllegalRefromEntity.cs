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
    [Table("wg_lllegalRefrom")]
    public class LllegalRefromEntity : BaseEntity
    {
        [Column("Id")]
        public String Id { get; set; }

        /// <summary>
        /// 违章Id
        /// </summary>
        [Column("LllegalId")]
        public String LllegalId { get; set; }

        /// <summary>
        /// 整改人
        /// </summary>
        [Column("RefromPeople")]
        public String RefromPeople { get; set; }

        /// <summary>
        /// 整改人Id
        /// </summary>
        [Column("RefromPeopleId")]
        public String RefromPeopleId { get; set; }

        /// <summary>
        /// 整改结果
        /// </summary>
        [Column("RefromResult")]
        public String RefromResult { get; set; }

        /// <summary>
        /// 整改结果
        /// </summary>
        [Column("RefromMind")]
        public String RefromMind { get; set; }

        /// <summary>
        /// 整改时间
        /// </summary>
        [Column("RefromTime")]
        public DateTime? RefromTime { get; set; }

        [NotMapped]
        /// <summary>
        /// 图片路劲
        /// </summary>
        public IList fileList { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
