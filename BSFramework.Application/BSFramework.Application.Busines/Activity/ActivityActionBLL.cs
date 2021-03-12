using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    /// <summary>
    /// 安全日活动-改进行动
    /// </summary>
    public class ActivityActionBLL
    {
        /// <summary>
        /// service
        /// </summary>
        public IActivityActionService _service;
        public ActivityActionBLL()
        {
            _service = new ActivityActionService();
        }

        /// <summary>
        /// 根据安全日活动的Id获取改进行动，包含上次落实情况
        /// </summary>
        /// <param name="activityId">安全日活动的Id</param>
        /// <returns></returns>
        public List<ActivityActionEntity> GetListByActionId(string activityId,string deptId)
        {
            return _service.GetListByActionId(activityId,deptId);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data"></param>
        public void Insert(ActivityActionEntity data)
        {
            _service.Insert(data);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="actionEntity">编辑后的实体</param>
        /// <param name="files">新增的附件</param>
        /// <param name="delIds">要删除的附件</param>
        public void Update(ActivityActionEntity actionEntity, List<FileInfoEntity> files, string[] delIds)
        {
            _service.Update(actionEntity, files, delIds);
        }

        public void Del(string keyValue)
        {
             _service.Del(keyValue);
        }
    }
}
