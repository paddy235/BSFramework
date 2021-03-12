using BSFramework.Application.Entity.SevenSManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{

    public class SevenSOffice
    {

        public SevenSOfficeEntity adddata { get; set; }
        public SevenSOfficeEntity updatedata { get; set; }
        public string deldata { get; set; }
        public string DelKeys { get; set; }
        public SevenSOfficeAuditEntity audit { get; set; }
        public SevenSOfficeAuditEntity auditupdate { get; set; }
    }

}

public class SevenSTotal
{
    public string deptid { get; set; }
    public string year { get; set; }

}

public class GetTypeDetailSevenS
{
    //   public bool ispush { get; set; }
    public List<SevenSEntity> entity { get; set; }
}
/// <summary>
/// 返回结果实体
/// </summary>
public class RetDataModel
{
    public string code { get; set; }
    public string info { get; set; }
    public int? count { get; set; }
    public dynamic data { get; set; }
}