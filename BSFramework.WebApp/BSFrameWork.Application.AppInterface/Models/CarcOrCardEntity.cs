using BSFramework.Application.Entity.CarcOrCardManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{


    public class getCarcOrCardModel
    {
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string deptid { get; set; }
        public string workname { get; set; }
        public int state { get; set; }
        public string type { get; set; }
        public string my { get; set; }

    }

    /// <summary>
    /// 上传自定义台
    /// </summary>
    public class CarcOrCardModel
    {
        public CarcEntity entity { get; set; }
        public string DelKeys { get; set; }
    }
}