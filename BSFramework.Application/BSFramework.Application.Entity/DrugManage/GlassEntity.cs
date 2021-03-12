using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    [Table("wg_glass")]
    public class GlassEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        [Column("GlassWareId")]
        public String GlassWareId { get; set; }

        [Column("Location")]
        public String Location { get; set; }
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

        [Column("CREATEUSERID")]
        public String CreateUserId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [Column("SPEC")]
        public String Spec { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column("AMOUNT")]
        public String Amount { get; set; }

        /// <summary>
        /// 预警值
        /// </summary>
        [Column("WARN")]
        public String Warn { get; set; }

        [Column("CreateUserName")]
        public String CreateUserName { get; set; }

        [NotMapped]
        public String BGPath { get; set; }

    }
}
