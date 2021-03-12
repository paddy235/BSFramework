using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    /// <summary>
    /// 药品信息表
    /// </summary>
    [Table("wg_drugmanage")]
    public class DrugEntity : BaseEntity
    {
        [Column("Id")]
        public string Id { get; set; }

        [Column("DrugInventoryId")]
        public string DrugInventoryId { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        [Column("DrugName")]
        public string DrugName { get; set; }
        /// <summary>
        /// 药品等级
        /// </summary>
        [Column("DrugLevel")]
        public string DrugLevel { get; set; }
        [Column("DrugLevelName")]
        public string DrugLevelName { get; set; }
        /// <summary>
        /// 药品单位
        /// </summary>
        [Column("Unit")]
        public string Unit { get; set; }
        public string Unit2 { get; set; }
        /// <summary>
        /// 药品总量 (库存总量（不变）)
        /// </summary>
        [Column("Total")]
        public Decimal? Total { get; set; }
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 库存余量（随入库出库变动）
        /// </summary>
        [Column("Surplus")]
        public Decimal? Surplus { get; set; }
        [Column("BZID")]
        public String BZId { get; set; }

        /// <summary>
        /// 出库余量
        /// </summary>
        [Column("OutSurplus")]
        public Decimal OutSurplus { get; set; }
        /// <summary>
        /// 出库预警
        /// </summary>
        [Column("Warn")]
        public string Warn { get; set; }

        /// <summary>
        /// 库存预警
        /// </summary>
        [Column("StockWarn")]
        public string StockWarn { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [Column("Spec")]
        public string Spec { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column("State")]
        public string State { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [Column("Location")]
        public string Location { get; set; }

        /// <summary>
        /// 库存数量(瓶)
        /// </summary>
        [Column("DrugNum")]
        public float DrugNum { get; set; }

        /// <summary>
        /// 出库数量
        /// </summary>
        [Column("OutNum")]
        public int OutNum { get; set; }
        [NotMapped]
        public List<newDrug> Specs { get; set; }



        [NotMapped]
        public string Less { get; set; }
        [NotMapped]
        public string Monitor { get; set; }
        public float Used { get; set; }

        public override void Create()
        {
        }
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
        }
    }
    public class newDrug
    {
        public string Id { get; set; }
        public string Spec { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public float DrugNum { get; set; }
        public string Unit { get; set; }
        public string Unit2 { get; set; }
        public float Used { get; set; }
    }
}
