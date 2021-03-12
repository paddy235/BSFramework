using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.PublicInfoManage.ViewMode
{
    public class KeyValue
    {
        public KeyValue()
        {

        }

        public KeyValue(int dataType)
        {
            this.dataType = dataType;
        }
        public string key { get; set; }
        public string value { get; set; }
        /// <summary>
        /// 0班组  1 双控
        /// </summary>
        public int dataType { get; set; }

    }
}
