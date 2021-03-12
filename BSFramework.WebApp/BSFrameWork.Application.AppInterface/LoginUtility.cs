using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace BSFrameWork.Application.AppInterface
{
    public class LoginUtility
    {
        public bool ValidRemote(string username, string password)
        {
            var dict = new Dictionary<string, string>() { { "account", username }, { "password", password } };
            var baseUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();
            var response = client.PostAsync($"{baseUrl}Directory/checkUser", new FormUrlEncodedContent(dict)).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"远程验证失败\r\n{content}");
                return false;
            }

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResult>(content);
            if (json.Code == 0) return true;

            return false;
        }
    }

    public class LoginResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}