using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    /// <summary>
    /// 药品取出信息表
    /// </summary>
    [Table("wg_drugout")]
    public class DrugOutEntity : BaseEntity
    {

        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        [Column("DrugName")]
        public string DrugName { get; set; }
        [Column("DrugId")]
        public string DrugId { get; set; }
        [Column("DrugLevel")]
        public string DrugLevel { get; set; }
        /// <summary>
        /// 取用量
        /// </summary>
        [Column("OutNum")]
        public decimal OutNum { get; set; }
        [Column("DrugUnit")]
        public string DrugUnit { get; set; }
        /// <summary>
        /// 取用人
        /// </summary>
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 剩余量
        /// </summary>
        [Column("Surplus")]
        public decimal? Surplus { get; set; }
        /// <summary>
        /// 监护人
        /// </summary>
        [Column("GuarDianId")]
        public string GuarDianId { get; set; }
        [Column("GuarDianName")]
        public string GuarDianName { get; set; }
        [Column("BZId")]
        public string BZId { get; set; }
        [Column("BZName")]
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
