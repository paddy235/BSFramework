using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{

    [Table("wg_instrument")]
    public class InstrumentEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        [Column("GlassWareId")]
        public String GlassWareId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("Name")]
        public String Name { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        [Column("Path")]
        public String Path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Spec")]
        public String Spec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Number")]
        public String Number { get; set; }

        [Column("Factory")]
        public String Factory { get; set; }

        [Column("BuyDate")]
        public DateTime? BuyDate { get; set; }

        [Column("DutyPeople")]
        public String DutyPeople { get; set; }

        [Column("DutyPeopleId")]
        public String DutyPeopleId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column("AMOUNT")]
        public Int32 Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CheckDept")]
        public String CheckDept { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CheckDeptId")]
        public String CheckDeptId { get; set; }

        /// <summary>
        /// 检验周期
        /// </summary>
        [Column("Cycle")]
        public String Cycle { get; set; }
        /// <summary>
        /// 标定周期
        /// </summary>
        [Column("BDCycle")]
        public String BDCycle { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        [Column("Validate")]
        public DateTime? Validate { get; set; }
        /// <summary>
        /// 标定有效期
        /// </summary>
        [Column("BDValidate")]
        public DateTime? BDValidate { get; set; }
        [Column("CheckResult")]
        public String CheckResult { get; set; }

        [Column("BDResult")]
        public String BDResult { get; set; }
        /// <summary>
        /// 下次标定提醒
        /// </summary>
        [Column("BDRemind")]
        public String BDRemind { get; set; }
        /// <summary>
        /// 下次检验提醒
        /// </summary>
        [Column("Remind")]
        public String Remind { get; set; }
        [Column("BZID")]
        public String BZID { get; set; }
        [Column("DeptId")]
        public String DeptId { get; set; }
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }
        [Column("CreateUserName")]
        public String CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
