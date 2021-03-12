using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BSFramework.Application.Code
{
    /// <summary>
    /// 描 述：系统信息
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// 当前Tab页面模块Id
        /// </summary>
        public static string CurrentModuleId
        {
            get
            {
                return WebHelper.GetCookie("currentmoduleId");
            }
        }
        /// <summary>
        /// 当前Tab页面模块Name
        /// </summary>
        public static string CurrentModuleName
        {
            get
            {
                return HttpUtility.UrlDecode(WebHelper.GetCookie("currentmoduleName"));
            }
        }
        /// <summary>
        /// 当前登录用户Id
        /// </summary>
        public static string CurrentUserId
        {
            get
            {
                return OperatorProvider.Provider.Current().UserId;
            }
        }
    }
}
