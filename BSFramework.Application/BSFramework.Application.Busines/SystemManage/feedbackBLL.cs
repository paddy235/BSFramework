using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.SystemManage
{
    /// <summary>
    /// 描 述：意见反馈
    /// </summary>
    public class FeedbackBLL
    {
        private IFeedbackService service = new FeedbackService();

        #region 获取数据
        /// <summary>
        /// 意见反馈列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<FeedbackEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 意见反馈实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FeedbackEntity GetEntity(string keyValue)
        {
            FeedbackEntity newsEntity = service.GetEntity(keyValue);
            //newsEntity.NewsContent = WebHelper.HtmlDecode(newsEntity.NewsContent);
            return newsEntity;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除意见反馈
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
        /// 保存意见反馈表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="newsEntity">意见反馈实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, FeedbackEntity opinionEntity)
        {
            try
            {
                service.SaveForm(keyValue, opinionEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
