using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Models
{
    public class EvaluateModel
    {
        public string EvaluateId { get; set; }
        public IList<EvaluateItemModel> EvaluateItems { get; set; }

    }
}