using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    /// <summary>
    /// 描 述：电子公告
    /// </summary>
    public class NoticeBLL
    {
        private INoticeService service = new NoticeService();

        #region 获取数据
        /// <summary>
        /// 公告列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<NewsEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        public IEnumerable<NewsEntity> GetAllNotice(string deptid, DateTime? from, DateTime? to, string name,int page, int pagesize, out int total)
        {
            return service.GetAllNotice(deptid, from, to,name,  page, pagesize, out total);
        }
        /// <summary>
        /// 公告实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public NewsEntity GetEntity(string keyValue)
        {
            NewsEntity newsEntity = service.GetEntity(keyValue);
            return newsEntity;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公告
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
        /// 保存公告表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="newsEntity">公告实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, NewsEntity newsEntity)
        {
            try
            {
                //newsEntity.NewsContent = WebHelper.HtmlEncode(newsEntity.NewsContent);
                service.SaveForm(keyValue, newsEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NewsEntity> GetCurrentNotice(string deptId, DateTime date)
        {
            return service.GetCurrentNotice(deptId, date);
        }
        #endregion
    }
}
