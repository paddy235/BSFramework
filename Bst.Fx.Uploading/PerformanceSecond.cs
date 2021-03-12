using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{

    public class PerformanceSecond
    {
        public void BaseMessage(string Performance)
        {
            var api = ConfigurationManager.AppSettings["bzapi"].ToString();
            var url = string.Format("{0}/PerformanceSecond/BaseDataInfo?Performance=" + Performance, api);
            var webclient = new WebClientPro();
            var values = new System.Collections.Specialized.NameValueCollection();
            webclient.UploadValues(url, values);
        }
    }
}
