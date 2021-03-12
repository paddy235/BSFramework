using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 终端指标表
    /// </summary>
    [Table("BASE_TERMINALDATASET")]
   public class TerminalDataSetEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        public string Id { get; set; }

        /// <summary>
        /// 指标名称
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [Column("UNINT")]
        public string Unint { get; set; }
        /// <summary>
        /// 是否启用 0禁用  1启用
        /// </summary>
        [Column("ISOPEN")]
        public int IsOpen { get; set; }

        /// <summary>
        /// 排序字段，越小越靠前
        /// </summary>
        [Column("SORT")]
        public int? Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Column("CODE")]
        public string Code { get; set; }

        /// <summary>
        /// 是否是班组指标 0 不是  1是
        /// </summary>
        [Column("BK1")]
        public string IsBZ { get; set; }

        /// <summary>
        /// 绑定的功能模块的Id
        /// </summary>
        [Column("BK2")]
        public string BindModuleId { get; set; }

        /// <summary>
        /// 自定义编码（双控指标需配置）
        /// </summary>
        [Column("BK3")]
        public string CustomCode { get; set; }

        /// <summary>
        /// 指标分类 
        /// 0 平台指标 
        /// 1 是安卓终端指标  
        /// 2 app指标
        /// </summary>
        [Column("BK4")]
        public string DataSetType { get; set; }
        /// <summary>
        /// 指标图标
        /// </summary>
        [Column("ICONURL")]
        public string IconUrl { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        [Column("ADDRESS")]
        public string  Address { get; set; }

        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
        }

        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            ModifyDate = DateTime.Now;
        }

    }
}
