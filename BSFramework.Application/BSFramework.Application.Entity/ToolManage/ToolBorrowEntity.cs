using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("wg_toolborrow")]
    public class ToolBorrowEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }


        [Column("TYPEID")]
        public String TypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("TOOLNAME")]
        public String ToolName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("TOOLSPEC")]
        public String ToolSpec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("BORROWPERSON")]
        public String BorrwoPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BORROWPERSONID")]
        public String BorrwoPersonId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("BORROWDATE")]
        public DateTime BorrwoDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("BACKDATE")]
        public DateTime? BackDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ISGOOD")]
        public String IsGood { get; set; }
        /// <summary>
        /// 备用字段  -- 破损状态
        /// </summary>
        [Column("REMARK")]
        public String Remark { get; set; }

        [Column("BZID")]
        /// <summary>
        /// 班组id
        /// </summary>
        public String BZId
        {
            get;
            set;
        }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [NotMapped]
        public String Name { get; set; }

        [Column("Instruction")]
        public String Instruction { get; set; }

        [Column("BZName")]
        public string BZName { get; set; }
    }
}
