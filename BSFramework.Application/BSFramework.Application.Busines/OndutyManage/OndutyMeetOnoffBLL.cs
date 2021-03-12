using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.IService.OndutyManage;
using BSFramework.Application.Service.OndutyManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.OndutyManage
{
  public   class OndutyMeetOnoffBLL
    {
        private IOndutyMeetOnoffService service = new OndutyMeetOnoffService();
        /// <summary>
        /// 修改开关
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="onoff"></param>
        public void OnOff(string keyvalue, string onoff)
        {

            service.OnOff(keyvalue,onoff);
        }
        /// <summary>
        /// 获取开关数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public OndutyMeetOnoffEntity getCk(string keyvalue)
        {
            return service.getCk(keyvalue);
        }
    }
}
