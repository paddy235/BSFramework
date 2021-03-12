using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("WG_TOOLTYPE")]
    public class ToolTypeEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Column("NAME")]
        public String Name { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        [Column("PATH")]
        public String Path { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Column("STATE")]
        public String State { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("REMARK")]
        public String Remark { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("DEPTID")]
        public String DeptId { get; set; }

        [Column("BZID")]
        public String BZId { get; set; }

        [Column("REMIND")]
        public String Remind { get; set; }
        [Column("NUMBERS")]
        public String Numbers { get; set; }
        [Column("INVENTORYID")]
        public String InventoryId { get; set; }

        /// <summary>
        /// 工器具类别
        /// </summary>
        [NotMapped]
        public string Type { get; set; }
    }
}
