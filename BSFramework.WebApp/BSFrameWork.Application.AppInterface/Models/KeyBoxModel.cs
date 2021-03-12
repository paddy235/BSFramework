using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class getKeyBoxCategoryDataModel
    {
        public string DeptId { get; set; }
        public string Category { get; set; }
    }

    public class GetPageKeyBoxListModel
    {
        public string DeptId { get; set; }
        public string Category { get; set; }

        public string State { get; set; }

        public string keyWord { get; set; }

    }



    public class GetPageKeyUseListByStateModel
    {
        public string DeptId { get; set; }
        public string Category { get; set; }
    }

    public class GetPageKeyUseListModel
    {
        public string DeptId { get; set; }
        public string Category { get; set; }

        public string KeyId { get; set; }
        public string keyWord { get; set; }
        public string CreateDate { get; set; }
        public string state { get; set; }
        public string userid { get; set; }
        public string districtId { get; set; }

        /// <summary>
        /// 1 今天 2本周 3本月
        /// </summary>
        public string TimeType { get; set; }
    }


    public class GetOnLocale
    {

        public string DistrictId { get; set; }
        public string DistrictCode { get; set; }
        /// <summary>
        /// 1 今天 2本周 3本月
        /// </summary>
        public string TimeType { get; set; }

    }


    public class GetPageSweepListModel
    {
        public string keyWord { get; set; }
        public string StartData { get; set; }

        public string EndData { get; set; }

        public string DistrictId { get; set; }

        public string DistrictCode { get; set; }
        public string State { get; set; }

        public string UserId { get; set; }

    }






}