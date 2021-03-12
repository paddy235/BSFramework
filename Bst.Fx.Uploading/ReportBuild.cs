using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{
    public class ReportBuild
    {
        public void Build(string reporttype)
        {
            var api = ConfigurationManager.AppSettings["bzapi"].ToString();
            var url = string.Format("{0}/Report/Build", api);
            var webclient = new WebClientPro();
            var values = new System.Collections.Specialized.NameValueCollection();
            values.Add("ReportType", reporttype);
            webclient.UploadValues(url, values);
        }
    }
}
