using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Uploading
{
    public class SevenS
    {

        public void sendMessage(string PictureType)
        {
            var api = ConfigurationManager.AppSettings["bzapi"].ToString();
            var url = string.Format("{0}/SevenS/sendMessage?PictureType=" + PictureType, api);
            var webclient = new WebClientPro();
            var values = new System.Collections.Specialized.NameValueCollection();
            webclient.UploadValues(url, values);
        }

        public void StartPicture(string PictureType)
        {
            var api = ConfigurationManager.AppSettings["bzapi"].ToString();
            var url = string.Format("{0}/SevenS/StartPicture?PictureType=" + PictureType, api);
            var webclient = new WebClientPro();
            var values = new System.Collections.Specialized.NameValueCollection();
            webclient.UploadValues(url, values);
        }
    }
}
