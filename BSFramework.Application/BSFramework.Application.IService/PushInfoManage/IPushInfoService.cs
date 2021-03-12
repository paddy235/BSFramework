using BSFramework.Application.Entity.PushInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PushInfoManage
{
   public interface IPushInfoService
    {
        /// <summary>
        /// 阅读
        /// </summary>
        /// <param name="model"></param>
        void PushRead(PushPersonEntity model);
        /// <summary>
        /// 推送保存推送数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelList"></param>
        void SavePushInfo(PushInfoEntity model, List<PushPersonEntity> modelList);
        /// <summary>
        /// 根据推送表id获取阅读人数
        /// </summary>
        /// <param name="pushid"></param>
        /// <returns></returns>
        IEnumerable<PushPersonEntity> GetPushPerson(string pushid);
        /// <summary>
        /// 根据用户获取推送的内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<PushInfoEntity> GetPushInfoList(string userId, string pushid);
    }
}
