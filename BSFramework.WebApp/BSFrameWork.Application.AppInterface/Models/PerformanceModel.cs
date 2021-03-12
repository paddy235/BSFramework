using BSFramework.Application.Entity.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{

    public class BaseDataModel
    {
        public string userId { get; set; }
        public bool allowPaging { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class BaseDataModel<T> : BaseDataModel where T : class
    {
        public T data { get; set; }
    }

    #region 绩效管理
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceSetUpModel
    {
        public List<PerformancesetupEntity> add { get; set; }
        public List<PerformancesetupEntity> del { get; set; }
        public List<PerformancesetupEntity> Listupdate { get; set; }
        public string departmentid { get; set; }
        public string time { get; set; }


    }

    public class PerformanceSetUpSecondModel
    {
        public List<PerformancesetupSecondEntity> add { get; set; }
        public List<PerformancesetupSecondEntity> del { get; set; }
        public List<PerformancesetupSecondEntity> Listupdate { get; set; }
        public PerformancePersonSecondEntity person { get; set; }
        public string departmentid { get; set; }
        public string time { get; set; }


    }

    public class PerformanceModel
    {
        public string departmentid { get; set; }
        public string time { get; set; }

    }
    public class PerformanceSecondModel
    {
        public string departmentid { get; set; }
        public DateTime time { get; set; }

    }
    #endregion




}