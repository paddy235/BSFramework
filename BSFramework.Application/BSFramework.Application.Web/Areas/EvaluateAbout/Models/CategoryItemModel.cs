using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Models
{
    public class CategoryItemModel
    {
        public string ItemId { get; set; }
        public string ItemContent { get; set; }
        public string ItemStandard { get; set; }
        public int Score { get; set; }
        public string EvaluateDept { get; set; }
        public string UseDept { get; set; }

        public string UseDeptId { get; set; }
        public CategoryModel Category { get; set; }
    }
}