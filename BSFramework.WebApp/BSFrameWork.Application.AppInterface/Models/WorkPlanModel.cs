using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.WorkPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class WorkPlanModel
    {
    }

    public class WorkPlanListModel
    {
        public string userId { get; set; }
        public PlanListModel data { get; set; }
    }
    public class PlanListModel
    {
        public string PlanType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class WorkPlanContentListModel
    {
        public string userId { get; set; }
        public ContentListModel data { get; set; }
    }
    public class ContentListModel
    {
        public string PlanType { get; set; }
    }

    public class PlanContentModel {
        public string userId { get; set; }

        public WorkPlanContentEntity data { get; set; }
    }
     public class ContentModel {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ParentId { get; set; }
        public string Remark { get; set; }

        public string WorkPeopleId { get; set; }
        public string WorkPeopleName { get; set; }
        public string IsFinished { get; set; }
        public List<PlanContentModel> ChildrenContent {get;set;}
    }

     public class DutyTypeModel {
         public string userId { get; set; }

         public PeopleDutyTypeEntity data { get; set; }
     }
}