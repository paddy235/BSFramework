using BSFramework.Application.Entity.SystemManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.SystemManage
{
    /// <summary>
    /// 描 述：意见反馈
    /// </summary>
    public interface IFeedbackService
    {
        #region 获取数据
        /// <summary>
        /// 意见反馈列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<FeedbackEntity> GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 意见反馈实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        FeedbackEntity GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存意见反馈表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="opinionEntity">意见反馈实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, FeedbackEntity opinionEntity); 
        #endregion
    }
}
