using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    [Table("wg_glassstock")]
    public class GlassStockEntity : BaseEntity
    {

        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 器皿id
        /// </summary>
        [Column("GLASSID")]
        public String GlassId { get; set; }

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
        /// 损耗原因
        /// </summary>
        [Column("REASON")]
        public String Reason { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column("AMOUNT")]
        public String Amount { get; set; }

        /// <summary>
        /// 出入库
        /// </summary>
        [Column("TYPE")]
        public String Type { get; set; }

        [Column("Spec")]
        public String Spec { get; set; }

        [Column("Name")]
        public String Name { get; set; }

        [Column("CurrentNumber")]
        public int CurrentNumber { get; set; }
    }
}
