using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    /// <summary>
    /// 药品入库信息表
    /// </summary>
    [Table("wg_drugstock")]
    public class DrugStockEntity : BaseEntity
    {
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 药品Id
        /// </summary>
        [Column("DrugId")]
        public string DrugId { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        [Column("DrugName")]
        public string DrugName { get; set; }
        /// <summary>
        /// 药品数量（瓶）
        /// </summary>
        [Column("DrugNum")]
        public float DrugNum { get; set; }
        /// <summary>
        /// 药品等级
        /// </summary>
        [Column("DrugLevel")]
        public string DrugLevel { get; set; }
        /// <summary>
        /// 药品单位
        /// </summary>
        [Column("DrugUnit")]
        public string DrugUnit { get; set; }
        /// <summary>
        /// 药品规格
        /// </summary>
        [Column("DrugUSL")]
        public decimal DrugUSL { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 出/入库
        /// </summary>
        [Column("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 库存余量
        /// </summary>
        [Column("StockNum")]
        public float StockNum { get; set; }
        [Column("BZId")]
        public string BZId { get; set; }

        [Column("BZName")]
        [NotMapped]
        public string BZName { get; set; }
        public string Monitor { get; set; }

        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
        }
    }
}
