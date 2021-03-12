using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("WG_TOOLINFO")]
    public class ToolInfoEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        [Column("TYPEID")]
        public String TypeId { get; set; }

        [Column("NAME")]
        public String Name { get; set; }

        [Column("SPEC")]
        public String Spec { get; set; }

        [Column("TOTAL")]
        public String Total { get; set; }

        [Column("OUTDATE")]
        public DateTime? OutDate { get; set; }

        [Column("PROFACTORY")]
        public String ProFactory { get; set; }
        [Column("VALIDATE")]
        public DateTime? ValiDate { get; set; }
        /// <summary>
        /// 检验日期
        /// </summary>
        [Column("TOOLCHECKDATE")]
        public DateTime? ToolcheckDate { get; set; }

        [Column("CHECKCYCLE")]
        public String CheckCycle { get; set; }

        [Column("HGZ")]
        public String HGZ { get; set; }

        [Column("CHECKREPORT")]
        public String CheckReport { get; set; }

        [Column("REGPERSONNAME")]
        public String RegPersonName { get; set; }

        [Column("REGDATE")]
        public DateTime? RegDate { get; set; }

        [Column("REGPERSONID")]
        public String RegPersonId { get; set; }

        [Column("CERTIFICATE")]
        public String Certificate { get; set; }

        [Column("CURRENTNUMBER")]
        public String CurrentNumber { get; set; }

        [Column("ISGOOD")]
        public String IsGood { get; set; }

        [Column("STATE")]
        public String State { get; set; }

        [Column("HGZPATH")]
        public String HGZPath { get; set; }
        [Column("CERPATH")]
        public String CerPath { get; set; }
        [Column("CHECKPATH")]
        public String CheckPath { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("REMIND")]
        public String Remind { get; set; }
        [Column("NUMBERS")]
        public String Numbers { get; set; }
        [Column("BREAKNUMBERS")]
        public String BreakNumbers { get; set; }
        [Column("INVENTORYID")]
        public String InventoryId { get; set; }
        [Column("BZID")]
        public String BZID { get; set; }
        [Column("BZNAME")]
        public String BZName { get; set; }
        [Column("AMOUNT")]
        public Int32 Amount { get; set; }
        /// <summary>
        /// 存放地点
        /// </summary>
        [Column("DEPOSITPLACE")]
        public string DepositPlace { get; set; }

        [NotMapped]
        public DateTime? CheckDate { get; set; }
        [NotMapped]
        public string CheckState { get; set; }
        [NotMapped]
        public string CheckResult { get; set; }
        [NotMapped]
        public string CheckPeople { get; set; }
    }
}
