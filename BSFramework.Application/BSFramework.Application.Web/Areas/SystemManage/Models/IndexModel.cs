using BSFramework.Application.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.SystemManage.Models
{
    public class IndexModel
    {
        private List<IndexModel> _childNodes;
        private int _checkstate;
        private bool _complete;
        private bool _hasChildren;
        private string _id;
        private bool _isexpand;
        private string _parentnodes;
        private bool _showcheck;
        private string _value;

        public IndexModel()
        {
            _checkstate = 0;
            _complete = true;
            _hasChildren = false;
            _id = "0";
            _isexpand = false;
            _parentnodes = "0";
            _showcheck = false;
            _value = "0";
            _childNodes = new List<IndexModel>();
        }
        public IndexModel(IndexManageEntity item, string parentId = "0", bool showCheckBox = false) : this()
        {
            this.id = item.Id;
            this.text = item.Title;
            this.value = item.Id;
            this.parentnodes = parentId;
            this.showcheck = showCheckBox;
        }

        public List<IndexModel> ChildNodes
        {
            get { return _childNodes; }
            set
            {
                _childNodes = value;
            }
        }
        public int checkstate { get { return _checkstate; } set { _checkstate = value; } }
        public bool complete { get { return _complete; } set { _complete = value; } }
        public bool hasChildren { get { return _hasChildren; } set { _hasChildren = value; } }
        public string id { get { return _id; } set { _id = value; } }
        public string img { get; set; }
        public bool isexpand { get { return _isexpand; } set { _isexpand = value; } }
        public string parentnodes { get { return _parentnodes; } set { _parentnodes = value; } }
        public bool showcheck { get { return _showcheck; } set { _showcheck = value; } }
        public string text { get; set; }
        public string value { get { return _value; } set { _value = value; } }
        public void AddChild(IndexModel menuTreeModel)
        {
            this.hasChildren = true;
            this.isexpand = true;
            this.ChildNodes.Add(menuTreeModel);
        }
    }
}