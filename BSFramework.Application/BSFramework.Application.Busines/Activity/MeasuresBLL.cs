using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    /// <summary>
    /// 描 述：安全预知训练
    /// </summary>
    public class MeasuresBLL
    {
        private MeasuresIService service = new MeasuresService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<MeasuresEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
        /// <summary>
        /// 根据危险预知训练Id获取数据列表
        /// </summary>
        /// <param name="dangerId">关联危险预知训练记录Id</param>
        /// <returns></returns>
        public List<MeasuresEntity> GetMeasureList(string dangerId)
        {
            return service.GetMeasureList(dangerId);
        }
          /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public MeasuresEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        public DataTable GetMeasuresById(Pagination pagination) 
        {
            return service.GetMeasuresById(pagination);
        }

        public List<MeasuresEntity> GetMeasuresByIds(Pagination pagination, string dangerid) {
            return service.GetMeasuresByIds(pagination, dangerid);

        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, MeasuresEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void Save(MeasuresEntity entity)
        {
            try
            {
                service.Save(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
