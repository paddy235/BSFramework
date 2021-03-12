using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.CustomParameterManage
{
    /// <summary>
    /// 自定义列model类
    /// </summary>
    public class CustomColsEntity
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int customsort { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string customlabel { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public string customrequired { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public string custommaxlength { get; set; }
        /// <summary>
        /// 内容值
        /// </summary>
        public string customtext { get; set; }
    }
    /// <summary>
    /// 表单内容实际值实体
    /// </summary>
    public class FormText {
        /// <summary>
        /// 序号
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string[] text{get;set;}
    }
  
}
