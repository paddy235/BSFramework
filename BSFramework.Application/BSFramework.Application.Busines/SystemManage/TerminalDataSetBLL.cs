using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SystemManage
{
   public class TerminalDataSetBLL
    {
        private ITerminalDataSetService service = new TerminalDataSetService();

        public List<TerminalDataSetEntity> GetPageList(Pagination pagination, string queryJson, string DataSetType)
        {
            return service.GetPageList(pagination, queryJson, DataSetType);
        }

        public void SaveForm(string keyValue, TerminalDataSetEntity ds)
        {
            service.SaveForm(keyValue, ds);
        }

        public TerminalDataSetEntity GetEntity(string keyValue)
        {
         return   service.GetEntity(keyValue);
        }

        public void RemoveForm(string keyValue)
        {
            service.RemoveForm(keyValue);
        }

        public List<TerminalDataSetEntity> GetList()
        {
            return service.GetList();
        }
        /// <summary>
        /// 获取双控授权的模块
        /// </summary>
        /// <param name="platform">平台类型 0 windows 1 安卓终端 2手机APP</param>
        /// <returns></returns>
        public List<MenuConfigEntity> GetMenuConfigList(int platform)
        {
            try
            {
                string res = HttpMethods.HttpGet(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "MenuConfig", "GetMenuList") + "?" + string.Format("platform={0}", platform));
                MenuRespone dy = JsonConvert.DeserializeObject<MenuRespone>(res);
                if (dy.Code == 0)
                {
                    return dy.Data;
                }
                else
                {
                    return new List<MenuConfigEntity>();
                }
            }
            catch (Exception ex)
            {
                return new List<MenuConfigEntity>();
            }
       
        }
        /// <summary>
        /// 获取手机app所有的授权的模块
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="platform">平台类型 0-window终端 1-Android终端 2-手机APP</param>
        /// <param name="themetype">第几套工作栏。默认第一套（0），因为安卓终端与app都是取的说有的模块</param>
        /// <returns></returns>
        public string GetAuthMenuConfigList(string userId, int platform,int themetype=0)
        {
            //正式
            string Value = "{ \"userid\":\"" + userId + "\", \"data\":{ \"themetype\":" + themetype + ",\"platform\":" + platform + "} }";
            //测试
            //string Value = "{ userid:\"9098221b-6bda-41bd-84d1-79c31877f6fb\", data:{ themetype:0,PaltformType:0} }";
            string url = Path.Combine(Config.GetValue("ErchtmsApiUrl"), "MenuConfig", "GetMenuList");
            var para = "json=" +  Uri.EscapeDataString(Value);
         
            string responseStr = HttpMethods.HttpPost(url, para);
            return responseStr;
        }
    }
}
