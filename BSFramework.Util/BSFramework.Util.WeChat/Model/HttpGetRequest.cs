using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Util.WeChat.Helper;

namespace BSFramework.Util.WeChat.Model
{
    public class HttpGetRequest : IHttpSend
    {
        public string Send(string url, string data)
        {
            return new HttpHelper().Get(url, Encoding.UTF8);
        }
    }
}
