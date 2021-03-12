using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.BaseManage.Models
{
    public class TreeModel
    {
        public TreeModel(bool showcheckbox =false)
        {
            showcheck = showcheckbox;
        }
        public string id { get; set; }
        public string value { get; set; }
        public string text { get; set; }
        public bool hasChildren { get; set; }
        public bool isexpand { get; set; }
        public bool complete { get; set; }
        public bool showcheck { get; set; }
        public string Code { get; set; }
        public List<TreeModel> ChildNodes { get; set; }
    }
}