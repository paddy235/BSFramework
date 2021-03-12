using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.IService.PublicInfoManage
{
    /// <summary>
    /// 描 述：电子公告
    /// </summary>
    public interface INoticeService
    {
        #region 获取数据
        /// <summary>
        /// 公告列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<NewsEntity> GetPageList(Pagination pagination, string queryJson);

        /// <summary>
        /// 公告实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        IEnumerable<NewsEntity> GetAllNotice(string deptid, DateTime? from, DateTime? to,string name, int page, int pagesize, out int total);

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存公告表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="newsEntity">公告实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, NewsEntity newsEntity);

        NewsEntity GetEntity(string keyValue);
        List<NewsEntity> GetCurrentNotice(string deptId, DateTime date);
        #endregion
    }
}
