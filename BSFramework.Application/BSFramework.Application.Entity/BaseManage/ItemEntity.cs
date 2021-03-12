using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.BaseManage
{
    public class ItemEntity
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string ItemCode { get; set; }
        public string ParentItemId { get; set; }
    }
}
