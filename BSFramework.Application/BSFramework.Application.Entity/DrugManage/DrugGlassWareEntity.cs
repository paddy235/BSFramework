using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    /// <summary>
    /// 玻璃器皿信息表
    /// </summary>
    [Table("wg_drugglassware")]
    public class DrugGlassWareEntity : BaseEntity
    {
        [Column("GlassWareId")]
        public string GlassWareId { get; set; }

        [Column("GlassWareType")]
        public string GlassWareType { get; set; }

        [Column("GlassWareName")]
        public string GlassWareName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Img")]
        public string Img { get; set; }

        [Column("BGImg")]
        public string BGImg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("OperateVideo")]
        public string OperateVideo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("UseWay")]
        public string UseWay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Attention")]
        public string Attention { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYUSERID")]
        public string MODIFYUSERID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYUSERNAME")]
        public string MODIFYUSERNAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYDATE")]
        public DateTime? MODIFYDATE { get; set; }
    }
}
