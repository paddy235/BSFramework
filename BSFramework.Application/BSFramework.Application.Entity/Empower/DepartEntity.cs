using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.Empower
{
    [Table("wg_depart")]
    public class DepartEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Column("Name")]
        public String Name { get; set; }

        /// <summary>
        /// 注册码
        /// </summary>
        [Column("RegisterCode")]
        public String RegisterCode { get; set; }


        /// <summary>
        /// 安卓版本
        /// </summary>
        [Column("AndroidVersion")]
        public String AndroidVersion { get; set; }

        /// <summary>
        /// 授权日期
        /// </summary>
        [Column("EmpowerDate")]
        public DateTime EmpowerDate { get; set; }

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
        /// 类型id
        /// </summary>
        [Column("TypeId")]
        public String TypeId { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [Column("TypeName")]
        public String TypeName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("Sort")]
        public String Sort { get; set; }
    }
}
