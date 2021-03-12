using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.ProducingCheck.Models
{
    public class CategoryModel
    {
        public string CategoryId { get; set; }
        public string Category { get; set; }
        public CategoryModel ParentCategory { get; set; }
    }
}