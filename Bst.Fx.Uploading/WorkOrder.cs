using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{
   public class WorkOrder
    {

        public void Produce(string ProduceType)
        {
            var api = ConfigurationManager.AppSettings["bzapi"].ToString();
            var url = string.Format("{0}/WorkPlan/StartProduce?ProduceType=" + ProduceType, api);
            var webclient = new WebClientPro();
            var values = new System.Collections.Specialized.NameValueCollection();
            webclient.UploadValues(url, values);
        }
    }
}
