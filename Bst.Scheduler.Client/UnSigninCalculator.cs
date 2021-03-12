using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Scheduler.Client
{
    public class UnSigninCalculator
    {
        public void CallService(string cycle)
        {
            var api = ConfigurationManager.AppSettings["bzapi"].ToString();
            var client = new HttpClient();
            var response = client.PostAsync($"{api}/DailyTask/CalculateUnSignin?cycle={cycle}", new MultipartContent()).Result;
            if (!response.IsSuccessStatusCode)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
