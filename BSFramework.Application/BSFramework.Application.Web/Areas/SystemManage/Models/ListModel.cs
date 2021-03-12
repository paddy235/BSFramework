using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.SystemManage.Models
{
    public class ListModel<TModel>
    {
        public List<TModel> Data { get; set; }
        public int Total { get; set; }
    }
}