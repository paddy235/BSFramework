using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.MessageData
{
    public class ConfigEntity
    {
        public Guid ConfigId { get; set; }
        public string ConfigKey { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public string Template { get; set; }
        public string Category { get; set; }
        public string RecieveType { get; set; }
    }
}
