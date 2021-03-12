using BSFramework.Application.Entity.OndutyManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.OndutyManage
{
  public  interface IOndutyMeetOnoffService
    {
        /// <summary>
        /// 修改开关
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="onoff"></param>
        void OnOff(string keyvalue, string onoff);
        OndutyMeetOnoffEntity getCk(string keyvalue);
        
        }
}
