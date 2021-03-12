using BSFramework.Application.Entity.PushInfoManage;
using BSFramework.Application.IService.PushInfoManage;
using BSFramework.Application.Service.PushInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.PushInfoManage
{
    public class PushInfoBLL
    {

        IPushInfoService service = new PushInfoService();

        /// <summary>
        /// 根据用户获取推送的内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<PushInfoEntity> GetPushInfoList(string userId, string pushid)
        {
            return service.GetPushInfoList(userId,pushid);
        }
        /// <summary>
        /// 根据推送表id获取阅读人数
        /// </summary>
        /// <param name="pushid"></param>
        /// <returns></returns>
        public IEnumerable<PushPersonEntity> GetPushPerson(string pushid)
        {
            return service.GetPushPerson(pushid);
        }

        /// <summary>
        /// 推送保存推送数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelList"></param>
        public void SavePushInfo(PushInfoEntity model, List<PushPersonEntity> modelList)
        {
            service.SavePushInfo(model, modelList);
        }

        /// <summary>
        /// 阅读
        /// </summary>
        /// <param name="model"></param>
        public void PushRead(PushPersonEntity model)
        {
            service.PushRead(model);
        }
    }
}
