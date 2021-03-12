using BSFramework.Application.Entity.CustomParameterManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// 上传自定义台
    /// </summary>
    public class CustomParameterModel
    {
        public CustomParameterEntity entity { get; set; }
        public string DelKeys { get; set; }
    }

    public class getCustomParameterModel
    {
        public string from { get; set; }

        public string to { get; set; }
        public string deptid { get; set; }
        public string ctid { get; set; }
    }

}