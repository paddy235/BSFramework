using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    [Table("wg_drugstockout")]
    public class DrugStockOutEntity : BaseEntity
    {

        [Column("Id")]
        public string Id { get; set; }

        [Column("DrugInventoryId")]
        public string DrugInventoryId { get; set; }
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
        /// 药品等级
        /// </summary>
        [Column("DrugLevel")]
        public string DrugLevel { get; set; }
        [Column("DrugLevelName")]
        public string DrugLevelName { get; set; }
        /// <summary>
        /// 药品单位
        /// </summary>
        [Column("DrugUnit")]
        public string DrugUnit { get; set; }

        /// <summary>
        /// 登记人
        /// </summary>
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("Total")]
        public decimal Total { get; set; }

        [Column("OutTotal")]
        public decimal OutTotal { get; set; }
        [Column("Warn")]
        public decimal Warn { get; set; }

        [Column("BZId")]
        public string BZId { get; set; }
        [NotMapped]
        public string DrugType { get; set; }
        public string Monitor { get; set; }

        public override void Create()
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }
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
