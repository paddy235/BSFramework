using BSFramework.Application.Entity.MessageManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.MessageManage
{
    /// <summary>
    /// 描 述：即时通信群组管理
    /// </summary>
    public interface IMsgContentService
    {
        /// <summary>
        /// 获取消息列表（单对单）
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        IEnumerable<IMReadModel> GetList(Pagination pagination, string userId, string sendId,string flag = "1");
        DataTable GetHistoryList(string sendId, string toId);
        /// <summary>
        /// 获取消息列表（群组）
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        IEnumerable<IMReadModel> GetListByGroupId(Pagination pagination, string queryJson);
        /// <summary>
        /// 获取消息数量列表
        /// </summary>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        IEnumerable<IMReadNumModel> GetReadList(string userId);
         /// <summary>
        /// 获取某用户某种消息的总数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        DataTable GetReadAllNum(string userId, string status);
        
        #region 提交数据
        /// <summary>
        /// 增加一条消息内容
        /// </summary>
        /// <param name="entity"></param>
        void Add(IMContentEntity entity, DataTable dtGroupUserId);
        /// <summary>
        /// 更新消息状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sendId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int Update(string userId, string sendId, string status);
        #endregion
    }
}
