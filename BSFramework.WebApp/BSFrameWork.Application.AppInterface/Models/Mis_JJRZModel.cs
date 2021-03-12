using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// MIS值班日志交接表查询参数实体
    /// </summary>
    public class Mis_JJRZModel
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string FL { get; set; }
        /// <summary>
        /// 班值
        /// </summary>
        public string BZMC { get; set; }

        /// <summary>
        /// 关键字(运行日志内容关键字)
        /// </summary>
        public string KEYWORD { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string STARTDATE { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string ENDDATE { get; set; }


    }
}