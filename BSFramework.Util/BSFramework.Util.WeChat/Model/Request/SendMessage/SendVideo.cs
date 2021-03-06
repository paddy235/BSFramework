using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Util.WeChat.Model.Request.SendMessage
{
    class SendVideo : MessageSend
    {
        public override string msgtype
        {
            get { return "video"; }
        }

        [IsNotNull]
        public SendVideo.SendItem video { get; set; }

        public class SendItem
        {
            /// <summary>
            /// 媒体资源文件ID
            /// </summary>
            /// <returns></returns>
            public string media_id { get; set; }

            /// <summary>
            /// 视频消息的标题
            /// </summary>
            /// <returns></returns>
            public string title { get; set; }

            /// <summary>
            /// 视频消息的描述
            /// </summary>
            /// <returns></returns>
            public string description { get; set; }
        }
    }
}
