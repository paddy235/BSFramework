using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.Empower
{
    [Table("wg_model")]
    public class ModelEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column("Name")]
        public String Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        /// <summary>
        /// 父节点id
        /// </summary>
        [Column("ParentId")]
        public String ParentId { get; set; }

        /// <summary>
        /// 父节点名称
        /// </summary>
        [Column("ParentName")]
        public String ParentName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("Sort")]
        public String Sort { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Column("Code")]
        public String Code { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("Remark")]
        public String Remark { get; set; }

        /// <summary>
        /// 节点等级 1 2
        /// </summary>
        [Column("NodeLevel")]
        public String NodeLevel { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [Column("DeleteMark")]
        public String DeleteMark { get; set; }
    }
}
