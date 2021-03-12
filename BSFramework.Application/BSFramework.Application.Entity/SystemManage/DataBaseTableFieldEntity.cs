
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：数据库表字段
    /// </summary>
    public class DataBaseTableFieldEntity
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [Column("COLUMN")]
        public string column_name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
          [Column("DATATYPE")]
        public string datatype { get; set; }
        /// <summary>
        /// 数据长度
        /// </summary>
         [Column("LENGTH")]
          public int? length_data { get; set; }
        /// <summary>
        /// 允许空
        /// </summary>
         [Column("ISNULLABLE")]
        public string isnullable { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
         [Column("IDENTITY")]
         public string identity_data { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
           [Column("KEY")]
        public string key { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
          [Column("DEFAULTS")]
        public string defaults { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
         [Column("REMARK")]
        public string remark { get; set; }
    }
}
