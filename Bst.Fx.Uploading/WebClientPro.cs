using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{
    public class WebClientPro : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var timeout = int.Parse((System.Configuration.ConfigurationManager.AppSettings["timeout"] ?? "60")) * 1000;
            var request = base.GetWebRequest(address);
            request.Timeout = timeout;
            return request;
        }
    }

}
