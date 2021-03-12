using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{
    public class JsonMessage
    {
        public string msg { get; set; }
        public string status { get; set; }
        public MessageData result { get; set; }
    }

    public class MessageData
    {
        public string fid { get; set; }
        public string token { get; set; }
        public int offset { get; set; }
        public string url { get; set; }
    }
}
