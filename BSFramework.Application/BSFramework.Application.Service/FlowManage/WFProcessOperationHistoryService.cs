using BSFramework.Application.Entity.FlowManage;
using BSFramework.Application.IService.FlowManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例操作记录表操作
    /// </summary>
    public class WFProcessOperationHistoryService : RepositoryFactory, WFProcessOperationHistoryIService
    {
        #region 获取数据


        #endregion

        #region 提交数据
        /// <summary>
        /// 保存或更新实体对象
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveEntity(string keyValue,WFProcessOperationHistoryEntity entity)
        {
            try
            {
                if(string.IsNullOrEmpty(keyValue))
                {
                    entity.Create();
                    this.BaseRepository().Insert(entity);
                }
                else
                {
                    entity.Modify(keyValue);
                    this.BaseRepository().Update(entity);
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
