using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models.QueryModels
{
    public class MeetingQuery
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public string  Id { get; set; }
    }
}