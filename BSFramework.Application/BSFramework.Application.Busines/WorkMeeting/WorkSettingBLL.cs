using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Cache.Factory;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class WorkSettingBLL
    {
        private IWorkSettingService service = new WorkSettingService();
        /// <summary>
        /// 初始化添加班次
        /// </summary>
        /// <param name="entityList"></param>
        public void saveForm(List<WorkSettingEntity> entityList)
        {
            service.saveForm(entityList);
        }


        /// <summary>
        /// 获取班次列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkSettingEntity> GetList( string depId) {
           return service.GetList(depId);

        }
        /// <summary>
        /// 获取班次
        /// </summary>
        /// <returns></returns>
        public WorkSettingEntity getEntity(string keyvalue)
        {
            return service.getEntity(keyvalue);

        }
        /// <summary>
        /// 获取班次
        /// </summary>
        /// <returns></returns>
        public WorkSettingEntity getEntitybySetUp(string keyvalue,string createuserid)
        {
            return service.getEntitybySetUp(keyvalue,  createuserid);

        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        public bool RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
                //CacheFactory.Cache().RemoveCache(cacheKey);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
