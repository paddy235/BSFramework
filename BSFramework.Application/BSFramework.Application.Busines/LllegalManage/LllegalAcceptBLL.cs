using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.IService.LllegalManage;
using BSFramework.Application.Service.LllegalManage;

namespace BSFramework.Application.Busines.LllegalManage
{
    public class LllegalAcceptBLL
    {
        IlllegalAcceptService refromService = new LllegalAcceptService();
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键Id</param>
        /// <returns></returns>
        public LllegalAcceptEntity GetEntity(string keyValue)
        {
            return refromService.GetEntity(keyValue);
        }


        public void SaveFrom(LllegalAcceptEntity refrom)
        {
            refromService.SaveFrom(refrom);
        }

        public LllegalAcceptEntity GetEntityByLllegalId(string id)
        {
            return refromService.GetEntityByLllegalId(id);
        }
    }
}
