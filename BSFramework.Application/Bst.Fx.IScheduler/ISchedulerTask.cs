using Bst.Fx.SchedulerEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.IScheduler
{
    public interface ISchedulerTask
    {
        /// <summary>
        /// 获取所有执行计划
        /// </summary>
        /// <returns></returns>
        List<SchedulerTaskEntity> GetTasks();

        SchedulerTaskEntity NewTask(SchedulerTaskEntity entity);

        void Execute(string task);
        void Enable(SchedulerTaskEntity task);
        void Disable(SchedulerTaskEntity task);
        SchedulerTaskEntity GetDetail(string id);
        void Edit(SchedulerTaskEntity model);
    }
}
