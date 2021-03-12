using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Util
{
    public class WMP
    {
        /// <summary>
        /// 利用WMPLib获取音频文件时长;
        /// 如果添加引用编译提示"不能引用嵌入式接口之类的错误",把引用属性里的嵌入式***,设为false;
        /// asp.net 环境下测试未通过,c#未测试;
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDurationByWMPLib(string filePath)
        {
            try
            {
                return string.Empty;
                //WMPLib.WindowsMediaPlayerClass wmp = new WMPLib.WindowsMediaPlayerClass();
                //WMPLib.IWMPMedia media = wmp.newMedia(@filePath);
                //return media.durationString;
                ////WMPLib.WindowsMediaPlayerClass player = new WMPLib.WindowsMediaPlayerClass();
                //player.URL = filePath;
                //var sss = player.duration;
                //var duration = Math.Ceiling(player.duration);
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }
}
