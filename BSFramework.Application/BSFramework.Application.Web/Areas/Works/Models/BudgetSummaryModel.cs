using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class BudgetSummaryModel
    {
        public string Category { get; set; }
        public decimal[] Data { get; set; }
    }
}