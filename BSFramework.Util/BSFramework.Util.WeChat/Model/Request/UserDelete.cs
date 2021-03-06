using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Util.WeChat.Model.Request
{
    public class UserDelete : OperationRequestBase<OperationResultsBase,HttpPostRequest>
    {
        private string url = "https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token=ACCESS_TOKEN&userid={0}";
        protected override string Url()
        {
            return string.Format(url, userid);
        }

        /// <summary>
        /// 员工UserID。对应管理端的帐号
        /// </summary>
        /// <returns></returns>
        [IsNotNull]
        public string userid { get; set; }
    }
}
