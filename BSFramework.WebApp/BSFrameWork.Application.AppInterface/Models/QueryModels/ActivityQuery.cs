using BSFramework.Application.Entity.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models.QueryModels
{
    public class ActivityQuery
    {
 
    }

    /// <summary>
    /// 改进行动
    /// </summary>
    public class ActivityActionQuery
    {
        public string DelKeys { get; set; }
        public ActivityActionEntity ActionEntity { get; set; }
    }
}