using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 安全日活动-改进行动
    /// </summary>
    public interface IActivityActionService
    {
        List<ActivityActionEntity> GetListByActionId(string activityId,string deptId);
        void Insert(ActivityActionEntity data);
        void Update(ActivityActionEntity actionEntity, List<FileInfoEntity> files, string[] delIds);
        void Del(string keyValue);
    }
}
