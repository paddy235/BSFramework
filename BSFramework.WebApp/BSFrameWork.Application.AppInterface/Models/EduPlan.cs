using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.EducationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class EduPlan
    {

    }

    public class AppListModel
    {
        public string userId { get; set; }
        public bool allowPaging { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public Param data { get; set; }
    }
    public class AppDetailModel
    {
        public string userId { get; set; }

        public string type { get; set; }
        public string Id { get; set; }
        public evadata data { get; set; }
    }
    public class evadata
    {
        public string Score { get; set; }
        public string EvaluateContent { get; set; }
    }
    public class Param
    {
        public string from { get; set; }

        public string to { get; set; }

        public string eduType { get; set; }

        public string bzId { get; set; }

        public string appraise { get; set; }
    }
    public class ListModel
    {
        public string userId { get; set; }
        public PlanInfoListModel data { get; set; }
    }
    public class NewPlanModel
    {
        public string userId { get; set; }
        public NewPlanModelEntity data { get; set; }
        
        }

    public class NewPlanModelEntity {
        public string DelKeys { get; set; }

        public EduPlanInfoEntity entity { get; set; }

    }
    public class PlanInfoModel
    {
        public string userId { get; set; }
        public EduPlanInfoEntity data { get; set; }
    }
    public class VerifyModel
    {
        public string userId { get; set; }
        public EduPlanVerifyEntity data { get; set; }
    }



    public class DelModel
    {
        public string userId { get; set; }

        public DelModelNew data { get; set; }
    }

    public class TrainingModel
    {
        public string userId { get; set; }
        public TrainingListModel data { get; set; }
    }
    public class TrainingListModel
    {
        public int month { get; set; }
        public int year { get; set; }
    }
    public class DelModelNew
    {
        public string Id { get; set; }
    }
    public class PlanInfoListModel
    {

        public string planId { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string eduType { get; set; }
        public string trainDate { get; set; }
        public string name { get; set; }
    }
}