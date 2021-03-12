using BSFramework.Application.Entity.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 描 述：安全预知训练
    /// </summary>
    public interface MeasuresIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<MeasuresEntity> GetList(string queryJson);
          /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        DataTable GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 根据危险预知训练Id获取数据列表
        /// </summary>
        /// <param name="dangerId">关联危险预知训练记录Id</param>
        /// <returns></returns>
        List<MeasuresEntity> GetMeasureList(string dangerId);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        MeasuresEntity GetEntity(string keyValue);

        DataTable GetMeasuresById(Pagination pagination);
        List<MeasuresEntity> GetMeasuresByIds(Pagination pagination, string dangerid);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, MeasuresEntity entity);

        void Save(MeasuresEntity entity);
        #endregion
    }
}
